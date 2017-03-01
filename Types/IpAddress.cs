// This file is part of SNMP#NET.
// 
// SNMP#NET is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// SNMP#NET is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with SNMP#NET.  If not, see <http://www.gnu.org/licenses/>.
// 
using System;
using System.Net;


namespace SnmpSharpNet
{
	
	/// <summary>ASN.1 IPAddress type implementation</summary>
	/// <remarks>
	/// You can assign the IP address value to the class using:
	/// 
	/// A string value representing the IP address in dotted decimal notation:
	/// <code>
	/// IpAddress ipaddr = new IpAddress("10.1.1.1");
	/// </code>
	/// 
	/// A string value representing the hosts domain name:
	/// <code>
	/// IpAddress ipaddr = new IpAddress("this.ismyhost.com");
	/// </code>
	/// 
	/// Another IpAddress class:
	/// <code>
	/// IpAddress first = new IpAddress("10.1.1.2");
	/// IpAddress second = new IpAddress(first);
	/// </code>
	/// 
	/// Or from the System.Net.IPAddress:
	/// <code>
	/// IPAddress addr = IPAddress.Any;
	/// IpAddress ipaddr = new IpAddress(addr);
	/// </code>
	/// 
	/// You can check if the IpAddress class contains a valid value by calling:
	/// <code>
	/// IpAddress ipaddr = new IpAddress("10.1.1.3");
	/// if( ! ipaddr.Valid ) {
	///	  Console.WriteLine("Invalid IP Address value.");
	///	  return;
	///	}
	/// </code>
	/// 
	/// There are other operations you can perform with the IpAddress class. For example, let say
	/// you retrieved an IP address and subnet mask from an SNMP agent and wish to scan the subnet for
	/// other hosts that can be managed. You could do this:
	/// IpAddress host = SomehowRetrieveIPAddress();
	/// IpAddress mask = SomehowRetrieveSubnetMask();
	/// IpAddress subnetAddr = host.GetSubnetAddress(mask);
	/// IpAddress broadcastAddr = host.GetBroadcastAddress(mask);
	/// IpAddress host = (IpAddress)subnetAddr.Clone();
	/// while( host.CompareTo(broadcastAddr) != 0 ) {
	///   host = host.Increment(1); // increment IP address by one
	///   if( ! host.Equals(broadcastAddr) )
	///     ScanHostInWhateverWayYouLike(host);
	/// }
	/// 
	/// Note: internally, IpAddress class holds the value in a byte array, network ordered format.
	/// </remarks>
	[Serializable]
	public class IpAddress : OctetString, IComparable, ICloneable
	{
		/// <summary> Constructs a default object with a 
		/// length of zero. See the super class
		/// constructor for more details.
		/// </summary>
		public IpAddress()
		{
			_asnType = SnmpConstants.SMI_IPADDRESS;
			_data = new byte[]{0, 0, 0, 0};
		}
		
		/// <summary> Constructs an Application String with the
		/// passed data. The data is managed by the 
		/// base class.
		/// </summary>
		/// <param name="data">The application string to manage (UTF-8)
		/// </param>
		public IpAddress(byte[] data):this()
		{
			_asnType = SnmpConstants.SMI_IPADDRESS;
			if (data.Length != 4)
				throw new OverflowException("Too much data passed to constructor: " + data.Length.ToString());
			Set(data);
		}
		
		/// <summary> Copy constructor. Constructs a duplicate object 
		/// based on the passed application string object.
		/// </summary>
		/// <param name="second">The object to copy.</param>
		/// <exception cref="ArgumentException">IpAddress argument is null</exception>
		public IpAddress(IpAddress second):this()
		{
			if( second == null )
				throw new ArgumentException("Constructor argument cannot be null.");

			if (!second.Valid)
			{
				_data = new byte[] { 0, 0, 0, 0 };
			}
			else
			{
				Set(second.GetData());
			}
		}
		
		/// <summary>Copy constructor.</summary>
		/// <param name="second">The object to copy</param>
		public IpAddress(OctetString second):this(second.GetData())
		{
		}
		
		/// <summary>Constructor</summary>
		/// <remarks>Construct the IpAddress class from supplied IPAddress value.</remarks>
		/// <param name="inetAddr">IP Address to use to initialize the object.</param>
		public IpAddress(System.Net.IPAddress inetAddr):this(inetAddr.GetAddressBytes())
		{
			_asnType = SnmpConstants.SMI_IPADDRESS;
		}
		
		/// <summary>Constructs the class and initializes the value to the supplied IP address. See comments
		/// in <see cref="IpAddress.Set(string)"/> method for details.
		/// </summary>
		/// <param name="inetAddr">IP address encoded as a string or host name</param>
		public IpAddress(string inetAddr):this()
		{
			Set(inetAddr);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <remarks>Initialize IpAddress class with the IP address value represented as 32-bit unsigned integer
		/// value</remarks>
		/// <param name="inetAddr">32-bit unsigned integer representation of an IP Address</param>
		public IpAddress(UInt32 inetAddr):this()
		{
			Set(inetAddr);
		}
				
		/// <summary>Clone current object.</summary>
		/// <returns> Cloned IpAddress object.</returns>
		public override System.Object Clone()
		{
			return (Object)new IpAddress(this);
		}

		/// <summary>Sets the class value to the IP address parsed from the string parameter.</summary>
		/// <remarks>Class value will be set with the parsed dotted decimal IP address in the parameter or
		/// if string parameter does not represent an IP address, DNS resolution
		/// will be performed.
		/// 
		/// Note: DNS resolution is performed using <see cref="System.Net.Dns.GetHostEntry(string)"/> and can result
		/// in a longer then expected delay in this function. In applications where responsiveness is
		/// important, name resolution should be performed using async methods available and result passed
		/// to this method.
		/// </remarks>
		/// <param name="value">IP address encoded as a string or host name</param>
		/// <exception cref="ArgumentException">Thrown when DNS resolution of the parameter value has failed.</exception>
		public override void Set(String value)
		{
			IPAddress ipa;
			if (!IPAddress.TryParse(value, out ipa))
			{
				try
				{
					IPHostEntry entry = Dns.GetHostEntry(value);
					// have to loop through all returned addresses to make sure we pick IPv4 address
					foreach (IPAddress addr in entry.AddressList)
					{
						if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
						{
							_data = addr.GetAddressBytes();
							break;
						}
					}
				}
				catch
				{
					throw new ArgumentException("Unable to parse or resolve supplied value to an IP address.","value");
				}
			}
			else
			{
				_data = ipa.GetAddressBytes();
			}
		}

		/// <summary>
		/// Set class value from 32-bit unsigned integer value representation of the IP address value
		/// </summary>
		/// <param name="ipvalue">IP address value as 32-bit unsigned integer value</param>
		public void Set(UInt32 ipvalue)
		{
			_data = new byte[4];
			_data[0] = (byte)(ipvalue);
			_data[1] = (byte)(ipvalue >> 8);
			_data[2] = (byte)(ipvalue >> 16);
			_data[3] = (byte)(ipvalue >> 24);
		}

		/// <summary>
		/// Set class value from the IPAddress argument
		/// </summary>
		/// <param name="ipaddr">Type: <see cref="System.Net.IPAddress"/> class instance</param>
		public void Set(IPAddress ipaddr)
		{
			base.Set(ipaddr.GetAddressBytes());
		}
		
		/// <summary>Allow explicit cast of the class as System.Net.IPAddress class.</summary>
		/// <param name="ipaddr">IpAddress class</param>
		/// <returns>IpAddress class value as System.Net.IPAddress class</returns>
		public static explicit operator IPAddress(IpAddress ipaddr)
		{
			if (ipaddr.Length != 4)
			{
				return IPAddress.Any;
			}
			return new IPAddress(ipaddr.GetData());
		}
		
		/// <summary> Returns the application string as a dotted decimal represented IP address.</summary>
		/// <returns>String representation of the class value.</returns>
		public override System.String ToString()
		{
			if (_data == null)
				return "";
			byte[] data = _data;
			
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			buf.Append((int) (data[0] < 0?256 + data[0]:data[0])).Append('.');
			buf.Append((int) (data[1] < 0?256 + data[1]:data[1])).Append('.');
			buf.Append((int) (data[2] < 0?256 + data[2]:data[2])).Append('.');
			buf.Append((int) (data[3] < 0?256 + data[3]:data[3]));
			
			return buf.ToString();
		}

		/// <summary>
		/// Compare value against IPAddress, byte array, UInt32, IpAddress of OctetString class value.
		/// </summary>
		/// <param name="obj">Type: <see cref="System.Net.IPAddress"/> or byte <see cref="Array"/> or <see cref="UInt32"/> or <see cref="IpAddress"/> or <see cref="OctetString"/></param>
		/// <returns>0 if class values are the same, -1 if current class value is less then or 1 if greater then the class value
		/// we are comparing against.</returns>
		public int CompareTo(object obj)
		{
			byte[] b = null;
			if (obj == null)
				return -1;
			if (obj is IPAddress)
			{
				IPAddress ipa = (IPAddress)obj;
				b = ipa.GetAddressBytes();
			}
			else if (obj is byte[])
			{
				b = (byte[])obj;
			}
			else if (obj is UInt32)
			{
				IpAddress ipa = new IpAddress((UInt32)obj);
				b = ipa.ToArray();
			}
			else if (obj is IpAddress)
			{
				b = ((IpAddress)obj).ToArray();
			}
			else if (obj is OctetString)
			{
				b = ((OctetString)obj).ToArray();
			}
			if (_data == null)
				return -1;
			if (b.Length != _data.Length)
			{
				if (_data.Length < b.Length)
					return -1;
				else
					return 1;
			}
			else
			{
				for (int i = 0; i < _data.Length; i++)
				{
					if (_data[i] < b[i])
					{
						return -1;
					}
					else if (_data[i] > b[i])
					{
						return 1;
					}
				}
				return 0;
			}
		}
		/// <summary>
		/// Compare 2 IpAddress objects.
		/// </summary>
		/// <param name="obj"><see cref="IpAddress"/> object to compare against</param>
		/// <returns>True if objects are the same, otherwise false.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			return ((this.CompareTo(obj) == 0) ? true : false);
		}
		/// <summary>
		/// Return hash representing the value of this object
		/// </summary>
		/// <returns>Integer hash representing the object</returns>
		public override int GetHashCode()
		{
			if (_data == null || _data.Length != 4)
				return 0;
			int hash = Convert.ToInt32(_data[0]) + Convert.ToInt32(_data[1]) + Convert.ToInt32(_data[2]) + Convert.ToInt32(_data[3]);
			return hash;
		}
		/// <summary>
		/// Returns true if object contains a valid IP address value.
		/// </summary>
		public bool Valid
		{
			get
			{
				if (_data != null)
				{
					if (_data.Length == 4)
					{
						if (_data[0] != 0x00 || _data[1] != 0x00 || _data[2] != 0x00 || _data[3] != 0x00)
							return true;
					}
				}
				return false;
			}
		}

		/// <summary>Decode ASN.1 encoded IP address value.</summary>
		/// <param name="buffer">BER encoded data buffer</param>
		/// <param name="offset">Offset in the array to start parsing from</param>
		/// <returns>Buffer position after the decoded value.</returns>
		/// <exception cref="OverflowException">Parsed data length is not 4 bytes long</exception>
		/// <exception cref="SnmpException">Parsed data is not in IpAddress format</exception>
		public override int decode(byte[] buffer, int offset)
		{
			offset = base.decode(buffer, offset);
			if (_data.Length != 4)
			{
				_data = null;
				throw new OverflowException("ASN.1 decoding error. Invalid data length.");
			}
			return offset;
		}

		#region Helper functions

		/// <summary>
		/// Class A IP address
		/// </summary>
		public static int ClassA = 1;
		/// <summary>
		/// Class B IP address
		/// </summary>
		public static int ClassB = 2;
		/// <summary>
		/// Class C IP address
		/// </summary>
		public static int ClassC = 3;
		/// <summary>
		/// Class D IP address
		/// </summary>
		public static int ClassD = 4;
		/// <summary>
		/// Class E IP address
		/// </summary>
		public static int ClassE = 5;
		/// <summary>
		/// Invalid IP address class
		/// </summary>
		public static int InvalidClass = 0;

		/// <summary>
		/// Return network class of the IP address
		/// </summary>
		/// <returns>Integer network class. Return values are <see cref="ClassA"/>, <see cref="ClassB"/>, <see cref="ClassC"/>,
		/// <see cref="ClassD"/>, <see cref="ClassE"/>, or <see cref="InvalidClass"/> on error.</returns>
		public int GetClass()
		{
			byte octet = _data[0];
			if ((octet & 0x80) == 0)
			{
				return ClassA; // Class A
			}
			else if ((octet & 0x80) != 0 && (octet & 0x40) == 0)
			{
				return ClassB; // Class B
			}
			else if ((octet & 0x80) != 0 && (octet & 0x40) != 0 && (octet & 0x20) == 0)
			{
				return ClassC; // Class C
			}
			else if ((octet & 0x80) != 0 && (octet & 0x40) != 0 && (octet & 0x20) != 0 && (octet & 0x10) == 0)
			{
				return ClassD;
			}
			else if ((octet & 0x80) != 0 && (octet & 0x40) != 0 && (octet & 0x20) != 0 && (octet & 0x10) != 0)
			{
				return ClassE;
			}
			else
			{
				return InvalidClass; // Other class
			}
		}

		/// <summary>
		/// Network byte order IP address
		/// </summary>
		/// <remarks>
		/// Convert internal byte array representation of the IP address into a network byte order 
		/// (most significant byte first) unsigned integer value.
		/// 
		/// To use the returned value in mathematical operations, you will need to change it to
		/// host byte order on little endian systems (all i386 compatible systems). To do that,
		/// call static method IpAddress.ReverseByteOrder().
		/// </remarks>
		/// <returns>Unsigned integer value representing the IP address in network byte order</returns>
		public UInt32 ToUInt32()
		{
			UInt32 ip = (uint)_data[3] << 24;
			ip += (uint)_data[2] << 16;
			ip += (uint)_data[1] << 8;
			ip += (uint)_data[0];
			return ip;
		}
		/// <summary>
		/// Return subnet address of the IP address in this object and supplied subnet mask
		/// </summary>
		/// <param name="mask">Subnet mask</param>
		/// <returns>New IpAddress object containing subnet address</returns>
		public IpAddress GetSubnetAddress(IpAddress mask)
		{
			byte[] ip = _data;
			byte[] m = mask.ToArray();
			byte[] res = new byte[4];
			for (int i = 0; i < 4; i++)
			{
				res[i] = (byte)(ip[i] & m[i]);
			}
			return new IpAddress(res);
		}
		/// <summary>
		/// Inverts IP address value. All 0s are converted to 1s and 1s to 0s
		/// </summary>
		/// <example>
		/// <code lang="cs">
		/// IpAddress testMask = new IpAddress("255.255.0.0");
		/// IpAddress invMask = testMask.Invert();
		/// Console.WriteLine("original: {0}", testMask.ToString());
		/// // Output: original: 255.255.0.0
		/// Console.WriteLine("inverted: {0}", invMask.ToString());
		/// // Output: inverted: 0.0.255.255
		/// </code>
		/// </example>
		/// <returns>Inverted value of the IP address</returns>
		public IpAddress Invert()
		{
			byte[] bitmask = new byte[] { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };
			byte[] ip = _data;
			byte[] iip = new byte[4] { 0, 0, 0, 0 };
			// Walk through each byte
			for (int b = 0; b < 4; b++)
			{
				// Check each bit
				for (int i = 0; i < bitmask.Length; i++)
				{
					if ((ip[b] & bitmask[i]) == 0)
					{
						iip[b] |= bitmask[i];
					}
				}
			}
			return new IpAddress(iip);
		}
		/// <summary>
		/// Returns broadcast address for the objects IP and supplied subnet mask.
		/// 
		/// Subnet mask validity is not confirmed before performing the new value calculation so
		/// you should make sure parameter is a valid subnet mask by using <see cref="IpAddress.IsValidMask"/>
		/// method.
		/// 
		/// Broadcast address is calculated by changing every bit in the subnet mask that is set to
		/// value 0 into 1 in the address value.
		/// </summary>
		/// <param name="mask">Subnet mask to apply to the object</param>
		/// <returns>New IP address containing broadcast address</returns>
		public IpAddress GetBroadcastAddress(IpAddress mask)
		{
			IpAddress im = mask.Invert();
			byte[] ip = _data;
			byte[] m = im.GetData();
			byte[] res = new byte[4];
			for (int i = 0; i < 4; i++)
			{
				res[i] = (byte)(ip[i] | m[i]);
			}
			return new IpAddress(res);
		}
		/// <summary>
		/// Returns network mask for the class value
		/// </summary>
		/// <remarks>Each IpAddress belongs to one of the 5 classes, A, B, C, D or E. D network class are multicast
		/// addresses and E is experimental class. Because D and E are not "real" network classes and as such don't
		/// have a network mask, only classes A, B and C will return a valid subnet mask. Classes D and E will return
		/// a null reference.</remarks>
		/// <returns>Network mask or null if address doesn't belong to classes A, B or C</returns>
		public IpAddress NetworkMask()
		{
			int cl = this.GetClass();
			switch (cl)
			{
				case 1:
					return new IpAddress(new byte[] { 255, 0, 0, 0 });
				case 2:
					return new IpAddress(new byte[] { 255, 255, 0, 0 });
				case 3:
					return new IpAddress(new byte[] { 255, 255, 255, 0 });
				default:
					return null;
			}
		}
		/// <summary>
		/// Checks if the value of the object is a valid subnet mask.
		/// 
		/// Subnet mask is a value of consecutive bits with value of 1. If a bit is found with a value of 0
		/// followed (immediately or anywhere else before the end of the value) by a 1, then subnet
		/// mask value is not valid.
		/// </summary>
		/// <returns>true if valid subnet mask, otherwise false</returns>
		public bool IsValidMask()
		{
			byte[] bitmask = new byte[] { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };
			byte[] ip = _data;
			bool endMask = false;
			for (int i = 0; i < 4; i++)
			{
				for (int b = 0; b < bitmask.Length; b++)
				{
					if ((ip[i] & bitmask[b]) == 0)
					{
						if (!endMask)
						{
							endMask = true;
						}
					}
					if ((ip[i] & bitmask[b]) != 0 && endMask)
					{
						return false; // Invalid mask. bit set to 1 after 0s were found
					}
				}
			}
			return true; // Mask is ok
		}
		/// <summary>
		/// Returns number of subnet bits in the mask.
		/// 
		/// Subnet bits are bits set to value 0 in the subnet mask.
		/// </summary>
		/// <example>
		///		<code lang="cs">
		///		IpAddress mask = new IpAddress("255.255.255.0");
		///		Console.WriteLine("Mask bits: {0}", mask.GetMaskBits()); # Print "Mask bits: 24"
		///		mask.Set("255.255.255.128");
		///		Console.WriteLine("Mask bits: {0}", mask.GetMaskBits()); # Print "Mask bits: 25"
		///		</code>
		/// </example>
		/// <returns>Set bits in the subnet mask or 0 if invalid subnet mask</returns>
		public int GetMaskBits()
		{
			if (!this.IsValidMask())
			{
				return 0; // Invalid mask
			}
			byte[] bitmask = new byte[] { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };
			byte[] ip = _data;
			int bitcount = 0;
			for (int i = 0; i < 4; i++)
			{
				for (int b = 0; b < bitmask.Length; b++)
				{
					if ((ip[i] & bitmask[b]) != 0)
					{
						bitcount += 1;
					}
				}
			}
			return bitcount;
		}
		/// <summary>
		/// Build a subnet mask from bit count value
		/// </summary>
		/// <example>This example shows how to use IpAddress.BuildMaskFromBits() method:
		/// <code lang="cs">
		/// Console.WriteLine("mask 1: {0}", IpAddress.BuildMaskFromBits(23));
		/// Console.WriteLine("mask 2: {0}", IpAddress.BuildMaskFromBits(24));
		/// Console.WriteLine("mask 3: {0}", IpAddress.BuildMaskFromBits(25));
		/// <i>Result:</i>
		/// mask 1: 255.255.254.0
		/// mask 2: 255.255.255.0
		/// mask 3: 255.255.255.128
		/// </code>
		/// </example>
		/// <param name="bits">Number of subnet mask bits</param>
		/// <returns>New IPAddress class containing the subnet mask</returns>
		public static IpAddress BuildMaskFromBits(int bits)
		{
			byte[] bitmask = new byte[] { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };
			byte[] res = new byte[] { 0, 0, 0, 0 };
			int bcount = 0;
			for (int i = 0; i <= 3; i++)
			{
				for (int b = 0; b < bitmask.Length; b++)
				{
					if (bcount < bits)
					{
						res[i] |= bitmask[b];
						++bcount;
					}
				}
			}
			return new IpAddress(res);
		}
		/// <summary>
		/// Reverse int value byte order. Static function in IPAddress class doesn't work 
		/// the way I expected it (didn't troubleshoot) so here is another implementation.
		/// 
		/// Reversing byte order is needed because network encoded IP address is presented in big-endian
		/// format. Intel based computers encode values in little-endian form so to perform numerical operations
		/// on received IP address values you will need to convert them from big to little-endian format.
		/// 
		/// By the same token, if you wish to transmit data using IP address value calculated on the
		/// local system, you will need to convert it to big-endian prior to transmission.
		/// </summary>
		/// <example>
		/// Here is an example that demonstrates what this method does:
		/// <code lang="cs">
		/// IpAddress test = new IpAddress(new byte[] { 10, 20, 30, 40 }); // Example IP: 10.20.30.40
		/// UInt32 testValue = test.ToUInt32();
		/// UInt32 reverseValue = IpAddress.ReverseByteOrder(testValue);
		/// IpAddress newTest = new IpAddress(reverseValue);
		/// Console.WriteLine("New IP: {0}", newTest.ToString()); // Output "New IP: 40.30.20.10"		/// </code>
		/// </example>
		/// <param name="val">Integer value to reverse</param>
		/// <returns>Reversed integer value</returns>
		public static uint ReverseByteOrder(uint val)
		{
			byte[] v = BitConverter.GetBytes(val);
			byte[] r = new byte[4];
			r[0] = v[3];
			r[1] = v[2];
			r[2] = v[1];
			r[3] = v[0];
			return BitConverter.ToUInt32(r, 0);
		}
		/// <summary>
		/// Increment IP address contained in the object by specific number and return the result as a new
		/// class.
		/// </summary>
		/// <example>
		/// <code lang="cs">
		/// IpAddress addr = new IpAddress("10.20.11.2");
		/// Console.WriteLine("Starting IP: {0}", addr.ToString());
		/// // Output: Starting IP: 10.20.11.2
		/// IpAddress incrAddr1 = addr.Increment(1);
		/// Console.WriteLine("Incremented by 1: {0}", incrAddr1.ToString());
		/// // Output: Incremented by 1: 10.20.11.3
		/// IpAddress incrAddr10 = addr.Increment(10);
		/// Console.WriteLine("Incremented by 10: {0}", incrAddr10.ToString());
		/// // Output: Incremented by 10: 10.20.11.12
		/// </code>
		/// </example>
		/// <param name="count">Number to increment IP address with</param>
		/// <returns>New class representing incremented IP address</returns>
		public IpAddress Increment(UInt32 count)
		{
			uint ip = ToUInt32();
			uint revip = ReverseByteOrder(ip);
			revip += count;
			return new IpAddress(ReverseByteOrder(revip));
		}

		/// <summary>
		/// Check if supplied string contains a valid IP address.
		/// 
		/// Valid IP address contains 3 full stops (".") and 4 numeric values in range 0 to 255.
		/// </summary>
		/// <remarks>
		/// Written to address slow performance of IPAddress.TryParse
		/// </remarks>
		/// <param name="val">String value</param>
		/// <returns>true if string contains an IP address in dotted decimal format, otherwise false</returns>
		public static bool IsIP(string val)
		{
			if (val.Length == 0 || val.Length < 7 || val.Length > 15)
				return false;
			bool validChar = true;
			int dotCount = 0;
			foreach (char c in val)
			{
				if (!Char.IsDigit(c) && c != '.')
				{
					validChar = false;
					break;
				}
				else if (c == '.')
					dotCount += 1;
			}
			if (!validChar)
				return false;
			if (dotCount != 3)
				return false;
			string[] ar = val.Split('.');
			if (ar.Length != 4)
				return false;
			for (int i = 0; i < 4; i++)
			{
				int v = Convert.ToInt32(ar[i]);
				if (v < 0 || v > 255)
					return false;
			}
			return true;
		}

		#endregion Helper functions
	}
}
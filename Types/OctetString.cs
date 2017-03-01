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
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace SnmpSharpNet
{
	
	/// <summary>ASN.1 OctetString type implementation</summary>
	[Serializable]
	public class OctetString : AsnType, ICloneable, IComparable<byte[]>, IComparable<OctetString>, IEnumerable<byte>
	{
		/// <summary>Data buffer</summary>
		protected byte[] _data;
		
		/// <summary>Constructor</summary>
		public OctetString()
		{
			_asnType = SnmpConstants.SMI_STRING;
		}

		/// <summary> Constructs an Octet String with the contents of the supplied string.
		/// </summary>
		/// <param name="data">String data to convert into OctetString class value.</param>
		public OctetString(String data):this()
		{
			Set(data);
		}
		
		/// <summary>Constructs the object and sets the data buffer to the byte array values
		/// </summary>
		/// <param name="data">Byte array to copy into the data buffer</param>
		public OctetString(byte[] data):this()
		{
			Set(data);
		}

		/// <summary>
		/// Construct the class and set value. Value can be set by reference (assigned passed array parameter
		/// to the interal class variable) or by value (copy data in the array into a new buffer).
		/// </summary>
		/// <param name="data">Byte array to set class value to</param>
		/// <param name="useReference">If true, set class value to reference byte array parameter, otherwise copy data into new internal byte array</param>
		public OctetString(byte[] data, bool useReference):this()
		{
			if (useReference)
				SetRef(data);
			else
				Set(data);
		}
		
		/// <summary>Constructor creating class from values in the supplied class.</summary>
		/// <param name="second">OctetString object to copy data from.</param>
		public OctetString(OctetString second):this()
		{
			Set(second);
		}

		/// <summary>
		/// Constructor. Initialize the class value to a 1 byte array with the supplied value
		/// </summary>
		/// <param name="data">Value to initialize the class data to.</param>
		public OctetString(byte data):this()
		{
			Set(data);
		}

		/// <summary>Get length of the internal byte array. 0 if byte array is undefined or zero length.</summary>
		virtual public int Length
		{
			get
			{
				int len = 0;
				if (_data != null)
					len = _data.Length;
				return len;
			}

		}
		/// <summary>
		/// Internal method to return OctetString byte array. Used for copy operations, comparisons and similar within the
		/// library. Not available for users of the library
		/// </summary>
		/// <returns></returns>
		internal byte[] GetData()
		{
			return _data;
		}

		/// <summary>
		/// Empty data buffer
		/// </summary>
		public void Clear()
		{
			_data = null;
		}

		/// <summary>
		/// Convert the OctetString class to a byte array. Internal class data buffer is *copied* and not passed to the caller.
		/// </summary>
		/// <returns>Byte array representing the OctetString class data</returns>
		public byte[] ToArray()
		{
			if (_data == null)
			{
				return null;
			}
			byte[] tmp = new byte[_data.Length];
			Buffer.BlockCopy(_data, 0, tmp, 0, _data.Length);
			return tmp;
		}
		/// <summary>Set object value to bytes from the supplied string. If argument string length == 0, internal OctetString
		/// buffer is set to null.</summary>
		/// <param name="value">String containing new class data</param>
		public virtual void Set(String value)
		{
			if (value == null)
			{
				_data = null;
			}
			if (value.Length == 0)
			{
				_data = null;
			}
			else
			{
				_data = System.Text.UTF8Encoding.UTF8.GetBytes(value);
			}
		}
		/// <summary>
		/// Set class value from the argument byte array. If byte array argument is null or length == 0,
		/// internal <see cref="OctetString"/> buffer is set to null.
		/// </summary>
		/// <param name="data">Byte array to copy data from.</param>
		public virtual void Set(byte[] data)
		{
			if (data == null || data.Length <= 0)
			{
				_data = null;
			}
			else
			{
				_data = new byte[data.Length];
				Buffer.BlockCopy(data, 0, _data, 0, data.Length);
			}
		}
		/// <summary>
		/// Set class value to an array 1 byte long and set the value to the supplied argument.
		/// </summary>
		/// <param name="data">Byte value to initialize the class value with</param>
		public virtual void Set(byte data)
		{
			_data = new byte[1];
			_data[0] = data;
		}
		/// <summary>
		/// Set value at specified position to the supplied value
		/// </summary>
		/// <param name="position">Zero based offset from the beginning of the buffer</param>
		/// <param name="value">Value to set</param>
		public virtual void Set(int position, byte value)
		{
			if (position < 0 || position >= Length)
			{
				return; // Don't throw exceptions here
			}
			_data[position] = value;
		}
		/// <summary>
		/// Set class value to reference of parameter byte array
		/// </summary>
		/// <param name="data">Data buffer parameter</param>
		public virtual void SetRef(byte[] data)
		{
			_data = data;
		}
		/// <summary>
		/// Append string value to the OctetString class. If current class content is length 0, new
		/// string value is set as the value of this class.
		/// 
		/// Class assumes that string value is UTF8 encoded.
		/// </summary>
		/// <param name="value">UTF8 encoded string value</param>
		public void Append(string value)
		{
			if (_data == null)
			{
				Set(value);
			}
			else
			{
				if (value.Length > 0)
				{
					byte[] buffer = System.Text.UTF8Encoding.UTF8.GetBytes(value);
					if (buffer != null && buffer.Length > 0)
					{
						Append(buffer);
					}
				}
			}
		}
		/// <summary>
		/// Append contents of the byte array to the class value. If class value is length 0, byte array
		/// content is set as the class value.
		/// </summary>
		/// <param name="value">Byte array</param>
		public void Append(byte[] value)
		{
			if (value == null || value.Length == 0)
				throw new ArgumentNullException("value");
			if (_data == null)
			{
				Set(value);
			}
			else
			{
				byte[] tempBuffer = new byte[_data.Length + value.Length];
				Buffer.BlockCopy(_data, 0, tempBuffer, 0, _data.Length);
				Buffer.BlockCopy(value, 0, tempBuffer, _data.Length, value.Length);
				_data = tempBuffer;
			}
		}
		/// <summary>
		/// Indexed access to the OctetString class data members.
		/// <code>
		/// OctetString os = new OctetString("test");
		/// for(int i = 0;i &lt; os.Length;i++) {
		///  Console.WriteLine("{0}",os[i]);
		/// }
		/// </code>
		/// </summary>
		/// <param name="index">Index position of the data value to access</param>
		/// <returns>Byte value at the index position. 0 if index is out of range</returns>
		public byte this[int index]
		{
			get
			{
				if (index < 0 || index >= Length)
				{
					return 0; // Don't throw exceptions here
				}
				return _data[index];
			}
			set
			{
				if (index < 0 || index >= Length)
				{
					return; // Don't throw exceptions here
				}
				_data[index] = value;
			}
		}
		
		/// <summary>Creates a duplicate copy of the object and returns it to the caller.</summary>
		/// <returns> A newly constructed copy of self</returns>
		public override Object Clone()
		{
			return new OctetString(this);
		}

		/// <summary>
		/// Return true if OctetString contains non-printable characters, otherwise return false.
		/// </summary>
		/// <remarks>Values recognized as hex are byte values less then decimal 32 that are not decimal
		/// 10 or 13 and byte values that are greater then 127 decimal. One exception is byte value 0x00
		/// when it is at the end of the byte array is not considered a hex value but a c-like string
		/// termination character.</remarks>
		public bool IsHex
		{
			get
			{
				if (_data == null || _data.Length < 0)
					return false; // empty string can't be hex :)
				bool isHex = false;
				for(int i=0;i<_data.Length;i++) 
				{
					byte b = _data[i];
					if ( b < 32 ) {
						if (b != 10 && b != 13 && !(b == 0x00 && (_data.Length - 1) == i))
						{
							isHex = true;
						}
					}
					else if (b > 127)
					{
						isHex = true;
					}

				}
				return isHex;
			}
		}
		
		/// <summary>Utility function to print a MAC address (binary string of 6 byte length.</summary>
		/// <returns>If data is of the correct length (6 bytes), string representing hex mac address in the
		/// format xxxx.xxxx.xxxx. If data is not of the correct length, empty string is returned.</returns>
		public System.String ToMACAddressString()
		{
			if( Length == 6 ) {
				return string.Format(CultureInfo.CurrentCulture, "{0:x2}{1:x2}.{2:x2}{3:x2}.{4:x2}{5:x2}",
					_data[0],_data[1],_data[2],_data[3],_data[4],_data[5]);
			}
			return "";
		}
		/// <summary>Return string representation of the OctetStrig object. If non-printable characters have been
		/// found in the object, output is a hex representation of the string.
		/// </summary>
		/// <returns>String representation of the object.</returns>
		public override String ToString()
		{
			if (_data == null || _data.Length <= 0)
			{
				return "";
			}
			bool asHex = IsHex;
			
			System.String rs = null;
			if (asHex)
			{
				rs = ToHexString();
			}
			else
			{
				rs = new String(UTF8Encoding.UTF8.GetChars(_data));
			}
			return rs;
		}

		/// <summary>
		/// Return string formatted hexadecimal representation of the objects value.
		/// </summary>
		/// <returns>String representation of hexadecimal formatted class value.</returns>
		public string ToHexString()
		{
			StringBuilder b = new StringBuilder();
			for (int i = 0; i < _data.Length; ++i)
			{
				int x = (int)_data[i] & 0xff;
				if (x < 16)
					b.Append('0');
				b.Append(System.Convert.ToString(x, 16).ToUpper());

				if (i < _data.Length - 1)
					b.Append(' ');
			}
			return b.ToString();
		}
		/// <summary>
		/// Compare against another object. Acceptable object types are <see cref="OctetString"/> and
		/// <see cref="System.String"/>.
		/// </summary>
		/// <param name="obj">Object of type <see cref="OctetString"/> or <see cref="System.String"/> to compare against</param>
		/// <returns>true if object content is the same, false if different or if incompatible object type</returns>
		public override bool Equals(object obj)
		{
			byte[] d = null;
			if( obj is OctetString ) {
				OctetString o = obj as OctetString;
				d = o.GetData();
			} else if( obj is System.String ) {
				d = System.Text.UTF8Encoding.UTF8.GetBytes((String)obj);
			} else {
				return false; // Incompatible object type
			}
			// check for null value in comparison
			if (d == null || _data == null)
			{
				if (d == null && _data == null)
					return true; // both values are null
				return false; // one value is not null
			}
			if( d.Length != _data.Length ) {
				return false; // Objects have different length
			}
			for(int cnt=0;cnt<d.Length;cnt++) {
				if( d[cnt] != _data[cnt] ) {
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// Dummy override to prevent compiler warning messages.
		/// </summary>
		/// <returns>Nothing of interest</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		/// <summary>
		/// Overloading equality operator
		/// </summary>
		/// <param name="str1">Source (this) string</param>
		/// <param name="str2">String to compare with</param>
		/// <returns>True if equal, otherwise false</returns>
		public static bool operator ==(OctetString str1, OctetString str2)
		{
			if (((Object)str1) == null && ((Object)str2) == null)
				return true;
			if (((Object)str1) == null || ((Object)str2) == null)
				return false;
			return str1.Equals(str2);
		}

		/// <summary>
		/// Negative equality operator
		/// </summary>
		/// <param name="str1">Source (this) string</param>
		/// <param name="str2">String to compare with</param>
		/// <returns>True if not equal, otherwise false</returns>
		public static bool operator !=(OctetString str1, OctetString str2)
		{
			return !(str1 == str2);
		}
		/// <summary>
		/// IComparable interface implementation. Compare class contents with contents of the byte array.
		/// </summary>
		/// <param name="other">Byte array to compare against</param>
		/// <returns>-1 if class value is greater (longer or higher value), 1 if byte array is greater or 0 if the same</returns>
		public int CompareTo(byte[] other)
		{
			if (_data == null && (other != null && other.Length > 0))
				return 1;
			if ((other == null || other.Length == 0) && _data.Length > 0)
				return -1;
			if (_data == null && other == null)
				return 0;
			if (_data.Length > other.Length)
				return -1;
			else if (_data.Length < other.Length)
				return 1;
			else
			{
				for (int i = 0; i < _data.Length; i++)
				{
					if (_data[i] > other[i])
						return -1;
					else if (_data[i] < other[i])
						return 1;
				}
				return 0;
			}
		}
		/// <summary>
		/// IComparable interface implementation. Compare class contents against another class.
		/// </summary>
		/// <param name="other">OctetString class to compare against.</param>
		/// <returns>-1 if class value is greater (longer or higher value), 1 if byte array is greater or 0 if the same</returns>
		public int CompareTo(OctetString other)
		{
			return CompareTo(other.GetData());
		}
		/// <summary>
		/// Implicit operator allowing cast of OctetString objects to byte[] array
		/// </summary>
		/// <param name="oStr">OctetString to cast as byte array</param>
		/// <returns>Byte array value of the supplied OctetString</returns>
		public static implicit operator byte[](OctetString oStr)
		{
			if (oStr == null)
				return null;
			return oStr.ToArray();
		}

		/// <summary>
		/// Reset internal buffer to null.
		/// </summary>
		public void Reset()
		{
			_data = null;
		}

		#region Encode and decode methods

		/// <summary>BER encode OctetString variable.</summary>
		/// <param name="buffer"><see cref="MutableByte"/> encoding destination.</param>
		public override void encode(MutableByte buffer)
		{
			if (_data == null || _data.Length == 0)
				BuildHeader(buffer, Type, 0);
			else
			{
				BuildHeader(buffer, Type, _data.Length);
				buffer.Append(_data);
			}
		}

		/// <summary>
		/// Decode OctetString from the BER format. 
		/// </summary>
		/// <param name="buffer">BER encoded buffer</param>
		/// <param name="offset">Offset in the <see cref="MutableByte"/> to start the decoding from</param>
		/// <returns>Buffer position after the decoded value</returns>
		/// <exception cref="SnmpException">Thrown if parsed data type is invalid.</exception>
		public override int decode(byte[] buffer, int offset)
		{
			int headerLength;
			byte asnType = ParseHeader(buffer, ref offset, out headerLength);

			if (asnType != Type)
				throw new SnmpException("Invalid ASN.1 type.");

			// verify that there is enough data to decode
			if ((buffer.Length - offset) < headerLength)
				throw new OverflowException("Data buffer is too small");

			if (headerLength == 0)
			{
				// Packet contains string length == 0
				_data = null;
			}
			else
			{
				//
				// copy the data
				//
				_data = new byte[headerLength];
				Buffer.BlockCopy(buffer, offset, _data, 0, headerLength);
				offset += headerLength;
			}
			return offset;
		}

		#endregion Encode and decode methods

		/// <summary>
		/// Returns an enumerator that iterates through the OctetString byte collection
		/// </summary>
		/// <returns>An IEnumerator  object that can be used to iterate through the collection.</returns>
		public IEnumerator<byte> GetEnumerator()
		{
			if (_data != null)
				return ((IEnumerable<byte>)_data).GetEnumerator();
			return null;
		}

		/// <summary>
		/// Returns an enumerator that iterates through the OctetString byte collection
		/// </summary>
		/// <returns>An IEnumerator  object that can be used to iterate through the collection.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			if (_data != null)
				return _data.GetEnumerator();
			return null;
		}
	}
}
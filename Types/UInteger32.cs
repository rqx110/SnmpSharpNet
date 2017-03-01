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
using System.Globalization;
namespace SnmpSharpNet
{
	
	/// <summary>SMI unsigned 32-bit integer value class.
	/// </summary>
	[Serializable]
	public class UInteger32 : AsnType, IComparable<UInteger32>, IComparable<UInt32>
	{
		/// <summary>Internal unsigned integer 32-bit value
		/// </summary>
		protected UInt32 _value;
		
		/// <summary>Constructor. Class value is set to 0.
		/// </summary>
		public UInteger32()
		{
			_asnType = SnmpConstants.SMI_UNSIGNED32;
		}
		
		/// <summary>Constructor. SET the class value to the supplied 32-bit value.
		/// </summary>
		/// <param name="val">Value to initialize the class with.
		/// </param>
		public UInteger32(UInt32 val):this()
		{
			_value = val;
		}
		
		/// <summary>Constructor. Initializes the class to the same value as the argument.
		/// </summary>
		/// <param name="second">Object whose value is used to initialize this class.
		/// </param>
		public UInteger32(UInteger32 second):this(second.Value)
		{
		}
		
		/// <summary>Constructor. Initialize the class with the unsigned integer 32-bit value
		/// stored as a string value in the argument.
		/// </summary>
		/// <param name="val">Unsigned integer value encoded as a string
		/// </param>
		public UInteger32(String val):this()
		{
			Set(val);
		}

		/// <summary>
		/// Parse an UInteger32 value from a string.
		/// </summary>
		/// <param name="value">String containing an UInteger32 value</param>
		/// <exception cref="ArgumentException">Argument string is length == 0</exception>
		public void Set(string value)
		{
			if (value.Length == 0)
				throw new ArgumentException("value","String has to be length greater then 0");

			_value = UInt32.Parse(value);
		}
		/// <summary>
		/// SET class value from another UInteger32 or Integer32 class cast as <seealso cref="AsnType"/>.
		/// </summary>
		/// <param name="value">UInteger32 class cast as <seealso cref="AsnType"/></param>
		/// <exception cref="ArgumentNullException">Parameter is null.</exception>
		public void Set(AsnType value)
		{
			if (value == null)
				throw new ArgumentNullException("value", "Parameter is null");
			if (value is UInteger32)
			{
				_value = ((UInteger32)value).Value;
			} else if( value is Integer32)
			{
				_value = (UInt32) ((Integer32) value).Value;
			}
		}
		
		/// <summary>
		/// Value of the object. Returns 32 bit unsigned integer
		/// </summary>
		public UInt32 Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		/// <summary> 
		/// Returns the string representation of the object.
		/// </summary>
		/// <returns>String representation of the of the class value.</returns>
		public override System.String ToString()
		{
			return System.Convert.ToString(_value, CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Returns a duplicate of the current object
		/// </summary>
		/// <returns>
		/// A duplicate copy of the current object
		/// </returns>
		public override Object Clone()
		{
			return new UInteger32(this);
		}

		/// <summary>
		/// Implicit casting of the object value as UInt32 value
		/// </summary>
		/// <param name="value">UInteger32 class</param>
		/// <returns>UInt32 value stored by the UInteger32 class</returns>
		public static implicit operator UInt32(UInteger32 value)
		{
			if (value == null)
				return 0;
			return value.Value;
		}

		#region Encode and decode methods

		/// <summary>BER encode class value.</summary>
		/// <param name="buffer">Target buffer. Value is appended to the end of it.</param>
		public override void encode(MutableByte buffer)
		{
			MutableByte tmp = new MutableByte();
			byte[] b = BitConverter.GetBytes(_value);

			for (int i = 3; i >= 0; i--)
			{
				if (b[i] != 0 || tmp.Length > 0)
					tmp.Append(b[i]);
			}
			// if (tmp.Length > 1 && tmp[0] == 0xff && (tmp[1] & 0x80) != 0)
			if( tmp.Length > 0 && (tmp[0] & 0x80) != 0)
			{
				tmp.Prepend(0);
			}
			else if (tmp.Length == 0)
				tmp.Append(0);
			BuildHeader(buffer, Type, tmp.Length);
			buffer.Append(tmp);
		}
		
		/// <summary>Decode BER encoded value</summary>
		/// <remarks>
		/// Used to decode the integer value from the ASN.1 buffer.
		/// The passed encoder is used to decode the ASN.1 information
		/// and the integer value is stored in the internal object.
		/// </remarks>
		/// <param name="buffer">BER encoded buffer</param>
		/// <param name="offset">The offset of the first byte of data to decode. This variable will hold the offset of the first byte
		/// immediately after the value we decoded.</param>
		/// <returns>Buffer position after the decoded value</returns>
		public override int decode(byte[] buffer, int offset)
		{
			//
			// parse the header first
			//
			int headerLength;
			byte asnType = ParseHeader(buffer, ref offset, out headerLength);

			if (asnType != Type)
				throw new SnmpException("Invalid ASN.1 type.");

			// check for sufficient data
			if ((buffer.Length - offset) < headerLength)
				throw new OverflowException("Buffer underflow error");

			//
			// check to see that we can actually decode
			// the value (must fit in integer == 32-bits)
			//
			if (headerLength > 5)
				throw new OverflowException("Integer too large: cannot decode");

			_value = 0;
			for (int i = 0; i < headerLength; i++)
			{
				_value <<= 8;
				_value = _value | buffer[offset++];
			}

			return offset;
		}
		
		#endregion Encode and decode methods

		/// <summary>
		/// Compare implementation that will compare this class value with the value of another <see cref="UInteger32"/> class.
		/// </summary>
		/// <param name="other">UInteger32 value to compare class value with.</param>
		/// <returns>True if values are the same, otherwise false</returns>
		public int CompareTo(UInteger32 other)
		{
			return _value.CompareTo(other.Value);
		}

		/// <summary>
		/// Compare implementation that will compare this class value with argument UInt32 value.
		/// </summary>
		/// <param name="other">UInt32 value to compare class value with.</param>
		/// <returns>True if values are the same, otherwise false</returns>
		public int CompareTo(UInt32 other)
		{
			return _value.CompareTo(other);
		}

		/// <summary>
		/// Compare class value against the object argument. Supported argument types are 
		/// <see cref="UInteger32"/> and Int32.
		/// </summary>
		/// <param name="obj">Object to compare values with</param>
		/// <returns>True if object value is the same as this class, otherwise false.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (obj is UInteger32)
			{
				UInteger32 u32 = (UInteger32)obj;
				return _value.Equals(u32.Value);
			}
			else if (obj is UInt32)
			{
				UInt32 u32 = (UInt32)obj;
				return _value.Equals(u32);
			}
			return false; // last resort
		}

		/// <summary>
		/// Comparison operator
		/// </summary>
		/// <param name="first">First <see cref="UInteger32"/> class value to compare</param>
		/// <param name="second">Second <see cref="UInteger32"/> class value to compare</param>
		/// <returns>True if class values match, otherwise false</returns>
		public static bool operator ==(UInteger32 first, UInteger32 second)
		{
			if ((object)first == null && (object)second == null)
				return true;
			if ((object)first == null)
				return false;
			return first.Equals(second);
		}

		/// <summary>
		/// Negative comparison operator
		/// </summary>
		/// <param name="first">First <see cref="UInteger32"/> class value to compare</param>
		/// <param name="second">Second <see cref="UInteger32"/> class value to compare</param>
		/// <returns>True if class values do NOT match, otherwise false</returns>
		public static bool operator !=(UInteger32 first, UInteger32 second)
		{
			return ! ( first == second );
		}
		/// <summary>
		/// Greater then operator
		/// </summary>
		/// <remarks>Compare two UInteger32 class values and return true if first class value is greater then second.</remarks>
		/// <param name="first">First class</param>
		/// <param name="second">Second class</param>
		/// <returns>True if first class value is greater then second class value, otherwise false</returns>
		public static bool operator >(UInteger32 first, UInteger32 second)
		{
			if ((object)first == null && (object)second == null)
				return false;
			if ((object)first == null)
				return false;
			if ((object)second == null)
				return true;
			if (first.Value > second.Value)
				return true;
			return false;
		}

		/// <summary>
		/// Less then operator
		/// </summary>
		/// <remarks>Compare two UInteger32 class values and return true if first class value is less then second.</remarks>
		/// <param name="first">First class</param>
		/// <param name="second">Second class</param>
		/// <returns>True if first class value is less then second class value, otherwise false</returns>
		public static bool operator <(UInteger32 first, UInteger32 second)
		{
			if ((object)first == null && (object)second == null)
				return false;
			if ((object)first == null)
				return true;
			if ((object)second == null)
				return false;
			if (first.Value < second.Value)
				return true;
			return false;
		}

		/// <summary>
		/// Returns hashed class value.
		/// </summary>
		/// <returns>Int32 hash value</returns>
		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}
		/// <summary>
		/// Addition operator.
		/// </summary>
		/// <remarks>
		/// Add two UInteger32 object values. Values of the two objects are added and
		/// a new class is instantiated with the result. Original values of the two parameter classes
		/// are preserved.
		/// </remarks>
		/// <param name="first">First UInteger32 object</param>
		/// <param name="second">Second UInteger32 object</param>
		/// <returns>New object with values of the 2 parameter objects added. If both parameters are null
		/// references then null is returned. If either of the two parameters is null, the non-null objects
		/// value is set as the value of the new object and returned.</returns>
		public static UInteger32 operator +(UInteger32 first, UInteger32 second)
		{
			if (first == null && second == null)
				return null;
			if (first == null)
			{
				return new UInteger32(second);
			}
			if (second == null)
			{
				return new UInteger32(first);
			}

			return new UInteger32(first.Value + second.Value);
		}
		/// <summary>
		/// Subtraction operator
		/// </summary>
		/// <remarks>
		/// Subtract the value of the second UInteger32 class value from the first UInteger32 class value. 
		/// Values of the two objects are subtracted and a new class is instantiated with the result. 
		/// Original values of the two parameter classes are preserved.
		/// </remarks>
		/// <param name="first">First UInteger32 object</param>
		/// <param name="second">Second UInteger32 object</param>
		/// <returns>New object with subtracted values of the 2 parameter objects. If both parameters are null
		/// references then null is returned. If either of the two parameters is null, the non-null objects
		/// value is set as the value of the new object and returned.</returns>
		public static UInteger32 operator -(UInteger32 first, UInteger32 second)
		{
			if (first == null && second == null)
				return null;
			if (first == null)
			{
				return new UInteger32(second);
			}
			if (second == null)
			{
				return new UInteger32(first);
			}

			return new UInteger32(first.Value - second.Value);
		}
	}
}
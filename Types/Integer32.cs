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
namespace SnmpSharpNet
{
	/// <summary>ASN.1 Integer32 class.</summary>
	/// <remarks>
	/// This class defines the SNMP 32-bit signed integer
	/// used by the SNMP SMI. This class also serves as a 
	/// base class for any additional SNMP SMI types that 
	/// exits now or may be defined in the future.
	/// </remarks>
	[Serializable]
	public class Integer32 : AsnType, IComparable<Integer32>, IComparable<Int32>, ICloneable
	{
		/// <summary>Internal class value</summary>
		protected int _value;
		
		/// <summary>Constructor
		/// </summary>
		public Integer32()
		{
			_asnType = SnmpConstants.SMI_INTEGER;
		}
		
		/// <summary>Constructor
		/// </summary>
		/// <param name="val">Class value initializer
		/// </param>
		public Integer32(int val):this()
		{
			_value = val;
		}
		
		
		/// <summary>Copy constructor
		/// </summary>
		/// <param name="second">Class value initializer
		/// </param>
		public Integer32(Integer32 second):this()
		{
			Set(second);
		}
		
		/// <summary>Constructor
		/// </summary>
		/// <param name="val">Integer value in a string format.
		/// </param>
		public Integer32(System.String val):this()
		{
			Set(val);
		}

		/// <summary>
		/// SET class value from another Integer32 class cast as <see cref="AsnType"/>.
		/// </summary>
		/// <param name="value">Integer32 class cast as <see cref="AsnType"/></param>
		/// <exception cref="ArgumentException">Argument is not Integer32 type.</exception>
		public void Set(AsnType value)
		{
			Integer32 val = value as Integer32;
			if (val != null)
			{
				_value = val.Value;
			}
			else
			{
				throw new ArgumentException("Invalid argument type.");
			}
		}

		/// <summary>
		/// Parse an Integer32 value from a string.
		/// </summary>
		/// <param name="value">String containing an Integer32 value</param>
		/// <exception cref="ArgumentOutOfRangeException">Argument string is length == 0</exception>
		/// <exception cref="ArgumentException">Unable to parse Integer32 value from the argument.</exception>
		public void Set(string value)
		{
			if (value.Length <= 0)
				throw new ArgumentOutOfRangeException(value,"String has to be length greater then 0");

			try
			{
				_value = System.Int32.Parse(value);
			}
			catch
			{
				throw new ArgumentException("Invalid argument format.");
			}
		}

		/// <summary>
		/// Get/SET internal integer value
		/// </summary>
		public int Value
		{
			get { return _value; }
			set { _value = value; }
		}

		/// <summary> Returns a duplicate of the current object.</summary>
		/// <returns> A newly allocated duplicate object.</returns>
		public override Object Clone()
		{
			return new Integer32(this);
		}
		
		/// <summary> Returns the string representation of the object.</summary>
		/// <returns>String representation of the class value.</returns>
		public override System.String ToString()
		{
			return _value.ToString();
		}

		/// <summary>
		/// Implicit casting of Integer32 value as Int32 value
		/// </summary>
		/// <param name="value">Integer32 class whose value is cast as Int32 value</param>
		/// <returns>Int32 value of the Integer32 class.</returns>
		public static implicit operator Int32(Integer32 value)
		{
			if (value == null)
				return 0;
			return value.Value;
		}

		/// <summary>
		/// Return class value hash code
		/// </summary>
		/// <returns>Int32 hash of the class stored value</returns>
		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}

		/// <summary>
		/// Set class value to a random integer.
		/// </summary>
		public void SetRandom()
		{
			Random rand = new Random();
			_value = rand.Next();
		}

		#region encode and decode methods

		/// <summary> Used to encode the integer value into an ASN.1 buffer.
		/// The passed encoder defines the method for encoding the
		/// data.
		/// </summary>
		/// <param name="buffer">Buffer target to write the encoded data</param>
		public override void encode(MutableByte buffer)
		{
			int val = _value;

			byte[] b = BitConverter.GetBytes(_value);

			MutableByte tmp = new MutableByte();
			// if value is negative
			if (val < 0)
			{
				for (int i = 3; i >= 0; i--)
				{
					if (tmp.Length > 0 || b[i] != 0xff)
					{
						tmp.Append(b[i]);
					}
				}
				if (tmp.Length == 0)
				{
					// if the value is -1 then all bytes in an integer are 0xff and will be skipped above
					tmp.Append(0xff);
				}
				// make sure value is negative
				if ((tmp[0] & 0x80) == 0)
					tmp.Prepend(0xff);
			}
			else if (val == 0)
			{
				// this is just a shortcut to save processing time
				tmp.Append(0);
			}
			else
			{

				// byte[] b = BitConverter.GetBytes(val);
				for (int i = 3; i >= 0; i--)
				{
					if (b[i] != 0 || tmp.Length > 0)
						tmp.Append(b[i]);
				}
				// if buffer length is 0 then value is 0 and we have to add it to the buffer
				if (tmp.Length == 0)
					tmp.Append(0);
				else
				{
					if ((tmp[0] & 0x80) != 0)
					{
						// first bit of the first byte has to be 0 otherwise value is negative.
						tmp.Prepend(0);
					}
				}
			}
			// check for 9 1s at the beginning of the encoded value
			if ((tmp.Length > 1 && tmp[0] == 0xff && (tmp[1] & 0x80) != 0))
			{

				tmp.Prepend(0);
			}
			BuildHeader(buffer, Type, tmp.Length);

			buffer.Append(tmp);
		}

		/// <summary> Used to decode the integer value from the BER buffer.
		/// The passed encoder is used to decode the BER encoded information
		/// and the integer value is stored in the internal object.
		/// </summary>
		/// <param name="buffer">Buffer holding BER encoded data</param>
		/// <param name="offset">Offset in the buffer to start parsing from</param>
		/// <returns>Buffer position after the decoded value</returns>
		public override int decode(byte[] buffer, int offset)
		{
			//
			// parse the header first
			//
			int headerLength;
			byte asnType = ParseHeader(buffer, ref offset, out headerLength);

			if (asnType != Type)
				throw new SnmpException("Invalid ASN.1 type");

			if ((buffer.Length - offset) < headerLength)
				throw new OverflowException("Buffer underflow error");

			bool isNegative = false;

			if( headerLength > 5 )
				throw new OverflowException("Integer size is invalid. Unable to decode.");

			if ((buffer[offset] & HIGH_BIT) != 0)
			{
				isNegative = true;
			}
			if (buffer[offset] == 0x80 && headerLength > 2 && 
				(buffer[offset+1] == 0xff && (buffer[offset+2] & 0x80) != 0) )
			{
				// this is a filler byte to comply with no 9 x consecutive 1s
				offset += 1;
				headerLength -= 1; // we've used one byte of the encoded length
			}
			if (isNegative)
				_value = -1;
			else
				_value = 0;
			for (int i = 0; i < headerLength; i++)
			{
				_value <<= 8;
				_value = _value | buffer[offset++];
			}
			return offset;
		}

		#endregion encode and decode methods

		/// <summary>
		/// Compare implementation that will compare this class value with the value of another <see cref="Integer32"/> class.
		/// </summary>
		/// <param name="other">Integer32 value to compare class value with.</param>
		/// <returns>less then 0 if if parameter is less then, 0 if parameter is equal and greater then 0 if parameter is greater then the class value</returns>
		public int CompareTo(Integer32 other)
		{
			if( ((Object)other) == null)
			{
				return 1;
			}
			return _value.CompareTo(other.Value);
		}

		/// <summary>
		/// Compare implementation that will compare this class value with argument Int32 value.
		/// </summary>
		/// <param name="other">Int32 value to compare class value with.</param>
		/// <returns>less then 0 if if parameter is less then, 0 if parameter is equal and greater then 0 if parameter is greater then the class value</returns>
		public int CompareTo(int other)
		{
			return _value.CompareTo(other);
		}

		/// <summary>
		/// Compare class value against the object argument. Supported argument types are 
		/// <see cref="Integer32"/> and Int32.
		/// </summary>
		/// <param name="obj">Object to compare values with</param>
		/// <returns>True if object value is the same as this class, otherwise false.</returns>
		public override bool Equals(object obj)
		{
			if (obj is Integer32)
			{
				Integer32 i32 = (Integer32)obj;
				return _value.Equals(i32.Value);
			}
			else if (obj is Int32)
			{
				Int32 i32 = (Int32)obj;
				return _value.Equals(i32);
			}
			return false; // last resort
		}

		/// <summary>
		/// Comparison operator
		/// </summary>
		/// <param name="first">First <see cref="Integer32"/> class value to compare</param>
		/// <param name="second">Second <see cref="Integer32"/> class value to compare</param>
		/// <returns>True if class values match, otherwise false</returns>
		public static bool operator ==(Integer32 first, Integer32 second)
		{
			if ((object)first == null && (object)second == null)
			{
				return true;
			}
			if ((object)first == null && (object)second != null)
			{
				return false;
			}
			return first.Equals(second);
		}

		/// <summary>
		/// Negative comparison operator
		/// </summary>
		/// <param name="first">First <see cref="Integer32"/> class value to compare</param>
		/// <param name="second">Second <see cref="Integer32"/> class value to compare</param>
		/// <returns>True if class values do NOT match, otherwise false</returns>
		public static bool operator !=(Integer32 first, Integer32 second)
		{
			return !(first == second);
		}

		/// <summary>
		/// Addition operator.
		/// </summary>
		/// <remarks>
		/// Add two Integer32 object values. Values of the two objects are added and
		/// a new class is instantiated with the result. Original values of the two parameter classes
		/// are preserved.
		/// </remarks>
		/// <param name="first">First Integer32 object</param>
		/// <param name="second">Second Integer32 object</param>
		/// <returns>New object with values of the 2 parameter objects added. If both parameters are null
		/// references then null is returned. If either of the two parameters is null, the non-null objects
		/// value is set as the value of the new object and returned.</returns>
		public static Integer32 operator +(Integer32 first, Integer32 second)
		{
			if (first == null && second == null)
			{
				return null;
			}
			if (first == null)
			{
				return new Integer32(second);
			}
			if (second == null)
			{
				return new Integer32(first);
			}
			
			return new Integer32(first.Value + second.Value);
		}
		/// <summary>
		/// Subtraction operator
		/// </summary>
		/// <remarks>
		/// Subtract the value of the second Integer32 class value from the first Integer32 class value. 
		/// Values of the two objects are subtracted and a new class is instantiated with the result. 
		/// Original values of the two parameter classes are preserved.
		/// </remarks>
		/// <param name="first">First Integer32 object</param>
		/// <param name="second">Second Integer32 object</param>
		/// <returns>New object with subtracted values of the 2 parameter objects. If both parameters are null
		/// references then null is returned. If either of the two parameters is null, the non-null objects
		/// value is set as the value of the new object and returned.</returns>
		public static Integer32 operator -(Integer32 first, Integer32 second)
		{
			if (first == null && second == null)
			{
				return null;
			}
			if (first == null)
			{
				return new Integer32(second);
			}
			if (second == null)
			{
				return new Integer32(first);
			}

			return new Integer32(first.Value - second.Value);
		}
	}
}
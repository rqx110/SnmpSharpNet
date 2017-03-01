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
	/// <summary>ASN.1 Counter64 value implementation.</summary>
	/// <remarks>
	/// Counter64 value is unsigned 64-bit integer value that is incremented by the agent
	/// until maximum value is reached. When maximum value is reached, Counter64 value will
	/// roll over to 0.
	/// </remarks>
	[Serializable]
	public class Counter64 : AsnType, IComparable<UInt64>, IComparable<Counter64>, ICloneable
	{
		/// <summary>
		/// Internal 64-bit unsigned integer value.
		/// </summary>
		protected UInt64 _value;
		
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public Counter64()
		{
			_asnType = SnmpConstants.SMI_COUNTER64;
		}
		
		/// <summary>Constructor.</summary>
		/// <param name="value">Value to set class value.
		/// </param>
		public Counter64(long value):this()
		{
			_value = Convert.ToUInt64(value);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="value">Copy value</param>
		public Counter64(Counter64 value):this()
		{
			_value = value.Value;
		}
		
		/// <summary>Constructor.</summary>
		/// <param name="value">Value to initialize the class with.
		/// </param>
		public Counter64(UInt64 value):this()
		{
			_value = value;
		}
				
		/// <summary>Constructor.</summary>
		/// <remarks>
		/// Initialize the class by parsing a 64-bit unsigned integer value
		/// from the supplied string value.
		/// </remarks>
		/// <param name="value">64-bit unsigned integer value encoded as a string.
		/// </param>
		public Counter64(String value):this()
		{
			this.Set(value);
		}

		/// <summary>Get/Set class 64-bit unsigned value
		/// </summary>
		virtual public UInt64 Value
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
		/// SET class value from another Counter64 class cast as <seealso cref="AsnType"/>.
		/// </summary>
		/// <param name="value">Counter64 class cast as <seealso cref="AsnType"/></param>
		/// <exception cref="ArgumentException">Argument is not Counter64 type.</exception>
		public void Set(AsnType value)
		{
			Counter64 val = value as Counter64;
			if (val != null)
			{
				_value = val.Value;
			}
			else
				throw new ArgumentException("Invalid argument type.");
		}

		/// <summary>
		/// Parse an Counter64 value from a string.
		/// </summary>
		/// <param name="value">String containing an Counter64 value</param>
		/// <exception cref="ArgumentOutOfRangeException">Argument length is 0 (zero)</exception>
		/// <exception cref="ArgumentException">Unable to parse Counter64 value from the argument.</exception>
		public void Set(String value)
		{
			if (value.Length <= 0)
				throw new ArgumentOutOfRangeException(value,"String has to be length greater then 0");

			try
			{
				_value = Convert.ToUInt64(value, CultureInfo.CurrentCulture);
			}
			catch
			{
				throw new ArgumentException("Invalid argument format.");
			}
		}
		
		
		/// <summary>Duplicate current object.</summary>
		/// <returns>Duplicate of the current object.</returns>
		public override Object Clone()
		{
			return new Counter64(this);
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
		/// Compare class value against the object argument. Supported argument types are 
		/// <see cref="Counter64"/> and UInt64.
		/// </summary>
		/// <param name="obj">Object to compare values with</param>
		/// <returns>True if object value is the same as this class, otherwise false.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (obj is Counter64)
				return _value.Equals(((Counter64)obj).Value);
			if (obj is UInt64)
				return _value.Equals((UInt64)obj);
			return false;
		}

		/// <summary>
		/// Returns the string representation of the object.
		/// </summary>
		/// <returns>String representation of the object value</returns>
		public override System.String ToString()
		{
			return Value.ToString(CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Implicit casting of Counter64 value as UInt64 value
		/// </summary>
		/// <param name="value">Counter64 class whose value is cast as UInt64 value</param>
		/// <returns>UInt64 value of the Counter64 class.</returns>
		public static implicit operator UInt64(Counter64 value)
		{
			return value.Value;
		}

		#region Encode & Decode methods

		/// <summary>BER encode class value</summary>
		/// <param name="buffer">MutableByte to append BER encoded value to.
		/// </param>
		public override void encode(MutableByte buffer)
		{
			byte[] b = BitConverter.GetBytes(_value);
			MutableByte tmp = new MutableByte();
			for (int i = b.Length - 1; i >= 0; i--)
			{
				if (b[i] != 0 || tmp.Length > 0)
					tmp.Append(b[i]);
			}
			if (tmp.Length == 0)
				tmp.Append(0); // value is 0. can't have an empty encoding
			BuildHeader(buffer, Type, tmp.Length);
			buffer.Append(tmp);
		}

		/// <summary>Decode BER encoded Counter64 value
		/// </summary>
		/// <param name="buffer">The encoded ASN.1 data</param>
		/// <param name="offset">Offset to start value decoding from.</param>
		/// <returns>Offset after the parsed value.</returns>
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

			// check to see that we can actually decode
			// the value (must fit in integer == 64-bits)
			if (headerLength > 9)
				throw new OverflowException("Integer too large: cannot decode");

			byte[] tmpBuf = new byte[8]; // we need 8 bytes to represent a UInt64
			if (headerLength == 9)
			{
				// if length is 9 we have a padding byte added. Skip it
				offset += 1;
				headerLength -= 1;
			}
			while (headerLength > 0)
			{
				tmpBuf[headerLength - 1] = buffer[offset];
				offset += 1;
				headerLength -= 1;
			}
			_value = BitConverter.ToUInt64(tmpBuf, 0);

			return offset;
		}

		#endregion


		/// <summary>
		/// Compare class value with the UInt64 variable
		/// </summary>
		/// <param name="other">Variable to compare with</param>
		/// <returns>less then 0 if if parameter is less then, 0 if paramater is equal and greater then 0 if parameter is greater then the class value</returns>
		public int CompareTo(UInt64 other)
		{
			return _value.CompareTo(other);
		}

		/// <summary>
		/// Compare class value with the value of the second class
		/// </summary>
		/// <param name="other">Class whose value we are comparing with</param>
		/// <returns>less then 0 if if parameter is less then, 0 if paramater is equal and greater then 0 if parameter is greater then the class value</returns>
		public int CompareTo(Counter64 other)
		{
			if (other == null)
				return -1;
			return _value.CompareTo(other.Value);
		}

		/// <summary>
		/// Check for equality of class values
		/// </summary>
		/// <param name="first">First class value to compare</param>
		/// <param name="second">Second class value to compare</param>
		/// <returns>True if values are the same, otherwise false</returns>
		public static bool operator ==(Counter64 first, Counter64 second)
		{
			if ((object)first == null && (object)second == null)
				return true;
			if ((object)first == null || (object)second == null)
				return false;
			return first.Equals(second);
		}
		/// <summary>
		/// Negative comparison
		/// </summary>
		/// <param name="first">First class value to compare</param>
		/// <param name="second">Second class value to compare</param>
		/// <returns>False if values are the same, otherwise true</returns>
		public static bool operator !=(Counter64 first, Counter64 second)
		{
			if ((object)first == null && (object)second == null)
				return false;
			if ((object)first == null || (object)second == null)
				return true;
			return (!first.Equals(second));
		}

		/// <summary>
		/// Greater then operator
		/// </summary>
		/// <remarks>Compare two Counter64 class values and return true if first class value is greater then second.</remarks>
		/// <param name="first">First class</param>
		/// <param name="second">Second class</param>
		/// <returns>True if first class value is greater then second class value, otherwise false</returns>
		public static bool operator >(Counter64 first, Counter64 second)
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
		/// <remarks>Compare two Counter64 class values and return true if first class value is less then second.</remarks>
		/// <param name="first">First class</param>
		/// <param name="second">Second class</param>
		/// <returns>True if first class value is less then second class value, otherwise false</returns>
		public static bool operator <(Counter64 first, Counter64 second)
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
		/// Addition operator.
		/// </summary>
		/// <remarks>
		/// Add two Counter64 object values. Values of the two objects are added and
		/// a new class is instantiated with the result. Original values of the two parameter classes
		/// are preserved.
		/// </remarks>
		/// <param name="first">First Counter64 object</param>
		/// <param name="second">Second Counter64 object</param>
		/// <returns>New object with values of the 2 parameter objects added. If both parameters are null
		/// references then null is returned. If either of the two parameters is null, the non-null objects
		/// value is set as the value of the new object and returned.</returns>
		public static Counter64 operator +(Counter64 first, Counter64 second)
		{
			if (first == null && second == null)
				return null;
			if (first == null)
				return new Counter64(second);
			if (second == null)
				return new Counter64(first);
			return new Counter64(first.Value + second.Value);
		}
		/// <summary>
		/// Subtraction operator
		/// </summary>
		/// <remarks>
		/// Subtract the value of the second Counter64 class value from the first Counter64 class value. 
		/// Values of the two objects are subtracted and a new class is instantiated with the result. 
		/// Original values of the two parameter classes are preserved.
		/// </remarks>
		/// <param name="first">First Counter64 object</param>
		/// <param name="second">Second Counter64 object</param>
		/// <returns>New object with subtracted values of the 2 parameter objects. If both parameters are null
		/// references then null is returned. If either of the two parameters is null, the non-null objects
		/// value is set as the value of the new object and returned.</returns>
		public static Counter64 operator -(Counter64 first, Counter64 second)
		{
			if (first == null && second == null)
				return null;
			if (first == null)
				return new Counter64(second);
			if (second == null)
				return new Counter64(first);
			return new Counter64(first.Value - second.Value);
		}
		/// <summary>
		/// Return difference between two Counter64 values taking counter roll-over into account.
		/// </summary>
		/// <param name="first">First or older value</param>
		/// <param name="second">Second or newer value</param>
		/// <returns>Difference between the two values</returns>
		public static UInt64 Diff(Counter64 first, Counter64 second)
		{
			UInt64 f = first.Value;
			UInt64 s = second.Value;
			UInt64 res = 0;
			if (s > f)
			{
				// in case of a roll-over event
				res = (UInt64.MaxValue - f) + s;
			}
			else
			{
				res = s - f;
			}
			return res;
		}
	}
}
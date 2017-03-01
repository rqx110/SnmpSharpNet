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
	/// <summary>
	/// Base class for all ASN.1 value classes
	/// </summary>
	public abstract class AsnType: ICloneable
	{
		/// <summary>Bool true/false value type</summary>
		public static readonly byte BOOLEAN = (byte)0x01;

		/// <summary>Signed 32-bit integer type</summary>
		public static readonly byte INTEGER = (byte)0x02;

		/// <summary>Bit sequence type</summary>
		public static readonly byte BITSTRING = (byte)0x03;

		/// <summary>Octet (byte) value type</summary>
		public static readonly byte OCTETSTRING = (byte)0x04;

		/// <summary>Null (no value) type</summary>
		public static readonly byte NULL = (byte)0x05;

		/// <summary>Object id type</summary>
		public static readonly byte OBJECTID = (byte)0x06;

		/// <summary>Arbitrary data type</summary>
		public static readonly byte SEQUENCE = (byte)0x10;

		/// <summary> Defined by referencing a fixed, unordered list of types,
		/// some of which may be declared optional. Each value is an
		/// unordered list of values, one from each component type.
		/// </summary>
		public static readonly byte SET = (byte)0x11;

		/// <summary> Generally useful, application-independent types and
		/// construction mechanisms.
		/// </summary>
		public static readonly byte UNIVERSAL = (byte)0x00;

		/// <summary> Relevant to a particular application. These are defined
		/// in standards other than ASN.1.
		/// </summary>
		public static readonly byte APPLICATION = (byte)0x40;

		/// <summary> Also relevant to a particular application, but limited by context
		/// </summary>
		public static readonly byte CONTEXT = (byte)0x80;

		/// <summary> These are types not covered by any standard but instead defined by users.
		/// </summary>
		public static readonly byte PRIVATE = (byte)0xC0;

		/// <summary> A primitive data object.</summary>
		public static readonly byte PRIMITIVE = (byte)0x00;

		/// <summary> A constructed data object such as a set or sequence.</summary>
		public static readonly byte CONSTRUCTOR = (byte)0x20;
		/// <summary> Defines the "high bit" that is the sign extension bit for a 8-bit signed value.
		/// </summary>
		protected static readonly byte HIGH_BIT = (byte)0x80;

		/// <summary> Defines the BER extension "value" that is used to mark an extension type.
		/// </summary>
		protected static readonly byte EXTENSION_ID = (byte)0x1F;
		/// <summary>
		/// ASN.1 type byte.
		/// </summary>
		protected byte _asnType;
		/// <summary>
		/// Get ASN.1 value type stored in this class.
		/// </summary>
		public byte Type
		{
			get
			{
				return _asnType;
			}
			set
			{
				_asnType = value;
			}
		}
		/// <summary>
		/// Encodes the data object in the specified buffer
		/// </summary>
		/// <param name="buffer">The buffer to write the encoded information</param>
		public abstract void encode(MutableByte buffer);

		/// <summary>
		/// Decodes the ASN.1 buffer and sets the values in the AsnType object.
		/// </summary>
		/// <param name="buffer">The encoded data buffer</param>
		/// <param name="offset">The offset of the first valid byte.</param>
		/// <returns>New offset pointing to the byte after the last decoded position
		/// </returns>
		public abstract int decode(byte[] buffer, int offset);
		/// <summary>
		/// Append BER encoded length to the <see cref="MutableByte"/>
		/// </summary>
		/// <param name="mb">MutableArray to append BER encoded length to</param>
		/// <param name="asnLength">Length value to encode.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when length value to encode is less then 0</exception>
		internal static void BuildLength(MutableByte mb, int asnLength)
		{
			if (asnLength < 0)
				throw new ArgumentOutOfRangeException("Length cannot be less then 0.");
			byte[] len = BitConverter.GetBytes(asnLength);
			MutableByte buf = new MutableByte();
			for (int i = 3; i >= 0; i--)
			{
				if (len[i] != 0 || buf.Length > 0)
					buf.Append(len[i]);
			}
			if (buf.Length == 0)
			{
				// we are encoding a 0 value. Can't have a 0 byte length encoding
				buf.Append(0);
			}
			// check for short form encoding
			if (buf.Length == 1 && (buf[0] & HIGH_BIT) == 0)
				mb.Append(buf); // done
			else
			{
				// long form encoding
				byte encHeader = (byte)buf.Length;
				encHeader = (byte)(encHeader | HIGH_BIT);
				mb.Append(encHeader);
				mb.Append(buf);
			}
		}
		/// <summary>
		/// MutableByte version of ParseLength. Retrieve BER encoded length from a byte array at supplied offset
		/// </summary>
		/// <param name="mb">BER encoded data</param>
		/// <param name="offset">Offset to start parsing length from</param>
		/// <returns>Length value</returns>
		/// <exception cref="OverflowException">Thrown when buffer is too short</exception>
		internal static int ParseLength(byte[] mb, ref int offset)
		{
			if( offset == mb.Length )
				throw new OverflowException("Buffer is too short.");
			int dataLen = 0;
			if ((mb[offset] & HIGH_BIT) == 0)
			{
				// short form encoding
				dataLen = mb[offset++];
				return dataLen; // we are done
			}
			else
			{
				dataLen = mb[offset++] & ~HIGH_BIT; // store byte length of the encoded length value
				int value = 0;
				for (int i = 0; i < dataLen; i++)
				{
					value <<= 8;
					value |= mb[offset++];
					if( offset > mb.Length || (i < (dataLen-1) && offset == mb.Length )	)
						throw new OverflowException("Buffer is too short.");
				}
				return value;
			}
		}
		/// <summary>
		/// Build ASN.1 header in the MutableByte array.
		/// </summary>
		/// <remarks>
		/// Header is the TL part of the TLV (type, length, value) BER encoded data representation.
		/// 
		/// Each value is encoded as a Type byte, length of the data field and the actual, encoded
		/// data. This method will encode the type and length fields.
		/// </remarks>
		/// <param name="mb">MurableByte array</param>
		/// <param name="asnType">ASN.1 header type</param>
		/// <param name="asnLength">Length of the data contained in the header</param>
		internal static void BuildHeader(MutableByte mb, byte asnType, int asnLength)
		{
			mb.Append(asnType);
			BuildLength(mb, asnLength);
		}
		/// <summary>
		/// Parse ASN.1 header.
		/// </summary>
		/// <param name="mb">BER encoded data</param>
		/// <param name="offset">Offset in the packet to start parsing the header from</param>
		/// <param name="length">Length of the data in the section starting with parsed header</param>
		/// <returns>ASN.1 type of the header</returns>
		/// <exception cref="OverflowException">Thrown when buffer is too short</exception>
		/// <exception cref="SnmpException">Thrown when invalid type is encountered in the header</exception>
		internal static byte ParseHeader(byte[] mb, ref int offset, out int length)
		{
			if ((mb.Length - offset) < 1)
			{
				throw new OverflowException("Buffer is too short.");
			}

			// ASN.1 type
			byte asnType = mb[offset++];
			if ((asnType & EXTENSION_ID) == EXTENSION_ID)
			{
				throw new SnmpException("Invalid SNMP header type");
			}

			// length
			length = ParseLength(mb, ref offset);
			return asnType;
		}
		/// <summary>
		/// Abstract Clone() member function
		/// </summary>
		/// <returns>Duplicated current object cast as Object</returns>
		public abstract object Clone();
	}
}

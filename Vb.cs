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
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SnmpSharpNet {
	/// <summary>
	/// Vb item. Stores Oid => value pair for each value
	/// </summary>
	public class Vb : AsnType, ICloneable
	{
		/// <summary>
		/// OID of the object
		/// </summary>
		private Oid _oid;
		/// <summary>
		/// Value of the object
		/// </summary>
		private AsnType _value;
		/// <summary>
		/// Standard constructor. Initializes values to null.
		/// </summary>
		public Vb() {
			_asnType = (byte)(SEQUENCE | CONSTRUCTOR);
		}
		/// <summary>
		/// Construct Vb with the supplied OID and Null value
		/// </summary>
		/// <param name="oid">OID</param>
		public Vb(Oid oid)
			: this()
		{
			_oid = (Oid)oid.Clone();
			_value = new Null();
		}
		/// <summary>
		/// Construct Vb with the OID and value
		/// </summary>
		/// <param name="oid">OID</param>
		/// <param name="value">Value</param>
		public Vb(Oid oid, AsnType value)
			: this(oid) {
				_value = (AsnType)value.Clone();
		}
		/// <summary>
		/// Construct Vb with the oid value and <seealso cref="Null"/> value.
		/// </summary>
		/// <param name="oid">String representing OID value to set</param>
		public Vb(string oid)
			: this()
		{
			_oid = new Oid(oid);
			_value = new Null();
		}
		/// <summary>
		/// Copy constructor. Initialize class with cloned values from second class.
		/// </summary>
		/// <param name="second">Vb class to clone data from.</param>
		public Vb(Vb second)
			: this()
		{
			Set(second);
		}
		/// <summary>
		/// SET class value from supplied Vb class
		/// </summary>
		/// <param name="value">Vb class to clone data from</param>
		public void Set(Vb value)
		{
			this._oid = (Oid)value.Oid.Clone();
			this._value = (Oid)value.Value.Clone();
		}
		/// <summary>
		/// SET/Get AsnType value of the Vb
		/// </summary>
		public AsnType Value {
			set {
				_value = (AsnType)value.Clone();
			}
			get {
				return _value;
			}
		}
		/// <summary>
		/// Get/SET OID of the Vb
		/// </summary>
		public Oid Oid {
			set {
				_oid = (Oid)value.Clone();
			}
			get {
				return _oid;
			}
		}

		/// <summary>
		/// Reset Vb value to Null
		/// </summary>
		public void ResetValue()
		{
			_value = new Null();
		}

		/// <summary>
		/// Clone Vb object
		/// </summary>
		/// <returns>Cloned Vb object cast to System.Object</returns>
		public override Object Clone() {
			return new Vb(this._oid, this._value);
		}
		/// <summary>
		/// Return printable string in the format oid: value
		/// </summary>
		/// <returns>Format Vb string</returns>
		public override string ToString() {
			return _oid.ToString() + ": (" + SnmpConstants.GetTypeName(_value.Type) + ") " + _value.ToString();
		}
		/// <summary>BER encode the variable binding
		/// </summary>
		/// <param name="buffer"><see cref="MutableByte"/> class to the end of which encoded variable
		/// binding values will be added.
		/// </param>
		public override void encode(MutableByte buffer)
		{
			// encode oid to the temporary buffer
			MutableByte oidbuf = new MutableByte();
			_oid.encode(oidbuf);
			// encode value to a temporary buffer
			MutableByte valbuf = new MutableByte();
			_value.encode(valbuf);

			// calculate data content length of the vb
			int vblen = oidbuf.Length + valbuf.Length;
			// encode vb header at the end of the result
			BuildHeader(buffer, Type, vblen);
			// add values to the encoded arrays to the end of the result
			buffer.Append(oidbuf);
			buffer.Append(valbuf);
		}
		/// <summary>Decode BER encoded variable binding.
		/// </summary>
		/// <param name="buffer">BER encoded buffer
		/// </param>
		/// <param name="offset">Offset in the data buffer from where to start decoding. Offset is
		/// passed by reference and will contain the offset of the byte immediately after the parsed
		/// variable binding.
		/// </param>
		/// <returns>Buffer position after the decoded value</returns>
		public override int decode(byte[] buffer, int offset)
		{
			int headerLength;
			byte asnType = ParseHeader(buffer, ref offset, out headerLength);

			if (asnType != Type)
				throw new SnmpException(string.Format("Invalid ASN.1 type. Expected 0x{0:x2} received 0x{1:x2}", Type, asnType));

			// verify the length
			if ((buffer.Length - offset) < headerLength)
				throw new OverflowException("Buffer underflow error");

			_oid = new Oid();
			offset = _oid.decode(buffer, offset);
			int saveOffset = offset;
			// Look ahead in the header to see the data type we need to parse
			asnType = ParseHeader(buffer, ref saveOffset, out headerLength);
			_value = SnmpConstants.GetSyntaxObject(asnType);
			if (_value == null)
				throw new SnmpDecodingException(String.Format("Invalid ASN.1 type encountered 0x{0:x2}. Unable to continue decoding.", asnType));
			offset = _value.decode(buffer, offset);
			return offset;
		}
	}
}

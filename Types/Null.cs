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
	
	/// <summary>ASN.1 Null value implementation.</summary>
	[Serializable]
	public class Null : AsnType, ICloneable
	{
		/// <summary>Constructor</summary>
		public Null()
		{
			_asnType = SnmpConstants.SMI_NULL;
		}
		
		/// <summary>Constructor</summary>
		/// <param name="second">Irrelevant. Nothing to copy</param>
		public Null(Null second) : this()
		{
		}
		
		#region Encode & decode methods

		/// <summary> 
		/// ASN.1 encode Null value
		/// </summary>
		/// <param name="buffer"><see cref="MutableByte"/> class to the end of which encoded data is appended
		/// </param>
		public override void encode(MutableByte buffer)
		{
			BuildHeader(buffer, Type, 0);
		}
		
		/// <summary>
		/// Decode null value from BER encoded buffer.
		/// </summary>
		/// <param name="buffer">BER encoded buffer</param>
		/// <param name="offset">Offset within the buffer from where to start decoding. On return,
		/// this argument contains the offset immediately following the decoded value.
		/// </param>
		/// <returns>Buffer position after the decoded value</returns>
		/// <exception cref="SnmpException">Thrown when parsed ASN.1 type is not null</exception>
		/// <exception cref="SnmpException">Thrown when length of null value is greater then 0 bytes</exception>
		public override int decode(byte[] buffer, int offset)
		{
			int headerLength;
			byte asnType = ParseHeader(buffer, ref offset, out headerLength);

			if (asnType != Type)
				throw new SnmpException("Invalid ASN.1 Type");

			// Verify length is 0
			if (headerLength != 0)
				throw new SnmpException("Malformed ASN.1 Type");

			return offset;
		}
		
		#endregion Encode & decode methods
		
		/// <summary>Clone current object</summary>
		/// <returns>Duplicate of the current object.</returns>
		public override Object Clone()
		{
			return new Null(this);
		}
		
		/// <summary> Returns a string representation of the SNMP NULL object</summary>
		/// <returns>String representation of the class</returns>
		public override System.String ToString()
		{
			return "Null";
		}
	}
}
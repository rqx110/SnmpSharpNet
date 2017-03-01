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
	/// Returned when end of MIB has been reached when performing GET-NEXT or GET-BULK operations.
	/// </summary>
	[Serializable]
	public class EndOfMibView : V2Error, System.ICloneable
	{
		/// <summary> The default class construtor.</summary>
		public EndOfMibView():base()
		{
			_asnType = SnmpConstants.SMI_ENDOFMIBVIEW;
		}
		
		/// <summary> The class copy constructor.
		/// </summary>
		/// <param name="second">The object to copy into self.
		/// </param>
		public EndOfMibView(EndOfMibView second):base(second)
		{
			_asnType = SnmpConstants.SMI_ENDOFMIBVIEW;
		}
		
		/// <summary> Returns a duplicate object of self. 
		/// </summary>
		/// <returns> A duplicate of self
		/// </returns>
		public override Object Clone()
		{
			return new EndOfMibView(this);
		}
		/// <summary>Decode ASN.1 encoded end-of-mib-view SNMP version 2 MIB value</summary>
		/// <param name="buffer">The encoded buffer</param>
		/// <param name="offset">The offset of the first byte of encoded data</param>
		/// <returns>Offset after the decoded value</returns>
		public override int decode(byte[] buffer, int offset)
		{
			int headerLength;
			byte asnType = ParseHeader(buffer, ref offset, out headerLength);
			if (asnType != Type)
			{
				throw new SnmpException("Invalid ASN.1 type");
			}

			if (headerLength != 0)
				throw new SnmpException("Invalid ASN.1 length");

			return offset;
		}

		/// <summary>
		/// ASN.1 encode end-of-mib-view SNMP version 2 MIB value
		/// </summary>
		/// <param name="buffer">MutableByte to append encoded variable to</param>
		public override void encode(MutableByte buffer)
		{
			BuildHeader(buffer, Type, 0);
		}
		
		/// <summary> Returns the string representation of the object.
		/// </summary>
		/// <returns>String prepresentation of the object.</returns>
		public override String ToString()
		{
			return "SNMP End-of-MIB-View";
		}
	}
}
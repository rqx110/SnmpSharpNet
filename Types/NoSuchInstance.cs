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
	
	
	/// <summary>SNMPv2 noSuchInstance error</summary>
	/// <remarks>
	/// Returned when requested instance is not present in a table of the SNMP version 2 agent.
	/// 
	/// This type is returned as value to a requested Oid. When looping through results, check Vb.Value
	/// member for V2Error returns.
	/// </remarks>
	[Serializable]
	public class NoSuchInstance : V2Error, ICloneable
	{
		/// <summary>Constructor.</summary>
		public NoSuchInstance():base()
		{
			_asnType = SnmpConstants.SMI_NOSUCHINSTANCE;
		}
		
		/// <summary>Constructor.</summary>
		/// <param name="second">The object to copy into self.</param>
		public NoSuchInstance(NoSuchInstance second):base(second)
		{
			_asnType = SnmpConstants.SMI_NOSUCHINSTANCE;
		}
		
		/// <summary>Returns a duplicate object of self.</summary>
		/// <returns>A duplicate of self</returns>
		public override System.Object Clone()
		{
			return new NoSuchInstance(this);
		}

		/// <summary>Decode BER encoded no-such-instance SNMP version 2 MIB value</summary>
		/// <param name="buffer">The BER encoded buffer</param>
		/// <param name="offset">The offset of the first byte of encoded data</param>
		/// <returns>Buffer position after the decoded value</returns>
		/// <exception cref="SnmpException">Invalid ASN.1 type found when parsing value header</exception>
		/// <exception cref="SnmpDecodingException">Invalid data length in ASN.1 header. Only data length 0 is accepted.</exception>
		public override int decode(byte[] buffer, int offset)
		{
			int headerLength;
			byte asnType = ParseHeader(buffer, ref offset, out headerLength);
			if (asnType != Type)
				throw new SnmpException("Invalid ASN.1 type");

			if (headerLength != 0)
				throw new SnmpDecodingException("Invalid ASN.1 length");

			return offset;
		}

		/// <summary>
		/// ASN.1 encode no-such-instance SNMP version 2 MIB value
		/// </summary>
		/// <param name="buffer">MutableByte reference to append encoded variable to</param>
		public override void encode(MutableByte buffer)
		{
			BuildHeader(buffer, Type, 0);
		}

		/// <summary> Returns the string representation of the object.</summary>
		/// <returns>String representatio of the class</returns>
		public override System.String ToString()
		{
			return "SNMP No-Such-Instance";
		}
	}
}
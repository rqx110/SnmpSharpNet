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
	
	/// <summary>Base class for SNMP version 2 error types.</summary>
	/// <remarks>
	/// For details see <see cref="NoSuchInstance"/>, <see cref="NoSuchObject"/> and <see cref="EndOfMibView"/>.
	/// </remarks>
	public class V2Error : AsnType, ICloneable
	{		
		/// <summary>
		/// Constructor
		/// </summary>
		public V2Error()
		{
			// do nothing
		}

		/// <summary>Constructor.</summary>
		/// <remarks>
		/// Since this class doesn't hold any meaningful information, constructor
		/// does nothing with the argument.
		/// </remarks>
		/// <param name="second">Second object</param>
		public V2Error(V2Error second)
		{
		}
		
		/// <summary>BER encode SNMP version 2 error.</summary>
		/// <param name="buffer">Buffer to append encoded value to the end of</param>
		public override void encode(MutableByte buffer)
		{
			BuildHeader(buffer, Type, 0);
		}
		
		/// <summary>Decode BER encoded SNMP version 2 error.</summary>
		/// <param name="buffer">BER encoded buffer</param>
		/// <param name="offset">Offset within the buffer to start decoding the value from. This argument will
		/// receive the new offset to the byte immediately following the decoded value.</param>
		/// <returns>Buffer position after the decoded value</returns>
		public override int decode(byte[] buffer, int offset)
		{
			int headerLength;
			byte asnType = ParseHeader(buffer, ref offset, out headerLength);
			
			/* Ignore type. This should be set in the inherited class. */
			if (asnType != Type)
				throw new SnmpException("Invalid ASN.1 type");
			
			if (headerLength != 0)
				throw new SnmpException("Invalid ASN.1 length");
			return offset;
		}
		
		/// <summary>Returns a duplicate of the object.
		/// </summary>
		/// <returns>A new copy of the current object cast as System.Object.
		/// </returns>
		public override Object Clone()
		{
			return new V2Error(this);
		}
	}
}
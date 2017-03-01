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

namespace SnmpSharpNet
{
	/// <summary>
	/// Enumeration of available Protocol Data Unit types
	/// </summary>
	public enum PduType: byte
	{
		/// <summary>
		/// SNMP Get request PDU type
		/// </summary>
		Get = 0xa0,
		/// <summary>
		/// SNMP GetNext request PDU type
		/// </summary>
		GetNext = 0xa1,
		/// <summary>
		/// SNMP Response PDU type
		/// </summary>
		Response = 0xa2,
		/// <summary>
		/// SNMP Set request PDU type
		/// </summary>
		Set = 0xa3,
		/// <summary>
		/// SNMP Trap notification PDU type
		/// </summary>
		Trap = 0xa4,
		/// <summary>
		/// SNMP GetBulk request PDU type
		/// </summary>
		GetBulk = 0xa5,
		/// <summary>
		/// SNMP Inform notification PDU type
		/// </summary>
		Inform = 0xa6,
		/// <summary>
		/// SNMP version 2 Trap notification PDU type
		/// </summary>
		V2Trap = 0xa7,
		/// <summary>
		/// SNMP version 3 Report notification PDU type
		/// </summary>
		Report = 0xa8
	}
}

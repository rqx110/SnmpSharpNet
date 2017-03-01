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
	/// <summary>Snmp V1 <see cref="Pdu.ErrorStatus" /> error values</summary>
	public enum SnmpV1PduErrorStatus: int
	{
		/// <summary>Unknown error code received.</summary>
		Unknown = -1,
		/// <summary>No error</summary>
		NoError = 0,
		/// <summary>Request too big</summary>
		TooBig = 1,
		/// <summary>Object identifier does not exist</summary>
		NoSuchName = 2,
		/// <summary>Invalid value</summary>
		BadValue = 3,
		/// <summary>Requested invalid operation on a read only table</summary>
		ReadOnly = 4,
		/// <summary>Generic error</summary>
		GenError = 5,
		/// <summary>Enterprise specific error</summary>
		EnterpriseSpecific = 6,
	}
}

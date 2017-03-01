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
	/// <summary>Snmp V2 <see cref="Pdu.ErrorStatus" /> error values</summary>
	public enum SnmpV2PduErrorStatus: int
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
		/// <summary>Access denied</summary>
		NoAccess = 6,
		/// <summary>Incorrect type</summary>
		WrongType = 7,
		/// <summary>Incorrect length</summary>
		WrongLength = 8,
		/// <summary>Invalid encoding</summary>
		WrongEncoding = 9,
		/// <summary>Object does not have correct value</summary>
		WrongValue = 10,
		/// <summary>Insufficient rights to perform create operation</summary>
		NoCreation = 11,
		/// <summary>Inconsistent value</summary>
		InconsistentValue = 12,
		/// <summary>Requested resource is not available</summary>
		ResourceUnavailable = 13,
		/// <summary>Unable to commit values</summary>
		CommitFailed = 14,
		/// <summary>Undo request failed</summary>
		UndoFailed = 15,
		/// <summary>Authorization failed</summary>
		AuthorizationError = 16,
		/// <summary>Instance not writable</summary>
		NotWritable = 17,
		/// <summary>Inconsistent object identifier</summary>
		InconsistentName = 18
	}
}

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
	/// Pdu and ScopedPdu error status value enumeration
	/// </summary>
	/// <remarks>Thanks to Pavel_Tatarinov@selinc.com</remarks>
	public enum PduErrorStatus
	{
		/// <summary>
		/// No error
		/// </summary>
		noError = 0,
		/// <summary>
		/// request or reply is too big
		/// </summary>
		tooBig = 1,
		/// <summary>
		/// requested name doesn't exist
		/// </summary>
		noSuchName = 2,
		/// <summary>
		/// bad value supplied
		/// </summary>
		badValue = 3,
		/// <summary>
		/// Oid is read only
		/// </summary>
		readOnly = 4,
		/// <summary>
		/// general error
		/// </summary>
		genErr = 5,
		/// <summary>
		/// access denied
		/// </summary>
		noAccess = 6,
		/// <summary>
		/// wrong type
		/// </summary>
		wrongType = 7,
		/// <summary>
		/// wrong length
		/// </summary>
		wrongLength = 8,
		/// <summary>
		/// wrong encoding
		/// </summary>
		wrongEncoding = 9,
		/// <summary>
		/// wrong value
		/// </summary>
		wrongValue = 10,
		/// <summary>
		/// no creation
		/// </summary>
		noCreation = 11,
		/// <summary>
		/// inconsistent value
		/// </summary>
		inconsistentValue = 12,
		/// <summary>
		/// resource is not available
		/// </summary>
		resourceUnavailable = 13,
		/// <summary>
		/// commit failed
		/// </summary>
		commitFailed = 14,
		/// <summary>
		/// undo failed
		/// </summary>
		undoFailed = 15,
		/// <summary>
		/// authorization error
		/// </summary>
		authorizationError = 16,
		/// <summary>
		/// not writable
		/// </summary>
		notWritable = 17,
		/// <summary>
		/// inconsistent name
		/// </summary>
		inconsistentName = 18
	}
}

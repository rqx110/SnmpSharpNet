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
	/// <summary>Helper returns error messages for SNMP v1 and v2 error codes</summary>
	/// <remarks>
	/// Helper class provides translation of SNMP version 1 and 2 error status codes to short, descriptive
	/// error messages.
	/// 
	/// To use, call the static member <see cref="SnmpError.ErrorMessage"/>.
	/// 
	/// Example:
	/// <code>Console.WriteLine("Agent error: {0}",SnmpError.ErrorMessage(12));</code>
	/// </remarks>
	public sealed class SnmpError
	{
		/// <summary>
		/// Return SNMP version 1 and 2 error code (errorCode field in the <see cref="Pdu"/> class) as
		/// a short, descriptive string.
		/// </summary>
		/// <param name="errorCode">Error code sent by the agent</param>
		/// <returns>Short error message for the error code</returns>
		public static string ErrorMessage(int errorCode)
		{
			if (errorCode == SnmpConstants.ErrNoError)
				return "No error";
			else if (errorCode == SnmpConstants.ErrTooBig)
				return "Request too big";
			else if (errorCode == SnmpConstants.ErrNoSuchName)
				return "noSuchName";
			else if (errorCode == SnmpConstants.ErrBadValue)
				return "badValue";
			else if (errorCode == SnmpConstants.ErrReadOnly)
				return "readOnly";
			else if (errorCode == SnmpConstants.ErrGenError)
				return "genericError";
			else if (errorCode == SnmpConstants.ErrNoAccess)
				return "noAccess";
			else if (errorCode == SnmpConstants.ErrWrongType)
				return "wrongType";
			else if (errorCode == SnmpConstants.ErrWrongLength)
				return "wrongLength";
			else if (errorCode == SnmpConstants.ErrWrongEncoding)
				return "wrongEncoding";
			else if (errorCode == SnmpConstants.ErrWrongValue)
				return "wrongValue";
			else if (errorCode == SnmpConstants.ErrNoCreation)
				return "noCreation";
			else if (errorCode == SnmpConstants.ErrInconsistentValue)
				return "inconsistentValue";
			else if (errorCode == SnmpConstants.ErrResourceUnavailable)
				return "resourceUnavailable";
			else if (errorCode == SnmpConstants.ErrCommitFailed)
				return "commitFailed";
			else if (errorCode == SnmpConstants.ErrUndoFailed)
				return "undoFailed";
			else if (errorCode == SnmpConstants.ErrAuthorizationError)
				return "authorizationError";
			else if (errorCode == SnmpConstants.ErrNotWritable)
				return "notWritable";
			else if (errorCode == SnmpConstants.ErrInconsistentName)
				return "inconsistentName";
			else
				return string.Format("Unknown error ({0})", errorCode);
		}

		/// <summary>
		/// Private constructor.
		/// </summary>
		private SnmpError()
		{
		}
	}
}

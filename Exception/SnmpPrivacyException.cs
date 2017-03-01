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
	/// Privacy encryption or decryption exception
	/// </summary>
	/// <remarks>
	/// Exception thrown when errors were encountered related to the privacy protocol encryption and decryption operations.
	/// 
	/// Use ParentException field to get the causing error details.
	/// </remarks>
	public class SnmpPrivacyException : SnmpException
	{
		/// <summary>
		/// Exception that caused this exception to be thrown
		/// </summary>
		private Exception _parentException;
		/// <summary>
		/// Exception that caused this exception to be thrown
		/// </summary>
		public Exception ParentException
		{
			get { return _parentException; }
		}
		/// <summary>
		/// Standard constructor initializes the exceptione error message
		/// </summary>
		/// <param name="msg">Error message</param>
		public SnmpPrivacyException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Constructor initializes error message and parent exception
		/// </summary>
		/// <param name="ex">Parent exception</param>
		/// <param name="msg">Error message</param>
		public SnmpPrivacyException(Exception ex, string msg)
			: base(msg)
		{
			_parentException = ex;
		}
	}
}

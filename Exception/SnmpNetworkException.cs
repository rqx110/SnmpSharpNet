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
	/// <summary>SNMP network exception</summary>
	/// <remarks>
	/// Exception thrown when network error was encountered. Network errors include host, network unreachable, connection refused, etc.
	/// 
	/// One network exception that is not covered by this exception is request timeout.
	/// </remarks>
	public class SnmpNetworkException: SnmpException
	{
		private Exception _systemException;
		/// <summary>
		/// Return system exception that caused raising of this Exception error.
		/// </summary>
		public System.Exception SystemException
		{
			get { return _systemException; }
		}
		/// <summary>
		/// Standard constructor
		/// </summary>
		/// <param name="sysException">System exception that caused the error</param>
		/// <param name="msg">Error message</param>
		public SnmpNetworkException(Exception sysException, string msg)
			: base(msg)
		{
			_systemException = sysException;
		}
		/// <summary>
		/// Constructor. Used when system exception did not cause the error and there is no parent
		/// exception associated with the error.
		/// </summary>
		/// <param name="msg">Error message</param>
		public SnmpNetworkException(string msg)
			: base(msg)
		{
			_systemException = null;
		}
	}
}

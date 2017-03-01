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
	/// Exception thrown by <see cref="SimpleSnmp"/> methods when SNMP request returned a SnmpStatus error in the reply and
	/// SuppressExceptions flag is set to false.
	/// </summary>
	public class SnmpErrorStatusException: Exception
	{
		/// <summary>
		/// SNMP reply ErrorStatus value
		/// </summary>
		protected int _errorStatus;
		/// <summary>
		/// SNMP reply ErrorIndex value
		/// </summary>
		protected int _errorIndex;
		/// <summary>
		/// Constructor
		/// </summary>
		public SnmpErrorStatusException()
			: base()
		{
			_errorStatus = 0;
			_errorIndex = 0;
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="msg">Exception message</param>
		/// <param name="status">ErrorStatus value</param>
		/// <param name="index">ErrorIndex value</param>
		public SnmpErrorStatusException(string msg, int status, int index)
			: base(msg)
		{
			_errorStatus = status;
			_errorIndex = index;
		}
		/// <summary>
		/// Get/Set SNMP ErrorStatus value
		/// </summary>
		public int ErrorStatus 
		{
			get { return _errorStatus; }
			set { _errorStatus = value; }
		}
		/// <summary>
		/// Get/Set SNMP ErrorIndex value
		/// </summary>
		public int ErrorIndex
		{
			get { return _errorIndex; }
			set { _errorIndex = value; }
		}
		/// <summary>
		/// Get exception message
		/// </summary>
		public override string Message
		{
			get
			{
				return String.Format("{0}> ErrorStatus {1} ErrorIndex {2}", base.Message, _errorStatus, _errorIndex);
			}
		}
	}
}

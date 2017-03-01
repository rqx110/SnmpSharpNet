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
	/// SNMP generic exception. Thrown every time SNMP specific error is encountered.
	/// </summary>
	[Serializable]
	public class SnmpException : Exception
	{
		/// <summary>
		/// No error
		/// </summary>
		public static int None = 0;
		/// <summary>
		/// Security model specified in the packet is not supported
		/// </summary>
		public static int UnsupportedSecurityModel = 1;
		/// <summary>
		/// Privacy enabled without authentication combination in a packet is not supported.
		/// </summary>
		public static int UnsupportedNoAuthPriv = 2;
		/// <summary>
		/// Invalid length of the authentication parameter field. Expected length is 12 bytes when authentication is
		/// enabled. Same length is used for both MD5 and SHA-1 authentication protocols.
		/// </summary>
		public static int InvalidAuthenticationParameterLength = 3;
		/// <summary>
		/// Authentication of the received packet failed.
		/// </summary>
		public static int AuthenticationFailed = 4;
		/// <summary>
		/// Privacy protocol requested is not supported.
		/// </summary>
		public static int UnsupportedPrivacyProtocol = 5;
		/// <summary>
		/// Invalid length of the privacy parameter field. Expected length depends on the privacy protocol. This exception
		/// can be raised when privacy packet contents are invalidly set by agent or if wrong privacy protocol is set in the
		/// packet class definition.
		/// </summary>
		public static int InvalidPrivacyParameterLength = 6;
		/// <summary>
		/// Authoritative engine id is invalid.
		/// </summary>
		public static int InvalidAuthoritativeEngineId = 7;
		/// <summary>
		/// Engine boots value is invalid
		/// </summary>
		public static int InvalidEngineBoots = 8;
		/// <summary>
		/// Received packet is outside the time window acceptable. Packet failed timeliness check.
		/// </summary>
		public static int PacketOutsideTimeWindow = 9;
		/// <summary>
		/// Invalid request id in the packet.
		/// </summary>
		public static int InvalidRequestId = 10;
		/// <summary>
		/// SNMP version 3 maximum message size exceeded. Packet that was encoded will exceed maximum message
		/// size acceptable in this transaction.
		/// </summary>
		public static int MaximumMessageSizeExceeded = 11;
		/// <summary>
		/// UdpTarget request cannot be processed because IAgentParameters does not contain required information
		/// </summary>
		public static int InvalidIAgentParameters = 12;

		/// <summary>
		/// Reply to a request was not received within the timeout period
		/// </summary>
		public static int RequestTimedOut = 13;
		/// <summary>
		/// Null data received on request.
		/// </summary>
		public static int NoDataReceived = 14;
		/// <summary>
		/// Security name (user name) in the reply does not match the name sent in request.
		/// </summary>
		public static int InvalidSecurityName = 15;
		/// <summary>
		/// Report packet was received when Reportable flag was set to false (we notified the peer that we do
		/// not receive report packets).
		/// </summary>
		public static int ReportOnNoReports = 16;
		/// <summary>
		/// Oid value type returned by an earlier operation does not match the value type returned by a subsequent entry.
		/// </summary>
		public static int OidValueTypeChanged = 17;
		/// <summary>
		/// Specified Oid is invalid
		/// </summary>
		public static int InvalidOid = 18;

		/// <summary>
		/// Error code. Provides a finer grained information about why the exception happened. This can be useful to
		/// the process handling the error to determine how critical the error that occured is and what followup actions
		/// to take.
		/// </summary>
		protected int _errorCode;
		/// <summary>
		/// Get/Set error code associated with the exception
		/// </summary>
		public int ErrorCode
		{
			get { return _errorCode; }
			set { _errorCode = value; }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SnmpException()
			: base()
		{
		}
		/// <summary>
		/// Standard constructor
		/// </summary>
		/// <param name="msg">SNMP Exception message</param>
		public SnmpException(string msg)
			: base(msg)
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="errorCode">Error code associated with the exception</param>
		/// <param name="msg">Error message</param>
		public SnmpException(int errorCode, string msg)
			: base(msg)
		{
			_errorCode = errorCode;
		}
	}
}

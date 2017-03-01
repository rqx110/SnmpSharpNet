using System;
using System.Collections.Generic;
using System.Text;

namespace SnmpSharpNet
{
	/// <summary>
	/// Result codes sent by UdpTarget class to the SnmpAsyncCallback delegate.
	/// </summary>
	public enum AsyncRequestResult
	{
		/// <summary>
		/// No error. Data was received from the socket.
		/// </summary>
		NoError = 0,
		/// <summary>
		/// Request is in progress. A new request can not be initiated until previous request completes.
		/// </summary>
		RequestInProgress,
		/// <summary>
		/// Request has timed out. Maximum number of retries has been reached without receiving a reply
		/// from the peer request was sent to
		/// </summary>
		Timeout,
		/// <summary>
		/// An error was encountered when attempting to send data to the peer. Request failed.
		/// </summary>
		SocketSendError,
		/// <summary>
		/// An error was encountered when attempting to receive data from the peer. Request failed.
		/// </summary>
		SocketReceiveError,
		/// <summary>
		/// Request has been terminated by the user.
		/// </summary>
		Terminated,
		/// <summary>
		/// No data was received from the peer
		/// </summary>
		NoDataReceived,
		/// <summary>
		/// Authentication error
		/// </summary>
		AuthenticationError,
		/// <summary>
		/// Privacy error
		/// </summary>
		PrivacyError,
		/// <summary>
		/// Error encoding SNMP packet
		/// </summary>
		EncodeError,
		/// <summary>
		/// Error decoding SNMP packet
		/// </summary>
		DecodeError
	}
}

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
using System.Net;
using System.Threading;

namespace SnmpSharpNet
{
	/// <summary>
	/// Internal class holding relevant information for async requests.
	/// </summary>
	internal class AsyncRequestState
	{
		/// <summary>
		/// Peer end point
		/// </summary>
		protected IPEndPoint _endPoint;
		/// <summary>
		/// Packet
		/// </summary>
		protected byte[] _packet;
		/// <summary>
		/// Packet length
		/// </summary>
		protected int _packetLength;
		/// <summary>
		/// Maximum number of retries (0 = single request, no retries)
		/// </summary>
		protected int _maxRetries;
		/// <summary>
		/// Request timeout in milliseconds
		/// </summary>
		protected int _timeout;
		/// <summary>
		/// Timer class
		/// </summary>
		protected Timer _timer;
		/// <summary>
		/// Current retry count. Value represents the number of retries that have been sent
		/// excluding the original request.
		/// </summary>
		protected int _currentRetry;

		/// <summary>
		/// Get/Set end point
		/// </summary>
		public System.Net.IPEndPoint EndPoint
		{
			get { return _endPoint; }
			set { _endPoint = value; }
		}
		/// <summary>
		/// Get/Set packet buffer
		/// </summary>
		public byte[] Packet
		{
			get { return _packet; }
			set { _packet = value; }
		}
		/// <summary>
		/// Get/Set packet length value
		/// </summary>
		public int PacketLength
		{
			get { return _packetLength; }
			set { _packetLength = value; }
		}
		/// <summary>
		/// Get/Set maximum retry value
		/// </summary>
		public int MaxRetries
		{
			get { return _maxRetries; }
			set { _maxRetries = value; }
		}
		/// <summary>
		/// Get/Set timeout value
		/// </summary>
		public int Timeout
		{
			get { return _timeout; }
			set { _timeout = value; }
		}
		/// <summary>
		/// Get/Set timer class
		/// </summary>
		public System.Threading.Timer Timer
		{
			get { return _timer; }
			set { _timer = value; }
		}
		/// <summary>
		/// Get/Set current retry count
		/// </summary>
		public int CurrentRetry
		{
			get { return _currentRetry; }
			set { _currentRetry = value; }
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="peerIP">Peer IP address</param>
		/// <param name="peerPort">Peer UDP port number</param>
		/// <param name="maxretries">Maximum number of retries</param>
		/// <param name="timeout">Timeout value in milliseconds</param>
		public AsyncRequestState(IPAddress peerIP, int peerPort, int maxretries, int timeout)
		{
			_endPoint = new IPEndPoint(peerIP, peerPort);
			_maxRetries = maxretries;
			_timeout = timeout;
			// current retry value is set to -1 because we do not count the first request as a retry.
			_currentRetry = -1;
			_timer = null;
		}
	}
}

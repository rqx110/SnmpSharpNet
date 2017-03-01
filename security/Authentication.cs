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
	/// <summary>Authentication helper class</summary>
	/// <remarks>
	/// Helper class to make dealing with multiple (if 2 qualifies as multiple) authentication protocols in a
	/// transparent way.
	/// 
	/// Calling class keeps the authentication protocol selection (as defined on the agent) in an integer
	/// variable that can have 3 values: <see cref="AuthenticationDigests.None"/>, <see cref="AuthenticationDigests.MD5"/>, or
	/// <see cref="AuthenticationDigests.SHA1"/>. Using <see cref="Authentication.GetInstance"/>, calling method can
	/// get authentication protocol implementation class instance cast as <see cref="IAuthenticationDigest"/> interface
	/// and perform authentication operations (either authenticate outgoing packets to verify authentication of incoming packets)
	/// without needing to further care about which authentication protocol is used.
	/// 
	/// Example of how to use this class:
	/// <code>
	/// IAuthenticationDigest authenticationImplementation = Authentication.GetInstance(AuthenticationDigests.MD5);
	/// authenticationImplementation.authenticateIncomingMsg(...);
	/// authenticationImplementation.authenticateOutgoingMsg(...);
	/// </code>
	/// </remarks>
	public sealed class Authentication
	{

		/// <summary>
		/// Get instance of authentication protocol.
		/// </summary>
		/// <param name="authProtocol">Authentication protocol code. Available codes are <see cref="AuthenticationDigests.MD5"/>,
		/// <see cref="AuthenticationDigests.SHA1"/> or <see cref="AuthenticationDigests.None"/></param>
		/// <returns>Instance of the authentication protocol or null if unrecognized authentication protocol or value
		/// <see cref="AuthenticationDigests.None"/> is passed.</returns>
		public static IAuthenticationDigest GetInstance(AuthenticationDigests authProtocol)
		{
			if (authProtocol == AuthenticationDigests.MD5)
				return new AuthenticationMD5();
			else if (authProtocol == AuthenticationDigests.SHA1)
				return new AuthenticationSHA1();
			return null;
		}
		/// <summary>
		/// Constructor. Private to prevent the class from being instantiated.
		/// </summary>
		private Authentication()
		{
		}
	}
}

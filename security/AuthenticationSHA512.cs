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
using System.Security.Cryptography;

namespace SnmpSharpNet
{
	/// <summary>
	/// SHA-512 Authentication class.
	/// </summary>
	public class AuthenticationSHA512 : IAuthenticationDigest
	{
		private const int authenticationLength = 48;
		private const int digestLength = 64;
		/// <summary>
		/// Standard constructor.
		/// </summary>
		public AuthenticationSHA512()
		{
		}
		/// <summary>
		/// Authenticate packet and return authentication parameters value to the caller
		/// </summary>
		/// <param name="authenticationSecret">User authentication secret</param>
		/// <param name="engineId">SNMP agent authoritative engine id</param>
		/// <param name="wholeMessage">Message to authenticate</param>
		/// <returns>Authentication parameters value</returns>
		public byte[] authenticate(byte[] authenticationSecret, byte[] engineId, byte[] wholeMessage)
		{
			byte[] result = new byte[authenticationLength];
			byte[] authKey = PasswordToKey(authenticationSecret, engineId);
			HMACSHA512 sha = new HMACSHA512(authKey);
			byte[] hash = sha.ComputeHash(wholeMessage);
			// copy "authentication lenght" bytes of the hash into the wholeMessage
			for (int i = 0; i < authenticationLength; i++)
			{
				result[i] = hash[i];
			}
			sha.Clear(); // release resources
			return result;
		}
		/// <summary>
		/// Authenticate packet and return authentication parameters value to the caller
		/// </summary>
		/// <param name="authKey">Authentication key (not password)</param>
		/// <param name="wholeMessage">Message to authenticate</param>
		/// <returns>Authentication parameters value</returns>
		public byte[] authenticate(byte[] authKey, byte[] wholeMessage)
		{
			byte[] result = new byte[authenticationLength];
		
			HMACSHA512 sha = new HMACSHA512(authKey);
			byte[] hash = sha.ComputeHash(wholeMessage);
			// copy "authentication lenght" bytes of the hash into the wholeMessage
			for (int i = 0; i < authenticationLength; i++)
			{
				result[i] = hash[i];
			}
			sha.Clear(); // release resources
			return result;
		}
		/// <summary>
		/// Verifies correct SHA-512 authentication of the frame. Prior to calling this method, you have to extract authentication
		/// parameters from the wholeMessage and reset authenticationParameters field in the USM information block to 12 0x00
		/// values.
		/// </summary>
		/// <param name="userPassword">User password</param>
		/// <param name="engineId">Authoritative engine id</param>
		/// <param name="authenticationParameters">Extracted USM authentication parameters</param>
		/// <param name="wholeMessage">Whole message with authentication parameters zeroed (0x00) out</param>
		/// <returns>True if message authentication has passed the check, otherwise false</returns>
		public bool authenticateIncomingMsg(byte[] userPassword, byte[] engineId, byte[] authenticationParameters, MutableByte wholeMessage)
		{
			byte[] authKey = PasswordToKey(userPassword, engineId);
			HMACSHA512 sha = new HMACSHA512(authKey);
			byte[] hash = sha.ComputeHash(wholeMessage);
			MutableByte myhash = new MutableByte(hash, authenticationLength);
			sha.Clear(); // release resources
			if (myhash.Equals(authenticationParameters))
			{
				return true;
			}
			return false;
		}
		/// <summary>
		/// Verify SHA-512 authentication of a packet.
		/// </summary>
		/// <param name="authKey">Authentication key (not password)</param>
		/// <param name="authenticationParameters">Authentication parameters extracted from the packet being authenticated</param>
		/// <param name="wholeMessage">Entire packet being authenticated</param>
		/// <returns>True on authentication success, otherwise false</returns>
		public bool authenticateIncomingMsg(byte[] authKey, byte[] authenticationParameters, MutableByte wholeMessage)
		{
			HMACSHA512 sha = new HMACSHA512(authKey);
			byte[] hash = sha.ComputeHash(wholeMessage);
			MutableByte myhash = new MutableByte(hash, authenticationLength);
			sha.Clear(); // release resources
			if (myhash.Equals(authenticationParameters))
			{
				return true;
			}
			return false;
		}
		/// <summary>
		/// Convert user password to acceptable authentication key.
		/// </summary>
		/// <param name="userPassword">User password</param>
		/// <param name="engineID">Authoritative engine id</param>
		/// <returns>Localized authentication key</returns>
		/// <exception cref="SnmpAuthenticationException">Thrown when key length is less then 8 bytes</exception>
		public byte[] PasswordToKey(byte[] userPassword, byte[] engineID)
		{
			// key length has to be at least 8 bytes long (RFC3414)
			if (userPassword == null || userPassword.Length < 8)
				throw new SnmpAuthenticationException("Secret key is too short.");

			int password_index = 0;
			int count = 0;
			SHA512 sha = new SHA512CryptoServiceProvider();

			/* Use while loop until we've done 1 Megabyte */
			byte[] sourceBuffer = new byte[1048576];
			byte[] buf = new byte[64];
			while (count < 1048576)
			{
				for (int i = 0; i < 64; ++i)
				{
					// Take the next octet of the password, wrapping
					// to the beginning of the password as necessary.
					buf[i] = userPassword[password_index++ % userPassword.Length];
				}
				Buffer.BlockCopy(buf, 0, sourceBuffer, count, buf.Length);
				count += 64;
			}

			byte[] digest = sha.ComputeHash(sourceBuffer);

			MutableByte tmpbuf = new MutableByte();
			tmpbuf.Append(digest);
			tmpbuf.Append(engineID);
			tmpbuf.Append(digest);
			byte[] res = sha.ComputeHash(tmpbuf);
			sha.Clear(); // release resources
			return res;
		}

		/// <summary>
		/// Length of the digest generated by the authentication protocol
		/// </summary>
		public int DigestLength
		{
			get { return digestLength; }
		}

		/// <summary>
		/// Length of the authentification header generated by the authentication protocol
		/// </summary>
		public int AuthentificationHeaderLength
		{
			get { return authenticationLength; }
		}

		/// <summary>
		/// Return authentication protocol name
		/// </summary>
		public string Name
		{
			get { return "HMAC-SHA512"; }
		}
		/// <summary>
		/// Compute hash using authentication protocol.
		/// </summary>
		/// <param name="data">Data to hash</param>
		/// <param name="offset">Compute hash from the source buffer offset</param>
		/// <param name="count">Compute hash for source data length</param>
		/// <returns>Hash value</returns>
		public byte[] ComputeHash(byte[] data, int offset, int count)
		{
			SHA512 sha = new SHA512CryptoServiceProvider();
			byte[] res = sha.ComputeHash(data, offset, count);
			sha.Clear();
			return res;
		}
	}
}

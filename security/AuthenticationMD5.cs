﻿// This file is part of SNMP#NET.
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
using System.Text;
using System.Security.Cryptography;

namespace SnmpSharpNet
{
	/// <summary>
	/// MD5 Authentication class.
	/// </summary>
	public class AuthenticationMD5: IAuthenticationDigest
	{
		/// <summary>
		/// Standard constructor
		/// </summary>
		public AuthenticationMD5()
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
			byte[] result = new byte[12];
			byte[] authKey = PasswordToKey(authenticationSecret, engineId);
			HMACMD5 md5 = new HMACMD5(authKey);
			byte[] hash = md5.ComputeHash(wholeMessage);
			// copy 12 bytes of the hash into the wholeMessage
			Buffer.BlockCopy(hash, 0, result, 0, 12);
			return result;
		}

		/// <summary>
		/// Authenticate packet and return authentication parameters value to the caller
		/// </summary>
		/// <param name="authKey">Pre-generated authentication key</param>
		/// <param name="wholeMessage">Message being authenticated</param>
		/// <returns>Authentication parameters value</returns>
		public byte[] authenticate(byte[] authKey, byte[] wholeMessage)
		{
			byte[] result = new byte[12];
			HMACMD5 md5 = new HMACMD5(authKey);
			byte[] hash = md5.ComputeHash(wholeMessage);
			// copy 12 bytes of the hash into the wholeMessage
			Buffer.BlockCopy(hash, 0, result, 0, 12);
			return result;
		}

		/// <summary>
		/// Verifies correct MD5 authentication of the frame. Prior to calling this method, you have to extract authentication
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
			HMACMD5 md5 = new HMACMD5(authKey);
			byte[] hash = md5.ComputeHash(wholeMessage, 0, wholeMessage.Length);
			MutableByte myhash = new MutableByte(hash, 12);
			if (myhash.Equals(authenticationParameters))
			{
				return true;
			}
			return false;
		}
		/// <summary>
		/// Verify MD5 authentication of a packet.
		/// </summary>
		/// <param name="authKey">Authentication key (not password)</param>
		/// <param name="authenticationParameters">Authentication parameters extracted from the packet being authenticated</param>
		/// <param name="wholeMessage">Entire packet being authenticated</param>
		/// <returns>True on authentication success, otherwise false</returns>
		public bool authenticateIncomingMsg(byte[] authKey, byte[] authenticationParameters, MutableByte wholeMessage)
		{
			HMACMD5 md5 = new HMACMD5(authKey);
			byte[] hash = md5.ComputeHash(wholeMessage, 0, wholeMessage.Length);
			MutableByte myhash = new MutableByte(hash, 12);
			if (myhash.Equals(authenticationParameters))
			{
				return true;
			}
			return false;
		}
		/// <summary>
		/// Convert user password to acceptable authentication key.
		/// </summary>
		/// <param name="userPassword">Authentication password</param>
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
#if !NETCOREAPP11 && !NETSTANDARD15
            MD5 md5 = new MD5CryptoServiceProvider();
#else
            MD5 md5 = MD5.Create();
#endif

            byte[] sourceBuffer = new byte[1048576];
			byte[] buf = new byte[64];
			while (count < 1048576)
			{
				for (int i = 0; i < 64; ++i)
				{
					buf[i] = userPassword[password_index++ % userPassword.Length];
				}
				Buffer.BlockCopy(buf, 0, sourceBuffer, count, buf.Length);
				count += 64;
			}

			byte[] digest = md5.ComputeHash(sourceBuffer);

			MutableByte tmpbuf = new MutableByte();
			tmpbuf.Append(digest);
			tmpbuf.Append(engineID);
			tmpbuf.Append(digest);
			byte[] key =  md5.ComputeHash(tmpbuf);

			return key;
		}

		/// <summary>
		/// Length of the digest generated by the authentication protocol
		/// </summary>
		public int DigestLength
		{
			get { return 16; }
		}

		/// <summary>
		/// Return protocol name.
		/// </summary>
		public string Name
		{
			get { return "HMAC-MD5"; }
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
#if !NETCOREAPP11 && !NETSTANDARD15
            MD5 md5 = new MD5CryptoServiceProvider();
#else
            MD5 md5 = MD5.Create();
#endif
            byte[] res = md5.ComputeHash(data, offset, count);
#if !NETCOREAPP11 && !NETSTANDARD15
			md5.Clear(); // release resources
#else
            md5.Dispose();
#endif
            return res;
		}
	}
}

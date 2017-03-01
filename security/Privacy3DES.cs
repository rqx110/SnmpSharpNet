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
	/// <summary>TripleDES privacy protocol implementation class.</summary>
	/// <remarks>
	/// TripleDES privacy implementation is based on the Internet Draft proposal to the
	/// SNMPv3 Working Group titled: Extension to the User-Based Security Model (USM) to Support 
	/// Triple-DES EDE in "Outside" CBC Mode
	/// 
	/// High level, TripleDES privacy in SNMPv3 uses DES-EDE. What this means is that a key is generated
	/// that is 24 bytes long. This key is split into 3 * 8 byte keys suitable for use with DES. Keys
	/// are then used to perform ecryption, decryption and another encryption using DES. Additionally, each
	/// block is XORed with the previous block of encrypted data, or if working on the first block, IV value.
	/// 
	/// For details see draft-reeder-snmpv3-usm-3desede-00.txt.
	/// 
	/// Important: TripleDES privacy protocol is not based on a standard. This extension to the USM standard has
	/// been proposed and has expired without approval or move to the standards track. Some vendors have implemented
	/// this privacy protocol and for the completeness of the library, it has been included in SnmpSharpNet.
	/// 
	/// Troubleshooting of TripleDES encryption is difficult because of the low availability so if you find problems
	/// with the SnmpSharpNet implementation, please try to provide me with as much detail, both about your code and
	/// the type/version/mode of the agent you are working with.
	/// </remarks>
	public class Privacy3DES : IPrivacyProtocol
	{
		/// <summary>
		/// Internal salt value
		/// </summary>
		protected Int32 _salt;
		/// <summary>
		/// Standard constructor.
		/// </summary>
		public Privacy3DES()
		{
			Random rand = new Random();
			_salt = rand.Next();
		}
		/// <summary>
		/// Returns next salt value.
		/// </summary>
		/// <returns>32-bit integer salt value in network byte order (big endian)</returns>
		public int NextSalt()
		{
			if (_salt == Int32.MaxValue)
				_salt = 1;
			else
				_salt += 1;
			return _salt;
		}
		/// <summary>
		/// Encrypt ScopedPdu using TripleDES encryption protocol
		/// </summary>
		/// <param name="unencryptedData">Unencrypted ScopedPdu byte array</param>
		/// <param name="offset">Offset to start encryption</param>
		/// <param name="length">Length of data to encrypt</param>
		/// <param name="key">Encryption key. Key has to be at least 32 bytes is length</param>
		/// <param name="engineBoots">Authoritative engine boots value</param>
		/// <param name="engineTime">Authoritative engine time value.</param>
		/// <param name="privacyParameters">Privacy parameters out buffer. This field will be filled in with information
		/// required to decrypt the information. Output length of this field is 8 bytes and space has to be reserved
		/// in the USM header to store this information</param>
		/// <param name="authDigest">Authentication digest class reference. Used by TripleDES.</param>
		/// <returns>Encrypted byte array</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when encryption key is null or length of the encryption key is too short.</exception>
		public byte[] Encrypt(byte[] unencryptedData, int offset, int length, byte[] key, int engineBoots, int engineTime, out byte[] privacyParameters, IAuthenticationDigest authDigest)
		{
			privacyParameters = GetSalt(engineBoots);
			byte[] privParamHash = authDigest.ComputeHash(privacyParameters, 0, privacyParameters.Length);
			privacyParameters = new byte[8];
			Buffer.BlockCopy(privParamHash, 0, privacyParameters, 0, 8);
			byte[] iv = GetIV(key, privacyParameters);

			byte[] encryptedData = null;
			try
			{
				TripleDES tdes = new TripleDESCryptoServiceProvider();
				tdes.Mode = CipherMode.CBC;
				tdes.Padding = PaddingMode.None;
				// normalize key - generated key is 32 bytes long, we need 24 bytes to encrypt
				byte[] normKey = new byte[24];
				Buffer.BlockCopy(key, 0, normKey, 0, normKey.Length);
				ICryptoTransform transform = tdes.CreateEncryptor(normKey, iv);
				if ((length % 8) == 0)
				{
					encryptedData = transform.TransformFinalBlock(unencryptedData, offset, length);
				}
				else
				{
					byte[] tmpbuffer = new byte[8 * ((length / 8) + 1)];
					Buffer.BlockCopy(unencryptedData, offset, tmpbuffer, 0, length);
					encryptedData = transform.TransformFinalBlock(tmpbuffer, 0, tmpbuffer.Length);
				}
			}
			catch (Exception ex)
			{
				throw new SnmpPrivacyException(ex, "Exception was thrown while TripleDES privacy protocol was encrypting data\r\n" + ex.ToString());
			}
			return encryptedData;
		}
		/// <summary>
		/// Decrypt TripleDES encrypted ScopedPdu
		/// </summary>
		/// <param name="encryptedData">Source data buffer</param>
		/// <param name="offset">Offset within the buffer to start decryption process</param>
		/// <param name="length">Length of data to decrypt</param>
		/// <param name="key">Decryption key. Key length has to be 32 bytes in length or longer (bytes beyond 32 bytes are ignored).</param>
		/// <param name="engineBoots">Authoritative engine boots value</param>
		/// <param name="engineTime">Authoritative engine time value</param>
		/// <param name="privacyParameters">Privacy parameters extracted from USM header</param>
		/// <returns>Decrypted byte array</returns>
		/// <exception cref="ArgumentNullException">Thrown when encrypted data is null or length == 0</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when encryption key length is less then 32 byte or if privacy parameters
		/// argument is null or length other then 8 bytes</exception>
		public byte[] Decrypt(byte[] encryptedData, int offset, int length, byte[] key, int engineBoots, int engineTime, byte[] privacyParameters)
		{
			if ((length % 8) != 0)
				throw new ArgumentOutOfRangeException("encryptedData", "Encrypted data buffer has to be divisable by 8.");
			if (encryptedData == null || encryptedData.Length < 8)
				throw new ArgumentOutOfRangeException("encryptedData", "Encrypted data buffer is null or smaller then 8 bytes in length.");
			if (offset > encryptedData.Length || (offset + length) > encryptedData.Length)
				throw new ArgumentOutOfRangeException("offset", "Offset and length arguments point beyond the bounds of the encryptedData array.");
			if (key == null || key.Length < 32)
				throw new ArgumentOutOfRangeException("decryptionKey", "Minimum acceptable length of the decryption key is 32 bytes.");
			if (privacyParameters == null || privacyParameters.Length != 8)
				throw new ArgumentOutOfRangeException("privacyParameters", "Privacy parameters field is not 8 bytes long.");

			byte[] iv = GetIV(key, privacyParameters);

			byte[] decryptedData = null;
			try
			{
				TripleDES tdes = new TripleDESCryptoServiceProvider();
				tdes.Mode = CipherMode.CBC;
				tdes.Padding = PaddingMode.None;

				// normalize key - generated key is 32 bytes long, we need 24 bytes to encrypt
				byte[] normKey = new byte[24];
				Buffer.BlockCopy(key, 0, normKey, 0, normKey.Length);

				ICryptoTransform transform = tdes.CreateDecryptor(normKey, iv);
				decryptedData = transform.TransformFinalBlock(encryptedData, offset, length);
			}
			catch (Exception ex)
			{
				throw new SnmpPrivacyException(ex, "Exception was thrown while TripleDES privacy protocol was decrypting data.");
			}
			return decryptedData;
		}

		/// <summary>
		/// Returns minimum encryption/decryption key length. For TripleDES, returned value is 32.
		/// </summary>
		/// <remarks>
		/// TripleDES protocol requires a 24 byte encryption key and additional 8 bytes are used for generating the
		/// encryption IV.
		/// </remarks>
		public int MinimumKeyLength
		{
			get { return 32; }
		}

		/// <summary>
		/// Return maximum encryption/decryption key length. For TripleDES, returned value is 32
		/// </summary>
		/// <remarks>
		/// TripleDES protocol requires a 24 byte encryption key and additional 8 bytes are used for generating the
		/// encryption IV.
		/// </remarks>
		public int MaximumKeyLength
		{
			get { return 32; }
		}

		/// <summary>
		/// Returns the length of privacyParameters USM header field. For TripleDES, field length is 8.
		/// </summary>
		public int PrivacyParametersLength
		{
			get { return 8; }
		}

		/// <summary>
		/// Get final encrypted length
		/// </summary>
		/// <remarks>
		/// TripleDES performs encryption on 8 byte blocks so the final encrypted size will be a
		/// mulitiple of 8 with padding added to the end of the ScopedPdu if required.
		/// </remarks>
		/// <param name="scopedPduLength">BER encoded ScopedPdu data length</param>
		/// <returns>Length of encrypted byte array</returns>
		public int GetEncryptedLength(int scopedPduLength)
		{
			if (scopedPduLength % 8 == 0)
			{
				return scopedPduLength;
			}
			return 8 * ((scopedPduLength / 8) + 1);
		}

		/// <summary>
		/// Get TripleDES encryption salt value.
		/// </summary>
		/// <remarks>
		/// Salt value is generated by concatenating engineBoots value with
		/// the random integer value.
		/// </remarks>s
		/// <param name="engineBoots">SNMP engine boots value</param>
		/// <returns>Salt byte array 8 byte in length</returns>
		private byte[] GetSalt(int engineBoots)
		{
			byte[] salt = new byte[8]; // salt is 8 bytes
			int s = NextSalt();
			byte[] eb = BitConverter.GetBytes(engineBoots);
			// C# is little endian so reverse the values
			salt[3] = eb[0];
			salt[2] = eb[1];
			salt[1] = eb[2];
			salt[0] = eb[3];
			byte[] sl = BitConverter.GetBytes(s);
			salt[7] = sl[0];
			salt[6] = sl[1];
			salt[5] = sl[2];
			salt[4] = sl[3];
			return salt;
		}

		/// <summary>
		/// Generate IV from the privacy key and salt value returned by GetSalt method.
		/// </summary>
		/// <param name="privacyKey">16 byte privacy key</param>
		/// <param name="salt">Salt value returned by GetSalt method</param>
		/// <returns>IV value used in the encryption process</returns>
		/// <exception cref="SnmpPrivacyException">Thrown when privacy key is less then 16 bytes long.</exception>
		private byte[] GetIV(byte[] privacyKey, byte[] salt)
		{
			if (privacyKey.Length < 32)
				throw new SnmpPrivacyException("Invalid privacy key length");
			byte[] iv = new byte[8];
			for (int i = 0; i < iv.Length; i++)
			{
				iv[i] = (byte)(salt[i] ^ privacyKey[24 + i]);
			}
			return iv;
		}
		/// <summary>
		/// Privacy protocol name
		/// </summary>
		public string Name
		{
			get { return "TripleDES"; }
		}

		/// <summary>
		/// Extends the encryption key if key size returned by PasswordToKey is less then minimum
		/// required by the encryption protocol.
		/// </summary>
		/// <remarks>
		/// There is no need to call this method in a user application becuase PasswordToKey() method will
		/// make the call if password it generates is too short.
		/// </remarks>
		/// <param name="shortKey">Encryption key</param>
		/// <param name="password">Privacy password</param>
		/// <param name="engineID">Authoritative engine id</param>
		/// <param name="authProtocol">Authentication protocol class instance</param>
		/// <returns>unaltered shortKey value</returns>
		public byte[] ExtendShortKey(byte[] shortKey, byte[] password, byte[] engineID, IAuthenticationDigest authProtocol)
		{
			int length = shortKey.Length;
			byte[] extendedKey = new byte[MinimumKeyLength];
			Buffer.BlockCopy(shortKey, 0, extendedKey, 0, shortKey.Length);

			while (length < MinimumKeyLength)
			{
				byte[] key =
					authProtocol.PasswordToKey(shortKey,
											   engineID);
				int copyBytes = Math.Min(MaximumKeyLength - length,
										 authProtocol.DigestLength);
				Buffer.BlockCopy(key, 0, extendedKey, length, copyBytes);
				length += copyBytes;
			}
			return extendedKey;
		}

		/// <summary>
		/// TripleDES implementation supports extending of a short encryption key. Always returns true.
		/// </summary>
		public bool CanExtendShortKey
		{
			get { return true; }
		}

		/// <summary>
		/// Convert privacy password into encryption key using packet authentication hash.
		/// </summary>
		/// <param name="secret">Privacy user secret/password</param>
		/// <param name="engineId">Authoritative engine id of the SNMP agent</param>
		/// <param name="authProtocol">Authentication protocol</param>
		/// <returns>Encryption key</returns>
		/// <exception cref="SnmpPrivacyException">Thrown when user secret/password is shorter then MinimumKeyLength</exception>
		public byte[] PasswordToKey(byte[] secret, byte[] engineId, IAuthenticationDigest authProtocol)
		{
			// RFC 3414 - password length is minimum of 8 bytes long
			if (secret == null || secret.Length < 8)
				throw new SnmpPrivacyException("Invalid privacy secret length.");
			byte[] encryptionKey = authProtocol.PasswordToKey(secret, engineId);
			if( encryptionKey.Length < MinimumKeyLength )
				encryptionKey = ExtendShortKey(encryptionKey, secret, engineId, authProtocol);
			return encryptionKey;
		}
	}
}

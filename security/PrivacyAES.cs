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
using System.Net;

namespace SnmpSharpNet
{
	/// <summary>AES privacy protocol implementation class.</summary>
	public class PrivacyAES : IPrivacyProtocol
	{
		/// <summary>
		/// Salt value
		/// </summary>
		protected Int64 _salt = 0;
		/// <summary>
		/// AES protocol key bytes. Valid values are 16 (for AES128), 24 (AES192) or 32 (AES256).
		/// </summary>
		protected int _keyBytes = 16; // Default is 128bit AES protocol

		/// <summary>
		/// Standard constructor.
		/// </summary>
		/// <param name="keyBytes">Key size in bytes. Acceptable values are
		/// 16 = 128-bit key size, 24 = 192-bit key size, or 32 = 256-bit key size.</param>
		public PrivacyAES(int keyBytes)
		{
			if (keyBytes != 16 && keyBytes != 24 && keyBytes != 32)
				throw new ArgumentOutOfRangeException("keyBytes", "Valid key sizes are 16, 24 and 32 bytes.");
			_keyBytes = keyBytes;
			// initialize salt generator
			Random rand = new Random();
			_salt = Convert.ToInt64(rand.Next(1, Int32.MaxValue));
		}

		/// <summary>
		/// Get next salt Int64 value. Used internally to encrypt data.
		/// </summary>
		/// <returns>Random Int64 value</returns>
		protected Int64 NextSalt()
		{
			if (_salt == Int64.MaxValue)
				_salt = 1;
			else
				_salt += 1;
			return _salt;
		}

		/// <summary>
		/// Encrypt <see cref="ScopedPdu"/> data BER encoded in a byte array.
		/// </summary>
		/// <param name="unencryptedData">BER encoded <see cref="ScopedPdu"/> byte array that needs to be encrypted</param>
		/// <param name="offset">Offset within the BER encoded byte array to start encryption operation from.</param>
		/// <param name="length">Length of data to encrypt</param>
		/// <param name="key">Encryption key</param>
		/// <param name="engineBoots">Authoritative engine boots value. Retrieved as part of SNMP v3 discovery process.</param>
		/// <param name="engineTime">Authoritative engine time value. Retrieved as part of SNMP v3 discovery process.</param>
		/// <param name="privacyParameters">Byte array that will receive privacy parameters information that is the result of the
		/// encryption procedure.</param>
		/// <param name="authDigest">Authentication digest reference. Not used by AES protocol and can be null</param>
		/// <returns>Byte array containing encrypted <see cref="ScopedPdu"/> BER encoded data</returns>
		public byte[] Encrypt(byte[] unencryptedData, int offset, int length, byte[] key, int engineBoots, int engineTime, out byte[] privacyParameters, IAuthenticationDigest authDigest)
		{
			// check the key before doing anything else
			if (key == null || key.Length < _keyBytes)
				throw new ArgumentOutOfRangeException("encryptionKey", "Invalid key length");

			byte[] iv = new byte[16];
			Int64 salt = NextSalt();
			privacyParameters = new byte[PrivacyParametersLength];
			byte[] bootsBytes = BitConverter.GetBytes(engineBoots);
			iv[0] = bootsBytes[3];
			iv[1] = bootsBytes[2];
			iv[2] = bootsBytes[1];
			iv[3] = bootsBytes[0];
			byte[] timeBytes = BitConverter.GetBytes(engineTime);
			iv[4] = timeBytes[3];
			iv[5] = timeBytes[2];
			iv[6] = timeBytes[1];
			iv[7] = timeBytes[0];

			// Set privacy parameters to the local 64 bit salt value
			byte[] saltBytes = BitConverter.GetBytes(salt);
			privacyParameters[0] = saltBytes[7];
			privacyParameters[1] = saltBytes[6];
			privacyParameters[2] = saltBytes[5];
			privacyParameters[3] = saltBytes[4];
			privacyParameters[4] = saltBytes[3];
			privacyParameters[5] = saltBytes[2];
			privacyParameters[6] = saltBytes[1];
			privacyParameters[7] = saltBytes[0];

			// Copy salt value to the iv array
			Buffer.BlockCopy(privacyParameters, 0, iv, 8, 8);

			Rijndael rm = new RijndaelManaged();
			rm.KeySize = _keyBytes * 8;
			rm.FeedbackSize = 128;
			rm.BlockSize = 128;
			// we have to use Zeros padding otherwise we get encrypt buffer size exception
			rm.Padding = PaddingMode.Zeros;
			rm.Mode = CipherMode.CFB;
			// make sure we have the right key length
			byte[] pkey = new byte[MinimumKeyLength];
			Buffer.BlockCopy(key, 0, pkey, 0, MinimumKeyLength);
			rm.Key = pkey;
			rm.IV = iv;
			ICryptoTransform cryptor = rm.CreateEncryptor();
			byte[] encryptedData = cryptor.TransformFinalBlock(unencryptedData, offset, length);
			// check if encrypted data is the same length as source data
			if (encryptedData.Length != unencryptedData.Length)
			{
				// cut out the padding
				byte[] tmp = new byte[unencryptedData.Length];
				Buffer.BlockCopy(encryptedData, 0, tmp, 0, unencryptedData.Length);
				return tmp;
			}
			return encryptedData;
		}

		/// <summary>
		/// Decrypt <see cref="ScopedPdu"/> BER encoded byte array.
		/// </summary>
		/// <param name="cryptedData">Encrypted data byte array</param>
		/// <param name="offset">Offset within the buffer to start decryption process from</param>
		/// <param name="length">Length of data to decrypt</param>
		/// <param name="key">Decryption key</param>
		/// <param name="engineBoots">Authoritative engine boots value. Retrieved as part of SNMP v3 discovery procedure</param>
		/// <param name="engineTime">Authoritative engine time value. Retrieved as part of SNMP v3 discovery procedure</param>
		/// <param name="privacyParameters">Privacy parameters parsed from the incoming packet.</param>
		/// <returns>Byte array containing decrypted <see cref="ScopedPdu"/> in BER encoded format.</returns>
		public byte[] Decrypt(byte[] cryptedData, int offset, int length, byte[] key, int engineBoots, int engineTime, byte[] privacyParameters)
		{
			if (key == null || key.Length < _keyBytes)
				throw new ArgumentOutOfRangeException("decryptionKey", "Invalid key length");

			byte[] iv = new byte[16];

			byte[] bootsBytes = BitConverter.GetBytes(engineBoots);
			iv[0] = bootsBytes[3];
			iv[1] = bootsBytes[2];
			iv[2] = bootsBytes[1];
			iv[3] = bootsBytes[0];
			byte[] timeBytes = BitConverter.GetBytes(engineTime);
			iv[4] = timeBytes[3];
			iv[5] = timeBytes[2];
			iv[6] = timeBytes[1];
			iv[7] = timeBytes[0];

			// Copy salt value to the iv array
			Buffer.BlockCopy(privacyParameters, 0, iv, 8, 8);

			byte[] decryptedData = null;

			// now do CFB decryption of the encrypted data
			Rijndael rm = Rijndael.Create();
			rm.KeySize = _keyBytes * 8;
			rm.FeedbackSize = 128;
			rm.BlockSize = 128;
			rm.Padding = PaddingMode.Zeros;
			rm.Mode = CipherMode.CFB;
			if (key.Length > MinimumKeyLength)
			{
				byte[] normKey = new byte[MinimumKeyLength];
				Buffer.BlockCopy(key, 0, normKey, 0, MinimumKeyLength);
				rm.Key = normKey;
			}
			else
			{
				rm.Key = key;
			}
			rm.IV = iv;
			System.Security.Cryptography.ICryptoTransform cryptor;
			cryptor = rm.CreateDecryptor();

			// We need to make sure that cryptedData is a collection of 128 byte blocks
			if ((cryptedData.Length % _keyBytes) != 0)
			{
				byte[] buffer = new byte[length];
				Buffer.BlockCopy(cryptedData, offset, buffer, 0, length);
				int div = (int)Math.Floor(buffer.Length / (double)16);
				int newLength = (div + 1) * 16;
				byte[] decryptBuffer = new byte[newLength];
				Buffer.BlockCopy(buffer, 0, decryptBuffer, 0, buffer.Length);
				decryptedData = cryptor.TransformFinalBlock(decryptBuffer, 0, decryptBuffer.Length);
				// now remove padding
				Buffer.BlockCopy(decryptedData, 0, buffer, 0, length);
				return buffer;
			}

			decryptedData = cryptor.TransformFinalBlock(cryptedData, offset, length);

			return decryptedData;
		}

		/// <summary>
		/// Get minimum encryption/decryption key length required by the protocol.
		/// </summary>
		public int MinimumKeyLength
		{
			get { return _keyBytes; }
		}

		/// <summary>
		/// Get maximum encryption/decryption key length required by the protocol.
		/// </summary>
		public int MaximumKeyLength
		{
			get { return _keyBytes; }
		}

		/// <summary>
		/// Get length of the privacy parameters byte array that is generated by the encryption method and used by the
		/// decryption method.
		/// </summary>
		public int PrivacyParametersLength
		{
			get { return 8; }
		}

		/// <summary>
		/// Calculates and returns length of the buffer that is the result of the encryption method.
		/// </summary>
		/// <param name="scopedPduLength">Length of the buffer that is needs to be encrypted.</param>
		/// <returns>Length of the encrypted byte array after the call to Encrypt method.</returns>
		public int GetEncryptedLength(int scopedPduLength)
		{
			return scopedPduLength;
		}

		/// <summary>
		/// Some protocols support a method to extend the encryption or decryption key when supplied key
		/// is too short.
		/// </summary>
		/// <param name="shortKey">Key that needs to be extended</param>
		/// <param name="password">Privacy password as configured on the SNMP agent.</param>
		/// <param name="engineID">Authoritative engine id. Value is retrieved as part of SNMP v3 discovery procedure</param>
		/// <param name="authProtocol">Authentication protocol class instance cast as <see cref="IAuthenticationDigest"/></param>
		/// <returns>Extended key value</returns>
		public byte[] ExtendShortKey(byte[] shortKey, byte[] password, byte[] engineID, IAuthenticationDigest authProtocol)
		{
			byte[] extKey = new byte[MinimumKeyLength];
			byte[] lastKeyBuf = new byte[shortKey.Length];
			Array.Copy(shortKey, lastKeyBuf, shortKey.Length);
			int keyLen = shortKey.Length > MinimumKeyLength ? MinimumKeyLength : shortKey.Length;
			Array.Copy(shortKey, extKey, keyLen);
			while (keyLen < MinimumKeyLength)
			{
				byte[] tmpBuf = authProtocol.PasswordToKey(lastKeyBuf, engineID);
				if (tmpBuf == null)
				{
					return null;
				}
				if (tmpBuf.Length <= (MinimumKeyLength - keyLen))
				{
					Array.Copy(tmpBuf, 0, extKey, keyLen, tmpBuf.Length);
					keyLen += tmpBuf.Length;
				}
				else
				{
					Array.Copy(tmpBuf, 0, extKey, keyLen, MinimumKeyLength - keyLen);
					keyLen += (MinimumKeyLength - keyLen);
				}
				lastKeyBuf = new byte[tmpBuf.Length];
				Array.Copy(tmpBuf, lastKeyBuf, tmpBuf.Length);
			}
			return extKey;
		}

		/// <summary>
		/// Privacy protocol name. Returns string "AES"
		/// </summary>
		public virtual string Name
		{
			get { return "AES"; }
		}

		/// <summary>
		/// AES implementation supports extending of a short encryption key. Always returns true.
		/// </summary>
		public bool CanExtendShortKey
		{
			get
			{
				return true;
			}
		}
		/// <summary>
		/// Convert privacy password into encryption key using packet authentication hash.
		/// </summary>
		/// <param name="secret">Privacy user secret</param>
		/// <param name="engineId">Authoritative engine id of the snmp agent</param>
		/// <param name="authProtocol">Authentication protocol</param>
		/// <returns>Encryption key</returns>
		/// <exception cref="SnmpPrivacyException">Thrown when key size is shorter then MinimumKeyLength</exception>
		public byte[] PasswordToKey(byte[] secret, byte[] engineId, IAuthenticationDigest authProtocol)
		{
			if (secret == null || secret.Length < 8)
				throw new SnmpPrivacyException("Invalid privacy secret length.");
			byte[] key = authProtocol.PasswordToKey(secret, engineId);
			if (key.Length < MinimumKeyLength)
			{
				key = ExtendShortKey(key, secret, engineId, authProtocol);
			}
			return key;
		}
	}
}

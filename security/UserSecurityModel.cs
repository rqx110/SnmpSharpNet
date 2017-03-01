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
/* Changes:
 *  Dec/05, 2008
 *   Added user name comparison in decode method
 */
using System;

namespace SnmpSharpNet
{
	/// <summary>
	/// User security model implementation class.
	/// </summary>
	public class UserSecurityModel : AsnType, ICloneable
	{
		/// <summary>
		/// Authoritative engine id
		/// </summary>
		protected OctetString _engineId;
		/// <summary>
		/// Authoritative engine boots value
		/// </summary>
		protected Integer32 _engineBoots;
		/// <summary>
		/// Authoritative engine time value
		/// </summary>
		protected Integer32 _engineTime;
		/// <summary>
		/// SNMP version 3 security name (or user name)
		/// </summary>
		protected OctetString _securityName;
		/// <summary>
		/// Authentication digest enumeration value. For acceptable values see <see cref="AuthenticationDigests"/>
		/// </summary>
		protected AuthenticationDigests _authentication;
		/// <summary>
		/// Authentication secret
		/// </summary>
		protected MutableByte _authenticationSecret;
		/// <summary>
		/// Authentication parameters in authNoPriv and authPriv requests
		/// </summary>
		private OctetString _authenticationParameters;
		/// <summary>
		/// Privacy protocol. For valid values see <see cref="PrivacyProtocols"/>
		/// </summary>
		protected PrivacyProtocols _privacy;
		/// <summary>
		/// Privacy secret
		/// </summary>
		protected MutableByte _privacySecret;
		/// <summary>
		/// Privacy parameters in authPriv requests
		/// </summary>
		protected OctetString _privacyParameters;

		/// <summary>
		/// Get SNMP version 3 agent authoritative engine id object
		/// </summary>
		public OctetString EngineId
		{
			get
			{
				return _engineId;
			}
		}
		/// <summary>
		/// Get SNMP version 3 agent authoritative engine boots object
		/// </summary>
		public Int32 EngineBoots
		{
			get { return _engineBoots.Value; }
			set { _engineBoots.Value = value; }
		}

		/// <summary>
		/// Get SNMP version 3 agent authoritative engine time object
		/// </summary>
		public Int32 EngineTime
		{
			get { return _engineTime.Value; }
			set { _engineTime.Value = value; }
		}

		/// <summary>
		/// Get SNMP version 3 authentication parameter field object
		/// </summary>
		public OctetString AuthenticationParameters
		{
			get { return _authenticationParameters; }
		}

		/// <summary>
		/// Set SNMP version 3 agent engine time related values.
		/// </summary>
		/// <param name="engineTime">SNMP version 3 agent engine time value</param>
		/// <param name="engineBoots">SNMP version 3 agent engine boot value</param>
		public void SetEngineTime(Int32 engineTime, Int32 engineBoots)
		{
			_engineTime.Value = engineTime;
			_engineBoots.Value = engineBoots;
		}
		/// <summary>
		/// Get/Set hash to use for SNMP version 3 authentication. For available values see <see cref="AuthenticationDigests"/>
		/// </summary>
		public AuthenticationDigests Authentication
		{
			get 
			{
				return _authentication; 
			}
			set
			{
				_authentication = value;
			}
		}
		/// <summary>
		/// Security name (or user name)
		/// </summary>
		public OctetString SecurityName
		{
			get
			{
				return _securityName;
			}
		}

		/// <summary>
		/// Authentication secret (or password).
		/// </summary>
		public MutableByte AuthenticationSecret
		{
			get { return _authenticationSecret; }
		}

		/// <summary>
		/// Privacy secret (or password)
		/// </summary>
		public MutableByte PrivacySecret
		{
			get { return _privacySecret; }
		}

		/// <summary>
		/// Get/set privacy protocol value. For available privacy protocols, see <see cref="PrivacyProtocols"/>
		/// enumeration.
		/// </summary>
		public PrivacyProtocols Privacy
		{
			get { return _privacy; }
			set { _privacy = value; }
		}

		/// <summary>
		/// Get privacy parameters object.
		/// </summary>
		public OctetString PrivacyParameters
		{
			get { return _privacyParameters; }
		}
		/// <summary>
		/// Standard constructor.
		/// </summary>
		public UserSecurityModel()
		{
			_asnType = 3;
			_engineId = new OctetString();
			_engineBoots = new Integer32();
			_engineTime = new Integer32();
			_authentication = AuthenticationDigests.None;

			_securityName = new OctetString();
			_authenticationSecret = new MutableByte();
			_authenticationParameters = new OctetString();
			_privacySecret = new MutableByte();
			_privacy = PrivacyProtocols.None;
			_privacyParameters = new OctetString();
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="value">Class to copy values from</param>
		public UserSecurityModel(UserSecurityModel value)
			: this()
		{
			_engineId.Set(value.EngineId);
			_engineBoots.Value = value.EngineBoots;
			_engineTime.Value = value.EngineTime;
			_securityName.Set(value.SecurityName);
			_authenticationParameters = new OctetString();
			_privacySecret = new MutableByte();
			_privacy = PrivacyProtocols.None;
			_privacyParameters = new OctetString();
		}

		/// <summary>
		/// Authenticate SNMP version 3 message.
		/// 
		/// Before calling this member, entire SNMP version 3 packet needs to be encoded. After authentication
		/// process is completed, authenticationParameters value in the USM header is updated and SNMPv3 packet
		/// needs to be re-encoded to include it in the BER encoded stream prior to transmission.
		/// </summary>
		/// <param name="wholePacket">SNMP version 3 BER encoded packet.</param>
		public void Authenticate(ref MutableByte wholePacket)
		{
			if (_authentication != AuthenticationDigests.None)
			{
				IAuthenticationDigest authProto = SnmpSharpNet.Authentication.GetInstance(_authentication);
				byte[] authParam = authProto.authenticate(AuthenticationSecret, EngineId.ToArray(), wholePacket);
				_authenticationParameters = new OctetString(authParam);
			}
		}
		/// <summary>
		/// Authenticate SNMP version 3 message.
		/// 
		/// Before calling this member, entire SNMP version 3 packet needs to be encoded. After authentication
		/// process is completed, authenticationParameters value in the USM header is updated and SNMPv3 packet
		/// needs to be re-encoded to include it in the BER encoded stream prior to transmission.
		/// </summary>
		/// <param name="authKey">Authentication key (not password)</param>
		/// <param name="wholePacket">SNMP version 3 BER encoded packet.</param>
		public void Authenticate(byte[] authKey, ref MutableByte wholePacket)
		{
			IAuthenticationDigest authProto = SnmpSharpNet.Authentication.GetInstance(_authentication);
			byte[] authParam = authProto.authenticate(authKey, wholePacket);
			_authenticationParameters = new OctetString(authParam);
		}
		/// <summary>
		/// Authenticate incoming packet
		/// </summary>
		/// <param name="wholePacket">Received BER encoded SNMP version 3 packet</param>
		/// <returns>True if packet is successfully authenticated, otherwise false.</returns>
		public bool IsAuthentic(MutableByte wholePacket)
		{
			if (_authentication != AuthenticationDigests.None)
			{
				IAuthenticationDigest authProto = SnmpSharpNet.Authentication.GetInstance(_authentication);
				if (authProto != null)
				{
					return authProto.authenticateIncomingMsg(AuthenticationSecret, _engineId, _authenticationParameters, wholePacket);
				}
			}
			return false; // Nothing to authenticate
		}

		/// <summary>
		/// Authenticate incoming packet
		/// </summary>
		/// <param name="authKey">Authentication key (not password)</param>
		/// <param name="wholePacket">Received BER encoded SNMP version 3 packet</param>
		/// <returns>True if packet is successfully authenticated, otherwise false.</returns>
		public bool IsAuthentic(byte[] authKey, MutableByte wholePacket)
		{
			if (_authentication != AuthenticationDigests.None)
			{
				IAuthenticationDigest authProto = SnmpSharpNet.Authentication.GetInstance(_authentication);
				if (authProto != null)
				{
					return authProto.authenticateIncomingMsg(authKey, _authenticationParameters, wholePacket);
				}
			}
			return false; // Nothing to authenticate
		}

		/// <summary>BER encode security model field.</summary>
		/// <remarks>
		/// USM security model is a SEQUENCE encoded inside a OCTETSTRING. To encode it, first encode the sequence
		/// of class values then "wrap" it inside a OCTETSTRING field
		/// </remarks>
		/// <param name="buffer">Buffer to store encoded USM security model header</param>
		public override void encode(MutableByte buffer)
		{
			MutableByte tmp = new MutableByte();
			// First encode all the values that will form the sequence
			_engineId.encode(tmp);
			// Encode engine boots
			_engineBoots.encode(tmp);
			// encode engine time
			_engineTime.encode(tmp);
			_securityName.encode(tmp);
			if (_authentication != AuthenticationDigests.None)
			{
				if (_authenticationParameters.Length <= 0)
				{
					// If authentication is used, set authentication parameters field to 12 bytes set to 0x00
					_authenticationParameters.Set(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
				}
			}
			else
			{
				_authenticationParameters.Reset();
			}
			_authenticationParameters.encode(tmp);
			if (_privacy != PrivacyProtocols.None)
			{
				if (_privacyParameters.Length <= 0)
				{
					IPrivacyProtocol privProto = PrivacyProtocol.GetInstance(_privacy);
					if (privProto != null)
					{
						byte[] parameter = new byte[privProto.PrivacyParametersLength];
						for (int i = 0; i < privProto.PrivacyParametersLength; i++)
						{
							parameter[i] = 0x00; // This is not necessary since all array members are, by default, initialized to 0
						}
						_privacyParameters.Set(parameter);
					}
					else
						throw new SnmpException(SnmpException.UnsupportedPrivacyProtocol, "Unrecognized privacy protocol specified.");
				}
			}
			else
			{
				_privacyParameters.Reset();
			}
			_privacyParameters.encode(tmp);
			MutableByte tmp1 = new MutableByte();

			BuildHeader(tmp1, SnmpConstants.SMI_SEQUENCE, tmp.Length);
			tmp1.Append(tmp);

			BuildHeader(buffer, OCTETSTRING, tmp1.Length);

			buffer.Append(tmp1);
		}

		/// <summary>
		/// Decode USM portion of the SNMP version 3 packet.
		/// </summary>
		/// <param name="buffer">Received SNMP packet BER encoded</param>
		/// <param name="offset">Offset within the buffer to start decoding USM information</param>
		/// <returns>Buffer position after the decoded value</returns>
		/// <exception cref="SnmpDecodingException">Thrown when decoding enountered invalid data type in USM information</exception>
		/// <exception cref="OverflowException">Thrown when packet is too small to contain information length specified in header</exception>
		public override int decode(byte[] buffer, int offset)
		{
			// Grab the octet string header
			int len;
			byte type = ParseHeader(buffer, ref offset, out len);
			if( type != OCTETSTRING )
				throw new SnmpDecodingException("Invalid value type found while looking for USM header.");

			if (len > (buffer.Length - offset))
				throw new OverflowException("Packet too small");
			// Now grab the sequence header
			type = ParseHeader(buffer, ref offset, out len);
			if( type != SnmpConstants.SMI_SEQUENCE )
				throw new SnmpDecodingException("Sequence missing from USM header.");

			if (len > (buffer.Length - offset))
				throw new OverflowException("Packet too small");

			// now grab values one at the time
			offset = _engineId.decode(buffer, offset);

			offset = _engineBoots.decode(buffer, offset);

			offset = _engineTime.decode(buffer, offset);

			offset = _securityName.decode(buffer, offset);

			int saveOffset = offset;
			offset = _authenticationParameters.decode(buffer, offset);

			if (_authenticationParameters.Length > 0)
			{
				// walk through and set the authentication parameters to 0x00 in the packet

				saveOffset += 2; // Skip BER encoded variable type and length
				for (int i = 0; i < _authenticationParameters.Length; i++)
				{
					buffer[saveOffset + i] = 0x00;
				}
			}
			offset = _privacyParameters.decode(buffer, offset);
			return offset;
		}

		/// <summary>
		/// Clone object
		/// </summary>
		/// <returns>Duplicate copy of the object</returns>
		public override object Clone()
		{
			return new UserSecurityModel(this);
		}

		/// <summary>
		/// Checks for validity and completeness of information in this class. This method doesn't "know" what you
		/// are trying to do so it tests for minimal information required.
		/// </summary>
		/// <returns>True if information is valid and complete enough for a successful request, otherwise false</returns>
		public bool Valid()
		{
			if ((_authentication != AuthenticationDigests.None || _privacy != PrivacyProtocols.None) && _securityName.Length <= 0)
				return false; // Have to provide a user name when using authentication or privacy
			if (_authentication == AuthenticationDigests.None && _privacy != PrivacyProtocols.None)
				return false; // noAuthPriv is not supported by SNMP version 3
			// check that secrets are properly configured
			if (_authentication != AuthenticationDigests.None && _authenticationSecret.Length <= 0)
				return false; // authentication configured without a secret
			if (_privacy != PrivacyProtocols.None && _privacySecret.Length <= 0)
				return false; // privacy configured without a secret
			// no point in checking the rest since discovery process is done with all other values being null or 0
			return true;
		}

		/// <summary>
		/// Reset USM object to default values. All OctetString and MutableByte members are reset to 0 length and
		/// privacy and authentication protocols are set to none.
		/// </summary>
		public void Reset()
		{
			_asnType = 3;
			_engineId = new OctetString();
			_engineBoots = new Integer32();
			_engineTime = new Integer32();
			_authentication = AuthenticationDigests.None;

			_securityName = new OctetString();
			_authenticationSecret = new MutableByte();
			_authenticationParameters = new OctetString();
			_privacySecret = new MutableByte();
			_privacy = PrivacyProtocols.None;
			_privacyParameters = new OctetString();
		}
    }
}

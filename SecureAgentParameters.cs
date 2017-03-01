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
	/// <summary>Secure SNMPv3 agent parameters</summary>
	/// <remarks>
	/// SNMP Agent specific values. This class stores values to access SNMP version 3
	/// agents.
	/// 
	/// Pass this class with your request data (Pdu) to the request method of the target class to make
	/// a request.
	/// 
	/// Based on the information in this class, an appropriate request will be made by the request class.
	/// 
	/// Following request types are generated:
	/// 
	/// * if EngineBoots and EngineTime are integer value 0 or if EngineId value is length 0, Discovery
	/// request is made and passed instance of the SecureAgentParameters is updated with returned values.
	/// 
	/// * in all other cases, SNMP request is made to the agent
	/// </remarks>
	public class SecureAgentParameters : IAgentParameters
	{

		#region Variables

		// <summary>
		// Protocol version. Always == SnmpConstants.SNMPV3
		// </summary>
		// protected Integer32 _version;
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
		/// Time stamp when authoritative engine time value was last refreshed with data from the agent.
		/// 
		/// This value is used to calculate up to date authoritative agent time value without having to
		/// repeat discovery process every 150 seconds.
		/// </summary>
		protected DateTime _engineTimeStamp;
		/// <summary>
		/// Security name value, or user name.
		/// </summary>
		protected OctetString _securityName;
		/// <summary>
		/// Privacy protocol to use. For available protocols, see <see cref="PrivacyProtocols"/> enumeration.
		/// </summary>
		protected PrivacyProtocols _privacyProtocol;
		/// <summary>
		/// Authentication digest to use in authNoPriv and authPriv security combinations. For available
		/// authentication digests, see <see cref="AuthenticationDigests"/> enumeration.
		/// </summary>
		protected AuthenticationDigests _authenticationProtocol;
		/// <summary>
		/// Privacy secret (or privacy password)
		/// </summary>
		protected MutableByte _privacySecret;
		/// <summary>
		/// Authentication secret (or authentication password)
		/// </summary>
		protected MutableByte _authenticationSecret;
		/// <summary>
		/// Context engine id. By default, this value is set to authoritative engine id value unless specifically
		/// set to a different value here.
		/// </summary>
		protected OctetString _contextEngineId;
		/// <summary>
		/// Context name. By default this value is a 0 length string (no context name). Set this value if you
		/// require it to be defined in ScopedPdu.
		/// </summary>
		protected OctetString _contextName;
		/// <summary>
		/// Maximum message size. This value is by default set to 64KB and then updated by the maximum message
		/// size value in the response from the agent.
		/// 
		/// This value should be the smallest message size supported by both the agent and manager.
		/// </summary>
		protected Integer32 _maxMessageSize;
		/// <summary>
		/// Reportable option flag. Set to true by default.
		/// 
		/// This flag controls if reportable flag will be set in the packet. When this flag is set in the packet,
		/// agent will respond to invalid requests with Report packets. Without this flag being set, all invalid
		/// requests are silently dropped by the agent.
		/// </summary>
		protected bool _reportable;
		/// <summary>
		/// Cached privacy key
		/// </summary>
		protected byte[] _privacyKey;
		/// <summary>
		/// Cached authentication key
		/// </summary>
		protected byte[] _authenticationKey;

		#endregion /* Variables */

		/// <summary>
		/// Constructor
		/// </summary>
		public SecureAgentParameters()
		{
			Reset();
		}
		/// <summary>
		/// Copy constructor. Initialize the class with the values of the parameter class values.
		/// </summary>
		/// <param name="second">Parameter class.</param>
		public SecureAgentParameters(SecureAgentParameters second)
			:this()
		{
			this._contextEngineId.Set(second.ContextEngineId);
			this._contextName.Set(second.ContextName);
			this._engineBoots.Value = second.EngineBoots.Value;
			this._engineId.Set(second.EngineId);
			this._engineTime.Value = second.EngineTime.Value;
			this._engineTimeStamp = second.EngineTimeStamp();
			this._maxMessageSize.Value = second.MaxMessageSize.Value;
			this._privacyProtocol = second.Privacy;
			this._privacySecret.Set(second.PrivacySecret);
			this._authenticationProtocol = second.Authentication;
			this._authenticationSecret.Set(second.AuthenticationSecret);
			this._reportable = second.Reportable;
			this._securityName.Set(second.SecurityName);
			if( second.AuthenticationKey != null )
				this._authenticationKey = (byte[])second.AuthenticationKey.Clone();
			if( second.PrivacyKey != null )
				this._privacyKey = (byte[])second.PrivacyKey.Clone();
		}

		#region Properties

		/// <summary>
		/// Agent authoritative engine id
		/// </summary>
		public OctetString EngineId
		{
			get
			{
				return _engineId;
			}
		}
		/// <summary>
		/// SNMP version 3 agent engine boots value
		/// </summary>
		public Integer32 EngineBoots
		{
			get
			{
				return _engineBoots;
			}
		}

		/// <summary>
		/// Get engine time stamp value (last time engine boots and time values were retrieved from the SNMP agent).
		/// </summary>
		/// <returns>DateTime stamp of the time timeliness values were last refreshed</returns>
		internal DateTime EngineTimeStamp()
		{
			return _engineTimeStamp;
		}
		/// <summary>
		/// SNMP version 3 agent engine time value.
		/// </summary>
		public Integer32 EngineTime
		{
			get
			{
				return _engineTime;
			}
		}
		/// <summary>
		/// Security or user name configured on the SNMP version 3 agent.
		/// </summary>
		public OctetString SecurityName
		{
			get
			{
				return _securityName;
			}
		}
		/// <summary>
		/// Privacy protocol used. Acceptable values are members of <see cref="PrivacyProtocols"/> enum.
		/// </summary>
		public PrivacyProtocols Privacy
		{
			get
			{
				return _privacyProtocol;
			}
			set
			{
				if (value != PrivacyProtocols.None && PrivacyProtocol.GetInstance(value) == null)
					throw new SnmpPrivacyException("Invalid privacy protocol");
				_privacyProtocol = value;
			}
		}
		/// <summary>
		/// Privacy secret. Length of the secret is dependent on the selected privacy method.
		/// </summary>
		public MutableByte PrivacySecret
		{
			get
			{
				return _privacySecret;
			}
		}
		/// <summary>
		/// Authentication method. Acceptable values are members of <see cref="AuthenticationDigests"/> enum.
		/// </summary>
		public AuthenticationDigests Authentication
		{
			get
			{
				return _authenticationProtocol;
			}
			set
			{
				if (value != AuthenticationDigests.None && SnmpSharpNet.Authentication.GetInstance(value) == null)
					throw new SnmpAuthenticationException("Invalid authentication protocol.");
				_authenticationProtocol = value;
			}
		}
		/// <summary>
		/// Authentication secret. Secret length depends on the hash algorithm selected.
		/// </summary>
		public MutableByte AuthenticationSecret
		{
			get
			{
				return _authenticationSecret;
			}
		}
		/// <summary>
		/// SNMP version. Only acceptable version is <see cref="SnmpVersion.Ver3"/>
		/// </summary>
		public SnmpVersion Version
		{
			get
			{
				// return (SnmpVersion)_version.Value;
				return SnmpVersion.Ver3;
			}
		}

		/// <summary>
		/// Get SNMP version 3 context engine id. By default, this value will be set
		/// to the same engine id as authoritative engine id (EngineId). I haven't see a
		/// scenario where this value needs to be different by a manager but now there
		/// is an option to do it.
		/// 
		/// To use the default operation, do not set this value or, if you've already set it,
		/// reset it to null (object.ContextEngineId.Reset()).
		/// </summary>
		public OctetString ContextEngineId
		{
			get
			{
				return _contextEngineId;
			}
		}
		/// <summary>
		/// Get SNMP version 3 context name
		/// </summary>
		public OctetString ContextName
		{
			get
			{
				return _contextName;
			}
		}

		/// <summary>
		/// Get SNMP version 3 maximum message size object
		/// </summary>
		public Integer32 MaxMessageSize
		{
			get
			{
				return _maxMessageSize;
			}
		}

		/// <summary>
		/// Get/Set reportable flag status in the SNMP version 3 packet.
		/// </summary>
		public bool Reportable
		{
			get
			{
				return _reportable;
			}
			set
			{
				_reportable = value;
			}
		}

		#endregion /* Properties */

		/// <summary>
		/// Prepare class for noAuthNoPriv operations. Set authentication and privacy protocols to none.
		/// </summary>
		/// <param name="securityName">User security name</param>
		public void noAuthNoPriv(String securityName)
		{
			_securityName.Set(securityName);
			_authenticationProtocol = AuthenticationDigests.None;
			_authenticationSecret.Clear();
			_privacyProtocol = PrivacyProtocols.None;
			_privacySecret.Clear();
		}
		/// <summary>
		/// Prepare class for authNoPriv operations. Set privacy protocol to none
		/// </summary>
		/// <param name="securityName">User security name</param>
		/// <param name="authDigest">Authentication protocol</param>
		/// <param name="authSecret">Authentication secret (password)</param>
		public void authNoPriv(String securityName, AuthenticationDigests authDigest, String authSecret)
		{
			_securityName.Set(securityName);
			_authenticationProtocol = authDigest;
			_authenticationSecret.Set(authSecret);
			_privacyProtocol = PrivacyProtocols.None;
			_privacySecret.Clear();
		}
		/// <summary>
		/// Prepare class for authPriv operations.
		/// </summary>
		/// <param name="securityName">User security name</param>
		/// <param name="authDigest">Authentication protocol</param>
		/// <param name="authSecret">Authentication secret (password)</param>
		/// <param name="privProtocol">Privacy protocol</param>
		/// <param name="privSecret">Privacy secret (encryption password)</param>
		public void authPriv(String securityName, AuthenticationDigests authDigest, String authSecret, PrivacyProtocols privProtocol, String privSecret)
		{
			_securityName.Set(securityName);
			_authenticationProtocol = authDigest;
			_authenticationSecret.Set(authSecret);
			_privacyProtocol = privProtocol;
			_privacySecret.Set(privSecret);
		}
		/// <summary>
		/// Get/Set cached privacy key value
		/// </summary>
		/// <remarks>
		/// Privacy key is set by reference.
		/// </remarks>
		public byte[] PrivacyKey
		{
			get
			{
				return _privacyKey;
			}
			set
			{
				_privacyKey = value;
			}
		}

		/// <summary>
		/// Get/Set cached authentication key value
		/// </summary>
		/// <remarks>
		/// Authentication key value is set by reference.
		/// </remarks>
		public byte[] AuthenticationKey
		{
			get
			{
				return _authenticationKey;
			}
			set
			{
				_authenticationKey = value;
			}
		}

		/// <summary>
		/// Check if cached privacy or authentication keys are available
		/// </summary>
		public bool HasCachedKeys
		{
			get
			{
				if (_authenticationProtocol != AuthenticationDigests.None)
				{
					if (_authenticationKey != null && _authenticationKey.Length > 0)
					{
						if (_privacyProtocol != PrivacyProtocols.None)
						{
							if (_privacyKey != null && _privacyKey.Length > 0)
								return true;
						}
						else
							return true;
					}
					return false;
				}
				return false;
			}
		}
		/// <summary>
		/// Checks validity of the class. 
		/// </summary>
		/// <returns>Returns false if all required values are not initialized, or if invalid
		/// combination of options is set, otherwise true.</returns>
		public bool Valid()
		{
			if (SecurityName.Length <= 0 && (_authenticationProtocol != AuthenticationDigests.None || _privacyProtocol != PrivacyProtocols.None))
			{
				// You have to supply security name when using security or privacy.
				// in theory you can use blank security name during discovery process so this is not exactly prohibited by it is discouraged
				return false; 
			}
			if (_authenticationProtocol == AuthenticationDigests.None && _privacyProtocol != PrivacyProtocols.None)
				return false; // noAuthPriv mode is not valid in SNMP version 3 
			if (_authenticationProtocol != AuthenticationDigests.None && _authenticationSecret.Length <= 0)
				return false; // Authentication protocol requires authentication secret
			if (_privacyProtocol != PrivacyProtocols.None && _privacySecret.Length <= 0)
				return false; // Privacy protocol requires privacy secret

			if (_engineTimeStamp != DateTime.MinValue)
			{
				if (!ValidateEngineTime())
					return false; // engine time is outside the acceptable timeliness window
			}
			// rest of the values can be empty during the discovery process so no point in checking
			return true;
		}

		/// <summary>
		/// InitializePacket SNMP packet with values from this class. Works only on SNMP version 3 packets.
		/// </summary>
		/// <param name="packet">Instance of <see cref="SnmpV3Packet"/></param>
		/// <exception cref="SnmpInvalidVersionException">Thrown when parameter packet is not SnmpV3Packet</exception>
		public void InitializePacket(SnmpPacket packet)
		{
			if (packet is SnmpV3Packet)
			{
				SnmpV3Packet pkt = (SnmpV3Packet)packet;
				bool isAuth = (_authenticationProtocol == AuthenticationDigests.None) ? false : true;
				bool isPriv = (_privacyProtocol == PrivacyProtocols.None) ? false : true;
				if (isAuth && isPriv)
				{
					pkt.authPriv(_securityName, _authenticationSecret, _authenticationProtocol, _privacySecret, _privacyProtocol);
				}
				else if (isAuth && !isPriv)
				{
					pkt.authNoPriv(_securityName, _authenticationSecret, _authenticationProtocol);
				}
				else
				{
					pkt.NoAuthNoPriv(_securityName);
				}
				pkt.USM.EngineId.Set(_engineId);
				pkt.USM.EngineBoots = _engineBoots.Value;
				pkt.USM.EngineTime = GetCurrentEngineTime();
				pkt.MaxMessageSize = _maxMessageSize.Value;
				pkt.MsgFlags.Reportable = _reportable;
				if (_contextEngineId.Length > 0)
					pkt.ScopedPdu.ContextEngineId.Set(_contextEngineId);
				else
                    pkt.ScopedPdu.ContextEngineId.Set(_engineId);
				if (_contextName.Length > 0)
                    pkt.ScopedPdu.ContextName.Set(_contextName);
				else
                    pkt.ScopedPdu.ContextName.Reset();
			}
			else
				throw new SnmpInvalidVersionException("Invalid SNMP version.");
		}

		/// <summary>
		/// Copy all relevant values from the SnmpV3Packet class. Do not use this class for
		/// updating the SNMP version 3 discovery process results because secret name, authentication
		/// and privacy values are updated as well which discovery process doesn't use.
		/// </summary>
		/// <param name="packet"><see cref="SnmpV3Packet"/> cast as <see cref="SnmpPacket"/></param>
		/// <exception cref="SnmpInvalidVersionException">Thrown when SNMP packet class other then version 3 
		/// is passed as parameter</exception>
		public void UpdateValues(SnmpPacket packet)
		{
			if (packet is SnmpV3Packet)
			{
				SnmpV3Packet pkt = (SnmpV3Packet)packet;
				_authenticationProtocol = pkt.USM.Authentication;
				_privacyProtocol = pkt.USM.Privacy;
				_authenticationSecret.Set(pkt.USM.AuthenticationSecret);
				_privacySecret.Set(pkt.USM.PrivacySecret);
				_securityName.Set(pkt.USM.SecurityName);
				if (pkt.MaxMessageSize < _maxMessageSize.Value)
					_maxMessageSize.Value = pkt.MaxMessageSize;
				UpdateDiscoveryValues(pkt);
			}
			else
				throw new SnmpInvalidVersionException("Invalid SNMP version.");
		}

		/// <summary>
		/// Update class values with SNMP version 3 discovery values from the supplied <see cref="SnmpV3Packet"/>
		/// class. Values updated are EngineId, EngineTime and EngineBoots.
		/// </summary>
		/// <param name="packet"><see cref="SnmpV3Packet"/> class cast as <see cref="SnmpPacket"/></param>
		/// <exception cref="SnmpInvalidVersionException">Thrown when SNMP packet class other then version 3 
		/// is passed as parameter</exception>
		public void UpdateDiscoveryValues(SnmpPacket packet)
		{
			if (packet is SnmpV3Packet)
			{
				SnmpV3Packet pkt = (SnmpV3Packet)packet;
				_engineId.Set(pkt.USM.EngineId);
				_engineTime.Value = pkt.USM.EngineTime;
				_engineBoots.Value = pkt.USM.EngineBoots;
				UpdateTimeStamp();
				_contextEngineId.Set(pkt.ScopedPdu.ContextEngineId);
				_contextName.Set(pkt.ScopedPdu.ContextName);
			}
			else
				throw new SnmpInvalidVersionException("Invalid SNMP version.");
		}

		/// <summary>
		/// Updates engine time timestamp. This value is used to determine if agents engine time stored
		/// in this class is valid.
		/// 
		/// Timestamp is saved as DateTime class by default initialized to DateTime.MinValue. Timestamp value
		/// is stored in GMT to make it portable (if it is saved on one computer and loaded on another that uses
		/// a different time zone).
		/// </summary>
		public void UpdateTimeStamp()
		{
			_engineTimeStamp = DateTime.UtcNow;
		}
		/// <summary>
		/// Validate agents engine time. Valid engine time value is time that has been initialized to
		/// a value other then default (DateTime.MinValue is default set in the constructor) and that
		/// has been updated in the last 10 times the SNMP v3 timely window (150 seconds). In other words,
		/// valid time is any time value in the last 1500 seconds (or 25 minutes).
		/// </summary>
		/// <returns>True if engine time value is valid, otherwise false.</returns>
		public bool ValidateEngineTime()
		{
			if (_engineTimeStamp == DateTime.MinValue)
				return false; // timestamp is at its initial value. not valid

			TimeSpan diff = DateTime.UtcNow.Subtract(_engineTimeStamp);

			// if EngineTime value has not been updated in 10 * max acceptable period (150 seconds) then
			// time is no longer valid
			if (diff.TotalSeconds >= (150 * 10))
				return false;

			return true;
		}

		/// <summary>
		/// Calculates and returns current agents engine time. <see cref="ValidateEngineTime"/> is called
		/// prior to calculation to make sure current engine time is timely enough to use.
		/// 
		/// EngineTime is calculated as last received engine time + difference in seconds between the time
		/// stamp saved when last time value was received and current time (using the internal GMT clock).
		/// </summary>
		/// <returns>Adjusted engine time value or 0 if time is outside the time window.</returns>
		public int GetCurrentEngineTime()
		{
			if (!ValidateEngineTime())
				return 0;
			TimeSpan diff = DateTime.UtcNow.Subtract(_engineTimeStamp);
			// increment the value by one to make sure we don't fall behind the agents clock
			return Convert.ToInt32(_engineTime.Value + diff.TotalSeconds + 1); 
		}

		/// <summary>
		/// Validate that incoming packet has arrived from the correct engine id and is using a correct
		/// combination of privacy and authentication values.
		/// </summary>
		/// <param name="packet">Received and parsed SNMP version 3 packet.</param>
		/// <returns>True if packet is valid, otherwise false.</returns>
		/// <exception cref="SnmpException">Thrown on following errors with ErrorCode:
		/// * ErrorCode = 0: SecureAgentParameters was updated after request was made but before reply was received (this is not allowed)
		/// * SnmpException.InvalidAuthoritativeEngineId: engine id in the reply does not match request
		/// * SnmpException.InvalidSecurityName: security name mismatch between request and reply packets
		/// * SnmpException.ReportOnNoReports: report packet received when we had reportable set to false in the request
		/// * SnmpException.UnsupportedNoAuthPriv: noAuthPriv is not supported
		/// </exception>
		/// <exception cref="SnmpPrivacyException">Thrown when configured privacy passwords in this class and in the packet class do not match</exception>
		/// <exception cref="SnmpAuthenticationException">Thrown when configured authentication passwords in this class and in the packet class do not match</exception>
		public bool ValidateIncomingPacket(SnmpV3Packet packet)
		{
			// First check if this is a report packet.
			if (packet.Pdu.Type == PduType.Report)
			{
				if (!_reportable)
				{
					// we do not expect report packets so dump it
					throw new SnmpException(SnmpException.ReportOnNoReports, "Unexpected report packet received.");
					// return false; 
				}
				if (packet.MsgFlags.Authentication == false && packet.MsgFlags.Privacy)
				{
					// no authentication and no privacy allowed in report packets
					throw new SnmpException(SnmpException.UnsupportedNoAuthPriv, "Authentication and privacy combination is not supported.");
					// return false; 
				}
				// the rest will not be checked, there is no point
			}
			else
			{
				if (packet.USM.EngineId != _engineId)
				{
					// different engine id is not allowed
					throw new SnmpException(SnmpException.InvalidAuthoritativeEngineId, "EngineId mismatch.");
					// return false; 
				}

				if (packet.USM.Authentication != _authenticationProtocol || packet.USM.Privacy != _privacyProtocol)
				{
					// we have to have the same authentication and privacy protocol - no last minute changes
					throw new SnmpException("Agent parameters updated after request was made.");
					// return false; 
				}
				if (packet.USM.Authentication != AuthenticationDigests.None)
				{
					if (packet.USM.AuthenticationSecret != _authenticationSecret)
					{
						// authentication secret has to match
						throw new SnmpAuthenticationException("Authentication secret in the packet class does not match the IAgentParameter secret.");
						// return false; 
					}
				}
				if (packet.USM.Privacy != PrivacyProtocols.None)
				{
					if (packet.USM.PrivacySecret != _privacySecret)
					{
						// privacy secret has to match
						throw new SnmpPrivacyException("Privacy secret in the packet class does not match the IAgentParameters secret.");
						// return false; 
					}
				}
				if (packet.USM.SecurityName != _securityName)
				{
					throw new SnmpException(SnmpException.InvalidSecurityName, "Security name mismatch.");
					// return false;
				}
			}
			return true;
		}
		/// <summary>
		/// Reset privacy and authentication keys to null.
		/// </summary>
		public void ResetKeys()
		{
			_privacyKey = null;
			_authenticationKey = null;
		}
		/// <summary>
		/// Reset the class. Initialize all member values to class defaults.
		/// </summary>
		public void Reset()
		{
			_engineId = new OctetString();
			_engineBoots = new Integer32();
			_engineTime = new Integer32();

			_engineTimeStamp = DateTime.MinValue;

			_privacyProtocol = PrivacyProtocols.None;
			_authenticationProtocol = AuthenticationDigests.None;

			_privacySecret = new MutableByte();
			_authenticationSecret = new MutableByte();

			_contextEngineId = new OctetString();
			_contextName = new OctetString();
			_securityName = new OctetString();

			// max message size is initialized to 64KB by default. It will be
			// to the smaller of the two values after discovery process
			_maxMessageSize = new Integer32(64 * 1024);

			_reportable = true;

			_privacyKey = null;
			_authenticationKey = null;
		}
		/// <summary>
		/// Clone current object
		/// </summary>
		/// <returns>Duplicate object initialized with values from this class.</returns>
		public object Clone()
		{
			return (object)new SecureAgentParameters(this);
		}
		/// <summary>
		/// Build cached authentication and privacy encryption keys if they are appropriate for the selected security mode.
		/// </summary>
		/// <remarks>
		/// This method should be called after discovery process has been completed and all security related values
		/// have been set. For noAuthNoPriv, none of the keys are generated. authNoPriv will result in authentication
		/// key cached. authPriv will generate authentication and privacy keys.
		/// 
		/// For successful key caching you need to set both relevant protocols and secret values.
		/// </remarks>
		public void BuildCachedSecurityKeys()
		{
			_authenticationKey = _privacyKey = null;

			if (_engineId == null || _engineId.Length <= 0)
				return;
			if (_authenticationSecret == null || _authenticationSecret.Length <= 0)
				return;
			if (_authenticationProtocol != AuthenticationDigests.None)
			{
				IAuthenticationDigest authProto = SnmpSharpNet.Authentication.GetInstance(_authenticationProtocol);
				if (authProto != null)
				{
					_authenticationKey = authProto.PasswordToKey(_authenticationSecret, _engineId);
					if (_privacyProtocol != PrivacyProtocols.None && _privacySecret != null && _privacySecret.Length > 0)
					{
						IPrivacyProtocol privProto = SnmpSharpNet.PrivacyProtocol.GetInstance(_privacyProtocol);
						if (privProto != null)
						{
							_privacyKey = privProto.PasswordToKey(_privacySecret, _engineId, authProto);
						}
					}
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SnmpSharpNet
{
	/// <summary>
	/// USM security SNMP version 3 target class.
	/// </summary>
	public class UTarget: ITarget
	{
		#region Private variables
		/// <summary>
		/// Target IP address
		/// </summary>
		protected IpAddress _address;
		/// <summary>
		/// Target port number
		/// </summary>
		protected int _port;
		/// <summary>
		/// Target SNMP version number
		/// </summary>
		protected SnmpVersion _version;
		/// <summary>
		/// Target request timeout period in milliseconds
		/// </summary>
		protected int _timeout;
		/// <summary>
		/// Target maximum retry count
		/// </summary>
		protected int _retry;
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

		#endregion Private variables

		/// <summary>
		/// Constructor
		/// </summary>
		public UTarget()
		{
			Reset();
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


		#endregion Properties

		#region Target specific methods

		/// <summary>
		/// Reset the class. Initialize all member values to class defaults.
		/// </summary>
		public void Reset()
		{
			_address = new IpAddress(System.Net.IPAddress.Loopback);
			_port = 161;
			_version = SnmpVersion.Ver3;
			_timeout = 2000;
			_retry = 1;

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

		#endregion Target specific methods

		#region ITarget Members

		/// <summary>
		/// Prepare packet for transmission by filling target specific information in the packet.
		/// </summary>
		/// <param name="packet">SNMP packet class for the required version</param>
		/// <returns>True if packet values are correctly set, otherwise false.</returns>
		public bool PreparePacketForTransmission(SnmpPacket packet)
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
			return false;
		}
		/// <summary>
		/// Validate received reply
		/// </summary>
		/// <param name="packet">Received SNMP packet</param>
		/// <returns>True if packet is validated, otherwise false</returns>
		/// <exception cref="SnmpException">Thrown on following errors with ErrorCode:
		/// * ErrorCode = 0: SecureAgentParameters was updated after request was made but before reply was received (this is not allowed)
		/// * SnmpException.InvalidAuthoritativeEngineId: engine id in the reply does not match request
		/// * SnmpException.InvalidSecurityName: security name mismatch between request and reply packets
		/// * SnmpException.ReportOnNoReports: report packet received when we had reportable set to false in the request
		/// * SnmpException.UnsupportedNoAuthPriv: noAuthPriv is not supported
		/// </exception>
		/// <exception cref="SnmpPrivacyException">Thrown when configured privacy passwords in this class and in the packet class do not match</exception>
		/// <exception cref="SnmpAuthenticationException">Thrown when configured authentication passwords in this class and in the packet class do not match</exception>
		public bool ValidateReceivedPacket(SnmpPacket packet)
		{
			if (! (packet is SnmpV3Packet))
				return false;
			SnmpV3Packet pkt = (SnmpV3Packet)packet;

			// First check if this is a report packet.
			if (pkt.Pdu.Type == PduType.Response)
			{
				if (!_reportable)
				{
					// we do not expect report packets so dump it
					throw new SnmpException(SnmpException.ReportOnNoReports, "Unexpected report packet received.");
					// return false; 
				}
				if (pkt.MsgFlags.Authentication == false && pkt.MsgFlags.Privacy)
				{
					// no authentication and no privacy allowed in report packets
					throw new SnmpException(SnmpException.UnsupportedNoAuthPriv, "Authentication and privacy combination is not supported.");
					// return false; 
				}
				// the rest will not be checked, there is no point
			}
			else
			{
				if (pkt.USM.EngineId != _engineId)
				{
					// different engine id is not allowed
					throw new SnmpException(SnmpException.InvalidAuthoritativeEngineId, "EngineId mismatch.");
					// return false; 
				}

				if (pkt.USM.Authentication != _authenticationProtocol || pkt.USM.Privacy != _privacyProtocol)
				{
					// we have to have the same authentication and privacy protocol - no last minute changes
					throw new SnmpException("Agent parameters updated after request was made.");
					// return false; 
				}
				if (pkt.USM.Authentication != AuthenticationDigests.None)
				{
					if (pkt.USM.AuthenticationSecret != _authenticationSecret)
					{
						// authentication secret has to match
						throw new SnmpAuthenticationException("Authentication secret in the packet class does not match the IAgentParameter secret.");
						// return false; 
					}
				}
				if (pkt.USM.Privacy != PrivacyProtocols.None)
				{
					if (pkt.USM.PrivacySecret != _privacySecret)
					{
						// privacy secret has to match
						throw new SnmpPrivacyException("Privacy secret in the packet class does not match the IAgentParameters secret.");
						// return false; 
					}
				}
				if (pkt.USM.SecurityName != _securityName)
				{
					throw new SnmpException(SnmpException.InvalidSecurityName, "Security name mismatch.");
					// return false;
				}
			}

			return true;
		}
		/// <summary>
		/// Get version of SNMP protocol this target supports. <see cref="UTarget"/> supports SNMP version 3 only.
		/// </summary>
		/// <exception cref="SnmpInvalidVersionException">Thrown when SNMP version other then 3 is set</exception>
		public SnmpVersion Version
		{
			get { return _version; }
			set
			{
				if (value != SnmpVersion.Ver3)
					throw new SnmpInvalidVersionException("UTarget is only suitable for use with SNMP v2 protocol version.");
				_version = value;
			}
		}
		/// <summary>
		/// Timeout in milliseconds for the target. Valid timeout values are between 100 and 10000 milliseconds.
		/// </summary>
		public int Timeout
		{
			get
			{
				return _timeout;
			}
			set
			{
				if (value < 100 || value > 10000)
					throw new OverflowException("Valid timeout value is between 100 milliseconds and 10000 milliseconds");
				_timeout = value;
			}
		}
		/// <summary>
		/// Number of retries for the target. Valid values are 0-5.
		/// </summary>
		public int Retry
		{
			get
			{
				return _retry;
			}
			set
			{
				if (value < 0 || value > 5)
					throw new OverflowException("Valid retry value is between 0 and 5");
				_retry = value;
			}
		}
		/// <summary>
		/// Target IP address
		/// </summary>
		public IpAddress Address
		{
			get
			{
				return _address;
			}
		}
		/// <summary>
		/// Target port number
		/// </summary>
		public int Port
		{
			get
			{
				return _port;
			}
			set
			{
				_port = value;
			}
		}
		/// <summary>
		/// Checks validity of the class. 
		/// </summary>
		/// <returns>Returns false if all required values are not initialized, or if invalid
		/// combination of options is set, otherwise true.</returns>
		public bool Valid()
		{
			if (_address == null || !_address.Valid)
				return false;
			if (_port == 0)
				return false;
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

		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace SnmpSharpNet
{
	/// <summary>
	/// Community based SNMP target. Used for SNMP version 1 and version 2c.
	/// </summary>
	public class CTarget: ITarget
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
		/// SNMP community name
		/// </summary>
		protected String _community;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public CTarget()
		{
			_address = new IpAddress(System.Net.IPAddress.Loopback);
			_port = 161;
			_version = SnmpVersion.Ver2;
			_timeout = 2000;
			_retry = 1;
			_community = "public";
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="addr">Target address</param>
		public CTarget(IPAddress addr)
			: this()
		{
			_address.Set(addr);
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="addr">Target address</param>
		/// <param name="community">SNMP community name to use with the target</param>
		public CTarget(IPAddress addr, String community)
			: this(addr)
		{
			_community = community;
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="addr">Target address</param>
		/// <param name="port">Taret UDP port number</param>
		/// <param name="community">SNMP community name to use with the target</param>
		public CTarget(IPAddress addr, int port, String community)
			: this(addr,community)
		{
			_port = port;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// SNMP community name for the target
		/// </summary>
		public String Community
		{
			get
			{
				return _community;
			}
			set
			{
				_community = value;
			}
		}

		#endregion Properties

		#region ITarget Members

		/// <summary>
		/// Prepare packet for transmission by filling target specific information in the packet.
		/// </summary>
		/// <param name="packet">SNMP packet class for the required version</param>
		/// <returns>True if packet values are correctly set, otherwise false.</returns>
		public bool PreparePacketForTransmission(SnmpPacket packet)
		{
			if (packet.Version != _version)
				return false;
			if (_version == SnmpVersion.Ver1)
			{
				SnmpV1Packet pkt = (SnmpV1Packet)packet;
				pkt.Community.Set(_community);
				return true;
			}
			else if (_version == SnmpVersion.Ver2)
			{
				SnmpV2Packet pkt = (SnmpV2Packet)packet;
				pkt.Community.Set(_community);
				return true;
			}
			return false;
		}
		/// <summary>
		/// Validate received reply
		/// </summary>
		/// <param name="packet">Received SNMP packet</param>
		/// <returns>True if packet is validated, otherwise false</returns>
		public bool ValidateReceivedPacket(SnmpPacket packet)
		{
			if (packet.Version != _version)
				return false;
			if( _version == SnmpVersion.Ver1 ) {
				SnmpV1Packet pkt = (SnmpV1Packet)packet;
				if (pkt.Community.Equals(_community))
					return true;
			}
			else if( _version == SnmpVersion.Ver2 )
			{
				SnmpV2Packet pkt = (SnmpV2Packet)packet;
				if (pkt.Community.Equals(_community))
					return true;
			}
			return false;
		}
		/// <summary>
		/// Get version of SNMP protocol this target supports
		/// </summary>
		/// <exception cref="SnmpInvalidVersionException">Thrown when SNMP version other then 1 or 2c is set</exception>
		public SnmpVersion Version
		{
			get { return _version; }
			set
			{
				if (value != SnmpVersion.Ver1 && value != SnmpVersion.Ver2)
					throw new SnmpInvalidVersionException("CTarget is only suitable for use with SNMP v1 and v2c protocol versions.");
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
		/// Check validity of the target information.
		/// </summary>
		/// <returns>True if valid, otherwise false.</returns>
		public bool Valid()
		{
			if (_community == null || _community.Length == 0)
				return false;
			if (_address == null || !_address.Valid)
				return false;
			if (_port == 0)
				return false;
			return true;
		}
		#endregion
	}
}

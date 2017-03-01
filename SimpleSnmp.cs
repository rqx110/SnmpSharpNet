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
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace SnmpSharpNet
{
	/// <summary>
	/// Utility class to enable simplified access to SNMP version 1 and version 2 requests and replies.
	/// </summary>
	/// <remarks>
	/// Use this class if you are not looking for "deep" SNMP functionality. Design of the class is based
	/// around providing simplest possible access to SNMP operations without getting stack into details.
	/// 
	/// If you are using the simplest way, you will leave SuppressExceptions flag to true and get all errors causing methods to return "null" result
	/// which will not tell you why operation failed. You can change the SuppressExceptions flag to false and catch any and all
	/// exception throwing errors.
	/// 
	/// Either way, have fun.
	/// </remarks>
	public class SimpleSnmp
	{
		/// <summary>
		/// SNMP Agents IP address
		/// </summary>
		protected IPAddress _peerIP;
		/// <summary>
		/// SNMP Agents name
		/// </summary>
		protected string _peerName;
		/// <summary>
		/// SNMP Agent UDP port number
		/// </summary>
		protected int _peerPort;
		/// <summary>
		/// Target class
		/// </summary>
		protected UdpTarget _target;
		/// <summary>
		/// Timeout value in milliseconds
		/// </summary>
		protected int _timeout;
		/// <summary>
		/// Maximum retry count excluding the first request
		/// </summary>
		protected int _retry;
		/// <summary>
		/// SNMP community name
		/// </summary>
		protected string _community;
		/// <summary>
		/// Non repeaters value used in SNMP GET-BULK requests
		/// </summary>
		protected int _nonRepeaters = 0;
		/// <summary>
		/// Maximum repetitions value used in SNMP GET-BULK requests
		/// </summary>
		protected int _maxRepetitions = 50;
		/// <summary>
		/// Flag determines if exceptions are suppressed or thrown. When exceptions are suppressed, methods
		/// return null on errors regardless of what the error is.
		/// </summary>
		protected bool _suppressExceptions = true;
		/// <summary>Constructor.</summary>
		/// <remarks>
		/// Class is initialized to default values. Peer IP address is set to loopback, peer port number
		/// to 161, timeout to 2000 ms (2 seconds), retry count to 2 and community name to public.
		/// </remarks>
		public SimpleSnmp()
		{
			_peerIP = IPAddress.Loopback;
			_peerPort = 161;
			_timeout = 2000;
			_retry = 2;
			_community = "public";
			_target = null;
			_peerName = "localhost";
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// Class is initialized with default values with the agent name set to the supplied DNS name (or 
		/// IP address). If peer name is a DNS name, DNS resolution will take place in the constructor attempting
		/// to resolve it an IP address.
		/// </remarks>
		/// <param name="peerName">Peer name or IP address</param>
		public SimpleSnmp(string peerName)
			: this()
		{
			_peerName = peerName;
			Resolve();
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="peerName">Peer name or IP address</param>
		/// <param name="community">SNMP community name</param>
		public SimpleSnmp(string peerName, string community)
			: this(peerName)
		{
			_community = community;
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="peerName">Peer name or IP address</param>
		/// <param name="peerPort">Peer UDP port number</param>
		/// <param name="community">SNMP community name</param>
		public SimpleSnmp(string peerName, int peerPort, string community)
			: this( peerName, community)
		{
			_peerPort = peerPort;
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="peerName">Peer name or IP address</param>
		/// <param name="peerPort">Peer UDP port number</param>
		/// <param name="community">SNMP community name</param>
		/// <param name="timeout">SNMP request timeout</param>
		/// <param name="retry">Maximum number of retries excluding the first request (0 = 1 request is sent)</param>
		public SimpleSnmp(string peerName, int peerPort, string community, int timeout, int retry)
			: this(peerName, peerPort, community)
		{
			_timeout = timeout;
			_retry = retry;
		}
		/// <summary>Class validity flag</summary>
		/// <remarks>
		/// Return class validity status. If class is valid, it is ready to send requests and receive
		/// replies. If false, requests to send requests will fail.
		/// </remarks>
		public bool Valid
		{
			get
			{
				if (_peerIP == IPAddress.None || _peerIP == IPAddress.Any)
				{
					return false;
				}
				if (_community.Length < 1 || _community.Length > 50)
				{
					return false;
				}
				return true;
			}
		}

		/// <summary>
		/// SNMP GET request
		/// </summary>
		/// <example>SNMP GET request:
		/// <code>
		/// String snmpAgent = "10.10.10.1";
		/// String snmpCommunity = "public";
		/// SimpleSnmp snmp = new SimpleSnmp(snmpAgent, snmpCommunity);
		/// // Create a request Pdu
		/// Pdu pdu = new Pdu();
		/// pdu.Type = SnmpConstants.GET; // type GET
		/// pdu.VbList.Add("1.3.6.1.2.1.1.1.0");
		/// Dictionary&lt;Oid, AsnType&gt; result = snmp.GetNext(SnmpVersion.Ver1, pdu);
		/// if( result == null ) {
		///   Console.WriteLine("Request failed.");
		/// } else {
		///   foreach (KeyValuePair&lt;Oid, AsnType&gt; entry in result)
		///   {
		///     Console.WriteLine("{0} = {1}: {2}", entry.Key.ToString(), SnmpConstants.GetTypeName(entry.Value.Type),
		///       entry.Value.ToString());
		///   }
		/// }
		/// </code>
		/// Will return:
		/// <code>
		/// 1.3.6.1.2.1.1.1.0 = OctetString: "Dual core Intel notebook"
		/// </code>
		/// </example>
		/// <param name="version">SNMP protocol version. Acceptable values are SnmpVersion.Ver1 and SnmpVersion.Ver2</param>
		/// <param name="pdu">Request Protocol Data Unit</param>
		/// <returns>Result of the SNMP request in a dictionary format with Oid => AsnType values</returns>
		public Dictionary<Oid, AsnType> Get(SnmpVersion version, Pdu pdu)
		{
			if (!Valid)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpException("SimpleSnmp class is not valid.");
				}
				return null; // class is not fully initialized.
			}
			// function only works on SNMP version 1 and SNMP version 2 requests
			if (version != SnmpVersion.Ver1 && version != SnmpVersion.Ver2)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpInvalidVersionException("SimpleSnmp support SNMP version 1 and 2 only.");
				}
				return null;
			}
			try
			{
				_target = new UdpTarget(_peerIP, _peerPort, _timeout, _retry);
			}
			catch( Exception ex )
			{
				_target = null;
				if (!_suppressExceptions)
				{
					throw ex;
				}
			}
			if( _target == null )
			{
				return null;
			}
			try
			{
				AgentParameters param = new AgentParameters(version, new OctetString(_community));
				SnmpPacket result = _target.Request(pdu, param);
				if (result != null)
				{
					if (result.Pdu.ErrorStatus == 0)
					{
						Dictionary<Oid, AsnType> res = new Dictionary<Oid, AsnType>();
						foreach (Vb v in result.Pdu.VbList)
						{
							if (version == SnmpVersion.Ver2 && (v.Value.Type == SnmpConstants.SMI_NOSUCHINSTANCE ||
								v.Value.Type == SnmpConstants.SMI_NOSUCHOBJECT))
							{
								if (!res.ContainsKey(v.Oid))
								{
									res.Add(v.Oid, new Null());
								}
								else
								{
									res.Add(Oid.NullOid(), v.Value);
								}		
							}
							else
							{
								if (!res.ContainsKey(v.Oid))
								{
									res.Add(v.Oid, v.Value);
								}
								else
								{
									if (res[v.Oid].Type == v.Value.Type)
									{
										res[v.Oid] = v.Value; // update value of the existing Oid entry
									}
									else
									{
										throw new SnmpException(SnmpException.OidValueTypeChanged, String.Format("Value type changed from {0} to {1}", res[v.Oid].Type, v.Value.Type));
									}
								}
							}
						}
						_target.Close();
						_target = null;
						return res;
					}
					else 
					{
						if (!_suppressExceptions)
						{
							throw new SnmpErrorStatusException("Agent responded with an error", result.Pdu.ErrorStatus, result.Pdu.ErrorIndex);
						}
					}
				}
			}
			catch( Exception ex )
			{
				if( ! _suppressExceptions ) 
				{
					_target.Close();
					_target = null;
					throw ex;
				}
			}
			_target.Close();
			_target = null;
			return null;
		}

		/// <summary>
		/// SNMP GET request
		/// </summary>
		/// <example>SNMP GET request:
		/// <code>
		/// String snmpAgent = "10.10.10.1";
		/// String snmpCommunity = "public";
		/// SimpleSnmp snmp = new SimpleSnmp(snmpAgent, snmpCommunity);
		/// Dictionary&lt;Oid, AsnType&gt; result = snmp.GetNext(SnmpVersion.Ver1, new string[] { "1.3.6.1.2.1.1.1.0" });
		/// if( result == null ) {
		///   Console.WriteLine("Request failed.");
		/// } else {
		///   foreach (KeyValuePair&lt;Oid, AsnType&gt; entry in result)
		///   {
		///     Console.WriteLine("{0} = {1}: {2}", entry.Key.ToString(), SnmpConstants.GetTypeName(entry.Value.Type),
		///       entry.Value.ToString());
		///   }
		/// }
		/// </code>
		/// Will return:
		/// <code>
		/// 1.3.6.1.2.1.1.1.0 = OctetString: "Dual core Intel notebook"
		/// </code>
		/// </example>
		/// <param name="version">SNMP protocol version number. Acceptable values are SnmpVersion.Ver1 and SnmpVersion.Ver2</param>
		/// <param name="oidList">List of request OIDs in string dotted decimal format.</param>
		/// <returns>Result of the SNMP request in a dictionary format with Oid => AsnType values</returns>
		public Dictionary<Oid, AsnType> Get(SnmpVersion version, string[] oidList)
		{
			if (!Valid)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpException("SimpleSnmp class is not valid.");
				}
				return null;
			}
			// function only works on SNMP version 1 and SNMP version 2 requests
			if (version != SnmpVersion.Ver1 && version != SnmpVersion.Ver2)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpInvalidVersionException("SimpleSnmp support SNMP version 1 and 2 only.");
				}
				return null;
			}
			Pdu pdu = new Pdu(PduType.Get);
			foreach (string s in oidList)
			{
				pdu.VbList.Add(s);
			}
			return Get(version, pdu);
		}

		/// <summary>
		/// SNMP GET-NEXT request
		/// </summary>
		/// <example>SNMP GET-NEXT request:
		/// <code>
		/// String snmpAgent = "10.10.10.1";
		/// String snmpCommunity = "private";
		/// SimpleSnmp snmp = new SimpleSnmp(snmpAgent, snmpCommunity);
		/// // Create a request Pdu
		/// Pdu pdu = new Pdu();
		/// pdu.Type = SnmpConstants.GETNEXT; // type GETNEXT
		/// pdu.VbList.Add("1.3.6.1.2.1.1");
		/// Dictionary&lt;Oid, AsnType&gt; result = snmp.GetNext(pdu);
		/// if( result == null ) {
		///   Console.WriteLine("Request failed.");
		/// } else {
		///   foreach (KeyValuePair&lt;Oid, AsnType&gt; entry in result)
		///   {
		///     Console.WriteLine("{0} = {1}: {2}", entry.Key.ToString(), SnmpConstants.GetTypeName(entry.Value.Type),
		///       entry.Value.ToString());
		///   }
		/// }
		/// </code>
		/// Will return:
		/// <code>
		/// 1.3.6.1.2.1.1.1.0 = OctetString: "Dual core Intel notebook"
		/// </code>
		/// </example>
		/// <param name="version">SNMP protocol version. Acceptable values are SnmpVersion.Ver1 and SnmpVersion.Ver2</param>
		/// <param name="pdu">Request Protocol Data Unit</param>
		/// <returns>Result of the SNMP request in a dictionary format with Oid => AsnType values</returns>
		public Dictionary<Oid, AsnType> GetNext(SnmpVersion version, Pdu pdu)
		{
			if (!Valid)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpException("SimpleSnmp class is not valid.");
				}
				return null; // class is not fully initialized.
			}
			// function only works on SNMP version 1 and SNMP version 2 requests
			if (version != SnmpVersion.Ver1 && version != SnmpVersion.Ver2)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpInvalidVersionException("SimpleSnmp support SNMP version 1 and 2 only.");
				}
				return null;
			}
			try
			{
				_target = new UdpTarget(_peerIP, _peerPort, _timeout, _retry);
			}
			catch
			{
				_target = null;
			}
			if( _target == null )
			{
				return null;
			}
			try
			{
				AgentParameters param = new AgentParameters(version, new OctetString(_community));
				SnmpPacket result = _target.Request(pdu, param);
				if (result != null)
				{
					if (result.Pdu.ErrorStatus == 0)
					{
						Dictionary<Oid, AsnType> res = new Dictionary<Oid, AsnType>();
						foreach (Vb v in result.Pdu.VbList)
						{
							if (version == SnmpVersion.Ver2 &&
								(v.Value.Type == SnmpConstants.SMI_ENDOFMIBVIEW) ||
								(v.Value.Type == SnmpConstants.SMI_NOSUCHINSTANCE) ||
								(v.Value.Type == SnmpConstants.SMI_NOSUCHOBJECT))
							{
								break;
							}
							if (res.ContainsKey(v.Oid))
							{
								if (res[v.Oid].Type != v.Value.Type)
								{
									throw new SnmpException(SnmpException.OidValueTypeChanged, "OID value type changed for OID: " + v.Oid.ToString());
								}
								else
								{
									res[v.Oid] = v.Value;
								}
							}
							else
							{
								res.Add(v.Oid, v.Value);
							}
						}
						_target.Close();
						_target = null;
						return res;
					}
					else
					{
						if( ! _suppressExceptions )
						{
							throw new SnmpErrorStatusException("Agent responded with an error", result.Pdu.ErrorStatus, result.Pdu.ErrorIndex);
						}
					}
				}
			}
			catch( Exception ex )
			{
				if (!_suppressExceptions)
				{
					_target.Close();
					_target = null;
					throw ex;
				}
			}
			_target.Close();
			_target = null;
			return null;
		}

		/// <summary>
		/// SNMP GET-NEXT request
		/// </summary>
		/// <example>SNMP GET-NEXT request:
		/// <code>
		/// String snmpAgent = "10.10.10.1";
		/// String snmpCommunity = "private";
		/// SimpleSnmp snmp = new SimpleSnmp(snmpAgent, snmpCommunity);
		/// Dictionary&lt;Oid, AsnType&gt; result = snmp.GetNext(SnmpVersion.Ver1, new string[] { "1.3.6.1.2.1.1" });
		/// if( result == null ) {
		///   Console.WriteLine("Request failed.");
		/// } else {
		///   foreach (KeyValuePair&lt;Oid, AsnType&gt; entry in result)
		///   {
		///     Console.WriteLine("{0} = {1}: {2}", entry.Key.ToString(), SnmpConstants.GetTypeName(entry.Value.Type),
		///       entry.Value.ToString());
		///   }
		/// }
		/// </code>
		/// Will return:
		/// <code>
		/// 1.3.6.1.2.1.1.1.0 = OctetString: "Dual core Intel notebook"
		/// </code>
		/// </example>
		/// <param name="version">SNMP protocol version number. Acceptable values are SnmpVersion.Ver1 and SnmpVersion.Ver2</param>
		/// <param name="oidList">List of request OIDs in string dotted decimal format.</param>
		/// <returns>Result of the SNMP request in a dictionary format with Oid => AsnType values</returns>
		public Dictionary<Oid, AsnType> GetNext(SnmpVersion version, string[] oidList)
		{
			if (!Valid)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpException("SimpleSnmp class is not valid.");
				}
				return null;
			}
			// function only works on SNMP version 1 and SNMP version 2 requests
			if (version != SnmpVersion.Ver1 && version != SnmpVersion.Ver2)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpInvalidVersionException("SimpleSnmp support SNMP version 1 and 2 only.");
				}
				return null;
			}
			Pdu pdu = new Pdu(PduType.GetNext);
			foreach (string s in oidList)
			{
				pdu.VbList.Add(s);
			}
			return GetNext(version, pdu);
		}

		/// <summary>
		/// SNMP GET-BULK request
		/// </summary>
		/// <remarks>
		/// GetBulk request type is only available with SNMP v2c agents. SNMP v3 also supports the request itself
		/// but that version of the protocol is not supported by SimpleSnmp.
		/// 
		/// GetBulk method will return a dictionary of Oid to value mapped values as returned form a
		/// single GetBulk request to the agent. You can change how the request itself is made by changing the
		/// SimpleSnmp.NonRepeaters and SimpleSnmp.MaxRepetitions values. SimpleSnmp properties are only used
		/// when values in the parameter Pdu are set to 0.
		/// </remarks>
		/// <example>SNMP GET-BULK request:
		/// <code>
		/// String snmpAgent = "10.10.10.1";
		/// String snmpCommunity = "private";
		/// SimpleSnmp snmp = new SimpleSnmp(snmpAgent, snmpCommunity);
		/// // Create a request Pdu
		/// Pdu pdu = new Pdu();
		/// pdu.Type = SnmpConstants.GETBULK; // type GETBULK
		/// pdu.VbList.Add("1.3.6.1.2.1.1");
		/// pdu.NonRepeaters = 0;
		/// pdu.MaxRepetitions = 10;
		/// Dictionary&lt;Oid, AsnType&gt; result = snmp.GetBulk(pdu);
		/// if( result == null ) {
		///   Console.WriteLine("Request failed.");
		/// } else {
		///   foreach (KeyValuePair&lt;Oid, AsnType&gt; entry in result)
		///   {
		///     Console.WriteLine("{0} = {1}: {2}", entry.Key.ToString(), SnmpConstants.GetTypeName(entry.Value.Type),
		///       entry.Value.ToString());
		///   }
		/// }
		/// </code>
		/// Will return:
		/// <code>
		/// 1.3.6.1.2.1.1.1.0 = OctetString: "Dual core Intel notebook"
		/// 1.3.6.1.2.1.1.2.0 = ObjectId: 1.3.6.1.9.233233.1.1
		/// 1.3.6.1.2.1.1.3.0 = TimeTicks: 0d 0h 0m 1s 420ms
		/// 1.3.6.1.2.1.1.4.0 = OctetString: "msinadinovic@users.sourceforge.net"
		/// 1.3.6.1.2.1.1.5.0 = OctetString: "milans-nbook"
		/// 1.3.6.1.2.1.1.6.0 = OctetString: "Developer home"
		/// 1.3.6.1.2.1.1.8.0 = TimeTicks: 0d 0h 0m 0s 10ms
		/// </code>
		/// </example>
		/// <param name="pdu">Request Protocol Data Unit</param>
		/// <returns>Result of the SNMP request in a dictionary format with Oid => AsnType values</returns>
		public Dictionary<Oid, AsnType> GetBulk(Pdu pdu)
		{
			if (!Valid)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpException("SimpleSnmp class is not valid.");
				}
				return null; // class is not fully initialized.
			}
			try
			{
				pdu.NonRepeaters = _nonRepeaters;
				pdu.MaxRepetitions = _maxRepetitions;
				_target = new UdpTarget(_peerIP, _peerPort, _timeout, _retry);
			}
			catch(Exception ex)
			{
				_target = null;
				if (!_suppressExceptions)
				{
					throw ex;
				}
			}
			if( _target == null )
			{
				return null;
			}
			try
			{
				AgentParameters param = new AgentParameters(SnmpVersion.Ver2, new OctetString(_community));
				SnmpPacket result = _target.Request(pdu, param);
				if (result != null)
				{
					if (result.Pdu.ErrorStatus == 0)
					{
						Dictionary<Oid, AsnType> res = new Dictionary<Oid, AsnType>();
						foreach (Vb v in result.Pdu.VbList)
						{
							if (
								(v.Value.Type == SnmpConstants.SMI_ENDOFMIBVIEW) ||
								(v.Value.Type == SnmpConstants.SMI_NOSUCHINSTANCE) ||
								(v.Value.Type == SnmpConstants.SMI_NOSUCHOBJECT)
								)
							{
								break;
							}
							if (res.ContainsKey(v.Oid))
							{
								if (res[v.Oid].Type != v.Value.Type)
								{
									throw new SnmpException(SnmpException.OidValueTypeChanged, "OID value type changed for OID: " + v.Oid.ToString());
								}
								else
								{
									res[v.Oid] = v.Value;
								}
							}
							else
							{
								res.Add(v.Oid, v.Value);
							}
						}
						_target.Close();
						_target = null;
						return res;
					}
					else
					{
						if( ! _suppressExceptions )
						{
							throw new SnmpErrorStatusException("Agent responded with an error", result.Pdu.ErrorStatus, result.Pdu.ErrorIndex);
						}
					}
				}
			}
			catch(Exception ex)
			{
				if( ! _suppressExceptions )
				{
					_target.Close();
					_target = null;
					throw ex;
				}
			}
			_target.Close();
			_target = null;
			return null;
		}

		/// <summary>
		/// SNMP GET-BULK request
		/// </summary>
		/// <remarks>
		/// Performs a GetBulk SNMP v2 operation on a list of OIDs. This is a convenience function that
		/// calls GetBulk(Pdu) method.
		/// </remarks>
		/// <example>SNMP GET-BULK request:
		/// <code>
		/// String snmpAgent = "10.10.10.1";
		/// String snmpCommunity = "private";
		/// SimpleSnmp snmp = new SimpleSnmp(snmpAgent, snmpCommunity);
		/// Dictionary&lt;Oid, AsnType&gt; result = snmp.GetBulk(new string[] { "1.3.6.1.2.1.1" });
		/// if( result == null ) {
		///   Console.WriteLine("Request failed.");
		/// } else {
		///   foreach (KeyValuePair&lt;Oid, AsnType&gt; entry in result)
		///   {
		///     Console.WriteLine("{0} = {1}: {2}", entry.Key.ToString(), SnmpConstants.GetTypeName(entry.Value.Type),
		///       entry.Value.ToString());
		///   }
		/// }
		/// </code>
		/// </example>
		/// <param name="oidList">List of request OIDs in string dotted decimal format.</param>
		/// <returns>Result of the SNMP request in a dictionary format with Oid => AsnType values</returns>
		public Dictionary<Oid, AsnType> GetBulk(string[] oidList)
		{
			if (!Valid)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpException("SimpleSnmp class is not valid.");
				}
				return null;
			}
			Pdu pdu = new Pdu(PduType.GetBulk);
			pdu.MaxRepetitions = _maxRepetitions;
			pdu.NonRepeaters = _nonRepeaters;
			foreach (string s in oidList)
			{
				pdu.VbList.Add(s);
			}
			return GetBulk(pdu);
		}

		/// <summary>
		/// SNMP SET request
		/// </summary>
		/// <example>Set operation in SNMP version 1:
		/// <code>
		/// String snmpAgent = "10.10.10.1";
		/// String snmpCommunity = "private";
		/// SimpleSnmp snmp = new SimpleSnmp(snmpAgent, snmpCommunity);
		/// // Create a request Pdu
		/// Pdu pdu = new Pdu();
		/// pdu.Type = SnmpConstants.SET; // type SET
		/// Oid setOid = new Oid("1.3.6.1.2.1.1.1.0"); // sysDescr.0
		/// OctetString setValue = new OctetString("My personal toy");
		/// pdu.VbList.Add(setOid, setValue);
		/// Dictionary&lt;Oid, AsnType&gt; result = snmp.Set(SnmpVersion.Ver1, pdu);
		/// if( result == null ) {
		///   Console.WriteLine("Request failed.");
		/// } else {
		///   Console.WriteLine("Success!");
		/// }
		/// </code>
		/// 
		/// To use SNMP version 2, change snmp.Set() method call first parameter to SnmpVersion.Ver2.
		/// </example>
		/// <param name="version">SNMP protocol version number. Acceptable values are SnmpVersion.Ver1 and SnmpVersion.Ver2</param>
		/// <param name="pdu">Request Protocol Data Unit</param>
		/// <returns>Result of the SNMP request in a dictionary format with Oid => AsnType values</returns>
		public Dictionary<Oid, AsnType> Set(SnmpVersion version, Pdu pdu)
		{
			if (!Valid)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpException("SimpleSnmp class is not valid.");
				}
				return null; // class is not fully initialized.
			}
			// function only works on SNMP version 1 and SNMP version 2 requests
			if (version != SnmpVersion.Ver1 && version != SnmpVersion.Ver2)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpInvalidVersionException("SimpleSnmp support SNMP version 1 and 2 only.");
				}
				return null;
			}
			try
			{
				_target = new UdpTarget(_peerIP, _peerPort, _timeout, _retry);
			}
			catch(Exception ex)
			{
				_target = null;
				if( ! _suppressExceptions)
				{
					throw ex;
				}
			}
			if( _target == null )
			{
				return null;
			}
			try {
				AgentParameters param = new AgentParameters(version, new OctetString(_community));
				SnmpPacket result = _target.Request(pdu, param);
				if (result != null)
				{
					if (result.Pdu.ErrorStatus == 0)
					{
						Dictionary<Oid, AsnType> res = new Dictionary<Oid, AsnType>();
						foreach (Vb v in result.Pdu.VbList)
						{
							if (res.ContainsKey(v.Oid))
							{
								if (res[v.Oid].Type != v.Value.Type)
								{
									throw new SnmpException(SnmpException.OidValueTypeChanged, "OID value type changed for OID: " + v.Oid.ToString());
								}
								else
								{
									res[v.Oid] = v.Value;
								}
							}
							else
							{
								res.Add(v.Oid, v.Value);
							}
						}
						_target.Close();
						_target = null;
						return res;
					}
					else
					{
						if( ! _suppressExceptions )
						{
							throw new SnmpErrorStatusException("Agent responded with an error", result.Pdu.ErrorStatus, result.Pdu.ErrorIndex);
						}
					}
				}
			}
			catch(Exception ex)
			{
				if( ! _suppressExceptions )
				{
					_target.Close();
					_target = null;
					throw ex;
				}
			}
			_target.Close();
			_target = null;
			return null;
		}

		/// <summary>
		/// SNMP SET request
		/// </summary>
		/// <example>Set operation in SNMP version 1:
		/// <code>
		/// String snmpAgent = "10.10.10.1";
		/// String snmpCommunity = "private";
		/// SimpleSnmp snmp = new SimpleSnmp(snmpAgent, snmpCommunity);
		/// // Create a request Pdu
		/// List&lt;Vb&gt; vbList = new List&lt;Vb&gt;();
		/// Oid setOid = new Oid("1.3.6.1.2.1.1.1.0"); // sysDescr.0
		/// OctetString setValue = new OctetString("My personal toy");
		/// vbList.Add(new Vb(setOid, setValue));
		/// Dictionary&lt;Oid, AsnType&gt; result = snmp.Set(SnmpVersion.Ver1, list.ToArray());
		/// if( result == null ) {
		///   Console.WriteLine("Request failed.");
		/// } else {
		///   Console.WriteLine("Success!");
		/// }
		/// </code>
		/// 
		/// To use SNMP version 2, change snmp.Set() method call first parameter to SnmpVersion.Ver2.
		/// </example>
		/// <param name="version">SNMP protocol version number. Acceptable values are SnmpVersion.Ver1 and SnmpVersion.Ver2</param>
		/// <param name="vbs">Vb array containing Oid/AsnValue pairs for the SET operation</param>
		/// <returns>Result of the SNMP request in a dictionary format with Oid => AsnType values</returns>
		public Dictionary<Oid, AsnType> Set(SnmpVersion version, Vb[] vbs)
		{
			if (!Valid)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpException("SimpleSnmp class is not valid.");
				}
				return null;
			}
			// function only works on SNMP version 1 and SNMP version 2 requests
			if (version != SnmpVersion.Ver1 && version != SnmpVersion.Ver2)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpInvalidVersionException("SimpleSnmp support SNMP version 1 and 2 only.");
				}
				return null;
			}
			Pdu pdu = new Pdu(PduType.Set);
			foreach (Vb vb in vbs)
			{
				pdu.VbList.Add(vb);
			}
			return Set(version, pdu);
		}

		/// <summary>SNMP WALK operation</summary>
		/// <remarks>
		/// When using SNMP version 1, walk is performed using GET-NEXT calls. When using SNMP version 2, 
		/// walk is performed using GET-BULK calls.
		/// </remarks>
		/// <example>Example SNMP walk operation using SNMP version 1:
		/// <code>
		/// String snmpAgent = "10.10.10.1";
		/// String snmpCommunity = "private";
		/// SimpleSnmp snmp = new SimpleSnmp(snmpAgent, snmpCommunity);
		/// Dictionary&lt;Oid, AsnType&gt; result = snmp.Walk(SnmpVersion.Ver1, "1.3.6.1.2.1.1");
		/// if( result == null ) {
		///   Console.WriteLine("Request failed.");
		/// } else {
		/// foreach (KeyValuePair&lt;Oid, AsnType&gt; entry in result)
		/// {
		///   Console.WriteLine("{0} = {1}: {2}", entry.Key.ToString(), SnmpConstants.GetTypeName(entry.Value.Type),
		///     entry.Value.ToString());
		/// }
		/// </code>
		/// Will return:
		/// <code>
		/// 1.3.6.1.2.1.1.1.0 = OctetString: "Dual core Intel notebook"
		/// 1.3.6.1.2.1.1.2.0 = ObjectId: 1.3.6.1.9.233233.1.1
		/// 1.3.6.1.2.1.1.3.0 = TimeTicks: 0d 0h 0m 1s 420ms
		/// 1.3.6.1.2.1.1.4.0 = OctetString: "msinadinovic@users.sourceforge.net"
		/// 1.3.6.1.2.1.1.5.0 = OctetString: "milans-nbook"
		/// 1.3.6.1.2.1.1.6.0 = OctetString: "Developer home"
		/// 1.3.6.1.2.1.1.8.0 = TimeTicks: 0d 0h 0m 0s 10ms
		/// </code>
		/// 
		/// To use SNMP version 2, change snmp.Set() method call first parameter to SnmpVersion.Ver2.
		/// </example>
		/// <param name="version">SNMP protocol version. Acceptable values are SnmpVersion.Ver1 and 
		/// SnmpVersion.Ver2</param>
		/// <param name="rootOid">OID to start WALK operation from. Only child OIDs of the rootOid will be
		/// retrieved and returned</param>
		/// <returns>Oid => AsnType value mappings on success, empty dictionary if no data was found or
		/// null on error</returns>
		public Dictionary<Oid, AsnType> Walk(SnmpVersion version, string rootOid)
		{
			if (!Valid)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpException("SimpleSnmp class is not valid.");
				}
				return null;
			}
			// function only works on SNMP version 1 and SNMP version 2 requests
			if (version != SnmpVersion.Ver1 && version != SnmpVersion.Ver2)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpInvalidVersionException("SimpleSnmp support SNMP version 1 and 2 only.");
				}
				return null;
			}
			if (rootOid.Length < 2)
			{
				if( ! _suppressExceptions)
				{
					throw new SnmpException(SnmpException.InvalidOid, "RootOid is not a valid Oid");
				}
				return null;
			}
			Oid root = new Oid(rootOid);
			if (root.Length <= 0)
			{
				return null; // unable to parse root oid
			}
			Oid lastOid = (Oid)root.Clone();

			Dictionary<Oid, AsnType> result = new Dictionary<Oid, AsnType>();
			while (lastOid != null && root.IsRootOf(lastOid))
			{
				Dictionary<Oid, AsnType> val = null;
				if (version == SnmpVersion.Ver1)
				{
					val = GetNext(version, new string[] { lastOid.ToString() });
				}
				else
				{
					val = GetBulk(new string[] { lastOid.ToString() });
				}
				// check that we have a result
				if (val == null)
				{
					// error of some sort happened. abort...
					return null;
				}
				foreach (KeyValuePair<Oid, AsnType> entry in val)
				{
					if (root.IsRootOf(entry.Key))
					{
						if (result.ContainsKey(entry.Key))
						{
							if (result[entry.Key].Type != entry.Value.Type)
							{
								throw new SnmpException(SnmpException.OidValueTypeChanged, "OID value type changed for OID: " + entry.Key.ToString());
							}
							else
								result[entry.Key] = entry.Value;
						}
						else
						{
							result.Add(entry.Key, entry.Value);
						}
						lastOid = (Oid)entry.Key.Clone();
					}
					else
					{
						// it's faster to check if variable is null then checking IsRootOf
						lastOid = null;
						break;
					}
				}
			}
			return result;
		}

		#region Properties

		/// <summary>
		/// Get/Set peer IP address
		/// </summary>
		public IPAddress PeerIP
		{
			get { return _peerIP; }
			set { _peerIP = value; }
		}
		/// <summary>
		/// Get/Set peer name
		/// </summary>
		public string PeerName
		{
			get { return _peerName; }
			set { 
				_peerName = value;
				Resolve();
			}
		}
		/// <summary>
		/// Get/Set peer port number
		/// </summary>
		public int PeerPort
		{
			get { return _peerPort; }
			set { _peerPort = value; }
		}
		/// <summary>
		/// Get set timeout value in milliseconds
		/// </summary>
		public int Timeout
		{
			get { return _timeout; }
			set { _timeout = value; }
		}
		/// <summary>
		/// Get/Set maximum retry count
		/// </summary>
		public int Retry
		{
			get { return _retry; }
			set { _retry = value; }
		}
		/// <summary>
		/// Get/Set SNMP community name
		/// </summary>
		public string Community
		{
			get { return _community; }
			set { _community = value; }
		}

		/// <summary>
		/// Get/Set NonRepeaters value
		/// </summary>
		/// <remarks>NonRepeaters value will only be used by SNMPv2 GET-BULK requests. Any other
		/// request type will ignore this value.</remarks>
		public int NonRepeaters
		{
			get { return _nonRepeaters; }
			set { _nonRepeaters = value; }
		}
		/// <summary>
		/// Get/Set MaxRepetitions value
		/// </summary>
		/// <remarks>MaxRepetitions value will only be used by SNMPv2 GET-BULK requests. Any other
		/// request type will ignore this value.</remarks>
		public int MaxRepetitions
		{
			get { return _maxRepetitions; }
			set { _maxRepetitions = value; }
		}

		#endregion

		#region Utility functions

		/// <summary>
		/// Resolve peer name to an IP address
		/// </summary>
		/// <remarks>
		/// This method will not throw any exceptions, even on failed DNS resolution. Make
		/// sure you call SimpleSnmp.Valid property to verify class state.
		/// </remarks>
		/// <returns>true if DNS resolution is successful, otherwise false</returns>
		internal bool Resolve()
		{
			_peerIP = IPAddress.None;
			if (_peerName.Length > 0)
			{
				if (IPAddress.TryParse(_peerName, out _peerIP))
				{
					return true;
				}
				else
				{
					_peerIP = IPAddress.None; // just in case
				}
				IPHostEntry he = null;
				try
				{
					he = Dns.GetHostEntry(_peerName);
				}
				catch(Exception ex)
				{
					if( ! _suppressExceptions )
					{
						throw ex;
					}
					he = null;
				}
				if (he == null)
					return false; // resolution failed
				foreach (IPAddress tmp in he.AddressList)
				{
					if (tmp.AddressFamily == AddressFamily.InterNetwork)
					{
						_peerIP = tmp;
						break;
					}
				}
				if (_peerIP != IPAddress.None)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Get/Set exception suppression flag.
		/// 
		/// If exceptions are suppressed all methods return null on any and all errors. With suppression disabled, you can
		/// capture error details in try/catch blocks around method calls.
		/// </summary>
		public bool SuppressExceptions
		{
			get
			{
				return _suppressExceptions;
			}
			set
			{
				_suppressExceptions = value;
			}
		}


		#endregion
	}
}

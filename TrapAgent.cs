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
// First introduced: 0.5.2
//
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace SnmpSharpNet
{
	/// <summary>
	/// Send SNMP Trap notifications
	/// </summary>
	/// <remarks>
	/// TrapAgent class is used to hide Socket operations from users and provide an easy method to send
	/// Trap notifications.
	/// 
	/// To use the class, you can use the TrapAgent class protocol specific members, recommended when you
	/// expect to send a lot of notifications, or a static helper TrapAgent.SendTrap method which will
	/// construct a new socket for each call.
	/// </remarks>
	public class TrapAgent
	{
		/// <summary>
		/// Internal Socket class
		/// </summary>
		protected Socket _sock;
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// Constructor initializes an internal Socket used to send traps. Socket is initialized by selecting a
		/// random UDP port number.
		/// </remarks>
		public TrapAgent()
		{
			_sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			_sock.Bind(new IPEndPoint(IPAddress.Any, 0));
		}
		/// <summary>
		/// Destructor.
		/// </summary>
		/// <remarks>Destructors only purpose is to close the Socket used by the class.</remarks>
		~TrapAgent()
		{
			_sock.Close();
		}
		/// <summary>
		/// Send SNMP version 1 Trap notification
		/// </summary>
		/// <param name="packet">SNMP v1 Trap packet class</param>
		/// <param name="peer">Manager (receiver) IP address</param>
		/// <param name="port">Manager (receiver) UDP port number</param>
		public void SendV1Trap(SnmpV1TrapPacket packet, IpAddress peer, int port)
		{
			byte[] outBuffer = packet.encode();
			_sock.SendTo(outBuffer, new IPEndPoint((IPAddress)peer, port));
		}
		/// <summary>
		/// Construct and send SNMP v1 Trap
		/// </summary>
		/// <param name="receiver">Receiver IP address</param>
		/// <param name="receiverPort">Receiver UDP port number</param>
		/// <param name="community">SNMP community name</param>
		/// <param name="senderSysObjectID">Senders sysObjectID</param>
		/// <param name="senderIpAdress">Sender IP address</param>
		/// <param name="genericTrap">Generic trap code</param>
		/// <param name="specificTrap">Specific trap code</param>
		/// <param name="senderUpTime">Senders sysUpTime</param>
		/// <param name="varList">Variable binding list</param>
		public void SendV1Trap(IpAddress receiver, int receiverPort, string community, Oid senderSysObjectID, 
			IpAddress senderIpAdress, Int32 genericTrap, Int32 specificTrap, UInt32 senderUpTime, 
			VbCollection varList)
		{
			SnmpV1TrapPacket packet = new SnmpV1TrapPacket(community);
			packet.Pdu.Generic = genericTrap;
			packet.Pdu.Specific = specificTrap;
			packet.Pdu.AgentAddress.Set(senderIpAdress);
			packet.Pdu.TimeStamp = senderUpTime;
			packet.Pdu.VbList.Add(varList);
			packet.Pdu.Enterprise.Set(senderSysObjectID);
			SendV1Trap(packet, receiver, receiverPort);
		}
		/// <summary>
		/// Send SNMP version 2 Trap notification
		/// </summary>
		/// <param name="packet">SNMP v2 Trap packet class</param>
		/// <param name="peer">Manager (receiver) IP address</param>
		/// <param name="port">Manager (receiver) UDP port number</param>
		public void SendV2Trap(SnmpV2Packet packet, IpAddress peer, int port)
		{
			if (packet.Pdu.Type != PduType.V2Trap)
				throw new SnmpInvalidPduTypeException("Invalid Pdu type.");
			byte[] outBuffer = packet.encode();
			_sock.SendTo(outBuffer, new IPEndPoint((IPAddress)peer, port));
		}

		/// <summary>
		/// Construct and send SNMP v2 Trap
		/// </summary>
		/// <param name="receiver">Trap receiver IP address</param>
		/// <param name="receiverPort">Trap receiver UDP port number</param>
		/// <param name="community">SNMP community name</param>
		/// <param name="senderUpTime">Sender sysUpTime</param>
		/// <param name="trapObjectID">Trap ObjectID</param>
		/// <param name="varList">Variable binding list</param>
		public void SendV2Trap(IpAddress receiver, int receiverPort, string community, UInt32 senderUpTime,
			Oid trapObjectID, VbCollection varList)
		{
			SnmpV2Packet packet = new SnmpV2Packet(community);
			packet.Pdu.Type = PduType.V2Trap;
			packet.Pdu.TrapObjectID = trapObjectID;
			packet.Pdu.TrapSysUpTime.Value = senderUpTime;
			packet.Pdu.SetVbList(varList);
			SendV2Trap(packet, receiver, receiverPort);
		}
		/// <summary>
		/// Send SNMP version 3 Trap notification
		/// </summary>
		/// <param name="packet">SNMP v3 Trap packet class</param>
		/// <param name="peer">Manager (receiver) IP address</param>
		/// <param name="port">Manager (receiver) UDP port number</param>
		public void SendV3Trap(SnmpV3Packet packet, IpAddress peer, int port)
		{
			if( packet.Pdu.Type != PduType.V2Trap )
				throw new SnmpInvalidPduTypeException("Invalid Pdu type.");
			byte[] outBuffer = packet.encode();
			_sock.SendTo(outBuffer, new IPEndPoint((IPAddress)peer, port));
		}

		/// <summary>
		/// Construct and send SNMP v3 noAuthNoPriv Trap
		/// </summary>
		/// <param name="receiver">Trap receiver IP address</param>
		/// <param name="receiverPort">Trap receiver UDP port number</param>
		/// <param name="engineId">Sender SNMP engineId</param>
		/// <param name="senderEngineBoots">Sender SNMP engine boots</param>
		/// <param name="senderEngineTime">Sender SNMP engine time</param>
		/// <param name="senderUserName">Security (user) name</param>
		/// <param name="senderUpTime">Sender upTime</param>
		/// <param name="trapObjectID">Trap object ID</param>
		/// <param name="varList">Variable binding list</param>
		public void SendV3Trap(IpAddress receiver, int receiverPort, byte[] engineId, Int32 senderEngineBoots,
			Int32 senderEngineTime, string senderUserName, UInt32 senderUpTime, Oid trapObjectID, VbCollection varList)
		{
			SnmpV3Packet packet = new SnmpV3Packet();
			packet.Pdu.Type = PduType.V2Trap;
			packet.NoAuthNoPriv(ASCIIEncoding.UTF8.GetBytes(senderUserName));
			packet.SetEngineId(engineId);
			packet.SetEngineTime(senderEngineBoots, senderEngineTime);
			packet.ScopedPdu.TrapObjectID.Set(trapObjectID);
			packet.ScopedPdu.TrapSysUpTime.Value = senderUpTime;
			packet.ScopedPdu.VbList.Add(varList);
			packet.MsgFlags.Reportable = false;
			SendV3Trap(packet, receiver, receiverPort);
		}

		/// <summary>
		/// Construct and send SNMP v3 authNoPriv Trap
		/// </summary>
		/// <param name="receiver">Trap receiver IP address</param>
		/// <param name="receiverPort">Trap receiver UDP port number</param>
		/// <param name="engineId">Sender SNMP engineId</param>
		/// <param name="senderEngineBoots">Sender SNMP engine boots</param>
		/// <param name="senderEngineTime">Sender SNMP engine time</param>
		/// <param name="senderUserName">Security (user) name</param>
		/// <param name="senderUpTime">Sender upTime</param>
		/// <param name="trapObjectID">Trap object ID</param>
		/// <param name="varList">Variable binding list</param>
		/// <param name="authDigest">Authentication digest. <see cref="AuthenticationDigests"/> enumeration for
		/// available digests</param>
		/// <param name="authSecret">Authentication secret</param>
		public void SendV3Trap(IpAddress receiver, int receiverPort, byte[] engineId, Int32 senderEngineBoots,
			Int32 senderEngineTime, string senderUserName, UInt32 senderUpTime, Oid trapObjectID, VbCollection varList,
			AuthenticationDigests authDigest, byte[] authSecret)
		{
			SnmpV3Packet packet = new SnmpV3Packet();
			packet.Pdu.Type = PduType.V2Trap;
			packet.authNoPriv(ASCIIEncoding.UTF8.GetBytes(senderUserName), authSecret, authDigest);
			packet.SetEngineId(engineId);
			packet.SetEngineTime(senderEngineBoots, senderEngineTime);
			packet.ScopedPdu.TrapObjectID.Set(trapObjectID);
			packet.ScopedPdu.TrapSysUpTime.Value = senderUpTime;
			packet.ScopedPdu.VbList.Add(varList);
			packet.MsgFlags.Reportable = false;
			SendV3Trap(packet, receiver, receiverPort);
		}

		/// <summary>
		/// Construct and send SNMP v3 authPriv Trap
		/// </summary>
		/// <param name="receiver">Trap receiver IP address</param>
		/// <param name="receiverPort">Trap receiver UDP port number</param>
		/// <param name="engineId">Sender SNMP engineId</param>
		/// <param name="senderEngineBoots">Sender SNMP engine boots</param>
		/// <param name="senderEngineTime">Sender SNMP engine time</param>
		/// <param name="senderUserName">Security (user) name</param>
		/// <param name="senderUpTime">Sender upTime</param>
		/// <param name="trapObjectID">Trap object ID</param>
		/// <param name="varList">Variable binding list</param>
		/// <param name="authDigest">Authentication digest. See <see cref="AuthenticationDigests"/> enumeration for
		/// available digests</param>
		/// <param name="authSecret">Authentication secret</param>
		/// <param name="privProtocol">Privacy protocol. See <see cref="PrivacyProtocols"/> enumeration for
		/// available privacy protocols.</param>
		/// <param name="privSecret">Privacy secret</param>
		public void SendV3Trap(IpAddress receiver, int receiverPort, byte[] engineId, Int32 senderEngineBoots,
			Int32 senderEngineTime, string senderUserName, UInt32 senderUpTime, Oid trapObjectID, VbCollection varList,
			AuthenticationDigests authDigest, byte[] authSecret, PrivacyProtocols privProtocol, byte[] privSecret)
		{
			SnmpV3Packet packet = new SnmpV3Packet();
			packet.Pdu.Type = PduType.V2Trap;
			packet.authPriv(ASCIIEncoding.UTF8.GetBytes(senderUserName), authSecret, authDigest,privSecret, privProtocol);
			packet.SetEngineId(engineId);
			packet.SetEngineTime(senderEngineBoots, senderEngineTime);
			packet.ScopedPdu.TrapObjectID.Set(trapObjectID);
			packet.ScopedPdu.TrapSysUpTime.Value = senderUpTime;
			packet.ScopedPdu.VbList.Add(varList);
			packet.MsgFlags.Reportable = false;
			SendV3Trap(packet, receiver, receiverPort);
		}
		
		/// <summary>
		/// Send SNMP Trap notification
		/// </summary>
		/// <remarks>
		/// Helper function to allow for seamless sending of SNMP notifications for all protocol versions.
		/// 
		/// packet parameter should be appropriately formatted SNMP notification in SnmpV1TrapPacket,
		/// SnmpV2Packet or SnmpV3Packet class cast as SnmpPacket class.
		/// 
		/// Function will determine which version of the notification is to be used by checking the type
		/// of the packet parameter and call appropriate TrapAgent member function to send it.
		/// </remarks>
		/// <param name="packet">SNMP trap packet</param>
		/// <param name="peer">Manager (receiver) IP address</param>
		/// <param name="port">Manager (receiver) UDP port number</param>
		public static void SendTrap(SnmpPacket packet, IpAddress peer, int port)
		{
			TrapAgent agent = new TrapAgent();
			if (packet is SnmpV1TrapPacket)
				agent.SendV1Trap((SnmpV1TrapPacket)packet, peer, port);
			else if (packet is SnmpV2Packet)
				agent.SendV2Trap((SnmpV2Packet)packet, peer, port);
			else if (packet is SnmpV3Packet)
				agent.SendV3Trap((SnmpV3Packet)packet, peer, port);
			else
				throw new SnmpException("Invalid SNMP packet type.");
		}
	}
}

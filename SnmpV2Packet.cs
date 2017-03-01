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
using System.Text;

namespace SnmpSharpNet
{
	/// <summary>
	/// SNMP version 2 packet class.
	/// </summary>
	/// 
	/// <remarks>
	/// Available packet classes are:
	/// <ul>
	/// <li><see cref="SnmpV1Packet"/></li>
	/// <li><see cref="SnmpV1TrapPacket"/></li>
	/// <li><see cref="SnmpV2Packet"/></li>
	/// <li><see cref="SnmpV3Packet"/></li>
	/// </ul>
	/// 
	/// This class is provided to simplify encoding and decoding of packets and to provide consistent interface
	/// for users who wish to handle transport part of protocol on their own without using the <see cref="UdpTarget"/> 
	/// class.
	/// 
	/// <see cref="SnmpPacket"/> and derived classes have been developed to implement SNMP version 1, 2 and 3 packet 
	/// support. 
	/// 
	/// For SNMP version 1 and 2 packet, <see cref="SnmpV1Packet"/> and <see cref="SnmpV2Packet"/> classes 
	/// provides  sufficient support for encoding and decoding data to/from BER buffers to satisfy requirements 
	/// of most applications. 
	/// 
	/// SNMP version 3 on the other hand requires a lot more information to be passed to the encoder method and 
	/// returned by the decode method. While using SnmpV3Packet class for full packet handling is possible, transport
	/// specific class <see cref="UdpTarget"/> uses <see cref="SecureAgentParameters"/> class to store protocol
	/// version 3 specific information that carries over from request to request when used on the same SNMP agent
	/// and therefore simplifies both initial definition of agents configuration (mostly security) as well as
	/// removes the need for repeated initialization of the packet class for subsequent requests.
	/// 
	/// If you decide not to use transport helper class(es) like <see cref="UdpTarget"/>, BER encoding and
	/// decoding and packets is easily done with SnmpPacket derived classes.
	/// 
	/// Example, SNMP version 2 packet encoding:
	/// <code>
	/// SnmpV2Packet packetv2 = new SnmpV2Packet();
	/// packetv2.Community.Set("public");
	/// packetv2.Pdu.Set(mypdu);
	/// byte[] berpacket = packetv2.encode();
	/// </code>
	/// 
	/// Example, SNMP version 2 packet decoding:
	/// <code>
	/// SnmpV2Packet packetv2 = new SnmpV2Packet();
	/// packetv2.decode(inbuffer,inlen);
	/// </code>
	/// </remarks>
	public class SnmpV2Packet: SnmpPacket
	{
		/// <summary>
		/// SNMP community name
		/// </summary>
		protected OctetString _snmpCommunity;

		/// <summary>
		/// Get SNMP community value used by SNMP version 1 and version 2 protocols.
		/// </summary>
		public OctetString Community
		{
			get
			{
				return _snmpCommunity;
			}
		}

		/// <summary>
		/// SNMP Protocol Data Unit
		/// </summary>
		public Pdu _pdu;

		/// <summary>
		/// Access to the packet <see cref="Pdu"/>.
		/// </summary>
		public override SnmpSharpNet.Pdu Pdu
		{
			get { return _pdu; }
		}

		/// <summary>
		/// Standard constructor.
		/// </summary>
		public SnmpV2Packet()
			: base(SnmpVersion.Ver2)
		{
			_protocolVersion.Value = (int)SnmpVersion.Ver2;
			_pdu = new Pdu();
			_snmpCommunity = new OctetString();
		}
		/// <summary>
		/// Standard constructor.
		/// </summary>
		/// <param name="snmpCommunity">SNMP community name for the packet</param>
		public SnmpV2Packet(string snmpCommunity)
			: this()
		{
			_snmpCommunity.Set(snmpCommunity);
		}

		#region Encode & Decode methods

		/// <summary>
		/// Decode received SNMP packet.
		/// </summary>
		/// <param name="buffer">BER encoded packet buffer</param>
		/// <param name="length">BER encoded packet buffer length</param>
		/// <exception cref="SnmpException">Thrown when invalid encoding has been found in the packet</exception>
		/// <exception cref="OverflowException">Thrown when parsed header points to more data then is available in the packet</exception>
		/// <exception cref="SnmpInvalidVersionException">Thrown when parsed packet is not SNMP version 1</exception>
		/// <exception cref="SnmpInvalidPduTypeException">Thrown when received PDU is of a type not supported by SNMP version 1</exception>
		public override int decode(byte[] buffer, int length)
		{
			MutableByte buf = new MutableByte(buffer, length);

			int headerLength;
			int offset = 0;

			offset = base.decode(buffer, buffer.Length);

			if (Version != SnmpVersion.Ver2)
				throw new SnmpInvalidVersionException("Invalid protocol version");

			offset = _snmpCommunity.decode(buf, offset);
			int tmpOffset = offset;
			byte asnType = AsnType.ParseHeader(buf, ref tmpOffset, out headerLength);

			// Check packet length
			if (headerLength + offset > buf.Length)
				throw new OverflowException("Insufficient data in packet");


			if (asnType != (byte)PduType.Get && asnType != (byte)PduType.GetNext && asnType != (byte)PduType.Set &&
				asnType != (byte)PduType.GetBulk && asnType != (byte)PduType.Response && asnType != (byte)PduType.V2Trap &&
				asnType != (byte)PduType.Inform)
				throw new SnmpInvalidPduTypeException("Invalid SNMP operation received: " + string.Format("0x{0:x2}", asnType));

			// Now process the Protocol Data Unit
			offset = this.Pdu.decode(buf, offset);
			return length;
		}
		/// <summary>
		/// Encode SNMP packet for sending.
		/// </summary>
		/// <returns>BER encoded SNMP packet.</returns>
		public override byte[] encode()
		{
			MutableByte buf = new MutableByte();

			if (this.Pdu.Type != PduType.Get && this.Pdu.Type != PduType.GetNext &&
				this.Pdu.Type != PduType.Set && this.Pdu.Type != PduType.V2Trap &&
				this.Pdu.Type != PduType.Response && this.Pdu.Type != PduType.GetBulk &&
				this.Pdu.Type != PduType.Inform)
				throw new SnmpInvalidPduTypeException("Invalid SNMP PDU type while attempting to encode PDU: " + string.Format("0x{0:x2}", this.Pdu.Type));

			// snmp version
			_protocolVersion.encode(buf);

			// community string
			_snmpCommunity.encode(buf);

			// pdu
			_pdu.encode(buf);

			// wrap the packet into a sequence
			MutableByte tmpBuf = new MutableByte();
			AsnType.BuildHeader(tmpBuf, SnmpConstants.SMI_SEQUENCE, buf.Length);

			buf.Prepend(tmpBuf);
			return buf;
		}

		#endregion

		#region Helper methods

		/// <summary>
		/// Build SNMP RESPONSE packet for the received INFORM packet.
		/// </summary>
		/// <returns>SNMP version 2 packet containing RESPONSE to the INFORM packet contained in the class instance.</returns>
		public SnmpV2Packet BuildInformResponse()
		{
			return SnmpV2Packet.BuildInformResponse(this);
		}

		/// <summary>
		/// Build SNMP RESPONSE packet for the INFORM packet class.
		/// </summary>
		/// <param name="informPacket">SNMP INFORM packet</param>
		/// <returns>SNMP version 2 packet containing RESPONSE to the INFORM packet contained in the parameter.</returns>
		/// <exception cref="SnmpInvalidPduTypeException">Parameter is not an INFORM SNMP version 2 packet class</exception>
		/// <exception cref="SnmpInvalidVersionException">Parameter is not a SNMP version 2 packet</exception>
		public static SnmpV2Packet BuildInformResponse(SnmpV2Packet informPacket)
		{
			if (informPacket.Version != SnmpVersion.Ver2)
				throw new SnmpInvalidVersionException("INFORM packet can only be parsed from an SNMP version 2 packet.");
			if (informPacket.Pdu.Type != PduType.Inform)
				throw new SnmpInvalidPduTypeException("Inform response can only be built for INFORM packets.");

			SnmpV2Packet response = new SnmpV2Packet(informPacket.Community.ToString());
			response.Pdu.Type = PduType.Response;
			response.Pdu.TrapObjectID.Set(informPacket.Pdu.TrapObjectID);
			response.Pdu.TrapSysUpTime.Value = informPacket.Pdu.TrapSysUpTime.Value;
			foreach (Vb v in informPacket.Pdu.VbList)
			{
				response.Pdu.VbList.Add(v.Oid, v.Value);
			}
			response.Pdu.RequestId = informPacket.Pdu.RequestId;

			return response;
		}

		#endregion

		/// <summary>
		/// String representation of the SNMP v1 Packet contents.
		/// </summary>
		/// <returns>String representation of the class.</returns>
		public override string ToString()
		{
			StringBuilder str = new StringBuilder();
			str.AppendFormat("SnmpV2Packet:\nCommunity: {0}\n{1}\n", Community.ToString(), Pdu.ToString());
			return str.ToString();
		}

	}
}

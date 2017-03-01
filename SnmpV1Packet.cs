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
	/// SNMP version 1 packet class.
	/// </summary>
	/// <remarks>
	/// Supported request types are SNMP-GET, SNMP-GETNEXT, SNMP-SET and SNMP-RESPONSE.
	/// 
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
	/// Example, SNMP version 1 packet encoding:
	/// <code>
	/// SnmpV1Packet packetv1 = new SnmpV1Packet();
	/// packetv1.Community.Set("public");
	/// packetv1.Pdu.Set(mypdu);
	/// byte[] berpacket = packetv1.encode();
	/// </code>
	/// 
	/// Example, SNMP version 1 packet decoding:
	/// <code>
	/// SnmpV1Packet packetv1 = new SnmpV1Packet();
	/// packetv1.decode(inbuffer,inlen);
	/// </code>
	/// </remarks>
	public class SnmpV1Packet : SnmpPacket
	{
		/// <summary>
		/// Standard constructor.
		/// </summary>
		public SnmpV1Packet()
            : base(SnmpVersion.Ver1)
		{
			_snmpCommunity = new OctetString();
			_pdu = new Pdu();
		}

		/// <summary>
		/// Standard constructor.
		/// </summary>
		/// <param name="snmpCommunity">SNMP community name for the packet</param>
		public SnmpV1Packet(string snmpCommunity)
			:this()
		{
			_snmpCommunity.Set(snmpCommunity);
		}

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
		/// Decode received SNMP packet.
		/// </summary>
		/// <param name="buffer">BER encoded packet buffer</param>
		/// <param name="length">BER encoded packet buffer length</param>
		/// <returns>Buffer position after the decoded packet.</returns>
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

			if (_protocolVersion.Value != (int)SnmpVersion.Ver1 )
				throw new SnmpInvalidVersionException("Invalid protocol version");

			offset = _snmpCommunity.decode(buf, offset);
			int tmpOffset = offset;
			byte asnType = AsnType.ParseHeader(buf, ref tmpOffset, out headerLength);

			// Check packet length
			if (headerLength + offset > buf.Length)
				throw new OverflowException("Insufficient data in packet");


			if (asnType != (byte)PduType.Get && asnType != (byte)PduType.GetNext && asnType != (byte)PduType.Set && asnType != (byte)PduType.Response)
				throw new SnmpInvalidPduTypeException("Invalid SNMP operation received: " + string.Format("0x{0:x2}", asnType));
			// Now process the Protocol Data Unit
			offset = this.Pdu.decode(buf, offset);
			return length;
		}

		/// <summary>
		/// Replacement for the base class encode method.  
		/// </summary>
		/// <param name="buffer">Buffer</param>
		private new void encode(MutableByte buffer)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Encode SNMP packet for sending.
		/// </summary>
		/// <returns>BER encoded SNMP packet.</returns>
		/// <exception cref="SnmpInvalidPduTypeException">Thrown when PDU being encoded is not a valid SNMP version 1 PDU. Acceptable 
		/// protocol version 1 operations are GET, GET-NEXT, SET and RESPONSE.</exception>
		public override byte[] encode()
		{
			if (this.Pdu.Type != PduType.Get && this.Pdu.Type != PduType.GetNext && 
				this.Pdu.Type != PduType.Set && this.Pdu.Type != PduType.Response)
				throw new SnmpInvalidVersionException("Invalid SNMP PDU type while attempting to encode PDU: " + string.Format("0x{0:x2}", this.Pdu.Type));
			if (this.Pdu.RequestId == 0)
			{
				System.Random rand = new System.Random((System.Int32)DateTime.Now.Ticks);
				this.Pdu.RequestId = rand.Next();
			}
			MutableByte tmpBuffer = new MutableByte();
			// snmp version
			_protocolVersion.encode(tmpBuffer);

			// community string
			_snmpCommunity.encode(tmpBuffer);

			// pdu
			this.Pdu.encode(tmpBuffer);

			MutableByte buf = new MutableByte();

			// wrap the packet into a sequence
			AsnType.BuildHeader(buf, SnmpConstants.SMI_SEQUENCE, tmpBuffer.Length);

			buf.Append(tmpBuffer);
			return buf;
		}
		/// <summary>
		/// String representation of the SNMP v1 Packet contents.
		/// </summary>
		/// <returns>String representation of the class.</returns>
		public override string ToString()
		{
			StringBuilder str = new StringBuilder();
			str.AppendFormat("SnmpV1Packet:\nCommunity: {0}\n{1}\n", Community.ToString(), Pdu.ToString());
			return str.ToString();
		}
	}
}

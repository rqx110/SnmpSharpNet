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
	/// <summary>SNMP version 1 TRAP packet class.</summary>
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
	/// <see cref="SnmpPacket"/> and derived classes have been developed to implement SNMP packet support. For
	/// SNMP version 1 and 2 packet, <see cref="SnmpV1Packet"/> and <see cref="SnmpV2Packet"/> classes provides 
	/// sufficient support for encoding and decoding data to/from BER buffers to satisfy requirements of most 
	/// applications. 
	/// 
	/// SNMP version 3 on the other hand requires a lot more information to be passed to the encoder method and 
	/// returned by the decode method. Attempt of implementing SNMP version 3 as part of <see cref="SnmpV3Packet"/> 
	/// class was operational but required too many function arguments to operate so a different interface was 
	/// developed using dedicated <see cref="SnmpV3Packet"/> class. 
	/// </remarks>
	public class SnmpV1TrapPacket : SnmpPacket
	{
		/// <summary>
		/// SNMP Protocol Data Unit
		/// </summary>
		protected TrapPdu _pdu;

		/// <summary>
		/// Access to the packet <see cref="TrapPdu"/>.
		/// </summary>
		public new TrapPdu Pdu
		{
			get { return _pdu; }
		}

		/// <summary>
		/// Get TrapPdu
		/// </summary>
		public TrapPdu TrapPdu
		{
			get
			{
				return _pdu;
			}
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
			get { return _snmpCommunity; }
		}

		/// <summary>
		/// Standard constructor.
		/// </summary>
		public SnmpV1TrapPacket()
			: base(SnmpVersion.Ver1)
		{
			_snmpCommunity = new OctetString();
			_pdu = new TrapPdu();
		}

		/// <summary>
		/// Standard constructor.
		/// </summary>
		/// <param name="snmpCommunity">SNMP community name for the packet</param>
		public SnmpV1TrapPacket(string snmpCommunity)
			: this()
		{
			_snmpCommunity.Set(snmpCommunity);
		}

		/// <summary>
		/// Decode received packet. This method overrides the base implementation that cannot be used with this type of the packet.
		/// </summary>
		/// <param name="buffer">Packet buffer</param>
		/// <param name="length">Buffer length</param>
		public override int decode(byte[] buffer, int length)
		{
			int offset = 0;
			MutableByte buf = new MutableByte(buffer, length);

			offset = base.decode(buffer, length);


			// parse community
			offset = _snmpCommunity.decode(buf, offset);

			// look ahead to make sure this is a TRAP packet
			int tmpOffset = offset;
			int headerLen;
			byte tmpAsnType = AsnType.ParseHeader(buffer, ref tmpOffset, out headerLen);
			if (tmpAsnType != (byte)PduType.Trap)
			{
				throw new SnmpException(string.Format("Invalid SNMP ASN.1 type. Received: {0:x2}", tmpAsnType));
			}
			// decode protocol data unit
			offset = this.Pdu.decode(buf, offset);
			return offset;
		}

		/// <summary>
		/// Encode SNMP packet for sending.
		/// </summary>
		/// <returns>BER encoded SNMP packet.</returns>
		public override byte[] encode()
		{
			MutableByte tmpBuffer = new MutableByte();

			//
			// encode the community strings
			_snmpCommunity.encode(tmpBuffer);
			this.Pdu.encode(tmpBuffer);

			base.encode(tmpBuffer);

			return (byte[])tmpBuffer;
		}
	}
}

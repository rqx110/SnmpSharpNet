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
	/// <summary>Base SNMP packet class.</summary>
	/// <remarks>
	/// All SNMP packets begin with the SMI_SEQUENCE header and SNMP protocol version number.
	/// This class parses and encodes these values. Derived classes parse further information from SNMP packets.
	/// </remarks>
	public abstract class SnmpPacket
	{
		/// <summary>
		/// SNMP protocol version
		/// </summary>
		protected Integer32 _protocolVersion;
		/// <summary>
		/// SNMP Protocol version
		/// </summary>
		public SnmpVersion Version
		{
			get 
			{ 
				return (SnmpVersion)_protocolVersion.Value; 
			}
		}
		/// <summary>
		/// Get Pdu
		/// </summary>
		public virtual Pdu Pdu
		{
			get
			{
				return null;
			}
		}
		/// <summary>
		/// Constructor. Sets SNMP version to SNMPV1.
		/// </summary>
		public SnmpPacket()
		{
			_protocolVersion = new Integer32((int)SnmpVersion.Ver1);
		}
		/// <summary>
		/// Constructor. Initialize SNMP version as supplied.
		/// </summary>
		/// <param name="protocolVersion">Protocol version. Acceptable values are SnmpConstants.SNMPV1, 
		/// SnmpConstants.SNMPV2 and SnmpConstants.SNMPV3</param>
		public SnmpPacket(SnmpVersion protocolVersion)
		{
			_protocolVersion = new Integer32((int)protocolVersion);
		}
		/// <summary>
		/// Decode SNMP packet header. This class decodes the initial sequence and SNMP protocol version
		/// number.
		/// </summary>
		/// <param name="buffer">BER encoded SNMP packet</param>
		/// <param name="length">Packet length</param>
		/// <returns>Offset position after the initial sequence header and protocol version value</returns>
		/// <exception cref="SnmpDecodingException">Thrown when invalid sequence type is found at the start of the SNMP packet being decoded</exception>
		public virtual int decode(byte[] buffer, int length)
		{
			int offset = 0;
			if (length < 2)
			{
				// we need at least 2 bytes
				throw new OverflowException("Packet too small.");
			}
			// make sure you get the right length buffer to be able to check for over/under flow errors
			MutableByte buf = new MutableByte(buffer, length);
			Sequence seq = new Sequence();
			offset = seq.decode(buf, offset);
			if( seq.Type != SnmpConstants.SMI_SEQUENCE )
				throw new SnmpDecodingException("Invalid sequence type at the start of the SNMP packet.");
			offset = _protocolVersion.decode(buf, offset);
			return offset;
		}
		/// <summary>
		/// Place holder for derived class implementations.
		/// </summary>
		/// <returns>Nothing</returns>
		public abstract byte[] encode();

		/// <summary>
		/// Wrap BER encoded SNMP information contained in the parameter <see cref="MutableByte"/> class.
		/// 
		/// Information in the parameter is prepended by the SNMP version field and wrapped in a sequence header.
		/// 
		/// Derived classes call this method to finalize SNMP packet encoding.
		/// </summary>
		/// <param name="buffer">Buffer containing BER encoded SNMP information</param>
		public virtual void encode(MutableByte buffer)
		{
			// Encode SNMP protocol version
			MutableByte temp = new MutableByte();
			_protocolVersion.encode(temp);
			buffer.Prepend(temp);
			temp.Reset();
			AsnType.BuildHeader(temp, SnmpConstants.SMI_SEQUENCE, buffer.Length);
			buffer.Prepend(temp);
		}
		/// <summary>
		/// Get SNMP protocol version from the packet. This routine does not verify if version number is valid. Caller
		/// should verify that returned value represents a valid SNMP protocol version number.
		/// 
		/// <code>
		/// int protocolVersion = Packet.GetProtocolVersion(inPacket, inLength);
		/// if( protocolVersion != -1 )
		/// {
		///     if( protocolVersion == SnmpConstants.SNMPV1 || protocolVersion == SnmpConstants.SNMPV2 || protocolVersion == SnmpConstants.SNMPV3 )
		///     {
		///         // do something
		///     }
		///     else
		///     {
		///         Console.WriteLine("Invalid SNMP protocol version.");
		///     }
		/// }
		/// else
		/// {
		///     Console.WriteLine("Invalid SNMP packet.");
		/// }
		/// </code>
		/// </summary>
		/// <param name="buffer">BER encoded SNMP packet</param>
		/// <param name="bufferLength">Length of the BER encoded packet</param>
		/// <returns>Returns SNMP protocol version, if packet is not valid returned value is -1.</returns>
		/// <exception cref="SnmpDecodingException">Thrown when invalid sequence type is found at the start of the SNMP packet being decoded</exception>
		public static int GetProtocolVersion(byte[] buffer, int bufferLength)
		{
			int offset = 0;
			int length = 0;
			byte asnType = AsnType.ParseHeader(buffer, ref offset, out length);
			if ((offset + length) > bufferLength)
			{
				return -1; // This is not a valid packet
			}
			if( asnType != SnmpConstants.SMI_SEQUENCE )
				throw new SnmpDecodingException("Invalid sequence type at the start of the SNMP packet.");
			Integer32 version = new Integer32();
			offset = version.decode(buffer, offset);
			return version.Value;
		}

		#region Packet type check properties

		/// <summary>
		/// Packet is a report
		/// </summary>
		public bool IsReport
		{
			get
			{
				if (Pdu.Type == PduType.Response)
					return true;
				return false;
			}
		}

		/// <summary>
		/// Packet is a request
		/// </summary>
		/// <remarks>
		/// Checks if the class content is a SNMP Get, GetNext, GetBulk or Set request.
		/// </remarks>
		public bool IsRequest
		{
			get
			{
				if (Pdu.Type == PduType.Get || Pdu.Type == PduType.GetNext ||
					Pdu.Type == PduType.GetBulk || Pdu.Type == PduType.Set)
					return true;
				return false;
			}
		}

		/// <summary>
		/// Packet is a response
		/// </summary>
		public bool IsResponse
		{
			get
			{
				if (Pdu.Type == PduType.Response)
					return true;
				return false;
			}
		}

		/// <summary>
		/// Packet is a notification
		/// </summary>
		public bool IsNotification
		{
			get
			{
				if (Pdu.Type == PduType.Trap || Pdu.Type == PduType.V2Trap || Pdu.Type == PduType.Inform)
					return true;
				return false;
			}
		}
		#endregion
	}
}

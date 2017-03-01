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
	/// <summary>SNMP Version 3 Scoped Protocol Data Unit.</summary>
	/// <remarks>
	/// ScopedPdu extends the <see cref="Pdu"/> class by adding SNMP version 3 specific Context Engine Id and Context Name
	/// variables to the beginning of the packet.
	/// 
	/// Context engine id is retrieved from the agent using the SNMP version 3 standard defined discovery process. Context
	/// name is used to define which subsection of the agents MIB user is allowed (or wants) to access.
	/// 
	/// When creating a new ScopedPdu, for a valid request, ContextEngineId value has to be set for a valid reply to be
	/// received. Context value is implementation specific (depends on the configuration of the agent) and might not be
	/// required for a successful request.
	/// </remarks>
	public class ScopedPdu: Pdu
	{
		#region Private variables

		/// <summary>
		/// Context Engine Id variable.
		/// </summary>
		protected OctetString _contextEngineId;
		/// <summary>
		/// Context name variable
		/// </summary>
		protected OctetString _contextName;

		#endregion

		#region Properties
		/// <summary>
		/// Get/Set context engine id value.
		/// </summary>
		public OctetString ContextEngineId
		{
			get { return _contextEngineId; }
		}

		/// <summary>
		/// Get/Set context name value
		/// </summary>
		public OctetString ContextName
		{
			get { return _contextName; }
		}

		#endregion

		#region Constructor(s)

		/// <summary>
		/// Standard constructor.
		/// </summary>
		/// <remarks>
		/// Intializes the ScopedPdu type to SNMP-GET. For details see base
		/// class <see cref="Pdu"/> standard constructor definition.
		/// 
		/// Context engine id and name variables are initialized to 0 length values.
		/// </remarks>
		public ScopedPdu()
			: base()
		{
			_contextEngineId = new OctetString();
			_contextName = new OctetString();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="pduType">Pdu type value. For available types see <see cref="PduType"/>.</param>
		public ScopedPdu(PduType pduType)
			: base(pduType)
		{
			_contextEngineId = new OctetString();
			_contextName = new OctetString();
		}

		/// <summary>Constructor.</summary>
		/// <remarks>
		/// Standard constructor that sets ScopedPdu request type and request id. For valid types see <see cref="PduType"/>.
		/// 
		/// By default, requestId value is set to a random value. Second argument allows caller to specify request
		/// id for this packet
		/// </remarks>
		/// <param name="pduType">Pdu type value.</param>
		/// <param name="requestId">Request id for the ScopedPdu instance.</param>
		public ScopedPdu(PduType pduType, int requestId)
			: base(pduType)
		{
			_requestId.Value = requestId;
			_contextEngineId = new OctetString();
			_contextName = new OctetString();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="pdu">Initialize class from this <see cref="Pdu"/> class</param>
		public ScopedPdu(Pdu pdu)
			: base(pdu)
		{
			_contextEngineId = new OctetString();
			_contextName = new OctetString();
		}

		#endregion

		#region Encode and decode

		/// <summary>
		/// Convert <see cref="ScopedPdu"/> into a BER encoded byte array. Resulting byte array is appended
		/// to the argument specified <see cref="MutableByte"/> class.
		/// 
		/// Privacy operations are not performed by this method. Value encoded and returned by this method is
		/// suitable for sending in NoAuthNoPriv or AuthNoPriv security configurations. If privacy is required,
		/// caller will have to perform encryption and decryption operations after BER encoding is performed.
		/// 
		/// In privacy protected SNMP version 3 packets, ScopedPdu is 1) encrypted using configured encryption
		/// method, 2) added to a <see cref="OctetString"/> field, and 3) appended to the data buffer.
		/// 
		/// Because privacy operation is intrusive, it is recommended that BER encoding of the ScopedPdu packet
		/// is stored in a temporary <see cref="MutableByte"/> class, where it can be privacy protected and
		/// added to the <see cref="OctetString"/> class for final encoding into the target SNMP v3 packet.
		/// </summary>
		/// <param name="buffer"><see cref="MutableByte"/> class passed by reference that encoded ScopedPdu
		/// value is appended to.</param>
		public override void encode(MutableByte buffer)
		{
			MutableByte tmp = new MutableByte();
			_contextEngineId.encode(tmp);
			_contextName.encode(tmp);
			// Encode base
			base.encode(tmp);
			BuildHeader(buffer, SnmpConstants.SMI_SEQUENCE, tmp.Length);
			buffer.Append(tmp);
		}

		/// <summary>
		/// Decode BER encoded <see cref="ScopedPdu"/> values. This method does not perform SNMP v3 privacy operations
		/// and is not aware of privacy requirements.
		/// 
		/// To decode a privacy protected SNMP v3 packet, you will need to a) extract <see cref="OctetString"/> value 
		/// holding encrypted <see cref="ScopedPdu"/> data, b) decrypt the encrypted ScopedPdu data into an unecrypted
		/// byte array, c) pass unencrypted <see cref="ScopedPdu"/> and BER encoded byte array to this method for
		/// final data conversion from BER into individual sequences and variables.
		/// </summary>
		/// <param name="buffer">Buffer holding BER encoded <see cref="ScopedPdu"/> data</param>
		/// <param name="offset">Offset within the buffer BER encoded <see cref="ScopedPdu"/> data begins.</param>
		/// <returns>Offset position after parsed ScopedPdu</returns>
		/// <exception cref="SnmpDecodingException">Error was encountered when decoding the PDU</exception>
		/// <exception cref="OverflowException">Thrown when buffer is too short to contain the PDU</exception>
		public override int decode(byte[] buffer, int offset)
		{
			int length;
			byte sequenceType = ParseHeader(buffer, ref offset, out length);
			if (sequenceType != SnmpConstants.SMI_SEQUENCE)
				throw new SnmpDecodingException("Invalid ScopedPdu sequence detected. Invalid ScopedPdu encoding.");
			// verify packet can match parsed length
			if (length > (buffer.Length - offset))
				throw new OverflowException("SNMP packet too short.");

			// Reset scoped pdu specific variables
			_contextEngineId.Reset();
			_contextName.Reset();
			// parse scoped pdu specific variables
			offset = _contextEngineId.decode(buffer, offset);
			offset = _contextName.decode(buffer, offset);
			// decode base pdu
			offset = base.decode(buffer, offset);

			return offset;
		}

		#endregion Encode and decode

	}
}

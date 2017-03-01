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
using System.Collections.Generic;

namespace SnmpSharpNet {
	/// <summary>
	/// SNMP Protocol Data Unit
	/// </summary>
	/// <remarks>
	/// SNMP PDU class that is the bases for all SNMP requests and replies. It is capable of processing
	/// SNMPv1 GET, GET-NEXT, REPLY and SNMPv2 GET, GET-NEXT, GET-BULK, REPLY, V2TRAP, INFORM and REPORT PDUs.
	/// <code>
	/// Pdu pdu = new Pdu();
	/// pdu.Type = PduType.Get;
	/// pdu.VbList.AddVb("1.3.6.1.2.1.1.1.0");
	/// pdu.VbList.AddVb("1.3.6.1.2.1.1.2.0");
	/// </code>
	/// 
	/// By default, Pdu class initializes the RequestId (unique identifier of each SNMP request made by the manager)
	/// with a random value. User can force a new, random request id generation at the time packet is encoding by
	/// changing RequestId to 0. If you wish to set a specific RequestId, you can do it this way:
	/// 
	/// <code>
	/// Pdu pdu = new Pdu();
	/// pdu.Type = PduType.GetNext;
	/// pdu.RequestId = 11; // Set a custom request id
	/// pdu.VbList.AddVb("1.3.6.1.2.1.1");
	/// </code>
	/// 
	/// Pdu types with special options are notification PDUs, V2TRAP and INFORM, and Get-Bulk requests.
	/// 
	/// Get-Bulk request is available in version 2c and 3 of the SNMP. Two special options can be set for these
	/// requests, NonRepeaters and MaxRepetitions. 
	/// 
	/// NonRepeaters is a value telling the agent how many OIDs in the VbList are to be treated as a single 
	/// GetNext request.
	/// 
	/// MaxRepeaters tells the agent how many variable bindings to return in a single Pdu for each requested Oid.
	/// </remarks>
	public class Pdu : AsnType, ICloneable, IEnumerable<Vb>
	{

		#region Internal variables

		/// <summary>
		/// Variable binding collection
		/// </summary>
		protected VbCollection _vbs;
		/// <summary>
		/// Error status value.
		/// </summary>
		/// <remarks>
		/// See <see cref="SnmpConstants"/> class for definition of error values. If no error
		/// is encountered, this value is set to 0.
		/// </remarks>
		protected Integer32 _errorStatus;
		/// <summary>
		/// Error index value.
		/// </summary>
		/// <remarks>
		/// Points to the Vb sequence that caused the error. If not Vb the cause of the
		/// error, or if there is no error, this value is 0.
		/// </remarks>
		protected Integer32 _errorIndex;
		/// <summary>
		/// Request id value.
		/// </summary>
		/// <remarks>
		/// Integer32 value that uniquely represents this request. Used to match requests and replies.
		/// </remarks>
		protected Integer32 _requestId;
		/// <summary>
		/// SNMPv2 trap first Vb is the trap time stamp. To create an SNMPv2 TRAP packet, set the timestamp value
		/// in this variable
		/// </summary>
		protected TimeTicks _trapTimeStamp;
		/// <summary>
		/// SNMPv2 trap second Vb is the trap object ID.
		/// </summary>
		/// <remarks>
		/// This variable should be set to the trap OID and will be inserted
		/// into the encoded packet.
		/// </remarks>
		protected Oid _trapObjectID;

		#endregion Internal variables

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// Initializes all values to NULL and PDU type to GET
		/// </remarks>
		public Pdu() {
			_vbs = null;
			_errorIndex = new Integer32();
			_errorStatus = new Integer32();
			_requestId = new Integer32();
			_requestId.SetRandom();
			_asnType = (byte)PduType.Get;
			_vbs = new VbCollection();
			_trapTimeStamp = new TimeTicks();
			_trapObjectID = new Oid();
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// Create Pdu of specific type.
		/// </remarks>
		/// <param name="pduType">Pdu type. For available values see <see cref="PduType"/></param>
		public Pdu(PduType pduType)
			: this()
		{
			_asnType = (byte)pduType;
			if (_asnType == (byte)PduType.GetBulk)
			{
				_errorStatus.Value = 0;
				_errorIndex.Value = 100;
			}
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// Sets the VarBind list to the Clone copy of the supplied list.
		/// </remarks>
		/// <param name="vbs">VarBind list to initialize the internal VbList to.</param>
		public Pdu(VbCollection vbs)
			: this() {
			_vbs = (VbCollection)vbs.Clone();
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// Initializes PDU class with supplied values.
		/// </remarks>
		/// <param name="vbs">VarBind list</param>
		/// <param name="type">PDU type</param>
		/// <param name="requestId">Request id</param>
		public Pdu(VbCollection vbs, PduType type, int requestId)
			: this(vbs) {
			_requestId.Value = requestId;
			_asnType = (byte)type;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <remarks>
		/// Initialize class from the passed pdu class.
		/// </remarks>
		/// <param name="pdu">Pdu class to use as source of information to initilize this class.</param>
		public Pdu(Pdu pdu)
			: this(pdu.VbList, pdu.Type, pdu.RequestId)
		{
			if (pdu.Type == PduType.GetBulk)
			{
				NonRepeaters = pdu.NonRepeaters;
				MaxRepetitions = pdu.MaxRepetitions;
			}
			else
			{
				ErrorStatus = pdu.ErrorStatus;
				ErrorIndex = pdu.ErrorIndex;
			}
		}

		/// <summary>
		/// Copy values from another Pdu class.
		/// </summary>
		/// <param name="value"><see cref="Pdu"/> cast as AsnType</param>
		/// <exception cref="ArgumentNullException">Thrown when received argument is null</exception>
		public void Set(AsnType value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			Pdu pdu = value as Pdu;
			if (pdu != null)
			{
				this.Type = pdu.Type;
				this._requestId.Value = pdu.RequestId;
				if (this.Type == PduType.GetBulk)
				{
					this.NonRepeaters = pdu.NonRepeaters;
					this.MaxRepetitions = pdu.MaxRepetitions;
				}
				else
				{
					this.ErrorStatus = pdu.ErrorStatus;
					this.ErrorIndex = pdu.ErrorIndex;
				}
				this._vbs.Clear();
				foreach (Vb v in pdu.VbList)
				{
					this._vbs.Add((Vb)v.Clone());
				}
			}
			else
			{
				throw new ArgumentNullException("value", "Argument is not an Oid class");
			}
		}
		/// <summary>
		/// Set VbList
		/// </summary>
		/// <remarks>
		/// Copy variable bindings from argument <see cref="VbCollection"/> into this classes variable 
		/// binding collection
		/// </remarks>
		/// <param name="value"><see cref="VbCollection"/> to copy variable bindings from</param>
		public void SetVbList(VbCollection value)
		{
			_vbs.Clear();
			foreach (Vb v in value)
			{
				_vbs.Add(v);
			}
		}
		/// <summary>
		/// Reset VbList.
		/// </summary>
		/// <remarks>
		/// Remove all entries in the VbList collections.
		/// </remarks>
		public void Reset()
		{
			_vbs.Clear();
			_errorStatus.Value = 0;
			_errorIndex.Value = 0;
			if( _requestId.Value == Int32.MaxValue )
				_requestId.Value = 1;
			else
				_requestId.Value = _requestId.Value + 1;
			_trapObjectID.Reset();
			_trapTimeStamp.Value = 0;
		}
		/// <summary>
		/// Create SNMP-GET Pdu from VbList
		/// </summary>
		/// <remarks>
		/// Helper static function to create GET PDU from the supplied VarBind list. Don't forget to set
		/// request id for the PDU.
		/// </remarks>
		/// <param name="vbs">VarBind list</param>
		/// <returns>Newly constructed GET PDU</returns>
		public static Pdu GetPdu(VbCollection vbs)
		{
			Pdu p = new Pdu(vbs);
			p.Type = PduType.Get;
			p.ErrorIndex = 0;
			p.ErrorStatus = 0;
			return p;
		}
		/// <summary>
		/// Create Get Pdu with empty VarBind array
		/// </summary>
		/// <returns>Instance of Get Pdu</returns>
		public static Pdu GetPdu()
		{
			return new Pdu(PduType.Get);
		}
		/// <summary>
		/// Create SNMP-SET Pdu
		/// </summary>
		/// <remarks>
		/// Helper static function to create SET PDU from the supplied VarBind list. Don't forget to set
		/// request id for the PDU.
		/// </remarks>
		/// <param name="vbs">VarBind list</param>
		/// <returns>Newly constructed SET PDU</returns>
		public static Pdu SetPdu(VbCollection vbs)
		{
			Pdu p = new Pdu(vbs);
			p.Type = PduType.Set;
			p.ErrorIndex = 0;
			p.ErrorStatus = 0;
			return p;
		}
		/// <summary>
		/// Create Set Pdu with empty VarBind array
		/// </summary>
		/// <returns>Instance of Set Pdu</returns>
		public static Pdu SetPdu()
		{
			return new Pdu(PduType.Set);
		}
		/// <summary>
		/// Create SNMP-GetNext Pdu
		/// </summary>
		/// <remarks>
		/// Helper static function to create GETNEXT PDU from the supplied VarBind list. Don't forget to set
		/// request id for the PDU.
		/// </remarks>
		/// <param name="vbs">VarBind list</param>
		/// <returns>Newly constructed GETNEXT PDU</returns>
		public static Pdu GetNextPdu(VbCollection vbs)
		{
			Pdu p = new Pdu(vbs);
			p.Type = PduType.GetNext;
			p.ErrorIndex = 0;
			p.ErrorStatus = 0;
			return p;
		}
		/// <summary>
		/// Create GetNext Pdu with empty VarBind array
		/// </summary>
		/// <returns>Instance of GetNext Pdu</returns>
		public static Pdu GetNextPdu()
		{
			return new Pdu(PduType.GetNext);
		}
		/// <summary>
		/// Create SNMP-GetBulk Pdu
		/// </summary>
		/// <remarks>
		/// Helper static function to create GETBULK PDU from the supplied VarBind list. MaxRepetitions are set to
		/// 256 and nonRepeaters are set to 0.
		/// </remarks>
		/// <param name="vbs">VarBind list</param>
		/// <returns>Newly constructed GETBULK PDU</returns>
		public static Pdu GetBulkPdu(VbCollection vbs) {
			Pdu p = new Pdu(vbs);
			p.Type = PduType.GetBulk;
			p.MaxRepetitions = 100;
			p.NonRepeaters = 0;
			return p;
		}
		/// <summary>
		/// Create GetBulk Pdu with empty VarBind array. By default initializes NonRepeaters to 0 and MaxRepetitions to 100
		/// </summary>
		/// <returns>Instance of GetBulk Pdu</returns>
		public static Pdu GetBulkPdu()
		{
			return new Pdu(PduType.GetBulk);
		}
		/// <summary>
		/// ErrorStatus Pdu value
		/// </summary>
		/// <remarks>
		/// Stores error status returned by the SNMP agent. Value 0 represents no error. Valid for all
		/// Pdu types except GetBulk requests.
		/// </remarks>
		/// <exception cref="SnmpInvalidPduTypeException">Thrown when property is access for GetBulk Pdu</exception>
		public int ErrorStatus
		{
			get {
				if (Type == PduType.GetBulk)
					throw new SnmpInvalidPduTypeException("ErrorStatus property is not valid for GetBulk packets.");
				return _errorStatus.Value;
			}
			set {
				_errorStatus.Value = value;
			}
		}
		/// <summary>
		/// ErrorIndex Pdu value
		/// </summary>
		/// <remarks>
		/// Error index points to the VbList entry that ErrorStatus error code refers to. Valid for all Pdu types
		/// except GetBulk requests.
		/// </remarks>
		/// <see cref="ErrorStatus"/>
		/// <exception cref="SnmpInvalidPduTypeException">Thrown when property is access for GetBulk Pdu</exception>
		public int ErrorIndex {
			get {
				if (Type == PduType.GetBulk)
					throw new SnmpInvalidPduTypeException("ErrorStatus property is not valid for GetBulk packets.");
				return _errorIndex.Value;
			}
			set {
				_errorIndex.Value = value;
			}
		}
		/// <summary>
		/// SNMP packet request id that is sent to the SNMP agent. SET this value before making SNMP requests.
		/// </summary>
		public int RequestId {
			set {
				_requestId.Value = value;
			}
			get {
				return _requestId.Value;
			}
		}
		/// <summary>
		/// Get or SET the PDU type. Available types are GET, GETNEXT, SET, GETBULK. PDU types are defined in Pdu class.
		/// </summary>
		/// <seealso cref="PduType.Get"/>
		/// <seealso cref="PduType.GetNext"/>
		/// <seealso cref="PduType.Set"/>
		/// <seealso cref="PduType.Response"/>"/>
		/// * version 2 specific:
		/// <seealso cref="PduType.GetBulk"/>
		public new PduType Type {
			set {
				if (_asnType == (byte)value)
					return; // nothing has changed
				// If type changes from GETBULK make sure errorIndex and errorStatus are set to 0
				// otherwise you'll send error messages to the receiver
				if (value != PduType.GetBulk)
				{
					_errorIndex.Value = 0;
					_errorStatus.Value = 0;
				}
				else
				{
					_errorStatus.Value = 0;
					_errorIndex.Value = 100;
				}
				_asnType = (byte)value;
			}
			get {
				return (PduType)_asnType;
			}
		}
		/// <summary>
		/// Tells SNMP Agent how many VBs to include in a single request. Only valid on GETBULK requests.
		/// </summary>
		/// <exception cref="SnmpInvalidPduTypeException">Thrown when PDU type is not GET-BULK</exception>
		public int MaxRepetitions
		{
			set {
				if (Type == PduType.GetBulk)
					_errorIndex.Value = value;
				else
					throw new SnmpInvalidPduTypeException("NonRepeaters property is only available in GET-BULK PDU type.");
			}
			get
			{
				if (Type == PduType.GetBulk)
				{
					return _errorIndex.Value;
				}
				throw new SnmpInvalidPduTypeException("NonRepeaters property is only available in GET-BULK PDU type.");
			}
		}
		/// <summary>
		/// Get/Set GET-BULK NonRepeaters value
		/// </summary>
		/// <remarks>
		/// Non repeaters variable tells the SNMP Agent how many GETNEXT like variables to retrieve (single Vb returned
		/// per request) before MaxRepetitions value takes affect. If you wish to retrieve as many values as you can
		/// in a single request, set this value to 0.
		/// </remarks>
		/// <exception cref="SnmpInvalidPduTypeException">Thrown when PDU type is not GET-BULK</exception>
		public int NonRepeaters
		{
			set
			{
				if (_asnType == (byte)PduType.GetBulk)
					_errorStatus.Value = value;
				else
					throw new SnmpInvalidPduTypeException("NonRepeaters property is only available in GET-BULK PDU type.");
			}
			get
			{
				if (Type == PduType.GetBulk)
					return _errorStatus.Value;
				throw new SnmpInvalidPduTypeException("NonRepeaters property is only available in GET-BULK PDU type.");
			}
		}
		/// <summary>
		/// VarBind list
		/// </summary>
		public VbCollection VbList
		{
			get {
				return _vbs;
			}
		}
		/// <summary>
		/// Get TRAP TimeStamp class from SNMPv2 TRAP and INFORM PDUs
		/// </summary>
		public TimeTicks TrapSysUpTime
		{
			get {
				return _trapTimeStamp;
			}
		}
		/// <summary>
		/// Get TRAP ObjectID class from SNMPv2 TRAP and INFORM PDUs
		/// </summary>
		/// <exception cref="SnmpInvalidPduTypeException">Thrown when property is access for a Pdu of a type other then V2TRAP, INFORM or RESPONSE</exception>
		public Oid TrapObjectID
		{
			get
			{
				if (Type != PduType.V2Trap && Type != PduType.Inform && Type != PduType.Response)
					throw new SnmpInvalidPduTypeException("TrapObjectID value can only be accessed in V2TRAP, INFORM and RESPONSE PDUs");

				return _trapObjectID;
			}
			set
			{
				_trapObjectID.Set(value);
			}
		}
		/// <summary>
		/// Get VB from VarBind list at the specified index
		/// </summary>
		/// <param name="index">Index position of the Vb in the array. Zero based.</param>
		/// <returns>Vb at the specified location in the array</returns>
		public Vb GetVb(int index)
		{
			return (Vb)_vbs[index];
		}
		/// <summary>
		/// Return the number of VB entries in the VarBind list
		/// </summary>
		public int VbCount
		{
			get
			{
				return _vbs.Count;
			}
		}
		/// <summary>
		/// Delete VB from the specified location in the VarBind list
		/// </summary>
		/// <param name="pos">0 based VB location</param>
		public void DeleteVb(int pos)
		{
			if (pos >= 0 && pos <= _vbs.Count)
			{
				_vbs.RemoveAt(pos);
			}
		}

		#region Encode & Decode methods

		/// <summary>
		/// Encode Pdu class to BER byte buffer
		/// </summary>
		/// <remarks>
		/// Encodes the protocol data unit using the passed encoder and stores
		/// the results in the passed buffer. An exception is thrown if an 
		/// error occurs with the encoding of the information. 
		/// </remarks>
		/// <param name="buffer">The buffer to write the encoded information.</param>
		public override void encode(MutableByte buffer)
		{
			MutableByte tmpBuffer = new MutableByte();

			// if request id is 0, get a random value
			if (_requestId.Value == 0)
				_requestId.SetRandom();

			_requestId.encode(tmpBuffer);
			_errorStatus.encode(tmpBuffer);
			_errorIndex.encode(tmpBuffer);

			// if V2TRAP PDU type, add sysUpTime and trapObjectID OIDs before encoding VarBind

			if (Type == PduType.V2Trap || Type == PduType.Inform)
			{
				if (_vbs.Count == 0)
				{
					// add sysUpTime and trapObjectID to the VbList
					_vbs.Add(SnmpConstants.SysUpTime, _trapTimeStamp);
					_vbs.Add(SnmpConstants.TrapObjectId, _trapObjectID);
				}
				else
				{
					// Make sure user didn't manually add sysUpTime and trapObjectID values
					// to the pdu

					// if we have more then one item in the VarBinds array check for sysUpTime
					if (_vbs.Count > 0)
					{
						// if the first Vb in the VarBinds array is not sysUpTime append it in the
						// encoded byte array
						if (!_vbs[0].Oid.Equals(SnmpConstants.SysUpTime))
						{
							Vb sysUpTimeVb = new Vb(SnmpConstants.SysUpTime, _trapTimeStamp);
							_vbs.Insert(0, sysUpTimeVb);
						}
					}
					// if we have 2 or more Vbs in the VarBinds array check for trapObjectID Vb
					if (_vbs.Count > 1)
					{
						// if second Vb in the VarBinds array is not trapObjectId encode the value
						if (!_vbs[1].Oid.Equals(SnmpConstants.TrapObjectId))
						{
							Vb trapObjectIdVb = new Vb(SnmpConstants.TrapObjectId, _trapObjectID);
							_vbs.Insert(1, trapObjectIdVb);
						}
					}
				}
			}

			// encode variable bindings
			_vbs.encode(tmpBuffer);

			// Now encode the header for the PDU
			BuildHeader(buffer, (byte)Type, tmpBuffer.Length);
			buffer.Append(tmpBuffer);
		}

		/// <summary>
		/// Decode BER encoded Pdu
		/// </summary>
		/// <remarks>
		/// Decodes the protocol data unit from the passed buffer. If an error
		/// occurs during the decoding sequence then an AsnDecodingException is
		/// thrown by the method. The value is decoded using the AsnEncoder
		/// passed to the object.
		/// </remarks>
		/// <param name="buffer">BER encoded buffer</param>
		/// <param name="offset">The offset byte to begin decoding</param>
		/// <returns>Buffer position after the decoded value</returns>
		/// <exception cref="OverflowException">Thrown when header points to more data then is available.</exception>
		public override int decode(byte[] buffer, int offset)
		{
			int headerLength;
			byte asnType = ParseHeader(buffer, ref offset, out headerLength);
			if (offset + headerLength > buffer.Length)
				throw new OverflowException("Insufficient data in packet");

			_asnType = asnType;

			// request id
			offset = _requestId.decode(buffer, offset);

			// error status
			offset = _errorStatus.decode(buffer, offset);

			// error index
			offset = _errorIndex.decode(buffer, offset);

			// clean out the current variables
			_vbs.Clear();

			// decode the Variable binding collection
			offset = _vbs.decode(buffer, offset);

			// if Pdu is an SNMP version 2 TRAP, remove sysUpTime and trapObjectID from the VarBinds array
			if (Type == PduType.V2Trap || Type == PduType.Inform)
			{
				if (_vbs.Count > 0)
				{
					if (_vbs[0].Oid.Equals(SnmpConstants.SysUpTime))
					{
						_trapTimeStamp.Set(_vbs[0].Value);
						_vbs.RemoveAt(0); // remove sysUpTime
					}
				}
				if (_vbs.Count > 0)
				{
					if (_vbs[0].Oid.Equals(SnmpConstants.TrapObjectId))
					{
						_trapObjectID.Set((Oid)_vbs[0].Value);
						_vbs.RemoveAt(0); // remove sysUpTime
					}
				}
			}

			return offset;
		}
		#endregion

		#region Overrides

		/// <summary>
		/// Return string dump of the Pdu class.
		/// </summary>
		/// <returns>String content of the Pdu class.</returns>
		public override string ToString()
		{
			StringBuilder str = new StringBuilder();
			str.Append("PDU-");
			switch (this._asnType)
			{
				case (byte)PduType.Get:
					str.Append("Get");
					break;
				case (byte)PduType.GetNext:
					str.Append("GetNext");
					break;
				case (byte)PduType.GetBulk:
					str.Append("GetBulk");
					break;
				case (byte)PduType.V2Trap:
					str.Append("V2Trap");
					break;
				case (byte)PduType.Response:
					str.Append("Response");
					break;
				case (byte)PduType.Inform:
					str.Append("Inform");
					break;
				default:
					str.Append("Unknown");
					break;
			}
			str.Append("\n");
			str.AppendFormat("RequestId: {0}\n", this.RequestId);
			if (this.Type != PduType.GetBulk)
				str.AppendFormat("ErrorStatus: {0}\nError Index: {1}\n", this.ErrorStatus, this.ErrorIndex);
			else
				str.AppendFormat("MaxRepeaters: {0}\nNonRepeaters: {1}\n", this.MaxRepetitions, this.NonRepeaters);
			if (this.Type == PduType.V2Trap || this.Type == PduType.Inform)
				str.AppendFormat("TimeStamp: {0}\nTrapOID: {1}\n", this.TrapSysUpTime.ToString(), this.TrapObjectID.ToString());
			str.AppendFormat("VbList entries: {0}\n", this.VbCount);
			if (this.VbCount > 0)
			{
				foreach (Vb v in this.VbList)
				{
					str.AppendFormat("Vb: {0}\n", v.ToString());
				}
			}
			return str.ToString();
		}
		/// <summary>
		/// Clone this object
		/// </summary>
		/// <returns>Copy of this object cast as type System.Object</returns>
		public override Object Clone()
		{
			Pdu p = new Pdu(_vbs, Type, _requestId);
			if (this.Type == PduType.GetBulk)
			{
				p.NonRepeaters = _errorStatus;
				p.MaxRepetitions = _errorIndex;
			}
			else
			{
				p.ErrorIndex = this.ErrorIndex;
				p.ErrorStatus = this.ErrorStatus;
			}
			if (this.Type == PduType.V2Trap)
			{
				p.TrapObjectID.Set(this.TrapObjectID);
				p.TrapSysUpTime.Value = this.TrapSysUpTime.Value;
			}
			return p;
		}
		/// <summary>
		/// Check class equality with argument.
		/// 
		/// Accepted argument types are:
		/// * Integer32 - compared against the request id
		/// * Pdu - compared against PduType, request id and contents of VarBind list
		/// </summary>
		/// <param name="obj">Integer32 or Pdu to compare</param>
		/// <returns>True if equal, otherwise false</returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if( obj is Integer32 )
			{
				if( ((Integer32)obj) == _requestId )
					return true;
				return false;
			}
			else if( obj is Pdu )
			{
				Pdu p = (Pdu)obj;
				if (p.Type != this.Type)
					return false;
				if (p.RequestId != this.RequestId)
					return false;
				if (p.VbCount != this.VbCount)
					return false;
				foreach (Vb v in VbList)
				{
					if (!p.VbList.ContainsOid(v.Oid))
						return false;
				}
				foreach (Vb v in p.VbList)
				{
					if (!VbList.ContainsOid(v.Oid))
						return false;
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Returns hash code representing class value.
		/// </summary>
		/// <returns>Class value hash code</returns>
		public override int GetHashCode()
		{
			return (byte)Type | RequestId;
		}

		#endregion // Overrides

		#region Quick access methods

		/// <summary>
		/// Indexed access to VarBind collection of the Pdu.
		/// </summary>
		/// <param name="index">Index position of the VarBind entry</param>
		/// <returns>VarBind entry at the specified index</returns>
		/// <exception cref="IndexOutOfRangeException">Thrown when index is outside the bounds of the collection</exception>
		public Vb this[int index]
		{
			get
			{
				return _vbs[index];
			}
		}

		/// <summary>
		/// Access variable bindings using Vb Oid value
		/// </summary>
		/// <param name="oid">Required Oid value</param>
		/// <returns>Variable binding with the Oid matching the parameter, otherwise null</returns>
		public Vb this[Oid oid]
		{
			get
			{
				if (!_vbs.ContainsOid(oid))
					return null;
				foreach (Vb v in _vbs)
				{
					if (v.Oid.Equals(oid))
						return v;
				}
				return null;
			}
		}
		/// <summary>
		/// Access variable bindings using Vb Oid value in the string format
		/// </summary>
		/// <param name="oid">Oid value in string representation</param>
		/// <returns>Variable binding with the Oid matching the parameter, otherwise null</returns>
		public Vb this[String oid]
		{
			get
			{
				foreach (Vb v in _vbs)
				{
					if (v.Oid.Equals(oid))
						return v;
				}
				return null;
			}
		}

		/// <summary>
		/// Get VarBind collection enumerator.
		/// </summary>
		/// <returns>Enumerator</returns>
		public IEnumerator<Vb> GetEnumerator()
		{
			return _vbs.GetEnumerator();
		}

		/// <summary>
		/// Get VarBind collection enumerator.
		/// </summary>
		/// <returns>Enumerator</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((System.Collections.IEnumerable)_vbs).GetEnumerator();
		}

		#endregion
	}
}

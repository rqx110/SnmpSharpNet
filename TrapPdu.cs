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
	
	/// <summary>SNMP version 1 TRAP Protocol Data Unit</summary>
	/// <remarks>
	/// Trap PDU for SNMP version 1 is a PDU with a unique layout requiring a dedicated class. SNMP versions
	/// 2 and 3 use standard PDU type for V2TRAP notifications.
	/// </remarks>
	public class TrapPdu: AsnType, ICloneable
	{
		#region Internal variables

		/// <summary>Trap enterprise Oid</summary>
		protected Oid _enterprise;

		/// <summary>The IP Address of the remote agent sending the trap.</summary>
		protected IpAddress _agentAddr;

		/// <summary>Generic trap code</summary>
		protected Integer32 _generic;

		/// <summary>Specific trap code.</summary>
		protected Integer32 _specific;

		/// <summary>sysUpTime timestamp of the trap event</summary>
		protected TimeTicks _timeStamp;

		/// <summary>Variable binding list</summary>
		private VbCollection _variables;

		#endregion Internal variables

		#region Properties

		/// <summary>Get remote agent's IP address.</summary>
		virtual public IpAddress AgentAddress
		{
			get
			{
				return _agentAddr;
			}
		}
		/// <summary>Get/Set generic code trap value object</summary>
		virtual public Int32 Generic
		{
			get
			{
				return _generic.Value;
			}
			set
			{
				_generic.Value = value;
			}
		}
		/// <summary>Get/Set specific code trap value object</summary>
		virtual public Int32 Specific
		{
			get
			{
				return _specific.Value;
			}
			set
			{
				_specific.Value = value;
			}
		}
		/// <summary>Get timeticks trap value object</summary>
		virtual public UInt32 TimeStamp
		{
			get
			{
				return _timeStamp.Value;
			}
			set
			{
				_timeStamp.Value = value;
			}
			
		}
		/// <summary> Returns the number oid/value pairs in the variable binding contained in the PDU</summary>
		virtual public int Count
		{
			get
			{
				return _variables.Count;
			}
		}

		#endregion Properties

		/// <summary>Constructor</summary>
		public TrapPdu()
		{
			_asnType = (byte)PduType.Trap;
			_enterprise = new Oid();
			_agentAddr = new IpAddress();
			_generic = new Integer32();
			_specific = new Integer32();
			_timeStamp = new TimeTicks();

			_variables = new VbCollection();
		}
		
		/// <summary> Constructs a new trap pdu that is identical to the
		/// passed pdu.
		/// </summary>
		/// <param name="second">The object to copy.
		/// </param>
		public TrapPdu(TrapPdu second)
			: this()
		{
			_enterprise.Set(second._enterprise);
			_agentAddr.Set(second._agentAddr);
			_generic.Value = second.Generic;
			_specific.Value = second.Specific;
			_timeStamp.Value = second.TimeStamp;

			for (int x = 0; x < second._variables.Count; x++)
			{
				_variables = (VbCollection)second.VbList.Clone();
			}
		}

		/// <summary>
		/// Not implemented. Throws NotImplementedException.
		/// </summary>
		/// <param name="value">Irrelevant</param>
		public void Set(string value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get PDU type.
		/// </summary>
		/// <remarks>Always returns PduType.Trap</remarks>
		public new PduType Type
		{
			get
			{
				return (PduType)_asnType;
			}
		}

		/// <summary>
		/// Initialize the class with values from another <see cref="TrapPdu"/> class.
		/// </summary>
		/// <param name="second">TrapPdu class whose values are used to initialize this class.</param>
		public void Set(TrapPdu second)
		{
			if (second != null)
			{
				_enterprise.Set(second._enterprise);
				_agentAddr.Set(second._agentAddr);
				_generic.Value = second.Generic;
				_specific.Value = second.Specific;
				_timeStamp.Value = second.TimeStamp;

				_variables.Clear();

				for (int x = 0; x < second._variables.Count; x++)
				{
					_variables = (VbCollection)second.VbList.Clone();
				}
			}
			else
				throw new ArgumentException("Invalid argument type.", "value");
		}
		
		/// <summary>Get trap enterprise identifier</summary>
		public Oid Enterprise
		{
			get
			{
				return _enterprise;
			}
		}

		/// <summary>
		/// Get <see cref="VbCollection"/> variable binding list.
		/// </summary>
		public VbCollection VbList
		{
			get
			{
				return _variables;
			}

		}

		/// <summary>
		/// Return number of entries in the VbList
		/// </summary>
		public int VbCount
		{
			get
			{
				return _variables.Count;
			}
		}
		
		/// <summary>ASN.1 encode SNMP version 1 trap</summary>
		/// <param name="buffer"><see cref="MutableByte"/> buffer to the end of which encoded values are appended.</param>
		public override void encode(MutableByte buffer)
		{

			MutableByte trapBuffer = new MutableByte();
			// encode the enterprise id & address
			_enterprise.encode(trapBuffer);

			_agentAddr.encode(trapBuffer);

			_generic.encode(trapBuffer);

			_specific.encode(trapBuffer);

			_timeStamp.encode(trapBuffer);

			_variables.encode(trapBuffer);
			MutableByte tmpBuffer = new MutableByte();

			BuildHeader(tmpBuffer, (byte)PduType.Trap, trapBuffer.Length);
			trapBuffer.Prepend(tmpBuffer);
			buffer.Append(trapBuffer);
		}

		/// <summary>Decode BER encoded SNMP version 1 trap packet</summary>
		/// <param name="buffer">BER encoded buffer</param>
		/// <param name="offset">Offset in the packet to start decoding from</param>
		/// <returns>Buffer position after the decoded value.</returns>
		/// <exception cref="SnmpException">Invalid SNMP Pdu type received. Not an SNMP version 1 Trap PDU.</exception>
		/// <exception cref="SnmpException">Invalid Variable Binding list encoding.</exception>
		public override int decode(byte[] buffer, int offset)
		{
			int headerLength;
			byte asnType = ParseHeader(buffer, ref offset, out headerLength);
			if (asnType != (byte)PduType.Trap)
				throw new SnmpException("Invalid PDU type.");

			if (headerLength > buffer.Length - offset)
				throw new OverflowException("Packet is too short.");

			offset = _enterprise.decode(buffer, offset);
			offset = _agentAddr.decode(buffer, offset);

			offset = _generic.decode(buffer, offset);

			offset = _specific.decode(buffer, offset);

			offset = _timeStamp.decode(buffer, offset);

			// clean out the current variables
			_variables.Clear();

			offset = _variables.decode(buffer, offset);

			return offset;
		}
		
		/// <summary>
		/// Clone object
		/// </summary>
		/// <returns>Cloned copy of this object.</returns>
		public override Object Clone()
		{
			return new TrapPdu(this);
		}
	}
}
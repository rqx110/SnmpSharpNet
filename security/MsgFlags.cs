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
	/// <summary>
	/// Message flags in the SNMP v3 header.
	/// </summary>
	/// <remarks>
	/// Message flags hold flags that
	/// indicate if packet is authenticated, privacy protected and if
	/// report reply is expected on errors.
	/// 
	/// Message flags field is a 1 byte <see cref="OctetString"/> encoded
	/// field.
	/// </remarks>
	public class MsgFlags : AsnType, ICloneable
	{
		/// <summary>
		/// Bit value that, when set, indicates that packet has been authenticated.
		/// </summary>
		public static byte FLAG_AUTH = 0x01;

		/// <summary>
		/// Bit value that, when set, indicates that packet has been privacy protected.
		/// </summary>
		public static byte FLAG_PRIV = 0x02;

		/// <summary>
		/// Bit value that, when set, indicates that sender of the packet expects report packet to be sent by the agent on errors.
		/// </summary>
		public static byte FLAG_REPORTABLE = 0x04;

		/// <summary>
		/// True if authentication is used to secure the packet, otherwise false.
		/// </summary>
		protected bool _authenticationFlag;
		/// <summary>
		/// Get/Set authentication flag. Value is true if authentication is used, false if packet is not authenticated
		/// </summary>
		public bool Authentication
		{
			get { return _authenticationFlag; }
			set { _authenticationFlag = value; }
		}
		/// <summary>
		/// True if ScopedPdu portion of the packet is privacy protected with encryption, otherwise false.
		/// </summary>
		protected bool _privacyFlag;
		/// <summary>
		/// Get/Set privacy flag. Value is true if privacy protection is used, false if packet is not privacy protected.
		/// </summary>
		public bool Privacy
		{
			get { return _privacyFlag; }
			set { _privacyFlag = value; }
		}
		/// <summary>
		/// True if reportable flag is set, otherwise false.
		/// </summary>
		protected bool _reportableFlag;
		/// <summary>
		/// Get/Set reportable flag. Value is true if sender expects report packet on errors, false if sender does
		/// not expect report packets.
		/// </summary>
		public bool Reportable
		{
			get { return _reportableFlag; }
			set { _reportableFlag = value; }
		}

		/// <summary>
		/// Standard constructor. All flags are set to false by default.
		/// </summary>
		public MsgFlags()
		{
			_authenticationFlag = _privacyFlag = _reportableFlag = false;
		}

		/// <summary>
		/// Constructor. Initialize individual flag values.
		/// </summary>
		/// <param name="authentication">true if authentication is used, otherwise false</param>
		/// <param name="privacy">true if privacy protection is used, otherwise false</param>
		/// <param name="reportable">true if report is expected, otherwise false</param>
		public MsgFlags(bool authentication, bool privacy, bool reportable)
		{
			_authenticationFlag = authentication;
			_privacyFlag = privacy;
			_reportableFlag = reportable;
		}

		/// <summary>
		/// Encode SNMP v3 message flag field
		/// </summary>
		/// <param name="buffer">Buffer to append encoded value to</param>
		public override void encode(MutableByte buffer)
		{
			byte flag = 0x00;
			if (_authenticationFlag)
				flag |= FLAG_AUTH;
			if (_privacyFlag)
				flag |= FLAG_PRIV;
			if (_reportableFlag)
				flag |= FLAG_REPORTABLE;
			OctetString flagObject = new OctetString(flag);
			flagObject.encode(buffer);
		}

		/// <summary>
		/// Decode message flags from the BER encoded buffer starting at specified offset.
		/// </summary>
		/// <param name="buffer">BER encoded buffer</param>
		/// <param name="offset">Offset within the buffer to start decoding process</param>
		/// <returns>Buffer position after the decoded value</returns>
		public override int decode(byte[] buffer, int offset)
		{
			// reset class values
			_authenticationFlag = false;
			_privacyFlag = false;
			_reportableFlag = false;
			OctetString flagObject = new OctetString();
			offset = flagObject.decode(buffer, offset);
			if (flagObject.Length > 0)
			{
				if ((flagObject[0] & FLAG_AUTH) != 0)
					_authenticationFlag = true;
				if ((flagObject[0] & FLAG_PRIV) != 0)
					_privacyFlag = true;
				if ((flagObject[0] & FLAG_REPORTABLE) != 0)
					_reportableFlag = true;
			}
			else
			{
				throw new SnmpDecodingException("Invalid SNMPv3 flag field.");
			}
			return offset;
		}

		/// <summary>
		/// Clone this class.
		/// </summary>
		/// <returns>Cloned class cast as Object</returns>
		public override object Clone()
		{
			return new MsgFlags(_authenticationFlag, _privacyFlag, _reportableFlag);
		}

		/// <summary>
		/// Return string representation of the object.
		/// </summary>
		/// <returns>String representation of the class values.</returns>
		public override string ToString()
		{
			return string.Format("Reportable {0} Authenticated {1} Privacy {2}", _reportableFlag.ToString(),
				_authenticationFlag.ToString(), _privacyFlag.ToString());
		}
	}
}

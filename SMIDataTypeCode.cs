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
	/// Enumeration of SMI type codes used in TLV (Type Length Value) BER encoding of SNMP packets.
	/// </summary>
	public enum SMIDataTypeCode: byte
	{
		/// <summary>Signed 32-bit integer ASN.1 data type. For implementation, see <see cref="Integer32"/></summary>
		Integer = (byte)0x02,
		
		/// <summary>Data type representing a sequence of zero or more 8-bit byte values. For implementation, see <see cref="OctetString"/></summary>
		OctetString  = (byte)0x04,
		
		/// <summary>Object id ASN.1 type. For implementation, see <see cref="Oid"/></summary>
		ObjectId  = (byte)0x06,
		
		/// <summary>Null ASN.1 value type. For implementation, see <see cref="Null"/>.</summary>
		Null  = (byte)0x05,
		
		/// <summary> An application string is a sequence of octets
		/// defined at the application level. Although the SMI
		/// does not define an Application String, it does define
		/// an IP Address which is an Application String of length
		/// four.
		/// </summary>
		AppString  = (byte)0x40,
		
		/// <summary> An IP Address is an application string of length four
		/// and is indistinguishable from the APPSTRING value.
		/// The address is a 32-bit quantity stored in network byte order.
		/// </summary>
		IPAddress  = (byte)0x40,
		
		/// <summary> A non-negative integer that may be incremented, but not
		/// decremented. The value is a 32-bit unsigned quantity representing
		/// the range of zero to 2^32-1 (4,294,967,295). When the counter
		/// reaches its maximum value it wraps back to zero and starts again.
		/// </summary>
		Counter32  = (byte)0x41,
		
		/// <summary> Represents a non-negative integer that may increase or
		/// decrease with a maximum value of 2^32-1. If the maximum
		/// value is reached the gauge stays latched until reset.
		/// </summary>
		Gauge32  = (byte)0x42,
		
		/// <summary> Used to represent the integers in the range of 0 to 2^32-1.
		/// This type is identical to the COUNTER32 and are
		/// indistinguishable in ASN.1
		/// </summary>
		Unsigned32  = (byte)0x42, // same as gauge
		
		/// <summary> This represents a non-negative integer that counts time, modulo 2^32.
		/// The time is represented in hundredths (1/100th) of a second.
		/// </summary>
		TimeTicks  = (byte)0x43,
		
		/// <summary> Used to support the transport of arbitrary data. The
		/// data itself is encoded as an octet string, but may be in
		/// any format defined by ASN.1 or another standard.
		/// </summary>
		Opaque  = (byte)0x44,
		
		/// <summary> Defines a 64-bit unsigned counter. A counter is an integer that
		/// can be incremented, but cannot be decremented. A maximum value
		/// of 2^64 - 1 (18,446,744,073,709,551,615) can be represented.
		/// When the counter reaches it's maximum it wraps back to zero and
		/// starts again.
		/// </summary>
		Counter64  = (byte)0x46, // SMIv2 only

		/// <summary> The SNMPv2 error representing that there is No-Such-Object
		/// for a particular object identifier. This error is the result
		/// of a requested object identifier that does not exist in the
		/// agent's tables
		/// </summary>
		NoSuchObject  = (byte)0xc0,
		
		/// <summary> The SNMPv2 error representing that there is No-Such-Instance
		/// for a particular object identifier. This error is the result
		/// of a requested object identifier instance does not exist in the
		/// agent's tables. 
		/// </summary>
		NoSuchInstance  = (byte)0xc1,
		
		/// <summary> The SNMPv2 error representing the End-Of-Mib-View.
		/// This error variable will be returned by a SNMPv2 agent
		/// if the requested object identifier has reached the 
		/// end of the agent's mib table and there is no lexicographic 
		/// successor.
		/// </summary>
		EndOfMibView  = (byte)0xc2,

		/// <summary>
		/// SEQUENCE Variable Binding code. Hex value: 0x30
		/// </summary>
		Sequence = (byte)0x30,

		/// <summary> Defines an SNMPv2 Party Clock. The Party Clock is currently
		/// Obsolete, but included for backwards compatibility. Obsoleted in RFC 1902.
		/// </summary>
		PartyClock = (byte)0x47
	}
}

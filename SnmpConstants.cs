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
	
	/// <summary>SNMP SMI version 1, version 2c and version 3 constants.
	/// </summary>
	public sealed class SnmpConstants
	{
		#region Snmp V1 errors

		/// <summary>No error</summary>
		public const int ErrNoError = 0;

		/// <summary>Request too big</summary>
		public const int ErrTooBig = 1;

		/// <summary>Object identifier does not exist</summary>
		public const int ErrNoSuchName = 2;

		/// <summary>Invalid value</summary>
		public const int ErrBadValue = 3;

		/// <summary>Requested invalid operation on a read only table</summary>
		public const int ErrReadOnly = 4;

		/// <summary>Generic error</summary>
		public const int ErrGenError = 5;

		/// <summary>Enterprise specific error</summary>
		public const int enterpriseSpecific = 6;

		#endregion SnmpV1errors

		#region SnmpV2errors

		/// <summary>Access denied</summary>
		public const int ErrNoAccess = 6;

		/// <summary>Incorrect type</summary>
		public const int ErrWrongType = 7;

		/// <summary>Incorrect length</summary>
		public const int ErrWrongLength = 8;

		/// <summary>Invalid encoding</summary>
		public const int ErrWrongEncoding = 9;

		/// <summary>Object does not have correct value</summary>
		public const int ErrWrongValue = 10;

		/// <summary>Insufficient rights to perform create operation</summary>
		public const int ErrNoCreation = 11;

		/// <summary>Inconsistent value</summary>
		public const int ErrInconsistentValue = 12;

		/// <summary>Requested resource is not available</summary>
		public const int ErrResourceUnavailable = 13;

		/// <summary>Unable to commit values</summary>
		public const int ErrCommitFailed = 14;

		/// <summary>Undo request failed</summary>
		public const int ErrUndoFailed = 15;

		/// <summary>Authorization failed</summary>
		public const int ErrAuthorizationError = 16;

		/// <summary>Instance not writable</summary>
		public const int ErrNotWritable = 17;

		/// <summary>Inconsistent object identifier</summary>
		public const int ErrInconsistentName = 18;

		#endregion SnmpV2errors

		#region SNMP version 1 trap generic error codes

		/// <summary>Cold start trap</summary>
		public const int ColdStart = 0;

		/// <summary>Warm start trap</summary>
		public const int WarmStart = 1;

		/// <summary>Link down trap</summary>
		public const int LinkDown = 2;

		/// <summary>Link up trap</summary>
		public const int LinkUp = 3;

		/// <summary>Authentication-failure trap</summary>
		public const int AuthenticationFailure = 4;

		/// <summary>EGP Neighbor Loss trap</summary>
		public const int EgpNeighborLoss = 5;

		/// <summary>Enterprise Specific trap</summary>
		public const int EnterpriseSpecific = 6;

		#endregion SNMP version 1 trap generic error codes

		#region SMI Type codes and type names

		/// <summary>Signed 32-bit integer ASN.1 data type. For implementation, see <see cref="Integer32"/></summary>
		public static readonly byte SMI_INTEGER = (byte)(AsnType.UNIVERSAL | AsnType.INTEGER);
		/// <summary>String representation of the AsnType.INTEGER type.</summary>
		public static readonly string SMI_INTEGER_STR = "Integer32";
		
		/// <summary>Data type representing a sequence of zero or more 8-bit byte values. For implementation, see <see cref="OctetString"/></summary>
		public static readonly byte SMI_STRING  = (byte)(AsnType.UNIVERSAL | AsnType.OCTETSTRING);
		/// <summary>String representation of the AsnType.OCTETSTRING type.</summary>
		public static readonly string SMI_STRING_STR = "OctetString";
		
		/// <summary>Object id ASN.1 type. For implementation, see <see cref="Oid"/></summary>
		public static readonly byte SMI_OBJECTID  = (byte)(AsnType.UNIVERSAL | AsnType.OBJECTID);
		/// <summary>String representation of the SMI_OBJECTID type.</summary>
		public static readonly string SMI_OBJECTID_STR = "ObjectId";
		
		/// <summary>Null ASN.1 value type. For implementation, see <see cref="Null"/>.</summary>
		public static readonly byte SMI_NULL  = (byte)(AsnType.UNIVERSAL | AsnType.NULL);
		/// <summary>String representation of the SMI_NULL type.</summary>
		public static readonly string SMI_NULL_STR = "NULL";
		
		/// <summary> An application string is a sequence of octets
		/// defined at the application level. Although the SMI
		/// does not define an Application String, it does define
		/// an IP Address which is an Application String of length
		/// four.
		/// </summary>
		public static readonly byte SMI_APPSTRING  = (byte)(AsnType.APPLICATION | 0x00);
		/// <summary>String representation of the SMI_APPSTRING type.</summary>
		public static readonly string SMI_APPSTRING_STR = "AppString";
		
		/// <summary> An IP Address is an application string of length four
		/// and is indistinguishable from the SMI_APPSTRING value.
		/// The address is a 32-bit quantity stored in network byte order.
		/// </summary>
		public static readonly byte SMI_IPADDRESS = (byte)(AsnType.APPLICATION | 0x00);
		/// <summary>String representation of the SMI_IPADDRESS type.</summary>
		public static readonly string SMI_IPADDRESS_STR = "IPAddress";
		
		/// <summary> A non-negative integer that may be incremented, but not
		/// decremented. The value is a 32-bit unsigned quantity representing
		/// the range of zero to 2^32-1 (4,294,967,295). When the counter
		/// reaches its maximum value it wraps back to zero and starts again.
		/// </summary>
		public static readonly byte SMI_COUNTER32  = (byte)(AsnType.APPLICATION | 0x01);
		/// <summary>String representation of the SMI_COUNTER32 type.</summary>
		public static readonly string SMI_COUNTER32_STR = "Counter32";
		
		/// <summary> Represents a non-negative integer that may increase or
		/// decrease with a maximum value of 2^32-1. If the maximum
		/// value is reached the gauge stays latched until reset.
		/// </summary>
		public static readonly byte SMI_GAUGE32  = (byte)(AsnType.APPLICATION | 0x02);
		/// <summary>String representation of the SMI_GAUGE32 type.</summary>
		public static readonly string SMI_GAUGE32_STR = "Gauge32";
		
		/// <summary> Used to represent the integers in the range of 0 to 2^32-1.
		/// This type is identical to the SMI_COUNTER32 and are
		/// indistinguishable in ASN.1
		/// </summary>
		public static readonly byte SMI_UNSIGNED32  = (byte)(AsnType.APPLICATION | 0x02); // same as gauge
		/// <summary>String representation of the SMI_UNSIGNED32 type.</summary>
		public static readonly string SMI_UNSIGNED32_STR = "Unsigned32";
		
		/// <summary> This represents a non-negative integer that counts time, modulo 2^32.
		/// The time is represented in hundredths (1/100th) of a second.
		/// </summary>
		public static readonly byte SMI_TIMETICKS  = (byte)(AsnType.APPLICATION | 0x03);
		/// <summary>String representation of the SMI_TIMETICKS type.</summary>
		public static readonly string SMI_TIMETICKS_STR = "TimeTicks";
		
		/// <summary> Used to support the transport of arbitrary data. The
		/// data itself is encoded as an octet string, but may be in
		/// any format defined by ASN.1 or another standard.
		/// </summary>
		public static readonly byte SMI_OPAQUE  = (byte)(AsnType.APPLICATION | 0x04);
		/// <summary>String representation of the SMI_OPAQUE type.</summary>
		public static readonly string SMI_OPAQUE_STR = "Opaque";
		
		/// <summary> Defines a 64-bit unsigned counter. A counter is an integer that
		/// can be incremented, but cannot be decremented. A maximum value
		/// of 2^64 - 1 (18,446,744,073,709,551,615) can be represented.
		/// When the counter reaches it's maximum it wraps back to zero and
		/// starts again.
		/// </summary>
		public static readonly byte SMI_COUNTER64  = (byte)(AsnType.APPLICATION | 0x06); // SMIv2 only
		/// <summary>String representation of the SMI_COUNTER64 type.</summary>
		public static readonly string SMI_COUNTER64_STR = "Counter64";

		/// <summary>String representation of the unknown SMI data type.</summary>
		public static readonly string SMI_UNKNOWN_STR = "Unknown";

		/// <summary> The SNMPv2 error representing that there is No-Such-Object
		/// for a particular object identifier. This error is the result
		/// of a requested object identifier that does not exist in the
		/// agent's tables
		/// </summary>
		public static readonly byte SMI_NOSUCHOBJECT  = (byte)(AsnType.CONTEXT | AsnType.PRIMITIVE | 0x00);
		
		/// <summary> The SNMPv2 error representing that there is No-Such-Instance
		/// for a particular object identifier. This error is the result
		/// of a requested object identifier instance does not exist in the
		/// agent's tables. 
		/// </summary>
		public static readonly byte SMI_NOSUCHINSTANCE  = (byte)(AsnType.CONTEXT | AsnType.PRIMITIVE | 0x01);
		
		/// <summary> The SNMPv2 error representing the End-Of-Mib-View.
		/// This error variable will be returned by a SNMPv2 agent
		/// if the requested object identifier has reached the 
		/// end of the agent's mib table and there is no lexicographic 
		/// successor.
		/// </summary>
		public static readonly byte SMI_ENDOFMIBVIEW  = (byte)(AsnType.CONTEXT | AsnType.PRIMITIVE | 0x02);

		/// <summary>
		/// SEQUENCE Variable Binding code. Hex value: 0x30
		/// </summary>
		public static readonly byte SMI_SEQUENCE = (byte)(AsnType.SEQUENCE | AsnType.CONSTRUCTOR);

		/// <summary> Defines an SNMPv2 Party Clock. The Party Clock is currently
		/// Obsolete, but included for backwards compatibility. Obsoleted in RFC 1902.
		/// </summary>
		public static readonly byte SMI_PARTY_CLOCK = (byte)(AsnType.APPLICATION | 0x07);

		#endregion

		#region SNMP version 2 TRAP OIDs

		/// <summary>
		/// sysUpTime.0 OID is the first value in the VarBind array of SNMP version 2 TRAP packets
		/// </summary>
		public static Oid SysUpTime = new Oid(new UInt32[] { 1, 3, 6, 1, 2, 1, 1, 3, 0 });

		/// <summary>
		/// trapObjectID.0 OID is the second value in the VarBind array of SNMP version 2 TRAP packets
		/// </summary>
		public static Oid TrapObjectId = new Oid(new UInt32[] { 1, 3, 6, 1, 6, 3, 1, 1, 4, 1, 0 });

		#endregion

		#region SNMP version 3 error OID values
		/// <summary>
		/// SNMP version 3, USM error
		/// </summary>
		public static Oid usmStatsUnsupportedSecLevels = new Oid(new UInt32[] { 1, 3, 6, 1, 6, 3, 15, 1, 1, 1, 0 });
		/// <summary>
		/// SNMP version 3, USM error
		/// </summary>
		public static Oid usmStatsNotInTimeWindows = new Oid(new UInt32[] { 1, 3, 6, 1, 6, 3, 15, 1, 1, 2, 0 });
		/// <summary>
		/// SNMP version 3, USM error
		/// </summary>
		public static Oid usmStatsUnknownSecurityNames = new Oid(new UInt32[] { 1, 3, 6, 1, 6, 3, 15, 1, 1, 3, 0 });
		/// <summary>
		/// SNMP version 3, USM error
		/// </summary>
		public static Oid usmStatsUnknownEngineIDs = new Oid(new UInt32[] { 1, 3, 6, 1, 6, 3, 15, 1, 1, 4, 0 });
		/// <summary>
		/// SNMP version 3, USM error
		/// </summary>
		public static Oid usmStatsWrongDigests = new Oid(new UInt32[] { 1, 3, 6, 1, 6, 3, 15, 1, 1, 5, 0 });
		/// <summary>
		/// SNMP version 3, USM error
		/// </summary>
		public static Oid usmStatsDecryptionErrors = new Oid(new UInt32[] { 1, 3, 6, 1, 6, 3, 15, 1, 1, 6, 0 });
		/// <summary>
		/// SNMP version 3, USM error
		/// </summary>
		public static Oid snmpUnknownSecurityModels = new Oid(new UInt32[] { 1, 3, 6, 1, 6, 3, 11, 2, 1, 1, 0 });
		/// <summary>
		/// SNMP version 3, USM error
		/// </summary>
		public static Oid snmpInvalidMsgs = new Oid(new UInt32[] { 1, 3, 6, 1, 6, 3, 11, 2, 1, 2, 0 });
		/// <summary>
		/// SNMP version 3, USM error
		/// </summary>
		public static Oid snmpUnknownPDUHandlers = new Oid(new UInt32[] { 1, 3, 6, 1, 6, 3, 11, 2, 1, 3, 0 });

		/// <summary>
		/// Array of all SNMP version 3 REPORT packet error OIDs
		/// </summary>
		public static Oid[] v3ErrorOids = new Oid[] { usmStatsUnsupportedSecLevels, usmStatsNotInTimeWindows, usmStatsUnknownSecurityNames,
			usmStatsUnknownEngineIDs, usmStatsWrongDigests, usmStatsDecryptionErrors, snmpUnknownSecurityModels, snmpUnknownPDUHandlers };

		#endregion SNMP version 3 error OID values

		#region Helper methods

		/// <summary>Used to create correct variable type object for the specified encoded type</summary>
		/// <param name="asnType">ASN.1 type code</param>
		/// <returns>A new object matching type supplied or null if type was not recognized.</returns>
		public static AsnType GetSyntaxObject(byte asnType)
		{
			AsnType obj = null;
			if (asnType == SnmpConstants.SMI_INTEGER)
				obj = new Integer32();
			else if (asnType == SnmpConstants.SMI_COUNTER32)
				obj = new Counter32();
			else if (asnType == SnmpConstants.SMI_GAUGE32)
				obj = new Gauge32();
			else if (asnType == SnmpConstants.SMI_COUNTER64)
				obj = new Counter64();
			else if (asnType == SnmpConstants.SMI_TIMETICKS)
				obj = new TimeTicks();
			else if (asnType == SnmpConstants.SMI_STRING)
				obj = new OctetString();
			else if (asnType == SnmpConstants.SMI_OPAQUE)
				obj = new Opaque();
			else if (asnType == SnmpConstants.SMI_IPADDRESS)
				obj = new IpAddress();
			else if (asnType == SnmpConstants.SMI_OBJECTID)
				obj = new Oid();
			else if (asnType == SnmpConstants.SMI_PARTY_CLOCK)
				obj = new V2PartyClock();
			else if (asnType == SnmpConstants.SMI_NOSUCHINSTANCE)
				obj = new NoSuchInstance();
			else if (asnType == SnmpConstants.SMI_NOSUCHOBJECT)
				obj = new NoSuchObject();
			else if (asnType == SnmpConstants.SMI_ENDOFMIBVIEW)
				obj = new EndOfMibView();
			else if (asnType == SnmpConstants.SMI_NULL)
			{
				obj = new Null();
			}

			return obj;
		}
		/// <summary>
		/// Return SNMP type object of the type specified by name. Supported variable types are:
		/// * <see cref="Integer32"/>
		/// * <see cref="Counter32"/>
		/// * <see cref="Gauge32"/>
		/// * <see cref="Counter64"/>
		/// * <see cref="TimeTicks"/>
		/// * <see cref="OctetString"/>
		/// * <see cref="IpAddress"/>
		/// * <see cref="Oid"/>
		/// * <see cref="Null"/>
		/// </summary>
		/// <param name="name">Name of the object type</param>
		/// <returns>New <see cref="AsnType"/> object.</returns>
		public static AsnType GetSyntaxObject(string name)
		{
			AsnType obj = null;
			if (name == "Integer32")
				obj = new Integer32();
			else if (name == "Counter32")
				obj = new Counter32();
			else if (name == "Gauge32")
				obj = new Gauge32();
			else if (name == "Counter64")
				obj = new Counter64();
			else if (name == "TimeTicks")
				obj = new TimeTicks();
			else if (name == "OctetString")
				obj = new OctetString();
			else if (name == "IpAddress")
				obj = new IpAddress();
			else if (name == "Oid")
				obj = new Oid();
			else if (name == "Null")
				obj = new Null();
			else
				throw new ArgumentException("Invalid value type name");

			return obj;
		}

		/// <summary>
		/// Return string representation of the SMI value type.
		/// </summary>
		/// <param name="type">AsnType class Type member function value.</param>
		/// <returns>String formatted name of the SMI type.</returns>
		public static string GetTypeName(byte type)
		{
			if( type ==  SMI_IPADDRESS)
				return SMI_IPADDRESS_STR;
			else if( type == SMI_APPSTRING)
				return SMI_APPSTRING_STR;
			else if( type == SMI_COUNTER32)
				return SMI_COUNTER32_STR;
			else if( type ==  SMI_COUNTER64)
				return SMI_COUNTER64_STR;
			else if( type ==  SMI_GAUGE32)
				return SMI_GAUGE32_STR;
			else if( type ==  SMI_INTEGER)
				return SMI_INTEGER_STR;
			else if( type ==  SMI_NULL)
				return SMI_NULL_STR;
			else if( type ==  SMI_OBJECTID)
				return SMI_OBJECTID_STR;
			else if( type ==  SMI_OPAQUE)
				return SMI_OPAQUE_STR;
			else if( type ==  SMI_STRING)
				return SMI_STRING_STR;
			else if( type ==  SMI_TIMETICKS)
				return SMI_TIMETICKS_STR;
			else if( type ==  SMI_UNSIGNED32)
				return SMI_UNSIGNED32_STR;
			else
				return SMI_UNKNOWN_STR;
		}
		/// <summary>
		/// Debugging function used to dump on the console supplied byte array in a format suitable for console output.
		/// </summary>
		/// <param name="data">Byte array data</param>
		public static void DumpHex(byte[] data) {
			int val = 0;
			for(int i=0; i<data.Length; i++ ) {
				if( val == 0 ) {
					Console.Write("{0:d04} ", i);
				}
				Console.Write("{0:x2}", data[i]);
				val += 1;
				if( val == 16 ) {
					val = 0;
					Console.Write("\n");
				} else {
					Console.Write(" ");
				}
			}
			if (val != 0)
				Console.WriteLine("\n");
		}

		/// <summary>
		/// Check if SNMP version value is correct
		/// </summary>
		/// <param name="version">SNMP version value</param>
		/// <returns>true if valid SNMP version, otherwise false</returns>
		public static bool IsValidVersion(int version)
		{
			if (version == (int)SnmpVersion.Ver1 || version == (int)SnmpVersion.Ver2 || version == (int)SnmpVersion.Ver3)
				return true;
			return false;
		}

		#endregion

		/// <summary>
		/// Private constructor to prevent the class with all static members from being instantiated.
		/// </summary>
		private SnmpConstants()
		{
			// nothing
		}
	}
}
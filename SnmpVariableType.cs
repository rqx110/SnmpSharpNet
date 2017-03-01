using System;
using System.Collections.Generic;
using System.Text;

namespace SnmpSharpNet
{
	/// <summary>
	/// Class containing SNMP type constants usable in switch/case evaluation of variable types.
	/// </summary>
	public sealed class SnmpVariableType
	{
		/// <summary>
		/// SNMP type Counter32
		/// </summary>
		public const byte Counter32 = 0x40;
		/// <summary>
		/// SNMP type Counter64
		/// </summary>
		public const byte Counter64 = 0x66;
		/// <summary>
		/// SNMP type EndOfMibView
		/// </summary>
		public const byte EndOfMibView = 0x82;
		/// <summary>
		/// SNMP type Gauge32
		/// </summary>
		public const byte Gauge32 = 0x41;
		/// <summary>
		/// SNMP type Integer
		/// </summary>
		public const byte Integer = 0x02;
		/// <summary>
		/// SNMP type IPAddress
		/// </summary>
		public const byte IPAddress = 0x40;
		/// <summary>
		/// SNMP type NoSuchInstance
		/// </summary>
		public const byte NoSuchInstance = 0x81;
		/// <summary>
		/// SNMP type NoSuchObject
		/// </summary>
		public const byte NoSuchObject = 0x80;
		/// <summary>
		/// SNMP type Null
		/// </summary>
		public const byte Null = 0x05;
		/// <summary>
		/// SNMP type OctetString
		/// </summary>
		public const byte OctetString = 0x04;
		/// <summary>
		/// SNMP type Oid
		/// </summary>
		public const byte Oid = 0x06;
		/// <summary>
		/// SNMP type Opaque
		/// </summary>
		public const byte Opaque = 0x44;
		/// <summary>
		/// SNMP type Sequence
		/// </summary>
		public const byte Sequence = 0x30;
		/// <summary>
		/// SNMP type TimeTicks
		/// </summary>
		public const byte TimeTicks = 0x43;
		/// <summary>
		/// SNMP type Unsigned32
		/// </summary>
		public const byte Unsigned32 = 0x42;
		/// <summary>
		/// SNMP type VarBind
		/// </summary>
		public const byte VarBind = 0x30;

		/// <summary>
		/// Private constructor to prevent the class with all static members from being instantiated.
		/// </summary>
		private SnmpVariableType()
		{
			// nothing
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SnmpSharpNet
{
	/// <summary>
	/// SNMP target interface
	/// </summary>
	public interface ITarget
	{
		/// <summary>
		/// Prepare packet for transmission by filling target specific information in the packet.
		/// </summary>
		/// <param name="packet">SNMP packet class for the required version</param>
		/// <returns>True if packet values are correctly set, otherwise false.</returns>
		bool PreparePacketForTransmission(SnmpPacket packet);
		/// <summary>
		/// Validate received reply
		/// </summary>
		/// <param name="packet">Received SNMP packet</param>
		/// <returns>True if packet is validated, otherwise false</returns>
		bool ValidateReceivedPacket(SnmpPacket packet);
		/// <summary>
		/// Get version of SNMP protocol this target supports
		/// </summary>
		SnmpVersion Version
		{
			get;
			set;
		}
		/// <summary>
		/// Timeout in milliseconds for the target
		/// </summary>
		int Timeout
		{
			get;
			set;
		}
		/// <summary>
		/// Number of retries for the target
		/// </summary>
		int Retry
		{
			get;
			set;
		}
		/// <summary>
		/// Target IP address
		/// </summary>
		IpAddress Address
		{
			get;
		}
		/// <summary>
		/// Target port number
		/// </summary>
		int Port
		{
			get;
			set;
		}
		/// <summary>
		/// Check validity of the target information.
		/// </summary>
		/// <returns>True if valid, otherwise false.</returns>
		bool Valid();
	}
}

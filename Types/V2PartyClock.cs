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
	
	/// <summary>Defines an SNMPv2 Party Clock.</summary>
	/// <remarks>
	/// The Party Clock is currently
	/// Obsolete, but included for backwards compatibility. Obsoleted in RFC 1902.
	/// </remarks>
	public class V2PartyClock:UInteger32,ICloneable
	{
		/// <summary>Constructor</summary>
		public V2PartyClock():base()
		{
			_asnType = SnmpConstants.SMI_PARTY_CLOCK;
		}
		
		/// <summary>Constructor</summary>
		/// <param name="second">Class to duplicate</param>
		public V2PartyClock(V2PartyClock second):base(second)
		{
			_asnType = SnmpConstants.SMI_PARTY_CLOCK;
		}
		
		/// <summary>Constructor</summary>
		/// <param name="uint32">Value to initialize the class with.</param>
		public V2PartyClock(UInteger32 uint32):base(uint32)
		{
			_asnType = SnmpConstants.SMI_PARTY_CLOCK;
		}

		/// <summary>Clone class</summary>
		/// <returns>Cloned class cast as object</returns>
		public override System.Object Clone()
		{
			return new V2PartyClock(this);
		}
		
		/// <summary>Returns the string representation of the object.</summary>
		/// <returns>String representation of the class value.</returns>
		public override System.String ToString()
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			long time = Value;
			long tmp = 0;
			if ((tmp = (time / (24 * 3600 * 100))) > 0)
			{
				buf.Append(tmp).Append("d ");
				time = time % (24 * 3600 * 100);
			}
			else
				buf.Append("0d ");
			
			if ((tmp = time / (3600 * 100)) > 0)
			{
				buf.Append(tmp).Append("h ");
				time = time % (3600 * 100);
			}
			else
				buf.Append("0h ");
			
			if ((tmp = time / 6000) > 0)
			{
				buf.Append(tmp).Append("m ");
				time = time % 6000;
			}
			else
				buf.Append("0m ");
			
			if ((tmp = time / 100) > 0)
			{
				buf.Append(tmp).Append("s ");
				time = time % 100;
			}
			else
				buf.Append("0s ");
			
			buf.Append(tmp * 10).Append("ms");
			
			return buf.ToString();
		}
	}
}
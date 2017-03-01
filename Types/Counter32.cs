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
	/// <summary>SMI Counter32 type implementation.</summary>
	/// <remarks>
	/// Counter32 value type is a 32-bit unsigned integer object that
	/// is incremented by an agent until maximum unsigned integer value
	/// is reached. When maximum value is reached, Counter32 value will
	/// roll over to 0.
	/// </remarks>
	[Serializable]
	public class Counter32:UInteger32, System.ICloneable
	{		
		/// <summary>Constructor</summary>
		public Counter32():base()
		{
			_asnType = SnmpConstants.SMI_COUNTER32;
		}
		
		/// <summary>Constructor</summary>
		/// <param name="second">Copy parameter</param>
		public Counter32(Counter32 second):base(second)
		{
			_asnType = SnmpConstants.SMI_COUNTER32;
		}
		
		/// <summary>Constructor</summary>
		/// <param name="uint32">UInteger32 value</param>
		public Counter32(UInteger32 uint32):base(uint32)
		{
			_asnType = SnmpConstants.SMI_COUNTER32;
		}
		
		/// <summary>Constructor</summary>
		/// <param name="val">Unsigned integer encoded in a string.</param>
		public Counter32(System.String val):base(val)
		{
			_asnType = SnmpConstants.SMI_COUNTER32;
		}

		/// <summary>Constructor.</summary>
		/// <param name="val">UInt32 value</param>
		public Counter32(UInt32 val):base(val)
		{
			_asnType = SnmpConstants.SMI_COUNTER32;
		}
		
		/// <summary>Duplicate current object</summary>
		/// <returns>Duplicate of the current object cast as Object class.</returns>
		public override System.Object Clone()
		{
			return new Counter32(this);
		}

		/// <summary>
		/// Return difference between two Counter32 values taking counter roll-over into account.
		/// </summary>
		/// <param name="first">First or older value</param>
		/// <param name="second">Second or newer value</param>
		/// <returns>Difference between the two values</returns>
		public static UInt32 Diff(Counter32 first, Counter32 second)
		{
			UInt32 f = first.Value;
			UInt32 s = second.Value;
			UInt32 res = 0;
			if (s > f)
			{
				// in case of a roll-over event
				res = (UInt32.MaxValue - f) + s;
			}
			else
			{
				res = s - f;
			}
			return res;
		}
	}
}
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
	/// <summary>ASN.1 Gauge32 value class.
	/// </summary>
	[Serializable]
	public class Gauge32:UInteger32, ICloneable
	{
		/// <summary> Constructs the default counter object.
		/// The initial value is defined
		/// by the super class default constructor
		/// </summary>
		public Gauge32():base()
		{
			_asnType = SnmpConstants.SMI_GAUGE32;
		}
		
		
		/// <summary> Constructs a new object with the same value
		/// as the passed object.
		/// </summary>
		/// <param name="second">The object to recover values from.
		/// </param>
		public Gauge32(Gauge32 second):base(second)
		{
			_asnType = SnmpConstants.SMI_GAUGE32;
		}
		
		/// <summary> Constructs a new object with the value
		/// constrained in the UInteger32 object.
		/// </summary>
		/// <param name="uint32">The UInteger32 object to copy.
		/// </param>
		public Gauge32(UInteger32 uint32):base(uint32)
		{
			_asnType = SnmpConstants.SMI_GAUGE32;
		}
		
		/// <summary>Constructor. Initialize class value with the unsigned integer 32-bit value
		/// encoded as string in the argument.
		/// </summary>
		/// <param name="val">32-bit unsigned integer encoded as a string
		/// </param>
		public Gauge32(System.String val):base(val)
		{
			_asnType = SnmpConstants.SMI_GAUGE32;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="val">Initializing value</param>
		public Gauge32(UInt32 val):base(val)
		{
			_asnType = SnmpConstants.SMI_GAUGE32;
		}
		
		/// <summary>Duplicate current object.
		/// </summary>
		/// <returns>Copy of the object.</returns>
		public override System.Object Clone()
		{
			return new Gauge32(this);
		}
	}
}
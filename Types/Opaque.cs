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


	/// <summary>Opaque type is an application-wide type supports the capability to pass arbitrary
	/// ASN.1 syntax</summary>
	/// <remarks>SMIv2 defines Opaque type as provided solely for backward-compatibility, and
	/// shall not be used for newly-defined object types</remarks>
	[Serializable]
	public class Opaque : OctetString, System.ICloneable
	{
		/// <summary>Constructor</summary>
		public Opaque():base()
		{
			_asnType = SnmpConstants.SMI_OPAQUE;
		}
		
		/// <summary>Constructor</summary>
		/// <param name="data">Data</param>
		public Opaque(byte[] data):base(data)
		{
			_asnType = SnmpConstants.SMI_OPAQUE;
		}
		
		/// <summary>Copy constructor</summary>
		/// <param name="second">The object to be duplicated.</param>
		public Opaque(Opaque second):base(second)
		{
			_asnType = SnmpConstants.SMI_OPAQUE;
		}
		
		/// <summary>Constructor</summary>
		/// <param name="second">The object to be duplicated.</param>
		public Opaque(OctetString second)
			: base(second)
		{
			_asnType = SnmpConstants.SMI_OPAQUE;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="value">Initializer value</param>
		public Opaque(string value)
			: base(value)
		{
			_asnType = SnmpConstants.SMI_OPAQUE;
		}
		
		/// <summary>Returns a duplicate of the current object.</summary>
		/// <returns>Copy of the current object cast as Object</returns>
		public override Object Clone()
		{
			return new Opaque(this);
		}
	}
}
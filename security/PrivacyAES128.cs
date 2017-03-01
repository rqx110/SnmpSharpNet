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

namespace SnmpSharpNet
{
	/// <summary>
	/// AES 128-bit key size privacy protocol implementation class.
	/// </summary>
	public class PrivacyAES128 : PrivacyAES
	{
		/// <summary>
		/// Standard constructor. Initializes the base <see cref="PrivacyAES"/> class with key size 16 bytes (128-bit).
		/// </summary>
		public PrivacyAES128()
			: base(16)
		{
		}

		/// <summary>
		/// Returns privacy protocol name "AES128"
		/// </summary>
		public override string Name
		{
			get
			{
				return "AES128";
			}
		}
	}
}

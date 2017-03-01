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
	/// Privacy class for AES 256-bit encryption. This is a helper class. Full functionality is implemented
	/// in <see cref="PrivacyAES"/> parent class.
    /// </summary>
	public class PrivacyAES256:PrivacyAES
	{
		/// <summary>
		/// Standard constructor initializes encryption key size in the parent <see cref="PrivacyAES"/> class to 32 bytes (256 bit).
		/// </summary>
		public PrivacyAES256()
			: base(32)
		{
		}

		/// <summary>
		/// Returns privacy protocol name "AES256".
		/// </summary>
		public override string Name
		{
			get
			{
				return "AES256";
			}
		}
	}
}

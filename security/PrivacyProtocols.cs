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
	/// Privacy protocol enumeration.
	/// </summary>
	public enum PrivacyProtocols
	{
		/// <summary>
		/// No privacy protocol. Data will not be encrypted
		/// </summary>
		None = 0,
		/// <summary>
		/// Privacy protocol is DES (56 bit encryption)
		/// </summary>
		DES,
		/// <summary>
		/// Privacy protocol is AES-128 (128 bit key). For implementation details, see <see cref="PrivacyAES128"/> and
		/// <see cref="PrivacyAES"/> classes.
		/// </summary>
		AES128,
		/// <summary>
		/// Privacy protocol is AES-192 (128 bit key). For implementation details, see <see cref="PrivacyAES192"/> and
		/// <see cref="PrivacyAES"/> classes.
		/// </summary>
		AES192,
		/// <summary>
		/// Privacy protocol is AES-156 (256 bit key). For implementation details, see <see cref="PrivacyAES256"/> and
		/// <see cref="PrivacyAES"/> classes.
		/// </summary>
		AES256,
		/// <summary>
		/// Privacy protocol is Triple-DES. For implementation details see <see cref="Privacy3DES"/>.
		/// </summary>
		TripleDES
	}
}

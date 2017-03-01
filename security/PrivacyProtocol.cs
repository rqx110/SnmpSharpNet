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
	/// <summary>Privacy protocol helper class.</summary>
	/// <remarks>
	/// This class is used to define privacy protocol encryption type in other
	/// classes using integer constants representing each protocol supported, and allows for easy instantiation
	/// of privacy protocol when used for encryption or decryption of data in a encryption method independent way.
	/// 
	/// Example of how to use this class:
	/// <code>
	/// int myPrivacyProtocol = PrivacyProtocol.AES128;
	/// 
	/// IPrivacyProtocol privacyImplementation = PrivacyProtocol.GetInstance(myPrivacyProtocol);
	/// byte[] result = privacyImplementation.Encrypt(....);
	/// </code>
	/// </remarks>
	public sealed class PrivacyProtocol
	{
		/// <summary>
		/// Based on the supplied privacyProtocol, return instance of the privacy protocol implementation class.
		/// </summary>
		/// <param name="privProtocol">Privacy protocol code. Available protocols are <see cref="PrivacyProtocols.DES"/>, 
		/// <see cref="PrivacyProtocols.AES128"/>, <see cref="PrivacyProtocols.AES192"/>, <see cref="PrivacyProtocols.AES256"/> and
		/// <see cref="PrivacyProtocols.TripleDES"/>.</param>
		/// <returns>Privacy protocol implementation class on success. If privacy protocol is <see cref="PrivacyProtocols.None"/>
		/// then null is returned.</returns>
		public static IPrivacyProtocol GetInstance(PrivacyProtocols privProtocol)
		{
			if (privProtocol == PrivacyProtocols.None)
				return null;
			else if (privProtocol == PrivacyProtocols.DES)
				return new PrivacyDES();
			else if (privProtocol == PrivacyProtocols.AES128)
				return new PrivacyAES128();
			else if (privProtocol == PrivacyProtocols.AES192)
				return new PrivacyAES192();
			else if (privProtocol == PrivacyProtocols.AES256)
				return new PrivacyAES256();
			else if (privProtocol == PrivacyProtocols.TripleDES)
				return new Privacy3DES();
			return null;
		}

		/// <summary>
		/// Private constructor. This class cannot be instantiated.
		/// </summary>
		private PrivacyProtocol()
		{
		}
	}
}

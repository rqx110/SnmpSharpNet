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
	/// Exception of this type is thrown when SNMP version 3 packet containing authentication information
	/// has failed authentication check.
	/// </summary>
	public class SnmpAuthenticationException: SnmpException
	{
		/// <summary>
		/// Standard constructor.
		/// </summary>
		/// <param name="msg">Error message</param>
		public SnmpAuthenticationException(string msg)
			: base(msg)
		{
		}
	}
}

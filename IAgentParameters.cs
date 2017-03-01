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
	/// Every agent parameters class implements this interface
	/// </summary>
	public interface IAgentParameters
	{
		/// <summary>
		/// Get SNMP version number.
		/// </summary>
		SnmpVersion Version
		{
			get;
		}

		/// <summary>
		/// Check validity of the agent properties object.
		/// </summary>
		/// <returns>true if object has all the information needed, otherwise false.</returns>
		bool Valid();

		/// <summary>
		/// Initialize SNMP packet class with values contained in this class.
		/// </summary>
		/// <param name="packet">SNMP packet class</param>
		void InitializePacket(SnmpPacket packet);

		/// <summary>
		/// Duplicate object
		/// </summary>
		/// <returns>Cloned copy of the object</returns>
		object Clone();
	}
}

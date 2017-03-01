// This file is part of SNMP#NET
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
using System.Globalization;
using System.Collections.Generic;
using System.Text;
namespace SnmpSharpNet
{
	
	/// <summary>
	/// SMI Object Identifier type implementation.
	/// </summary>
	[Serializable]
	public class Oid: AsnType, ICloneable, IComparable, IEnumerable<UInt32>
	{
		/// <summary>Internal buffer</summary>
		protected UInt32[] _data;

		#region Constructors

		/// <summary> Creates a default empty object identifier.
		/// </summary>
		public Oid()
		{
			_asnType = SnmpConstants.SMI_OBJECTID;
			_data = null;
		}
		
		/// <summary>Constructor. Initialize ObjectId value to the unsigned integer array</summary>
		/// <param name="data">Integer array representing objectId</param>
		public Oid(UInt32[] data)
			:this()
		{
			Set(data);
		}

		/// <summary>Constructor. Initialize ObjectId value to integer array</summary>
		/// <param name="data">Integer array representing objectId</param>
		public Oid(int[] data)
			:this()
		{
			Set(data);
		}
		
		/// <summary>Constructor. Duplicate objectId value from argument.</summary>
		/// <param name="second">objectId whose value is used to initilize this class value</param>
		public Oid(Oid second)
			:this()
		{
			Set(second);
		}
		
		/// <summary>Constructor. Initialize objectId value from string argument.</summary>
		/// <param name="value">String value representing objectId</param>
		public Oid(System.String value)
			: this()
		{
			Set(value);
		}

		#endregion Constructors

		#region Set members

		/// <summary>
		/// Set Oid value from integer array. If integer array is null or length == 0, internal buffer is set to null.
		/// </summary>
		/// <param name="value">Integer array</param>
		/// <exception cref="ArgumentNullException">Parameter is null</exception>
		/// <exception cref="ArgumentOutOfRangeException">Parameter contains less then 2 integer values</exception>
		/// <exception cref="OverflowException">Paramater contains a value that is less then zero. This is an invalid instance value</exception>
		public virtual void Set(int[] value)
		{
			if (value == null)
				_data = null;
			else
			{
				// Verify all values are greater then or equal 0
				foreach (int i in value)
				{
					if (i < 0)
						throw new OverflowException("OID instance value cannot be less then zero.");
				}
				_data = new UInt32[value.Length];
				for (int i = 0; i < value.Length; i++)
				{
					_data[i] = (UInt32)value[i];
				}
			}
		}

		/// <summary>
		/// Set Oid value from integer array. If integer array is null or length == 0, internal buffer is set to null.
		/// </summary>
		/// <param name="value">Integer array</param>
		/// <exception cref="ArgumentNullException">Parameter is null</exception>
		/// <exception cref="ArgumentOutOfRangeException">Parameter contains less then 2 integer values</exception>
		public virtual void Set(UInt32[] value)
		{
			if (value == null)
				_data = null;
			else
			{
				_data = new UInt32[value.Length];
				Array.Copy(value, 0, _data, 0, value.Length);
			}
		}

		/// <summary>
		/// Set class value from another Oid class.
		/// </summary>
		/// <param name="value">Oid class</param>
		/// <exception cref="ArgumentNullException">Thrown when parameter is null</exception>
		public void Set(Oid value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			Set(value.GetData());
		}

		/// <summary> Sets the object to the passed dotted decimal 
		/// object identifier string.
		/// </summary>
		/// <param name="value">The dotted decimal object identifier.
		/// </param>
		public void Set(System.String value)
		{
			if (value == null || value.Length == 0)
			{
				_data = null;
			}
			else
			{
				_data = Parse(value);
			}
		}

		#endregion Set members

		/// <summary> Gets the number of object identifiers
		/// in the object.
		/// </summary>
		/// <returns> Returns the number of object identifiers
		/// </returns>
		virtual public int Length
		{
			get
			{
				if (_data == null)
					return 0;
				return _data.Length;
			}

		}
		
		/// <summary> Add an array of identifiers to the current object.</summary>
		/// <param name="ids">The array Int32 identifiers to append to the object</param>
		/// <exception cref="OverflowException">Thrown when one of the instance IDs to add are less then zero</exception>
		public virtual void Add(int[] ids)
		{
			if (ids == null || ids.Length == 0)
			{
				return;
			}
			if (_data != null)
			{
				if (ids != null && ids.Length != 0)
				{
					UInt32[] tmp = new UInt32[_data.Length + ids.Length];
					Array.Copy(_data, 0, tmp, 0, _data.Length);
					for (int i = 0; i < ids.Length; i++)
					{
						if (ids[i] < 0)
						{
							throw new OverflowException("Instance value cannot be less then zero.");
						}
						tmp[_data.Length + i] = (UInt32)ids[i];
					}
					_data = tmp;
				}
			}
			else
			{
				_data = new UInt32[ids.Length];
				for (int i = 0; i < ids.Length; i++)
				{
					if (ids[i] < 0)
					{
						throw new OverflowException("Instance value cannot be less then zero.");
					}
					_data[i] = (UInt32)ids[i];
				}
			}
		}

		/// <summary> Add UInt32 identifiers to the current object.</summary>
		/// <param name="ids">The array of identifiers to append</param>
		/// <exception cref="OverflowException">Thrown when one of the instance IDs to add are less then zero</exception>
		public virtual void Add(UInt32[] ids)
		{
			if (ids == null || ids.Length == 0)
			{
				return;
			}
			if (_data != null)
			{
				if (ids != null && ids.Length != 0)
				{
					UInt32[] tmp = new UInt32[_data.Length + ids.Length];
					Array.Copy(_data, 0, tmp, 0, _data.Length);
					Array.Copy(ids, 0, tmp, _data.Length, ids.Length);
					_data = tmp;
				}
			}
			else
			{
				_data = new UInt32[ids.Length];
				Array.Copy(ids, _data, ids.Length);
			}
		}

		/// <summary>Add a single UInt32 id to the end of the object</summary>
		/// <param name="id">Id to add to the oid</param>
		public virtual void Add(UInt32 id)
		{
			if (_data != null)
			{
				UInt32[] tmp = new UInt32[_data.Length + 1];
				Array.Copy(_data, 0, tmp, 0, _data.Length);
				tmp[_data.Length] = id;
				_data = tmp;
			}
			else
			{
				_data = new UInt32[1];
				_data[0] = id;
			}
		}

		/// <summary>Add a single Int32 id to the end of the object</summary>
		/// <param name="id">Id to add to the oid</param>
		/// <exception cref="OverflowException">Thrown when id value is less then zero</exception>
		public virtual void Add(int id)
		{
			if (id < 0)
			{
				throw new OverflowException("Instance id is less then zero.");
			}
			if (_data != null)
			{
				UInt32[] tmp = new UInt32[_data.Length + 1];
				Array.Copy(_data, 0, tmp, 0, _data.Length);
				tmp[_data.Length] = (UInt32)id;
				_data = tmp;
			}
			else
			{
				_data = new UInt32[1];
				_data[0] = (UInt32)id;
			}
		}

		/// <summary> Converts the passed string to an object identifier
		/// and appends them to the current object.
		/// </summary>
		/// <param name="strOids">The dotted decimal identifiers to Append
		/// </param>
		public virtual void Add(string strOids)
		{
			UInt32[] oids = Parse(strOids);
			Add(oids);
		}

		/// <summary> Appends the passed Oid object to 
		/// self.
		/// </summary>
		/// <param name="second">The object to Append to self
		/// </param>
		public virtual void Add(Oid second)
		{
			Add(second.GetData());
		}
		

		#region Comparison methods

		/// <summary>Compare Oid value with array of UInt32 integers</summary>
		/// <param name="ids">Array of integers</param>
		/// <returns>-1 if class is less then, 0 if the same or 1 if greater then the integer array value</returns>
		public virtual int Compare(UInt32[] ids)
		{
			if (ids == null && _data == null)
				return 0;
			else if (ids != null && _data == null)
				return 1;
			else if (ids == null && _data != null)
				return -1;
			return Compare(ids, _data.Length > ids.Length ? ids.Length : _data.Length);
		}

		/// <summary>Compare class value with the contents of the array. Compare up to dist number of Oid values
		/// to determine equality.</summary>
		/// <param name="ids">Unsigned integer array to Compare with</param>
		/// <param name="dist">Number of oid instance values to compare</param>
		/// <returns>0 if equal, -1 if less then and 1 if greater then.</returns>
		public virtual int Compare(UInt32[] ids, int dist)
		{
			if (_data == null)
			{
				if (ids == null)
					return 0;
				return -1;
			}
			if (ids == null)
			{
				return 1;
			}
			if (ids.Length < dist || _data.Length < dist)
			{
				if (_data.Length < ids.Length || _data.Length == ids.Length)
				{
					return -1;
				}
				return 1;
			}
			for (int cnt = 0; cnt < dist; cnt++)
			{
				if (_data[cnt] < ids[cnt])
				{
					return -1;
				}
				else if (_data[cnt] > ids[cnt])
				{
					return 1;
				}
			}
			// If we made it all the way through, the Oids are the same
			return 0;
		}

		/// <summary>
		/// Exact comparison of two Oid values
		/// </summary>
		/// <param name="oid">Oid to compare against</param>
		/// <returns>1 if class is greater then argument, -1 if class value is less then argument, 0 if the same</returns>
		public int CompareExact(Oid oid)
		{
			return CompareExact(oid.GetData());
		}
		/// <summary>
		/// Exact comparison of two Oid values
		/// </summary>
		/// <remarks>
		/// This method is required for cases when exact comparison is required and not lexographical comparison.
		/// 
		/// This method will compare the lengths first and, if not the same, make a comparison determination based
		/// on it before looking into the data.
		/// </remarks>
		/// <param name="ids">Array of unsigned integers to compare against</param>
		/// <returns>1 if class is greater then argument, -1 if class value is less then argument, 0 if the same</returns>
		public int CompareExact(UInt32[] ids)
		{
			int cmpVal = Compare(ids);
			if (cmpVal == 0)
			{
				if (ids == null)
				{
					if (_data == null)
						return 0;
					return 1;
				}
				if (_data == null)
					return -1;
				if (_data.Length != ids.Length)
				{
					if (_data.Length > ids.Length)
						return 1;
					else if (_data.Length < ids.Length)
						return -1;
				}
			}
			return cmpVal;
			/*
			for (int i = 0; i < _data.Length; i++)
			{
				if (_data[i] != ids[i])
				{
					if (_data[i] > ids[i])
						return 1;
					else if (_data[i] < ids[i])
						return -1;
				}
			}
			return 0;
			 */
		}

		/// <summary>Compare objectId values</summary>
		/// <param name="cmp">ObjectId to Compare with</param>
		/// <returns>0 if equal, -1 if less then and 1 if greater then.</returns>
		public virtual int Compare(Oid cmp)
		{
			if (((Object)cmp) == null)
				return 1;
			if (cmp.GetData() == null && _data == null)
				return 0;
			else if (cmp.GetData() != null && _data == null)
				return 1;
			else if (cmp.GetData() == null && _data != null)
				return -1;
			return Compare(cmp.GetData());
		}

		/// <summary> Test for equality. Returns true if 'o' is an instance of an Oid and is equal to self.</summary>
		/// <param name="obj">The object to be tested for equality.</param>
		/// <returns> True if the object is an Oid and is equal to self. False otherwise.</returns>
		public override bool Equals(System.Object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is Oid)
			{
				return (CompareExact(((Oid)obj)._data) == 0);
			}
			else if (obj is System.String)
			{
				return (CompareExact(Parse((System.String)obj)) == 0);
			}
			else if (obj is UInt32[])
			{
				return (CompareExact((UInt32[])obj) == 0);
			}
			return false;
		}

		/// <summary>Compares the passed object identifier against self
		/// to determine if self is the root of the passed object.
		/// If the passed object is in the same root tree as self
		/// then a true value is returned. Otherwise a false value
		/// is returned from the object.
		/// </summary>
		/// <param name="leaf">The object to be tested
		/// </param>
		/// <returns> True if leaf is in the tree.
		/// </returns>
		public virtual bool IsRootOf(Oid leaf)
		{
			return (Compare(leaf._data, _data == null ? 0 : _data.Length) == 0);
		}

		/// <summary>
		/// IComparable interface implementation. Internally uses <see cref="Oid.CompareExact(Oid)"/> method to perform comparisons.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>1 if class is greater then argument, -1 if class value is less then argument, 0 if the same</returns>
		public int CompareTo(object obj)
		{
			if (obj == null)
				return 1;
			if( obj is Oid )
				return CompareExact((Oid)obj);
			return 1;
		}
		#endregion Comparison methods

		/// <summary>
		/// Return internal integer array. This is required by static members of the class and other methods in
		/// this library so internal attribute is applied to it.
		/// </summary>
		/// <returns>Internal unsigned integer array buffer.</returns>
		protected UInt32[] GetData()
		{
			return _data;
		}

		/// <summary>
		/// Reset class value to null
		/// </summary>
		public void Reset()
		{
			if (_data != null)
			{
				_data = null;
			}
		}

		/// <summary>
		/// Is Oid a null value or Oid equivalent (0.0)
		/// </summary>
		public bool IsNull
		{
			get
			{
				if (Length == 0)
					return true;
				if (Length == 2 && _data[0] == 0 && _data[1] == 0)
					return true;
				return false;
			}
		}

		/// <summary>
		/// Convert the Oid class to a integer array. Internal class data buffer is *copied* and not passed to the caller.
		/// </summary>
		/// <returns>Unsigned integer array representing the Oid class IDs</returns>
		public UInt32[] ToArray()
		{
			if (_data == null || _data.Length == 0)
				return null;
			UInt32[] tmp = new UInt32[_data.Length];
			Array.Copy(_data, 0, tmp, 0, _data.Length);
			return tmp;
		}

		/// <summary>
		/// Access individual Oid values.
		/// </summary>
		/// <param name="index">Index of the Oid value to access (0 based)</param>
		/// <returns>Oid instance at index value</returns>
		/// <exception cref="OverflowException">Requested instance is outside the bounds of the Oid array</exception>
		public UInt32 this[int index]
		{
			get
			{
				if (_data == null || index < 0 || index >= _data.Length)
					throw new OverflowException("Requested instance is outside the bounds of the Oid array");
				return _data[index];
			}
		}

		/// <summary>
		/// Return child components of the leaf OID.
		/// </summary>
		/// <param name="root">Root Oid</param>
		/// <param name="leaf">Leaf Oid</param>
		/// <returns>Returns int array of child OIDs, if there was an error or no child IDs are present, returns null.</returns>
		public static UInt32[] GetChildIdentifiers(Oid root, Oid leaf)
		{
			UInt32[] tmp;
			if (((Object)leaf) == null || leaf.IsNull)
			{
				return null;
			}
			else if ((((Object)root) == null || root.IsNull) && ((Object)leaf) != null)
			{
				tmp = new UInt32[leaf.Length];
				Array.Copy(leaf.GetData(), tmp, leaf.Length);
				return tmp;
			}
			if (!root.IsRootOf(leaf))
			{
				// Has to be a child OID
				return null;
			}
			if (leaf.Length <= root.Length)
			{
				return null; // There are not child ids if this oid is longer
			}
			int leafLen = leaf.Length - root.Length;
			tmp = new UInt32[leafLen];
			Array.Copy(leaf.GetData(), root.Length, tmp, 0, leafLen);
			return tmp;
		}

		/// <summary>
		/// Return a string formatted as OID value of the passed integer array
		/// </summary>
		/// <param name="vals">Array of integers</param>
		/// <returns>String formatted OID</returns>
		public static string ToString(int[] vals)
		{
			string r = "";
			if (vals == null)
				return r;
			for (int i = 0; i < vals.Length; i++)
			{
				r += vals[i].ToString(CultureInfo.CurrentCulture);
				if (i != (vals.Length - 1))
				{
					r += ".";
				}
			}
			return r;
		}
		/// <summary>
		/// Return a string formatted as OID value of the passed integer array starting at array item startpos.
		/// </summary>
		/// <param name="vals">Array of integers</param>
		/// <param name="startpos">Start position in the array. 0 based.</param>
		/// <returns>String formatted OID</returns>
		/// <exception cref="IndexOutOfRangeException">Thrown when start position is outside of the bounds of the available data.</exception>
		public static string ToString(int[] vals, int startpos)
		{
			string r = "";
			if (vals == null)
				return r;
			if (startpos < 0 || startpos >= vals.Length)
			{
				throw new IndexOutOfRangeException("Requested value is out of range");
			}
			for (int i = startpos; i < vals.Length; i++)
			{
				r += vals[i].ToString();
				if (i != (vals.Length - 1))
				{
					r += ".";
				}
			}
			return r;
		}

		#region Operators

		/// <summary>
		/// Add Oid class value and oid values in the integer array into a new class instance.
		/// </summary>
		/// <param name="oid">Oid class</param>
		/// <param name="ids">Unsigned integer array to add to the Oid</param>
		/// <returns>New Oid class with the two values added together</returns>
		public static Oid operator +(Oid oid, UInt32[] ids)
		{
			if (((Object)oid) == null && ((Object)ids) == null)
				return null;
			if (((Object)ids) == null)
				return (Oid)oid.Clone();
			Oid newoid = new Oid(oid);
			newoid.Add(ids);
			return newoid;
		}

		/// <summary>
		/// Add Oid class value and oid represented as a string into a new Oid class instance
		/// </summary>
		/// <param name="oid">Oid class</param>
		/// <param name="strOids">string value representing an Oid</param>
		/// <returns>New Oid class with the new oid value.</returns>
		public static Oid operator +(Oid oid, string strOids)
		{
			if (((Object)oid) == null && (strOids == null || strOids.Length == 0))
				return null;
			if (strOids == null || strOids.Length == 0)
				return (Oid)oid.Clone();
			Oid newoid = new Oid(oid);
			newoid.Add(strOids);
			return newoid;
		}

		/// <summary>
		/// Add two Oid values and return the new class
		/// </summary>
		/// <param name="oid1">First Oid</param>
		/// <param name="oid2">Second Oid</param>
		/// <returns>New class with two Oid values added.</returns>
		public static Oid operator +(Oid oid1, Oid oid2)
		{
			if (((Object)oid1) == null && ((Object)oid2) == null )
				return null;
			if ((Object)oid2 == null || oid2.IsNull)
				return (Oid)oid1.Clone();
			if ((Object)oid1 == null)
				return (Oid)oid2.Clone();
			Oid newoid = new Oid(oid1);
			newoid.Add(oid2);
			return newoid;
		}

		/// <summary>
		/// Add integer id to the Oid class
		/// </summary>
		/// <param name="oid1">Oid class to add id to</param>
		/// <param name="id">Id value to add to the oid</param>
		/// <returns>New Oid class with id added to the Oid class.</returns>
		public static Oid operator +(Oid oid1, UInt32 id)
		{
			if (((Object)oid1) == null)
				return null;
			Oid newoid = new Oid(oid1);
			newoid.Add(id);
			return newoid;
		}

		/// <summary>
		/// Operator allowing explicit conversion from Oid class to integer array int[]
		/// </summary>
		/// <param name="oid">Oid to present as integer array int[]</param>
		/// <returns>Integer array representing the Oid class value</returns>
		public static explicit operator UInt32[](Oid oid)
		{
			return oid.ToArray();
		}

		/// <summary>
		/// Comparison of two Oid class values.
		/// </summary>
		/// <param name="oid1">First Oid class</param>
		/// <param name="oid2">Second Oid class</param>
		/// <returns>true if class values are same, otherwise false</returns>
		public static bool operator ==(Oid oid1, Oid oid2)
		{
			// I'm casting the oid values to Object to avoid recursive calls to this operator override.
			if ( ((Object)oid1) == null && ((Object)oid2) == null)
			{
				return true;
			}
			else if (((Object)oid1) == null || ((Object)oid2) == null)
			{
				return false;
			}
			return oid1.Equals(oid2);
		}

		/// <summary>
		/// Negative comparison of two Oid class values.
		/// </summary>
		/// <param name="oid1">First Oid class</param>
		/// <param name="oid2">Second Oid class</param>
		/// <returns>true if class values are not the same, otherwise false</returns>
		public static bool operator !=(Oid oid1, Oid oid2)
		{
			if (((Object)oid1) == null && ((Object)oid2) == null)
			{
				return false;
			}
			else if (((Object)oid1) == null || ((Object)oid2) == null)
			{
				return true;
			}
			return !oid1.Equals(oid2);
		}

		/// <summary>
		/// Greater then operator.
		/// </summary>
		/// <remarks>Compare first oid with second and if first OID is greater return true</remarks>
		/// <param name="oid1">First oid</param>
		/// <param name="oid2">Second oid</param>
		/// <returns>True if first oid is greater then second, otherwise false</returns>
		public static bool operator >(Oid oid1, Oid oid2)
		{
			if (((Object)oid1) == null && ((Object)oid2) == null)
			{
				return false;
			}
			else if (((Object)oid1) == null)
			{
				return false;
			}
			else if (((Object)oid2) == null )
			{
				return true;
			}
			if (oid1.Compare(oid2) > 0)
				return true;
			return false;
		}

		/// <summary>
		/// Less then operator.
		/// </summary>
		/// <remarks>Compare first oid with second and if first OID is less return true</remarks>
		/// <param name="oid1">First oid</param>
		/// <param name="oid2">Second oid</param>
		/// <returns>True if first oid is less then second, otherwise false</returns>
		public static bool operator <(Oid oid1, Oid oid2)
		{
			if (((Object)oid1) == null && ((Object)oid2) == null)
			{
				return false;
			}
			else if (((Object)oid1) == null)
			{
				return true;
			}
			else if (((Object)oid2) == null)
			{
				return false;
			}
			if (oid1.Compare(oid2) < 0)
				return true;
			return false;
		}

		#endregion Operators

		/// <summary> Converts the object identifier to a dotted decimal
		/// string representation.
		/// </summary>
		/// <returns> Returns the dotted decimal object id string.
		/// </returns>
		public override System.String ToString()
		{
			StringBuilder buf = new StringBuilder();
			if (_data == null)
			{
				buf.Append("0.0");
			}
			else
			{
				for (int x = 0; x < _data.Length; x++)
				{
					if (x > 0)
					{
						buf.Append('.');
					}
					buf.Append(_data[x].ToString());
				}
			}
			return buf.ToString();
		}
		
		/// <summary>Hash value for OID value
		/// </summary>
		/// <returns> The hash code for the object.
		/// </returns>
		public override int GetHashCode()
		{
			int hash = 0;

			if (_data == null)
			{
				return hash;
			}

			for (int i = 0; i < _data.Length; i++)
			{
				hash = hash ^ (_data[i] > Int32.MaxValue ? Int32.MaxValue : (int)_data[i]);
			}
			return hash;
		}

		/// <summary>Duplicate current object.</summary>
		/// <returns> Returns a new Oid copy of self cast as Object.</returns>
		public override Object Clone()
		{
			return new Oid(this);
		}

		#region Encode & Decode

		/// <summary>
		/// Encodes ASN.1 object identifier and append it to the end of the passed buffer.
		/// </summary>
		/// <param name="buffer">
		/// Buffer to append the encoded information to.
		/// </param>
		public override void encode(MutableByte buffer)
		{

			MutableByte tmpBuffer = new MutableByte();
			UInt32[] values = _data;
			if (values == null || values.Length < 2)
			{
				values = new UInt32[2];
				values[0] = values[1] = 0;
			}

			// verify that it is a valid object id!
			if (values[0] < 0 || values[0] > 2)
				throw new SnmpException("Invalid Object Identifier");

			if (values[1] < 0 || values[1] > 40)
				throw new SnmpException("Invalid Object Identifier");


			// add the first oid!
			tmpBuffer.Append((byte)(values[0] * 40 + values[1]));

			// encode remaining instance values
			for (int i = 2; i < values.Length; i++)
			{
				tmpBuffer.Append(encodeInstance(values[i]));
			}

			// build value header
			BuildHeader(buffer, Type, tmpBuffer.Length);
			// Append encoded value to the result buffer
			buffer.Append(tmpBuffer);
		}

		/// <summary>
		/// Encode single OID instance value
		/// </summary>
		/// <param name="number">Instance value</param>
		/// <returns>Encoded instance value</returns>
		protected byte[] encodeInstance(UInt32 number)
		{
			MutableByte result = new MutableByte();
			if (number <= 127)
			{
				result.Set((byte)(number));
			}
			else
			{
				UInt32 val = number;
				MutableByte tmp = new MutableByte();
				while (val != 0)
				{
					byte[] b = BitConverter.GetBytes(val);
					byte bval = b[0];
					if ((bval & 0x80) != 0)
					{
						bval = (byte)(bval & ~HIGH_BIT); // clear high bit
					}
					val >>= 7; // shift original value by 7 bits
					tmp.Append(bval);
				}
				// now we need to reverse the bytes for the final encoding
				for (int i = tmp.Length - 1; i >= 0; i--)
				{
					if (i > 0)
						result.Append((byte)(tmp[i] | HIGH_BIT));
					else
						result.Append(tmp[i]);
				}
			}
			return result;
		}

		/// <summary>Decode BER encoded Oid value.</summary>
		/// <param name="buffer">BER encoded buffer</param>
		/// <param name="offset">The offset location to begin decoding</param>
		/// <returns>Buffer position after the decoded value</returns>
		public override int decode(byte[] buffer, int offset)
		{
			int headerLength;
			byte asnType = ParseHeader(buffer, ref offset, out headerLength);

			if (asnType != Type)
				throw new SnmpException("Invalid ASN.1 type.");

			// check for sufficient data
			if ((buffer.Length - offset) < headerLength)
				throw new OverflowException("Buffer underflow error");

			if (headerLength == 0)
			{
				_data = null;
				return offset;
			}

			List<UInt32> list = new List<UInt32>();


			// decode the first byte
			--headerLength;
			UInt32 oid = Convert.ToUInt32(buffer[offset++]);
			list.Add(oid / 40);
			list.Add(oid % 40);

			//
			// decode the rest of the identifiers
			//
			while (headerLength > 0)
			{
				UInt32 result = 0;

				// this is where we decode individual values
				{
					if ((buffer[offset] & HIGH_BIT) == 0)
					{
						// short encoding
						result = (UInt32)buffer[offset];
						offset += 1;
						--headerLength;
					}
					else
					{
						// long encoding
						MutableByte tmp = new MutableByte();
						bool completed = false;
						do
						{
							tmp.Append((byte)(buffer[offset] & ~HIGH_BIT));
							if ((buffer[offset] & HIGH_BIT) == 0)
								completed = true;
							offset += 1; // advance offset
							--headerLength; // take out the processed byte from the header length
						} while (!completed);

						// convert byte array to integer
						for (int i = 0; i < tmp.Length; i++)
						{
							result <<= 7;
							result |= tmp[i];
						}
					}
				}
				list.Add(result);
			}
			_data = list.ToArray();

			if (_data.Length == 2 && _data[0] == 0 && _data[1] == 0)
				_data = null;

			return offset;
		}
		#endregion Encode & Decode

		/// <summary>Parse string formatted oid value into an array of integers</summary>
		/// <param name="oidStr">string formatted oid</param>
		/// <returns>Integer array representing the oid or null if invalid object id was passed</returns>
		private static UInt32[] Parse(System.String oidStr)
		{
			if (oidStr == null || oidStr.Length <= 0)
			{
				return null;
			}
			// verify correct values are the only ones present in the string
			foreach (char c in oidStr.ToCharArray())
			{
				if (!char.IsNumber(c) && c != '.')
				{
					return null;
				}
			}
			// check if oid starts with a '.' and remove it if it does
			if (oidStr[0] == '.')
			{
				oidStr = oidStr.Remove(0, 1);
			}
			// split string into an array
			string[] splitString = oidStr.Split(new char[] { '.' }, StringSplitOptions.None);

			// if we didn't find any entries, return null
			if (splitString.Length < 0)
				return null;

			List<UInt32> result = new List<UInt32>();
			foreach (string s in splitString)
			{
				result.Add(Convert.ToUInt32(s));
			}
			return result.ToArray();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the Oid integer collection
		/// </summary>
		/// <returns>An IEnumerator  object that can be used to iterate through the collection.</returns>
		public IEnumerator<UInt32> GetEnumerator()
		{
			if( _data != null )
				return ((IEnumerable<UInt32>)_data).GetEnumerator();
			return null;
		}
		/// <summary>
		/// Returns an enumerator that iterates through the Oid integer collection
		/// </summary>
		/// <returns>An IEnumerator  object that can be used to iterate through the collection.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			if (_data != null)
				return _data.GetEnumerator();
			return null;
		}

		/// <summary>
		/// Return instance of Oid class set to null value {0,0}
		/// </summary>
		/// <returns>Oid class instance set to null Oid value</returns>
		public static Oid NullOid()
		{
			return new Oid(new UInt32[] { 0, 0 });
		}
	}
}
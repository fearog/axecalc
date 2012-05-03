// Based on http://www.codeproject.com/Articles/9280/Add-Remove-Items-to-from-PropertyGrid-at-Runtime

// AxeCalc - guitar design software https://github.com/fearog/axecalc
// Copyright (C) 2012 Tristan Williams

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace AxeCalc
{
	/// <summary>
	/// CustomClass (Which is binding to property grid)
	/// </summary>
	public class CustomClass : CollectionBase, ICustomTypeDescriptor
	{
		/// <summary>
		/// Add CustomProperty to Collectionbase List
		/// </summary>
		/// <param name="Value"></param>
		public void Add( CustomProperty Value )
		{
			base.List.Add( Value );
		}

		public void AddMarkedUpVariables( object o )
		{
			Type t = o.GetType();
			foreach( MemberInfo member in t.GetMembers( BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public ) )
			{
				object[] attributes = member.GetCustomAttributes( typeof( DisplayAttribute ), true );
				if( attributes.Length == 0 )
					continue;

				if( member.MemberType == MemberTypes.Field )
				{
					FieldInfo field = ( FieldInfo )member;
					Add( new CustomProperty( o, field, attributes ) );
				}
				else if( member.MemberType == MemberTypes.Property )
				{
					PropertyInfo property = ( PropertyInfo )member;
					Add( new CustomProperty( o, property, attributes ) );
				}
			}
		}

		/// <summary>
		/// Remove item from List
		/// </summary>
		/// <param name="Name"></param>
		public void Remove( string Category, string Name )
		{
			foreach( CustomProperty prop in base.List )
			{
				if( prop.Name == Name && prop.Category == Category )
				{
					base.List.Remove( prop );
					return;
				}
			}
		}

		/// <summary>
		/// Remove items from List
		/// </summary>
		/// <param name="Name"></param>
		public void RemoveCategory( string Category )
		{
			for( int i = 0; i < base.List.Count; ++i )
			{
				CustomProperty prop = ( CustomProperty )base.List[ i ];
				if( prop.Category == Category )
				{
					base.List.Remove( prop );
					--i;
				}
			}
		}


		/// <summary>
		/// Indexer
		/// </summary>
		public CustomProperty this[ int index ]
		{
			get
			{
				return ( CustomProperty )base.List[ index ];
			}
			set
			{
				base.List[ index ] = ( CustomProperty )value;
			}
		}


		#region "TypeDescriptor Implementation"
		/// <summary>
		/// Get Class Name
		/// </summary>
		/// <returns>String</returns>
		public String GetClassName()
		{
			return TypeDescriptor.GetClassName( this, true );
		}

		/// <summary>
		/// GetAttributes
		/// </summary>
		/// <returns>AttributeCollection</returns>
		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes( this, true );
		}

		/// <summary>
		/// GetComponentName
		/// </summary>
		/// <returns>String</returns>
		public String GetComponentName()
		{
			return TypeDescriptor.GetComponentName( this, true );
		}

		/// <summary>
		/// GetConverter
		/// </summary>
		/// <returns>TypeConverter</returns>
		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter( this, true );
		}

		/// <summary>
		/// GetDefaultEvent
		/// </summary>
		/// <returns>EventDescriptor</returns>
		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent( this, true );
		}

		/// <summary>
		/// GetDefaultProperty
		/// </summary>
		/// <returns>PropertyDescriptor</returns>
		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty( this, true );
		}

		/// <summary>
		/// GetEditor
		/// </summary>
		/// <param name="editorBaseType">editorBaseType</param>
		/// <returns>object</returns>
		public object GetEditor( Type editorBaseType )
		{
			return TypeDescriptor.GetEditor( this, editorBaseType, true );
		}

		public EventDescriptorCollection GetEvents( Attribute[] attributes )
		{
			return TypeDescriptor.GetEvents( this, attributes, true );
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents( this, true );
		}

		public PropertyDescriptorCollection GetProperties( Attribute[] attributes )
		{
			PropertyDescriptor[] newProps = new PropertyDescriptor[ this.Count ];
			int iResultCount = 0;
			for( int i = 0; i < this.Count; i++ )
			{
				CustomProperty prop = ( CustomProperty )this[ i ];
				if( prop.Visible )
				{
					newProps[ iResultCount ] = new CustomPropertyDescriptor( ref prop, attributes );
					++iResultCount;
				}
			}
			Array.Resize( ref newProps, iResultCount );
			newProps = newProps.OrderBy( x => x.DisplayName ).ToArray();

			return new PropertyDescriptorCollection( newProps );
		}

		public PropertyDescriptorCollection GetProperties()
		{
			return TypeDescriptor.GetProperties( this, true );
		}

		public object GetPropertyOwner( PropertyDescriptor pd )
		{
			return this;
		}
		#endregion

	}

	public class TristanExpandableObjectConverter : TypeConverter
	{

		public TristanExpandableObjectConverter()
		{
		}

		public override PropertyDescriptorCollection GetProperties( ITypeDescriptorContext context,
										object value, Attribute[] inAttributes )
		{
			//	return TypeDescriptor.GetProperties( value, attributes );
			/*	PropertyDescriptor[] newProps = new PropertyDescriptor[ this.Count ];
				for( int i = 0; i < this.Count; i++ )
				{
					CustomProperty prop = ( CustomProperty )this[ i ];
					if( prop.Visible )
						newProps[ i ] = new CustomPropertyDescriptor( ref prop, attributes );
				}

				return new PropertyDescriptorCollection( newProps );*/
			Type t = value.GetType();
			MemberInfo[] members = t.GetMembers( BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
			MemberInfo[] interestingMembers = members.Where( member => member.GetCustomAttributes( typeof( DisplayAttribute ), true ).Length > 0 ).ToArray();
			PropertyDescriptor[] newProps = new PropertyDescriptor[ interestingMembers.Length ];
			for( int i = 0; i < interestingMembers.Length; i++ )
			{
				MemberInfo member = interestingMembers[ i ];
				object[] attributes = member.GetCustomAttributes( typeof( DisplayAttribute ), true );
				CustomProperty prop = null;
				if( member.MemberType == MemberTypes.Field )
				{
					FieldInfo field = ( FieldInfo )member;
					prop = new CustomProperty( value, field, attributes );
				}
				else if( member.MemberType == MemberTypes.Property )
				{
					PropertyInfo property = ( PropertyInfo )member;
					prop = new CustomProperty( value, property, attributes );
				}
				if( prop != null && prop.Visible )
					newProps[ i ] = new CustomPropertyDescriptor( ref prop, inAttributes );
			}

			return new PropertyDescriptorCollection( newProps );

		/*	foreach( MemberInfo member in t.GetMembers( BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public ) )
			{
				object[] attributes = member.GetCustomAttributes( typeof( DisplayAttribute ), true );
				if( attributes.Length == 0 )
					continue;

				if( member.MemberType == MemberTypes.Field )
				{
					FieldInfo field = ( FieldInfo )member;
					Add( new CustomProperty( o, field, attributes ) );
				}
				else if( member.MemberType == MemberTypes.Property )
				{
					PropertyInfo property = ( PropertyInfo )member;
					Add( new CustomProperty( o, property, attributes ) );
				}
			}*/
		}

		public override bool GetPropertiesSupported( ITypeDescriptorContext context )
		{
			return true;
		}
	}

	/// <summary>
	/// Custom property class 
	/// </summary>
	public class CustomProperty
	{
		private string sName = string.Empty;
		private bool bReadOnly = false;
		private bool bVisible = true;
		private string sDescription = string.Empty;
		private string sCategory = string.Empty;
		private object parent = null;
		private Type parentType = null;
		private PropertyInfo property = null;
		private int identifier = -1;
		private Type valueType = null;

		public delegate object ValueGetter( int identifier );
		public delegate void ValueSetter( int identifier, object value );
		private ValueGetter getter = null;
		private ValueSetter setter = null;

		public delegate bool FlagTester();
		private FlagTester readOnlyTester = null;
		private FlagTester visibleTester = null;

		public CustomProperty( object parent, PropertyInfo property, object[] displayAttributes )
		{
			this.bReadOnly = !property.CanWrite;
			this.parent = parent;
			this.parentType = parent.GetType();
			this.property = property;
			valueType = property.PropertyType;

			ApplyAttributes( displayAttributes );
		}

		void ApplyAttributes( object[] displayAttributes )
		{
			readOnlyTester = delegate() { return bReadOnly; };
			visibleTester = delegate() { return bVisible; };


			foreach( object attribute in displayAttributes )
			{
				Type t = attribute.GetType();
				if( t == typeof( CategoryAttribute ) )
					sCategory = ( ( CategoryAttribute )attribute ).m_strText;
				else if( t == typeof( DescriptionAttribute ) )
					sDescription = ( ( DescriptionAttribute )attribute ).m_strText;
				else if( t == typeof( NameAttribute ) )
					sName = ( ( NameAttribute )attribute ).m_strText;
				else if( t == typeof( ReadOnlyAttribute ) )
					readOnlyTester = ( ( ReadOnlyAttribute )attribute ).GetDelegate( parent );
				else if( t == typeof( VisibleAttribute ) )
					visibleTester = ( ( VisibleAttribute )attribute ).GetDelegate( parent );
			}
		}

		public CustomProperty( string sCategory, string sName, string sDescription, string sPropertyName, object parent, bool bReadOnly, bool bVisible )
		{
			this.sCategory = sCategory;
			this.sDescription = sDescription;
			this.sName = sName;
			this.bReadOnly = bReadOnly;
			this.bVisible = bVisible;
			this.parent = parent;
			this.parentType = parent.GetType();

			// find the property
			property = parentType.GetProperty( sPropertyName );
			valueType = property.PropertyType;

			readOnlyTester = delegate() { return bReadOnly; };
			visibleTester = delegate() { return bVisible; };
		}

		public CustomProperty( string sCategory, string sName, string sDescription, int identifier, ValueGetter g, ValueSetter s, bool bReadOnly, bool bVisible )
		{
			this.sCategory = sCategory;
			this.sDescription = sDescription;
			this.sName = sName;
			this.bReadOnly = bReadOnly;
			this.bVisible = bVisible;
			this.identifier = identifier;
			this.getter = g;
			this.setter = s;
			valueType = g( identifier ).GetType();

			readOnlyTester = delegate() { return bReadOnly; };
			visibleTester = delegate() { return bVisible; };
		}

		FieldInfo m_field = null;

		public CustomProperty( object parent, FieldInfo field, object[] displayAttributes )
		{
			this.parent = parent;
			this.parentType = parent.GetType();
			this.identifier = 0;
			m_field = field;
			valueType = field.FieldType;
			getter = id => { return m_field.GetValue( parent ); };
			setter = ( id, v ) => { m_field.SetValue( parent, v ); };

			ApplyAttributes( displayAttributes );
		}

		public bool ReadOnly
		{
			get
			{
				return readOnlyTester();
			}
		}

		public string Name
		{
			get
			{
				return sName;
			}
		}
		public string Description
		{
			get { return sDescription; }
		}
		public string Category
		{
			get { return sCategory; }
		}

		public bool Visible
		{
			get
			{
				return visibleTester();
			}
		}

		public object Value
		{
			get
			{
				object value;
				if( getter != null )
				{
					value = getter( identifier );
				}
				else
					value = property.GetValue( parent, null );

				if( valueType.Name == "Double" )
				{
					return ( (double)value ).ToString( "F3" );
				}
				return value;
			}
			set
			{
				object inValue = value;
				if( valueType.Name == "Double" )
				{
					inValue = Double.Parse( ( string )inValue );
				}

				if( setter != null )
				{
					setter( identifier, inValue );
				}
				else
					property.SetValue( parent, inValue, null );
			}
		}

	}


	/// <summary>
	/// Custom PropertyDescriptor
	/// </summary>
	public class CustomPropertyDescriptor : PropertyDescriptor
	{
		CustomProperty m_Property;

		public CustomPropertyDescriptor( ref CustomProperty myProperty, Attribute[] attrs )
			: base( myProperty.Name, attrs )
		{
			m_Property = myProperty;
		}

		#region PropertyDescriptor specific

		public override bool CanResetValue( object component )
		{
			return false;
		}

		public override Type ComponentType
		{
			get
			{
				return null;
			}
		}

		public override object GetValue( object component )
		{
			return m_Property.Value;
		}

		public override string Description
		{
			get
			{
				return m_Property.Description;
			}
		}

		public override string Category
		{
			get
			{
				return m_Property.Category;
			}
		}

		public override string DisplayName
		{
			get
			{
				return m_Property.Name;
			}

		}



		public override bool IsReadOnly
		{
			get
			{
				return m_Property.ReadOnly;
			}
		}

		public override void ResetValue( object component )
		{
			//Have to implement
		}

		public override bool ShouldSerializeValue( object component )
		{
			return false;
		}

		public override void SetValue( object component, object value )
		{
			m_Property.Value = value;
		}

		public override Type PropertyType
		{
			get { return m_Property.Value.GetType(); }
		}

		#endregion

		//
		// Summary:
		//     Gets the type converter for this property.
		//
		// Returns:
		//     A System.ComponentModel.TypeConverter that is used to convert the System.Type
		//     of this property.
		public override TypeConverter Converter
		{
			get 
			{ 
				if( PropertyType.Name != "String" && PropertyType.IsClass )
					return new TristanExpandableObjectConverter();
				else
					return base.Converter;
			}
		}

	}
}

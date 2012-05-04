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
using System.Reflection;
using System.ComponentModel;

namespace AxeCalc
{
	[AttributeUsage( AttributeTargets.Field )]
	public class NoSaveAttribute : Attribute
	{
	}

	public class DisplayAttribute : Attribute
	{
	}

	[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
	public class CategoryAttribute : DisplayAttribute
	{
		public string m_strText;
		public CategoryAttribute( string str )
		{
			m_strText = str;
		}
	}
	[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
	public class NameAttribute : DisplayAttribute
	{
		public string m_strText;
		public NameAttribute( string str )
		{
			m_strText = str;
		}
	}

	[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
	public class DescriptionAttribute : DisplayAttribute
	{
		public string m_strText;
		public DescriptionAttribute( string str )
		{
			m_strText = str;
		}
	}

	public class ValueAttributeHelper<T>
	{
		public bool m_bValueSet = false;
		public T m_value;
		public string m_strCallbackName;
		public int m_iIdentifier = -1;

		public delegate T GetValueDelegate();

		public ValueAttributeHelper( T value )
		{
			m_bValueSet = true;
			m_value = value;
		}

		public ValueAttributeHelper( string strCallbackName, int iIdentifier = -1 )
		{
			m_strCallbackName = strCallbackName;
			m_iIdentifier = iIdentifier;
		}

		public GetValueDelegate GetDelegate( object o )
		{
			if( m_bValueSet )
				return delegate() { return m_value; };

			// find the callback in question
		/*	MethodInfo method = o.GetType().GetMethods( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance ).Single( v => v.Name == m_strCallbackName );
			if( method != null )
			{
				ParameterInfo[] parms = method.GetParameters();
				if( m_iIdentifier == -1 )
				{
					return delegate()
					{
						object returnValue = method.Invoke( o, null );
						return ( T )returnValue;
					};
				}
				else
				{
					object[] input = new object[ 1 ];
					input[ 0 ] = m_iIdentifier;
					return delegate()
					{
						object returnValue = method.Invoke( o, input );
						return ( T )returnValue;
					};
				}
			}*/

			// not a method! check parameters
			MemberInfo[] members = o.GetType().GetMember( m_strCallbackName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
			if( members.Length == 0 )
			{
				System.Diagnostics.Debug.Assert( false );
				return null;
			}

			System.Diagnostics.Debug.Assert( members.Length == 1 );
			MemberInfo member = members[ 0 ];
			if( member is MethodInfo )
			{
				MethodInfo method = member as MethodInfo;
				ParameterInfo[] parms = method.GetParameters();
				if( m_iIdentifier == -1 )
				{
					return delegate()
					{
						object returnValue = method.Invoke( o, null );
						return ( T )returnValue;
					};
				}
				else
				{
					object[] input = new object[ 1 ];
					input[ 0 ] = m_iIdentifier;
					return delegate()
					{
						object returnValue = method.Invoke( o, input );
						return ( T )returnValue;
					};
				}
			}
			else if( member is PropertyInfo )
			{
				PropertyInfo property = member as PropertyInfo;
				System.Diagnostics.Debug.Assert( m_iIdentifier == -1 );
				return delegate()
				{
					object returnValue = property.GetValue( o, null );
					return ( T )returnValue;
				};
			}
			else if( member is FieldInfo )
			{
				FieldInfo field = member as FieldInfo;
				System.Diagnostics.Debug.Assert( m_iIdentifier == -1 );
				return delegate()
				{
					object returnValue = field.GetValue( o );
					return ( T )returnValue;
				};
			}
			else
			{
			}

			System.Diagnostics.Debug.Assert( false );
			return null;
		}
	}

	[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
	public class BoolAttribute : DisplayAttribute
	{
		ValueAttributeHelper<bool> m_helper;

		public BoolAttribute( bool bValue )
		{
			m_helper = new ValueAttributeHelper<bool>( bValue );
		}

		public BoolAttribute( string strCallbackName, int iIdentifier = -1 )
		{
			m_helper = new ValueAttributeHelper<bool>( strCallbackName, iIdentifier );
		}

		public CustomProperty.FlagTester GetDelegate( object o )
		{
			ValueAttributeHelper<bool>.GetValueDelegate d = m_helper.GetDelegate( o );
			return delegate() { return d(); };
		}
	}

	[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
	public class ReadOnlyAttribute : BoolAttribute
	{
		public ReadOnlyAttribute( bool bReadOnly ) : base( bReadOnly ) { }
		public ReadOnlyAttribute( string strCallbackName, int iIdentifier = -1 ) : base( strCallbackName, iIdentifier ) { }
	}

	[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
	public class VisibleAttribute : BoolAttribute
	{
		public VisibleAttribute( bool bVisible ) : base( bVisible ) { }
		public VisibleAttribute( string strCallbackName, int iIdentifier = -1 ) : base( strCallbackName, iIdentifier ) { }
	}
}

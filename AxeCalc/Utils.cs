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
using System.Xml;
using System.ComponentModel;
using System.Reflection;

namespace AxeCalc
{
	class Utils
	{
		public static void SaveObject( XmlTextWriter xml, object o )
		{
			// generically reflect the fields
			Type t = o.GetType();
			System.Reflection.FieldInfo[] fields = t.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
			foreach( System.Reflection.FieldInfo field in fields )
			{
				if( field.GetCustomAttributes( typeof( NoSaveAttribute ), true ).Length > 0 )
					continue;

				xml.WriteStartElement( "member" );
				xml.WriteAttributeString( "name", field.Name );
				xml.WriteAttributeString( "type", field.FieldType.Name );

				if( field.FieldType.Name == "Double" )
					xml.WriteAttributeString( "value", ( ( double )field.GetValue( o ) ).ToString( "F3" ) );
				else if( field.FieldType.IsClass )
					SaveObject( xml, field.GetValue( o ) );
				else
					xml.WriteAttributeString( "value", field.GetValue( o ).ToString() );
				xml.WriteEndElement();
			}
		}

		public static void LoadObject( XmlNode xml, object o )
		{
			// generically reflect the fields
			Type t = o.GetType();
			System.Reflection.FieldInfo[] fields = t.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
			foreach( System.Reflection.FieldInfo field in fields )
			{
				if( field.GetCustomAttributes( typeof( NoSaveAttribute ), true ).Length > 0 )
					continue;

				XmlNode member;
				try
				{
					member = Utils.getXmlNodeByAttribute( xml, "member", "name", field.Name );
				}
				catch
				{
					// TODO: Some sort of warnings or something? Maybe call into a patching thing?
					continue;
				}

				string type = Utils.getXmlNodeAttribute<string>( member, "type" );
				if( type != field.FieldType.Name )
					throw new System.Exception( "Loaded member " + field.Name + " has different type " + type );

				if( field.FieldType.IsClass )
					LoadObject( member, field.GetValue( o ) );
				else
					field.SetValue( o, Utils.getXmlNodeAttribute( member, "value", field.FieldType ) );
			}
		}


		public static T getXmlNodeAttribute<T>( XmlNode node, string strName )
		{
			foreach( XmlNode attribute in node.Attributes )
			{
				if( attribute.Name == strName )
					return ( T )Convert.ChangeType( attribute.Value, typeof( T ) );
			}

			throw new System.Exception( "Couldn't find attribute " + strName );
		}
		public static object getXmlNodeAttribute( XmlNode node, string strName, Type t )
		{
			foreach( XmlNode attribute in node.Attributes )
			{
				if( attribute.Name == strName )
				{
					if( !t.IsEnum )
						return Convert.ChangeType( attribute.Value, t );
					else
						return Enum.Parse( t, attribute.Value );
				}
			}

			throw new System.Exception( "Couldn't find attribute " + strName );
		}
		public static XmlNode getXmlNodeByName( XmlNode node, string strElementName )
		{
			foreach( XmlNode subnode in node.ChildNodes )
			{
				if( subnode.Name == strElementName )
					return subnode;
			}

			throw new System.Exception( "Couldn't find subnode with name " + strElementName );
		}
		public static XmlNode getXmlNodeByAttribute( XmlNode node, string strElementName, string strAttribute, string strAttributeValue )
		{
			foreach( XmlNode subnode in node.ChildNodes )
			{
				if( subnode.Name == strElementName )
				{
					foreach( XmlNode attribute in subnode.Attributes )
					{
						if( attribute.Name == strAttribute && attribute.Value == strAttributeValue )
						{
							return subnode;
						}
					}
				}
			}

			throw new System.Exception( "Couldn't find subnode with attribute " + strAttributeValue );
		}
	}
}

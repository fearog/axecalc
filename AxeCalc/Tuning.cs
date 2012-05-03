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

namespace AxeCalc
{
	class Tuning
	{
		List<string> m_strings;
		GuitarProperties m_guitar;
		CustomClass m_calculatedStuff;

		public Tuning( GuitarProperties guitar, CustomClass calculatedStuff )
		{
			m_strings = new List<string>();
			m_guitar = guitar;
			m_calculatedStuff = calculatedStuff;
			SetStandardTuning();
		}

		public void Save( XmlWriter xml )
		{
			xml.WriteStartElement( "tuning" );
			xml.WriteAttributeString( "storedStringCount", m_strings.Count.ToString() );
			for( int i = 0; i < m_strings.Count; ++i )
			{
				xml.WriteStartElement( "string" + i.ToString() );
				xml.WriteAttributeString( "value", m_strings[ i ] );
				xml.WriteEndElement();
			}
			xml.WriteEndElement();
		}

		public void Load( XmlNode xml )
		{
			XmlNode tuning = Utils.getXmlNodeByName( xml, "tuning" );
			int iStringCount = Utils.getXmlNodeAttribute<int>( tuning, "storedStringCount" );
			m_strings.Clear();
			for( int i = 0; i < iStringCount; ++i )
			{
				XmlNode stringn = Utils.getXmlNodeByName( tuning, "string" + i.ToString() );
				m_strings.Add( Utils.getXmlNodeAttribute<string>( stringn, "value" ) );
			}
		}

		public void SetStandardTuning()
		{
			m_strings.Clear();
			m_strings.Add( "E4" );
			m_strings.Add( "B3" );
			m_strings.Add( "G3" );
			m_strings.Add( "D3" );
			m_strings.Add( "A2" );
			m_strings.Add( "E2" );
			m_strings.Add( "B1" );
			m_strings.Add( "F#1" );
		}

		public void SetStringCount( int iStringCount )
		{
			// If need be, add extra notes to the new strings.
			for( int i = m_strings.Count; i < iStringCount; ++i )
				m_strings.Add( "F#1" );
		}

		public void SetupProperties()
		{
			m_guitar.RemoveCategory( "Tuning" );
			m_calculatedStuff.RemoveCategory( "Compensation" );
			m_calculatedStuff.RemoveCategory( "Strings" );
			for( int i = 0; i < m_guitar.StringCount; ++i )
			{
				m_guitar.Add( new CustomProperty( "Tuning", "String " + ( i + 1 ).ToString() + " note", "Note that string " + ( i + 1 ).ToString() + " will be tuned to", i, GetStringTuning, SetStringTuning, false, true ) );
				m_calculatedStuff.Add( new CustomProperty( "Compensation", "Comp. length " + ( i + 1 ).ToString(), "Compensated string length", i, GetStringLength, null, true, true ) );
				m_calculatedStuff.Add( new CustomProperty( "Strings", "Ideal string " + ( i + 1 ).ToString(), "Ideal D'Addario string to use for string " + ( i + 1 ).ToString(), i, GetIdealString, null, true, true ) );
				//	m_calculatedStuff.Add( new CustomProperty( "Strings", "Ideal plain string " + ( i + 1 ).ToString(), "Ideal D'Addario plain string to use for string " + ( i + 1 ).ToString(), i, GetIdealPlainString, null, true, true ) );
				//	m_calculatedStuff.Add( new CustomProperty( "Strings", "Ideal wound string " + ( i + 1 ).ToString(), "Ideal D'Addario wound string to use for string " + ( i + 1 ).ToString(), i, GetIdealWoundString, null, true, true ) );
			}
		}

		public string GetStringTuning( int iStringNumber )
		{
			return m_strings[ iStringNumber ];
		}
		private void SetStringTuning( int iStringNumber, object value )
		{
			m_strings[ iStringNumber ] = ( string )value;
		}

		private object GetStringLength( int iStringNumber )
		{
			return m_guitar.GetStringLine( m_guitar.StringCount - iStringNumber - 1 ).length() + m_guitar.GetRequiredCompensation( m_guitar.StringCount - iStringNumber - 1, m_guitar.m_dStringHeightAt12th );
		}

		private object GetIdealString( int iStringNumber )
		{
			double dResultantTension = 0;
			StringInfo s = m_guitar.GetIdealString( m_guitar.StringCount - iStringNumber - 1, ref dResultantTension );
			return s.m_strName + " -> " + dResultantTension.ToString( "F1" ) + "lb/in";
		}
		private object GetIdealPlainString( int iStringNumber )
		{
			double dResultantTension = 0;
			StringInfo s = m_guitar.GetIdealPlainString( m_guitar.StringCount - iStringNumber - 1, ref dResultantTension );
			return s.m_strName + " -> " + dResultantTension.ToString( "F1" ) + "lb/in";
		}
		private object GetIdealWoundString( int iStringNumber )
		{
			double dResultantTension = 0;
			StringInfo s = m_guitar.GetIdealWoundString( m_guitar.StringCount - iStringNumber - 1, ref dResultantTension );
			return s.m_strName + " -> " + dResultantTension.ToString( "F1" ) + "lb/in";
		}
	}
}

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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Data;

namespace AxeCalc
{
	class GuitarProperties : CustomClass
	{
		private int m_iStringCount;

		[Category( "General" ), Name( "Fret count" ), Description( "Number of frets" )]
		private int m_iFretCount;
		[Category( "Scale" ), Name( "Neutral fret" ), Description( "On a multiscale guitar this is the fret that is not angled" ), Visible( "MultiScale" )]
		private int m_iStraightFret;

		[Category( "Scale" ), Name( "Bass scale Length" ), Description( "Distance from nut to bridge of the bass string" )]
		private double m_fBassScaleLength;
		[Category( "Scale" ), Name( "Treble scale Length" ), Description( "Distance from nut to bridge of the treble string" )]
		private double m_fTrebleScaleLength;

		[Category( "String Spacing" ), Name( "Nut string spacing" ), Description( "String spacing at the nut (between each string)" )]
		private double m_fPerStringSpacingAtNut;
		[Category( "String Spacing" ), Name( "Bridge string spacing" ), Description( "String spacing at the bridge (between each string)" )]
		private double m_fPerStringSpacingAtBridge;

		[Category( "General" ), Name( "Fretboard edge @ Nut/Zero fret" ), Description( "Width between the outside strings and edge of fretboard at the nut (or zero fret, if it is used)" )]
		private double m_fFretboardEdgeAtNut;
		[Category( "General" ), Name( "Fretboard edge @ 12th" ), Description( "Width between the outside strings and edge of fretboard at the 12th fret" )]
		private double m_fFretboardEdgeAt12th;

		[Category( "Strings" ), Name( "Plain string tension" ), Description( "The target tension for the plain strings" )]
		private double m_dPlainSteelStringTensionTarget;
		[Category( "Strings" ), Name( "Wound string tension" ), Description( "The target tension for the wound strings" )]
		private double m_dNickelWoundStringTensionTarget;

		[Category( "General" ), Name( "Fret width" ), Description( "Width of the fretwire" )]
		private double m_dFretwireWidth;
		[Category( "General" ), Name( "Fret crown height" ), Description( "Height of the fretwire crown" )]
		private double m_dFretwireCrownHeight;

		[Category( "General" ), Name( "Zero fret" ), Description( "Use a zero fret?" )]
		private bool m_bZeroFret;
		[Category( "General" ), Name( "Zero fret nut offset" ), Description( "How far back the nut is offset from the zero fret?" ), Visible( "ZeroFretNutOffsetVisible" )]
		private double m_dZeroFretNutOffset;
		private bool ZeroFretNutOffsetVisible() { return m_bZeroFret; }
		[Category( "General" ), Name( "Nut width" ), Description( "Width of the nut" )]
		private double m_dNutWidth;
		[Category( "General" ), Name( "Action @ 12th nom" ), Description( "Expected action at the 12th fret" )]
		public double m_dStringHeightAt12th;
		[Category( "General" ), Name( "Action @ 12th min" ), Description( "Minimum expected action at the 12th fret" )]
		private double m_dMinStringHeightAt12th;
		[Category( "General" ), Name( "Action @ 12th max" ), Description( "Maximum expected action at the 12th fret" )]
		private double m_dMaxStringHeightAt12th;

		public enum MarkerStyle
		{
			None,
			Centred,
			Edge,
			OpposingEdges,
			SquareBlocks
		}
		[Category( "Aesthetics" ), Name( "Fretboard marker style" ), Description( "What fret marker style to use" )]
		private MarkerStyle m_eMarkerStyle;
		private double m_dDotRadius;
		[Category( "Aesthetics" ), Name( "Fretboard dot spacing" ), Description( "Spacing between dots & edge of fretboard" ), Visible( "DotEdgeSpacingVisible" )]
		private double m_dDotEdgeSpacing;
		private bool DotEdgeSpacingVisible() { return m_eMarkerStyle == MarkerStyle.Edge || m_eMarkerStyle == MarkerStyle.OpposingEdges; }
		private double m_dSquareBlockWidth;
		private double m_dSquareBlockSpacing;

		public enum SaddleStyles
		{
			None,
			Hardtail,
			TuneOMatic
		}
		private SaddleStyles m_eSaddleStyle;
		[NoSave]
		private Dictionary<SaddleStyles, netDxf.DxfDocument> m_saddlePrefabs;

		public enum PickupTypes
		{
			None,
			Singlecoil,
			Humbucker,
			Tele,
			P90
		}

		public class PickupSettings
		{
			public PickupSettings()
			{
				StringSpacing = Spacing.Humbucker_492;
			}

			public PickupTypes m_eType = PickupTypes.None;
			[Name( "Type" ), Description( "Pickup Type" )]
			public PickupTypes Type
			{
				get { return m_eType; }
				set
				{ 
					m_eType = value;
					if( m_eType == PickupTypes.Humbucker )
						m_dBobbinWidth = 17.653;
					else if( m_eType == PickupTypes.Singlecoil )
						m_dBobbinWidth = 15.5;
				}
			}

			[Name( "Angle" ), Description( "Angle from vertical (positive = clockwise, negative = anticlockwise)" )]
			public double m_dAngle = 0;
			[Name( "Position" ), Description( "Position as a fraction of scale length" )]
			public double m_dPosition = 0;

			public enum Spacing
			{
				Humbucker_492,
				P90_500,
				Strat_524,		// 52.3875mm
				TeleBridge_550,	// 54.991mm
				TeleNeck_502,	//50.165mm
				Perfect,
				Custom
			}
			public Spacing m_eStringSpacing;

			[Name( "String spacing" ), Description( "Pickup to base polepiece spacing on" )]
			public Spacing StringSpacing
			{
				get { return m_eStringSpacing; }
				set { m_eStringSpacing = value; m_dPerPoleStringSpacing = GetStringSpacingForEnum( m_eStringSpacing ) / 5; }
			}

			private double GetStringSpacingForEnum( Spacing e )
			{
				if( e != Spacing.Perfect && e != Spacing.Custom )
				{
					string strTemp = e.ToString();
					strTemp = strTemp.Substring( strTemp.Length - 3 );
					return Double.Parse( strTemp ) / 10;
				}
				return m_dPerPoleStringSpacing * 5;
			}

			[Name( "String spacing per pole" ), Description( "Per pole string spacing" )]
			public double m_dPerPoleStringSpacing;

			[Name( "Bobbin Width" ), Description( "Width of the bobbin used" )]
			public double m_dBobbinWidth;
		}

		[Category( "Hardware" ), Name( "Bridge Pickup" ), Description( "Bridge pickup" )]
		private PickupSettings m_bridgePickup = new PickupSettings();
		[Category( "Hardware" ), Name( "Middle Pickup" ), Description( "Middle pickup" )]
		private PickupSettings m_middlePickup = new PickupSettings();
		[Category( "Hardware" ), Name( "Neck Pickup" ), Description( "Neck pickup" )]
		private PickupSettings m_neckPickup = new PickupSettings();

		// Runtime things - not for storage
		[NoSave]
		private Tuning m_tuning;
		[NoSave]
		private CustomClass m_calculatedStuff;
		[NoSave]
		private bool m_bResetView = true;
		[NoSave]
		private netDxf.DxfDocument m_currentSaddlePrefab = null;

		[NoSave]
		DataTable m_bassFretChartTable = new DataTable();
		[NoSave]
		DataTable m_trebleFretChartTable = new DataTable();

		public DataTable BassFretDataTable
		{
			get { return m_bassFretChartTable; }
		}
		public DataTable TrebleFretDataTable
		{
			get { return m_trebleFretChartTable; }
		}

		public bool MultiScale { get { return m_fBassScaleLength != m_fTrebleScaleLength; } }

		#region saving and loading
		public void Save( string strFilename )
		{
			using( XmlTextWriter xml = new XmlTextWriter( strFilename, null ) )
			{
				xml.Formatting = Formatting.Indented;
				xml.WriteStartDocument();
				xml.WriteStartElement( "Guitar" );
				xml.WriteAttributeString( "version", "1" );

				Utils.SaveObject( xml, this );

				m_tuning.Save( xml );
				xml.WriteEndDocument();
			}
		}

		public void Load( string strFilename )
		{
			XmlDocument xml = new XmlDocument();
			xml.Load( strFilename );
			if( xml.LastChild.Name == "Guitar" )
			{
				XmlNode guitar = xml.LastChild;
				int version = Utils.getXmlNodeAttribute< Int32 >( guitar, "version" );
				Utils.LoadObject( guitar, this );

				m_tuning.Load( guitar );
			}
		}
		#endregion

		public GuitarProperties( AxeCalc app, CustomClass calculatedStuff )
		{
			// initialize the prefabs
			m_saddlePrefabs = new Dictionary<SaddleStyles, netDxf.DxfDocument>();
			foreach( SaddleStyles style in Enum.GetValues( typeof( SaddleStyles ) ) )
			{
				string strName = Enum.GetName( typeof( SaddleStyles ), style );
				try
				{
					netDxf.DxfDocument dxf = new netDxf.DxfDocument();
					dxf.Load( "Data/Saddles/" + strName + ".dxf" );
					m_saddlePrefabs.Add( style, dxf );
				}
				catch
				{
				}
			}

			m_calculatedStuff = calculatedStuff;
			m_iStringCount = 6;
			m_iFretCount = 22;
			m_iStraightFret = 8;
			ScaleLength = "630";
			TotalBridgeStringSpacing = 51.85; // 52.5;
			TotalNutStringSpacing = 35.05; // 35.7;
			FretboardWidthAtNut = 43;
			FretboardWidthAt12th = 53;
			m_dFretwireWidth = 2.79;
			m_dFretwireCrownHeight = 1.32;
			m_bZeroFret = true;
			m_dZeroFretNutOffset = 8;
			m_dNutWidth = 5;
			m_dStringHeightAt12th = 3;
			m_dMinStringHeightAt12th = 1.5;
			m_dMaxStringHeightAt12th = 4;
			m_dDotRadius = 3;
			m_dDotEdgeSpacing = 6;
			m_eMarkerStyle = MarkerStyle.Centred;
			m_dSquareBlockSpacing = 3;
			m_dSquareBlockWidth = 30;
			SaddleStyle = SaddleStyles.Hardtail;
			m_bridgePickup.Type = PickupTypes.Humbucker;
			m_bridgePickup.m_dPosition = 43 / 630.0;
			m_neckPickup.Type = PickupTypes.Humbucker;
			m_neckPickup.m_dPosition = 148 / 630.0;

/*
			fStratNeckPos = 6.375 / 25.5
			fStratMiddlePos = 3.875 / 25.5
			fStratBridgePos = 1.625 / 25.5
			fStratBridgeBassPos = 1.815 / 25.5
			fStratBridgeTreblePos = 1.435 / 25.5
			fJBNeckPos = 6 / 34.0
			fJBBridgePos = 2.13 / 34.0
			fPRSNeckPos = 148 / 635.0
			fPRSBridgePos = 43 / 635.0
 */ 
			// These are based on what D'Addario's Regular Light strings give on a 25.5" scale guitar
			m_dPlainSteelStringTensionTarget = 16;
			m_dNickelWoundStringTensionTarget = 18.5;

			m_tuning = new Tuning( this, m_calculatedStuff );

			Type t = typeof( GuitarProperties );

			//Add( new CustomProperty( this, m_iTestValue ) );
			AddMarkedUpVariables( this );

			Refresh( app );
		}
		//---------------------------------------------------------------------

		public void Refresh( AxeCalc app )
		{
			m_tuning.SetupProperties();

			// calculate perfect string spacings
			if( m_bridgePickup.m_eStringSpacing == PickupSettings.Spacing.Perfect )
				m_bridgePickup.m_dPerPoleStringSpacing = GetCrossFretboardLine( m_bridgePickup.m_dPosition ).length() / ( m_iStringCount - 1 );
			if( m_middlePickup.m_eStringSpacing == PickupSettings.Spacing.Perfect )
				m_middlePickup.m_dPerPoleStringSpacing = GetCrossFretboardLine( m_middlePickup.m_dPosition ).length() / ( m_iStringCount - 1 );
			if( m_neckPickup.m_eStringSpacing == PickupSettings.Spacing.Perfect )
				m_neckPickup.m_dPerPoleStringSpacing = GetCrossFretboardLine( m_neckPickup.m_dPosition ).length() / ( m_iStringCount - 1 );

			RefreshFretCharts( app );

			// cache the prefab used
			try
			{
				m_currentSaddlePrefab = m_saddlePrefabs[ m_eSaddleStyle ];
			}
			catch
			{
				m_currentSaddlePrefab = null;
			}

			Type t = typeof( GuitarProperties );

			m_calculatedStuff.Remove( "General", "Bridge angle" );
			m_calculatedStuff.Remove( "General", "Nut angle" );
			if( m_fBassScaleLength != m_fTrebleScaleLength )
			{
				m_calculatedStuff.Add( new CustomProperty( "General", "Bridge angle", "The angle of the bridge from the vertical", "BridgeAngle", this, true, true ) );
				m_calculatedStuff.Add( new CustomProperty( "General", "Nut angle", "The angle of the nut from the vertical", "NutAngle", this, true, true ) );
			}

		/*	Remove( "Aesthetics", "Fretboard block width" );
			Remove( "Aesthetics", "Fretboard block spacing" );
			if( m_eMarkerStyle == MarkerStyle.SquareBlocks )
			{
				Add( new CustomProperty( "Aesthetics", "Fretboard block width", "Width of the square fretboard blocks", "SquareBlockWidth", this, false, true ) );
				Add( new CustomProperty( "Aesthetics", "Fretboard block spacing", "Spacing between blocks & edge of fretboard", "SquareBlockEdgeSpacing", this, false, true ) );
			}*/
		}
		//---------------------------------------------------------------------

		public void RefreshFretCharts( AxeCalc app )
		{
			m_bassFretChartTable.Clear();
			m_bassFretChartTable.Columns.Clear();
			m_bassFretChartTable.Rows.Clear();
			m_trebleFretChartTable.Clear();
			m_trebleFretChartTable.Columns.Clear();
			m_trebleFretChartTable.Rows.Clear();

			Line2[] fretLines = new Line2[ m_iFretCount + 1 ];
			for( int i = 0; i <= m_iFretCount; ++i )
				fretLines[ i ] = GetFretLine( i );

			if( !MultiScale )
			{
				m_bassFretChartTable.Columns.Add( "Fret" );
				m_bassFretChartTable.Columns.Add( "From nut" );
				m_bassFretChartTable.Columns.Add( "Fret to fret" );

				for( int i = 1; i <= m_iFretCount; ++i )
				{
					object[] values = new object[ 3 ];
					values[ 0 ] = i;
					values[ 1 ] = ( fretLines[ 0 ].v1.x - fretLines[ i ].v1.x ).ToString( "F3" );
					values[ 2 ] = ( fretLines[ i - 1 ].v1.x - fretLines[ i ].v1.x ).ToString( "F3" );
					if( i == 1 )
						values[ 2 ] += " (nut - 1)";
					else
						values[ 2 ] += String.Format( " ({0} - {1})", i-1, i );
					m_bassFretChartTable.Rows.Add( values );
				}
			}
			else
			{
				m_bassFretChartTable.Columns.Add( "Fret" );
				m_bassFretChartTable.Columns.Add( "From reference" );
				m_bassFretChartTable.Columns.Add( "Fret to fret" );
				m_trebleFretChartTable.Columns.Add( "Fret" );
				m_trebleFretChartTable.Columns.Add( "From reference" );
				m_trebleFretChartTable.Columns.Add( "Fret to fret" );

				// extend all the lines out to the edges of the blank
				Line2 blankTopLine = new Line2( 0, app.FretboardBlankWidth / 2, 1000, app.FretboardBlankWidth / 2 );
				Line2 blankBottomLine = new Line2( 0, -app.FretboardBlankWidth / 2, 1000, -app.FretboardBlankWidth / 2 );
				for( int i = 0; i <= m_iFretCount; ++i )
				{
					fretLines[ i ].extendToInfinite( blankTopLine );
					fretLines[ i ].extendToInfinite( blankBottomLine );
				}

				double dReferenceX = Math.Max( fretLines[ 0 ].v1.x, fretLines[ 0 ].v2.x );
				// bass side
				for( int i = 0; i <= m_iFretCount; ++i )
				{
					object[] values = new object[ 3 ];
					values[ 1 ] = ( dReferenceX - fretLines[ i ].v1.x ).ToString( "F3" );

					if( i == 0 )
					{
						values[ 0 ] = "nut";
						values[ 2 ] = ( dReferenceX - fretLines[ i ].v1.x ).ToString( "F3" ) + " (ref - nut)";
					}
					else
					{
						values[ 0 ] = i;
						values[ 2 ] = ( fretLines[ i - 1 ].v1.x - fretLines[ i ].v1.x ).ToString( "F3" );
						if( i == 1 )
							values[ 2 ] += " (nut - 1)";
						else
							values[ 2 ] += String.Format( " ({0} - {1})", i - 1, i );
					}
					m_bassFretChartTable.Rows.Add( values );
				}

				// treble side
				for( int i = 0; i <= m_iFretCount; ++i )
				{
					object[] values = new object[ 3 ];
					values[ 1 ] = ( dReferenceX - fretLines[ i ].v2.x ).ToString( "F3" );

					if( i == 0 )
					{
						values[ 0 ] = "nut";
						values[ 2 ] = ( dReferenceX - fretLines[ i ].v2.x ).ToString( "F3" ) + " (ref - nut)";
					}
					else
					{
						values[ 0 ] = i;
						values[ 2 ] = ( fretLines[ i - 1 ].v2.x - fretLines[ i ].v2.x ).ToString( "F3" );
						if( i == 1 )
							values[ 2 ] += " (nut - 1)";
						else
							values[ 2 ] += String.Format( " ({0} - {1})", i - 1, i );
					}
					m_trebleFretChartTable.Rows.Add( values );
				}
			}
		}
		//---------------------------------------------------------------------

		[Category( "General" ), Name( "String count" ), Description( "Number of Strings" )]
		public int StringCount
		{
			get { return m_iStringCount; }
			set { m_iStringCount = Math.Max( 2, value ); m_tuning.SetStringCount( m_iStringCount ); }
		}
		//---------------------------------------------------------------------

		// Scale is a compound property, changing based on some others
		[Category( "Scale" ), Name( "Scale length" ), Description( "Distance from nut to bridge" )]
		public string ScaleLength
		{
			get
			{
				if( m_fBassScaleLength == m_fTrebleScaleLength )
					return m_fBassScaleLength.ToString();
				else
					return "Multiscale";
			}
			set
			{
				m_fBassScaleLength = Math.Max( 1, double.Parse( value ) );
				m_fTrebleScaleLength = m_fBassScaleLength;
				m_bResetView = true;
			}
		}
		//---------------------------------------------------------------------

		[Category( "String Spacing" ), Name( "Total bridge string spacing" ), Description( "Total string spacing at the bridge, measured from highest to lowest" )]
		public double TotalBridgeStringSpacing
		{
			get { return m_fPerStringSpacingAtBridge * ( m_iStringCount - 1 ); }
			set
			{ 
				m_fPerStringSpacingAtBridge = Math.Max( 1, value );
				if( m_iStringCount > 1 )
					m_fPerStringSpacingAtBridge = m_fPerStringSpacingAtBridge / ( m_iStringCount - 1 );
			}
		}
		//---------------------------------------------------------------------

		[Category( "String Spacing" ), Name( "Total nut string spacing" ), Description( "Total string spacing at the nut, measured from highest to lowest" )]
		public double TotalNutStringSpacing
		{
			get { return m_fPerStringSpacingAtNut * ( m_iStringCount - 1 ); }
			set
			{
				m_fPerStringSpacingAtNut = Math.Max( 1, value );
				if( m_iStringCount > 1 )
					m_fPerStringSpacingAtNut = m_fPerStringSpacingAtNut / ( m_iStringCount - 1 );
			}
		}
		//---------------------------------------------------------------------

		[Category( "General" ), Name( "Fretboard width @ Nut/Zero fret" ), Description( "Width of the fretboard at the nut (or zero fret, if it is used)" )]
		public double FretboardWidthAtNut
		{
			get { return TotalNutStringSpacing + m_fFretboardEdgeAtNut * 2; }
			set	{ m_fFretboardEdgeAtNut = Math.Max( ( value - TotalNutStringSpacing ) / 2, 0 ); }
		}
		//---------------------------------------------------------------------
		[Category( "General" ), Name( "Fretboard width @ 12th" ), Description( "Width of the fretboard at the 12th fret" )]
		public double FretboardWidthAt12th
		{
			get { return ( TotalNutStringSpacing + TotalBridgeStringSpacing ) / 2 + m_fFretboardEdgeAt12th * 2; }
			set { m_fFretboardEdgeAt12th = Math.Max( ( value - ( TotalNutStringSpacing + TotalBridgeStringSpacing ) / 2 ) / 2, 0 ); }
		}
		//---------------------------------------------------------------------

		[Category( "Aesthetics" ), Name( "Fretboard dot size" ), Description( "Size of the fretboard dot markers" )]
		public double DotSize
		{
			get { return m_dDotRadius * 2; }
			set { m_dDotRadius = value / 2; }
		}
		public double SquareBlockEdgeSpacing
		{
			get { return m_dSquareBlockSpacing; }
			set { m_dSquareBlockSpacing = value; }
		}
		public double SquareBlockWidth
		{
			get { return m_dSquareBlockWidth; }
			set { m_dSquareBlockWidth = value; }
		}
		//---------------------------------------------------------------------

		[Category( "Hardware" ), Name( "Saddle style" ), Description( "What type of saddles to draw" )]
		public SaddleStyles SaddleStyle
		{
			get { return m_eSaddleStyle; }
			set 
			{ 
				m_eSaddleStyle = value;
			}
		}
		//---------------------------------------------------------------------

		[Category( "General" ), Name( "Bridge angle" ), Description( "The angle of the bridge from the vertical" ), Visible( "MultiScale" ) ]
		public double BridgeAngle
		{
			get { return 90 + Math.Atan2( bridgeStringLine.v2.y - bridgeStringLine.v1.y, bridgeStringLine.v2.x - bridgeStringLine.v1.x ) * 180.0 / Math.PI; }
		}

		[Category( "General" ), Name( "Nut angle" ), Description( "The angle of the nut from the vertical" ), Visible( "MultiScale" ) ]
		public double NutAngle
		{
			get { return 90 - Math.Atan2( nutStringLine.v1.y - nutStringLine.v2.y, nutStringLine.v1.x - nutStringLine.v2.x ) * 180.0 / Math.PI; }
		}
		//---------------------------------------------------------------------

		double calculateFret( double scaleLength, double fretNumber )
		{
			double result = ( scaleLength / Math.Pow( 2.0, ( fretNumber / 12.0 ) ) );
			//print( "calculateFret( %.3f, %i ) = %.3f  fretFactor = %.3f, powresult = %.3f" % ( scaleLength, fretNumber, result, fretFactor, powresult ) );
			return result;
		}

		double interpolate( double factor, double a, double b )
		{
		    return a + ( b - a ) * factor;
		}
		Vec2 interpolate( double factor, Vec2 a, Vec2 b )
		{
			return a + ( b - a ) * factor;
		}
		//---------------------------------------------------------------------

		private double scaleTrebleOffset { get { return calculateFret( m_fBassScaleLength, m_iStraightFret ) - calculateFret( m_fTrebleScaleLength, m_iStraightFret ); } }
		private Line2 bridgeStringLine { get { return new Line2( new Vec2( 0, TotalBridgeStringSpacing / 2 ), new Vec2( scaleTrebleOffset, -TotalBridgeStringSpacing / 2 ) ); } }
		private Line2 nutStringLine { get { return new Line2( new Vec2( bridgeStringLine.v1.x + m_fBassScaleLength, TotalNutStringSpacing / 2 ), new Vec2( bridgeStringLine.v2.x + m_fTrebleScaleLength, -TotalNutStringSpacing / 2 ) ); } }
		private Line2 bassStringLine { get { return new Line2( bridgeStringLine.v1, nutStringLine.v1 ); } }
		private Line2 trebleStringLine { get { return new Line2( bridgeStringLine.v2, nutStringLine.v2 ); } }
		//---------------------------------------------------------------------

		private Line2 GetCrossFretboardLine( double factor )
		{
			return new Line2( interpolate( factor, bridgeStringLine.v1, nutStringLine.v1 ), interpolate( factor, bridgeStringLine.v2, nutStringLine.v2 ) );
		}

		private Line2 GetFretLine( int iFretNumber )
		{
			double factor = calculateFret( 1, iFretNumber );
			return GetCrossFretboardLine( factor );
		}

		public Line2 GetStringLine( int iStringNumber )
		{
			double factor = iStringNumber / ( double )( m_iStringCount - 1 );
			Vec2 bridgeSpot = interpolate( factor, bridgeStringLine.v1, bridgeStringLine.v2 );
			Vec2 nutSpot = interpolate( factor, nutStringLine.v1, nutStringLine.v2 );
			return new Line2( bridgeSpot, nutSpot );
		}
		//---------------------------------------------------------------------

		public StringInfo GetIdealString( int iStringNumber, ref double dResultantTension )
		{
			double L = GetStringLine( iStringNumber ).length() / 25.4;
			double F = DataTables.GetNoteFrequency( m_tuning.GetStringTuning( m_iStringCount - iStringNumber - 1 ) );
			return DataTables.SelectBestString( L, F, m_dPlainSteelStringTensionTarget, m_dNickelWoundStringTensionTarget, ref dResultantTension );
		}
		public StringInfo GetIdealPlainString( int iStringNumber, ref double dResultantTension )
		{
			double L = GetStringLine( iStringNumber ).length() / 25.4;
			double F = DataTables.GetNoteFrequency( m_tuning.GetStringTuning( m_iStringCount - iStringNumber - 1 ) );
			return DataTables.SelectBestPlainString( L, F, m_dPlainSteelStringTensionTarget, ref dResultantTension );
		}
		public StringInfo GetIdealWoundString( int iStringNumber, ref double dResultantTension )
		{
			double L = GetStringLine( iStringNumber ).length() / 25.4;
			double F = DataTables.GetNoteFrequency( m_tuning.GetStringTuning( m_iStringCount - iStringNumber - 1 ) );
			return DataTables.SelectBestWoundString( L, F, m_dNickelWoundStringTensionTarget, ref dResultantTension );
		}

		public double GetRequiredCompensation( int iStringNumber, double dAction )
		{
			double dResultantTension = 0;
			StringInfo s = GetIdealString( iStringNumber, ref dResultantTension );

			double H = dAction / 25.4;
			double E = s.m_dYoungsModulus;
			double A = s.m_dCoreCrossSection;
			double L = GetStringLine( iStringNumber ).length() / 25.4;
			double T = dResultantTension;

			double C = H * H * E * A / ( L * T );
			return C * 25.4;
		}
		//---------------------------------------------------------------------

		public RectangleF RoughExtents
		{
			get
			{
				float fMaxScale = ( float )Math.Max( m_fBassScaleLength, m_fTrebleScaleLength );
				return new RectangleF( -60.0f, ( float )-TotalBridgeStringSpacing, fMaxScale + 120, ( float )TotalBridgeStringSpacing * 2 );
			}
		}

		public bool NeedsViewReset
		{
			get { return m_bResetView; }
			set { m_bResetView = value; }
		}
		//---------------------------------------------------------------------
			
		public void Draw( Drawer d )
		{
			d.AddLayer( "Scale", Color.FromArgb( 255, 255, 255, 255 ) );
			d.AddLayer( "FretCentres", Color.FromArgb( 255, 0, 192, 192 ) );
			d.AddLayer( "Fretwire", Color.FromArgb( 255, 192, 192, 192 ) );
			d.AddLayer( "StringCenterLines", Color.FromArgb( 255, 255, 255, 255 ) );
			d.AddLayer( "Strings", Color.FromArgb( 255, 255, 255, 255 ) );
			d.AddLayer( "FretBoard", Color.FromArgb( 255, 255, 255, 255 ) );
			d.AddLayer( "Saddles", Color.FromArgb( 255, 255, 255, 0 ) );
			d.AddLayer( "SaddleScrews", Color.FromArgb( 255, 255, 192, 0 ) );

			// find out what strings we need
			double[] stringWidths = new double[ m_iStringCount ];
			double[] stringCompensations = new double[ m_iStringCount ];
			for( int i = 0; i < m_iStringCount; ++i )
			{
				double dResultantTension = 0;
				StringInfo s = GetIdealString( i, ref dResultantTension );
				stringWidths[ i ] = s.m_dDiameter * 25.4;
				stringCompensations[ i ] = GetRequiredCompensation( i, m_dStringHeightAt12th );
			}

			RectangleF extents = RoughExtents; 
			double fMaxScale = Math.Max( m_fBassScaleLength, m_fTrebleScaleLength );

			d.SetLimits( extents.Left, extents.Top, extents.Right, extents.Bottom );

			// draw centreline & lines showing the scale points for the two scales
			d.DrawLine( "Scale", -30, 0, fMaxScale + 30, 0 );
		//	d.DrawLine( "Scale", 0, TotalBridgeStringSpacing / 2 + 30, 0, -TotalBridgeStringSpacing / 2 - 30 );
		//	if( m_fBassScaleLength != m_fTrebleScaleLength )
		//		d.DrawLine( "Scale", scaleTrebleOffset, TotalBridgeStringSpacing / 2 + 30, scaleTrebleOffset, -TotalBridgeStringSpacing / 2 - 30 );

			// find fret locations
			Line2[] fretLines = new Line2[ m_iFretCount + 1 ];
			for( int i = 0; i < m_iFretCount + 1; ++i )
			{
				fretLines[ i ] = GetFretLine( i );
			}
/*
			# do constant-spacing based string spacing, to take each string's thickness into account
			#spacingLeft = TotalNutStringSpacing + stringWidths[ 0 ] / 2 + stringWidths[ stringCount - 1 ] / 2
			#for i in range( stringCount ):
			#    spacingLeft -= stringWidths[ i ]
			#spacingBetweenStrings = spacingLeft / ( stringCount - 1.0 )

			#positionUpto = 0
			#stringPositions = list()
			#for i in range( stringCount ):
			#    stringPositions.append( positionUpto / TotalNutStringSpacing )
			#    print( "string %i nut position %.3f" % (i, positionUpto ))
			#    if( i != stringCount - 1 ):
			#        positionUpto += stringWidths[ i ] / 2
			#        positionUpto += spacingBetweenStrings
			#        positionUpto += stringWidths[ i + 1 ] / 2
*/

			// properly figure out the fretboard outlines, taking into account zero fret, etc etc etc...
			Line2 zeroFretLine = new Line2( fretLines[ 0 ] );
			Line2 nutLineLeft;
			if( !m_bZeroFret )
				nutLineLeft = zeroFretLine;
			else
				nutLineLeft = zeroFretLine.offset( m_dZeroFretNutOffset, Vec2.right );
			Line2 nutLineRight = nutLineLeft.offset( m_dNutWidth, Vec2.right );
			Line2 fbEndLine = new Line2( fretLines[ m_iFretCount ] );
			Line2 fret12Line = fretLines[ 12 ];

			Vec2 bassLine0 = new Vec2( 0, 0 );
			Vec2 bassLine12th = new Vec2( 0, 0 );
			zeroFretLine.intersectInfinite( new Line2( 0, FretboardWidthAtNut / 2, 100, FretboardWidthAtNut / 2 ), ref bassLine0 );
			fretLines[ 12 ].intersectInfinite( new Line2( 0, FretboardWidthAt12th / 2, 100, FretboardWidthAt12th / 2 ), ref bassLine12th );

			Line2 fbBassLine = new Line2( bassLine0, bassLine12th ); //new Line2( //stringTopEdges[ 0 ].offset( edgeOfFretboardSpacing, Vec2.up );
			Line2 fbTrebleLine = fbBassLine.mirror( new Line2( 0, 0, 1, 0 ) ); //stringBottomEdges[ m_iStringCount - 1 ].offset( edgeOfFretboardSpacing, Vec2.down );
			fbEndLine.v1.x -= 8;
			fbEndLine.v2.x -= 8;
			nutLineLeft.trimToInfinite( fbBassLine, Vec2.down );
			nutLineLeft.trimToInfinite( fbTrebleLine, Vec2.up );
			nutLineRight.trimToInfinite( fbBassLine, Vec2.down );
			nutLineRight.trimToInfinite( fbTrebleLine, Vec2.up );
			fbEndLine.trimToInfinite( fbBassLine, Vec2.down );
			fbEndLine.trimToInfinite( fbTrebleLine, Vec2.up );
			fbBassLine.trimToInfinite( fbEndLine, Vec2.right );
			fbBassLine.trimToInfinite( nutLineRight, Vec2.left );
			fbTrebleLine.trimToInfinite( fbEndLine, Vec2.right );
			fbTrebleLine.trimToInfinite( nutLineRight, Vec2.left );

			// draw fretboard
			d.DrawLine( "FretBoard", nutLineLeft );
			d.DrawLine( "FretBoard", nutLineRight );
			d.DrawLine( "FretBoard", fbEndLine );
			d.DrawLine( "FretBoard", fbBassLine );
			d.DrawLine( "FretBoard", fbEndLine );
			d.DrawLine( "FretBoard", fbTrebleLine );

			// draw frets
			for( int i = 0; i <= m_iFretCount; ++i )
			{
				if( i == 0 && !m_bZeroFret )
					continue;

				Line2 fret = fretLines[ i ];
				fret.extendToInfinite( fbBassLine );
				fret.extendToInfinite( fbTrebleLine );
				d.DrawLine( "FretCentres", fret );

				// fretwire
				Line2 fretRight = fret.offset( m_dFretwireWidth / 2, Vec2.right );
				fretRight.trimToInfinite( fbBassLine, Vec2.down );
				fretRight.trimToInfinite( fbTrebleLine, Vec2.up );
				d.DrawLine( "Fretwire", fretRight );
				Line2 fretLeft = fret.offset( m_dFretwireWidth / 2, Vec2.left );
				fretLeft.trimToInfinite( fbBassLine, Vec2.down );
				fretLeft.trimToInfinite( fbTrebleLine, Vec2.up );
				d.DrawLine( "Fretwire", fretLeft );

				// put a little ellipse at the end to make it purty
				//d.DrawEllipseArc( "Fretwire", fretLeft.v2, fretRight.v2, 0.5, fretLeft.v2, fretRight.v2 );
			}

			// draw strings
			Line2[] stringLines = new Line2[ m_iStringCount ];
			Vec2[] saddleSpots = new Vec2[ m_iStringCount ];
			double dMaxSaddleX = -10000000.0f;
			double dMinSaddleX = 10000000.0f;

			for( int i = 0; i < m_iStringCount; ++i )
			{
				stringLines[ i ] = GetStringLine( i );
				stringLines[ i ].trimToInfinite( nutLineRight, Vec2.left );

				// calculate the estimated string intonation point
				saddleSpots[ i ] = stringLines[ i ].v1 + stringLines[ i ].direction * -stringCompensations[ i ];
				saddleSpots[ i ].y = stringLines[ i ].v1.y;
				dMaxSaddleX = Math.Max( dMaxSaddleX, saddleSpots[ i ].x );
				dMinSaddleX = Math.Min( dMinSaddleX, saddleSpots[ i ].x );

				// extend the string out to the saddle
				Line2 saddleLine = new Line2( saddleSpots[ i ].x, saddleSpots[ i ].y + 5.0f, saddleSpots[ i ].x, saddleSpots[ i ].y - 5.0f );
				stringLines[ i ].extendToInfinite( saddleLine );
	
				Line2 topEdge = stringLines[ i ].offset( stringWidths[ i ] / 2.0, Vec2.up );
				Line2 bottomEdge = stringLines[ i ].offset( stringWidths[ i ] / 2.0, Vec2.down );
				topEdge.trimToInfinite( nutLineRight, Vec2.left );
				bottomEdge.trimToInfinite( nutLineRight, Vec2.left );
				d.DrawLine( "StringCenterLines", stringLines[ i ] );
				d.DrawLine( "Strings", topEdge );
				d.DrawLine( "Strings", bottomEdge );

				//d.DrawLine( "Saddles", saddleLine );
			}

			// figure out where the saddle screw should go, based on its max adjustment
			//double dMaxSaddleAdjustment = 8.5;
			//double dAdjustmentRangeNeeded = dMaxSaddleX - dMinSaddleX;
			// DrawSaddleScrew will draw at the centre of the adjustment, so figure out an X that keeps everything close to the middle range


			// draw saddles & saddle screws
			for( int i = 0; i < m_iStringCount; ++i )
			{
				DrawSaddle( d, saddleSpots[ i ] );

				// draw min & max compensation points
				Vec2 minSaddleSpot = GetStringLine( i ).v1 + GetStringLine( i ).direction * -GetRequiredCompensation( i, m_dMinStringHeightAt12th );
				minSaddleSpot.y = saddleSpots[ i ].y;
				Vec2 maxSaddleSpot = GetStringLine( i ).v1 + GetStringLine( i ).direction * -GetRequiredCompensation( i, m_dMaxStringHeightAt12th );
				maxSaddleSpot.y = saddleSpots[ i ].y;

				//DrawSaddle( d, minSaddleSpot );
				//DrawSaddle( d, maxSaddleSpot );
				

				if( m_fBassScaleLength == m_fTrebleScaleLength )
				{
					Vec2 screwSpot = new Vec2( saddleSpots[ i ].x, saddleSpots[ i ].y );
					screwSpot.x = ( dMinSaddleX + dMaxSaddleX ) * 0.5;
					DrawSaddleScrew( d, screwSpot );
				}
				else
				{
					DrawSaddleScrew( d, saddleSpots[ i ] );
				}
			}

			d.AddLayer( "FretMarkers", Color.FromArgb( 255, 255, 255, 255 ) );
			//d.DrawCircle( "FretMarkers", new Vec2( 0, 0 ), 4 );
			DrawFretMarker( d, fretLines, 3 );
			DrawFretMarker( d, fretLines, 5 );
			DrawFretMarker( d, fretLines, 7 );
			DrawFretMarker( d, fretLines, 9 );
			DrawFretMarker( d, fretLines, 12, true );
			DrawFretMarker( d, fretLines, 15 );
			DrawFretMarker( d, fretLines, 17 );
			DrawFretMarker( d, fretLines, 19 );
			DrawFretMarker( d, fretLines, 21 );
			DrawFretMarker( d, fretLines, 24, true );

			// draw pickup positions
			DrawPickup( d, m_bridgePickup );
			DrawPickup( d, m_middlePickup );
			DrawPickup( d, m_neckPickup );

/*			# draw some comparison pickup positions
			fStratNeckPos = 6.375 / 25.5
			fStratMiddlePos = 3.875 / 25.5
			fStratBridgePos = 1.625 / 25.5
			fStratBridgeBassPos = 1.815 / 25.5
			fStratBridgeTreblePos = 1.435 / 25.5
			fJBNeckPos = 6 / 34.0
			fJBBridgePos = 2.13 / 34.0
			fPRSNeckPos = 148 / 635.0
			fPRSBridgePos = 43 / 635.0

			fNewNeckPos1 = interpolate( 1/3.0, fJBNeckPos, fPRSNeckPos )
			fNewNeckPos2 = interpolate( 2/3.0, fJBNeckPos, fPRSNeckPos )

			writeScalePos( f, fStratNeckPos, "StratPickups", bridgeStringLine, nutStringLine )
			writeScalePos( f, fStratMiddlePos, "StratPickups", bridgeStringLine, nutStringLine )
			writeAngledScalePos( f, fStratBridgeBassPos, fStratBridgeTreblePos, "StratPickups", bridgeStringLine, nutStringLine )
			writeScalePos( f, fJBNeckPos, "JBPickups", bridgeStringLine, nutStringLine )
			writeScalePos( f, fJBBridgePos, "JBPickups", bridgeStringLine, nutStringLine )
			writeScalePos( f, fPRSNeckPos, "PRSPickups", bridgeStringLine, nutStringLine )
			writeScalePos( f, fPRSBridgePos, "PRSPickups", bridgeStringLine, nutStringLine )
			writeScalePos( f, fNewNeckPos1, "TristanPickups", bridgeStringLine, nutStringLine )
			writeScalePos( f, fNewNeckPos2, "TristanPickups", bridgeStringLine, nutStringLine )
			*/
		}
		void DrawFretMarker( Drawer d, Line2[] fretLines, int iFretNumber, bool bDouble = false )
		{
			if( fretLines.Length < iFretNumber )
				return;

			Line2 fret1 = fretLines[ iFretNumber - 1 ];
			Line2 fret2 = fretLines[ iFretNumber ];

			Line2 newLine = new Line2( ( fret1.v1 + fret2.v1 ) * 0.5, ( fret1.v2 + fret2.v2 ) * 0.5 );
			if( m_eMarkerStyle == MarkerStyle.Centred )
			{
				if( !bDouble )
				{
					Vec2 vCentre = newLine.origin + newLine.direction * ( newLine.distance / 2 );
					d.DrawCircle( "FretMarkers", vCentre, m_dDotRadius );
				}
				else
				{
					Vec2 vCentre1 = newLine.origin + newLine.direction * ( newLine.distance * 1 / 4 );
					Vec2 vCentre2 = newLine.origin + newLine.direction * ( newLine.distance * 3 / 4 );
					d.DrawCircle( "FretMarkers", vCentre1, m_dDotRadius );
					d.DrawCircle( "FretMarkers", vCentre2, m_dDotRadius );

				}
			}
			else if( m_eMarkerStyle == MarkerStyle.Edge || m_eMarkerStyle == MarkerStyle.OpposingEdges )
			{
				if( m_eMarkerStyle == MarkerStyle.OpposingEdges && iFretNumber > 12 )
					newLine = new Line2( newLine.v2, newLine.v1 );

				d.DrawCircle( "FretMarkers", newLine.origin + newLine.direction * ( m_dDotEdgeSpacing + m_dDotRadius ), m_dDotRadius );
				if( bDouble )
				{
					d.DrawCircle( "FretMarkers", newLine.origin + newLine.direction * ( m_dDotEdgeSpacing*2 + m_dDotRadius*3 ), m_dDotRadius );
				}
			}
			else if( m_eMarkerStyle == MarkerStyle.SquareBlocks )
			{

			}
		}

		void DrawSaddle( Drawer d, Vec2 vIntonationPoint )
		{
			if( m_currentSaddlePrefab != null )
				d.DrawPrefab( vIntonationPoint, m_currentSaddlePrefab );
		}

		void DrawSaddleScrew( Drawer d, Vec2 vIntonationPoint )
		{
			if( m_eSaddleStyle == SaddleStyles.Hardtail )
			{
				d.DrawLine( "SaddleScrews", new Vec2( vIntonationPoint.x - 27, vIntonationPoint.y + 1.45 ), new Vec2( vIntonationPoint.x - 11.4, vIntonationPoint.y + 1.45 ) );
				d.DrawLine( "SaddleScrews", new Vec2( vIntonationPoint.x - 27, vIntonationPoint.y - 1.45 ), new Vec2( vIntonationPoint.x - 11.4, vIntonationPoint.y - 1.45 ) );
				d.DrawLine( "SaddleScrews", new Vec2( vIntonationPoint.x - 11.4, vIntonationPoint.y + 1.45 ), new Vec2( vIntonationPoint.x - 11.4, vIntonationPoint.y - 1.45 ) );
				d.DrawLine( "SaddleScrews", new Vec2( vIntonationPoint.x - 27, vIntonationPoint.y + 2.65 ), new Vec2( vIntonationPoint.x - 27, vIntonationPoint.y - 2.65 ) );
				d.DrawLine( "SaddleScrews", new Vec2( vIntonationPoint.x - 27, vIntonationPoint.y + 2.65 ), new Vec2( vIntonationPoint.x - 27, vIntonationPoint.y - 2.65 ) );
				d.DrawLine( "SaddleScrews", new Vec2( vIntonationPoint.x - 28.73, vIntonationPoint.y + 2.65 ), new Vec2( vIntonationPoint.x - 28.73, vIntonationPoint.y - 2.65 ) );
				d.DrawLine( "SaddleScrews", new Vec2( vIntonationPoint.x - 27, vIntonationPoint.y + 2.65 ), new Vec2( vIntonationPoint.x - 28.73, vIntonationPoint.y + 2.65 ) );
				d.DrawLine( "SaddleScrews", new Vec2( vIntonationPoint.x - 27, vIntonationPoint.y - 2.65 ), new Vec2( vIntonationPoint.x - 28.73, vIntonationPoint.y - 2.65 ) );
			}
		}

		void DrawPickup( Drawer d, PickupSettings p )
		{
			if( p.m_eType == PickupTypes.None )
				return;

			d.AddLayer( "PickupCentres", Color.FromArgb( 255, 128, 128 ) );
			d.AddLayer( "Pickups", Color.FromArgb( 255, 255, 255 ) );

			// centreline
			Line2 centreLine = GetCrossFretboardLine( p.m_dPosition );
			d.DrawLine( "PickupCentres", centreLine );

			if( p.m_eType == PickupTypes.Singlecoil )
			{
				DrawPickupBobbin( d, centreLine, p.m_dPerPoleStringSpacing, p.m_dBobbinWidth );
			}
			else if( p.m_eType == PickupTypes.Humbucker )
			{
				Line2 leftLine = centreLine.offset( p.m_dBobbinWidth / 2, Vec2.left );
				Line2 rightLine = centreLine.offset( p.m_dBobbinWidth / 2, Vec2.right );
				leftLine.trimToInfinite( bassStringLine, Vec2.down );
				leftLine.trimToInfinite( trebleStringLine, Vec2.up );
				rightLine.trimToInfinite( bassStringLine, Vec2.down );
				rightLine.trimToInfinite( trebleStringLine, Vec2.up );
				DrawPickupBobbin( d, leftLine, p.m_dPerPoleStringSpacing, p.m_dBobbinWidth );
				DrawPickupBobbin( d, rightLine, p.m_dPerPoleStringSpacing, p.m_dBobbinWidth );
			}
		}

		void DrawPickupBobbin( Drawer d, Line2 centreLine, double dStringSpacing, double dWidth )
		{
			// polepiece positions
			Vec2 vStartSpot = ( centreLine.v1 + centreLine.v2 ) * 0.5 - centreLine.direction * ( m_iStringCount - 1 ) * 0.5 * dStringSpacing;
			Vec2 vEndSpot = vStartSpot + centreLine.direction * dStringSpacing * ( m_iStringCount - 1 );
			Line2 trueCentreLine = new Line2( vStartSpot, vEndSpot );
			for( int i = 0; i < m_iStringCount; ++i )
			{
				Vec2 vSpot = vStartSpot + centreLine.direction * dStringSpacing * i;
				d.DrawCircle( "Pickups", vSpot, 2.5 );
			}


			// outline
			Line2 left = trueCentreLine.offset( dWidth / 2, Vec2.left );
			Line2 right = trueCentreLine.offset( dWidth / 2, Vec2.right );
			//Line2 perpendicular = trueCentreLine.perpendicular( Vec2.right );
		//	perpendicular.v1 = left.v1;
			d.DrawLine( "Pickups", left );
			d.DrawLine( "Pickups", right );

			double dAngle = 180 * left.direction.angleBetween( Vec2.down ) / Math.PI;
			d.DrawArc( "Pickups", trueCentreLine.v1, dWidth / 2, dAngle, 180 + dAngle );
			d.DrawArc( "Pickups", trueCentreLine.v2, dWidth / 2, 180 + dAngle, dAngle );
		}
	}
}
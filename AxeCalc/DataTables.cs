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

namespace AxeCalc
{
	public class StringInfo
	{
		public string m_strName;
		public double m_dUnitWeight;
		public double m_dDiameter;
		public double m_dCoreCrossSection;
		public double m_dYoungsModulus;

		public StringInfo( string strName, double dUnitWeight, double dDiameter ) 
		{ 
			m_strName = strName; 
			m_dUnitWeight = dUnitWeight; 
			m_dDiameter = dDiameter;
			m_dYoungsModulus = 30000000;

			if( strName.Substring( 0, 2 ) != "PL" )
			{
				double dHexSizeApprox = ( 0.016 / 0.042 ) * dDiameter;
				m_dCoreCrossSection = dHexSizeApprox * dHexSizeApprox * Math.Cos( 30 * Math.PI / 180 );
			}
			else
				m_dCoreCrossSection = Math.PI * m_dDiameter * m_dDiameter / 4;
		}
	}

	class DataTables
	{
		public static Dictionary< string, double > s_NoteFrequencies;
		public static List<StringInfo> s_PlainSteelStrings;
		public static List<StringInfo> s_NickelWoundStrings;
		public static List<StringInfo> s_BassStrings;

		static DataTables()
		{
			// 9 octaves worth of note frequencies! Should really just calculate these but meh... regexps for the win...
			s_NoteFrequencies = new Dictionary<string, double>();
			s_NoteFrequencies[ "C0" ] = 16.351; s_NoteFrequencies[ "C#0" ] = 17.324; s_NoteFrequencies[ "Db0" ] = 17.324; s_NoteFrequencies[ "D0" ] = 18.354; s_NoteFrequencies[ "D#0" ] = 19.445; s_NoteFrequencies[ "Eb0" ] = 19.445; s_NoteFrequencies[ "E0" ] = 20.601; s_NoteFrequencies[ "F0" ] = 21.827; s_NoteFrequencies[ "F#0" ] = 23.124; s_NoteFrequencies[ "Gb0" ] = 23.124; s_NoteFrequencies[ "G0" ] = 24.499; s_NoteFrequencies[ "G#0" ] = 25.956; s_NoteFrequencies[ "Ab0" ] = 25.956; s_NoteFrequencies[ "A0" ] = 27.5; s_NoteFrequencies[ "A#0" ] = 29.135; s_NoteFrequencies[ "Bb0" ] = 29.135; s_NoteFrequencies[ "B0" ] = 30.868;
			s_NoteFrequencies[ "C1" ] = 32.703; s_NoteFrequencies[ "C#1" ] = 34.648; s_NoteFrequencies[ "Db1" ] = 34.648; s_NoteFrequencies[ "D1" ] = 36.708; s_NoteFrequencies[ "D#1" ] = 38.891; s_NoteFrequencies[ "Eb1" ] = 38.891; s_NoteFrequencies[ "E1" ] = 41.203; s_NoteFrequencies[ "F1" ] = 43.654; s_NoteFrequencies[ "F#1" ] = 46.249; s_NoteFrequencies[ "Gb1" ] = 46.249; s_NoteFrequencies[ "G1" ] = 48.999; s_NoteFrequencies[ "G#1" ] = 51.913; s_NoteFrequencies[ "Ab1" ] = 51.913; s_NoteFrequencies[ "A1" ] = 55; s_NoteFrequencies[ "A#1" ] = 58.27; s_NoteFrequencies[ "Bb1" ] = 58.27; s_NoteFrequencies[ "B1" ] = 61.735;
			s_NoteFrequencies[ "C2" ] = 65.406; s_NoteFrequencies[ "C#2" ] = 69.296; s_NoteFrequencies[ "Db2" ] = 69.296; s_NoteFrequencies[ "D2" ] = 73.416; s_NoteFrequencies[ "D#2" ] = 77.782; s_NoteFrequencies[ "Eb2" ] = 77.782; s_NoteFrequencies[ "E2" ] = 82.407; s_NoteFrequencies[ "F2" ] = 87.307; s_NoteFrequencies[ "F#2" ] = 92.499; s_NoteFrequencies[ "Gb2" ] = 92.499; s_NoteFrequencies[ "G2" ] = 97.999; s_NoteFrequencies[ "G#2" ] = 103.826; s_NoteFrequencies[ "Ab2" ] = 103.826; s_NoteFrequencies[ "A2" ] = 110; s_NoteFrequencies[ "A#2" ] = 116.541; s_NoteFrequencies[ "Bb2" ] = 116.541; s_NoteFrequencies[ "B2" ] = 123.471;
			s_NoteFrequencies[ "C3" ] = 130.813; s_NoteFrequencies[ "C#3" ] = 138.591; s_NoteFrequencies[ "Db3" ] = 138.591; s_NoteFrequencies[ "D3" ] = 146.832; s_NoteFrequencies[ "D#3" ] = 155.563; s_NoteFrequencies[ "Eb3" ] = 155.563; s_NoteFrequencies[ "E3" ] = 164.814; s_NoteFrequencies[ "F3" ] = 174.614; s_NoteFrequencies[ "F#3" ] = 184.997; s_NoteFrequencies[ "Gb3" ] = 184.997; s_NoteFrequencies[ "G3" ] = 195.998; s_NoteFrequencies[ "G#3" ] = 207.652; s_NoteFrequencies[ "Ab3" ] = 207.652; s_NoteFrequencies[ "A3" ] = 220; s_NoteFrequencies[ "A#3" ] = 233.082; s_NoteFrequencies[ "Bb3" ] = 233.082; s_NoteFrequencies[ "B3" ] = 246.942;
			s_NoteFrequencies[ "C4" ] = 261.626; s_NoteFrequencies[ "C#4" ] = 277.183; s_NoteFrequencies[ "Db4" ] = 277.183; s_NoteFrequencies[ "D4" ] = 293.665; s_NoteFrequencies[ "D#4" ] = 311.127; s_NoteFrequencies[ "Eb4" ] = 311.127; s_NoteFrequencies[ "E4" ] = 329.628; s_NoteFrequencies[ "F4" ] = 349.228; s_NoteFrequencies[ "F#4" ] = 369.994; s_NoteFrequencies[ "Gb4" ] = 369.994; s_NoteFrequencies[ "G4" ] = 391.995; s_NoteFrequencies[ "G#4" ] = 415.305; s_NoteFrequencies[ "Ab4" ] = 415.305; s_NoteFrequencies[ "A4" ] = 440; s_NoteFrequencies[ "A#4" ] = 466.164; s_NoteFrequencies[ "Bb4" ] = 466.164; s_NoteFrequencies[ "B4" ] = 493.883;
			s_NoteFrequencies[ "C5" ] = 523.251; s_NoteFrequencies[ "C#5" ] = 554.365; s_NoteFrequencies[ "Db5" ] = 554.365; s_NoteFrequencies[ "D5" ] = 587.33; s_NoteFrequencies[ "D#5" ] = 622.254; s_NoteFrequencies[ "Eb5" ] = 622.254; s_NoteFrequencies[ "E5" ] = 659.255; s_NoteFrequencies[ "F5" ] = 698.456; s_NoteFrequencies[ "F#5" ] = 739.989; s_NoteFrequencies[ "Gb5" ] = 739.989; s_NoteFrequencies[ "G5" ] = 783.991; s_NoteFrequencies[ "G#5" ] = 830.609; s_NoteFrequencies[ "Ab5" ] = 830.609; s_NoteFrequencies[ "A5" ] = 880; s_NoteFrequencies[ "A#5" ] = 932.328; s_NoteFrequencies[ "Bb5" ] = 932.328; s_NoteFrequencies[ "B5" ] = 987.767;
			s_NoteFrequencies[ "C6" ] = 1046.502; s_NoteFrequencies[ "C#6" ] = 1108.731; s_NoteFrequencies[ "Db6" ] = 1108.731; s_NoteFrequencies[ "D6" ] = 1174.659; s_NoteFrequencies[ "D#6" ] = 1244.508; s_NoteFrequencies[ "Eb6" ] = 1244.508; s_NoteFrequencies[ "E6" ] = 1318.51; s_NoteFrequencies[ "F6" ] = 1396.913; s_NoteFrequencies[ "F#6" ] = 1479.978; s_NoteFrequencies[ "Gb6" ] = 1479.978; s_NoteFrequencies[ "G6" ] = 1567.982; s_NoteFrequencies[ "G#6" ] = 1661.219; s_NoteFrequencies[ "Ab6" ] = 1661.219; s_NoteFrequencies[ "A6" ] = 1760; s_NoteFrequencies[ "A#6" ] = 1864.655; s_NoteFrequencies[ "Bb6" ] = 1864.655; s_NoteFrequencies[ "B6" ] = 1975.533;
			s_NoteFrequencies[ "C7" ] = 2093.005; s_NoteFrequencies[ "C#7" ] = 2217.461; s_NoteFrequencies[ "Db7" ] = 2217.461; s_NoteFrequencies[ "D7" ] = 2349.318; s_NoteFrequencies[ "D#7" ] = 2489.016; s_NoteFrequencies[ "Eb7" ] = 2489.016; s_NoteFrequencies[ "E7" ] = 2637.021; s_NoteFrequencies[ "F7" ] = 2793.826; s_NoteFrequencies[ "F#7" ] = 2959.955; s_NoteFrequencies[ "Gb7" ] = 2959.955; s_NoteFrequencies[ "G7" ] = 3135.964; s_NoteFrequencies[ "G#7" ] = 3322.438; s_NoteFrequencies[ "Ab7" ] = 3322.438; s_NoteFrequencies[ "A7" ] = 3520; s_NoteFrequencies[ "A#7" ] = 3729.31; s_NoteFrequencies[ "Bb7" ] = 3729.31; s_NoteFrequencies[ "B7" ] = 3951.066;
			s_NoteFrequencies[ "C8" ] = 4186.009; s_NoteFrequencies[ "C#8" ] = 4434.922; s_NoteFrequencies[ "Db8" ] = 4434.922; s_NoteFrequencies[ "D8" ] = 4698.636; s_NoteFrequencies[ "D#8" ] = 4978.032; s_NoteFrequencies[ "Eb8" ] = 4978.032; s_NoteFrequencies[ "E8" ] = 5274.042; s_NoteFrequencies[ "F8" ] = 5587.652; s_NoteFrequencies[ "F#8" ] = 5919.91; s_NoteFrequencies[ "Gb8" ] = 5919.91; s_NoteFrequencies[ "G8" ] = 6271.928; s_NoteFrequencies[ "G#8" ] = 6644.876; s_NoteFrequencies[ "Ab8" ] = 6644.876; s_NoteFrequencies[ "A8" ] = 7040; s_NoteFrequencies[ "A#8" ] = 7458.62; s_NoteFrequencies[ "Bb8" ] = 7458.62; s_NoteFrequencies[ "B8" ] = 7902.132;
			s_NoteFrequencies[ "C9" ] = 8372.018; s_NoteFrequencies[ "C#9" ] = 8869.844; s_NoteFrequencies[ "Db9" ] = 8869.844; s_NoteFrequencies[ "D9" ] = 9397.272; s_NoteFrequencies[ "D#9" ] = 9956.064; s_NoteFrequencies[ "Eb9" ] = 9956.064; s_NoteFrequencies[ "E9" ] = 10548.084; s_NoteFrequencies[ "F9" ] = 11175.304; s_NoteFrequencies[ "F#9" ] = 11839.82; s_NoteFrequencies[ "Gb9" ] = 11839.82; s_NoteFrequencies[ "G9" ] = 12543.856; s_NoteFrequencies[ "G#9" ] = 13289.752; s_NoteFrequencies[ "Ab9" ] = 13289.752; s_NoteFrequencies[ "A9" ] = 14080; s_NoteFrequencies[ "A#9" ] = 14917.24; s_NoteFrequencies[ "Bb9" ] = 14917.24; s_NoteFrequencies[ "B9" ] = 15804.264;

			// Info harvested from http://www.daddariostrings.com/Resources/JDCDAD/images/tension_chart.pdf
			s_PlainSteelStrings = new List<StringInfo>();
			s_NickelWoundStrings = new List<StringInfo>();
			s_BassStrings = new List<StringInfo>();
			// Plain Steel -  Lock Twist
			s_PlainSteelStrings.Add( new StringInfo( "PL007", 0.00001085, 0.007 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL008", 0.00001418, 0.008 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL0085", 0.00001601, 0.0085 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL009", 0.00001794, 0.009 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL0095", 0.00001999, 0.0095 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL010", 0.00002215, 0.010 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL0105", 0.00002442, 0.0105 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL011", 0.00002680, 0.011 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL0115", 0.00002930, 0.0115 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL012", 0.00003190, 0.012 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL013", 0.00003744, 0.013 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL0135", 0.00004037, 0.0135 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL014", 0.00004342, 0.014 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL015", 0.00004984, 0.015 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL016", 0.00005671, 0.016 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL017", 0.00006402, 0.017 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL018", 0.00007177, 0.018 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL019", 0.00007997, 0.019 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL020", 0.00008861, 0.020 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL022", 0.00010722, 0.022 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL024", 0.00012760, 0.024 ) );
			s_PlainSteelStrings.Add( new StringInfo( "PL026", 0.00014975, 0.026 ) );
	
			//XL - Nickelplated Steel Round Wound
			s_NickelWoundStrings.Add( new StringInfo( "NW017", 0.00005524, 0.017 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW018", 0.00006215, 0.018 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW019", 0.00006947, 0.019 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW020", 0.00007495, 0.020 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW021", 0.00008293, 0.021 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW022", 0.00009184, 0.022 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW024", 0.00010857, 0.024 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW026", 0.00012671, 0.026 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW028", 0.00014666, 0.028 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW030", 0.00017236, 0.030 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW032", 0.00019347, 0.032 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW034", 0.00021590, 0.034 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW036", 0.00023964, 0.036 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW038", 0.00026471, 0.038 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW039", 0.00027932, 0.039 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW042", 0.00032279, 0.042 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW044", 0.00035182, 0.044 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW046", 0.00038216, 0.046 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW048", 0.00041382, 0.048 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW049", 0.00043014, 0.049 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW052", 0.00048109, 0.052 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW054", 0.00053838, 0.054 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW056", 0.00057598, 0.056 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW059", 0.00064191, 0.059 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW060", 0.00066542, 0.060 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW062", 0.00070697, 0.062 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW064", 0.00074984, 0.064 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW066", 0.00079889, 0.066 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW068", 0.00084614, 0.068 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW070", 0.00089304, 0.070 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW072", 0.00094124, 0.072 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW074", 0.00098869, 0.074 ) );
			s_NickelWoundStrings.Add( new StringInfo( "NW080", 0.00115011, 0.080 ) );

			//XL - Nickelplated Round Wound
		/*	s_BassStrings.Add( new StringInfo( "XLB018P", 0.00007265, 0.018 ) );
			s_BassStrings.Add( new StringInfo( "XLB020P", 0.00009093, 0.020 ) );
			s_BassStrings.Add( new StringInfo( "XLB028W", 0.00015433, 0.028 ) );
			s_BassStrings.Add( new StringInfo( "XLB042", 0.00032252, 0.042 ) );
			s_BassStrings.Add( new StringInfo( "XLB052", 0.00051322, 0.052 ) );
			s_BassStrings.Add( new StringInfo( "XLB032", 0.00019000, 0.032 ) );
			s_BassStrings.Add( new StringInfo( "XLB035", 0.00022362, 0.035 ) );
			s_BassStrings.Add( new StringInfo( "XLB040", 0.00029322, 0.040 ) );
			s_BassStrings.Add( new StringInfo( "XLB045", 0.00037240, 0.045 ) );
			s_BassStrings.Add( new StringInfo( "XLB050", 0.00046463, 0.050 ) );
			s_BassStrings.Add( new StringInfo( "XLB055", 0.00054816, 0.055 ) );
			s_BassStrings.Add( new StringInfo( "XLB060", 0.00066540, 0.060 ) );
			s_BassStrings.Add( new StringInfo( "XLB065", 0.00079569, 0.065 ) );
			s_BassStrings.Add( new StringInfo( "XLB070", 0.00093218, 0.070 ) );
			s_BassStrings.Add( new StringInfo( "XLB075", 0.00104973, 0.075 ) );
			s_BassStrings.Add( new StringInfo( "XLB080", 0.00116023, 0.080 ) );
			s_BassStrings.Add( new StringInfo( "XLB085", 0.00133702, 0.085 ) );
			s_BassStrings.Add( new StringInfo( "XLB090", 0.00150277, 0.090 ) );
			s_BassStrings.Add( new StringInfo( "XLB095", 0.00169349, 0.095 ) );
			s_BassStrings.Add( new StringInfo( "XLB100", 0.00179687, 0.100 ) );
			s_BassStrings.Add( new StringInfo( "XLB105", 0.00198395, 0.105 ) );
			s_BassStrings.Add( new StringInfo( "XLB110", 0.00227440, 0.110 ) );
			s_BassStrings.Add( new StringInfo( "XLB120", 0.00250280, 0.120 ) );
			s_BassStrings.Add( new StringInfo( "XLB125", 0.00274810, 0.125 ) );
			s_BassStrings.Add( new StringInfo( "XLB130", 0.00301941, 0.130 ) );
			s_BassStrings.Add( new StringInfo( "XLB135", 0.00315944, 0.135 ) );
			s_BassStrings.Add( new StringInfo( "XLB145", 0.00363204, 0.145 ) );
			s_BassStrings.Add( new StringInfo( "XLB120T", 0.00250280, 0.120 ) );
			s_BassStrings.Add( new StringInfo( "XLB125T", 0.00274810, 0.125 ) );
			s_BassStrings.Add( new StringInfo( "XLB130T", 0.00301941, 0.130 ) );
			s_BassStrings.Add( new StringInfo( "XLB135T", 0.00315944, 0.135 ) );
			s_BassStrings.Add( new StringInfo( "XLB145T", 0.00363204, 0.145 ) );*/
		}


		public static double GetNoteFrequency( string strNote )
		{
			try
			{
				return s_NoteFrequencies[ strNote ];
			}
			catch
			{
				return 440;
			}
		}

		public static StringInfo SelectBestString( double dLengthInInches, double dFrequency, double dTargetTensionPlain, double dTargetTensionWound, ref double dResultantTension )
		{
			double den = 4 * dLengthInInches * dLengthInInches * dFrequency * dFrequency;
			double dIdealPlainUnitWeight = 386.4 * dTargetTensionPlain / den;
			double dIdealWoundUnitWeight = 386.4 * dTargetTensionWound / den;

			double dClosest = 100000000;
			StringInfo closest = null;
			foreach( StringInfo s in s_PlainSteelStrings )
			{
				double dError = dIdealPlainUnitWeight - s.m_dUnitWeight;
				if( Math.Abs( dError ) < dClosest )
				{
					dClosest = Math.Abs( dError );
					closest = s;
				}
			}

			// favour plain strings - if it is within 1 lb/in then just use it
			dResultantTension = closest.m_dUnitWeight * den / 386.4; 
			if( Math.Abs( dResultantTension - dTargetTensionPlain ) <= 1 )
				return closest;

			foreach( StringInfo s in s_NickelWoundStrings )
			{
				double dError = dIdealWoundUnitWeight - s.m_dUnitWeight;
				if( Math.Abs( dError ) < dClosest )
				{
					dClosest = Math.Abs( dError );
					closest = s;
				}
			}

			// favour normal strings - if it is within 1 lb/in then just use it
			dResultantTension = closest.m_dUnitWeight * den / 386.4;
			if( Math.Abs( dResultantTension - dTargetTensionPlain ) <= 1 )
				return closest;

			foreach( StringInfo s in s_BassStrings )
			{
				double dError = dIdealWoundUnitWeight - s.m_dUnitWeight;
				if( Math.Abs( dError ) < dClosest )
				{
					dClosest = Math.Abs( dError );
					closest = s;
				}
			}

			// calculate the resultant tension
			dResultantTension = closest.m_dUnitWeight * den / 386.4;
			return closest;
		}

		public static StringInfo SelectBestPlainString( double dLengthInInches, double dFrequency, double dTargetTension, ref double dResultantTension )
		{
			double den = 4 * dLengthInInches * dLengthInInches * dFrequency * dFrequency;
			double dIdealUnitWeight = 386.4 * dTargetTension / den;

			double dClosest = 100000000;
			StringInfo closest = null;
			foreach( StringInfo s in s_PlainSteelStrings )
			{
				double dError = dIdealUnitWeight - s.m_dUnitWeight;
				if( Math.Abs( dError ) < dClosest )
				{
					dClosest = Math.Abs( dError );
					closest = s;
				}
			}

			// calculate the resultant tension
			dResultantTension = closest.m_dUnitWeight * den / 386.4;
			return closest;
		}
		public static StringInfo SelectBestWoundString( double dLengthInInches, double dFrequency, double dTargetTension, ref double dResultantTension )
		{
			double den = 4 * dLengthInInches * dLengthInInches * dFrequency * dFrequency;
			double dIdealUnitWeight = 386.4 * dTargetTension / den;

			double dClosest = 100000000;
			StringInfo closest = null;
			foreach( StringInfo s in s_NickelWoundStrings )
			{
				double dError = dIdealUnitWeight - s.m_dUnitWeight;
				if( Math.Abs( dError ) < dClosest )
				{
					dClosest = Math.Abs( dError );
					closest = s;
				}
			}

			// calculate the resultant tension
			dResultantTension = closest.m_dUnitWeight * den / 386.4;
			return closest;
		}
	}
}

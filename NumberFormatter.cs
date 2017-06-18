using System;

namespace DiskFill
{
	/// <summary>
	/// Extended formatting of numbers
	/// </summary>
	public static class NumberFormatter
	{
	    /// <summary>
		/// Rounds number to specified precision.  
		/// </summary>
		/// <param name="val">The number to be rounded.</param>
		/// <param name="precision">The number of digits of precision to round to.</param>
		/// <returns>Rounded number.</returns>
	    private static double Round( double val, int precision )
		{
		  	if ( precision<1 )
				throw new ArgumentException( "Precision must be >= 1." );

			// Remove the sign
			double unsigned = Math.Abs( val );
			
			double log10 = 0;
			if (unsigned != 0.0)
				log10 = Math.Log10( unsigned );

			// Dividing by scalefactor results in number between 0-1
			double scaleFactor = Math.Pow( 10, Math.Ceiling( log10 ) );

			double res = Math.Round( unsigned / scaleFactor, precision ) * scaleFactor;

			// Put sign back on
			if (val<0)
				res = -res;

			return res;
		}

		/// <summary>
		/// Calculates percentage (0-100%)
		/// </summary>
		/// <param name="value"></param>
		/// <param name="total"></param>
		/// <returns></returns>
		public static double ToPercentage( double value, double total )
		{
			double res = 100.0*(total-value) / total;
			return res;
		}

	    /// <summary>
		/// Converts a floating point value to a string containing suffixes = 1024
		/// </summary>
		/// <param name="value">The value to be converted</param>
		/// <sample>SetSI( 3152.55, 1, "B" ) -> "3.1KB"</sample>
		/// <param name="precision">Number of digits to return in string.</param>
		/// <param name="suffix">Optional suffix to be appended </param>
		/// <returns>SI formatted number with prefixes</returns>
		public static string To1024BaseString( ulong value, int precision, string suffix )
		{
			char[] pref = {' ', 'k','M','G','T'};

			int prefixNum = 0;
			double dValue = value;
			while (dValue > 1024 && prefixNum < pref.GetUpperBound(0) )
			{
				dValue = dValue / 1024;
				prefixNum++;
			}

			dValue = Round( dValue, precision );
			int digitsInFront = (int) Math.Log10(dValue)+1;
			if (digitsInFront < 0)
				digitsInFront = 0;
			int decimals = precision - digitsInFront;
			if (decimals < 0)
				decimals = 0;
			string format = "{0:f" + decimals.ToString() + "}";
			string number = string.Format( format, dValue );

			// Add prefix if any
			string prefix="";
			if (prefixNum != 0)
				prefix = pref[prefixNum].ToString();
		
			// Final, units and prefixes inserted
			return $"{number} {prefix}{suffix}";
		}
	}
}


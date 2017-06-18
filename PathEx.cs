using System;
using System.IO;

namespace DiskFill
{
	public static class PathEx
	{
		/// <summary>
		/// Compute parent directory of the specified directory.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private static string ParentDir( string path )
		{
			int parPos = path.LastIndexOf( Path.DirectorySeparatorChar, path.Length-2 );
			if (parPos<0)
				throw new ArgumentException($"Path {path} has no parent dir.");
			
			string parent = path.Substring( 0, parPos+1 );
			return parent;
		}

		public static void MoveDirectory( string source, string dest )
		{
			// Create parent for destination if missing
			string parent = ParentDir( dest );
			if ( !Directory.Exists( parent ) )
				Directory.CreateDirectory( parent );

			Directory.Move( source, dest );
		}

		/// <summary>
		/// Extracts the relative path from base path to the sub path.
		/// </summary>
		/// <param name="basePath">The path to calculate relative path from.</param>
		/// <param name="subPath">The path to calculate relative path to.</param>
		/// <returns>Relative path from base to sub.</returns>
		public static string RelativePath( string basePath, string subPath )
		{
			string baseAbsDir = Path.GetFullPath( basePath ).Trim('\\');
			string subFull = Path.GetFullPath( subPath ).Trim('\\');
			int baseLen = baseAbsDir.Length;
			if ( baseLen > subFull.Length )
				// Subdirectory is not under base
				throw new ArgumentException( "Subdirectory is not below base directory." );
			if ( baseAbsDir.ToLower() != subFull.Substring(0,baseLen).ToLower() )
				// Subdirectory is in different location than base
				throw new ArgumentException( "Subdirectory is not below base directory." );

			string relPath = subFull.Substring( baseLen );
			return relPath;
		}


		/// <summary>
		/// Check if a pattern containing wildcards fits the string. (globbing)
		/// </summary>
		/// <param name="wild"></param>
		/// <param name="str"></param>
		/// <returns></returns>
		private static bool MatchWildPattern( string wild, string str ) 
		{
			int sidx = 0;
			int widx = 0;
			int slen = str.Length;
			int wlen = wild.Length;
			wild += "_";

			while (sidx < slen ) 
			{
				if (widx >= wlen)
					return false;
				if (wild[ widx ] == '*')
					break;
				if ((wild[ widx ] != str[ sidx ]) && (wild[ widx ] != '?')) 
					return false;
				widx++;
				sidx++;
			}
		
			int cp  = 0;
			int mp = 0;
			while (sidx < slen ) 
			{
				if (wild[ widx ] == '*') 
				{
					widx++;
					if ( widx >= wlen ) 
						return true;
	
					mp = widx;
					cp = sidx+1;
				} 
				else if ((wild[ widx ] == str[ sidx ]) || (wild[ widx ] == '?')) 
				{
					widx++;
					sidx++;
				} 
				else 
				{
					widx = mp;
					sidx = cp++;
				}
			}
		
			while (wild[ widx ] == '*') 
				widx++;
			return widx >= wlen;
		}

	    /// <summary>
		/// Calculate sum of bytes in directory including any subdirectories.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static long DirectorySize( string path, bool includeSubDirs )
		{
			long size = 0;
			string[] files = Directory.GetFiles( path );
			foreach( string file in files )
			{
				FileInfo f = new FileInfo( file );
				size += f.Length;
			}

			if ( includeSubDirs )
			{
				files = Directory.GetDirectories( path );
				foreach( string dir in files )
				{
					size += DirectorySize( dir, includeSubDirs );
				}
			}
			return size;
		}

	    public static long FileSize( string file )
		{
			FileInfo fi = new FileInfo( file );
			return fi.Length;
		}
	}
/*
	[TestFixture]
	public class PathTest 
	{

		[Test]
		public void TestMatchWildPattern()
		{
			Assertion.AssertEquals( PathEx.MatchWildPattern( "", ""), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "*", ""), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "*", "A"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "", "A"), false );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*", ""), false );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*", "AAB"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*", "BAA"), false );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*", "A"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B", ""), false );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B", "AAB"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B", "AB"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B", "AABA"), false );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B", "ABAB"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B", "ABBBB"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B*C", ""), false );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B*C", "ABC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B*C", "ABCC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B*C", "ABBBC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B*C", "ABBBBCCCC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B*C", "ABCBBBCBCCCBCBCCCC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B*", "AB"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B*", "AABA"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B*", "ABAB"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B*", "ABBBB"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B*C*", ""), false );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B*C*", "ABC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B*C*", "ABCC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B*C*", "ABBBC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B*C*", "ABBBBCCCC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A*B*C*", "ABCBBBCBCCCBCBCCCC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A?", "AAB"), false );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A?B", "AAB"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A?*", "A"), false );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A?*", "ABBCC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPattern( "A?*", "BAA"), false );		
			Assertion.AssertEquals( PathEx.MatchWildPattern( "*.*", "aaaaa"), false );		
		}

		[Test]
		public void TestMatchWildPatternToFileName()
		{
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "", ""), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "*", ""), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "*", "A"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "", "A"), false );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*", ""), false );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*", "AAB"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*", "BAA"), false );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*", "A"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B", ""), false );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B", "AAB"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B", "AB"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B", "AABA"), false );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B", "ABAB"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B", "ABBBB"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B*C", ""), false );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B*C", "ABC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B*C", "ABCC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B*C", "ABBBC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B*C", "ABBBBCCCC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B*C", "ABCBBBCBCCCBCBCCCC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B*", "AB"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B*", "AABA"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B*", "ABAB"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B*", "ABBBB"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B*C*", ""), false );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B*C*", "ABC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B*C*", "ABCC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B*C*", "ABBBC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B*C*", "ABBBBCCCC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A*B*C*", "ABCBBBCBCCCBCBCCCC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A?", "AAB"), false );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A?B", "AAB"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A?*", "A"), false );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A?*", "ABBCC"), true );
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "A?*", "BAA"), false );		

			// Added MatchWildPatternToFileName functionality: (DOS compatibility)
			Assertion.AssertEquals( PathEx.MatchWildPatternToFileName( "*.*", "aaaaa"), true );		
		}
	}
*/
}

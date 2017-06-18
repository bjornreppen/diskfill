using System;
using Microsoft.Win32;

namespace DiskFill
{
	public class WindowsRegistry
	{
		private readonly RegistryKey regKey;
		private RegistryKey subKey;
	    private readonly bool AutoCreateMissing = true; 

		public WindowsRegistry(RegistryHive hive)
		{
			regKey = RegistryKey.OpenRemoteBaseKey( hive,"" );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="subKeyName"></param>
		public void OpenSubKey( string subKeyName )
		{
			if (AutoCreateMissing)
				subKey = regKey.CreateSubKey(subKeyName);
			else
				subKey = regKey.OpenSubKey( subKeyName ); 
		}
		/// <summary>
		/// Reads a value from the registry
		/// </summary>
		/// <param name="Name"></param>
		/// <returns></returns>
		public object GetValue( string Name )
		{
			object Value = subKey.GetValue( Name );			
			if ( Value==null ) 
				throw new ArgumentException("Missing value for registry key " + ToString() + 
@"\" + Name + ", and no default value supplied!");
 			return Value;
		}

		/// <summary>
		/// Reads a value from the registry
		/// </summary>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns>Value stored in registry, DefaultValue if not found.</returns>
		public object GetValue( string name, object defaultValue )
		{
			object value = subKey.GetValue( name);			
			if (value == null)
			{
				if (AutoCreateMissing)
					subKey.SetValue( name, defaultValue );
				return defaultValue;
			}
			return value;
		}

		/// <summary>
		/// Write a value to the registry
		/// </summary>
		public void SetValue( string name, object data )
		{
			subKey.SetValue( name, data );			
		}

		public override string ToString()
		{
			return regKey.ToString();
		}
	}
}

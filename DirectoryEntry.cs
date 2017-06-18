using System;

namespace DiskFill
{
	/// <summary>
	/// 
	/// </summary>
	public class DirectoryEntry : IComparable
	{
	    /// <summary>
		/// Property Path (string)
		/// </summary>
		public string FullPath { get; set; }

	    /// <summary>
		/// Property Size (ulong)
		/// </summary>
		public ulong Size { get; set; }


	    public int CompareTo(object obj)
		{
			if (Size < ((DirectoryEntry)obj).Size)
				return -1;
			if (Size > ((DirectoryEntry)obj).Size)
				return 1;
			return 0;
		}

		public override string ToString()
		{
			return FullPath;
		}
	}

}

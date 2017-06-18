using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace DiskFill
{
	/// <summary>
	/// Wrapper class to browse for a folder with more options
	/// </summary>
	public class BrowseForFolderDialog : FolderNameEditor 
	{
		/// <summary>
		/// Folder name styles.
		/// </summary>
		public enum FolderNameStyles
		{
			/// <summary>
			/// BrowseForComputer   
			/// </summary>
			BrowseForComputer    = 0x1000,
			/// <summary>
			/// BrowseForEverything   
			/// </summary>
			BrowseForEverything   = 0x4000,
			/// <summary>
			/// BrowseForPrinter        
			/// </summary>
			BrowseForPrinter          = 0x2000,
			/// <summary>
			/// RestrictToDomain       
			/// </summary>
			RestrictToDomain         = 0x0002,
			/// <summary>
			/// RestrictToFilesystem 
			/// </summary>
			RestrictToFilesystem    = 0x0001,
			/// <summary>
			/// RestrictToSubfolders    
			/// </summary>
			RestrictToSubfolders    = 0x0008,
			/// <summary>
			/// ShowTextBox						
			/// </summary>
			ShowTextBox						= 0x000f,
		};

	    /// <summary>
		/// Default constructor
		/// </summary>
		public BrowseForFolderDialog()
		{
			dlg = new FolderBrowser();
			Description = "Choose a folder";
		}

		/// <summary>
		/// Displays the modal folder browse dialog.
		/// </summary>
		/// <returns></returns>
		public DialogResult ShowDialog()
		{
			return dlg.ShowDialog();
		}

		/// <summary>
		/// Displays the modal folder browse dialog.
		/// </summary>
		/// <param name="owner"></param>
		/// <returns></returns>
		public DialogResult ShowDialog( IWin32Window owner )
		{
			return dlg.ShowDialog(owner);
		}

		/// <summary>
		/// Description to print on form.
		/// </summary>
		public string Description
		{
			get => dlg.Description;
		    set => dlg.Description = value;
		}

		/// <summary>
		/// The path selected.
		/// </summary>
		public string DirectoryPath => dlg.DirectoryPath;

	    /// <summary>
		/// They style of the browse dialog. 
		/// </summary>
		public FolderNameStyles Style
		{
			get => (FolderNameStyles) dlg.Style;
	        set => dlg.Style = (FolderBrowserStyles)value;
	    }

	    FolderBrowser  dlg;
	}
}

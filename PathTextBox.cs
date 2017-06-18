using System;
using System.Windows.Forms;
using DiskFill;

namespace King.Windows.Forms
{
	public class PathTextBox : System.Windows.Forms.UserControl
	{
		private TextBox textBox;
		private Button buttonBrowse;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

	    public PathTextBox()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call

		}

		/// <summary>
		/// The directory the control points to
		/// </summary>
		public override string Text
		{
			get => textBox.Text;
		    set
			{ 
				textBox.Text = value; 
				OnTextChanged( EventArgs.Empty );
			}
		}

		/// <summary>
		/// Title for display in the browse window opened by browse button.
		/// </summary>
		public string Description { get; set; }

	    /// <summary>
		/// The text displayed on the browse button
		/// </summary>
		public string ButtonText
		{
			get => buttonBrowse.Text;
	        set => buttonBrowse.Text = value;
	    }

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBox = new System.Windows.Forms.TextBox();
			this.buttonBrowse = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBox
			// 
			this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.textBox.Location = new System.Drawing.Point(1, 1);
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(252, 20);
			this.textBox.TabIndex = 0;
			this.textBox.Text = "";
			this.textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
			// 
			// buttonBrowse
			// 
			this.buttonBrowse.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.buttonBrowse.Location = new System.Drawing.Point(254, 1);
			this.buttonBrowse.Name = "buttonBrowse";
			this.buttonBrowse.Size = new System.Drawing.Size(60, 20);
			this.buttonBrowse.TabIndex = 1;
			this.buttonBrowse.Text = "Browse&...";
			this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
			// 
			// PathTextBox
			// 
			this.AllowDrop = true;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.buttonBrowse,
																																	this.textBox});
			this.Name = "PathTextBox";
			this.Size = new System.Drawing.Size(315, 22);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.PathTextBox_DragEnter);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.PathTextBox_DragDrop);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Allow drag and drop of folder
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PathTextBox_DragEnter(object sender, DragEventArgs e)
		{
			if ( e.Data.GetDataPresent( DataFormats.FileDrop, false ) ) 
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None; 
		}

		/// <summary>
		/// Handle the dropped folder
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PathTextBox_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);

			if (files.Length != 1)
			{
				MessageBox.Show( "Only a single folder can be dragged and dropped on this control." );
				return;
			}

			Text = files[0];
		}

		/// <summary>
		/// Browse for directory
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonBrowse_Click(object sender, EventArgs e)
		{
			BrowseForFolderDialog fn = new BrowseForFolderDialog();
			fn.Style = BrowseForFolderDialog.FolderNameStyles.RestrictToFilesystem;
			fn.Description = Description;
			if (fn.ShowDialog(this) == DialogResult.OK)
			{
				Text = fn.DirectoryPath;
			}
		}

		private void textBox_TextChanged(object sender, EventArgs e)
		{
			OnTextChanged( EventArgs.Empty );
		}

		public override string ToString()
		{
			return Text;
		}
	}
}

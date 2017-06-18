using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using King.Windows.Forms;


namespace DiskFill
{
	/// <summary>
	/// The form DiskFill lives in.
	/// </summary>
	public class FormMain : Form
	{
		enum ComputeState
		{
			Idle,       // Not started or complete
			Processing, // Worker thread is busy
			Canceled    // User canceled, worker thread is still running
		}

		ComputeState _state = ComputeState.Idle;

		private PathTextBox pathBox;
		private Label label1;
		private TextBox textBoxResult;
		private Label label2;
		private Label label3;
		private Button buttonCompute;
		private NumericUpDown numericUpDownLevel;
		private Label label4;

		private Computer _computer;
		private Button buttonMove;
		private Label label5;
		private NumericUpDown numericUpDownSize;
		private ToolTip toolTip;
		private System.ComponentModel.IContainer components;

		private readonly StatusBarPanel statusBarText;

		const string REG_PATH = "Path";
		const string  REG_SUBLEVEL = "SubLevel";
		const string REG_SIZE = "Size";
		const int DEFAULT_SIZE = 700*1024;
		const int DEFAULT_LEVEL = 1;

		private StatusBar statusBar;
		private ImageList imageList;
		private ComboBox comboBoxUnits;
		static readonly string REG_SUBKEY = @"Software\King\" + Application.ProductName;

		public FormMain()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();


			statusBarText = new StatusBarPanel();
			statusBarText.Style = StatusBarPanelStyle.Text;
			statusBarText.Text = "Ready.";
			statusBar.Panels.Add( statusBarText );
			statusBarText.AutoSize = StatusBarPanelAutoSize.Spring;
			statusBar.ShowPanels = true;

			Text = $"{Application.ProductName} v{Application.ProductVersion}";
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.pathBox = new King.Windows.Forms.PathTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxResult = new System.Windows.Forms.TextBox();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonCompute = new System.Windows.Forms.Button();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.numericUpDownLevel = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonMove = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownSize = new System.Windows.Forms.NumericUpDown();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.comboBoxUnits = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSize)).BeginInit();
            this.SuspendLayout();
            // 
            // pathBox
            // 
            this.pathBox.AllowDrop = true;
            this.pathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pathBox.ButtonText = "Browse&...";
            this.pathBox.Description = "Select the directory you wish to select files from.";
            this.pathBox.Location = new System.Drawing.Point(9, 27);
            this.pathBox.Name = "pathBox";
            this.pathBox.Size = new System.Drawing.Size(601, 22);
            this.pathBox.TabIndex = 1;
            this.toolTip.SetToolTip(this.pathBox, "Directory to pick files from.");
            // 
            // label1
            // 
            this.label1.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.label1.Location = new System.Drawing.Point(7, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source &path:";
            // 
            // textBoxResult
            // 
            this.textBoxResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResult.Location = new System.Drawing.Point(8, 128);
            this.textBoxResult.Multiline = true;
            this.textBoxResult.Name = "textBoxResult";
            this.textBoxResult.ReadOnly = true;
            this.textBoxResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxResult.Size = new System.Drawing.Size(612, 263);
            this.textBoxResult.TabIndex = 10;
            this.toolTip.SetToolTip(this.textBoxResult, "The best combination of files found by the program to fill the target size.");
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 399);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(637, 22);
            this.statusBar.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.label2.Location = new System.Drawing.Point(7, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "&Target size:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(78, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "MB";
            // 
            // buttonCompute
            // 
            this.buttonCompute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCompute.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonCompute.ImageIndex = 0;
            this.buttonCompute.ImageList = this.imageList;
            this.buttonCompute.Location = new System.Drawing.Point(390, 64);
            this.buttonCompute.Name = "buttonCompute";
            this.buttonCompute.Size = new System.Drawing.Size(107, 42);
            this.buttonCompute.TabIndex = 7;
            this.buttonCompute.Text = "&Compute";
            this.toolTip.SetToolTip(this.buttonCompute, "Start searching for best combination of files.");
            this.buttonCompute.Click += new System.EventHandler(this.buttonCompute_Click);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "");
            this.imageList.Images.SetKeyName(1, "");
            // 
            // numericUpDownLevel
            // 
            this.numericUpDownLevel.Location = new System.Drawing.Point(89, 82);
            this.numericUpDownLevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownLevel.Name = "numericUpDownLevel";
            this.numericUpDownLevel.Size = new System.Drawing.Size(53, 20);
            this.numericUpDownLevel.TabIndex = 6;
            this.toolTip.SetToolTip(this.numericUpDownLevel, "How far down in directories to pick single files/subfolders. Set to 1 for files/f" +
        "olders in selected directory only (no subfiles/folders).");
            this.numericUpDownLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.label4.Location = new System.Drawing.Point(7, 84);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "&Max recursion:";
            // 
            // buttonMove
            // 
            this.buttonMove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMove.Enabled = false;
            this.buttonMove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonMove.ImageIndex = 1;
            this.buttonMove.ImageList = this.imageList;
            this.buttonMove.Location = new System.Drawing.Point(501, 64);
            this.buttonMove.Name = "buttonMove";
            this.buttonMove.Size = new System.Drawing.Size(107, 42);
            this.buttonMove.TabIndex = 8;
            this.buttonMove.Text = "&Move files";
            this.toolTip.SetToolTip(this.buttonMove, "Move the best match files to a directory of your choice.");
            this.buttonMove.Click += new System.EventHandler(this.buttonMove_Click);
            // 
            // label5
            // 
            this.label5.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.label5.Location = new System.Drawing.Point(7, 111);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 16);
            this.label5.TabIndex = 9;
            this.label5.Text = "Selected files:";
            // 
            // numericUpDownSize
            // 
            this.numericUpDownSize.DecimalPlaces = 1;
            this.numericUpDownSize.Location = new System.Drawing.Point(75, 57);
            this.numericUpDownSize.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numericUpDownSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownSize.Name = "numericUpDownSize";
            this.numericUpDownSize.Size = new System.Drawing.Size(55, 20);
            this.numericUpDownSize.TabIndex = 3;
            this.toolTip.SetToolTip(this.numericUpDownSize, "The (maximum) size you wish the selected files to occupy.");
            this.numericUpDownSize.Value = new decimal(new int[] {
            700,
            0,
            0,
            0});
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.InitialDelay = 400;
            this.toolTip.ReshowDelay = 100;
            // 
            // comboBoxUnits
            // 
            this.comboBoxUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxUnits.Items.AddRange(new object[] {
            "KB",
            "MB",
            "GB"});
            this.comboBoxUnits.Location = new System.Drawing.Point(134, 57);
            this.comboBoxUnits.Name = "comboBoxUnits";
            this.comboBoxUnits.Size = new System.Drawing.Size(40, 21);
            this.comboBoxUnits.TabIndex = 4;
            // 
            // FormMain
            // 
            this.AcceptButton = this.buttonCompute;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(637, 421);
            this.Controls.Add(this.comboBoxUnits);
            this.Controls.Add(this.numericUpDownSize);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.buttonMove);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numericUpDownLevel);
            this.Controls.Add(this.buttonCompute);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.textBoxResult);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pathBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(408, 256);
            this.Name = "FormMain";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FormMain_Closing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new FormMain());
		}

		/// <summary>
		/// Starts processing.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonCompute_Click(object sender, EventArgs e)
		{
			switch( _state )
			{
				case ComputeState.Idle:
					// User wants to start computing
					StartProcessing();
					break;
				case ComputeState.Processing:
					// User wants to cancel
					MakeStateTransition( ComputeState.Canceled );
					break;
				case ComputeState.Canceled:
					// Should never happen (UI is paused while state is Canceled
					Debug.Assert( false );
					break;
				default:
					Debug.Assert( false );
					break;
			}
		}

		void MakeStateTransition( ComputeState newState )
		{
			switch( newState )
			{
				case ComputeState.Idle:
					buttonCompute.Enabled = true;
					buttonCompute.Text = "&Compute";
					numericUpDownSize.Enabled = true;
					buttonMove.Enabled = (textBoxResult.Text.Length > 0);
					comboBoxUnits.Enabled = true;
					numericUpDownLevel.Enabled = true;
					pathBox.Enabled = true;
					break;
				case ComputeState.Processing:
					buttonCompute.Enabled = true;
					buttonCompute.Text = "&Stop";
					buttonMove.Enabled = false;
					numericUpDownSize.Enabled = false;
					comboBoxUnits.Enabled = false;
					numericUpDownLevel.Enabled = false;
					pathBox.Enabled = false;
					break;
				case ComputeState.Canceled:
					buttonCompute.Enabled = false;
					_computer.Stop();
					break;
				default:
					Debug.Assert( false );
					break;
			}
			_state = newState;
		}

		void StartProcessing()
		{
			if (pathBox.Text.Length == 0)
			{
				MessageBox.Show( "Error", "Please specify a source path." );
				return;
			}

			if (!pathBox.Text.EndsWith( Path.DirectorySeparatorChar.ToString() )) 
				pathBox.Text += Path.DirectorySeparatorChar;

			ulong targetSize = GetTargetSize();
			_computer = new Computer( pathBox.Text, (int) numericUpDownLevel.Value, targetSize );
			_computer.BetterMatch += OnProgress;
			_computer.ComputeComplete += OnComplete;

			// Everything looking good, let's start processing.
			MakeStateTransition( ComputeState.Processing );

			System.Threading.ThreadStart entry = _computer.FindOptimalCombination;
			System.Threading.Thread _thread = new System.Threading.Thread( entry );
			_thread.Priority = System.Threading.ThreadPriority.BelowNormal;
			_thread.IsBackground = true;
			_thread.Start();
		}

		/// <summary>
		/// Called periodically while computing.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnProgress( object sender, ShowProgressArgs e )
		{
			try
			{
				if ( InvokeRequired == false )
				{
					if (e.StatusMessage.Length > 0)
						statusBarText.Text = e.StatusMessage;
					else
						statusBarText.Text =
						        $"Selected {NumberFormatter.To1024BaseString(e.BytesUsed, 3, "bytes")}, unused space is {NumberFormatter.To1024BaseString(e.BytesTotal - e.BytesUsed, 3, "bytes")} ({NumberFormatter.ToPercentage(e.BytesUsed, e.BytesTotal):f2}%).";
					textBoxResult.Text = e.FileList;
				}
				else
				{
					Computer.ProgressEvent showProgress = OnProgress;

					Invoke( showProgress, sender, e);
				}
			}
			catch (ObjectDisposedException)
			{
				// User decided to quit
				e.Cancel = true;
			}		
		}

		/// <summary>
		/// Calculates the target size in bytes from the numeric input 
		/// and the suffix combo.
		/// </summary>
		/// <returns>Target size in bytes.</returns>
		private ulong GetTargetSize()
		{
			ulong value = (ulong) numericUpDownSize.Value;
			switch (comboBoxUnits.Text)
			{
				case "KB":
					value = value * 1024;
					break;
				case "MB":
					value = value * 1024 * 1024;
					break;
				case "GB":
					value = value * 1024 * 1024 * 1024;
					break;
				default:
					Debug.Assert( false );
					break;
			}
			return value;
		}

		/// <summary>
		/// Decodes the target size in bytes into numericUpdown value
		/// and suffix combo.
		/// </summary>
		/// <returns>Target size in bytes.</returns>
		private void SetTargetSize( long value )
		{
			int suffix = (int) Math.Log10(value)/3;
			switch (suffix)
			{
				case 2:
					value = value / (1024 * 1024);
					comboBoxUnits.Text = "MB";
					break;
				case 3:
					value = value / (1024 * 1024 * 1024);
					comboBoxUnits.Text = "GB";
					break;
				default:
					value = value / 1024;
					comboBoxUnits.Text = "KB";
					break;
			}
			if (value < numericUpDownSize.Minimum)
				value = (int) numericUpDownSize.Minimum;
			else if (value > numericUpDownSize.Maximum)
				value = (int) numericUpDownSize.Maximum;
			numericUpDownSize.Value = value;
		}

		/// <summary>
		/// Called when processing has been completed or aborted.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnComplete( object sender, ShowProgressArgs e )
		{
			if (IsDisposed)
				return;

			if ( InvokeRequired == false )
			{
				statusBar.Text = e.StatusMessage;
				if (e.Failed)
				{
					textBoxResult.Text = "";
					MessageBox.Show( e.StatusMessage, "Operation failed.",
						MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				textBoxResult.Text = e.FileList;
				MakeStateTransition( ComputeState.Idle );
			}
			else
			{
				Computer.ProgressEvent completeHandler = OnComplete;
				Invoke(completeHandler, sender, e);
			}
		}

		/// <summary>
		/// Move the selected file to another directory.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonMove_Click(object sender, EventArgs e)
		{
			try
			{
				// Browse for destination directory
				BrowseForFolderDialog dialog = new BrowseForFolderDialog();
				dialog.Description = "Please choose what directory the files should be moved to?";
				if ( dialog.ShowDialog() != DialogResult.OK )
					return;
				string destinationPath = dialog.DirectoryPath;		

				// Move the files
				MoveFiles( destinationPath );
				statusBarText.Text = $"Moved selected files to {destinationPath}.";
			}
			catch ( Exception caught )
			{
				MessageBox.Show( caught.Message );
				statusBarText.Text = $"Error while moving: {caught.Message}.";
			}
		}


		private void FormMain_Load(object sender, EventArgs e)
		{
			// Load settings
			try
			{
				WindowsRegistry registry = new WindowsRegistry( Microsoft.Win32.RegistryHive.CurrentUser );
				registry.OpenSubKey( REG_SUBKEY );
				pathBox.Text = (string) registry.GetValue( REG_PATH, "" );
				numericUpDownLevel.Value = (int) registry.GetValue( REG_SUBLEVEL, DEFAULT_LEVEL );
				
				// Registry contains KBytes
				int targetBytes = (int) registry.GetValue( REG_SIZE,	DEFAULT_SIZE );
				SetTargetSize( (long)targetBytes*1024 );
			}
			catch
			{}
		}

		private void FormMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
		    _computer?.Stop();

		    // Save settings
			try
			{
				WindowsRegistry registry = new WindowsRegistry( Microsoft.Win32.RegistryHive.CurrentUser );
				registry.OpenSubKey( REG_SUBKEY );
				registry.SetValue( REG_PATH, pathBox.Text );
				registry.SetValue( REG_SUBLEVEL, (int) numericUpDownLevel.Value );
				
				// Store KBytes
				registry.SetValue( REG_SIZE, (int) (GetTargetSize()/1024) );
			}
			catch
			{}
		}

		private void MoveFiles( string destinationPath )
		{
			string[] best = textBoxResult.Text.Split('\n');
			foreach( string sourceRaw in best)
			{
				string source = sourceRaw.Trim();
				if (source.Length == 0)
					continue;
				string sub = PathEx.RelativePath( _computer.SourcePath, source );
				string newFull = destinationPath + Path.DirectorySeparatorChar + sub;
				if ( source.EndsWith( Path.DirectorySeparatorChar.ToString() ) )
					// Directory
					PathEx.MoveDirectory( source, newFull );
				else
				{
					// File
					string newPath = Path.GetDirectoryName( newFull );
					if (!Directory.Exists( newPath ))
						Directory.CreateDirectory( newPath );
					File.Move( source, newFull );
				}
			}		
		}
	}
}

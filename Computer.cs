using System;
using System.Collections;
using System.IO;

namespace DiskFill
{
    /// <summary>
    /// Computes best mix of files to use specified amount of space.
    /// </summary>
    public class Computer
    {
        public delegate void ProgressEvent(object from, ShowProgressArgs args);

        public ProgressEvent BetterMatch;
        public ProgressEvent ComputeComplete;

        private readonly ulong _maxBytes;
        private ulong _sourceBytes;
        private readonly string _sourcePath;
        private readonly int _nestingLevel;
        private DirectoryEntry[] _sourceFiles;

        private ulong _bestBytes;
        bool[] _currentSelelection;
        bool[] _bestSelelection;
        private bool _cancel;

        public Computer(string path, int nestingLevel, ulong maxBytes)
        {
            _sourcePath = path;
            _nestingLevel = nestingLevel;
            _maxBytes = maxBytes;
        }

        /// <summary>
        /// Property SourcePath (string)
        /// </summary>
        public string SourcePath => _sourcePath;

        public void FindOptimalCombination()
        {
            _cancel = false;

            ShowProgressArgs progress = new ShowProgressArgs(0, _maxBytes);
            try
            {
                BuildFileList(_sourcePath, _nestingLevel);
                if (_cancel)
                    return;

                _bestBytes = 0;
                _bestSelelection = new bool[_sourceFiles.Length];
                _currentSelelection = new bool[_sourceFiles.Length];

                for (int y = 0; y < _currentSelelection.Length; y++)
                    _currentSelelection[y] = false;

                // Sorting descending makes it easier to test the largest files first.
                Array.Sort(_sourceFiles);
                Array.Reverse(_sourceFiles);

                if (_sourceFiles.Length <= 0)
                    throw new ApplicationException(
                        $"No file was found on source path '{_sourcePath}'.");
                if (_sourceFiles[_sourceFiles.GetUpperBound(0)].Size > _maxBytes)
                    throw new ApplicationException("All files are too big for target.");

                if (_maxBytes > _sourceBytes)
                {
                    // Not enough source files, select all and quit.
                    for (int i = 0; i < _bestSelelection.Length; i++)
                        _bestSelelection[i] = true;
                    _bestBytes = _sourceBytes;
                    UpdateProgress(string.Empty, false);
                }
                else
                    FindOptimalCombination(0, 0);
            }
            catch (Exception caught)
            {
                progress.Failed = true;
                progress.StatusMessage = caught.Message;
            }
            finally
            {
                if (ComputeComplete != null)
                {
                    progress.BytesUsed = _bestBytes;
                    if (!progress.Failed)
                        progress.FileList = GetBestFileText();
                    ComputeComplete(this, progress);
                }
            }
        }

        private string GetBestFileText()
        {
            DirectoryEntry[] entries = GetBestFiles();
            string[] files = new String[entries.Length];
            for (int i = 0; i < entries.Length; i++)
            {
                files[i] = entries[i].FullPath;
            }
            Array.Sort(files);
            string res = string.Join(Environment.NewLine, files);
            return res;
        }

        private DirectoryEntry[] GetBestFiles()
        {
            if (_bestSelelection == null)
                return new DirectoryEntry[0];
            int cnt = 0;
            for (int y = 0; y < _bestSelelection.Length; y++)
                if (_bestSelelection[y])
                    cnt++;

            DirectoryEntry[] best = new DirectoryEntry[cnt];
            int idx = 0;
            for (int y = 0; y < _bestSelelection.Length; y++)
            {
                if (_bestSelelection[y])
                {
                    best[idx] = _sourceFiles[y];
                    idx++;
                }
            }
            return best;
        }

        private void UpdateProgress(string statusMessage, bool failed)
        {
            if (BetterMatch != null)
            {
                ShowProgressArgs e =
                    new ShowProgressArgs(_bestBytes, _maxBytes);
                if (_bestSelelection != null)
                    e.FileList = GetBestFileText();
                e.StatusMessage = statusMessage;
                e.Failed = failed;

                BetterMatch(this, e);
                if (e.Cancel)
                    Stop();
            }
        }

        /// <summary>
        /// Request that computing be stopped.
        /// </summary>
        public void Stop()
        {
            _cancel = true;
        }

        private void FindOptimalCombination(ulong bytes, int pos)
        {
            if (_cancel)
                return;

            if (bytes > _bestBytes)
            {
                if (bytes > _maxBytes)
                    return;
                _bestBytes = bytes;
                _currentSelelection.CopyTo(_bestSelelection, 0);
                UpdateProgress(string.Empty, false);
                if (_bestBytes == _maxBytes)
                {
                    // Perfect match found
                    Stop();
                    return;
                }
            }

            if (pos >= _currentSelelection.Length)
                return;

            ulong entBytes = _sourceFiles[pos].Size;
            _currentSelelection[pos] = true;
            FindOptimalCombination(bytes + entBytes, pos + 1);
            _currentSelelection[pos] = false;
            FindOptimalCombination(bytes, pos + 1);
        }

        private void BuildSubList(ArrayList list, string path, int level)
        {
            if (level > 1)
            {
                //Scan subdirs
                string[] dirList = Directory.GetDirectories(path);
                foreach (string file in dirList)
                {
                    if (_cancel)
                        return;
                    BuildSubList(list, file, level - 1);
                }
            }
            else
            {
                // Add directories on the 0 level
                string[] dirList = Directory.GetDirectories(path);
                foreach (string file in dirList)
                {
                    DirectoryEntry dir = new DirectoryEntry();
                    dir.FullPath = file + Path.DirectorySeparatorChar;
                    dir.Size = (ulong) PathEx.DirectorySize(file, true);
                    list.Add(dir);
                }
            }

            // Add files on all levels.
            string[] fileList = Directory.GetFiles(path);
            foreach (string file in fileList)
            {
                DirectoryEntry dir = new DirectoryEntry();
                dir.FullPath = file;
                dir.Size = (ulong) PathEx.FileSize(file);
                list.Add(dir);
            }
        }

        private void BuildFileList(string path, int level)
        {
            string msg = $"Scanning '{path}' for files.";
            UpdateProgress(msg, false);

            ArrayList list = new ArrayList();
            BuildSubList(list, path, level);
            if (_cancel)
                return;
            _sourceFiles = new DirectoryEntry[list.Count];
            for (int y = 0; y < _sourceFiles.Length; y++)
            {
                _sourceFiles[y] = (DirectoryEntry) list[y];
                _sourceBytes += _sourceFiles[y].Size;
            }

            msg = $"{_sourceFiles.Length} files/dirs found.";
            UpdateProgress(msg, false);
        }
    }

    public class ShowProgressArgs : EventArgs
    {
        public ulong BytesUsed;
        public readonly ulong BytesTotal;
        public bool Cancel;
        public bool Failed;
        public string FileList = string.Empty;
        public string StatusMessage = string.Empty;

        public ShowProgressArgs(ulong usedBytes, ulong totalBytes)
        {
            BytesUsed = usedBytes;
            BytesTotal = totalBytes;
        }
    }
}
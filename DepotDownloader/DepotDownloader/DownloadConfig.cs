// This file is subject to the terms and conditions defined
// in file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DepotDownloader
{
    public class DownloadConfig
    {
        public int CellID { get; set; }
        public bool DownloadAllPlatforms { get; set; }
        public bool DownloadAllArchs { get; set; }
        public bool DownloadAllLanguages { get; set; }
        public bool DownloadManifestOnly { get; set; }
        public string InstallDirectory { get; set; }

        public bool UsingFileList { get; set; }
        public HashSet<string> FilesToDownload { get; set; }
        public List<Regex> FilesToDownloadRegex { get; set; }

        public string BetaPassword { get; set; }

        public bool VerifyAll { get; set; }

        public int MaxDownloads { get; set; }

        public bool RememberPassword { get; set; }

        // A Steam LoginID to allow multiple concurrent connections
        public uint? LoginID { get; set; }

        public bool UseQrCode { get; set; }
        public bool SkipAppConfirmation { get; set; }

        public bool Silent { get; set; }

        /// <summary>
        /// A download progress callback.
        /// Params:
        /// 1. The number of bytes downloaded.
        /// 2. The number of bytes to download.
        /// 3. The percentage of bytes downloaded.
        /// </summary>
        public event ProgressEventHandler ProgressCallback;

        public delegate void ProgressEventHandler(object sender, ProgressEventInfo info);

        public class ProgressEventInfo : EventArgs
        {
            public ulong bytesDownloaded;
            public ulong bytesToDownload;
            public float percentage;
        }

        public virtual void OnProgressCallback(ProgressEventInfo info)
        {
            ProgressCallback?.Invoke(this, info);
        }
    }
}

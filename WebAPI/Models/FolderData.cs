using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;


namespace WebAPI.Models
{
    public class Entry
    {
        public bool DirFlag { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public Entry(bool DirFlag, string Name, long Size)
        {
            this.DirFlag = DirFlag;
            this.Name = Name;
            this.Size = Size;
        }
    }

        public class FolderData
        {
            public string CurrentPath { get; set; }
            public List<Entry> entries;
            private long LeftBound { get; set; }
            private long RightBound { get; set; }

            public FolderData(string targetPath, long leftBound = 10 * 1024 * 1024, long rightBound = 100 * 1024 * 1024)
            {
                this.CurrentPath = targetPath;
                this.entries = new List<Entry> { };
                this.LeftBound = leftBound;
                this.RightBound = rightBound;

                foreach (string directory in Directory.GetDirectories(targetPath))
                {
                    Entry entry = new Entry(true, directory.Replace(targetPath,""), 0);
                    this.entries.Add(entry);
                };

                foreach (FileInfo file in new DirectoryInfo(targetPath).GetFiles())
                {
                    Entry entry = new Entry(false, file.Name, file.Length);
                    this.entries.Add(entry);
                    if (entry.Size < LeftBound) { SmallSizeFiles += 1; }
                    else if (entry.Size > RightBound) { BigSizeFiles += 1; }
                    else MiddleSizeFiles += 1;
                }
            }

            public int SmallSizeFiles { get; set; }
            public int MiddleSizeFiles { get; set; }
            public int BigSizeFiles { get; set; }
        }
}
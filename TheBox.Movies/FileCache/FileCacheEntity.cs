using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBox.Movies.FileCache
{
    /// <summary>
    /// File Cache Entity
    /// </summary>
    public class FileCacheEntity
    {
        /// <summary>
        /// Gets or sets the full name of the path and.
        /// </summary>
        public string FullPathAndName { get; set; }

        /// <summary>
        /// Gets or sets the file name only.
        /// </summary>
        public string FileNameOnly { get; set; }

        /// <summary>
        /// Gets or sets the file parts.
        /// </summary>
        public string[] FileParts { get; set; }
    }
}

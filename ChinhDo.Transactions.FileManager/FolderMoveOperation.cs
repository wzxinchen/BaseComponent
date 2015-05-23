using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChinhDo.Transactions
{
    public class FolderMoveOperation : IRollbackableOperation
    {
        private readonly string sourceFolderName;
        private readonly string destFolderName;

        /// <summary>
        /// Instantiates the class.
        /// </summary>
        /// <param name="sourceFolderName">The name of the file to move.</param>
        /// <param name="destFolderName">The new path for the file.</param>
        public FolderMoveOperation(string sourceFolderName, string destFolderName)
        {
            this.sourceFolderName = sourceFolderName;
            this.destFolderName = destFolderName;
        }

        public void Execute()
        {
            Directory.Move(sourceFolderName, destFolderName);
        }

        public void Rollback()
        {
            Directory.Move(destFolderName, sourceFolderName);
        }
    }
}

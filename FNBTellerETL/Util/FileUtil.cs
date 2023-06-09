using System;
using System.Collections.Generic;
using System.IO;

namespace FNBTellerETL.Util
{
    internal static class FileUtil
    {

        /// <summary>
        /// Given a path to a directory and a file name to look for within the directory, this method returns the full file path
        /// for the given file. If no file is found, null is returned.
        /// <para>An exception is thrown if the provided directory does not exist</para>
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="fname"></param>
        /// <returns></returns>
        public static string GetFileByName(string dirPath, string fname)
        {
            if(!Directory.Exists(dirPath))
                throw new ApplicationException($"The provided directory path {dirPath} does not exist.");

            var fullFilePath = Path.Combine(dirPath, fname);

            if (File.Exists(fullFilePath))
                return fullFilePath;
            else
                return null;
        }

        /// <summary>
        /// Returns a list of files (including their paths) located in the specified directory where the Created Date of the file 
        /// is between (inclusive) the @from and @to parameters provided.
        /// <para>An exception is thrown if the directory does not exist</para>
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="from">Minimum DateTime to look for a Created File</param>
        /// <param name="to">Maximum DateTime to look for a Created File</param>
        /// <returns></returns>
        public static List<string> GetFilesBetweenDates(string dirPath, DateTime from, DateTime to)
        {
            if (!Directory.Exists(dirPath))
                throw new ApplicationException($"The provided directory path {dirPath} does not exist.");

            var retVal = new List<string>();

            var files = Directory.GetFiles(dirPath);
            foreach (var fname in files)
            {
                var finfo = new FileInfo(fname);
                if (finfo.CreationTime >= from && finfo.CreationTime <= to)
                {
                    retVal.Add(fname);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Copies source file to destination directory. Optional overwrite parameter, default is false.
        /// <para>This method does error checking to see if source file exists and destination directory exists.</para>
        /// <para>An exception will be thrown if overwrite is false and the file exists in the destination dir</para>
        /// </summary>
        /// <param name="fullPathToSrc">Full filepath to source file</param>
        /// <param name="fullPathToDestDir">Full path to destination directory</param>
        /// <param name="overwrite">optional boolean overwrite flag</param>
        /// <returns>True if copy is successful, false otherwise.</returns>
        public static bool CopyFile(string fullPathToSrc, string fullPathToDestDir, bool overwrite = false)
        {
            if (!File.Exists(fullPathToSrc) || !Directory.Exists(fullPathToDestDir))
            {
                return false;
            }

            string fname = Path.GetFileName(fullPathToSrc);
            string fullPathToDest = Path.Combine(fullPathToDestDir, fname);

            File.Copy(fullPathToSrc, fullPathToDest, overwrite);
            return true;
        }

        /// <summary>
        /// Moves source file to destination directory.
        /// <para>This method does error checking to see if source file exists and destination directory exists.</para>
        /// <para>An exception will be thrown if the file exists in the destination directory, because of this we return false</para>
        /// </summary>
        /// <param name="fullPathToSrc">Full filepath to source file</param>
        /// <param name="fullPathToDestDir">Full path to destination directory</param>
        /// <returns>True if move is successful, false otherwise.</returns>
        public static bool MoveFile(string fullPathToSrc, string fullPathToDestDir)
        {
            if (!File.Exists(fullPathToSrc) || !Directory.Exists(fullPathToDestDir))
            {
                return false;
            }

            string fname = Path.GetFileName(fullPathToSrc);
            string fullPathToDest = Path.Combine(fullPathToDestDir, fname);

            //since we currently dont handle overwrite's return false if the source file exists at destination
            if (File.Exists(fullPathToDest))
            {
                return false;
            }

            //TODO: Maybe implement overwrite capabilities if that's ever needed. Couldn't find an overwrite method with File.Move
            File.Move(fullPathToSrc, fullPathToDest);
            return true;
        }
    }
}

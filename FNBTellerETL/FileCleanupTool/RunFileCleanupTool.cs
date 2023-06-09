using FNBCoreETL.Framework;
using FNBTellerETL.Config;
using FNBTellerETL.Util;
using System;
using System.Collections.Generic;
using System.IO;

namespace FNBTellerETL.FileCleanupTool
{

    /// <summary>
    ///  Deletes all files in folders in App.Config FileCleanupTool.FoldersToClean
    ///  
    /// Functions via spliting list into each Directory
    /// Then Listing the contents of the directory where older than X Date
    /// Thoes OLD files are then sent to delete method where they are deleted permently
    /// 
    /// NOTE: THIS IS PERMANENT
    ///       no recycle bin just gone
    ///       
    /// if you need to test, comment out actual delete line in DeleteFiles method, (use FakeDelete insted)
    /// </summary>


    public static class RunFileCleanupTool
    {
        public static ETLJobOut Go(ETLJobIn job)
        {

            DateTime cutoffDateTime = new DateTime(1970,1,1);

            switch (job.ModeName)
            {
                case "Normal":
                    cutoffDateTime = DateTime.Now.AddDays(-90);
                    break;
                case "Past30Days":
                    cutoffDateTime = DateTime.Now.AddDays(-30);
                    break;
                case "Past7Days":
                    cutoffDateTime = DateTime.Now.AddDays(-7);
                    break;
                case "DateRange":
                    cutoffDateTime = DateUtil.customParseDateTime(job.ArgumentsByName["Older"]);
                    break;
                default:
                    break;
            }

            var mainOldFileList = new List<string>();

            foreach (var fileDirPath in FileConfiguration.FileCleanupTool.FileCleanupDirs)
            {
                if (Directory.Exists(fileDirPath))
                {
                    mainOldFileList.AddRange(ListDirContentOlderThen(cutoffDateTime, fileDirPath));
                }
                else
                {
                    FNBCoreETL.Logging.Logger.JobLog.LogInfo(0, 0, job, $"Provided directory to clean {fileDirPath}, does not exist... skipping directory");
                }
            }

            var numDeletedFiles = DeleteFiles(mainOldFileList);

            return new ETLJobOut(numDeletedFiles, $"Deleted a total of {numDeletedFiles} files from these across folders: {String.Join(", ", FileConfiguration.FileCleanupTool.FileCleanupDirs)}");
        }



        private static IEnumerable<string> ListDirContentOlderThen(DateTime olderDate, string dirFolderPath)
        {
            string[] fileEntries = Directory.GetFiles(dirFolderPath);
            var OldFiles = new List<string>();

            foreach (string fileName in fileEntries)
            {
                var fileInfo = new FileInfo(fileName);
                if(fileInfo.LastAccessTime < olderDate)
                {
                    OldFiles.Add(fileName);
                }
            }

            return OldFiles;
        }

        private static int DeleteFiles(IEnumerable<string> oldFilesList)
        {
            int deletedFileCount = 0;
            foreach(var filePath in oldFilesList)
            {
                //Console.WriteLine("FAKE DELETE: " + filePath);

                File.Delete(filePath);

                deletedFileCount++;
            }

            return deletedFileCount;
        }
    }
}

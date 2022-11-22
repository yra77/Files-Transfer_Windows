

using WindowsLocalNetwork.Constants;
using WindowsLocalNetwork.Interfaces;
using WindowsLocalNetwork.Services.GoogleServices;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace WindowsLocalNetwork.Services
{

    internal class CloudReceive
    {

        public event ProgressBardelegate progressBarEvent;
        public event ProgressChangeDelegate progressChangeEvent;
        public event Error_Text_CallBack textErrorEvent;

        private int _countFilesNow;
        private int _countFiles;
        private IRemoteWork _googleDrive;
        private Split_Merge_File _split_Merge;


        public async void StartReceive(Split_Merge_File split_Merge)
        {
                _googleDrive = new GoogleDrive_Work();
                _split_Merge = split_Merge;

                await ReceiveFilesCount_Async();
            await ReceiveFiles_Async();
        }

        private async Task ReceiveFiles_Async()
        {
            int countParstFile_Real = -1;
            int countParstFile_Now = 0;

            do
            {
               (string, string) tuple = await WaitFile_In_Cloud_Async();

                string folderPath = "";

                if(tuple.Item1.Split('.').Last<string>() == "myf" || tuple.Item1 == "Info.txt")
                {
                    folderPath = Split_Merge_File.folderPath;
                    countParstFile_Now++;
                }
                else
                {
                    folderPath = Split_Merge_File.folderPath + "FullFiles\\";
                    _countFilesNow--;
                }

                progressBarEvent(false);

                _googleDrive.DownloadFromDrive(tuple.Item1, tuple.Item2, folderPath);
                _googleDrive.DeleteFile(tuple.Item2);

               
                if(tuple.Item1 == "Info.txt")
                {
                    countParstFile_Now--;
                    var fileInfo = _split_Merge.ReadFileInfo();
                    countParstFile_Real = 1 + fileInfo.PartsCount;
                }

                if(countParstFile_Now == countParstFile_Real)
                {
                    _split_Merge.MergeFiles();
                    countParstFile_Now = 0;
                    countParstFile_Real = 0;
                    _countFilesNow--;
                }

                double percentage = (double)(_countFiles - _countFilesNow) * 100.0 / _countFiles;
                progressChangeEvent(percentage, false);

            } while (_countFilesNow > 0);

            progressChangeEvent(0.0, true);
            Console.WriteLine("Downloading End");
        }

        private async Task ReceiveFilesCount_Async()
        {
            (string, string) tuple = await WaitFile_In_Cloud_Async();
           
            if (tuple.Item1 == "FullCount.txt")
            {
                _googleDrive.DownloadFromDrive(tuple.Item1, tuple.Item2, Split_Merge_File.folderPath + "FullFiles\\");

                _countFilesNow = Int16.Parse(File.ReadAllText(Split_Merge_File.folderPath + "FullFiles\\" + "FullCount.txt"));
                _countFiles = _countFilesNow;
                _googleDrive.DeleteFile(tuple.Item2);
            }
        }

        private bool IsEmptyFolderInCloud(ref IEnumerable<Google.Apis.Drive.v3.Data.File> driveList)
        {
            driveList = _googleDrive.GetFilesList(Constant.FOLDER_IN_GOOGLE_DRIVE);

            if (driveList != null && driveList.Count() > 0)
            {
                return true;
            }
            return false;
        }

        private async Task<(string, string)> WaitFile_In_Cloud_Async()
        {
            IEnumerable<Google.Apis.Drive.v3.Data.File> files = new List<Google.Apis.Drive.v3.Data.File>();
            
            do//checking google drive is empty
            {
                await Task.Delay(60000);
            } while (!IsEmptyFolderInCloud(ref files));

                var tuple = (files.ElementAtOrDefault(0).Name, files.ElementAtOrDefault(0).Id);
          
                return tuple;
        }

        private IEnumerable<Google.Apis.Drive.v3.Data.File> GetList_GoogleDrive()
        {
            return _googleDrive.GetFilesList(Constant.FOLDER_IN_GOOGLE_DRIVE);
        }

    }
}

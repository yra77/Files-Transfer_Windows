

using WindowsLocalNetwork.Constants;
using WindowsLocalNetwork.Interfaces;
using WindowsLocalNetwork.Services.GoogleServices;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace WindowsLocalNetwork.Services
{
    internal class CloudSend
    {
        public event ProgressBardelegate progressBarEvent;
        public event ProgressChangeDelegate progressChangeEvent;
        public event Error_Text_CallBack textErrorEvent;

        private static string _countFiles;
        private List<(string, string)> _bigFiles;
        private List<(string, string)> _smallFiles;
        private IRemoteWork _googleDrive;
        private Split_Merge_File _split_Merge;


        public async void StartSending_Async(string[] fileList, Split_Merge_File split_Merge)
        {
            _bigFiles = new List<(string, string)>();
            _smallFiles = new List<(string, string)>();
            _googleDrive = new GoogleDrive_Work();
            _split_Merge = split_Merge;

            _googleDrive.EventProgressCloudBytes += ProgressCloud;

            _countFiles = fileList.Length.ToString();

            CheckFiles(fileList);

            await Send_TXT_Async();

            if (await SendSmallFiles_Async() && (await SendBigFiles_Async()))
            {
                Console.WriteLine("CloudSend Send is complete");
                progressChangeEvent(0.0, true);
            }
            else
            {
                textErrorEvent("Send small files Error");
            }
        }

        private void CheckFiles(string[] fileList)
        {
            foreach (var file in fileList)
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Exists)
                {
                    if (fileInfo.Length > 999999999)
                    {
                        var tuple = (file, fileInfo.Name);
                        _bigFiles.Add(tuple);
                        continue;
                    }
                    else
                    {
                        var tuple = (file, fileInfo.Name);
                        _smallFiles.Add(tuple);
                        continue;
                    }
                }
            }
        }

        private async Task<bool> SendSmallFiles_Async()
        {
            foreach (var file in _smallFiles)
            {
                await Wait_Empty_Cloud_Async();//checking google drive is empty

                _googleDrive.UploadToDrive(file.Item1, file.Item2);
            }
            return true;
        }

        private async Task<bool> SendBigFiles_Async()
        {

            DirectoryInfo folder = new DirectoryInfo(Split_Merge_File.folderPath);

            foreach (var file in _bigFiles)
            {
                await Wait_Empty_Cloud_Async();//checking google drive is empty

                if (_split_Merge.BigFileSplit(file.Item1))
                {
                    progressBarEvent(false);
                    FileInfo[] files = folder.GetFiles();//List with parts file

                    foreach (var item in files)
                    {
                        await Wait_Empty_Cloud_Async();
                        _googleDrive.UploadToDrive(Split_Merge_File.folderPath + item.Name, item.Name);
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private async Task Send_TXT_Async()//send file with files count
        {
            File.WriteAllText(Split_Merge_File.folderPath + "FullCount.txt", _countFiles);

            await Wait_Empty_Cloud_Async();//checking google drive is empty
            _googleDrive.UploadToDrive(Split_Merge_File.folderPath + "FullCount.txt", "FullCount.txt");

            File.Delete(Split_Merge_File.folderPath + "FullCount.txt");
        }

        private bool IsEmptyFolderInCloud()
        {
            var driveList = _googleDrive.GetFilesList(Constant.FOLDER_IN_GOOGLE_DRIVE);
            if (driveList != null && driveList.Count() > 0)
            {
                return true;
            }
            return false;
        }

        private async Task Wait_Empty_Cloud_Async()
        {
            do//checking google drive is empty
            {
                await Task.Delay(60000);
            } while (IsEmptyFolderInCloud());
        }

        private void ProgressCloud(long bytes, long size)
        {
            double percentage = (double)bytes * 100.0 / size;
            progressChangeEvent(percentage, false);
        }
    }
}

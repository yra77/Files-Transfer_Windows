

using WindowsLocalNetwork.Interfaces;
using WindowsLocalNetwork.Constants;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

using System;
using System.Collections.Generic;
using System.IO;


namespace WindowsLocalNetwork.Services.GoogleServices
{
    internal class GoogleDrive_Work : IRemoteWork
    {

        public event Delegate_ProgressCloudBytes EventProgressCloudBytes;

        private long _sizeFile;

        public void DownloadFromDrive(string filename, string fileId, string savePath)
        {
           // string mimeType = "application/unknown";
           
            try
            {
                if (Path.HasExtension(filename))
                {
                    var service = GetService();
                    var request = service.Files.Get(fileId);

                    var stream = new System.IO.MemoryStream();

                    Console.WriteLine(fileId);

                    request.MediaDownloader.ProgressChanged += DownloadFromDrive_ProgressChanged;//progress bytes

                    request.Download(stream);
                    ConvertMemoryStreamToFileStream(stream, savePath + @"\" + @filename);

                    stream.Dispose();
                }
            }
            catch (Exception exc)
            {
               Console.WriteLine(exc.Message + " Download From Drive Error");
            }
        }

        public IEnumerable<Google.Apis.Drive.v3.Data.File> GetFilesList(string folder)
        {
            var service = GetService();

            var fileList = service.Files.List();
            fileList.Q = $"mimeType!='application/vnd.google-apps.folder' and '{folder}' in parents";
            fileList.Fields = "nextPageToken, files(id, name, size, mimeType)";

            var result = new List<Google.Apis.Drive.v3.Data.File>();
            string pageToken = null;
            do
            {
                fileList.PageToken = pageToken;
                var filesResult = fileList.Execute();
                var files = filesResult.Files;
                pageToken = filesResult.NextPageToken;
                result.AddRange(files);
            } while (pageToken != null);

           return result;
        }

        public void DeleteFile(string fileId)
        {
            var service = GetService();
            var command = service.Files.Delete(fileId);
            var result = command.Execute();
            Console.WriteLine("Delete File " + result);
        }

        public string CreateFolder(string parent, string folderName)
        {
            var service = GetService();
            var driveFolder = new Google.Apis.Drive.v3.Data.File();
            driveFolder.Name = folderName;
            driveFolder.MimeType = "application/vnd.google-apps.folder";
            driveFolder.Parents = new string[] { parent };
            var command = service.Files.Create(driveFolder);
            var file = command.Execute();
            return file.Id;
        }

        public string UploadToDrive(string filePath, string nameFileInGoogleDrive)
        {
           // string mimeType = "application/unknown";
            try
            {
                var service = GetService();

                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = nameFileInGoogleDrive,
                    Parents = new List<string>() { Constant.FOLDER_IN_GOOGLE_DRIVE }
                };

                // Create a new file on drive
                FilesResource.CreateMediaUpload request;
              
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    request = service.Files.Create(fileMetadata, stream, "application/unknown");
                    request.Fields = "id";

                    _sizeFile = stream.Length;

                    request.ProgressChanged += UploadToDrive_ProgressChanged;//progress bytes

                    var response = request.Upload();

                    if (response.Status != Google.Apis.Upload.UploadStatus.Completed)
                    {
                        throw response.Exception;
                    }
                }
                
                var file = request.ResponseBody;

                Console.WriteLine("Upload File ID: " + file.Id);
                return file.Id;
            }
            catch (Exception e)
            {
                if (e is AggregateException)
                {
                    Console.WriteLine("Credential Not found");
                }
                else if (e is FileNotFoundException)
                {
                    Console.WriteLine("File not found");
                }
                else
                {
                    throw;
                }
            }
            return null;
        }

        private DriveService GetService()
        {
            GoogleCredential credential = GoogleCredential.FromFile
                (Constant.BASE_FOLDER + "\\logical-grammar-369211-fddf30bb18f0.json")//GetApplicationDefault()
                   .CreateScoped(DriveService.Scope.Drive);

            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Files transfer"
            });

            return service;
        }

        private void ConvertMemoryStreamToFileStream(MemoryStream stream, string savePath)
        {
            FileStream fileStream;
            using (fileStream = new System.IO.FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                try
                {
                    // System.IO.File.Create(saveFile)
                    stream.WriteTo(fileStream);
                    fileStream.Close();
                }
                catch (Exception)
                {
                }
            }
        }

        private void DownloadFromDrive_ProgressChanged(IDownloadProgress obj)
        {
            switch (obj.Status)
            {
                case DownloadStatus.Downloading:
                    {
                        Console.WriteLine(obj.BytesDownloaded);
                       // EventProgressCloudBytes(obj.BytesDownloaded, _sizeFile);
                        break;
                    }
                case DownloadStatus.Completed:
                    {
                        Console.WriteLine("Download complete.");
                        break;
                    }
                case DownloadStatus.NotStarted:
                    {
                        Console.WriteLine("Download not Started.");
                        break;
                    }
                case DownloadStatus.Failed:
                    {
                        Console.WriteLine("Download failed.");
                        break;
                    }
            }
        }

        private void UploadToDrive_ProgressChanged(Google.Apis.Upload.IUploadProgress obj)
        {
            switch (obj.Status)
            {
                case Google.Apis.Upload.UploadStatus.NotStarted:
                    Console.WriteLine("Upload not Started.");
                    break;
                case Google.Apis.Upload.UploadStatus.Starting:
                    Console.WriteLine("Upload Starting.");
                    break;
                case Google.Apis.Upload.UploadStatus.Uploading:
                    Console.WriteLine(obj.BytesSent);
                    EventProgressCloudBytes(obj.BytesSent, _sizeFile);
                    break;
                case Google.Apis.Upload.UploadStatus.Completed:
                    Console.WriteLine("Upload complete.");
                    break;
                case Google.Apis.Upload.UploadStatus.Failed:
                    Console.WriteLine("Upload failed.");
                    break;
                default:
                    break;
            }
        }
    }
}



using System.Collections.Generic;


namespace WindowsLocalNetwork.Interfaces
{
    internal interface IRemoteWork
    {

        event Delegate_ProgressCloudBytes EventProgressCloudBytes;

        void DownloadFromDrive(string filename, string fileId, string savePath);
        IEnumerable<Google.Apis.Drive.v3.Data.File> GetFilesList(string folder);
        void DeleteFile(string fileId);
        string CreateFolder(string parent, string folderName);
        string UploadToDrive(string filePath, string nameFileInGoogleDrive);
    }
}

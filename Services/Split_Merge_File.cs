

using WindowsLocalNetwork.Models;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace WindowsLocalNetwork.Services
{
    internal class Split_Merge_File
    {

        public static string folderPath;

        public event ProgressBardelegate progressBarEvent;
        public event ProgressChangeDelegate progressChangeEvent;
        public event Error_Text_CallBack textErrorEvent;


        static Split_Merge_File()
        {
            FolderExist();
        }

        public void MergeFiles()
        {

            string[] files = System.IO.Directory.GetFiles(folderPath, "*.myf");
            SplitMerge_Info filesInfo = ReadFileInfo();//read info from file


            if (files != null && files.Length > filesInfo.PartsCount)
            {
                using (FileStream fsWrite = new FileStream(folderPath + "FullFiles\\" + filesInfo.Name, FileMode.Create, FileAccess.Write))
                {
                    progressBarEvent(false);
                    long sizeFile = filesInfo.Size;
                    long loadingByte = 0;

                    foreach (var item in files)
                    {

                        using (var inStream = System.IO.File.OpenRead(item))
                        {

                            long remaining = inStream.Length;

                            do
                            {
                                byte[] buffer = new byte[81920];
                                int bytesRead = inStream.Read(buffer, 0, (int)Math.Min(buffer.Length, remaining));

                                if (bytesRead == 0)
                                {
                                    break;
                                }

                                fsWrite.Write(buffer, 0, bytesRead);

                                remaining -= bytesRead;
                                loadingByte += bytesRead;

                                double percentage = (double)loadingByte * 100.0 / sizeFile;
                                progressChangeEvent(percentage, false);
                            }
                            while (remaining > 0);
                        }
                    }
                }

                if (GetHash(folderPath + "FullFiles\\" + filesInfo.Name) == filesInfo.Hash)
                {
                    progressChangeEvent(0.0, true);
                     ClearFolder();
                }
                else
                {
                    textErrorEvent("The file was written with errors. Start again.");
                }
            }
            else
            {
                textErrorEvent("File not saved.");
            }
        }


        public bool BigFileSplit(string inputFilePath)
        {
            ClearFolder();

            using (var inStream = System.IO.File.OpenRead(inputFilePath))
            {

                inStream.Seek(0, SeekOrigin.Begin);

                long remaining = inStream.Length;
                long countBytes = 0;
                int countFiles = (int)(remaining / 491520000);

               Task task = Task.Run(() =>
                {
                    SaveFileInfo(remaining, countFiles, inputFilePath);//create Info.txt
                });

                progressBarEvent(false);

                for (int i = 0; i <= countFiles; i++)
                {

                    using (var outStream = System.IO.File.OpenWrite(folderPath + i + ".myf"))
                    {
                        do
                        {

                            byte[] buffer = new byte[81920];
                            int bytesRead = inStream.Read(buffer, 0, (int)Math.Min(buffer.Length, remaining));

                            if (bytesRead == 0)
                            {
                                break;
                            }

                            outStream.Write(buffer, 0, bytesRead);

                            remaining -= bytesRead;
                            countBytes += bytesRead;

                            if (countBytes % 491520000 == 0)
                            {
                                break;
                            }
                        }
                        while (remaining > 0);
                    }

                    double percentage = (double)i * 100.0 / countFiles;
                    progressChangeEvent(percentage, false);
                }
               task.Wait();
            }
            progressChangeEvent(0.0, true);
            return true;
        }

        public SplitMerge_Info ReadFileInfo()
        {
            string[] lines = System.IO.File.ReadAllLines(folderPath + "Info.txt");

            SplitMerge_Info splitMerge = new SplitMerge_Info()
            {
                PartsCount = Int16.Parse(lines[0]),
                Size = Int64.Parse(lines[1]),
                Name = lines[2],
                Hash = lines[3]
            };

            return splitMerge;
        }

        private void SaveFileInfo(long size, int partsCount, string name)
        {

            string hashFile = GetHash(name);

            name = name.Split('/', '\\').Last<string>();

            string[] lines =
            {
               partsCount.ToString(), size.ToString(), name, hashFile
            };

            File.WriteAllLines(folderPath + "Info.txt", lines);
        }

        private string GetHash(string fileName)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(fileName))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "");
                }
            }
        }

        private void ClearFolder()
        {
            DirectoryInfo folder = new DirectoryInfo(folderPath);
            foreach (FileInfo file in folder.GetFiles())
            {
                file.Delete();
            }
        }

        private static void FolderExist()
        {
            folderPath = Environment.GetFolderPath
                                 (Environment.SpecialFolder.Desktop) + "\\" + "FilesTranferParts\\";
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
                System.IO.Directory.CreateDirectory(folderPath + "FullFiles");
            }
        }
    }
}

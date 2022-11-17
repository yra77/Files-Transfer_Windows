
using WindowsLocalNetwork.Models;
using WindowsLocalNetwork.Helpers;

using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace WindowsLocalNetwork.Services
{
    public class Server
    {

        public static string fileName;

        public event ProgressBardelegate progressBarEvent;
        public event ProgressChangeDelegate progressChangeEvent;
        public event Error_Text_CallBack textErrorEvent;
        public event Server_Text_CallBack server_Text_Event;
        public event ListDevice_CallBack listDevice_Event;


        public Server()
        {
            Task task = Task.Factory.StartNew(() =>
            {
                ReceiveFile();
            });
        }

        public async void Start()
        {

            TcpClient server = default(TcpClient);
            TcpListener listener;

            listener = new TcpListener(GetIP.MyIP, 1234);
            listener.Start();

            server_Text_Event("IP: " + GetIP.MyIP + "  Name: " + GetIP.MyHOST);

            while (true)
            {

                try
                {
                    server = await listener.AcceptTcpClientAsync();

                    const int bytesize = 1024 * 1024;
                    byte[] buffer = new byte[bytesize];
                    string x = server.GetStream().Read(buffer, 0, bytesize).ToString();
                    var data = ASCIIEncoding.ASCII.GetString(buffer);

                    if (data != null && data != " ")
                    {
                        listDevice_Event(data);
                        // Console.WriteLine("Message from client - " + data.Length);
                    }
                }
                catch (Exception)
                {
                    break;
                }
            }

            server.Dispose();
            server.Close();
            listener.Stop();
            Console.WriteLine("Message to server ERROR - ");
            Start();
        }

        public async void ReceiveFile()
        {

            TcpClient server = default(TcpClient);
            TcpListener listener;

            listener = new TcpListener(GetIP.MyIP, 1235);
            listener.Start();

            while (true)
            {

                try
                {

                    server = await listener.AcceptTcpClientAsync();

                    byte[] lenBytes = new byte[112];

                    if (server.Client.Receive(lenBytes) < 112)
                        return;

                    Info_File myStruct2 = new Info_File();
                    myStruct2 = FromBytes(lenBytes);

                    fileName = myStruct2.File_Name;
                    long len = myStruct2.File_Size;

                    using (FileStream fs = File.Create(Environment.GetFolderPath
                                          (Environment.SpecialFolder.Desktop) + "\\" + fileName))
                    {
                        progressBarEvent(false);
                        long loadingByte = 0;

                        while (fs.Position < len)
                        {
                            byte[] buffer = new byte[1024];
                            int bytesRead = server.Client.Receive(buffer);
                            if (bytesRead <= 0)
                                break;
                            fs.Write(buffer, 0, bytesRead);

                            loadingByte += bytesRead;
                            double percentage = (double)loadingByte * 100.0 / len;
                            progressChangeEvent(percentage, false);
                        }

                        if (File.Exists(Environment.GetFolderPath
                                          (Environment.SpecialFolder.Desktop) + "\\" + fileName))
                        {
                            progressChangeEvent(0.0, true);
                        }
                        else
                            textErrorEvent("File not saved.");
                    }

                    fileName = null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Receive error " + ex.Message);
                }
            }
        }

        public Info_File FromBytes(byte[] arr)
        {

            Info_File str = new Info_File();
            int size = Marshal.SizeOf(str);
            IntPtr ptr = IntPtr.Zero;

            try
            {
                ptr = Marshal.AllocHGlobal(size);

                Marshal.Copy(arr, 0, ptr, size);

                str = (Info_File)Marshal.PtrToStructure(ptr, str.GetType());
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return str;
        }

    }
}

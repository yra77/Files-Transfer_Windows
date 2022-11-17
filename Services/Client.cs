
using WindowsLocalNetwork.Models;
using WindowsLocalNetwork.Helpers;

using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace WindowsLocalNetwork.Services
{

    public class Client
    {

        public event Action event_EndCheckDevice;
        public event ProgressBardelegate progressBarEvent;
        public event ProgressChangeDelegate progressChangeEvent;

        public async void ClientConnect(string clientIP,
                                        string[] filePathList)
        {
            string[] tempList = filePathList;

            for (int i = 0; i < filePathList.Length; i++)
            {
                TcpClient client = new TcpClient();

                await client.ConnectAsync(clientIP, 1235);

                if (!client.Connected)
                {
                    Console.WriteLine("Client connection Error " + clientIP);
                    ClientConnect(clientIP, tempList);
                }
                else
                {

                    using (FileStream fs = File.OpenRead(filePathList[i]))
                    {
                        Info_File myStruct = new Info_File();

                        myStruct.File_Name = filePathList[i].Split('/', '\\').Last<string>();
                        myStruct.File_Size = (int)fs.Length;

                        byte[] lenBytes = ToByteArray(myStruct);

                        client.Client.Send(lenBytes);

                        progressBarEvent(true);
                        long loadingByte = 0;
                        long len = (int)fs.Length;

                        byte[] buffer = new byte[1024];
                        int bytesRead;
                        fs.Position = 0;

                        while ((bytesRead = fs.Read(buffer, 0, 1024)) > 0)
                        {
                            client.Client.Send(buffer, bytesRead, SocketFlags.None);

                            loadingByte += bytesRead;
                            double percentage = (double)loadingByte * 100.0 / len;
                            progressChangeEvent(percentage, false);
                        }

                        progressChangeEvent(0.0, true);
                        //remove the element this
                        tempList = filePathList.Where(e => e != filePathList[i]).ToArray();
                    }
                }
                client.Close();

            }
        }

        public byte[] ToByteArray(Info_File info_File)
        {

            int size = Marshal.SizeOf(info_File);

            byte[] arr = new byte[size];

            GCHandle h = default(GCHandle);

            try
            {
                h = GCHandle.Alloc(arr, GCHandleType.Pinned);

                Marshal.StructureToPtr(info_File, h.AddrOfPinnedObject(), false);
            }
            finally
            {
                if (h.IsAllocated)
                {
                    h.Free();
                }
            }

            return arr;
        }

        public void Check_Devices()
        {
            
            string[] temp = GetIP.MyIP.ToString().Split('.');
            string firstStr = temp[0] + "." + temp[1] + "." + temp[2] + ".";
            int last = Convert.ToInt16(temp[3]);

            if (last < 100)
                last = 0;
            else if (last > 100 && last < 200)
                last = 100;
            else
                last = 200;

            Parallel.For(1, 20, i =>
            {
                TcpClient client = new TcpClient();

                if (GetIP.MyIP.ToString() != firstStr + (i + last)
                    && Connect_Server(client, firstStr + (i + last)).Result)
                {
                    SendMessage(client, GetIP.MyIP.ToString(), GetIP.MyHOST);
                    // Console.WriteLine("192.168.0." + i.ToString() + " = " + Server.mashineIP);
                }
                else
                {
                    client.Close();
                }
            });

           // Console.WriteLine("Search end");

            event_EndCheckDevice.Invoke();
        }

        public void SendMessage(TcpClient client, string ip, string deviceName)
        {
            try
            {
                NetworkStream stream;
                stream = client.GetStream();//_client
                string s = ip + ";" + deviceName + ";";
                byte[] message = Encoding.ASCII.GetBytes(s);
                stream.Write(message, 0, message.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("Send message to client - " + e.Message);
            }
        }

        private async Task<bool> Connect_Server(TcpClient client, string clientIP)
        {

            try
            {
                // client.SendTimeout = 100;
                await client.ConnectAsync(clientIP, 1234);

                if (client.Connected)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception x)
            {
                Console.WriteLine("Connection failed!" + x.Message);
                return false;
            }
        }

    }
}

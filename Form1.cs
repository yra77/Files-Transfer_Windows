


using WindowsLocalNetwork.Services;
using WindowsLocalNetwork.Helpers;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Media;


namespace WindowsLocalNetwork
{

    public delegate void Server_Text_CallBack(string taskResult);
    public delegate void Error_Text_CallBack(string taskResult);
    public delegate void ListDevice_CallBack(string deviceList);
    public delegate void ProgressBardelegate(bool isSending);
    public delegate void ProgressChangeDelegate(double percentage, bool isComplete);
    delegate void Delegate_ProgressCloudBytes(long bytes, long size);


    public partial class Form1 : Form
    {

        private List<Tuple<string, string>> _deviceList;
        private Thread _threadClient;
        private Server _server;
        private Client _client;
        private Split_Merge_File _split_Merge;
        private CloudSend _cloudSend;
        private CloudReceive _cloudReceive;


        public Form1()
        {
            InitializeComponent();

            this.FormClosing += Form1_FormClosing;

            _deviceList = new List<Tuple<string, string>>();

            progressBar1.Hide();
            label2.Hide();

            CheckingEthernet.TextErrorEvent += ErrorText_Callback;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                Start();
                //await Task.Delay(5000);
                //AuthGoogle authGoogle = new AuthGoogle();
                //authGoogle.StartAuth_Async();
            });
        }

        private async void Start()
        {
            _server = new Server();
            _client = new Client();
            _split_Merge = new Split_Merge_File();
            _cloudSend = new CloudSend();
            _cloudReceive = new CloudReceive();

            _server.server_Text_Event += ServerText_Callback;
            _server.listDevice_Event += ClientText_Callback;
            _server.textErrorEvent += ErrorText_Callback;
            _server.progressBarEvent += ProgressBarVisible;
            _server.progressChangeEvent += ProgressBarChange;

            _client.progressBarEvent += ProgressBarVisible;
            _client.progressChangeEvent += ProgressBarChange;
            _client.event_EndCheckDevice += EndCheckDevice;

            _split_Merge.textErrorEvent += ErrorText_Callback;
            _split_Merge.progressBarEvent += ProgressBarVisible;
            _split_Merge.progressChangeEvent += ProgressBarChange;

            _cloudSend.textErrorEvent += ErrorText_Callback;
            _cloudSend.progressBarEvent += ProgressBarVisible;
            _cloudSend.progressChangeEvent += ProgressBarChange;

            _cloudReceive.textErrorEvent += ErrorText_Callback;
            _cloudReceive.progressBarEvent += ProgressBarVisible;
            _cloudReceive.progressChangeEvent += ProgressBarChange;

            // is disable Wi-Fi connection.
            await CheckingEthernet.IsConnections();
            await Task.Delay(1500);//задержка нужна

            Thread threadServer = new Thread(() =>
            {
                _server.Start();
            });

            threadServer.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _threadClient = new Thread(_client.Check_Devices);
            _threadClient.Start();

            button1.Text = "Update";
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ip = listBox1.Text.Split(' ');

            string[] filePathList = OpenFileDialog();

            if (filePathList != null && filePathList.Length > 0)
            {
                ClientConnect(ip[0], filePathList);
            }
        }

        private string[] OpenFileDialog()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Multiselect = true;
            openFileDialog1.ShowDialog();

            return openFileDialog1.FileNames;
        }

        public void ServerText_Callback(string str)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    this.label1.Text = str;
                }));
            }
        }

        public void ErrorText_Callback(string str)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    this.label3.Text = str;
                    OnePing();
                }));
            }
        }

        public void ClientText_Callback(string deviceList)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    string[] name_IP = deviceList.Split(';');

                    if (name_IP[0] != null && name_IP[1] != null)
                    {
                        _deviceList.Add(new Tuple<string, string>(name_IP[0], name_IP[1]));

                        listBox1.Items.Add(name_IP[0] + " " + name_IP[1]);
                        //listBox1.EndUpdate();
                    }
                    else
                        Console.WriteLine(deviceList);
                }));
            }
        }

        private void ProgressBarVisible(bool isSending)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    if (isSending)
                        label2.Text = "Sending file";
                    else
                        label2.Text = "Loading file";

                    label2.Visible = true;
                    progressBar1.Visible = true;
                }));
            }
        }

        private void ProgressBarChange(double percentage, bool isComplete)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    progressBar1.Value = (int)percentage;

                    if (isComplete)
                    {
                        progressBar1.Hide();
                        label2.ForeColor = System.Drawing.Color.Green;
                        label2.Text = "Downloading End";
                        OnePing();
                    }
                }));
            }
        }

        private void ClientConnect(string clientIP, string[] filePathList)
        {
            if (_threadClient != null)
                _threadClient.Abort();

            Thread threadClient = new Thread(() =>
            {
                _client.ClientConnect(clientIP, filePathList);
            });

            threadClient.Start();
        }

        private void EndCheckDevice()
        {
            _threadClient.Abort();
        }

        public void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Windows.Forms.Application.ExitThread();
            System.Environment.Exit(0);
        }

        private void SplitFiles_Click(object sender, EventArgs e)
        {
            string[] inputFilePath = OpenFileDialog();

            if (inputFilePath != null && inputFilePath.Length > 0)
            {
                Task.Run(() =>
                {
                    _split_Merge.BigFileSplit(inputFilePath[0]);
                });
            }
        }

        private void MergeFiles_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                _split_Merge.MergeFiles();
            });
        }

        private void SendFiles_Click(object sender, EventArgs e)
        {

            string[] inputFilePath = OpenFileDialog();

            if (inputFilePath != null && inputFilePath.Length > 0)
            {
                Task.Run(() =>
                {
                    _cloudSend.StartSending_Async(inputFilePath, _split_Merge);
                });
            }
        }

        private void ReceiveFiles_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                _cloudReceive.StartReceive(_split_Merge);
            });
        }

        private void OnePing()
        {
            SystemSounds.Beep.Play();
        }

    }
}

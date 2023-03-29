using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.VisualStyles;

namespace AplikasiChat_20SA1106
{
    public partial class Form2 : Form
    {
        private TcpClient client;
        private StreamReader STR;
        private StreamWriter STW;
        public string receive;
        public string textSend;

        public Form2()
        {
            InitializeComponent();

            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress address in localIP)
            {
                if(address.AddressFamily== AddressFamily.InterNetwork)
                {
                    txtIPC.Text = address.ToString();
                }
            }

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            TcpListener listener = new TcpListener(IPAddress.Any,int.Parse(txtPortS.Text));
            listener.Start();   
            client = listener.AcceptTcpClient();   
            STR = new StreamReader(client.GetStream()); 
            STW = new StreamWriter(client.GetStream()); 
            STW.AutoFlush = true;

            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.WorkerSupportsCancellation= true;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            client = new TcpClient();
            IPEndPoint IP_End = new IPEndPoint(IPAddress.Parse(txtIPC.Text), int.Parse(txtPortC.Text));
            try
            {
                client.Connect(IP_End);
                if(client.Connected)
                {
                    txtChat.AppendText("Connected to server "+"\n");
                    STR = new StreamReader(client.GetStream());
                    STW = new StreamWriter(client.GetStream());
                    STW.AutoFlush = true;

                    backgroundWorker1.RunWorkerAsync();
                    backgroundWorker2.WorkerSupportsCancellation = true;
                }
            }

            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if(txtPesan.Text != "")
            {
                textSend = txtPesan.Text;
                backgroundWorker2.RunWorkerAsync();

                txtPesan.Text = "";
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    receive = STR.ReadLine();
                    this.txtChat.Invoke(new MethodInvoker(delegate ()
                    {
                        txtChat.AppendText("Anda : " + receive + "\n");
                    }));
                    receive = "";
                }

                catch(Exception ex ) { 
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if(client.Connected)
            {
                STW.WriteLine(textSend);
                this.txtChat.Invoke(new MethodInvoker(delegate ()
                {
                    txtChat.AppendText("Saya : " + textSend + "\n");
                }));
            } else {
                MessageBox.Show("Send Failed !");
            }
            backgroundWorker2.CancelAsync();    
        }
    }
}

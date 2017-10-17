using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;

namespace ChatApp
{
    public partial class Form1 : Form
    {

        Socket sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        public Form1()
        {
            InitializeComponent();
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            // set up socket
            
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            //get own IP
            textLocalip.Text = GetLocalIP();
            textFriendsIp.Text = GetLocalIP();

            //Return own IP
            string GetLocalIP()
            {
                //IPHostEntry provides properties: AddressList, Aliases and HostName
                IPHostEntry host;

                //GetHostEntry takes in the dns or ip address and returns dns information
                //GetHostName gets the DNS host name of the local computer
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }

                return "127.0.0.1";
            }


        }

        public void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                // Binding Socket
                IPEndPoint epLocal = new IPEndPoint(IPAddress.Parse(textLocalip.Text), Convert.ToInt32(textLocalPort.Text));
                sck.Bind(epLocal);

                //Connect to remote IP and port
                EndPoint epRemote = new IPEndPoint(IPAddress.Parse(textFriendsIp.Text), Convert.ToInt32(textFriendsPort.Text));
                sck.Connect(epRemote);

                //start listening to an specific port
                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack),
                    buffer);

                //release button to send message
                buttonSend.Enabled = true;
                buttonStart.Text = "Connected";
                buttonStart.Enabled = false;
                textMessage.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            public delegate int MessageCallBack
            {
                try
                {
                    int size = sck.EndReceiveFrom(aResult, ref epRemote);

                    byte[] receivedData = new byte[1464];

                    // getting the message data
                    receivedData = (byte[]) aResult.AsyncState;

                    //converts message data from byte to array string
                    ASCIIEncoding eEncoding = new ASCIIEncoding();
                    string receivedMessage = eEncoding.GetString(receivedData);

                    // adding Message to the listbox
                    listMessage.Items.Add("Friend" + receivedMessage);
                }

                //byte[] buffer = new byte[1500];
                //sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote,
                //    new AsyncCallback(MessageCallBack), buffer);
        


                catch (Exception exp)
                {
                    MessageBox.Show(exp.ToString());
                }
           }
        }

        

        private void buttonSend_Click(object sender, EventArgs e)
        {
            try
            {
                //converts from string to byte
                ASCIIEncoding enc = new ASCIIEncoding();
                byte[] msg = new byte[1500];
                msg = enc.GetBytes(textMessage.Text);

                //sending the messsage
                sck.Send(msg);
                
                //Add to listbox
                listMessage.Items.Add("You" + textMessage.Text);

                textMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}

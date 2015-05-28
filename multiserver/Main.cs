using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace multiserver
{
    public partial class Main : Form
    {
        Listener listener;
        public Main()
        {
            InitializeComponent();
            listener = new Listener(8);
            listener.SocketAccepted += new Listener.SocketAcceptedHandler(listener_SocketAccepted);
            Load += new EventHandler(Main_Load);
        }

        void listener_SocketAccepted(System.Net.Sockets.Socket e)
        {
            Sclient sclient = new Sclient(e);
            sclient.Received += new Sclient.SclintReceiveHandler(client_Received);
            sclient.Disconnected += new Sclient.SclientDisconnectedHandler(client_Disconnected);

            Invoke((MethodInvoker)delegate
            {
                ListViewItem i = new ListViewItem();
                i.Text = sclient.EndPoint.ToString();
                i.SubItems.Add(sclient.ID);
                i.SubItems.Add("XX");
                i.SubItems.Add("XX");
                i.Tag = sclient;
                lstClients.Items.Add(i);
            });
        }

        void client_Disconnected(Sclient sender)
        {
            Invoke((MethodInvoker)delegate
            {
                for (int i = 0; i < lstClients.Items.Count;i++ )
                {
                    Sclient sclient = lstClients.Items[i].Tag as Sclient;

                    if (sclient.ID == sender.ID){
                        lstClients.Items.RemoveAt(i);
                        break;
                    }
                }
            });
        }

        void client_Received(Sclient sender, byte[] data)
        {
            Invoke((MethodInvoker)delegate
            {
                for (int i = 0; i < lstClients.Items.Count; i++)
                {
                    Sclient sclient = lstClients.Items[i].Tag as Sclient;

                    if (sclient.ID == sender.ID)
                    {
                        lstClients.Items[i].SubItems[2].Text = Encoding.Default.GetString(data);
                        lstClients.Items[i].SubItems[3].Text = DateTime.Now.ToString();
                        break;
                    }
                }
            });
        }
        private void Main_Load(object sender, EventArgs e)
        {
            listener.Start();
        }
    }
}

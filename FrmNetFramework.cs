using Microsoft.AspNet.SignalR.Client;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SignalR_Windows
{
    public partial class FrmNetFramework : Form
    {
        public FrmNetFramework()
        {
            InitializeComponent();
        }

        //string url = @"http://localhost:12316/";
        string url = "http://signal.pn.com/";
        IHubProxy _hub;
        private void Form1_Load(object sender, EventArgs e)
        {
            var connection = new HubConnection(url);
            _hub = connection.CreateHubProxy("ChatHub");
            
            _hub.On("Test", x =>
            {
                new Thread(delegate () {
                    Test(Newtonsoft.Json.JsonConvert.SerializeObject(x));
                }).Start();
            });
            connection.Start().Wait();
        }

        delegate void DelVoid();
        public void Test(string message)
        {
            label1.Invoke(new DelVoid(() => label1.Text += (Environment.NewLine + message)));
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string msg = txtuserName.Text.Trim() + ": " + txt.Text.Trim();
            _hub.Invoke("Test", msg).Wait();
            txt.Text = string.Empty;
            txt.Focus();
        }

        private void txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSend.PerformClick();
            }
        }
    }
}

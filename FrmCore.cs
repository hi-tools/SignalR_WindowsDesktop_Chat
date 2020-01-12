//using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Client;
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
    public partial class FrmCore : Form
    {
        public FrmCore()
        {
            InitializeComponent();
        }

        string url = @"http://localhost:12316/ChatHub";
        //string url = @"http://signal.pn.com//ChatHub";
        //
        HubConnection _hub;
        private async void Form1_Load(object sender, EventArgs e)
        {
            var connection = new HubConnectionBuilder();
            connection.WithUrl(url);
            _hub = connection.Build();
            await _hub.StartAsync();

            _hub.On<string, string>("ReceiveMessage", (user, message) =>
            {
                new Thread(delegate ()
                {
                    ReceiveMessage(user, Newtonsoft.Json.JsonConvert.SerializeObject(message));
                }).Start();
            });

            _hub.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _hub.StartAsync();
            };

            //_hub.On<object[]>("ReceiveMessage", (data) =>
            //{
            //    new Thread(delegate ()
            //    {
            //        Update(data);
            //    }).Start();
            //});
        }

        delegate void DelVoid();
        public void ReceiveMessage(string user, string message)
        {
            label1.Invoke(new DelVoid(() => label1.Text += (Environment.NewLine + message)));
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            string msg = txt.Text.Trim();
            try
            {
                await _hub.InvokeAsync("SendMessage", txtuserName.Text, txt.Text);
                txt.Text = string.Empty;
                txt.Focus();
            }
            catch (Exception ex)
            {
                label1.Text += (Environment.NewLine + ex.Message);
            }
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

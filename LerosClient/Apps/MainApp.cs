using LerosClient;
using LerosClient.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spider.Apps
{
    public class main : Spider.App
    {
        private object JSON;
        public main(SpiderHost host)
            : base(host)
        {
            tmrReload = new Timer();
            tmrReload.Tick += tmrReload_Tick;
            tmrReload.Interval = 1000;
            tmrReload.Start();
            this.Spider.LoadFile("testlua.xml");
            OAuthForm oauthForm = new OAuthForm("http://leros.cobresia.webfactional.com/test", "http://home:8080/myproject/oauth2/authorize", "4c5b175ea7cd6e51022cb999da4c0d", "2fa2469afcb2be1ee6133ede33e384");
            if (LerosClient.Properties.Settings.Default.access_token == null)
            {
                if (oauthForm.ShowDialog() == DialogResult.OK)
                {
                    String code = oauthForm.Code;
                }
            }
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += wc_DownloadStringCompleted;
            wc.DownloadStringAsync(new Uri("http://home:8080/myproject/leros/api/v1/session/?format=json&bearer_token=" + LerosClient.Properties.Settings.Default.access_token));

        }

        /// <summary>
        /// Navigate within the app
        /// </summary>
        /// <param name="arguments"></param>
        public new void Navigate(String[] arguments)
        {
            base.Navigate(arguments);
           
        }
        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            Result result = (Result)JsonConvert.DeserializeObject<Result>(e.Result);
         
            
            JSON = new
            {
                result = result
            };
        }

        void tmrReload_Tick(object sender, EventArgs e)
        {
            if (JSON != null)
            {
                this.Spider.Refresh(JSON);
                tmrReload.Stop();
            }
        }
        private System.Windows.Forms.Timer tmrReload;

        object spiderView_ViewUpdating(object sender)
        {
            return JSON;

        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "MainApp";
            this.Load += new System.EventHandler(this.MainApp_Load);
            this.ResumeLayout(false);

        }

        private void MainApp_Load(object sender, EventArgs e)
        {

        }

    }
}

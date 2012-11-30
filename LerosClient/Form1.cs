using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace LerosClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        System.Windows.Forms.Timer tmrReload;
        
        private SpiderView spiderView;
        public Style Stylesheet = new Style();
        Object JSON = new
        {
            number = 1
        };
        private void Form1_Load(object sender, EventArgs e)
        {
            OAuthForm oauthForm = new OAuthForm("http://leros.cobresia.webfactional.com/test", "http://home:8080/myproject/oauth2/authorize", "4c5b175ea7cd6e51022cb999da4c0d", "2fa2469afcb2be1ee6133ede33e384");
            if (Properties.Settings.Default.access_token == null)
            {
                if (oauthForm.ShowDialog() == DialogResult.OK)
                {
                    String code = oauthForm.Code;
                }
            }
            tmrReload = new System.Windows.Forms.Timer();
            this.spiderView = new SpiderView();
            //this.board =  new Board(this);
            this.Controls.Add(spiderView);
            this.spiderView.Dock = DockStyle.Fill;
            spiderView.Dock = DockStyle.Fill;
            spiderView.LoadFile("testsection.xml");
            spiderView.ViewUpdating += spiderView_ViewUpdating;
            tmrReload.Tick += tmrReload_Tick;
            tmrReload.Interval = 1000;
            tmrReload.Start();
           

        }

        void tmrReload_Tick(object sender, EventArgs e)
        {
            JSON = new
            {
                number = new Random().Next()
            };
        }

        object spiderView_ViewUpdating(object sender)
        {
            return JSON;

        }
    }
}

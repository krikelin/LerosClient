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
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using Spider;
namespace LerosClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        System.Windows.Forms.Timer tmrReload;
        
        public Style Stylesheet = new Style();
        Object JSON = new
        {
            number = 1
        };
        
        private SPListView listView;
        SpiderHost SpiderHost;

        private void Form1_Load(object sender, EventArgs e)
        {
            SpiderHost = new Spider.SpiderHost();
            SpiderHost.RegistredAppTypes.Add("main", typeof(Spider.Apps.main));
            tmrReload = new System.Windows.Forms.Timer();

            this.Controls.Add(SpiderHost);
            SpiderHost.Dock = DockStyle.Fill;
            SPListView listView = new SPListView(this.Stylesheet);
            this.Controls.Add(listView);
            listView.AddItem("Test", new Uri("leros:internal:test"));
            listView.Dock = DockStyle.Left;
            listView.Width = 270;
            SpiderHost.Navigate("spider:main:t");
            

        }

        
    }
}

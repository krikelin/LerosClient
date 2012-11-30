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
        private SpiderView spiderView;
        public Style Stylesheet = new Style();
        private void Form1_Load(object sender, EventArgs e)
        {
            this.spiderView = new SpiderView();
            //this.board =  new Board(this);
            this.Controls.Add(spiderView);
            this.spiderView.Dock = DockStyle.Fill;
            spiderView.Dock = DockStyle.Fill;
            spiderView.LoadFile("testsection.xml");
          

        }
    }
}

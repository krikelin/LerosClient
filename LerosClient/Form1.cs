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
        private Board board = new Board();
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Controls.Add(board);
            board.Dock = DockStyle.Fill;
            XmlDocument xd = new XmlDocument();
            xd.Load("testsection.xml");
            board.LoadNodes(xd.DocumentElement);

        }
    }
}

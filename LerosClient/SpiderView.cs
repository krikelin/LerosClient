using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using DotLiquid;

namespace LerosClient
{
    /// <summary>
    /// Clean room implementation of Spotify spider layout engine
    /// </summary>
    public partial class SpiderView : UserControl
    {
        public Dictionary<String, Board> Sections = new Dictionary<string, Board>();
        public Style Stylesheet = new Style();
        public SpiderView()
        {
            InitializeComponent();
            this.tabBar = new TabBar(this);
            this.deck = new Panel();
            this.deck.AutoScroll = true;
            tabBar.Dock = DockStyle.Top;
            tabBar.Height = 28;
            this.Controls.Add(deck);
            this.Controls.Add(tabBar);
            deck.Dock = DockStyle.Fill;
            this.tabBar.Dock = DockStyle.Top;
            this.BackColor = Stylesheet.BackColor;
            this.tabBar.TabChange += tabBar_TabChange;
            
        }

        void tabBar_TabChange(object sender, TabBar.TabChangedEventArgs e)
        {
            this.deck.Controls.Clear();
            this.deck.Controls.Add(Sections[e.Tab.ID]);
        }
        private String template;
        public void LoadFile(String fileName)
        {
            using (StreamReader sr = new StreamReader(fileName))
            {
                template = sr.ReadToEnd();
            }
            Refresh(new Object());
        }
        public void Refresh(Object obj)
        {
            this.tabBar.Clear();
            Preprocessor processor = new Preprocessor();

            Template template = Template.Parse(this.template);
            String DOM = template.Render(Hash.FromAnonymousObject(obj));
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(DOM);
            this.LoadNodes(xmlDoc.DocumentElement);
        }
        public void LoadNodes(XmlElement element)
        {
            var sections = element.GetElementsByTagName("section");
            foreach (XmlElement _section in sections)
            {
                Tab tab = new Tab();
                tab.Title = _section.GetAttribute("title");
                tab.ID = _section.GetAttribute("id");
                this.tabBar.Tabs.Add(tab);
                Board childBoard = new Board(this);
                Sections.Add(tab.ID, childBoard);
                childBoard.LoadNodes(_section);
                this.deck.Controls.Add(childBoard);
            }
        }

        private TabBar tabBar;
        private Panel deck;
        private void SpiderView_Load(object sender, EventArgs e)
        {

        }
    }
}

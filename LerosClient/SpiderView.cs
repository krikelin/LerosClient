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
using Spider;
namespace LerosClient
{
    /// <summary>
    /// Clean room implementation of Spotify spider layout engine
    /// </summary>
    public partial class SpiderView : UserControl
    {
       
        /// <summary>
        /// Delegator that deelgates update of view
        /// </summary>
        /// <param name="sender">The sender of the object</param>
        /// <returns></returns>
        public delegate Object UpdateView(object sender);

        /// <summary>
        /// Raised when the view has expired and is renewing
        /// </summary>
        public event UpdateView ViewUpdating;
        public Dictionary<String, Board> Sections = new Dictionary<string, Board>();
        public Style Stylesheet = new Style();
        public int RefreshRate
        {
            get
            {
                return this.timer.Interval;
            }
            set
            {
                this.timer.Interval = value;
            }
        }
        private System.Windows.Forms.Timer timer;
        public SpiderView()
        {

            this.timer = new Timer();
            InitializeComponent();
            this.tabBar = new TabBar(this);
            this.deck = new Panel();
            this.deck.AutoScroll = true;
            tabBar.Dock = DockStyle.Top;
            tabBar.Height = 28;
            this.Controls.Add(deck);
            this.Controls.Add(tabBar);
            this.tabBar.Dock = DockStyle.Top;
            this.BackColor = Stylesheet.BackColor;
            this.tabBar.TabChange += tabBar_TabChange;
            this.timer.Tick += timer_Tick;
            this.timer.Interval = 1000;
            this.timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (this.ViewUpdating != null)
            {
                this.Refresh(this.ViewUpdating(this));
            }
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
            Preprocessor processor = new Preprocessor();

            Template template = Template.Parse(this.template);
            String DOM = template.Render(Hash.FromAnonymousObject(obj));
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(DOM);
            if (this.Sections.Count > 0)
            {
                this.LoadNodesAgain(xmlDoc.DocumentElement);
            }
            else
            {
                this.LoadNodes(xmlDoc.DocumentElement);
            }
        }
        public void LoadNodesAgain(XmlElement element)
        {
            var sections = element.GetElementsByTagName("section");
            foreach (XmlElement _section in sections)
            {
                Board section = this.Sections[_section.GetAttribute("id")];
                section.Children.Clear();
                section.LoadNodes(_section);
            }
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
                this.deck = DockStyle.Fill;
            }
        }

        private TabBar tabBar;
        private Panel deck;
        private void SpiderView_Load(object sender, EventArgs e)
        {

        }
    }
}

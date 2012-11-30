﻿using System;
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
    public partial class Board : UserControl
    {
        public class LinkEventArgs
        {
            public Uri Link { get; set; }
        }
        public void RaiseLink(Uri uri)
        {
            if (this.LinkClicked != null)
            {
                this.LinkClicked(this, new LinkEventArgs() { Link = uri });
            }
        }
        public delegate void LinkEventHandler(object sender, LinkEventArgs e);
        public event LinkEventHandler LinkClicked;
        public SpiderView SpiderView;
        public Style Stylesheet;
        public int ScrollX { get; set; }
        public int ScrollY {get;set;}
        public Padding Padding = new Padding("0");
        public List<Element> Children = new List<Element>();
        public String template = "";
        
        public Board(SpiderView spiderView)
        {
            this.SpiderView = spiderView;
            this.Stylesheet = (this.SpiderView).Stylesheet;

            InitializeComponent();

            this.Paint += Board_Paint;
            this.Resize += Board_Resize;
            this.ForeColor = Stylesheet.ForeColor;
            this.BackColor = Stylesheet.BackColor;
            tmrDraw = new Timer();
            tmrDraw.Tick += tmrDraw_Tick;
            tmrDraw.Interval = 100;
            tmrDraw.Start();
            this.MouseMove += Board_MouseMove;
        }
        public Board()
        {
            InitializeComponent();
            
            this.Paint += Board_Paint;
            this.Resize += Board_Resize;
            this.Stylesheet = ((Form1)this.ParentForm).Stylesheet;
            this.ForeColor = Stylesheet.ForeColor;
            this.BackColor = Stylesheet.BackColor;
            tmrDraw = new Timer();
            tmrDraw.Tick += tmrDraw_Tick;
            tmrDraw.Interval = 100;
            tmrDraw.Start();
            this.MouseMove += Board_MouseMove;
        }

        void Board_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            foreach (Element elm in Children)
            {
                x -= elm.X;
                y -= elm.Y;
                if ((x > elm.X && x < elm.X + elm.Width) && (y > elm.Y && y < elm.Y + elm.Height))
                {
                    elm.CheckHover(x, y);
                }
                x += elm.X;
                y += elm.Y;
            }
        }

        void tmrDraw_Tick(object sender, EventArgs e)
        {
            Graphics g = this.CreateGraphics();
            Draw(g, new Rectangle(0, 0, Width, Height));
        }
        private Timer tmrDraw;

        void Board_Resize(object sender, EventArgs e)
        {
            this.PackChildren();
        }

        void Board_Paint(object sender, PaintEventArgs e)
        {
            Draw(e.Graphics, e.ClipRectangle);
        }
       
        public void PackChildren()
        {
            int row = 0;
            int left = Padding.Left;
            int max_height = 0;
            foreach (Element child in this.Children)
            {
                if (child.Dock == LerosClient.Element.DockStyle.Right)
                {
                    child.Width = this.Width - Padding.Right * 2 - child.Margin.Right * 2 - child.X;
                }
               /* if (max_height > child.Height)
                    max_height = child.Height;
                if (left + child.Width > this.Width - this.Padding.Right * 2)
                {
                    
                }
                child.X = left;
                child.Y = row;*/
                
                child.X = this.Padding.Left;
                child.Y = this.Padding.Top + row;
                left += child.Width + child.Padding.Right;
                child.PackChildren();
                row += child.Height;
            }
        }
        
        BufferedGraphicsContext BGC = new BufferedGraphicsContext();
        public void LoadNodes(XmlElement root)
        {
            foreach (XmlNode elm in root.ChildNodes)
            {
                if (elm.GetType() == typeof(XmlElement))
                {
                    try
                    {
                        Element _elm = (Element)Type.GetType("LerosClient." + elm.Name).GetConstructor(new Type[] { typeof(Board), typeof(XmlElement) }).Invoke(new Object[] { this, elm });
                        this.Children.Add(_elm);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            PackChildren();
        }
        private void Board_Load(object sender, EventArgs e)
        {

        }
        public void Draw(Graphics g, Rectangle target)
        {
            this.BackColor = Stylesheet.BackColor;
            try
            {
                BufferedGraphics bgc = BGC.Allocate(g, target);
                bgc.Graphics.FillRegion(new SolidBrush(Stylesheet.BackColor), new System.Drawing.Region(new Rectangle(0, 0, this.Width, this.Height)));
                bgc.Graphics.TranslateTransform(ScrollX, ScrollY);
                foreach (Element elm in Children)
                {

                    bgc.Graphics.TranslateTransform(elm.X, elm.Y);
                    elm.Draw(bgc.Graphics);
                    bgc.Graphics.TranslateTransform(-elm.X, -elm.Y);

                }
                bgc.Graphics.TranslateTransform(-ScrollX, -ScrollY);
                bgc.Render();
            }
            catch (System.ComponentModel.Win32Exception e)
            {

            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            this.Draw(e.Graphics, this.Bounds);
        }

        public Form1 Form { get; set; }
    }
}

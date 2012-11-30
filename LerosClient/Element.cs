using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Reflection;
using System.Net;
using System.IO;
using System.Drawing.Html;


namespace LerosClient
{
    public class Constraint
    {
        public Constraint(String value)
        {
            if (value.Contains(','))
            {
                String[] t = value.Split(',');
                Left = int.Parse(t[0]);
                Top = int.Parse(t[1]);
                Bottom = int.Parse(t[2]);
                Right = int.Parse(t[3]);
            }
            else
            {
                int val = int.Parse(value);
                Left = val;
                Top = val;
                Right = val;
                Bottom = val;
            }
        }
        public int Top { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
    }
    public class Margin : Constraint
    {
        public Margin(String value) :
            base(value)
        {

        }
    }
    public class Padding : Constraint
    {
        public Padding(String value) :
            base(value)
        {

        }
    }
    /// <summary>
    /// Section
    /// </summary>
    public class Section
    {
        public String Title { get; set; }
        public String Name { get; set; }
        public Section(String title, String name)
        {
            this.Title = title;
            this.Name = name;
        }
        public List<Element> Children = new List<Element>();
    }
    
    /// <summary>
    /// Element
    /// </summary>
    public abstract class Element
    {
        public Color BackColor = Color.Transparent;
        public Color ForeColor = Color.Black;
        public Margin Margin = new Margin("0");
        public Padding Padding = new Padding("0");
        public int Flex = 0;
        public enum DockStyle {
            Left, Top, Bottom, Right
        }
        public DockStyle Dock;
        public void CheckHover(int x, int y)
        {


            this.OnMouseOver(x, y);
           
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
        public event MouseEventHandler MouseMove;
        private void OnMouseOver(int x, int y)
        {
            if (this.MouseMove != null)
                this.MouseMove(this, new MouseEventArgs() { X = x, Y = y });
        }
        public void CheckClick(int x, int y)
        {
            
            
                this.OnClick(x, y);
               
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
        private Color ParseColorAttribute(String propertyName, String attribute, XmlElement elm)
        {
            if (elm.HasAttribute(attribute))
            {
                return ParseColor(elm.GetAttribute(attribute));
            }
            else
            {
                Type type = Stylesheet.GetType();
                MemberInfo member = type.GetMember(propertyName)[0];
                if (member.MemberType == System.Reflection.MemberTypes.Property)
                {
                    PropertyInfo property = (PropertyInfo)member;
                    return (Color)property.GetValue(Stylesheet);
                }
            }
            return Color.Transparent;
        }
        private Color ParseColor(String value)
        {
            if (value.StartsWith("@"))
            {
                Type type = Stylesheet.GetType();
                MemberInfo member = type.GetMember(value.Replace("@", "") + "Color")[0];
                if (member.MemberType == System.Reflection.MemberTypes.Property)
                {
                    PropertyInfo property = (PropertyInfo)member;
                    return (Color)property.GetValue(Stylesheet);
                }
            }
            else if (value.StartsWith("#"))
            {
                
                
                  return ColorTranslator.FromHtml(value);
                
            }
            return Stylesheet.BackColor;
        }
        public String Name { get; set; }
        public Style Stylesheet = new Style();
        private XmlNode node;
        public String Hyperlink { get; set; }
        int flex = 0;
        public Element(Board Host, XmlElement node)
        {
            this.Board = Host;
            this.node = node;
            this.BackColor = ParseColorAttribute("BackColor", ("bgcolor"), node);

            this.ForeColor = ParseColorAttribute("ForeColor", "color", node);
            
            
            if (node.HasAttribute("margin"))
            {
                this.Margin = new Margin(node.GetAttribute("margin"));
            }
            if (node.HasAttribute("flex"))
            {
                this.Flex = int.Parse(node.GetAttribute("flex"));
            }
            if (node.HasAttribute("padding"))
            {
                this.Padding = new Padding(node.GetAttribute("padding"));
            }
            if (node.HasAttribute("uri"))
            {
                this.Hyperlink = node.GetAttribute("uri");
            }
            if (node.HasAttribute("name"))
            {
                this.Name = node.GetAttribute("name");
            }
            if (node.HasAttribute("width"))
            {
                if (node.GetAttribute("width") == "100%")
                {
                    Dock |= DockStyle.Right;
                    //Width = Parent.Width - Margin * 2 + Parent.Padding * 2;
                }
                else
                {
                    this.Width = int.Parse(node.GetAttribute("width"));
                }
            }
            else
            {
                this.Width = Parent != null ? Parent.Width : Board.Width;
            }
            if (node.HasAttribute("height"))
            {
                this.Height = int.Parse(node.GetAttribute("height"));
            }
            else
            {
                this.Height = 32;
            }
            foreach (XmlNode elm in node.ChildNodes)
            {
                if (elm.GetType() == typeof(XmlElement))
                {
                    try
                    {
                        Element _elm = (Element)Type.GetType("LerosClient." + elm.Name).GetConstructor(new Type[] { typeof(Board), typeof(XmlElement) }).Invoke( new Object[] {this.Board, elm});
                        this.Children.Add(_elm);
                        _elm.Parent=this;
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            PackChildren();
        }
        public Board Board { get; set; }
        public Element Parent { get; set; }
        public class MouseEventArgs {
            public int X {get;set;}
            public int Y {get;set;}
        }
        public delegate void MouseEventHandler(object sender, MouseEventArgs e);
        public event MouseEventHandler Click;
        public virtual void Draw(Graphics g)
        {
            if (this.Stylesheet == null)
            {
                this.Stylesheet = (Parent.Stylesheet != null ? Parent.Stylesheet : Board.Stylesheet);
            }
       
           foreach(Element elm in this.Children)
           {
                g.TranslateTransform(elm.X , elm.Y);
                if (elm.BackColor != null)
                {
                    g.FillRectangle(new SolidBrush(elm.BackColor), new Rectangle(elm.X + this.Padding.Left, elm.Y + this.Padding.Top, elm.Width - this.Padding.Left * 2, elm.Height - this.Padding.Top * 2));
                }
                elm.Draw(g);
                g.TranslateTransform(-elm.X, -elm.Y);
           }
        
        }
        public void OnClick(int x, int y)
        {
            if (this.Click != null)
            {
                this.Click(this, new MouseEventArgs());
            }

        }
        public void MouseOver(int x, int y)
        {
        }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<Element> Children = new List<Element>();
        public abstract void PackChildren();

    }
    public class text : Element
    {
        private String textContent;
        public String Text
        {
            get
            {
                return this.textContent;
                
            }
            set
            {
                this.textContent = value;
                this.bitmap = GenerateBitmap(); 
            }
        }
        
        public text(Board host, XmlElement node)
            : base(host, node)
        {
            Text = node.InnerText;
        }
        private Bitmap bitmap;
        private Bitmap GenerateBitmap()
        {
            Bitmap c = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(c);
            HtmlRenderer.Render(g, "<style type=\"text/css\">body{color: " + ColorTranslator.ToHtml(this.ForeColor) + ";}</style> <span color=\"" + ColorTranslator.ToHtml(ForeColor) + "\">" + Text + "</span>", new Point(0, 0), this.Width);
            return c;
        }
        /// <summary>
        /// Draw
        /// </summary>
        /// <param name="g"></param>
        public override void Draw(Graphics g)
        {
            base.Draw(g);
            if(bitmap != null)
               g.DrawImage(bitmap, new Point(0, 0));
            /*label.Width = this.Width;
            label.Height = this.Height;
            label.BackColor =this.BackColor;
            label.ForeColor = this.ForeColor;
            if(this.Width > 0 && this.Height > 0) {
                Bitmap bitmap = new Bitmap(this.Width, this.Height);
                label.DrawToBitmap(bitmap, new Rectangle(0, 0, this.Width, this.Height));
                g.DrawImage(bitmap, 0, 0);
             }*/
            

        }
        public void Resize(int newX, int newY)
        {
            foreach (Element child in Children)
            {
                this.Resize(newX, newY);
            }
            this.PackChildren();
        }
        public void MouseOver(int x, int y)
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        public override void PackChildren()
        {
            
        }
    }
 
    public class img : Element
    {
        private Image image;
        public img(Board host, XmlElement elm)
            : base(host, elm)
        {
            if (elm.HasAttribute("src"))
            {
                this.LoadImage(elm.GetAttribute("src"));
            }
        }
        public override void Draw(Graphics g)
        {
            base.Draw(g);
            if(image != null)
                g.DrawImage(image, 0, 0, this.Width, this.Height);
        }
        public void LoadImage(String url)
        {
            WebClient wc = new WebClient();
            wc.DownloadDataCompleted += wc_DownloadDataCompleted;
            wc.DownloadDataAsync(new Uri(url));
        }
        public override void PackChildren()
        {
            
        }
        void wc_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            Image image = Image.FromStream(new MemoryStream(e.Result, false));
            this.image = image;
            this.Width = this.image.Width;
            this.Height = this.image.Height;
        }
    }
    /// <summary>
    /// HBox
    /// </summary>
    public class hbox : Element
    {
        
        public hbox(Board host, XmlElement node)
            : base(host, node)
        {
            
        }
       

      

        public override void PackChildren()
        {
            int pos = 0;
            int quote = this.Width / this.Children.Count;
            int count_flex= 0;
            int flexible_width = 0;
            int static_width = 0;
            foreach (Element elm in this.Children)
            {
                if (elm.Flex > 0)
                {
                    count_flex += 1;
                    
                }
                else
                {
                    static_width += elm.Width;
                }
            }
            if(count_flex > 0)
            flexible_width = (this.Width - static_width) / count_flex;

            for (int i = 0; i < this.Children.Count; i++)
            {
                Element child = this.Children[i];
                child.X = child.Margin.Left + this.Padding.Left + pos;
                child.Y = child.Margin.Top + this.Padding.Bottom;
                
                child.Height = this.Height - child.Margin.Bottom * 2 - this.Padding.Top * 2;
                if (child.Flex > 0)
                {
                    child.Width = flexible_width;
                    pos += flexible_width; ;
                }
                else
                {

                    pos += child.Width + child.X;
                }
               
              
                
                
                child.PackChildren();
            }
        }
    }
    /// <summary>
    /// Flowbox
    /// </summary>
    public class flow : Element
    {
        public flow(Board parent, XmlElement node)
            : base(parent, node)
        {
        }
        public override void Draw(Graphics g)
        {
            base.Draw(g);
            
            
        }
        public override void PackChildren()
        {
            int row = 0;
            int left = Padding.Left;
            int max_height = 0;
            foreach (Element child in this.Children)
            {
                if (child.Dock == LerosClient.Element.DockStyle.Right)
                {
                    child.Width = this.Width - this.Padding.Right * 2 - child.Margin.Right * 2 - child.X;
                }
                if (max_height > child.Height)
                    max_height = child.Height;
                if (left + child.Width > this.Width - this.Padding.Right * 2)
                {
                    row += max_height + this.Padding.Bottom *2 + child.Margin.Bottom;
                }
                child.X = left;
                child.Y = row;
                left += child.Width + this.Padding.Left + child.Margin.Right;
                child.PackChildren();
            }
        }
    }
    /// <summary>
    /// VBox element
    /// </summary>
    public class vbox : Element
    {
        public vbox(Board parent, XmlElement node)
            : base(parent, node)
        {
        }
        public void Draw(Graphics g)
        {
            base.Draw(g);
            
        }



        public override void PackChildren()
        {
            int pos = 0;
            int quote = this.Width / this.Children.Count;
            int count_flex = 0;
            int flexible_height = 0;
            int static_height = 0;
            foreach (Element elm in this.Children)
            {
                if (elm.Flex > 0)
                {
                    count_flex += 1;

                }
                else
                {
                    static_height += elm.Height;
                }
            }
            if(count_flex > 0)
            flexible_height = (this.Height - static_height) / count_flex;

            for (int i = 0; i < this.Children.Count; i++)
            {
                Element child = this.Children[i];
                child.Y = child.Margin.Top + this.Padding.Top;

                child.Width = this.Width - child.Margin.Right * 2 - this.Padding.Left * 2;
                child.Y = pos;
                if (child.Flex > 0)
                {
                    child.Height = flexible_height;
                    pos += flexible_height; ;
                }
                else
                {

                    pos += child.Height + child.Y;
                }

              



                child.PackChildren();
            }
        }
    }
}

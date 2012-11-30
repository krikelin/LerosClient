using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LerosClient
{
    [Serializable]
    [XmlRoot("skin")]
    public class Style
    {
        public Style()
        {
            ForeColor = Color.FromArgb(255, 211, 211, 211);
            AlternateColor = Color.Gray;
            BackColor = Color.FromArgb(255, 55, 55, 55);
            EntryColor = Color.FromArgb(255, 88, 88, 88);
            Font = new Font("MS Sans Serif", 11);
        }
        [XmlElement("forecolor")]
        public Color ForeColor { get; set; }
        public Color EntryColor { get; set; }
        public Color BackColor { get; set; }
        public Color AlternateColor { get; set; }
        public Font Font { get; set; }
        
    }
}

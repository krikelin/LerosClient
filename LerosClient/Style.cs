using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LerosClient
{
    public class Style
    {
        public Style()
        {
            ForeColor = Color.FromArgb(255, 211, 211, 211);
            AlternateColor = Color.Gray;
            BackColor = Color.FromArgb(255, 55, 55, 55);
            Font = new Font("MS Sans Serif", 11);
        }
        public Color ForeColor { get; set; }
        public Color BackColor { get; set; }
        public Color AlternateColor { get; set; }
        public Font Font { get; set; }
        
    }
}

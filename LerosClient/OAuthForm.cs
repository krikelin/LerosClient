using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LerosClient
{
    public partial class OAuthForm : Form
    {
        public OAuthForm()
        {
            InitializeComponent();
        }
        public OAuthForm(String redirect_uri, String auth_uri, String client_id, String client_secret)
        {
            InitializeComponent();
            this.RedirectUri = redirect_uri;
            this.AuthUri = auth_uri + "?client_id=" + client_id + "&redirect_uri=" + Uri.EscapeUriString(redirect_uri) + "&grant_type=token";
            this.ClientSecret = client_secret;
            
            this.webBrowser1.Navigated += webBrowser1_Navigated;
            this.webBrowser1.Navigating += webBrowser1_Navigating;
        }

        void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            
        }

        void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.ToString().Contains("code"))
            {
                String code = e.Url.Query.Split('=')[1];
                this.Code = code;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        public String AccessToken { get; set; }
        public String RedirectUri {get;set;}
        public String AuthUri {get;set;}
        public String ClientID {get; set;}
        public String ClientSecret { get; set; }
        private void OAuthForm_Load(object sender, EventArgs e)
        {
            this.webBrowser1.Navigate(this.AuthUri);
        }

        public string Code { get; set; }
    }
}

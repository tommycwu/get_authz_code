using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace get_authz_code
{
    public partial class Form1 : Form
    {
        private static string authorizeUri;
        private static async Task<string> GetAuthzCode(string s, string d, string a, string i, string r, string c, string t, string n, string o)
        {
            var domain = d;
            var oktaAuthorizationServer = a;
            var clientId = i;
            var redirectUriEncoded = System.Net.WebUtility.UrlEncode(r);
            var responseType = System.Net.WebUtility.UrlEncode(c);
            var state = t;
            var nonce = n;
            var scope = System.Net.WebUtility.UrlEncode(o);

            var proxy = new WebProxy
            {
                Address = new Uri($"http://{proxyHost}:{proxyPort}"),
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false,

                // *** These creds are given to the proxy server, not the web server ***
                Credentials = new NetworkCredential(
                    userName: proxyUserName,
                    password: proxyPassword)
            };

            // Now create a client handler which uses that proxy
            var httpClientHandler = new HttpClientHandler
            {
                Proxy = proxy,
                AllowAutoRedirect = false,
            };

            using (var httpClient = new HttpClient(httpClientHandler))
            {
                httpClient.DefaultRequestHeaders
                    .Accept
                    //.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    .Add(new MediaTypeWithQualityHeaderValue("*/*"));


                authorizeUri = $"{domain}/oauth2/{oktaAuthorizationServer}/v1/authorize?client_id={clientId}" +
                    $"&redirect_uri={redirectUriEncoded}&response_type={responseType}&sessionToken={s}" +
                    $"&state={state}&nonce={nonce}&scope={scope}";


                HttpResponseMessage authorizeResponse = await httpClient.GetAsync(authorizeUri);
                var statusCode = (int)authorizeResponse.StatusCode;

                if (statusCode == (int)HttpStatusCode.Found)
                {
                    var redirectUri = authorizeResponse.Headers.Location;
                    var queryDictionary = HttpUtility.ParseQueryString(redirectUri.AbsoluteUri);
                    return queryDictionary[0];
                }
                return statusCode.ToString();            
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = await GetAuthzCode(textBox1.Text, textBox3.Text, textBox4.Text, textBox5.Text, textBox6.Text, textBox7.Text, textBox8.Text, textBox9.Text, textBox10.Text);
            textBox11.Text = authorizeUri;
        }
    }
}

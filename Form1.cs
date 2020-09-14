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
        private static async Task<string> GetAuthzCode(string sessionToken)
        {
            var domain = @"https://tfsciampoc.oktapreview.com/";
            var oktaAuthorizationServer = @"austimjntkiqGPeQF0h7";
            var clientId = @"0oatimej1j8kubhRD0h7";
            var rdirectUri = @"http://localhost:8080/authorization-code/callback";
            var redirectUriEncoded = System.Net.WebUtility.UrlEncode(rdirectUri);
            var responseType = "code";
            var state = "state";
            var nonce = "nonce";
            var scope = "openid";

            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.AllowAutoRedirect = false;

            using (var httpClient = new HttpClient(httpClientHandler))
            {
                httpClient.DefaultRequestHeaders
                    .Accept
                    //.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    .Add(new MediaTypeWithQualityHeaderValue("*/*"));


                var authorizeUri = $"{domain}/oauth2/{oktaAuthorizationServer}/v1/authorize?client_id={clientId}" +
                    $"&redirect_uri={redirectUriEncoded}&response_type={responseType}&sessionToken={sessionToken}" +
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
            textBox2.Text = await GetAuthzCode(textBox1.Text);
        }
    }
}

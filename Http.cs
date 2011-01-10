using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace Http
{
    /// <summary>
    /// класс для работы с http
    /// </summary>
    class Http
    {
        private string useragent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; ru; rv:1.9.2.4) Gecko/20100611 Firefox/3.6.4";// будет хранить тип браузера
        private string reffer = ""; // будет хранить reffer
        private bool allowautoredirect = true; // делать редерект по запросам?
        private CookieContainer cookie = new CookieContainer(); // будет хранить кукисы
        private string postData = null;

        private static ManualResetEvent allDone = new ManualResetEvent(false);
        public Http() { }

        public string PostData
        {
            set
            {
                postData = value;
            }
        }

        public void Post(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.CookieContainer = cookie;
            request.UserAgent = useragent;
            request.Referer = reffer;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.AllowAutoRedirect = allowautoredirect;
            request.BeginGetRequestStream(new AsyncCallback(StreamCallback), request);
        }

        private void StreamCallback(IAsyncResult result)
        {
            HttpWebRequest request = (HttpWebRequest)result.AsyncState;
            Stream stream = request.EndGetRequestStream(result);
            byte[] d = Encoding.UTF8.GetBytes(postData);
            stream.Write(d, 0, d.Length);
            cookie = request.CookieContainer;
            request.BeginGetResponse(new AsyncCallback(PostCallback), request);
            stream.Close();
        }

        private void PostCallback(IAsyncResult result)
        {
            HttpWebRequest request = (HttpWebRequest)result.AsyncState;

            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);
            StreamReader streamreader = new StreamReader(response.GetResponseStream());

            string buffer = streamreader.ReadToEnd();
            if (Message != null)
                Message(this, buffer);
        }

        public void Get(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.UserAgent = useragent;
            request.Referer = reffer;
            request.ContentType = "application/x-www-form-urlencoded";
            request.CookieContainer = cookie;
            request.AllowAutoRedirect = allowautoredirect;
            request.BeginGetResponse(new AsyncCallback(GetCallback), request);
        }

        private void GetCallback(IAsyncResult result)
        {
            HttpWebRequest req = (HttpWebRequest)result.AsyncState;
            HttpWebResponse response = (HttpWebResponse)req.EndGetResponse(result);
            StreamReader streamreader = new StreamReader(response.GetResponseStream());
            string buffer = streamreader.ReadToEnd();
            if (Message != null)
                Message(this, buffer);
        }

        public delegate void HttpHandler(object sender, string message);
        public event HttpHandler Message;
    }
}

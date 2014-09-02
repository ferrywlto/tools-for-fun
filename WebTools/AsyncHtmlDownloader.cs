using HtmlAgilityPack;
using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace com.verdantsparks.csharp.tools.web
{
    public class AsyncHtmlDownloader
    {
        public string _encoding = "utf-8";

        public AsyncHtmlDownloader(string encoding = "utf-8")
        {
            _encoding = encoding;
        }

        public delegate void ParseDelegate(HtmlDocument htmlDoc);

        public HtmlDocument byteArrayToHtmlDocument(byte[] bytes)
        {
            String source = Encoding.GetEncoding(_encoding).GetString(bytes, 0, bytes.Length - 1);
            source = WebUtility.HtmlDecode(source);
            HtmlDocument resultDoc = new HtmlDocument();
            resultDoc.LoadHtml(source);
            return resultDoc;
        }

        public async void loadHtmlDocument(string url, ParseDelegate callback)
        {
            HttpClient http = new HttpClient();
            var response = await http.GetByteArrayAsync(url);
            HtmlDocument resultDoc = byteArrayToHtmlDocument(response);
            callback(resultDoc);
        }
    }
}
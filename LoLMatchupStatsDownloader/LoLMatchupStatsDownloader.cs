using com.verdantsparks.csharp.tools.web;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace com.verdantsparks.csharp.tools.game
{
    public class LoLMatchupStatsDownloader
    {
        public static readonly string ERR_WORK_FINISHED = "Parsing was previously completed, please create another instance for another parsing.";
        public static readonly string ERR_INVALID_URL = "Invalid URL.";
        private bool _workfinish = false;
        private int champCount = 0;
        private string _url = string.Empty;

        private Dictionary<string, ChampionStats> champList = new Dictionary<string, ChampionStats>();
        private Dictionary<string, string> errorChampList = new Dictionary<string, string>();
        private AsyncHtmlDownloader htmlDownloader;
        private Parser parser;
        
        public LoLMatchupStatsDownloader()
        {
            htmlDownloader = new AsyncHtmlDownloader();
            parser = new Parser();
        }

        public bool WorkFinish
        {
            get { return _workfinish; }
        }

        public void start(string url)
        {
            if (WorkFinish)
                throw new Exception(ERR_WORK_FINISHED);

            if (url == string.Empty)
                throw new Exception(ERR_INVALID_URL);

            _url = url;
            htmlDownloader.loadHtmlDocument(url, parseChampList);
        }

        private void parseChampList(HtmlDocument htmlDoc)
        {
            HtmlNode championListNode = parser.getDivBlock(htmlDoc, "champions");

            List<HtmlNode> champNodes = championListNode.Descendants().Where(x => (x.Name == "a")).ToList();

            List<string> champURLs = new List<string>();

            foreach (HtmlNode item in champNodes)
            {
                string link = item.GetAttributeValue("href", null);
                link = link.Substring(link.LastIndexOf('/'), link.Length - link.LastIndexOf('/'));
                champURLs.Add(link);
            }

            champCount = champURLs.Count;

            foreach (string champName in champURLs)
                htmlDownloader.loadHtmlDocument(_url + champName, parseChampStats);

            while (champList.Count != champCount)
                Thread.Sleep(100);

#if DEBUG
                printMatchupStats();
#endif
            _workfinish = true;
        }

        private void parseChampStats(HtmlDocument htmlDoc)
        {
            parser.parseChampStats(htmlDoc, champList);
        }

        private void printMatchupStats()
        {
            foreach (KeyValuePair<string, ChampionStats> champPair in champList)
            {
                ChampionStats stats = champPair.Value;
                Debug.Print(String.Format("===== {0} =====", stats.champName));
                Debug.Print("----- weak -----");
                foreach (KeyValuePair<string, ChampionMatchupEntry> entryPair in stats.weak)
                {
                    ChampionMatchupEntry entry = entryPair.Value;
                    Debug.Print(String.Format("{0} up:{1} down:{2} rate:{3}", entry.champName, entry.upVote, entry.downVote, entry.rate));
                }
                Debug.Print("----- strong -----");
                foreach (KeyValuePair<string, ChampionMatchupEntry> entryPair in stats.strong)
                {
                    ChampionMatchupEntry entry = entryPair.Value;
                    Debug.Print(String.Format("{0} up:{1} down:{2} rate:{3}", entry.champName, entry.upVote, entry.downVote, entry.rate));
                }
                Debug.Print("----- good with -----");
                foreach (KeyValuePair<string, ChampionMatchupEntry> entryPair in stats.partners)
                {
                    ChampionMatchupEntry entry = entryPair.Value;
                    Debug.Print(String.Format("{0} up:{1} down:{2} rate:{3}", entry.champName, entry.upVote, entry.downVote, entry.rate));
                }
            }
        }
    }
}
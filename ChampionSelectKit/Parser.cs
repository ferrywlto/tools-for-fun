using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace com.verdantsparks.csharp.tools.game
{
    public class Parser
    {
        public void extractMatchupInfo(List<HtmlNode> fromList, Dictionary<string, ChampionMatchupEntry> toDictionary)
        {
            foreach (HtmlNode node in fromList)
            {
                HtmlNode _nameNode = getDivBlock(node, "name");
                if (toDictionary.ContainsKey(_nameNode.InnerText))
                    continue;

                ChampionMatchupEntry entry = new ChampionMatchupEntry();
                HtmlNode upNode = getDivBlock(node, "uvote tag_green");
                HtmlNode downNode = getDivBlock(node, "dvote tag_red");

                entry.champName = _nameNode.InnerText;
                entry.upVote = uint.Parse(upNode.FirstChild.NextSibling.InnerText.Replace(",", string.Empty));
                entry.downVote = uint.Parse(downNode.FirstChild.NextSibling.InnerText.Replace(",", string.Empty));
                entry.rate = (float)(entry.upVote) / (float)(entry.upVote + entry.downVote);
                toDictionary.Add(entry.champName, entry);
            }
        }

        public List<HtmlNode> getChampDivList(HtmlNode node)
        {
            List<HtmlNode> champList = node.Descendants().Where
                (x => (x.Name == "div" &&
                    x.Attributes["class"] != null &&
                    (
                        x.Attributes["class"].Value.Contains("champ-block") ||
                        x.Attributes["class"].Value.Contains("champ-block hidden"))
                    )
                ).ToList();
            return champList;
        }

        public HtmlNode getDivBlock(HtmlDocument htmlDoc, string blockName)
        {
            HtmlNode champNode = htmlDoc.DocumentNode.Descendants().Where
            (x => (x.Name == "div" &&
                x.Attributes["class"] != null &&
                x.Attributes["class"].Value.Contains(blockName))).First();
            return champNode;
        }

        public HtmlNode getDivBlock(HtmlNode htmlNode, string blockName)
        {
            HtmlNode champNode = htmlNode.Descendants().Where
            (x => (x.Name == "div" &&
                x.Attributes["class"] != null &&
                x.Attributes["class"].Value.Contains(blockName))).First();
            return champNode;
        }

        public void parseChampStats(HtmlDocument htmlDoc, Dictionary<string, ChampionStats> outputTo)
        {
            HtmlNode infoNode = getDivBlock(htmlDoc, "champion-stats");
            HtmlNode nameNode = getDivBlock(infoNode, "name");
            string champName = nameNode.InnerText;

            if (outputTo.ContainsKey(champName))
                return;

            ChampionStats stats = new ChampionStats();
            stats.champName = champName;

            HtmlNode allNode = getDivBlock(htmlDoc, "block3 _all");
            HtmlNode weakNode = getDivBlock(allNode, "weak-block");
            HtmlNode strongNode = getDivBlock(allNode, "strong-block");
            HtmlNode goodNode = getDivBlock(allNode, "good-block");

            List<HtmlNode> weakList = getChampDivList(weakNode);
            List<HtmlNode> strongList = getChampDivList(strongNode);
            List<HtmlNode> goodList = getChampDivList(goodNode);

            extractMatchupInfo(weakList, stats.weak);
            extractMatchupInfo(strongList, stats.strong);
            extractMatchupInfo(goodList, stats.partners);

            outputTo.Add(stats.champName, stats);
        }
    }
}
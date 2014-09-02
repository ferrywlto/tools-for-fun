using System.Collections.Generic;

namespace com.verdantsparks.csharp.tools.game
{
    public class ChampionStats
    {
        public string champName = string.Empty;
        public Dictionary<string, ChampionMatchupEntry> partners = new Dictionary<string, ChampionMatchupEntry>();
        public Dictionary<string, ChampionMatchupEntry> strong = new Dictionary<string, ChampionMatchupEntry>();
        public Dictionary<string, ChampionMatchupEntry> weak = new Dictionary<string, ChampionMatchupEntry>();
    }

    public class ChampionMatchupEntry
    {
        public string champName = string.Empty;
        public uint downVote = 0;
        public float rate = 0;
        public uint upVote = 0;
    }
}
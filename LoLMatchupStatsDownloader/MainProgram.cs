using System;
using System.Threading;

namespace com.verdantsparks.csharp.tools.game
{
    internal class MainProgram
    {
        private static string url2Parse = "http://www.championselect.net/champions";

        private static void Main(string[] args)
        {
            if (args.Length == 1)
                url2Parse = args[0];

            Console.WriteLine("=== League of Legends Matchup Stats Downloader ===");

            DateTime startup = DateTime.Now;
            try
            {
                LoLMatchupStatsDownloader downloader = new LoLMatchupStatsDownloader();
                Console.WriteLine(string.Format("Downloading source from: {0}", url2Parse));
                Console.WriteLine(string.Format("[{0}] Start parsing...", DateTime.Now));
                downloader.start(url2Parse);
                while (!downloader.WorkFinish)
                    Thread.Sleep(100);
            }
            catch (Exception e)
            {
                if (e.Message == LoLMatchupStatsDownloader.ERR_WORK_FINISHED)
                {
#if Debug
                    Console.WriteLine(ERR_WORK_FINISHED);
#endif
                }
            }
            Console.WriteLine(string.Format("[{0}] Done.", DateTime.Now));
            Console.WriteLine(String.Format("Time taken: {0} seconds", (DateTime.Now - startup).Seconds));
            Console.ReadLine();
        }
    }
}
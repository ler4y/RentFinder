using log4net;
using RentFinder.Base.BL;
using System;
using System.Configuration;
using System.IO;

namespace RentFinder.Roma
{
    class Program
    {
        private static ILog Logger = LogManager.GetLogger("RentFinder");

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ForRoma();
            Console.WriteLine("Press ENTER to exit app");
            Console.ReadLine();
        }

        private static void ForRoma()
        {
            var link = ConfigurationManager.AppSettings["LinkToProcess"];
            var maxAdsForNumber = uint.Parse(ConfigurationManager.AppSettings["MaxAdsForNumber"]);
            var minPrice = double.Parse(ConfigurationManager.AppSettings["MinPrice"]);
            var maxPrice = double.Parse(ConfigurationManager.AppSettings["MaxPrice"]);

            var search = new OlxSearch(Logger);
            var forReport = search.GetReport(link, maxAdsForNumber, minPrice, maxPrice);

            using (var sw = new StreamWriter("Result_report.txt"))
            {
                sw.WriteLine("Ads count: {0}", forReport.Count);
                foreach (var ad in forReport)
                {
                    sw.WriteLine(ad.Link);
                }
            }

            Logger.Info("Done!");
        }
    }
}
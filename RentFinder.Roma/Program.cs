using AngleSharp;
using AngleSharp.Parser.Html;
using log4net;
using MoreLinq;
using RentFinder.Core;
using RentFinder.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

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
            var res = GetLinks(link);
            res = res.DistinctBy(s => s.TempId).ToList();
            var blackNumberManager = new BlackNumberManager();
            blackNumberManager.BulkAdd(res);
            var maxAdsForNumber = uint.Parse(ConfigurationManager.AppSettings["MaxAdsForNumber"]);
            var blackNumbers = blackNumberManager.GetBlackNumbers(maxAdsForNumber);

            var minPrice = double.Parse(ConfigurationManager.AppSettings["MinPrice"]);
            var maxPrice = double.Parse(ConfigurationManager.AppSettings["MaxPrice"]);

            var forReport = res.Where(s => s.PhoneNumbers.All(c => !blackNumbers.Contains(c))).ToList();
            forReport = res.Where(s => s.Price > minPrice && s.Price < maxPrice).ToList();

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

        private static List<AdModel> GetLinks(string link)
        {
            var res = new List<AdModel>();
            var brContextFactory = new BrowsingContextFactory();
            Logger.Info(string.Format("Start processing for: {0}", link));
            var linkPage = "?page={0}";
            var regexPattern = "ID(.*).html";
            var brContext = brContextFactory.GetNew();
            var processedIds = new List<string>();
            var pagesCount = GetPagesCount(link, brContextFactory.GetNew());
            Logger.Info(string.Format("Total pages: {0}", pagesCount));
            for (int i = 1; i <= pagesCount; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.WriteLine("Processing page: {0}", i);
                    var procLink = i == 1 ? link : link + string.Format(linkPage, i);
                    var task = brContext.OpenAsync(procLink);
                    var doc = task.Result;
                    var rawLinks = doc.QuerySelectorAll(".marginright5.link.linkWithHash.detailsLink");
                    if (rawLinks.Count() == 0) { Thread.Sleep(3000); }
                    else
                    {
                        var k = 1;
                        foreach (var rawLink in rawLinks)
                        {
                            try
                            {
                                Logger.Debug(string.Format("\tProcessing link: {0}", k));
                                var resLink = new AdModel { Link = rawLink.Attributes["href"].Value };
                                resLink.TempId = Regex.Match(resLink.Link, regexPattern).Groups[1].Value;
                                if (!processedIds.Contains(resLink.TempId))
                                {
                                    var priceTask = brContextFactory.GetNew().OpenAsync(rawLink.Attributes["href"].Value);
                                    resLink.PhoneNumbers.AddRange(GetPhoneNumbers(resLink.TempId, brContextFactory.GetNew()));
                                    var docAd = priceTask.Result;
                                    var priceString = docAd.QuerySelector(".pricelabel.tcenter").Children[0].InnerHtml;
                                    string value = Regex.Replace(priceString, "[А-Яа-яA-Za-z$ .]", "");
                                    resLink.Price = double.Parse(value);
                                    string rooms = docAd.QuerySelector("#offerdescription > div.clr.descriptioncontent.marginbott20 > table > tbody > tr:nth-child(2) > td.col > table > tbody > tr > td > strong").InnerHtml;
                                    resLink.Rooms = int.Parse(Regex.Replace(rooms, "[А-Яа-яA-Za-z .]", ""));
                                    res.Add(resLink);
                                    processedIds.Add(resLink.TempId);
                                    k++;
                                }
                                else Logger.Warn("Dublicate");

                            }
                            catch (Exception ex)
                            {
                               Logger.Error(ex.ToString());
                            }
                        }
                        break;
                    }

                }
                Console.WriteLine();
            }

            return res;
        }

        private static List<string> GetPhoneNumbers(string tempId, IBrowsingContext brContext)
        {
            var result = new List<string>();
            var regexNumberPattern = "\"value\":\"(.*)\"";
            var numberLinkPattern = "http://www.olx.ua/ajax/misc/contact/phone/{0}/white/";
            var numberTask = brContext.OpenAsync(string.Format(numberLinkPattern, tempId));
            var docNumber = numberTask.Result;
            var values = Regex.Match(docNumber.Body.InnerHtml, regexNumberPattern);
            if (values.Groups.Count > 1)
            {
                var numbersString = values.Groups[1].Value;
                if (numbersString.Contains("span"))
                {
                    var parser = new HtmlParser();
                    numbersString = numbersString.Replace("&lt;", "<").Replace("&gt;", ">").Replace("\\\"", "\"").Replace("\\/", "/");
                    var document = parser.Parse(numbersString);
                    var numbers = document.QuerySelectorAll(".block").Select(s => PrepareNumber(s.InnerHtml));
                    result.AddRange(numbers);
                }
                else
                {
                    if (numbersString != string.Empty)
                        result.Add(PrepareNumber(numbersString));
                }
            }
            else
            {
                Console.WriteLine("No number");
            }
            return result;
        }

        private static string PrepareNumber(string input)
        {
            input = Regex.Replace(input, @"[()\-\+]|\s|[a-zA-Z]", "");
            return Regex.Replace(input, @"^38|^8", "");
        }

        private static int GetPagesCount(string link, IBrowsingContext brContext)
        {
            var doc = brContext.OpenAsync(link).Result;
            var pager = doc.QuerySelector("#body-container > div:nth-child(3) > div > div.pager.rel.clr");
            var spans = pager.QuerySelectorAll(".item.fleft");
#if DEBUG
            return 1;
#else
            return spans.Select(s=> 
                {
                    int pageNumber;
                    if (int.TryParse(s.Children.First().Children.First().InnerHtml, out pageNumber)) return pageNumber;
                    else return -1;
                    
                }).Max();
#endif
        }

        //private static List<PreviewAdModel> GetPreviewModels(string link)
        //{
        //    var res = new List<PreviewAdModel>();
        //    var brContextFactory = new BrowsingContextFactory();
        //    var linkPage = "?page={0}";

        //    var pagesCount = GetPagesCount(link, brContextFactory.GetNew());
        //    for (int i = 1; i <= pagesCount; i++)
        //    {

        //        var procLink = i == 1 ? link : link + string.Format(linkPage, i);

        //        res.AddRange(GetPreviewModels(brContextFactory, procLink));
        //        System.Console.WriteLine();
        //    }

        //    return res;
        //}

        //private static List<PreviewAdModel> GetPreviewModels(BrowsingContextFactory brContextFactory, string procLink)
        //{
        //    List<PreviewAdModel> res = new List<PreviewAdModel>();
        //    for (int j = 0; j < 3; j++)
        //    {
        //        Logger.Info(string.Format("Processing page: {0}", procLink));
        //        var regexPattern = "ID(.*).html";
        //        var brContext = brContextFactory.GetNew();
        //        var task = brContext.OpenAsync(procLink);
        //        var doc = task.Result;
        //        var offers = doc.QuerySelectorAll(".offer");
        //        if (!offers.Any())
        //        {
        //            Thread.Sleep(3000);
        //        }
        //        else
        //        {
        //            var k = 1;
        //            foreach (var offer in offers)
        //            {
        //                try
        //                {
        //                    Logger.Debug(string.Format("\tProcessing offer: {0}", k));
        //                    var resLink = new PreviewAdModel();
        //                    resLink.AdId = uint.Parse(offer.Children.First().Attributes["data-id"].Value);
        //                    resLink.TempId =
        //                        Regex.Match(offer.QuerySelector(".detailsLink").Attributes["href"].Value, regexPattern).Groups[1
        //                            ].Value;
        //                    var priceString = offer.QuerySelector(".price").Children.First().InnerHtml;
        //                    resLink.Price = double.Parse(Regex.Replace(priceString, "[А-Яа-яA-Za-z$ .]", ""));
        //                    resLink.PhoneNumbers.AddRange(GetPhoneNumbers(resLink.TempId, brContextFactory.GetNew()));
        //                    res.Add(resLink);
        //                    k++;
        //                }
        //                catch (Exception ex)
        //                {
        //                    Logger.Error(ex.ToString());
        //                }
        //            }
        //            break;
        //        }
        //    }
        //    return res;
        //}
    }
}

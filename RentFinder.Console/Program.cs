using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp;
using System.Text.RegularExpressions;
using MoreLinq;
using System.IO;
using System.Threading;
using RentFinder.Model;
using RentFinder.Core;
using AngleSharp.Parser.Html;
using System.Net;
using AngleSharp.Network.Default;

namespace RentFinder.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            TestBlackNumberManager();   
            System.Console.ReadLine();
        }

        private static void TestBlackNumberManager()
        {
            var manager = new BlackNumberManager();
            using (var sw = new FileStream("blackNumbers.txt", FileMode.Open))
            {
                manager.LoadFromStream(sw);
            }
            var linkPattern = "https://www.olx.ua/nedvizhimost/arenda-kvartir/dolgosrochnaya-arenda-kvartir/dnepropetrovsk/";// "https://www.olx.ua/nedvizhimost/arenda-kvartir/dolgosrochnaya-arenda-kvartir/"; //"https://www.olx.ua/nedvizhimost/arenda-kvartir/dolgosrochnaya-arenda-kvartir/dnepropetrovsk/?search%5Bfilter_float_number_of_rooms%3Afrom%5D=2&search%5Bfilter_float_number_of_rooms%3Ato%5D=3";//"http://www.olx.ua/nedvizhimost/arenda-kvartir/dolgosrochnaya-arenda-kvartir/dnepropetrovsk/"; //"http://www.olx.ua/nedvizhimost/arenda-domov/dnepropetrovsk/";
            //System.Console.WriteLine("Pages count: {0}", GetPagesCount(linkPattern));
            var previewAdModels = GetPreviewModels(linkPattern);
            manager.BulkAdd(previewAdModels);
            System.Console.WriteLine(manager.GetShortReport());
            using (var sw = new StreamWriter("long_report.txt"))
            {
                sw.Write(manager.GetLongReport());
            }

            using (var sw = new FileStream("blackNumbers.txt", FileMode.OpenOrCreate))
            {
                manager.SaveToStream(sw);
            }
        }

        private static void ForKovalchuk()
        {
            var res = GetLinks();
            res = res.DistinctBy(s => s.TempId).ToList();
            var grouped = res.GroupBy(s => s.PhoneNumbers.First()).OrderBy(s => s.Count());
            var blackNumbersSoft = grouped.Where(s => s.Count() > 2).Select(s => s.Key).ToList();
            var blackNumbersHard = grouped.Where(s => s.Count() > 1).Select(s => s.Key).ToList();
            var withoutRieltorSoft = res.Where(s => !blackNumbersSoft.Contains(s.PhoneNumbers.First())).ToList();
            var withoutRieltorHard = res.Where(s => !blackNumbersHard.Contains(s.PhoneNumbers.First())).ToList();
            var room2VariantSoft = withoutRieltorSoft.Where(s => s.Rooms == 2 && s.Price <= 4000).OrderBy(s => s.Price).ToList();
            var room2VariantHard = withoutRieltorHard.Where(s => s.Rooms == 2 && s.Price <= 4000).OrderBy(s => s.Price).ToList();
            var room3VariantSoft = withoutRieltorSoft.Where(s => s.Rooms == 3 && s.Price <= 4500).OrderBy(s => s.Price).ToList();
            var room3VariantHard = withoutRieltorHard.Where(s => s.Rooms == 3 && s.Price <= 4500).OrderBy(s => s.Price).ToList();

            using (var sw = new StreamWriter("2room_soft.txt"))
            {
                sw.WriteLine("Ads count: {0}", room2VariantSoft.Count);
                foreach (var ad in room2VariantSoft)
                {
                    sw.Write(ad.ToString());
                }
            }

            using (var sw = new StreamWriter("2room_hard.txt"))
            {
                sw.WriteLine("Ads count: {0}", room2VariantHard.Count);
                foreach (var ad in room2VariantHard)
                {
                    sw.Write(ad.ToString());
                }
            }

            using (var sw = new StreamWriter("3room_soft.txt"))
            {
                sw.WriteLine("Ads count: {0}", room3VariantSoft.Count);
                foreach (var ad in room3VariantSoft)
                {
                    sw.Write(ad.ToString());
                }
            }

            using (var sw = new StreamWriter("3room_hard.txt"))
            {
                sw.WriteLine("Ads count: {0}", room3VariantHard.Count);
                foreach (var ad in room3VariantHard)
                {
                    sw.Write(ad.ToString());
                }
            }
            System.Console.WriteLine("Done!");
        }

        private static List<PreviewAdModel> GetPreviewModels(string link)
        {
            var res = new List<PreviewAdModel>();
            var brContextFactory = new BrowsingContextFactory();
            var linkPage = "?page={0}";
            var regexPattern = "ID(.*).html";

           
            var processedIds = new List<string>();
            var pagesCount = GetPagesCount(link, brContextFactory.GetNew());
            for (int i = 1; i <= pagesCount; i++)
            {
                var requester = new HttpRequester();
                requester.Headers["User-Agent"] = Guid.NewGuid().ToString().Replace("-","");
                var configuration = Configuration.Default.WithDefaultLoader(requesters: new[] { requester });
                var brContext = BrowsingContext.New(configuration);
                for (int j = 0; j < 3; j++)
                {
                    System.Console.WriteLine("Processing page: {0}", i);
                    var procLink = i == 1 ? link : link + string.Format(linkPage, i);
                    var task = brContext.OpenAsync(procLink);
                    var doc = task.Result;
                    var offers = doc.QuerySelectorAll(".offer");
                    if (offers.Count() == 0) { Thread.Sleep(3000); }
                    else
                    {
                        var k = 1;
                        foreach (var offer in offers)
                        {
                            try
                            {
                                System.Console.WriteLine("\tProcessing offer: {0}", k);
                                var resLink = new PreviewAdModel();
                                resLink.AdId = uint.Parse(offer.Children.First().Attributes["data-id"].Value);
                                resLink.TempId = Regex.Match(offer.QuerySelector(".detailsLink").Attributes["href"].Value, regexPattern).Groups[1].Value;
                                var priceString = offer.QuerySelector(".price").Children.First().InnerHtml;
                                resLink.Price = double.Parse(Regex.Replace(priceString, "[А-Яа-яA-Za-z$ .]", ""));
                                resLink.PhoneNumbers.AddRange(GetPhoneNumbers(resLink.TempId, brContextFactory.GetNew()));
                                res.Add(resLink);
                                k++;

                            }
                            catch (Exception ex)
                            {
                                System.Console.WriteLine(ex.Message);
                            }
                        }
                        break;
                    }

                }
                System.Console.WriteLine();
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
            if(values.Groups.Count > 1)
            {
                var numbersString = values.Groups[1].Value;
                if (numbersString.Contains("span"))
                {
                    var parser = new HtmlParser();
                    numbersString = numbersString.Replace("&lt;", "<").Replace("&gt;", ">").Replace("\\\"", "\"").Replace("\\/", "/");
                    var document = parser.Parse(numbersString);
                    var numbers = document.QuerySelectorAll(".block").Select(s=> PrepareNumber(s.InnerHtml));
                    result.AddRange(numbers);
                }
                else
                {
                    if(numbersString != string.Empty)
                    result.Add(PrepareNumber(numbersString));
                }
            }
            else
            {
                System.Console.WriteLine("No number");
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
#if RELEASE
            return spans.Select(s=> 
                {
                    int pageNumber;
                    if (int.TryParse(s.Children.First().Children.First().InnerHtml, out pageNumber)) return pageNumber;
                    else return -1;
                    
                }).Max();
#else
            return 1;
#endif
        }

        private static List<AdModel> GetLinks()
        {
            var res = new List<AdModel>();

            var linkPattern = "https://www.olx.ua/nedvizhimost/arenda-kvartir/dolgosrochnaya-arenda-kvartir/dnepropetrovsk/?search%5Bfilter_float_number_of_rooms%3Afrom%5D=2&search%5Bfilter_float_number_of_rooms%3Ato%5D=3";//"http://www.olx.ua/nedvizhimost/arenda-kvartir/dolgosrochnaya-arenda-kvartir/dnepropetrovsk/"; //"http://www.olx.ua/nedvizhimost/arenda-domov/dnepropetrovsk/";
            var linkPage = "?page={0}";
            var regexPattern = "ID(.*).html";
            var regexNumberPattern = "\"value\":\"(.*)\"";
            var numberLinkPattern = "http://www.olx.ua/ajax/misc/contact/phone/{0}/white/";
            var brContext = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
            var processedIds = new List<string>();
            for (int i = 1; i <= 26; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    System.Console.WriteLine("Processing page: {0}", i);
                    var link = i == 1 ? linkPattern : linkPattern + string.Format(linkPage, i);
                    var task = brContext.OpenAsync(link);
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
                                System.Console.WriteLine("\tProcessing link: {0}", k);
                                var resLink = new AdModel { Link = rawLink.Attributes["href"].Value };
                                resLink.TempId = Regex.Match(resLink.Link, regexPattern).Groups[1].Value;
                                if (!processedIds.Contains(resLink.TempId))
                                {
                                    var numberTask = brContext.OpenAsync(string.Format(numberLinkPattern, resLink.TempId));
                                    var priceTask = brContext.OpenAsync(rawLink.Attributes["href"].Value);
                                    var docNumber = numberTask.Result;
                                    resLink.PhoneNumbers.Add(Regex.Match(docNumber.Body.InnerHtml, regexNumberPattern).Groups[1].Value);
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
                                else System.Console.WriteLine("Dublicate");

                            }
                            catch (Exception ex)
                            {
                                System.Console.WriteLine(ex.Message);
                            }
                        }
                        break;
                    }
                   
                }
                System.Console.WriteLine();
            }

            return res;
        }
    }
}

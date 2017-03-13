using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using AngleSharp;
using AngleSharp.Parser.Html;
using log4net;
using MoreLinq;
using RentFinder.Model;

namespace RentFinder.Base.BL
{

    public class ProgressChangedEventArgs
    {
        public int ProgressInPercents { get; set; }
        public int StartPage { get; set; }
        public int StopPage { get; set; }
        public int CurrentPage { get; set; }
        public int StartAd { get; set; }
        public int StopAd { get; set; }
        public int CurrentAd { get; set; }
    }
    public class OlxSearch
    {
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged; 
        protected ILog Logger { get; private set; }
        public OlxSearch(ILog logger)
        {
            Logger = logger;
        }

        public List<AdModel> GetReport(string link, uint maxAdsForNumber, double minPrice, double maxPrice)
        {
            var res = GetLinks(link);
            res = res.DistinctBy(s => s.TempId).ToList();
            var blackNumberManager = new BlackNumberManager();
            blackNumberManager.BulkAdd(res);
            var blackNumbers = blackNumberManager.GetBlackNumbers(maxAdsForNumber);

            var forReport = res.Where(s => s.IsPrivate).ToList();
            forReport = forReport.Where(s => s.PhoneNumbers.All(c => !blackNumbers.Contains(c))).ToList();
            forReport = forReport.Where(s => s.Price > minPrice && s.Price < maxPrice).ToList();
            return forReport.OrderBy(s=>s.Link).ToList();
        }

        private List<AdModel> GetLinks(string link)
        {
            var res = new List<AdModel>();
            var brContextFactory = new BrowsingContextFactory();
            Logger.Info(String.Format("Start processing for: {0}", link));
            var linkPage = "?page={0}";
            var regexPattern = "ID(.*).html";
            var brContext = brContextFactory.GetNew();
            var processedIds = new List<string>();
            var pagesCount = GetPagesCount(link, brContextFactory.GetNew());
            Logger.Info(String.Format("Total pages: {0}", pagesCount));
            for (int i = 1; i <= pagesCount; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Logger.Info(String.Format("Processing page: {0}", i));
                    var procLink = i == 1 ? link : link + String.Format(linkPage, i);
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
                                Logger.Debug(String.Format("\tProcessing link: {0}", k));
                                var resLink = new AdModel {Link = rawLink.Attributes["href"].Value};
                                resLink.TempId = Regex.Match(resLink.Link, regexPattern).Groups[1].Value;
                                if (!processedIds.Contains(resLink.TempId))
                                {
                                    var priceTask = brContextFactory.GetNew()
                                        .OpenAsync(rawLink.Attributes["href"].Value);
                                    resLink.PhoneNumbers.AddRange(GetPhoneNumbers(resLink.TempId,
                                        brContextFactory.GetNew()));
                                    var docAd = priceTask.Result;
                                    var priceString = docAd.QuerySelector(".pricelabel.tcenter").Children[0].InnerHtml;
                                    var address = docAd.QuerySelector("#offerdescription > div.offer-titlebox > div.offer-titlebox__details > a > strong").InnerHtml;
                                    resLink.Address = address;
                                    var isPrivateString =
                                        docAd.QuerySelector(
                                            "#offerdescription > div.clr.descriptioncontent.marginbott20 > table > tbody > tr:nth-child(1) > td:nth-child(1) > table > tbody > tr > td > strong > a")
                                            .InnerHtml;
                                    isPrivateString = Regex.Replace(isPrivateString, "[\t\n]", "");
                                    resLink.IsPrivate = !isPrivateString.Equals("Бизнес");
                                    string value = Regex.Replace(priceString, "[А-Яа-яA-Za-z$ .]", "");
                                    resLink.Price = Double.Parse(value);
                                    string rooms =
                                        docAd.QuerySelector(
                                            "#offerdescription > div.clr.descriptioncontent.marginbott20 > table > tbody > tr:nth-child(2) > td.col > table > tbody > tr > td > strong")
                                            .InnerHtml;
                                    resLink.Rooms = Int32.Parse(Regex.Replace(rooms, "[А-Яа-яA-Za-z .]", ""));
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
                            finally
                            {
                                if (ProgressChanged != null)
                                {
                                    ProgressChanged(this, new ProgressChangedEventArgs()
                                    {
                                        CurrentAd = k,
                                        StartAd = 1,
                                        StopAd = rawLinks.Count(),
                                        StartPage = 1,
                                        StopPage = pagesCount,
                                        CurrentPage = i
                                    });
                                }
                            }
                            Thread.Sleep(1000);
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
            var numberTask = brContext.OpenAsync(String.Format(numberLinkPattern, tempId));
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
                    if (numbersString != String.Empty)
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
    }
}

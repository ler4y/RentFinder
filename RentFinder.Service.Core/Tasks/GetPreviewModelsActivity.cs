using System;
using AngleSharp;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using AngleSharp.Parser.Html;
using RentFinder.Base;
using RentFinder.Model;

namespace RentFinder.Service.Core.Tasks
{
    public class GetPreviewModelsActivity : IActivity<IEnumerable<PreviewAdModel>>
    {
        private readonly BrowsingContextFactory _brContextFactory;
        private readonly string _link;
        private readonly uint _maxEfforts;
        public GetPreviewModelsActivity(string link, BrowsingContextFactory brContextFactory, uint maxEfforts = 3)
        {
            _link = link;
            _brContextFactory = brContextFactory;
            _maxEfforts = maxEfforts;
        }

        public void Run()
        {
            Result = GetPreviewModels();
        }

        public IEnumerable<PreviewAdModel> Result { get; private set; }

        private List<PreviewAdModel> GetPreviewModels()
        {
            List<PreviewAdModel> res = new List<PreviewAdModel>();
            for (int j = 0; j < _maxEfforts; j++)
            {
                //TODO: Create logging
                Console.WriteLine("Processing page: {0}", _link);
                var regexPattern = "ID(.*).html";
                var brContext = _brContextFactory.GetNew();
                var task = brContext.OpenAsync(_link);
                var doc = task.Result;
                var offers = doc.QuerySelectorAll(".offer");
                if (!offers.Any())
                {
                    Thread.Sleep(3000);
                }
                else
                {
                    var k = 1;
                    foreach (var offer in offers)
                    {
                        try
                        {
                            // TODO: Create logging
                            Console.WriteLine("\tProcessing offer: {0}", k);
                            var resLink = new PreviewAdModel();
                            resLink.AdId = uint.Parse(offer.Children.First().Attributes["data-id"].Value);
                            resLink.TempId =
                                Regex.Match(offer.QuerySelector(".detailsLink").Attributes["href"].Value, regexPattern).Groups[1
                                    ].Value;
                            var priceString = offer.QuerySelector(".price").Children.First().InnerHtml;
                            resLink.Price = double.Parse(Regex.Replace(priceString, "[А-Яа-яA-Za-z$ .]", ""));
                            resLink.PhoneNumbers.AddRange(GetPhoneNumbers(resLink.TempId, _brContextFactory.GetNew()));
                            res.Add(resLink);
                            k++;
                        }
                        catch (Exception ex)
                        { 
                            //TODO: Create logging
                            Console.WriteLine(ex.Message);
                            System.Diagnostics.Debug.WriteLine(ex.ToString());
                        }
                    }
                    break;
                }
            }
            return res;
        }

        private List<string> GetPhoneNumbers(string tempId, IBrowsingContext brContext)
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
                //TODO: Create logging
                //System.Console.WriteLine("No number");
            }
            return result;
        }

        private static string PrepareNumber(string input)
        {
            input = Regex.Replace(input, @"[()\-\+]|\s|[a-zA-Z]", "");
            return Regex.Replace(input, @"^38|^8", "");
        }
    }
}
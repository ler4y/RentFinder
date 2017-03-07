using System.Configuration;
using System.IO;
using System.Windows.Controls;
using RentFinder.Base.BL;
using System.Threading.Tasks;
using RentFinder.Model;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RentFinder.WPF.Client
{
    /// <summary>
    /// Interaction logic for SearchProgress.xaml
    /// </summary>
    public partial class SearchProgressPage : NavigationPage
    {
        public SearchProgressPage(Frame parentFrame) : base(parentFrame)
        {
            InitializeComponent();
            Task.Run(()=>ForRoma()).ContinueWith(x=> this.Dispatcher.Invoke(()=> ParentFrame.Navigate(new ResultsPage(parentFrame, x.Result))));
        }

        private List<AdModel> ForRoma()
        {
            var link = ConfigurationManager.AppSettings["LinkToProcess"];
            var maxAdsForNumber = uint.Parse(ConfigurationManager.AppSettings["MaxAdsForNumber"]);
            var minPrice = double.Parse(ConfigurationManager.AppSettings["MinPrice"]);
            var maxPrice = double.Parse(ConfigurationManager.AppSettings["MaxPrice"]);

            var search = new OlxSearch(App.Logger);
            search.ProgressChanged += Search_ProgressChanged;
            var forReport = search.GetReport(link, maxAdsForNumber, minPrice, maxPrice);

            using (var sw = new StreamWriter("Result_report.txt"))
            {  
                sw.Write(JsonConvert.SerializeObject(forReport));
            }

            App.Logger.Info("Done!");
            return forReport;
        }

        private void Search_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                pagesProgressBar.Minimum = e.StartPage;
                pagesProgressBar.Maximum = e.StopPage;
                pagesProgressBar.Value = e.CurrentPage;
                adsProgressBar.Minimum = e.StartAd;
                adsProgressBar.Maximum = e.StopAd;
                adsProgressBar.Value = e.CurrentAd;
            });
        }
    }
}

using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RentFinder.Base.BL;

namespace RentFinder.WPF.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new StartPage(MainFrame));
        }

        private void ForRoma()
        {
            var link = ConfigurationManager.AppSettings["LinkToProcess"];
            var maxAdsForNumber = uint.Parse(ConfigurationManager.AppSettings["MaxAdsForNumber"]);
            var minPrice = double.Parse(ConfigurationManager.AppSettings["MinPrice"]);
            var maxPrice = double.Parse(ConfigurationManager.AppSettings["MaxPrice"]);

            var search = new OlxSearch(App.Logger);
            var forReport = search.GetReport(link, maxAdsForNumber, minPrice, maxPrice);

            using (var sw = new StreamWriter("Result_report.txt"))
            {
                sw.WriteLine("Ads count: {0}", forReport.Count);
                foreach (var ad in forReport)
                {
                    sw.WriteLine(ad.Link);
                }
            }

            App.Logger.Info("Done!");
        }
       
    }
}

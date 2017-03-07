using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using RentFinder.Model;

namespace RentFinder.WPF.Client
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : NavigationPage
    {
        public StartPage(Frame parentFrame) :base(parentFrame)
        {
            InitializeComponent();
        }

        private void openLastBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var sr = new StreamReader("Result_report.txt"))
            {
                ParentFrame.Navigate(new ResultsPage(ParentFrame, JsonConvert.DeserializeObject<List<AdModel>>(sr.ReadToEnd())));
            }
        }

        private void newSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            ParentFrame.Navigate(new SearchProgressPage(ParentFrame));
        }
    }
}

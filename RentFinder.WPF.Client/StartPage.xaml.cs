using System.Windows;
using System.Windows.Controls;

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
           ParentFrame.Navigate(new ResultsPage(ParentFrame));
        }

        private void newSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            ParentFrame.Navigate(new SearchProgressPage(ParentFrame));
        }
    }
}

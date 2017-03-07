using System.Configuration;
using System.IO;
using System.Windows;
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
    }
}

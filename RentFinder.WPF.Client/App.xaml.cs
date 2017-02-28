using log4net;
using System.Windows;

namespace RentFinder.WPF.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ILog Logger = LogManager.GetLogger("RentFinder");
        static App()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}

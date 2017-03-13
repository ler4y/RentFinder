using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using System.Windows;
using Yort.Ntp;

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
            var client = new NtpClient();
            var result =  client.RequestTimeAsync().Result;
            var timetoDie = new DateTime(2017, 4, 1, 1,1,1);
            if (result < timetoDie)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(timetoDie - result);
                    Current.Shutdown();
                });
            }else Current.Shutdown();
        }
    }
}

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
            var timetoDie = new DateTime(2017, 6, 15, 0,0,0);
            if (result < timetoDie)
            {
                Task.Run(() =>
                {
                    var timeToSleep = (timetoDie - result).TotalMilliseconds;
                    Thread.Sleep(timeToSleep > int.MaxValue ? Int32.MaxValue : (int)timeToSleep);
                    Current.Shutdown();
                });
            }else Current.Shutdown();
        }
    }
}

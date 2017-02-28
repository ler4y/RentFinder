using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RentFinder.WPF.Client
{
    /// <summary>
    /// Interaction logic for ResultsPage.xaml
    /// </summary>
    public partial class ResultsPage : Page
    {
        public ResultsPage()
        {
            InitializeComponent();
            listbox1.SelectionChanged += (sender, args) => browser.Load(listbox1.SelectedValue.ToString());
        }
        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            listbox1.Items.Clear();
            var links = (sender as TextBox).Text.Split(new[] { "\n" }, StringSplitOptions.None).ToList();
            foreach (var link in links)
            {
                listbox1.Items.Add(link);
            }
        }
    }
}

using RentFinder.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace RentFinder.WPF.Client
{
    /// <summary>
    /// Interaction logic for ResultsPage.xaml
    /// </summary>
    public partial class ResultsPage : NavigationPage
    {
        private List<AdModel> _results;
        public ResultsPage(Frame parentFrame):base(parentFrame)
        {
            InitializeComponent();
            listbox1.SelectionChanged += (sender, args) =>
            {
                if (listbox1.SelectedValue != null) browser.Load(listbox1.SelectedValue.ToString());
            };
        }

        public ResultsPage(Frame parentFrame, List<AdModel> data) : this(parentFrame)
        {
            var variants = new List<string> {"Все"};
            variants.AddRange(data.Select(s => s.Area).Distinct());
            areasCmb.ItemsSource = variants;
            areasCmb.SelectionChanged += AreasCmb_SelectionChanged;
            _results = data;
            areasCmb.SelectedItem = "Все";
            listbox1.DisplayMemberPath = "Title";
            listbox1.SelectedValuePath = "Link";
        }

        private void AreasCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listbox1.ItemsSource = (string)areasCmb.SelectedValue == "Все" 
                ? _results 
                : _results.Where(s => s.Area == (string)areasCmb.SelectedValue);
            Dispatcher.Invoke(() => listbox1.Items.Refresh());
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

        private void deleteAdBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (listbox1.SelectedItem != null)
            {
                _results.Remove((AdModel)listbox1.SelectedItem);
                Dispatcher.Invoke(() => listbox1.Items.Refresh());
                using (var sw = new StreamWriter("Result_report.txt"))
                {
                    sw.Write(JsonConvert.SerializeObject(listbox1.ItemsSource));
                }
            }
        }
    }
}

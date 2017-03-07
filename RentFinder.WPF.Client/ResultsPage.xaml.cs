using RentFinder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace RentFinder.WPF.Client
{
    /// <summary>
    /// Interaction logic for ResultsPage.xaml
    /// </summary>
    public partial class ResultsPage : NavigationPage
    {
        public ResultsPage(Frame parentFrame):base(parentFrame)
        {
            InitializeComponent();
            listbox1.SelectionChanged += (sender, args) => browser.Load(listbox1.SelectedValue.ToString());
        }

        public ResultsPage(Frame parentFrame, List<AdModel> data) : this(parentFrame)
        {
            listbox1.DisplayMemberPath = "TempId";
            listbox1.SelectedValuePath = "Link";
            listbox1.ItemsSource = data;
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

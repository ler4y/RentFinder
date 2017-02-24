using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            listbox1.SelectionChanged += (sender, args) => browser.Load(listbox1.SelectedValue.ToString());
        }

        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            listbox1.Items.Clear();
            var links = (sender as TextBox).Text.Split(new [] { "\n" }, StringSplitOptions.None).ToList();
            foreach (var link in links)
            {
                listbox1.Items.Add(link);
            }
        }
    }
}

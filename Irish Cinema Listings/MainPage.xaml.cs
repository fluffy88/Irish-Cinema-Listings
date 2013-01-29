using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Irish_Cinema_Listings.Models;

namespace Irish_Cinema_Listings
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            DataContext = App.CountyModel;
        }

        private void CountyListBox_Tap(object sender, GestureEventArgs e)
        {
            var listBox = (ListBox)sender;
            var selectedItem = (ModelItem)listBox.SelectedItem;

            if (selectedItem != null)
            {
                NavigationService.Navigate(new Uri("/CinemaPage.xaml?id=" + selectedItem.Id, UriKind.Relative));
            }
        }
    }
}
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
using Microsoft.Phone.Shell;

namespace Irish_Cinema_Listings
{
    public partial class CinemaPage : PhoneApplicationPage
    {
        private CinemaModel CinemaModel = new CinemaModel();

        public CinemaPage()
        {
            InitializeComponent();

            DataContext = CinemaModel;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            String Id = NavigationContext.QueryString["id"];
            CinemaModel.Load(Id);
        }

        private void CinemaListBox_Tap(object sender, GestureEventArgs e)
        {
            var listBox = (ListBox)sender;
            var selectedItem = (ModelItem)listBox.SelectedItem;

            if (selectedItem != null)
            {
                NavigationService.Navigate(new Uri("/MoviesPage.xaml?id=" + selectedItem.Id, UriKind.Relative));
            }
        }
    }
}
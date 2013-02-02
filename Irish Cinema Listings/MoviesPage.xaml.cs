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
    public partial class MoviesPage : PhoneApplicationPage
    {
        private MoviesModel MoviesModel = new MoviesModel();

        public MoviesPage()
        {
            InitializeComponent();

            DataContext = MoviesModel;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            String id = NavigationContext.QueryString["id"];
            this.MoviesModel.Load(id);
        }

        private void MoviesListBox_Tap(object sender, GestureEventArgs e)
        {
            var listBox = (ListBox)sender;
            var selectedItem = (ModelItem)listBox.SelectedItem;

            if (selectedItem != null)
            {
                String url = HttpUtility.UrlEncode(selectedItem.Url);
                NavigationService.Navigate(new Uri("/MoviePage.xaml?url=" + url, UriKind.Relative));
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Irish_Cinema_Listings.Models;
using System.Windows.Navigation;
using Irish_Cinema_Listings.Utils;
using Microsoft.Phone.Shell;

namespace Irish_Cinema_Listings
{
    public class MoviesModel
    {
        public ObservableCollection<ModelItem> Movies { get; private set; }

        public MoviesModel()
        {
            this.Movies = new ObservableCollection<ModelItem>();
        }

        public void Load(String id)
        {
            HttpWebRequest request = HttpUtils.GetHttpRequest("http://api.entertainment.ie/entertainme/cinemas.asp?id=" + id);
            request.BeginGetResponse(new AsyncCallback(ReadWebRequestCallback), request);
        }

        private void ReadWebRequestCallback(IAsyncResult callbackResult)
        {
            try
            {
                String results = HttpUtils.GetResponse(callbackResult);
                var unsortedItems = JsonUtils.GetItems(results, new String[] { "name", "url" });
                var sortedItems = ModelItem.SortItems(unsortedItems, "name");

                Deployment.Current.Dispatcher.BeginInvoke(() => this.Movies.Clear());
                foreach (Dictionary<String, String> movie in sortedItems)
                {
                    // need to create local variable reference as loop variable cinema will change before UI gets update.
                    ModelItem movieItem = new ModelItem();
                    movieItem.Name = movie["name"];
                    movieItem.Url = JsonUtils.StripSlashes(movie["url"]);
                    Deployment.Current.Dispatcher.BeginInvoke(() => this.Movies.Add(movieItem));
                }
                Deployment.Current.Dispatcher.BeginInvoke(() => SystemTray.ProgressIndicator.IsIndeterminate = false);
            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show(ex.Message));
            }
        }
    }
}
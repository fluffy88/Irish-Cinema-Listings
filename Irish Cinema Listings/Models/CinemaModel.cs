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
    public class CinemaModel
    {
        public ObservableCollection<ModelItem> Cinemas { get; private set; }

        public CinemaModel()
        {
            this.Cinemas = new ObservableCollection<ModelItem>();
        }

        public void Load(String id)
        {
            HttpWebRequest request = HttpUtils.GetHttpRequest("http://api.entertainment.ie/entertainme/cinemas.asp?county=" + id);
            request.BeginGetResponse(new AsyncCallback(ReadWebRequestCallback), request);
        }

        private void ReadWebRequestCallback(IAsyncResult callbackResult)
        {
            try
            {
                String results = HttpUtils.GetResponse(callbackResult);
                var unsortedItems = JsonUtils.GetItems(results, new String[] { "name", "id" });
                var sortedItems = ModelItem.SortItems(unsortedItems, "name");

                Deployment.Current.Dispatcher.BeginInvoke(() => this.Cinemas.Clear());
                foreach (Dictionary<String, String> cinema in sortedItems)
                {
                    // need to create local variable reference as loop variable cinema will change before UI gets update.
                    ModelItem cinemaItem = new ModelItem();
                    cinemaItem.Name = cinema["name"];
                    cinemaItem.Id = cinema["id"];
                    Deployment.Current.Dispatcher.BeginInvoke(() => this.Cinemas.Add(cinemaItem));
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
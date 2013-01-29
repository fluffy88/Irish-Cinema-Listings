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
using Irish_Cinema_Listings.Utils;
using Microsoft.Phone.Shell;

namespace Irish_Cinema_Listings
{
    public class CountyModel
    {
        public ObservableCollection<ModelItem> Counties { get; private set; }

        public CountyModel()
        {
            this.Counties = new ObservableCollection<ModelItem>();

            HttpWebRequest request = HttpUtils.GetHttpRequest("http://api.entertainment.ie/entertainme/listings.asp");
            request.BeginGetResponse(new AsyncCallback(ReadWebRequestCallback), request);
        }

        private void ReadWebRequestCallback(IAsyncResult callbackResult)
        {
            try
            {
                String results = HttpUtils.GetResponse(callbackResult);
                Collection<Dictionary<String, String>> unsortedItems = JsonUtils.GetItems(results, new String[] { "name", "id" });
                Collection<Dictionary<String, String>> sortedItems = ModelItem.SortItems(unsortedItems, "name");

                Deployment.Current.Dispatcher.BeginInvoke(() => this.Counties.Clear());
                foreach (Dictionary<String, String> county in sortedItems)
                {
                    ModelItem countyItem = new ModelItem();
                    countyItem.Name = county["name"];
                    countyItem.Id = county["id"];
                    Deployment.Current.Dispatcher.BeginInvoke(() => this.Counties.Add(countyItem));
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
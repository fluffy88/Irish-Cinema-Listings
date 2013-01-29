using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Collections.ObjectModel;
using Irish_Cinema_Listings.Models;

namespace Irish_Cinema_Listings
{
    public class HttpUtils
    {
        public static HttpWebRequest GetHttpRequest(String url)
        {
            System.Uri targetUri = new System.Uri(url);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(targetUri);

            return request;
        }

        public static String GetResponse(IAsyncResult callbackResult)
        {
            String results = "";
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.EndGetResponse(callbackResult);
                using (StreamReader httpwebStreamReader = new StreamReader(myResponse.GetResponseStream()))
                {
                    results = httpwebStreamReader.ReadToEnd();
                }
            }
            catch
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show("No internet connection!"));
            }

            return results;
        }
    }
}

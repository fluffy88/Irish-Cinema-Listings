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
using Irish_Cinema_Listings.Utils;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using Microsoft.Phone.Tasks;

namespace Irish_Cinema_Listings
{
    public partial class MoviePage : PhoneApplicationPage
    {
        private MediaPlayerLauncher mediaPlayerLauncher { get; set; }
        private int ItemsLoading = 0;

        public MoviePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            String url = HttpUtility.UrlDecode(NavigationContext.QueryString["url"]);
            this.ItemsLoading++;
            HttpWebRequest request = HttpUtils.GetHttpRequest(url);
            request.BeginGetResponse(new AsyncCallback(ReadMovieWebRequestCallback), request);
        }

        private void ReadMovieWebRequestCallback(IAsyncResult callbackResult)
        {
            String results = HttpUtils.GetResponse(callbackResult);
            var unsortedItems = JsonUtils.GetItems(results, new String[] { "name", "rating", "cert", "poster", "trailer", "review", "director", "cast", "info" });

            IEnumerator<Dictionary<string, string>> iterator = unsortedItems.GetEnumerator();
            iterator.MoveNext();
            var movie = iterator.Current;

            this.ItemsLoading++;
            var url = new Uri(JsonUtils.StripSlashes(movie["poster"]), UriKind.Absolute);

            Dispatcher.BeginInvoke(() =>
            {
                MovieTitle.Title = movie["name"];
                RatingText.Text = movie["rating"];
                CertText.Text = movie["cert"];
                BitmapImage image = new BitmapImage(url);
                image.DownloadProgress += new EventHandler<DownloadProgressEventArgs>(bitmapImage_DownloadProgress);
                PosterImage.Source = image;
                RuntimeText.Text = movie["info"].Substring(movie["info"].IndexOf(" "));
            });

            if (movie["review"] != "")
            {
                String reviewUrl = JsonUtils.StripSlashes(movie["review"]);
                this.ItemsLoading++;
                HttpWebRequest request = HttpUtils.GetHttpRequest(reviewUrl);
                request.BeginGetResponse(new AsyncCallback(ReadReviewWebRequestCallback), request);
            }
            else
            {
                Dispatcher.BeginInvoke(() => ReviewText.DataContext = new String[] { "No Review!" });
            }

            if (movie["trailer"] != "")
            {
                mediaPlayerLauncher = new MediaPlayerLauncher();
                Uri trailer = new Uri(JsonUtils.StripSlashes(movie["trailer"]));

                mediaPlayerLauncher.Media = trailer;
                mediaPlayerLauncher.Controls = MediaPlaybackControls.Pause | MediaPlaybackControls.Stop;
                mediaPlayerLauncher.Orientation = MediaPlayerOrientation.Landscape;
            }

            var movieTimes = new Collection<String>();
            var timesResult = JsonUtils.GetItems(results, new String[] { "day", "screenings" });
            foreach (Dictionary<String, String> times in timesResult)
            {
                if (times["day"] != "")
                {
                    movieTimes.Add(times["day"] + "\n" + times["screenings"]);
                }
                else
                {
                    int last = movieTimes.Count - 1;
                    var previousTime = movieTimes.ElementAt(last);
                    previousTime += " " + times["screenings"];
                    movieTimes.RemoveAt(last);
                    movieTimes.Insert(last, previousTime);
                }
            }
            Dispatcher.BeginInvoke(() => ShowingsList.ItemsSource = movieTimes);

            FinishedLoading();
        }

        void bitmapImage_DownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            FinishedLoading();
        }

        private void ReadReviewWebRequestCallback(IAsyncResult callbackResult)
        {
            String results = HttpUtils.GetResponse(callbackResult);
            var unsortedItems = JsonUtils.GetItems(results, new String[] { "name", "text" });

            foreach (Dictionary<String, String> movie in unsortedItems)
            {
                String review = JsonUtils.StripSlashes(HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(movie["text"])));
                String[] paragraphs = review.Split(new string[] { "\\r\\n" }, StringSplitOptions.RemoveEmptyEntries);
                Dispatcher.BeginInvoke(() =>
                {
                    ReviewText.DataContext = paragraphs;
                    FinishedLoading();
                });
                break;
            }
        }

        private void FinishedLoading()
        {
            this.ItemsLoading--;
            if (ItemsLoading <= 0)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => SystemTray.ProgressIndicator.IsIndeterminate = false);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayerLauncher.Show();
        }
    }
}
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

            Dispatcher.BeginInvoke(() =>
            {
                MovieTitle.Title = movie["name"];
                RatingText.Text = movie["rating"];
                CertText.Text = movie["cert"];
            });

            this.doImage(movie["poster"]);
            this.doTrailer(movie["trailer"]);
            this.doReview(movie["review"]);
            this.doTimes(results);

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
            if (mediaPlayerLauncher != null)
            {
                mediaPlayerLauncher.Show();
            }
        }

        private void doImage(String strUrl)
        {
            if (strUrl != "")
            {
                var url = new Uri(JsonUtils.StripSlashes(strUrl), UriKind.Absolute);
                Dispatcher.BeginInvoke(() =>
                {
                    this.ItemsLoading++;
                    BitmapImage image = new BitmapImage(url);
                    image.DownloadProgress += new EventHandler<DownloadProgressEventArgs>(bitmapImage_DownloadProgress);
                    PosterImage.Source = image;
                });
            }
        }

        private void doTrailer(String strUrl)
        {
            if (strUrl != "")
            {
                mediaPlayerLauncher = new MediaPlayerLauncher();
                Uri trailer = new Uri(JsonUtils.StripSlashes(strUrl));

                mediaPlayerLauncher.Media = trailer;
                mediaPlayerLauncher.Controls = MediaPlaybackControls.All;
                mediaPlayerLauncher.Orientation = MediaPlayerOrientation.Landscape;
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    TrailerButton.Content = "No Trailer!";
                    TrailerButton.IsEnabled = false;
                });
            }
        }

        private void doReview(String strUrl)
        {
            if (strUrl != "")
            {
                String reviewUrl = JsonUtils.StripSlashes(strUrl);
                this.ItemsLoading++;
                HttpWebRequest request = HttpUtils.GetHttpRequest(reviewUrl);
                request.BeginGetResponse(new AsyncCallback(ReadReviewWebRequestCallback), request);
            }
            else
            {
                Dispatcher.BeginInvoke(() => ReviewText.DataContext = new String[] { "No Review!" });
            }
        }

        private void doTimes(String results)
        {
            var movieTimes = new Collection<String>();
            var timesResult = JsonUtils.GetItems(results, new String[] { "day", "screenings" });
            foreach (Dictionary<String, String> times in timesResult)
            {
                if (times["day"] != "")
                {
                    movieTimes.Add(times["day"] + "\n" + times["screenings"]);
                }
                else if (times["screenings"] != "")
                {
                    int last = movieTimes.Count - 1;
                    var previousTime = movieTimes.ElementAt(last);
                    previousTime += " " + times["screenings"];
                    movieTimes.RemoveAt(last);
                    movieTimes.Insert(last, previousTime);
                }
                else
                {
                    movieTimes.Add("No Showings!");
                }
            }
            Dispatcher.BeginInvoke(() => ShowingsList.ItemsSource = movieTimes);
        }
    }
}
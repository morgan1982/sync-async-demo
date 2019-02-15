using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Net;

namespace sync_async
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ExecuteSync_Click(object sender, RoutedEventArgs e)
        {
            var watch = Stopwatch.StartNew();

            RunDownloadSync();
            watch.Stop();

            var elapsedMs = watch.ElapsedMilliseconds;
            results.Text += $"total execution time: {elapsedMs}";
        }
        private List<string> PrepData()
        {
            List<string> output = new List<string>();
            output.Add("https://www.yahoo.com");
            output.Add("https://www.google.com");
            output.Add("https://www.bing.com");
            output.Add("https://www.amazon.com");
            output.Add("https://www.ebay.com");
            output.Add("https://www.walmart.com");
            output.Add("https://www.stackoverflow.com");

            return output;

        }
        private async Task RunDownloadAsync()
        {
            List<string> websites = PrepData();
            results.Text = "";
            foreach (string site in websites)
            {
                WebSiteDataModel results = await Task.Run(() => DownloadWebsite(site));
                ReportWebsiteInfo(results);
            }
        }

        private void RunDownloadSync()
        {
            List<string> websites = PrepData();

            results.Text = "";

            foreach(string site in websites)
            {
                WebSiteDataModel results = DownloadWebsite(site);
                ReportWebsiteInfo(results);
            }
        }
        private WebSiteDataModel DownloadWebsite(string websiteUrl)
        {
            WebSiteDataModel output = new WebSiteDataModel();
            WebClient client = new WebClient();

            output.WebSiteUrl = websiteUrl;
            output.WebSiteData = client.DownloadString(websiteUrl);

            return output;
        }

        private async Task<WebSiteDataModel> DownloadWebsiteAsync(string websiteUrl)
        {
            WebSiteDataModel output = new WebSiteDataModel();
            WebClient client = new WebClient();

            output.WebSiteUrl = websiteUrl;
            output.WebSiteData = await client.DownloadStringTaskAsync(websiteUrl);

            return output;
        }

        private void ReportWebsiteInfo(WebSiteDataModel data)
        {
            results.Text += $"{ data.WebSiteUrl } downloaded: { data.WebSiteData.Length } chars long. { Environment.NewLine}";
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            results.Text = "";
        }

        private async void ExecuteAsync_Click(object sender, RoutedEventArgs e)
        {
            var watch = Stopwatch.StartNew();

            await RunDownloadAsync();
            watch.Stop();

            var elapsedMs = watch.ElapsedMilliseconds;
            results.Text += $"total execution time: {elapsedMs}";
        }

        private async void ExecuteParallelAsync_Click(object sender, RoutedEventArgs e)
        {
            var watch = Stopwatch.StartNew();

            await RunDownloadParallelAsync();
            watch.Stop();

            var elapsedMs = watch.ElapsedMilliseconds;
            results.Text += $"total execution time: {elapsedMs}";
        }

        private async Task RunDownloadParallelAsync()
        {
            List<string> websites = PrepData();
            List<Task<WebSiteDataModel>> tasks = new List<Task<WebSiteDataModel>>();

            results.Text = "";

            foreach (string site in websites)
            {
                tasks.Add(DownloadWebsiteAsync(site));
            }

            var resultsFromList = await Task.WhenAll(tasks);

            foreach(var item in resultsFromList)
            {
                ReportWebsiteInfo(item);
            }
        }
    }
}

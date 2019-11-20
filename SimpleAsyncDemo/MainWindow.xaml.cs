using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace SimpleAsyncDemo
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

    private void ExecuteSync_Click(object sender, RoutedEventArgs e)
    {
      var watch = System.Diagnostics.Stopwatch.StartNew();

      RunDownloadSync();

      watch.Stop();

      var elapsedMs = watch.ElapsedMilliseconds;

      resultsWindow.Text += $"Total execution time was: {elapsedMs}";
    }

    private async void ExecuteAsync_Click(object sender, RoutedEventArgs e)
    {
      var watch = System.Diagnostics.Stopwatch.StartNew();

      //await RunDownloadAsync();

      await RunDownloadParallelAsync();

      watch.Stop();

      var elapsedMs = watch.ElapsedMilliseconds;

      resultsWindow.Text += $"Total execution time was: {elapsedMs}{Environment.NewLine}";
    }

    private async Task RunDownloadAsync()
    {
      List<string> websites = PrepData();

      foreach (string site in websites)
      {
        WebsiteDataModel results = await Task.Run( () => DownloadWebsite(site));
        ReportWebsiteInfo(results);
      }
    }

    private async Task RunDownloadParallelAsync()
    {
      List<string> websites = PrepData();
      List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();

      foreach (string site in websites)
      {
        tasks.Add(DownloadWebsiteAsync(site)));
      }

      var results = await Task.WhenAll(tasks);

      foreach (var item in results)
      {
        ReportWebsiteInfo(item);
      }
    }

    private List<string> PrepData()
    {
      List<string> output = new List<string>();

      resultsWindow.Text = "";

      output.Add("https://www.yahoo.com");
      output.Add("https://www.google.com");
      output.Add("https://www.microsoft.com");
      output.Add("https://www.cnn.com");
      output.Add("https://www.codeproject.com");
      output.Add("https://www.stackoverflow.com");

      return output;
    }

    private void RunDownloadSync()
    {
      List<string> websites = PrepData();

      foreach (string site in websites)
      { 
        WebsiteDataModel results = DownloadWebsite(site);
        ReportWebsiteInfo(results);
      }
    }

    private WebsiteDataModel DownloadWebsite(string websiteUrl)
    {
      WebsiteDataModel output = new WebsiteDataModel();
      WebClient client = new WebClient();

      output.WebsiteUrl = websiteUrl;
      output.WebsiteData = client.DownloadString(websiteUrl);

      return output;
    }

    private async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteUrl)
    {
      WebsiteDataModel output = new WebsiteDataModel();
      WebClient client = new WebClient();

      output.WebsiteUrl = websiteUrl;
      output.WebsiteData = await client.DownloadStringTaskAsync(websiteUrl);

      return output;
    }

    private void ReportWebsiteInfo(WebsiteDataModel data)
    {
      resultsWindow.Text += $"{data.WebsiteUrl} downloaded: {data.WebsiteData.Length} characters long.{Environment.NewLine}";
    }
  }
}

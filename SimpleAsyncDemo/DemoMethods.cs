using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleAsyncDemo
{
  public static class DemoMethods
  {
    private static List<string> PrepData()
    {
      List<string> output = new List<string>();

      output.Add("https://www.yahoo.com");
      output.Add("https://www.google.com");
      output.Add("https://www.microsoft.com");
      output.Add("https://www.cnn.com");
      output.Add("https://www.amazon.com");
      output.Add("https://www.facebook.com");
      output.Add("https://www.twitter.com");
      output.Add("https://www.codeproject.com");
      output.Add("https://www.stackoverflow.com");
      output.Add("https://en.wikipedia.org/wiki/.NET_Framework");

      return output;
    }

    public static List<WebsiteDataModel> RunDownloadSync()
    {
      List<string> websites = PrepData();
      List<WebsiteDataModel> results = new List<WebsiteDataModel>();

      foreach (string site in websites)
      {
        results.Add(DownloadWebsite(site));
      }

      return results;
    }

    public static List<WebsiteDataModel> RunDownloadParallelSync()
    {
      List<string> websites = PrepData();
      List<WebsiteDataModel> results = new List<WebsiteDataModel>();

      Parallel.ForEach<string>(websites, (site) =>
      {
        results.Add(DownloadWebsite(site));
      });

      return results;
    }

    public static async Task<List<WebsiteDataModel>> RunDownloadParallelAsyncV2(IProgress<ProgressReportModel> progress, CancellationToken cancellationToken)
    {
      List<string> websites = PrepData();
      List<WebsiteDataModel> results = new List<WebsiteDataModel>();
      ProgressReportModel report = new ProgressReportModel();

      await Task.Run(() =>
      {
          Parallel.ForEach<string>(websites, (site) =>
          {
            results.Add(DownloadWebsite(site));
            cancellationToken.ThrowIfCancellationRequested();
            report.SitesDownloaded = results;
            report.PercentageComplete = (results.Count * 100) / websites.Count;

            progress.Report(report);
          });
      });

      return results;
    }

    public static async Task<List<WebsiteDataModel>> RunDownloadAsync(IProgress<ProgressReportModel> progress, CancellationToken cancellationToken )
    {
      List<string> websites = PrepData();
      List<WebsiteDataModel> results = new List<WebsiteDataModel>();
      ProgressReportModel report = new ProgressReportModel();

      foreach (string site in websites)
      {
        //results.Add(await Task.Run(() => DownloadWebsite(site)));
        results.Add(await DownloadWebsiteAsync(site));

        cancellationToken.ThrowIfCancellationRequested();

        report.SitesDownloaded = results;
        report.PercentageComplete = (results.Count * 100) / websites.Count;

        progress.Report(report);
      }
      return results;
    }

    public static async Task<List<WebsiteDataModel>> RunDownloadParallelAsync()
    {
      List<string> websites = PrepData();
      List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();

      foreach (string site in websites)
      {
        tasks.Add(DownloadWebsiteAsync(site));
      }

      var results = await Task.WhenAll(tasks);

      return results.ToList();
    }

    public static async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteUrl)
    {
      WebsiteDataModel output = new WebsiteDataModel();
      WebClient client = new WebClient();

      output.WebsiteUrl = websiteUrl;
      output.WebsiteData = await client.DownloadStringTaskAsync(websiteUrl);

      return output;
    }

    private static WebsiteDataModel DownloadWebsite(string websiteUrl)
    {
      WebsiteDataModel output = new WebsiteDataModel();
      WebClient client = new WebClient();

      output.WebsiteUrl = websiteUrl;
      output.WebsiteData = client.DownloadString(websiteUrl);

      return output;
    }

  }
}

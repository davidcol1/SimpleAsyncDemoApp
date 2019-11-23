using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

    CancellationTokenSource cts = new CancellationTokenSource();

    public MainWindow()
    {
      InitializeComponent();
      resultsWindow.Text = "";
    }

    private void ExecuteSync_Click(object sender, RoutedEventArgs e)
    {
      resultsWindow.Text = "";
      var watch = System.Diagnostics.Stopwatch.StartNew();

      var results = DemoMethods.RunDownloadParallelSync();
      PrintResults(results);

      watch.Stop();

      var elapsedMs = watch.ElapsedMilliseconds;

      resultsWindow.Text += $"Total execution time was: {elapsedMs}";
    }

    private async void ExecuteAsync_Click(object sender, RoutedEventArgs e)
    {
      resultsWindow.Text = "";
      cts.Dispose(); // Clean up old token source....
      cts = new CancellationTokenSource(); // "Reset" the cancellation token source..
      Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
      progress.ProgressChanged += ReportProgress;

      var watch = System.Diagnostics.Stopwatch.StartNew();

      try
      {
        var results = await DemoMethods.RunDownloadAsync(progress, cts.Token);
        PrintResults(results);
      }
      catch (OperationCanceledException)
      {
        resultsWindow.Text += $"The async download was cancelled.{Environment.NewLine}";
      }

      watch.Stop();

      var elapsedMs = watch.ElapsedMilliseconds;

      resultsWindow.Text += $"Total execution time was: {elapsedMs}{Environment.NewLine}";
    }

    private void ReportProgress(object sender, ProgressReportModel e)
    {
      dashboardProgress.Value = e.PercentageComplete;
      PrintResults(e.SitesDownloaded);
    }

    private async void ExecuteParallelAsync_Click(object sender, RoutedEventArgs e)
    {
      resultsWindow.Text = "";
      cts.Dispose(); // Clean up old token source....
      cts = new CancellationTokenSource(); // "Reset" the cancellation token source..
      Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
      progress.ProgressChanged += ReportProgress;

      var watch = System.Diagnostics.Stopwatch.StartNew();

      var results = await DemoMethods.RunDownloadParallelAsyncV2(progress, cts.Token);
      PrintResults(results);
      resultsWindow.Text += $"The async download was cancelled.{Environment.NewLine}";
      watch.Stop();

      var elapsedMs = watch.ElapsedMilliseconds;

      resultsWindow.Text += $"Total execution time was: {elapsedMs}{Environment.NewLine}";
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
      cts.Cancel();
    }

    private void PrintResults(List<WebsiteDataModel> results)
    {
      resultsWindow.Text = "";
      foreach (var item in results)
      {
        resultsWindow.Text += $"{item.WebsiteUrl} downloaded: {item.WebsiteData.Length} characters long.{Environment.NewLine}";
      }
      
    }
  }
}

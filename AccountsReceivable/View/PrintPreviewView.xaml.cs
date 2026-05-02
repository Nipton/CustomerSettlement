using AccountsReceivable.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AccountsReceivable.View
{
    public partial class PrintPreviewView : Window
    {
        private string reportTitle = "Отчёт";
        public PrintPreviewView()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadHtml();
        }
        private async Task LoadHtml()
        {
            if(DataContext is PrintPreviewViewModel vm)
            {
                await Browser.EnsureCoreWebView2Async();
                Browser.NavigateToString(vm.HtmlContent);
                reportTitle = vm.HtmlTitle;
                Browser.ZoomFactor = 0.8;
                UpdateZoomDisplay();
                Browser.ZoomFactorChanged += Browser_ZoomFactorChanged;
            }
        }
        private async void Print_Click(object sender, RoutedEventArgs e)
        {
            ButtonsGrid.IsEnabled = false;
            await Browser.CoreWebView2.ExecuteScriptAsync("window.print();");
            ButtonsGrid.IsEnabled = true;
        }
        private async void SavePdf_Click(object sender, RoutedEventArgs e)
        {
            string fileName = $"{reportTitle} {DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            await Browser.CoreWebView2.PrintToPdfAsync(path);
            MessageBox.Show($"Файл сохранён на рабочий стол:\n{fileName}", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);  
        }
        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            Browser.ZoomFactor += 0.1;
            UpdateZoomDisplay();
        }
        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            Browser.ZoomFactor -= 0.1;
            UpdateZoomDisplay();
        }
        private void Browser_ZoomFactorChanged(object? sender, EventArgs e)
        {
            UpdateZoomDisplay();
        }
        private void ResetZoomTextBlock(object sender, MouseButtonEventArgs e)
        {
            Browser.ZoomFactor = 1.0;
            UpdateZoomDisplay();
        }
        private void UpdateZoomDisplay()
        {
            int percent = (int)(Browser.ZoomFactor * 100);
            zoomTextBlock.Text = $"{percent}%";
        }
    }
}


using CustomerSettlement.Helpers;
using CustomerSettlement.Interfaces;
using CustomerSettlement.ViewModels.Commands;
using System.Windows.Input;

namespace CustomerSettlement.ViewModels
{
    public class PrintPreviewViewModel : ViewModelBase
    {
        private readonly IDialogService dialogService;
        private readonly string htmlContent;
        public string HtmlContent => htmlContent;
        public string HtmlTitle { get; }
        public ICommand CloseCommand { get; }
        public PrintPreviewViewModel(string htmlContent, IDialogService dialogService)
        {
            this.htmlContent = htmlContent;
            this.dialogService = dialogService;
            CloseCommand = new RelayCommand(_ => CloseWindow());
            HtmlTitle = StringFormattingHelper.GetTitleFromHtml(htmlContent);
        }
        private void CloseWindow()
        {
            dialogService.CloseWindow(this, false);
        }

    }
}

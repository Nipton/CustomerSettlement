using System.Windows.Controls;
using System.Windows.Input;

namespace CustomerSettlement.View
{
    public partial class NomenclatureView : UserControl
    {
        public NomenclatureView(){ InitializeComponent(); }
        private void OnBackgroundClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            NomenclatureListBox.UnselectAll();
            NomenclatureListBox.Focus();
        }
    }
}

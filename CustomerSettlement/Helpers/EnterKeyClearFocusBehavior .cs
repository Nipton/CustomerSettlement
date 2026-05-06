using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CustomerSettlement.Helpers
{
    public class EnterKeyClearFocusBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
            base.OnDetaching();
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // костыль, чтобы прокнуть LostFocus по Enter
            if (e.Key == Key.Enter)
            {
                var textBox = AssociatedObject;
                var parent = VisualTreeHelper.GetParent(textBox) as UIElement;
                if (parent != null)
                {
                    parent.Focus();
                    Keyboard.ClearFocus();
                }
                e.Handled = true;
            }
        }
    }
}

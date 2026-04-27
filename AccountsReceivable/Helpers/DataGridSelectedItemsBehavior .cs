using Microsoft.Xaml.Behaviors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AccountsReceivable.Helpers
{
    public class DataGridSelectedItemsBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty SelectedItemsProperty =
        DependencyProperty.Register("SelectedItems", typeof(IList), typeof(DataGridSelectedItemsBehavior), new PropertyMetadata(new List<object>()));
        public IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value ?? new List<object>());
        }
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += OnSelectionChanged;
            SelectedItems = AssociatedObject.SelectedItems.Cast<object>().ToList();
        }
        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged -= OnSelectionChanged;
            base.OnDetaching();
        }
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItems = AssociatedObject.SelectedItems.Cast<object>().ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfUI.ViewModel
{
    public static class Helpers
    {


        public static bool GetInteractiveTextChangeBinding(DependencyObject obj)
        {
            return (bool)obj.GetValue(InteractiveTextChangeBindingProperty);
        }

        public static void SetInteractiveTextChangeBinding(DependencyObject obj, bool value)
        {
            obj.SetValue(InteractiveTextChangeBindingProperty, value);
        }

        // Using a DependencyProperty as the backing store for InteractiveTextChangeBinding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InteractiveTextChangeBindingProperty =
            DependencyProperty.RegisterAttached("InteractiveTextChangeBinding", typeof(bool), typeof(Helpers), new PropertyMetadata(false, OnInteractiveTextChangeBindingPropertyChanged));

        private static void OnInteractiveTextChangeBindingPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs ea)
        {
            if (ea.OldValue is bool oldValue && ea.NewValue is bool newValue && obj is TextBox textBox)
            {
                if (newValue && !oldValue)
                {
                    textBox.TextChanged += TextBox_TextChanged;
                }
                else if (oldValue && !newValue)
                {
                    textBox.TextChanged -= TextBox_TextChanged;
                }

            }
        }

        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var textPropertyBinding = textBox.GetBindingExpression(TextBox.TextProperty);

            if (textPropertyBinding != null)
                textPropertyBinding.UpdateSource();
        }
    }
}

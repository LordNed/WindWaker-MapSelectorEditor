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
using System.Windows.Shapes;

namespace WPF_Editor
{
    /// <summary>
    /// Interaction logic for GenericModalWindow.xaml
    /// </summary>
    public partial class GenericModalWindow : Window
    {
        public GenericModalWindow()
        {
            InitializeComponent();
        }

        public GenericModalWindow(string windowTitle, string bodyText, string cancelText, string confirmText)
        {
            InitializeComponent();

            this.Title = windowTitle;
            m_text.Text = bodyText;
            m_cancelButton.Content = cancelText;
            m_confirmButton.Content = confirmText;
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void OnConfirmButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

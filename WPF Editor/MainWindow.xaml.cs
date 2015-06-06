using CLIReadout;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DebugMenuModifier DebugMenu
        {
            get { return m_debugMenu; }
            set { m_debugMenu = value; this.DataContext = value; }
        }

        private DebugMenuModifier m_debugMenu;


        public MainWindow()
        {
            InitializeComponent();

            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            //string menuFile = "C:\\Users\\Matt\\Downloads\\Menu1.dat";

            //DebugMenuModifier reader = new DebugMenuModifier();
            //reader.Load(menuFile);

            //this.DataContext = reader;
        }

        private void OnAboutButtonClicked(object sender, RoutedEventArgs e)
        {

        }

        private void OnOpenFileClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Menu Files (*.dat)|*.dat|All files(*.*)|*.*";

            if(ofd.ShowDialog() == true)
            {
                var debugMenu = new DebugMenuModifier();
                debugMenu.Load(ofd.FileName);
                DebugMenu = debugMenu;
            }
        }

        private void OnCloseFileClicked(object sender, RoutedEventArgs e)
        {
            DebugMenu = null;
        }

        private void OnSaveClicked(object sender, RoutedEventArgs e)
        {

        }

        private void OnSaveAsClicked(object sender, RoutedEventArgs e)
        {

        }

        private void OnExitClicked(object sender, RoutedEventArgs e)
        {

        }
    }
}

using CLIReadout;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        private string m_filePath;


        public MainWindow()
        {
            InitializeComponent();

            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            m_filePath = "C:\\Users\\Matt\\Downloads\\Menu1.dat";

            var debugMenu = new DebugMenuModifier();
            debugMenu.Load(m_filePath);
            DebugMenu = debugMenu;
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
                m_filePath = ofd.FileName;
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
            DebugMenu.Save(m_filePath);
        }

        private void OnSaveAsClicked(object sender, RoutedEventArgs e)
        {
            if (m_debugMenu == null || string.IsNullOrEmpty(m_filePath))
                throw new ArgumentNullException("No currently loaded debug menu to save!");

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Menu Files (*.dat)|*.dat|All files(*.*)|*.*";
            sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;
            sfd.InitialDirectory = System.IO.Path.GetDirectoryName(m_filePath);
            sfd.OverwritePrompt = true;

            if(sfd.ShowDialog() == true)
            {
                m_debugMenu.Save(sfd.FileName);
                m_filePath = sfd.FileName;
            }
        }

        private void OnExitClicked(object sender, RoutedEventArgs e)
        {
            if (DebugMenu != null && !string.IsNullOrEmpty(m_filePath))
            {
                GenericModalWindow dialog = new GenericModalWindow("Save before Exit?", "Do you want to save before exiting?", "No", "Yes");
                if (dialog.ShowDialog() == true)
                {
                    DebugMenu.Save(m_filePath);
                }
            }

            Application.Current.Shutdown();
        }
    }
}

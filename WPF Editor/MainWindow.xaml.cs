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

        private void OnAddEntryClicked(object sender, RoutedEventArgs e)
        {
            var newEntry = new DebugMenuModifier.EntryHeader();
            m_debugMenu.Entries.Add(newEntry);
            m_mapEntryComboBox.SelectedItem = newEntry;
        }

        private void OnDeleteEntryClicked(object sender, RoutedEventArgs e)
        {
            m_debugMenu.Entries.Remove((DebugMenuModifier.EntryHeader)m_mapEntryComboBox.SelectedItem);
            int prevIndex = m_mapEntryComboBox.SelectedIndex - 1;
            if (prevIndex < 0)
                prevIndex = 0;
            m_mapEntryComboBox.SelectedIndex = prevIndex;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Apply a SizeToContent size of Manual after we load. This way when we load the horizontal
            // scroll stuff it doesn't balloon the program outwards.
            this.SizeToContent = System.Windows.SizeToContent.Manual;
        }

        private void OnAddSubEntryClicked(object sender, RoutedEventArgs e)
        {
            // Get the currently selected header entry
            var header = (DebugMenuModifier.EntryHeader)m_mapEntryComboBox.SelectedItem;
            var newEntry = new DebugMenuModifier.EntrySubOption();
            header.SubOptions.Add(newEntry);
            m_subOptionsList.SelectedItem = newEntry;
            m_subOptionsList.ScrollIntoView(newEntry);
        }

        private void OnDeleteSubEntry(object sender, RoutedEventArgs e)
        {
            var header = (DebugMenuModifier.EntryHeader)m_mapEntryComboBox.SelectedItem;

            header.SubOptions.Remove((DebugMenuModifier.EntrySubOption)m_subOptionsList.SelectedItem);
            int prevIndex = m_subOptionsList.SelectedIndex - 1;
            if (prevIndex < 0)
                prevIndex = 0;
            m_subOptionsList.SelectedIndex = prevIndex;
        }
    }
}

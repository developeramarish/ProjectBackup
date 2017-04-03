using ProjectBackup.Backend_Sources;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjectBackup.Backend_Sources.Classes;
using ProjectBackup.Backend_Sources.Threads;
using System.Runtime.InteropServices;

namespace ProjectBackup
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(MainWindow));

        // Define the mainProcess here to be able to access it
        private readonly MainProcess _mainProcess;

        private List<FileWatcher> _fileWatchers;

        /// <summary>
        /// Default method that will manage everything that happen in the program
        /// </summary>
        public MainWindow()
        {
            AllocConsole();                                                 // We open a console for logs
            //log4net.Config.XmlConfigurator.Configure();                     // Configurate the logger
            InitializeComponent();                                          // Initialize the components

            _logger.Info("Application is starting");

            _mainProcess = new MainProcess();                               // Define a new mainProcess
            _mainProcess.backupList = new List<Backup>();                   // Define a new backup list
            _fileWatchers = new List<FileWatcher>();                        // Define a new FileWatcher list

            // WE LOAD ALL BACKUP FROM CONFIG FILE
            Backup bTest = new Backup("Test", "C:\\Users\\vinid223\\Desktop\\source\\", "C:\\Users\\vinid223\\Desktop\\destination\\");    
            _mainProcess.backupList.Add(bTest);

            foreach (var backup in _mainProcess.backupList)
            {
                _fileWatchers.Add(new FileWatcher(backup));
            }

            dataGridBackupList.ItemsSource = _mainProcess.backupList;       // We load the backup list in the window item
        }

        /// <summary>
        /// This methos is called when clicking on the X button on the left side of a backup entry
        /// </summary>
        private void btnDeleteBackup_Click(object sender, RoutedEventArgs e)
        {
            // We extract the Backup element from the button
            var button = sender as Button;
            if (button != null)
            {
                Backup backup = (Backup) (button.CommandParameter);

                // We show a confirmation box to delete or not the Backup
                var result = MessageBox.Show("Voulez-vous supprimer cette sauvegarde : " + backup.Name, "Avertissement",
                    MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.No);

                // If the answer is yes
                if (result == MessageBoxResult.Yes)
                {
                    // We remove the backup from the list
                    _mainProcess.backupList.Remove(backup);

                    // We update the content in the window
                    dataGridBackupList.Items.Refresh();
                }
            }
        }

        /// <summary>
        /// Method that add backup to the list
        /// </summary>
        private void btnAddBackup_Click(object sender, RoutedEventArgs e)
        {
            // We initiate a new modal of the backup elements
            BackupModal view = new BackupModal();

            // We show the Modal
            view.ShowDialog();

            if (view.backup != null)
            {
                _mainProcess.backupList.Add(view.backup);
                dataGridBackupList.Items.Refresh();
            }

        }
    }
}

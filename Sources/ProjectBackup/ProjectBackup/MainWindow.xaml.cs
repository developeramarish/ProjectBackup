using ProjectBackup.Backend_Sources;
using System;
using System.Collections.Generic;
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
using ProjectBackup.Backend_Sources.Classes;
using ProjectBackup.Backend_Sources.Threads;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

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

        private string BackupExportFile = "backups.xml";

        /// <summary>
        /// Default method that will manage everything that happen in the program
        /// </summary>
        public MainWindow()
        {
            AllocConsole();                                                 // We open a console for logs
            InitializeComponent();                                          // Initialize the components

            _logger.Info("Application is starting");

            _mainProcess = new MainProcess();                               // Define a new mainProcess
            _mainProcess.backupList = new List<Backup>();                   // Define a new backup list
            _fileWatchers = new List<FileWatcher>();                        // Define a new FileWatcher list
            
            UnserializeCollection(BackupExportFile);                        // We load the backup configuration file

            // We start all the watchers of all the backup
            foreach (var backup in _mainProcess.backupList)
            {
                _fileWatchers.Add(new FileWatcher(backup));
            }

            dataGridBackupList.ItemsSource = _mainProcess.backupList;       // We load the backup list in the window item

            _logger.Info("Application started successfully");
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
                    _logger.Info("Backup delete trigger. Name : " + backup.Name);

                    // We remove the backup from the list
                    _mainProcess.backupList.Remove(backup);

                    // We update the content in the window
                    dataGridBackupList.Items.Refresh();

                    // We serialize the new set of backup collection
                    SerializeCollection(BackupExportFile);
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
                _logger.Info("New backup trigger. Name : " + view.backup.Name);

                // We add the backup in the backup list
                _mainProcess.backupList.Add(view.backup);

                // We refresh the datagrid list
                dataGridBackupList.Items.Refresh();

                // We save the content in the save file
                SerializeCollection(BackupExportFile);
            }
        }

        /// <summary>
        /// Method that will generage and save the configuration file for all the backups
        /// </summary>
        /// <param name="filename">File name of the backup</param>
        private void SerializeCollection(string filename)
        {
            // Create the collection of backups
            Backups backups = new Backups();

            // Name te collection
            backups.CollectionName = "Backups";

            // Add the value to the collection from the existing backup list
            foreach (var backup in _mainProcess.backupList)
            {
                backups.Add(backup);
            }

            // Initiate a Serializer
            XmlSerializer x = new XmlSerializer(typeof(Backups));

            // Initiate a stream writer
            TextWriter writer = new StreamWriter(filename);

            // Serialise the collection and print it into the file
            x.Serialize(writer, backups);
        }

        /// <summary>
        /// Method that will read and generate the backup objects from a save file
        /// </summary>
        /// <param name="filename">File name of the configuration</param>
        private void UnserializeCollection(string filename)
        {
            // Initiate a serializer
            XmlSerializer serializer = new XmlSerializer(typeof(Backups));

            // Initiate a filestream
            FileStream fs = new FileStream(filename, FileMode.Open);

            // Deserialize the file into the backup collection
            Backups backups = (Backups) serializer.Deserialize(fs);

            // Add the backup to the working backup list
            foreach (var backup in backups)
            {
                _mainProcess.backupList.Add((Backup) backup);
            }
        }
    }
}

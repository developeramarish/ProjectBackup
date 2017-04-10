using ProjectBackup.Backend_Sources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ProjectBackup.Backend_Sources.Classes;
using ProjectBackup.Backend_Sources.Threads;
using System.Runtime.InteropServices;
using System.Threading;
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

        // Name of the configuration folder
        private string BackupExportFile = "backups.xml";

        private Backup _selectedBackup;
        public Backup SelectedBackup
        {
            get { return _selectedBackup; }
            set { _selectedBackup = value; }
        }

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
            
            UnserializeCollection(BackupExportFile);                        // We load the backup configuration file

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

            // Check if the backup is not null
            if (button != null)
            {
                // Cast the backup to be able to use it
                Backup backup = (Backup) (button.CommandParameter);

                // We show a confirmation box to delete or not the Backup
                var result = MessageBox.Show("Voulez-vous supprimer cette sauvegarde : " + backup.Name, "Avertissement",
                    MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.No);

                // If the answer is yes
                if (result == MessageBoxResult.Yes)
                {
                    _logger.Info("Backup delete trigger. Name : " + backup.Name);

                    // We dispose the watcher before deleting the backup
                    backup.FileWatcher.Watcher.Dispose();

                    // Set the watcher running to false
                    backup.FileWatcher.watcherRunning = false;

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

                // Create a new filewatcher
                view.backup.FileWatcher = new FileWatcher(view.backup.Source, view.backup.Destination);

                // Init the backup to make a first copy of a folder
                // Start it in a thread to limit the blocking on the main program
                Thread t = new Thread(() => view.backup.FileWatcher.InitNew());
                t.Start();

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
            _logger.Info("Save backup in the configuration file : " + filename);

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

            // Close the file
            writer.Close();
        }

        /// <summary>
        /// Method that will read and generate the backup objects from a save file
        /// </summary>
        /// <param name="filename">File name of the configuration</param>
        private void UnserializeCollection(string filename)
        {
            _logger.Info("Load backup data from the save file : " + filename);

            try
            {
                // Initiate a serializer
                XmlSerializer serializer = new XmlSerializer(typeof(Backups));

                // Initiate a filestream
                FileStream fs = new FileStream(filename, FileMode.Open);

                // Deserialize the file into the backup collection
                Backups backups = (Backups)serializer.Deserialize(fs);

                // Add the backup to the working backup list
                foreach (var backup in backups)
                {
                    // We cast the object as a backup
                    Backup b = (Backup)backup;

                    // Initiate a new FileWatcher with the source path and destination path
                    b.FileWatcher = new FileWatcher(b.Source, b.Destination);
                    
                    // Init start to update the backup before the watcher start
                    // start on a new thread to let an other backup make de verification
                    Thread t = new Thread(() => b.FileWatcher.InitStart());
                    t.Start();

                    // Add the backup in the backup list of the window app
                    _mainProcess.backupList.Add(b);
                }

                // Close the open file
                fs.Close();
            }
            catch (Exception)
            {
                _logger.Warn("The configuration file could not be loaded. Might be missing or invalid");
            }
        }

        /// <summary>
        /// This method is triggered when a row in the backup list is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridBackupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the button value 
            var dataGrid = sender as DataGrid;

            // Check if the backup is not null
            if (dataGrid != null)
            {
                // Cast the backup to be able to use it
                Backup backup = (Backup)(dataGrid.SelectedItem);

                // Update the buttons of the main window
                UpdateWatcherButtons(backup);
            }
        }

        /// <summary>
        /// This method is triggered when the play button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlayBackup_Click(object sender, RoutedEventArgs e)
        {
            // If the watcher is not running
            if (SelectedBackup != null && !SelectedBackup.FileWatcher.watcherRunning)
            {
                // Start the watcher with the verification of change made while it was paused
                Thread t = new Thread(() => SelectedBackup.FileWatcher.InitStart());
                t.Start();

                // Update the buttons in the main window
                UpdateWatcherButtons(SelectedBackup);
            }
        }

        /// <summary>
        /// This method is triggered when the pause button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPauseBackup_Click(object sender, RoutedEventArgs e)
        {
            // If the watcher is running
            if (SelectedBackup != null && SelectedBackup.FileWatcher.watcherRunning)
            {
                // Stop it and save it to the flag
                SelectedBackup.FileWatcher.watcherRunning = false;
                SelectedBackup.FileWatcher.Watcher.Dispose();

                // Update the buttons in the main window
                UpdateWatcherButtons(SelectedBackup);
            }
        }

        /// <summary>
        /// This method update the play and pause button in the main windows
        /// </summary>
        /// <param name="backup">Selected Backup</param>
        private void UpdateWatcherButtons(Backup backup)
        {
            // In case the backup selected is not deleted
            if (backup != null)
            {
                // Set the current backup
                SelectedBackup = backup;

                // Set the enabled value of the buttons
                btnPauseBackup.IsEnabled = backup.FileWatcher.watcherRunning;
                btnPlayBackup.IsEnabled = !backup.FileWatcher.watcherRunning;
            }
            else
            {
                // Set the current backup
                SelectedBackup = null;

                // If it's deleted just disable the buttons
                btnPauseBackup.IsEnabled = false;
                btnPlayBackup.IsEnabled = false;
            }
        }
    }
}

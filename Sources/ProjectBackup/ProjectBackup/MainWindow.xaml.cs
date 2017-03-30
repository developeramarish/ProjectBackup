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

namespace ProjectBackup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Define the mainProcess here to be able to access it
        private readonly MainProcess _mainProcess;

        /// <summary>
        /// Default method that will manage everything that happen in the program
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            
            _mainProcess = new MainProcess();                               // Define a new mainProcess
            _mainProcess.backupList = new List<Backup>();                   // Define a new backup list


            Backup bTest = new Backup("Test", "C:\\Users\\vinid223\\Desktop\\source\\", "C:\\Users\\vinid223\\Desktop\\destination\\");    
            _mainProcess.backupList.Add(bTest);

            dataGridBackupList.ItemsSource = _mainProcess.backupList;       // We load the backup list in the window item
            FileWatcher f = new FileWatcher(bTest);
            f.Run();
        }

        /// <summary>
        /// This methos is called when clicking on the X button on the left side of a backup entry
        /// </summary>
        private void btnDeleteBackup_Click(object sender, RoutedEventArgs e)
        {
            // Initialise the result value of the confirmation box
            MessageBoxResult result = MessageBoxResult.None;

            // We extract the Backup element from the button
            Backup backup = (Backup) ((sender as Button).CommandParameter);

            // We show a confirmation box to delete or not the Backup
            result = MessageBox.Show("Voulez-vous supprimer cette sauvegarde : " + backup.Name, "Avertissement",
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ProjectBackup.Backend_Sources.Classes;
using MessageBox = System.Windows.MessageBox;

namespace ProjectBackup
{
    /// <summary>
    /// Interaction logic for BackupModal.xaml
    /// </summary>
    public partial class BackupModal : Window
    {
        public Backup backup;

        public BackupModal()
        {
            InitializeComponent();
            
        }

        /// <summary>
        /// When the user click on cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// This method is trigger when a user click on the select source folder button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetDirectoryModalSource_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Veuillez choisir le dossier source";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtBoxSource.Text = dialog.SelectedPath + "\\";
                }
            }
        }

        /// <summary>
        /// This method is triggered when a user click on the select destination folder button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetDirectoryModalDestination_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Veuillez choisir le dossier destination";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtBoxDestination.Text = dialog.SelectedPath + "\\";
                }
            }
        }

        /// <summary>
        /// This method is triggered when a user click on the add button 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            // Export the data into a backup object
            backup = new Backup(txtBoxName.Text, txtBoxSource.Text, txtBoxDestination.Text);

            // Validate the information in defined in the backup
            List<string> listErrors = backup.ValidateParameters();

            // If there is some errors in the data given
            if (listErrors.Count > 0)
            {
                // Show the errors on the screen
                string show = "Il y a des erreurs dans les informations suivantes : \n";

                foreach (var str in listErrors)
                {
                    show += str + "\n";
                }

                MessageBox.Show(show, "Erreur", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                backup = null;
            }
            else
            {
                // Otherwise close the modal
                Close();
            }
        }
    }
}
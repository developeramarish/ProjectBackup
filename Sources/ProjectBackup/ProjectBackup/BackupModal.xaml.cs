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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

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

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            backup = new Backup(txtBoxName.Text, txtBoxSource.Text, txtBoxDestination.Text);
            List<string> listErrors = backup.validateParameters();

            if (listErrors.Count > 0)
            {
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
                Close();
            }
        }
    }
}
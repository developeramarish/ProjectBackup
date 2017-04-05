using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProjectBackup.Backend_Sources.Threads;

namespace ProjectBackup.Backend_Sources.Classes
{
    /// <summary>
    /// Class Backup that have all the information about a backup data
    /// </summary>
    public class Backup
    {
        public string Name { get; set; }                    // Name of the backup
        public DateTime CreationDate { get; set; }          // Creation date of the backup
        public string Source { get; set; }                  // Source path of the backup
        public string Destination { get; set; }             // Destination path of the backup

        [System.Xml.Serialization.XmlIgnore]
        public FileWatcher FileWatcher;                     // FileWatcher of the backup. Not saved in save file

        /// <summary>
        /// Constructor with no parameters
        /// </summary>
        public Backup()
        {
            // Used only when retrieving backup config in the save file
        }

        /// <summary>
        /// Constructor that assume that the data is valid 
        /// </summary>
        /// <param name="name">Name of the backup</param>
        /// <param name="source">Path of source folder</param>
        /// <param name="destination">Path of destination folder</param>
        /// <param name="creationDate">Date of creation, default date NOW</param>
        public Backup(string name, string source, string destination, DateTime creationDate = new DateTime())
        {
            this.Name = name;
            this.CreationDate = creationDate;
            this.Source = source;
            this.Destination = destination;
        }

        /// <summary>
        /// Method that validate all Backup parameters
        /// </summary>
        /// <returns>List of error messages</returns>
        public List<string> ValidateParameters()
        {
            // List of all invalid characters
            char[] invalidChar = Path.GetInvalidPathChars();

            // List of error messages
            List<string> listError = new List<string>();

            // Validate if the source path is valid
            if (string.IsNullOrEmpty(Source) || Source.Any(charE => invalidChar.Any(charI => charE == charI)))
            {
                listError.Add("Le path du répertoire source n'est pas valide");
            }
            else
            {
                // Validate if the source directory exist
                FileAttributes attr = File.GetAttributes(@Source);
                if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
                {
                    listError.Add("Le path du répertoire source n'est pas un répertoire");
                }
            }

            // Validate if the destination path is valid
            if (string.IsNullOrEmpty(Destination) || Destination.Any(charE => invalidChar.Any(charI => charE == charI)))
            {
                listError.Add("Le path du répertoire destination n'est pas valide");
            }
            else
            {
                // Validate if the destination path exist
                FileAttributes attr = File.GetAttributes(@Destination);
                if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
                {
                    listError.Add("Le path du répertoire destination n'est pas un répertoire");
                }
            }


            // Validate the name of the backup
            if (string.IsNullOrEmpty(Name))
            {
                listError.Add("Le nom de la sauvegarde ne doit pas être vide");
            }

            return listError;
        }
    }
}

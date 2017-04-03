using ProjectBackup.Backend_Sources.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectBackup.Backend_Sources.Threads
{
    public class FileDiffEvaluator
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(FileDiffEvaluator));
        private const int NumberOfTry = 10;

        /// <summary>
        /// Waits until a file can be opened with read permission to be able to backup
        /// 
        /// This method check if the file is readable to be able to copy it
        /// 
        /// Origin of the code:
        /// http://stackoverflow.com/a/699587/3900435
        /// </summary>
        public static void WaitReady(string fileName)
        {
            int counter = 0;
            while (counter < 100)
            {
                Logger.Info("File check : " + fileName);
                try
                {
                    using (Stream stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        Logger.Info(string.Format("Output file {0} ready.", fileName));
                        break;
                    }
                }
                catch (FileNotFoundException)
                {
                    break;
                }
                catch (IOException ex)
                {
                    Logger.Info(string.Format("Output file {0} not yet ready ({1})", fileName, ex.Message));
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logger.Info(string.Format("Output file {0} not yet ready ({1})", fileName, ex.Message));
                }
                Thread.Sleep(100);
                counter++;
            }
        }

        /// <summary>
        /// Method that is called when a watcher trigger a new gile or a changed file. This method will copy the new file at the destination
        /// </summary>
        /// <param name="e">FileSystemEventArgs to get name and path of the file</param>
        /// <param name="source">Source folder of the file</param>
        /// <param name="destination">Destination folder of the file</param>
        public static void NewOrChangedFile(FileSystemEventArgs e, string source, string destination)
        {
            Logger.Info("Nouveau/Edit fichier : " + e.Name);

            // Check if the file can be read
            WaitReady(Path.Combine(source, e.Name));
            try
            {
                // Copy the file at the destination
                File.Copy(Path.Combine(source, e.Name), Path.Combine(destination, e.Name), true);
            }
            catch (Exception excep)
            {
                Logger.Info("Exception Nouveau/Edit fichier : " + excep.Message);
            }
        }

        /// <summary>
        /// Method that is called when a watcher trigger a deleted file. This method will delete the file at the destination folder
        /// </summary>
        /// <param name="e">FileSystemEventArgs to get name and path</param>
        /// <param name="source">Source folder of the file</param>
        /// <param name="destination">Destination folder of the file</param>
        public static void DeletedFile(FileSystemEventArgs e, string source, string destination)
        {
            Logger.Info("Fichier supprime : " + e.Name);

            // There is no need to verify if the file is in use because the destination folder is only used by this program
            try
            {
                // We delete the file at the destination
                File.Delete(Path.Combine(destination, e.Name));
            }
            catch (Exception excep)
            {
                Logger.Info("Exception suppression fichier : " + excep.Message);
            }
        }

        /// <summary>
        /// Method that is called when a watcher trigger a renamed file. This method will delete the old file and copie the new one
        /// </summary>
        /// <param name="e">FileSystemEventArgs to get name and path</param>
        /// <param name="source">Source folder of the file</param>
        /// <param name="destination">Destination folder of the file</param>
        public static void RenamedFile(RenamedEventArgs e, string source, string destination)
        {
            Logger.Info("Fichier renomme : " + e.Name);

            // Verify if both file are usable
            WaitReady(Path.Combine(source, e.Name));
            WaitReady(Path.Combine(destination, e.OldName));
            try
            {
                File.Copy(Path.Combine(source, e.Name), Path.Combine(destination, e.Name), true);
                File.Delete(Path.Combine(destination, e.OldName));
            }
            catch (Exception excep)
            {
                Logger.Info("Exception renommage du fichier : " + excep.Message);
            }            
        }
    }
}

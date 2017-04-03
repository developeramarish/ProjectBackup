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
        /// Waits until a file can be opened with write permission
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
                    using (Stream stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
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

        public static void NewOrChangedFile(FileSystemEventArgs e, Backup b)
        {
            Logger.Info("Nouveau/Edit fichier : " + e.Name);
            WaitReady(Path.Combine(b.Source, e.Name));
            try
            {
                File.Copy(Path.Combine(b.Source, e.Name), Path.Combine(b.Destination, e.Name), true);
            }
            catch (Exception excep)
            {
                Logger.Info("Exception Nouveau/Edit fichier : " + excep.Message);
            }
        }

        public static void DeletedFile(FileSystemEventArgs e, Backup b)
        {
            WaitReady(Path.Combine(b.Destination, e.Name));
            Logger.Info("Fichier supprime : " + e.Name);
            try
            {
                File.Delete(Path.Combine(b.Destination, e.Name));
            }
            catch (Exception excep)
            {
                Logger.Info("Exception suppression fichier : " + excep.Message);
            }
        }

        public static void RenamedFile(RenamedEventArgs e, Backup b)
        {
            WaitReady(Path.Combine(b.Source, e.Name));
            WaitReady(Path.Combine(b.Destination, e.OldName));
            Logger.Info("Fichier renomme : " + e.Name);
            try
            {
                File.Copy(Path.Combine(b.Source, e.Name), Path.Combine(b.Destination, e.Name), true);
                File.Delete(Path.Combine(b.Destination, e.OldName));
            }
            catch (Exception excep)
            {
                Logger.Info("Exception renommage du fichier : " + excep.Message);
            }            
        }
    }
}

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
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(FileDiffEvaluator));
        private const int numberOfTry = 10;

        /// <summary>
        /// Waits until a file can be opened with write permission
        /// http://stackoverflow.com/a/699587/3900435
        /// </summary>
        public static void WaitReady(string fileName)
        {
            int counter = 0;
            while (counter < 100)
            {
                try
                {
                    File.Copy(Path.Combine(b.Source, e.Name), Path.Combine(b.Destination, e.Name), true);
                }
                catch (Exception excep)
                {
                    Console.WriteLine("Exception nouveau fichier : \n" + excep.ToString());
                    break;
                }

                counter++;

                if (counter == numberOfTry)
                {
                    break;
                }
            }
        }

        static public void deletedFile(FileSystemEventArgs e, Backup b)
        {
            int counter = 0;
            while (IsFileLocked(new FileInfo(Path.Combine(b.Destination, e.Name))))
            {
                Console.WriteLine("Fichier supprime : " + e.Name);
                try
                {
                    File.Delete(Path.Combine(b.Destination, e.Name));
                }
                catch (Exception excep)
                {
                    Console.WriteLine("Exception suppression fichier : \n" + excep.ToString());
                }

                counter++;

                if (counter == numberOfTry)
                {
                    break;
                }
                File.Copy(Path.Combine(b.Source, e.Name), Path.Combine(b.Destination, e.Name), true);
            }
            catch (Exception excep)
            {
            }
        }

        static public void deletedFile(FileSystemEventArgs e, Backup b)
        {
            WaitReady(Path.Combine(b.Destination, e.Name));
            try
            {
                File.Delete(Path.Combine(b.Destination, e.Name));
            }
            catch (Exception excep)
            {
            }
        }

        static public void renamedFile(RenamedEventArgs e, Backup b)
        {
            WaitReady(Path.Combine(b.Source, e.Name));
            WaitReady(Path.Combine(b.Destination, e.OldName));
            try
            {
                File.Copy(Path.Combine(b.Source, e.Name), Path.Combine(b.Destination, e.Name), true);
                File.Delete(Path.Combine(b.Destination, e.OldName));
            }
            catch (Exception excep)
            {
            }            
        }
    }
}

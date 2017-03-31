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
    static class FileDiffEvaluator
    {
        private const int numberOfTry = 3;
        private static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                Thread.Sleep(500);
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            
            return false;
        }

        static public void newFile(FileSystemEventArgs e, Backup b)
        {
            int counter = 0;
            while (IsFileLocked(new FileInfo(Path.Combine(b.Source, e.Name))))
            {
                Console.WriteLine("Nouveau fichier : " + e.Name);
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
            }
        }

        static public void changedFile(FileSystemEventArgs e, Backup b)
        {
            int counter = 0;
            while (IsFileLocked(new FileInfo(Path.Combine(b.Source, e.Name))))
            {
                Console.WriteLine("Fichier modifie : " + e.Name);
                try
                {
                    File.Copy(Path.Combine(b.Source, e.Name), Path.Combine(b.Destination, e.Name), true);
                }
                catch (Exception excep)
                {
                    Console.WriteLine("Exception modification fichier : \n" + excep.ToString());
                }

                counter++;

                if (counter == numberOfTry)
                {
                    break;
                }
            }
        }

        static public void renamedFile(RenamedEventArgs e, Backup b)
        {
            int counter = 0;
            while (IsFileLocked(new FileInfo(Path.Combine(b.Source, e.Name))))
            {
                Console.WriteLine("Fichier renomme : " + e.Name);
                try
                {
                    File.Copy(Path.Combine(b.Source, e.Name), Path.Combine(b.Destination, e.Name), true);
                    File.Delete(Path.Combine(b.Destination, e.OldName));
                }
                catch (Exception excep)
                {
                    Console.WriteLine("Exception renommage du fichier : \n" + excep.ToString());
                }

                counter++;

                if (counter == numberOfTry)
                {
                    break;
                }
            }
            
        }
    }
}

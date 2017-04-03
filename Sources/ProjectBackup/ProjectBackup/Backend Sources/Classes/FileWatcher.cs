using ProjectBackup.Backend_Sources.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectBackup.Backend_Sources.Threads
{
    public class FileWatcher
    {
        public FileSystemWatcher Watcher;
        public Backup backup;

        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(FileWatcher));

        public FileWatcher(Backup b)
        {
            backup = b;

            Run();
        }

        public void Run()
        {
            Logger.Info("Initialisation of the file watcher");

            try
            {
                Watcher = new FileSystemWatcher();
                Watcher.Path = backup.Source;
                Watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastAccess;
                Watcher.Filter = "*.*";
                Watcher.Changed += new FileSystemEventHandler(NewOrChangedFile);
                Watcher.Created += new FileSystemEventHandler(NewOrChangedFile);
                Watcher.Deleted += new FileSystemEventHandler(DeletedFile);
                Watcher.Renamed += new RenamedEventHandler(RenamedFile);
                Watcher.EnableRaisingEvents = true;
                Watcher.IncludeSubdirectories = true;
                Watcher.InternalBufferSize = 32768;          
            }
            catch (Exception)
            {

                Logger.Error("Initialisation of the watcher did not work");
                throw;
            }
            Logger.Info("Initialisation of the watcher completed");
        }

        private void NewOrChangedFile(object source, FileSystemEventArgs e)
        {
            Logger.Info("New file");
            //FileDiffEvaluator.NewOrChangedFile(e, backup);
            Thread t = new Thread(() => FileDiffEvaluator.NewOrChangedFile(e, backup));
            t.Start();
        }

        private void DeletedFile(object source, FileSystemEventArgs e)
        {
            Logger.Info("Deleted file");
            //FileDiffEvaluator.DeletedFile(e, backup);
            Thread t = new Thread(() => FileDiffEvaluator.DeletedFile(e, backup));
            t.Start();
        }

        private void RenamedFile(object source, RenamedEventArgs e)
        {
            Logger.Info("renamed file");
            //FileDiffEvaluator.RenamedFile(e, backup);
            Thread t = new Thread(() => FileDiffEvaluator.RenamedFile(e, backup));
            t.Start();
        }
    }
}

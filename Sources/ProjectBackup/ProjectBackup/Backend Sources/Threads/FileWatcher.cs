using ProjectBackup.Backend_Sources.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBackup.Backend_Sources.Threads
{
    class FileWatcher
    {
        public FileSystemWatcher watcher;
        private Backup backup;

        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(FileWatcher));

        public FileWatcher(Backup b)
        {
            backup = b;
        }

        public void Run()
        {
            _logger.Info("Initialisation of the file watcher");

            try
            {
                watcher = new FileSystemWatcher();
                watcher.Path = backup.Source;
                watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                watcher.Filter = "*.*";
                watcher.Changed += new FileSystemEventHandler(changedFile);
                watcher.Created += new FileSystemEventHandler(newFile);
                watcher.Deleted += new FileSystemEventHandler(deletedFile);
                watcher.Renamed += new RenamedEventHandler(renamedFile);
                watcher.EnableRaisingEvents = true;
            }
            catch (Exception)
            {

                _logger.Error("Initialisation of the watcher did not work");
                throw;
            }
            _logger.Info("Initialisation of the watcher completed");
        }

        private void newFile(object source, FileSystemEventArgs e)
        {
            _logger.Info("New file");
            FileDiffEvaluator.newFile(e, backup);
        }

        private void deletedFile(object source, FileSystemEventArgs e)
        {
            _logger.Info("Deleted file");
            FileDiffEvaluator.deletedFile(e, backup);
        }

        private void changedFile(object source, FileSystemEventArgs e)
        {
            _logger.Info("Changed file");
            FileDiffEvaluator.changedFile(e, backup);
        }

        private void renamedFile(object source, RenamedEventArgs e)
        {
            _logger.Info("renamed file");
            FileDiffEvaluator.renamedFile(e, backup);
        }
    }
}

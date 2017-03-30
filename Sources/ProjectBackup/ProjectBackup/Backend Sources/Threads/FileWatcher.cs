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

        public FileWatcher(Backup b)
        {
            backup = b;
        }

        public void Run()
        {
            watcher = new FileSystemWatcher();
            watcher.Path = backup.Source;
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.*";
            watcher.Changed += new FileSystemEventHandler(launchCopy);
            watcher.Created += new FileSystemEventHandler(launchCopy);
            watcher.EnableRaisingEvents = true;
        }

        private void launchCopy(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("On lit le fichier changé");
        }
    }
}

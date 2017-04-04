using ProjectBackup.Backend_Sources.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectBackup.Backend_Sources.Threads
{
    /// <summary>
    /// Class that watch a folder and trigger methods on certains elements
    /// </summary>
    public class FileWatcher
    {
        public FileSystemWatcher Watcher;           // Watcher that will trigger the changes
        public string source;                       // Source folder to watch
        public string destination;                  // Destination folder
        public bool watcherRunning;                 // Define if the watcher is running

        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(FileWatcher));

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="source">Source folder of the watch</param>
        /// <param name="destination">Destination folder</param>
        public FileWatcher(string source, string destination)
        {
            this.source = source;
            this.destination = destination;
        }

        /// <summary>
        /// Method that make a first copy of the source folder
        /// </summary>
        public void InitNew()
        {
            string[] files = Directory.GetFiles(source);

            foreach (var item in files)
            {
                FileSystemEventArgs e = new FileSystemEventArgs(WatcherChangeTypes.All, source, Path.GetFileName(item));
                Thread t = new Thread(() => FileDiffEvaluator.NewOrChangedFile(e, source, destination));
                t.Start();
            }
        }

        /// <summary>
        /// Method that will make a copy of the file that changed when the watcher is offline
        /// </summary>
        public void InitStart()
        {
            // If the time file exist
            if (File.Exists(Path.Combine(source, ".backup")))
            {
                // Get the time of the modification of the file
                DateTime modification = File.GetLastWriteTime(Path.Combine(source, ".backup"));

                // Get the file list of the directory
                string[] files = Directory.GetFiles(source);

                // For each of the file
                foreach (var item in files)
                {
                    // Start a new thread that will update all the file of a backup folder
                    Thread t = new Thread(() => FileDiffEvaluator.UpdateFileByDate(modification, item, source, destination));
                    t.Start();
                }

            }

            // Otherwise just do nothing
        }

        /// <summary>
        /// Method that will initiate the watcher 
        /// </summary>
        public void Run()
        {
            Logger.Info("Initialisation of the file watcher");

            try
            {
                Watcher = new FileSystemWatcher();
                Watcher.Path = source;
                Watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastAccess;
                Watcher.Filter = "*.*";
                Watcher.Changed += new FileSystemEventHandler(NewOrChangedFile);
                Watcher.Created += new FileSystemEventHandler(NewOrChangedFile);
                Watcher.Deleted += new FileSystemEventHandler(DeletedFile);
                Watcher.Renamed += new RenamedEventHandler(RenamedFile);
                Watcher.EnableRaisingEvents = true;
                Watcher.IncludeSubdirectories = true;
                Watcher.InternalBufferSize = 32768;

                watcherRunning = true;
            }
            catch (Exception)
            {

                Logger.Error("Initialisation of the watcher did not work");
                throw;
            }
            Logger.Info("Initialisation of the watcher completed");
        }

        /// <summary>
        /// Method that will trigger on new or changed file
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private void NewOrChangedFile(object obj, FileSystemEventArgs e)
        {
            Logger.Info("New file");
            
            // Start a new thread of the file to lower the wait time of the watcher
            Thread t = new Thread(() => FileDiffEvaluator.NewOrChangedFile(e, source, destination));
            t.Start();
        }

        /// <summary>
        /// Method that will trigger on a deleted file
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private void DeletedFile(object obj, FileSystemEventArgs e)
        {
            Logger.Info("Deleted file");

            // Start a new thread of the file to lower the wait time of the watcher
            Thread t = new Thread(() => FileDiffEvaluator.DeletedFile(e, source, destination));
            t.Start();
        }

        /// <summary>
        /// Method that will trigger when a file is renamed
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private void RenamedFile(object obj, RenamedEventArgs e)
        {
            Logger.Info("renamed file");

            // Start a new thread of the file to lower the wait time of the watcher
            Thread t = new Thread(() => FileDiffEvaluator.RenamedFile(e, source, destination));
            t.Start();
        }
    }
}

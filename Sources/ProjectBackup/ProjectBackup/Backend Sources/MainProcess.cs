using System.Collections.Generic;
using ProjectBackup.Backend_Sources.Classes;

namespace ProjectBackup.Backend_Sources
{
    class MainProcess
    {
        /***
         * For the purpose of the project, the main class will manage all backup configurations,
         * Saved configuration and will manage the thread lunch
         */
        public List<Backup> backupList { get; set; }
    }
}

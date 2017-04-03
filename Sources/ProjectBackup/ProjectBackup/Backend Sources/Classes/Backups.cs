using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBackup.Backend_Sources.Classes
{
    /// <summary>
    /// This class is used to create a collection to simplify the save of the backup list into
    /// a configuration file.
    /// 
    /// This class is inspired of this:
    /// https://msdn.microsoft.com/en-us/library/58a18dwa(v=vs.110).aspx
    /// 
    /// The serialize and deserialize methods are in the MainWindow.xaml.cs file
    /// </summary>
    public class Backups : ICollection
    {
        public string CollectionName;
        private ArrayList backupArray = new ArrayList();

        public Backup this[int index]
        {
            get { return (Backup) backupArray[index]; }
        }

        public void CopyTo(Array array, int index)
        {
            backupArray.CopyTo(array, index);
        }

        public int Count
        {
            get { return backupArray.Count; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public IEnumerator GetEnumerator()
        {
            return backupArray.GetEnumerator();
        }

        public void Add(Backup newBackup)
        {
            backupArray.Add(newBackup);
        }
    }
}

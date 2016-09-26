﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCore;
using DataCore.Structures;
using Grimoire.Functions;

namespace Grimoire.Functions
{
    public class FileIO
    {
        protected Core core;
        public List<IndexEntry> Index = null;
        protected string indexPath = null;
        protected string dataDir = null;
        protected string buildDir = null;
        protected long pbMax = 0;
        protected ProgressBar pb;

        public int Count { get { return Index.Count; } }

        public bool IsEncoded(string name) { return core.IsEncoded(name); }

        public string EncodeName(string name) { return core.EncodeName(name); }

        public string DecodeName(string name) { return core.DecodeName(name); }

        public int GetID(string name) { return core.GetID(name); }

        public bool CoreEncoded { get { return core.NamesEncoded; } }

        public void Start()
        {
            dataDir = Path.GetDirectoryName(indexPath);
            buildDir = OPT.Instance.GetString("build.path");
            core = new Core(System.Text.Encoding.Default, OPT.Instance.GetBool("backups"));
            setEvents();
        }

        internal void Clear()
        {
            Console.WriteLine("Closing now could result in unsaved work! You should be very careful what you do! Are you sure you would like to continue");
            if (Input.YesNo)
            {
                Index.Clear();
                Index = new List<IndexEntry>();
                Console.WriteLine("\t- The index has been cleared, you must now load a new data.000 by using the load command.");
            }
        }

        protected void setEvents()
        {
            core.ErrorOccured += (o, x) => { };
            core.WarningOccured += (o, x) => { };
            core.TotalMaxDetermined += (o, x) =>
            {
                pbMax = x.Maximum;

                if (x.IsTasks) { Console.WriteLine("Executing a total of {0} tasks...", x.Maximum); }
                //TODO: TotalProgress doesn't need to have a reportProgress
            };
            core.TotalProgressChanged += (o, x) =>
            {
                if (x.IgnoreStatus) { pb.Report((double)x.Value / pbMax); }
                else
                {
                    if (pbMax == 8) { Console.WriteLine("\t- Task: {0}", x.Status); }
                    else { Console.Write("\t- Task: {0}", x.Status); }
                }
            };
            core.TotalProgressReset += (o, x) =>
            {
                pb.Dispose();
                pbMax = 0;
            };
            core.CurrentMaxDetermined += (o, x) =>
            {
                pb = new Functions.ProgressBar();
                pbMax = x.Maximum;
            };
            core.CurrentProgressChanged += (o, x) =>
            {
                if (!string.IsNullOrEmpty(x.Status)) { Console.Write("\t\t- {0}...", x.Status); }
                pb.Report((double)x.Value / pbMax);
            };
            core.CurrentProgressReset += (o, x) =>
            {
                pb.Dispose();
                pbMax = 0;
                if (x.WriteOK) { Console.WriteLine("[OK]"); }
            };
        }


        // TODO: Dynamically decide the size of the reportInterval
        public bool Load()
        {
            Console.Write("Indexing the data.000...");
            Index = core.Load(OPT.Instance.GetString("client.path"), true, 64000);
            Console.WriteLine("\t\t- {0} file entries loaded!", Count);

            return Index.Count > 0 ? true : false;
        }

        public List<IndexEntry> Search(string term, int type)
        {
            List<IndexEntry> temp = new List<IndexEntry>();

            switch (type)
            {
                case 0:
                    IndexEntry entry = core.GetEntry(ref Index, term);
                    if (entry != null) { temp.Add(entry); }
                    break;

                case 1:
                    temp = core.GetEntriesByPartialName(ref Index, term);
                    break;

                case 2:
                    temp = core.GetEntriesByExtension(ref Index, term);
                    break;
            }

            return temp;
        }

        public string SHA512(ref IndexEntry entry)
        {
            return core.GetFileSHA512(dataDir, entry.DataID, entry.Offset, entry.Length, Path.GetExtension(entry.Name).Remove(0, 1)); 
        }

        public void Delete(ref List<IndexEntry> entries)
        {
            Console.WriteLine("Please review the following files and make absolutely sure you want to delete them!");

            foreach (IndexEntry entry in entries) { Console.WriteLine("\tName: {0}\n\tLocation: data.00{1}\n\tSize: {2}", entry.Name, entry.DataID, entry.Length); }

            Console.WriteLine("Are you sure you wish to continue?");
            if (Input.YesNo)
            {
                Console.WriteLine("Erasing files from the Data File System...");

                foreach (IndexEntry entry in entries)
                {
                    Console.Write("\t- Deleting {0}...", entry.Name);
                    core.DeleteEntryByName(ref Index, entry.Name, dataDir, entry.DataID, entry.Offset, entry.Length);
                    Console.WriteLine("[OK]");
                }
            }
        }

        public void Export(ref List<IndexEntry> entries)
        {
            if (entries.Count > 1) { core.ExportFileEntries(ref entries, dataDir, buildDir, 64000); }
            else if (entries.Count == 1)
            {
                Console.Write("\t- Exporting file {0}...", entries[0].Name);
                core.ExportFileEntry(dataDir, string.Format(@"{0}\{1}", buildDir, entries[0].Name), entries[0].Offset, entries[0].Length, 64000);
            }
        }
        
        // TODO: Dynamically set chunkSize
        public void Import(string filePath)
        {
            if (File.Exists(filePath))
            {
                string fileName = Path.GetFileName(filePath);
                if (IsEncoded(fileName)) { fileName = DecodeName(fileName); }
                Console.Write("\t- Importing file {0} to data.00{1}...", fileName, GetID(fileName));

                core.ImportFileEntry(ref Index, dataDir, filePath, 64000);
            }
        }

        public void Save()
        {
            Console.Write("Saving the data.000 index to disk...");
            core.Save(ref Index, dataDir, false);
        }
    }
}

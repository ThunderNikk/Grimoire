using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Client.Structures;

namespace Grimoire.Functions
{
    public class OPT
    {
        protected string optName = "config.opt";
        protected string optPath;
        protected XDes des;
        protected string encKey = "2121d00sd";
        protected List<LauncherSetting> settingsList = new List<LauncherSetting>();
        protected List<string> defaultSettings = new List<string>
        {
            "s|client.path|0",
            "s|build.path|0",
            "b|auto.hash|false",
            "b|auto.load|false",
            "b|auto.show|false",
            "b|backups|true",
            "n|chunk.size|0",
            "n|open.action|0", // 0 = Hash, 1 = Insert, 2 = Compare           
        };

        protected static OPT instance;
        public static OPT Instance
        {
            get
            {
                if (instance == null) { instance = new OPT(); }
                return instance;
            }
        }

        public void Start()
        {
            des = new XDes(encKey);
            optPath = Path.Combine(Directory.GetCurrentDirectory(), optName);

            if (!optExists) { preloadDefaults(); writeDefault(); }
            else { read(); }
        }

        protected bool optExists
        {
            get { return File.Exists(optPath); }
        }

        internal int Count { get { return settingsList.Count; } }

        internal bool SettingExists(string name)
        {
            return settingsList.Find(s => s.Name == name) != null ? true : false;
        }

        internal string GetType(string name)
        {
            LauncherSetting setting = settingsList.Find(s => s.Name == name);
            return (setting != null) ? setting.Type : "e";
        }

        internal int GetInt(string name)
        {
            LauncherSetting setting = settingsList.Find(s => s.Name == name);
            return (setting != null) ? Convert.ToInt32(setting.Value) : 0;
        }

        internal string GetString(string name)
        {
            LauncherSetting setting = settingsList.Find(s => s.Name == name);
            return (setting != null) ? setting.Value.ToString() : null;
        }

        internal bool GetBool(string name)
        {
            LauncherSetting setting = settingsList.Find(s => s.Name == name);
            return (setting != null) ? Convert.ToBoolean(setting.Value) : false;
        }

        internal void Update(string name, object value)
        {
            settingsList.Find(s => s.Name == name).Value = value;
        }

        protected void preloadDefaults()
        {
            if (settingsList.Count > 0) { settingsList.Clear(); }

            foreach (string setting in defaultSettings)
            {
                string[] optBlocks = setting.Split('|');
                if (optBlocks.Length == 3)
                {
                    string type = optBlocks[0];
                    string name = optBlocks[1];
                    object value = null;

                    switch (type)
                    {
                        case "s": // string
                            value = optBlocks[2].TrimEnd('\0');
                            break;

                        case "n":
                            value = Convert.ToInt32(optBlocks[2]);
                            break;

                        case "b": // bool
                            value = Convert.ToBoolean(optBlocks[2]);
                            break;
                    }

                    if (!string.IsNullOrEmpty(name) && value != null) { settingsList.Add(new LauncherSetting() { Type = type, Name = name, Value = value }); }
                }
            }
        }

        protected void writeDefault()
        {
            using (FileStream fs = new FileStream(optPath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    foreach (string setting in defaultSettings)
                    {
                        byte[] encBuffer = des.Encrypt(setting);
                        bw.Write(Convert.ToInt32(encBuffer.Length));
                        bw.Write(encBuffer);
                    }
                }
            }           
        }

        protected void read()
        {
            bool deleteOpt = false;

            try
            {
                using (BinaryReader br = new BinaryReader(File.Open(optPath, FileMode.Open, FileAccess.Read), Encoding.ASCII))
                {
                    while (br.PeekChar() != -1)
                    {
                        int len = 0;
                        len = br.ReadInt32();
                        string[] optBlocks = des.Decrypt(br.ReadBytes(len)).Split('|');

                        if (optBlocks.Length == 3)
                        {
                            string type = optBlocks[0];
                            string name = optBlocks[1];
                            object value = null;

                            switch (type)
                            {
                                case "s": // string
                                    value = optBlocks[2].TrimEnd('\0');
                                    break;

                                case "b": // bool
                                    value = Convert.ToBoolean(optBlocks[2]);
                                    break;

                                case "n":
                                    value = Convert.ToInt32(optBlocks[2]);
                                    break;
                            }

                            if (!string.IsNullOrEmpty(name) && value != null) { settingsList.Add(new LauncherSetting() { Type = type, Name = name, Value = value }); }
                        }
                        else
                        {
                            deleteOpt = true;
                            break;
                        }
                    }
                }
            }
            catch
            {
                deleteOpt = true;
            }

            if (deleteOpt)
            {
                Console.WriteLine("Malformed OPT File, Resetting...");
                File.Delete("config.opt");
                preloadDefaults();
                writeDefault();
            }
        }

        internal void Write()
        {
            Task.Run(() => 
            {
                using (BinaryWriter bw = new BinaryWriter(File.Open(optPath, FileMode.OpenOrCreate, FileAccess.Write), Encoding.ASCII))
                {
                    foreach (LauncherSetting setting in settingsList)
                    {
                        string encString = encString = string.Format("{0}|{1}|{2}", setting.Type, setting.Name, setting.Value.ToString());
                        byte[] encBuffer = des.Encrypt(encString);
                        bw.Write(Convert.ToInt32(encBuffer.Length));
                        bw.Write(encBuffer);

                    }
                }
            });
        }
    }
}

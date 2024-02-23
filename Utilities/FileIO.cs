using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleUserLoginForm.Utilities
{
    public class FileIO<T>
    {
        string Path { get; }
        public FileIO(string path)
        {
            Path = path;
        }
        public T LoadData()
        {
            bool fileExists = File.Exists(Path);
            if (!fileExists)
            {
                File.CreateText(Path).Dispose();
                return default(T);
            }
            using (var reader = File.OpenText(Path))
            {
                var filetext = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(filetext);
            }

        }
        public void SaveData(T list)
        {
            using (StreamWriter sw = new StreamWriter(Path))
            {
                string output = JsonConvert.SerializeObject(list);
                sw.Write(output);
            }
        }
    }
}

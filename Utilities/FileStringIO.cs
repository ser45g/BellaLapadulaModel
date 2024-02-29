using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleUserLoginForm.Utilities
{
    
    public class FileStrings
    {
        string Path { get; }
        public FileStrings(string path)
        {
            Path = path;
        }
        public List<string> LoadData()
        {
            List<string> list = new List<string>();
            bool fileExists = File.Exists(Path);
            if (!fileExists)
            {
                //File.CreateText(Path).Dispose();
                //return default(T);
                using (var stream = File.Create(Path))
                {

                }
            }
            using (var reader = File.OpenText(Path))
            {
                while (!reader.EndOfStream)
                {
                    list.Add(reader.ReadLine());
                }
                return list;
            }

        }
        public void SaveData(IEnumerable<string> list)
        {
            using (StreamWriter sw = new StreamWriter(Path))
            {

                foreach (var item in list) { sw.WriteLine(item); }
            }
        }
    }
}

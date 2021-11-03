using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot_WPF
{
    class FileSaveAndLoade : IInfoFilesSource<InfoFiles>
    {
        // путь к файлу
        public readonly string path;

        public FileSaveAndLoade(string path)
        {
            this.path = path;
        }

        public ObservableCollection<InfoFiles> LoadFile()
        {
            var fileExists = File.Exists(path);

            if (!fileExists)
            {
                File.CreateText(path).Dispose();
                return new ObservableCollection<InfoFiles>();
            }
            using (var reader = File.OpenText(path))
            {
                var fileText = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<ObservableCollection<InfoFiles>>(fileText); 
            }
        }
        public ObservableCollection<InfoFiles> LoadFile(string pathOfUser)
        {
            using (var reader = File.OpenText(pathOfUser))
            {
                var fileText = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<ObservableCollection<InfoFiles>>(fileText);
            }
        }

        public void SaveFile(ObservableCollection<InfoFiles> InfoFile)
        {
            using (StreamWriter writer = File.CreateText(path))
            {
                string output = JsonConvert.SerializeObject(InfoFile);
                writer.Write(output);
            }
        }
        public void SaveFile(ObservableCollection<InfoFiles> InfoFile, string name)
        {
           string output = JsonConvert.SerializeObject(InfoFile);

           File.WriteAllText(name, output);
        }
        

    }

    interface IInfoFilesSource<T>
    {
        void SaveFile(ObservableCollection<T> InfoFile);
        ObservableCollection<T> LoadFile();
    }



}

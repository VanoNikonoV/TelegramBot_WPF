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
    class MessageLogSaveAndLoade : IFilesSource<TelegramUser>
    {
        // путь к файлу
        public readonly string path;

        public MessageLogSaveAndLoade(string path)
        {
            this.path = path;
        }

        public ObservableCollection<TelegramUser> LoadFile()
        {
            var fileExists = File.Exists(path);

            if (!fileExists)
            {
                File.CreateText(path).Dispose();
                return new ObservableCollection<TelegramUser>();
            }
            using (var reader = File.OpenText(path))
            {
                var fileText = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<ObservableCollection<TelegramUser>>(fileText); 
            }
        }
        public ObservableCollection<TelegramUser> LoadFile(string pathOfUser)
        {
            using (var reader = File.OpenText(pathOfUser))
            {
                var fileText = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<ObservableCollection<TelegramUser>>(fileText);
            }
        }

        public void SaveFile(ObservableCollection<TelegramUser> InfoFile)
        {
            using (StreamWriter writer = File.CreateText(path))
            {
                string output = JsonConvert.SerializeObject(InfoFile);
                writer.Write(output);
            }
        }
        public void SaveFile(ObservableCollection<TelegramUser> InfoFile, string name)
        {
           string output = JsonConvert.SerializeObject(InfoFile);

           File.WriteAllText(name, output);
        }
        

    }

    interface IFilesSource<T>
    {
        void SaveFile(ObservableCollection<T> InfoFile);
        ObservableCollection<T> LoadFile();
    }



}

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
    public class FileSaveAndLoade : IInfoFilesSource<InfoFiles>
    {
        /// <summary>
        /// Десерилазует файл по указаному пути
        /// </summary>
        /// <param name="pathOfUser">Путь к файлу .json</param>
        /// <returns></returns>
        public ObservableCollection<InfoFiles> LoadFile(string pathOfUser)
        {
            using (var reader = File.OpenText(pathOfUser))
            {
                var fileText = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<ObservableCollection<InfoFiles>>(fileText);
            }
        }
        /// <summary>
        /// Сериализует коллекцию в файл .json
        /// </summary>
        /// <param name="InfoFile">Коллекция для сериализации</param>
        /// <param name="path">Путь к файлу</param>
        public void SaveFile(ObservableCollection<InfoFiles> InfoFile, string path)
        {
           string output = JsonConvert.SerializeObject(InfoFile);

           File.WriteAllText(path, output);
        }
    }

    interface IInfoFilesSource<T>
    {
        void SaveFile(ObservableCollection<T> InfoFile, string path);
        ObservableCollection<T> LoadFile(string pathOfUser);
    }



}

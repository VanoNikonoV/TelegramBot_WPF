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
        private readonly string path;

        public MessageLogSaveAndLoade(string path)
        {
            this.path = path;
        }

        /// <summary>
        /// Десерилазует файл по указаному пути
        /// </summary>
        /// <returns></returns>
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

                var temp = JsonConvert.DeserializeObject<ObservableCollection<TelegramUser>>(fileText);

                if (temp is null) return new ObservableCollection<TelegramUser>();

                else return temp;
            }
        }

        /// <summary>
        /// Десерилазует файл по указаному пути
        /// </summary>
        /// <param name="pathOfUser">путь к сохраняемому файлу</param>
        /// <returns></returns>
        public ObservableCollection<TelegramUser> LoadFile(string pathOfUser)
        {
            using (var reader = File.OpenText(pathOfUser))
            {
                var fileText = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<ObservableCollection<TelegramUser>>(fileText);
            }
        }

        /// <summary>
        /// Сериализует коллекцию в файл .json
        /// </summary>
        /// <param name="User">Коллеция для сериализации</param>
        public void SaveFile(ObservableCollection<TelegramUser> User)
        {
            using (StreamWriter writer = File.CreateText(path))
            {
                string output = JsonConvert.SerializeObject(User);
                writer.Write(output);
            }
        }

        /// <summary>
        /// Сериализует коллекцию в файл .json
        /// </summary>
        /// <param name="Message">Коллеция для сериализации</param>
        /// <param name="path">путь к сохраняемому файлу</param>
        public void SaveMessageUser (ObservableCollection<Message> Message, string path)
        {
            string output = JsonConvert.SerializeObject(Message);

            File.WriteAllText(path, output);
        }

    }

    interface IFilesSource<T>
    {
        void SaveFile(ObservableCollection<T> User);
        ObservableCollection<T> LoadFile();
    }



}

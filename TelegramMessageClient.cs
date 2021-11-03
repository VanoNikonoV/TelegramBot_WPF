using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot_WPF
{
    class TelegramMessageClient
    {
        private MainWindow window;

        private TelegramBotClient bot;

        public ObservableCollection<TelegramUser> Users { get; set; }

        public ObservableCollection<InfoFiles> InfoFiles { get; set; }

        [Obsolete]
        public TelegramMessageClient(MainWindow W, string PathToken = @"token.txt")
        {
            this.Users = new ObservableCollection<TelegramUser>();

            this.InfoFiles = new ObservableCollection<InfoFiles>();

            this.window = W;

            bot = new TelegramBotClient(System.IO.File.ReadAllText(PathToken));

            bot.OnMessage += MessageListener;

            bot.StartReceiving();
        }

        [Obsolete]
        private async void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string text = $"{DateTime.Now.ToLongTimeString()}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";

            Debug.WriteLine(text);

            //UserProfilePhotos u = bot.GetUserProfilePhotosAsync(e.Message.Chat.Id, 1, 1);

            if (e.Message.Type == MessageType.Text)
            {
                var messageText = e.Message.Text;

                window.Dispatcher.Invoke(() =>
                {
                    var person = new TelegramUser(e.Message.Chat.FirstName, e.Message.Chat.Id);

                    if (!Users.Contains(person)) Users.Add(person);
                    {
                        Users[Users.IndexOf(person)].AddMessage(e.Message.Text);
                    }
                }); 
            }

            #region скачать файл
            
            if (e.Message.Type == MessageType.Document)
            {
                DownLoad(e.Message.Document.FileId, e.Message.Document.FileName);

                await bot.SendTextMessageAsync(e.Message.From.Id, $"файл {e.Message.Document.FileName} скачен");
            }

            if (e.Message.Type == MessageType.Audio)
            {
                DownLoad(e.Message.Audio.FileId, e.Message.Audio.FileName); //??= e.Message.Audio.Title
                
                await bot.SendTextMessageAsync(e.Message.From.Id, $"файл {e.Message.Audio.FileName} скачен");
            }

            if (e.Message.Type == MessageType.Photo)
            {
                DownLoad(e.Message.Photo[e.Message.Photo.Length - 1].FileId, e.Message.Caption); //??= e.Message.Photo[e.Message.Photo.Length - 1].FileUniqueId

                await bot.SendTextMessageAsync(e.Message.From.Id, $"файл {e.Message.Caption} скачен");
            }

            if (e.Message.Type == MessageType.Video)
            {
                DownLoad(e.Message.Video.FileId, e.Message.Video.FileName);

                await bot.SendTextMessageAsync(e.Message.From.Id, $"файл {e.Message.Video.FileName} скачен");
            }

            if (e.Message.Type == MessageType.Sticker)
            {
                DownLoad(e.Message.Sticker.FileId, e.Message.Sticker.SetName + e.Message.Sticker.FileUniqueId);

                await bot.SendTextMessageAsync(e.Message.From.Id, $"файл {e.Message.Sticker.SetName + e.Message.Sticker.FileUniqueId} скачен");
            }

            if (e.Message.Type == MessageType.Voice)
            {
                DownLoad(e.Message.Voice.FileId, e.Message.Voice.MimeType);

                await bot.SendTextMessageAsync(e.Message.From.Id, $"файл {e.Message.Voice.MimeType} скачен");
            }
            #endregion
        }

        public void SendMessage(string Text, string Id, TelegramUser user)
        {
            long id = Convert.ToInt64(Id);

            string responseMsg = $"Support: {Text}";

            user.AddMessage(responseMsg);

            bot.SendTextMessageAsync(id, Text);
        }

        /// <summary>
        /// Метод для скачивания файлов на диск
        /// </summary>
        /// <param name="fileId">Уникальный идентификатор файла на сервере Telegram</param>
        /// <param name="name">Имя файла</param>
        protected async void DownLoad(string fileId, string name)
        {
            var file = await bot.GetFileAsync(fileId);

            FileStream fs = new FileStream("_" + name, FileMode.Create);

            await bot.DownloadFileAsync(file.FilePath, fs);

            InfoFiles.Add(new InfoFiles(fileId, name));

            fs.Close();

            fs.Dispose();

            using (StreamWriter writer = System.IO.File.CreateText("FileDownload.json"))
            {
                string output = JsonConvert.SerializeObject(InfoFiles);
                writer.Write(output);
            }

        }
    }
}

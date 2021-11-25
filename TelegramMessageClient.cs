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
using Telegram.Bot.Types.InputFiles;

namespace TelegramBot_WPF
{
    class TelegramMessageClient
    {
        private Home window;

        private TelegramBotClient bot;

        public ObservableCollection<TelegramUser> Users { get; set; }

        public ObservableCollection<InfoFiles> InfoFiles { get; set; }

        private string folderUserPhoto = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image\\");
        //private string UserPhotoImage { get; set; }  // поле?

        [Obsolete]
        public TelegramMessageClient(Home W, string PathToken = @"token.txt")
        {
            this.Users = new ObservableCollection<TelegramUser>();

            this.InfoFiles = new ObservableCollection<InfoFiles>();

            //this.UserPhotoImage = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image\\");

            Directory.CreateDirectory(folderUserPhoto);

            this.window = W;

            bot = new TelegramBotClient(System.IO.File.ReadAllText(PathToken));

            bot.OnMessage += MessageListener;

            bot.StartReceiving();
        }

        [Obsolete]
        private async void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string text = $"{DateTime.Now.ToLongTimeString()}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";

            if (e.Message.Type == MessageType.Text)
            {
                var messageText = e.Message.Text;

                window.Dispatcher.Invoke(() =>
                {
                    var person = new TelegramUser(e.Message.Chat.FirstName, e.Message.Chat.Id);

                    if (!Users.Contains(person))

                    {   // Фото пользователя
                        var userPhoto = bot.GetUserProfilePhotosAsync(e.Message.From.Id).Result;
                        // если у пользователя есть фото то скачать и установить фото
                        if (userPhoto.TotalCount != 0)
                        {
                            DownLoadUserPhoto(userPhoto.Photos[0][2].FileId, e.Message.Chat.Id);
                            //перезаписываю свойство
                            person.PathUserPhoto = this.folderUserPhoto + e.Message.Chat.Id + ".jpg";
                        }
                        // иначе добавить с параметрам по умолчанию
                        Users.Add(person);
                        
                    } 
                    {
                        Users[Users.IndexOf(person)].AddMessage(person.Nick, e.Message.Text, DateTime.Now);
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

        private async void DownLoadUserPhoto(string fileId, long id)
        {
            string path = this.folderUserPhoto + id + ".jpg";

            var file = await bot.GetFileAsync(fileId);

            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);

            await bot.DownloadFileAsync(file.FilePath, fs);
          
            fs.Close();

            fs.Dispose();
        }

        public void SendMessage(string Text, string Id, TelegramUser user)
        {
            long id = Convert.ToInt64(Id);

            string responseMsg = Text; //$"Support: {Text}"

            user.AddMessage("Support", responseMsg, DateTime.Now);

            bot.SendTextMessageAsync(id, Text);  
        }

        public void SendVoice(InputOnlineFile fileVoice, string Id, string name)  
        {
            bot.SendAudioAsync(Id, fileVoice, caption: name);
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

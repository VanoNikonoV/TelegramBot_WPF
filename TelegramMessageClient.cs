using System;
using System.Collections.ObjectModel;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace TelegramBot_WPF
{
    class TelegramMessageClient
    {
        private Home window;

        private TelegramBotClient bot;

        private MessageLogSaveAndLoade MessageLog = new MessageLogSaveAndLoade(pathMessageLog);

        public ObservableCollection<TelegramUser> Users { get; set; }

        public ObservableCollection<InfoFiles> InfoFiles { get; set; }

        // путь к системным папкам для хранения фото, файлов от пользователей и всех сообщений
        public static readonly string folderUserPhoto = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PhotoUsers\\");
        
        public static readonly string folderFiles = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FilesUser\\");

        public static readonly string pathMessageLog = $"{Environment.CurrentDirectory}\\MessageLog.json";

        [Obsolete]
        public TelegramMessageClient(Home W, string PathToken = @"token.txt")
        {
            this.Users = new ObservableCollection<TelegramUser>();

            this.InfoFiles = new ObservableCollection<InfoFiles>();

            Directory.CreateDirectory(folderUserPhoto);

            Directory.CreateDirectory(folderFiles);
           
            this.window = W;

            bot = new TelegramBotClient(System.IO.File.ReadAllText(PathToken));

            bot.OnMessage += MessageListener;

            bot.StartReceiving();
        }

        /// <summary>
        /// Обработка сообщений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        { 
            if (e.Message.Type == MessageType.Text)
            {
                window.Dispatcher.Invoke(() =>
                {
                    TelegramUser person = new TelegramUser(e.Message.Chat.FirstName, e.Message.Chat.Id);

                    if (person != null)
                    {
                        if (!Users.Contains(person))
                        {   // Получаю массив с фото пользователя
                            var userPhoto = bot.GetUserProfilePhotosAsync(e.Message.From.Id).Result;

                            // если у пользователя есть фото то скачать
                            if (userPhoto.TotalCount != 0) DownloadUserPhoto(userPhoto.Photos[0][2].FileId, e.Message.Chat.Id);

                            Users.Add(person);
                        }
                        {
                            Users[Users.IndexOf(person)].AddMessage(person.Nick, e.Message.Text, DateTime.Now);
                            MessageLog.SaveFile(Users);
                        }
                    }
                }); 
            }

            #region скачать файл

            if (e.Message.Type == MessageType.Document)
            {
                window.Dispatcher.Invoke(() =>
                {
                    var person = new TelegramUser(e.Message.Chat.FirstName, e.Message.Chat.Id);

                    DownloadFile(e.Message.Document.FileId, e.Message.Document.FileName, person);

                    bot.SendTextMessageAsync(e.Message.From.Id, $"файл {e.Message.Document.FileName} скачен");
                });
            }

            if (e.Message.Type == MessageType.Audio)
            {
                window.Dispatcher.Invoke(() =>
                {
                    var person = new TelegramUser(e.Message.Chat.FirstName, e.Message.Chat.Id);

                    DownloadFile(e.Message.Audio.FileId, e.Message.Audio.FileName, person); //??= e.Message.Audio.Title

                    bot.SendTextMessageAsync(e.Message.From.Id, $"файл {e.Message.Audio.FileName} скачен");
                });
            }

            if (e.Message.Type == MessageType.Photo)
            {
                window.Dispatcher.Invoke(() =>
                {
                    var person = new TelegramUser(e.Message.Chat.FirstName, e.Message.Chat.Id);

                    DownloadFile(e.Message.Photo[e.Message.Photo.Length - 1].FileId, e.Message.Caption, person); //??= e.Message.Photo[e.Message.Photo.Length - 1].FileUniqueId

                    bot.SendTextMessageAsync(e.Message.From.Id, $"файл {e.Message.Caption} скачен");
                });

            }

            if (e.Message.Type == MessageType.Video)
            {
                window.Dispatcher.Invoke(() =>
                {
                    var person = new TelegramUser(e.Message.Chat.FirstName, e.Message.Chat.Id);

                    DownloadFile(e.Message.Video.FileId, e.Message.Video.FileName, person);

                    bot.SendTextMessageAsync(e.Message.From.Id, $"файл {e.Message.Video.FileName} скачен");
                });
            }

            if (e.Message.Type == MessageType.Sticker)
            {
                window.Dispatcher.Invoke(() =>
                {
                    var person = new TelegramUser(e.Message.Chat.FirstName, e.Message.Chat.Id);

                    DownloadFile(e.Message.Sticker.FileId, e.Message.Sticker.SetName + e.Message.Sticker.FileUniqueId, person);

                    bot.SendTextMessageAsync(e.Message.From.Id, $"файл {e.Message.Sticker.SetName + e.Message.Sticker.FileUniqueId} скачен");
                });
            }

            if (e.Message.Type == MessageType.Voice)
            {
                window.Dispatcher.Invoke(() =>
                {
                    var person = new TelegramUser(e.Message.Chat.FirstName, e.Message.Chat.Id);

                    DownloadFile(e.Message.Voice.FileId, e.Message.Voice.MimeType, person);

                    bot.SendTextMessageAsync(e.Message.From.Id, $"файл {e.Message.Voice.MimeType} скачен");
                });
            }
            #endregion
        }

        /// <summary>
        /// Скачивает фото пользователя в системную папку (см. folderUserPhoto)
        /// </summary>
        /// <param name="fileId">ID файла</param>
        /// <param name="id">ID пользователя</param>
        private async void DownloadUserPhoto(string fileId, long id)
        {
            string path = folderUserPhoto + id + ".jpg";

            var file = await bot.GetFileAsync(fileId);

            var fileExists = System.IO.File.Exists(path);

            if (!fileExists)
            {
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);

                await bot.DownloadFileAsync(file.FilePath, fs);
          
                fs.Close();

                fs.Dispose();
            }
        }

        /// <summary>
        /// Метод для скачивания файлов на диск
        /// </summary>
        /// <param name="fileId">Уникальный идентификатор файла на сервере Telegram</param>
        /// <param name="name">Имя файла</param>
        protected async void DownloadFile(string fileId, string name, TelegramUser person)
        {
            var file = await bot.GetFileAsync(fileId);

            FileStream fs = new FileStream("_" + name, FileMode.Create);

            await bot.DownloadFileAsync(file.FilePath, fs);

            Users[Users.IndexOf(person)].AddFile(fileId, name);

            //// путь к сериализованной коллекции 
            //string path = folderFiles + person.Id + ".json";

            //FileDownload.SaveFile(Users[Users.IndexOf(person)].InfoDowloadFiles, path);

            MessageLog.SaveFile(Users);

            fs.Close();

            fs.Dispose();
        }

        /// <summary>
        /// Отправить сообщение пользователью user(ID)
        /// </summary>
        /// <param name="Text">Текст сообщения</param>
        /// <param name="Id">ID пользователя - инициатора сообщения</param>
        /// <param name="user">TelegramUser - инициатор сообщения</param>
        public async void SendMessage(string Text, string Id, TelegramUser user)
        {
            long id = Convert.ToInt64(Id);

            string responseMsg = Text;

            user.AddMessage("Support", responseMsg, DateTime.Now);
            
            MessageLog.SaveFile(Users);

            await bot.SendTextMessageAsync(id, Text);
        }

        /// <summary>
        /// Отправить озвученное сообщений 
        /// </summary>
        /// <param name="fileVoice">Аудио файл</param>
        /// <param name="Id">ID пользователя - инициатора сообщения</param>
        public async void SendVoice(InputOnlineFile fileVoice, string Id)
        {
            await bot.SendAudioAsync(Id, fileVoice);
        }

    }
}

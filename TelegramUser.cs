using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;


namespace TelegramBot_WPF
{
    /// <summary>
    /// Информация о пользователе Telegram
    /// </summary>
    public class TelegramUser : INotifyPropertyChanged, IEquatable<TelegramUser>
    {
        public TelegramUser(string Nickname, long ChatId)
        {
            this.nick = Nickname;
            this.id = ChatId;
            this.Chat = new ObservableCollection<Message>();
            this.InfoDowloadFiles = new ObservableCollection<InfoFiles>(); 
        }

        private string nick;

        [JsonProperty("nick")]
        public string Nick
        {
            get { return this.nick; }
            set
            {
                this.nick = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Nick)));
            }
        }

        private long id;

        [JsonProperty("id user")]
        public long Id
        {
            get { return this.id; }
            set
            {
                this.id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Id)));
            }
        }

        private string folderUserPhoto = TelegramMessageClient.folderUserPhoto;

        /// <summary>
        /// Полная ссылка на фото пользователя (пременная часть + имя файла)
        /// </summary>
        [JsonIgnore]
        public string PathUserPhoto 
        {
            get {   string p = folderUserPhoto + this.id + ".jpg";

                    bool flag = File.Exists(p);

                    if (flag) return folderUserPhoto + this.id + ".jpg";

                    else return @"Image\NoPhoto.png";
                } 

        }

        [JsonIgnore]
        public string LastMessage
        {
            get
            {
                int i = this.Chat.Count;
                --i;
                return Chat[i].Text;
            }
        }

        [JsonIgnore]
        public string LastDateTime 
        { 
            get
            {
                int i = this.Chat.Count;
                --i;
                return Chat[i].Time.ToString();
            }
                
        }        

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Сравнение двух пользователей
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(TelegramUser other) => other.Id == this.id;

        /// <summary>
        /// Коллекция для хранения информации о полученных и отправлнных сообщениях
        /// </summary>
        [JsonProperty("chat")]
        public ObservableCollection<Message> Chat { get; set; }

        /// <summary>
        /// Добавление сообщения в коллекцию
        /// </summary>
        /// <param name="Text">Текс полученного сообщения</param>
        public void AddMessage(string Nick, string Text, DateTime dateTime)
        {
            Chat.Add(new Message(Nick, Text, dateTime));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty)); //что бы обговлялось фото пользователя

            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Chat)));
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.LastMessage)));
        }

        /// <summary>
        /// Коллекция для хранения информации о скачанных файлах
        /// </summary>
        [JsonProperty("infoFiles")]
        public ObservableCollection<InfoFiles> InfoDowloadFiles { get; set; }

        /// <summary>
        /// Добавляет информацию об отправленных пользователем файлах 
        /// </summary>
        /// <param name="fileId">ID скаченого файла</param>
        /// <param name="fileName">Имя скаченого файла</param>
        public void AddFile(string fileId, string fileName)
        {
            InfoDowloadFiles.Add(new InfoFiles(fileId, fileName, InfoDowloadFiles.Count));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.InfoDowloadFiles)));
        }

    }
}

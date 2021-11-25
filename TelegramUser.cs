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
        // путь к папке с фотографиями пользоватей
        private string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image\\");

        public TelegramUser(string Nickname, long ChatId, string noPhoto = @"NoPhoto.png")
        {
            this.nick = Nickname;
            this.id = ChatId;
            this.Chat = new ObservableCollection<Message>();
            this.pathUserPhoto = path + noPhoto;
            //Messages = new ObservableCollection<string>();
            //this.Time = DateTime.Now;
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

        string pathUserPhoto;
        //ссылка на фото пользователя
        [JsonProperty("pathUserPhoto")]
        public string PathUserPhoto 
        {
            get { return this.pathUserPhoto; }

            set { this.pathUserPhoto = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PathUserPhoto)));} 
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

        //public string AllMessage { 

        //    get { return this.Chat[].Text;  }

        //}

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Сравнение двух пользователей
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(TelegramUser other) => other.Id == this.id;

        /// <summary>
        /// Коллекция всех сообщений
        /// </summary>
        //public ObservableCollection<string> Messages { get; set; }

        [JsonProperty("chat")]
        public ObservableCollection<Message> Chat { get; set; }

        /// <summary>
        /// Добавление сообщения в коллекцию
        /// </summary>
        /// <param name="Text">Текс полученного сообщения</param>
        public void AddMessage(string Nick, string Text, DateTime dateTime)
        {
            Chat.Add(new Message(Nick, Text, dateTime));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Chat)));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.LastMessage)));
        }

    }
}

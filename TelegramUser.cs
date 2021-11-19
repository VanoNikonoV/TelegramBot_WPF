using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Newtonsoft.Json;


namespace TelegramBot_WPF
{
    /// <summary>
    /// Информация о пользователе Telegram
    /// </summary>
    public class TelegramUser : INotifyPropertyChanged, IEquatable<TelegramUser>
    {

        // Фото пользователя
        // var userPhoto = bot.GetUserProfilePhotosAsync(message.From.Id).Result;
        // var file = await bot.GetFileAsync(userPhoto.Photos[0][userPhoto.Photos.Length - 1].FileId);
        // DownLoad(userPhoto.Photos[0][userPhoto.Photos.Length - 1].FileId, "Иван");

        public TelegramUser(string Nickname, long ChatId)
        {
            this.nick = Nickname;
            this.id = ChatId;
            this.Chat = new ObservableCollection<Message>();
            //Messages = new ObservableCollection<string>();
            //this.Time = DateTime.Now;
        }

        private string nick;

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

        public long Id
        {
            get { return this.id; }
            set
            {
                this.id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Id)));
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

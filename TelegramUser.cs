using System;
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace TelegramBot_WPF
{
    /// <summary>
    /// Информация о пользователе Telegram
    /// </summary>
    public class TelegramUser : INotifyPropertyChanged, IEquatable<TelegramUser>
    {
        //public UserPhoto -  getUserProfilePhotos
        public TelegramUser(string Nickname, long ChatId)
        {
            this.nick = Nickname;
            this.id = ChatId;
            Messages = new ObservableCollection<string>();
            this.Time = DateTime.Now;
        }
        public TelegramUser()
        {
                
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

        public string LastMessage
        {
            get { 
                    int i = this.Messages.Count; 
                    --i;
                    return Messages[i]; 
                } 
        }
        public DateTime Time { get; }

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
        public ObservableCollection<string> Messages { get; set; }

        //public override string ToString()
        //{
        //    return $"{Messages} + \n + {Time}";
        //}

        /// <summary>
        /// Добавление сообщения в коллекцию
        /// </summary>
        /// <param name="Text">Текс полученного сообщения</param>
        public void AddMessage(string Text)
        { 
            Messages.Add(Text);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Messages)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.LastMessage)));
        }

    }
}

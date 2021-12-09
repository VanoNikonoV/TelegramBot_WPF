using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace TelegramBot_WPF
{
    public class Message : INotifyPropertyChanged
    {
        private string text;

        [JsonProperty("time")]
        public DateTime Time { get; set; }

        [JsonProperty("nick")]
        public string Nick { get; set; }

        [JsonProperty("text")]
        public string Text
        {
            get { return this.text; }
            set {
                this.text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Text)));
            }
        }

        public Message(string nick, string text, DateTime dateTime) =>
                    (this.Nick, this.Text, this.Time) = (nick, text, dateTime);

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

    


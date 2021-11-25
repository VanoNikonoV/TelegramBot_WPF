using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        
        public Message(string nick, string text, DateTime dateTime) 
        {
            this.Nick = nick;
            this.Text = text;
            this.Time = dateTime;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            string time = Time.ToShortTimeString();
            return $"{Text}\n{time}";
        }
    }
}

    


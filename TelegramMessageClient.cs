using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace TelegramBot_WPF
{
    class TelegramMessageClient
    {
        private MainWindow window;

        private TelegramBotClient bot;

        public ObservableCollection<TelegramUser> Users { get; set; }

        [Obsolete]
        public TelegramMessageClient(MainWindow W, string PathToken = @"token.txt")
        {
            this.Users = new ObservableCollection<TelegramUser>();

            this.window = W;

            bot = new TelegramBotClient(File.ReadAllText(PathToken));

            bot.OnMessage += MessageListener;

            bot.StartReceiving();
        }

        [Obsolete]
        private void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string text = $"{DateTime.Now.ToLongTimeString()}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";

            Debug.WriteLine(text);

            if (e.Message.Text == null) return;

            var messageText = e.Message.Text;

            window.Dispatcher.Invoke(() =>
            {
                var person = new TelegramUser(e.Message.Chat.FirstName, e.Message.Chat.Id);

                if (!Users.Contains(person)) Users.Add(person);
                {
                    Users[Users.IndexOf(person)].AddMessage($"{e.Message.Text}"); //{person.Nick}:
                }
            });
        }

        public void SendMessage(string Text, string Id, TelegramUser user)
        {
            long id = Convert.ToInt64(Id);

            string responseMsg = $"Support: {Text}";

            user.AddMessage(responseMsg);

            bot.SendTextMessageAsync(id, Text);

        }
    }
}

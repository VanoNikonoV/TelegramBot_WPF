using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telegram.Bot.Types.InputFiles;

namespace TelegramBot_WPF
{
    /// <summary>
    /// Логика взаимодействия для Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        private readonly string pathMsg = $"{Environment.CurrentDirectory}\\MessageLog.json";

        private readonly string pathFile = $"{Environment.CurrentDirectory}\\FileDownload.json";

        private MessageLogSaveAndLoade MessageLog;

        private FileSaveAndLoade FileDownload;

        TelegramMessageClient client;

        [Obsolete]
        public Home()
        {
            InitializeComponent();

            client = new TelegramMessageClient(this);

            //значения по умолчанию для ComboBox
            this.voiceComboBox.SelectedIndex = 0;
            this.speedComboBox.SelectedIndex = 0;
            this.formatComboBox.SelectedIndex = 0;
            this.emotionComboBox.SelectedIndex = 0;

            txtMsgSend.KeyDown += (s, e) => 

            { 
                if (e.Key == Key.Return) 
                {
                    var curUser = usersList.SelectedItem as TelegramUser; // если null вывести сообщение
                    if (curUser != null)
                    {
                        client.SendMessage(txtMsgSend.Text, TargetSend.Text, curUser);
                        MessageLog.SaveFile(client.Users);
                    }
                    else
                    {
                        MessageBox.Show("Выберите пользователя");
                    }
                    
                } 
            };
        }

        private void btnMsgSendClick(object sender, RoutedEventArgs e)
        {
            var curUser = usersList.SelectedItem as TelegramUser;

            client.SendMessage(txtMsgSend.Text, TargetSend.Text, curUser);

            txtMsgSend.Text = string.Empty;

            MessageLog.SaveFile(client.Users);
        }

        /// <summary>
        /// Загрузка данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MessageLog = new MessageLogSaveAndLoade(pathMsg);

            FileDownload = new FileSaveAndLoade(pathFile);

            try
            {
                client.Users = MessageLog.LoadFile();

                client.InfoFiles = FileDownload.LoadFile(); //TelegramMessageClient.InfoFiles = FileDownload.LoadFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }

            usersList.Items.Clear();

            usersList.ItemsSource = client.Users;

            
        }

        private void Close()
        {
            throw new NotImplementedException();
        }

        private void OpenCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var openDlg = new OpenFileDialog { Filter = "Text files|*.txt" };

            if (true == openDlg.ShowDialog())
            {
                client.Users = MessageLog.LoadFile(openDlg.FileName);
                //string dataFromFile = File.ReadAllText(openDlg.FileName);
            }
        }

        private void SaveCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var saveDlg = new SaveFileDialog { Filter = "Text files|*.json" };

            if (true == saveDlg.ShowDialog())
            {
                MessageLog.SaveFile(client.Users, saveDlg.FileName);

                //FileDownload.SaveFile(client.InfoFiles);
            }
        }

        private void download_Click(object sender, RoutedEventArgs e)
        {
            Dowload expenseReportPage = new Dowload (this.client.InfoFiles);
            this.NavigationService.Navigate(expenseReportPage);
        }

        private void ckick_MsgSendaAndVoice(object sender, RoutedEventArgs e)
        {
            string voice = (this.voiceComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string format = (this.formatComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string speed = string.Empty;
            string emotion = string.Empty;

            //если установлен параметр по умолчанию вернуть 0 иначе выбранное значени
            if (this.speedComboBox.SelectedIndex == 0)
            {
                speed = "0";
            }
            else
            {
                speed = (this.speedComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            }

            //если установлен параметр по умолчанию вернуть 0 иначе выбранное значени
            if (this.emotionComboBox.SelectedIndex == 0)
            {
                emotion = "neutral";
            }
            else
            {
                emotion = (this.emotionComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            }
             
            // формирование строки запроса
            StringBuilder data = new StringBuilder();

            data.AppendFormat("text={0}", txtMsgSend.Text);
            data.AppendFormat("&voice={0}", voice);
            data.AppendFormat("&format={0}", format);
            data.AppendFormat("&speed=1", speed);
            data.AppendFormat("&pitch=1&emotion={0}", emotion);

            (int id, int status) info = HttpClientVoice.Request(data.ToString());

            // строка запроса
            string id_voice = $"id={info.id}";

            if (info.status == 1)
            {
                (string path, string expansion) = HttpClientVoice.Request_2(id_voice);

                InputOnlineFile pathFile = new InputOnlineFile(path);

                string[] text = File.ReadAllLines(@"info2.txt");

                var curUser = usersList.SelectedItem as TelegramUser;
                
                client.SendMessage(txtMsgSend.Text, TargetSend.Text, curUser);

                client.SendVoice(pathFile, TargetSend.Text, expansion);
            }
            else
            {
                MessageBox.Show("Что то пошло не так!");
            }

        }

        private void txtMsgSend_MouseEnter(object sender, MouseEventArgs e)
         {
             txtMsgSend.Text = string.Empty;
         }

        private void txtMsgSend_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!txtMsgSend.IsFocused)
            {
                txtMsgSend.Text = "Напишите текс для отправки";
            }
           
        }

        private void VoiceChanged(object sender, SelectionChangedEventArgs e)
        {
            string voice = (this.voiceComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            // отпарить в запрос
        }

        private void FormatChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SpeedChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void EmotionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

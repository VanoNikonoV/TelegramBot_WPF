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

        
        private void txtMsgSend_MouseEnter(object sender, MouseEventArgs e)
        {
            //txtMsgSend.Text = string.Empty;
            txtMsgSend.Text = "Напишите текс для отправки";
        }

        private void ckick_MsgSendaAndVoice(object sender, RoutedEventArgs e)
        {
            (int id, int status) info = HttpClientVoice.Request(txtMsgSend.Text);

            string id_voice = $"id={info.id}";

            //if (info.status == 1)
            //{
            //    InputOnlineFile path = HttpClientVoice.Request_2(id_voice);

            //    string[] text = File.ReadAllLines(@"info2.txt");

            //    await client.bot.SendDocumentAsync(chatId: e.CallbackQuery.From.Id,
            //                             path,
            //                             caption: text[1]);//"Результат"
            //}
            //else
            //{
            //    await bot.SendTextMessageAsync(chatId: e.CallbackQuery.From.Id, text: "Что-то пошло не так");
            //}

        }

        //private void txtMsgSend_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    txtMsgSend.Text = "Напишите текс для отправки";
        //}
    }
}

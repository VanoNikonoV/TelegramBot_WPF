using System;
using System.IO;
using Microsoft.Win32;
using System.Collections.Generic;
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

#region Что нужно сделать
 
//В приложении необходимо реализовать отображение списка сообщений, которые написал боту пользователь. 
//В списке присутствуют как минимум имя пользователя и его сообщение. 
//При нажатии на сообщение оно становится выделенным.
//Помимо этого, приложение может отправлять выбранному пользователю ответ в виде текста. Для реализации
//этого потребуется использовать элементы управления Button и TextBox. 
//При каждом новом полученном сообщении приложение сохраняет его в истории сообщений.
//Потом её можно импортировать в файл формата JSON. 
//Также вы можете добавить в приложение меню, в котором можно совершить сохранение истории,
//выход из приложения, просмотр список присланных файлов в новом окне и так далее.
//При этом приложение не должно выглядеть некрасиво при растягивании на весь экран.
#endregion

namespace TelegramBot_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string pathMsg = $"{Environment.CurrentDirectory}\\MessageLog.txt";

        private readonly string pathFile = $"{Environment.CurrentDirectory}\\FileDownload.json";

        private MessageLogSaveAndLoade MessageLog;

        private FileSaveAndLoade FileDownload;

        TelegramMessageClient client;

        public Page1 p1 = new Page1();
        

        [Obsolete]
        public MainWindow()
        {
            InitializeComponent();

            client = new TelegramMessageClient(this);
        }

        /// <summary>
        /// Отправляет сообщение пользователю
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                Close();
            }

            usersList.Items.Clear();

            usersList.ItemsSource = client.Users;

        }

        private void OpenCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var openDlg = new OpenFileDialog { Filter = "Text files|*.txt" };

            if(true == openDlg.ShowDialog())
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
            Download.Content = p1;
            p1.DataContext = client.InfoFiles;
        }



        /// <summary>
        /// Выводи сообщение от пользователя на экран
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void user_Selected(object sender, SelectionChangedEventArgs e)
        //{
        //    TelegramUser c = (TelegramUser)usersList.SelectedItem;

        //    concreteUser.ItemsSource = c.Messages;
        //}
    }
}

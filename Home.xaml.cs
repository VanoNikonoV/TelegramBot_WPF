using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telegram.Bot.Types.InputFiles;


namespace TelegramBot_WPF
{
    /// <summary>
    /// Логика взаимодействия для Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        private readonly string pathMsgLog = TelegramMessageClient.pathMessageLog;

        private readonly string pathFile = TelegramMessageClient.folderFiles;

        private MessageLogSaveAndLoade MessageLog;
        
        Dowload downloadedPage = new Dowload();

        private TelegramMessageClient client;

        private static readonly HttpClient httpClient = new HttpClient();

        [Obsolete]
        public Home()
        {
            InitializeComponent();

            client = new TelegramMessageClient(this);

            //значения по умолчанию для Voice ComboBox 
            this.voiceComboBox.SelectedIndex = 0;
            this.speedComboBox.SelectedIndex = 0;
            this.emotionComboBox.SelectedIndex = 0;

            //Отправка сообщения по нажатию кнопки
            txtMsgSend.KeyDown += (s, e) => 
            { 
                if (e.Key == Key.Return) 
                {
                    var curUser = usersList.SelectedItem as TelegramUser;

                    if (curUser != null && !string.IsNullOrWhiteSpace(txtMsgSend.Text))
                    {
                        client.SendMessage(txtMsgSend.Text, TargetSend.Text, curUser);

                        MessageLog.SaveFile(client.Users);

                        txtMsgSend.Text = string.Empty;
                    }
                    else
                    {
                        MessageBox.Show("Выберите пользователя", caption: nameof(TelegramMessageClient));
                    }
                } 
            };
        }


        /// <summary>
        /// Загрузка данных при запуске программы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MessageLog = new MessageLogSaveAndLoade(pathMsgLog);

            try
            {
                client.Users = MessageLog.LoadFile();
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

        /// <summary>
        /// Показать чат с тользователем
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplayChat(object sender, SelectionChangedEventArgs e)
        {
            var User = usersList.SelectedItem as TelegramUser;

            concreteUser.ItemsSource = User.Chat;

            concreteUser.ScrollIntoView(concreteUser.Items[concreteUser.Items.Count - 1]);
        }

        /// <summary>
        /// Показать вкладку с файлами отправленными пользователем
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowDownloaded_Click(object sender, RoutedEventArgs e)
        {
            var curUser = usersList.SelectedItem as TelegramUser;

            this.downloadedPage.DataContext = curUser.InfoDowloadFiles;

            this.NavigationService.Navigate(downloadedPage);
        }

        /// <summary>
        /// Оправляет и записывает сообщение по нажатию кнопки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MsgSend_buttonClick(object sender, RoutedEventArgs e)
        {
            var curUser = usersList.SelectedItem as TelegramUser;

            if (curUser != null && !string.IsNullOrWhiteSpace(txtMsgSend.Text))
            {
                client.SendMessage(txtMsgSend.Text, TargetSend.Text, curUser);

                txtMsgSend.Text = string.Empty;
            }
            else
            {
                MessageBox.Show("Выберите пользователя", caption: nameof(TelegramMessageClient));
            }
        }

        /// <summary>
        /// Отправляет текст и его озвучку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MsgSendAndVoice_buttonClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtMsgSend.Text))
            {
                var curUser = usersList.SelectedItem as TelegramUser;

                client.SendMessage(txtMsgSend.Text, TargetSend.Text, curUser);

                // получаю значения из ComboBoxs
                string voice = (voiceComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
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

                #region Запрос первый

                // формирование строки запроса
                StringBuilder data = new StringBuilder();
                data.AppendFormat(@"https://zvukogram.com/index.php?r=api/");
                data.AppendFormat("longtext&token=7b79dbc5ed7d3f5f0a51f32fb7e6ca23&email=cmn.nia@gmail.com&&text={0}", txtMsgSend.Text);
                data.AppendFormat("&voice={0}", voice);
                data.AppendFormat("&format=mp3");
                data.AppendFormat("&speed={0}", speed);
                data.AppendFormat("&pitch=1&emotion={0}", emotion);

                (int id, string status_or_error) info = await Home.RequestOne(data.ToString());

                statBarText.Text = "Запрос отправлен!";

                // выводит ощибку
                if (info.id == 0) MessageBox.Show(messageBoxText: $"Статуса запроса {info.status_or_error}",
                                                    caption: "Запрос на озвучку",
                                                    MessageBoxButton.OK,
                                                    MessageBoxImage.Warning);
                #endregion

                #region Запрос второй

                data.Clear();
                data.AppendFormat(@"https://zvukogram.com/index.php?r=api/");
                data.Append("result&token=7b79dbc5ed7d3f5f0a51f32fb7e6ca23&email=cmn.nia@gmail.com&");
                data.AppendFormat("id={0}", info.id);

                string path = await Home.RequestTwo(data.ToString());

                #endregion

                InputOnlineFile pathFile = new InputOnlineFile(path);

                client.SendVoice(pathFile, TargetSend.Text);

                statBarText.Text = "Голосовое сообщение отправленно на сервер Telegram!";

            }
            else MessageBox.Show(messageBoxText: "Отсутсвует текст для озвучки",
                                caption: "Неверный запрос",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            
        }

        /// <summary>
        /// Деактивирует ComboBox Emotion при выборе Бот Татьяна - она не подерживает эмоции
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelecChanged_Voice(object sender, SelectionChangedEventArgs e)
        {
            string voice = (this.voiceComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (voice == "Бот Татьяна")
            {
                this.emotionComboBox.IsEnabled = false;
            }
            else this.emotionComboBox.IsEnabled = true;

        }

        /// <summary>
        /// Сохранить переписку с выбранным пользователем
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveCmdExecuted(sender, e as ExecutedRoutedEventArgs);
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
                var curUser = usersList.SelectedItem as TelegramUser;

                MessageLog.SaveMessageUser(curUser.Chat, saveDlg.FileName);
            }
        }

        #region MouseEnter and MouseLeave
        
        private void MouseLeave_SatusBar(object sender, MouseEventArgs e)
        {
            statBarText.Text = "Здесь будет подсказка";
        }

        private void txtMsgSend_MouseEnter(object sender, MouseEventArgs e)
             {
                if (txtMsgSend.Text == "Напишите текс для отправки") txtMsgSend.Text = string.Empty;   
             }

        private void txtMsgSend_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!txtMsgSend.IsFocused) txtMsgSend.Text = "Напишите текс для отправки";
 
        }

        private void MouseEnter_Download(object sender, MouseEventArgs e)
        {
            statBarText.Text = "Нажмите чтобы посмотреть загруженные фалы";
        }

        private void MouseEnter_Send(object sender, MouseEventArgs e)
        {
            statBarText.Text = "Нажмите кнопу или Enter чтобы отправить текст";
        }

        private void MouseEnter_SendaAndVoice(object sender, MouseEventArgs e)
        {
            statBarText.Text = "Нажмите чтобы отправить и озвучить текст";
        }

        private void MouseEnter_Save(object sender, MouseEventArgs e)
        {
            statBarText.Text = "Нажмите чтобы сохранить переписку для выбранного пользователя";
        }
        #endregion

        /// <summary>
        /// Отправляет запрос для постановки задачи на озвучку текста.
        /// </summary>
        /// <param name="request">Параметры запроса</param>
        /// <returns>"id" - уникальный идентификатор озвучки 
        /// В зависимость от полученного статуса один из вариантов:
        ///          "status" - текущий статус озвучки
        ///          "error" - информацию об ощибке
        ///     
        /// Доступны 3 значения status_or_error:
        ///     0  - в процессе
        ///     1  - завершен успешно
        ///     error  - ошибка запроса (при этом id = 0)</returns>
        static async Task<(int id, string path)> RequestOne(string request)
        {
            HttpResponseMessage response = (await httpClient.GetAsync(request)).EnsureSuccessStatusCode();

            using (var streamResponse = await response.Content.ReadAsStreamAsync())

            using (StreamReader sr = new StreamReader(streamResponse))
            {
                var result = sr.ReadToEnd();//ответ

                int id = Convert.ToInt32(JObject.Parse(result)["id"].ToString());

                string status = Convert.ToString(JObject.Parse(result)["status"]);

                string error = Convert.ToString(JObject.Parse(result)["error"]);

                if (status == "-1") return (0, error);

                else return (id, status);
            }

        }

        /// <summary>
        /// Отправляет запрос, чтобы узнать результат озвучки и путь к файлу
        /// </summary>
        /// <param name="request">Параметры запроса</param>
        /// <returns>
        /// path - путь к файлу
        /// </returns>
        static async Task<string> RequestTwo(string request)
        {
            bool flag = true;
            
            string path = string.Empty;

            while (flag) // иногда прихожит ответ на запрос longtex вместо result, поэтому делаю проверку path
            {
                HttpResponseMessage response = (await httpClient.GetAsync(request)).EnsureSuccessStatusCode();

                using (var streamResponse = await response.Content.ReadAsStreamAsync())

                using (StreamReader sr = new StreamReader(streamResponse))
                {
                        var result = sr.ReadToEnd();

                        int status = Convert.ToInt32(JObject.Parse(result)["status"]);

                        if ((int)status == 1) path = Convert.ToString(JObject.Parse(result)["file"]);
                        

                        if (status == 0) await Task.Delay(60000); 
                }

                if (path != string.Empty) flag = false;
            }

            return path;
        }
    }
}

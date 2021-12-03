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
using System.Diagnostics;
using System.Collections.ObjectModel;

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

        //private FileSaveAndLoade FileDownload;
        
        Dowload expenseReportPage = new Dowload();

        private TelegramMessageClient client;

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
                    if (curUser != null)
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
        /// Оправляет и записывает сообщение по нажатию кнопки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MsgSend_buttonClick(object sender, RoutedEventArgs e)
        {
            var curUser = usersList.SelectedItem as TelegramUser;

            if (curUser != null)
            {
                client.SendMessage(txtMsgSend.Text, TargetSend.Text, curUser);

                txtMsgSend.Text = string.Empty;

                MessageLog.SaveFile(client.Users);
            }
            else
            {
                MessageBox.Show("Выберите пользователя", caption: nameof(TelegramMessageClient));
            }
        }

        /// <summary>
        /// Загрузка данных при запуске программы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MessageLog = new MessageLogSaveAndLoade(pathMsgLog);

            //FileDownload = new FileSaveAndLoade(); // pathFile + ID 

            try
            {
                client.Users = MessageLog.LoadFile();

                //foreach (var user in client.Users)
                //{
                //    string path = pathFile +  user.Id.ToString() + ".json";

                //    if (File.Exists(path))
                //    {
                //        user.InfoDowloadFiles = FileDownload.LoadFile(path);
                //    } 
                //}

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

        private void download_Click(object sender, RoutedEventArgs e)
        {
            var curUser = usersList.SelectedItem as TelegramUser;

            this.expenseReportPage.DataContext = curUser.InfoDowloadFiles;

            //Dowload expenseReportPage = new Dowload (this.client.InfoFiles); // передать infoFiles конктреного пользователя
            this.NavigationService.Navigate(expenseReportPage);
        }

        private void MsgSendaAndVoice_buttonClick(object sender, RoutedEventArgs e)
        {
            if (txtMsgSend.Text != string.Empty)
            {
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

                // формирование строки запроса
                StringBuilder data = new StringBuilder();

                data.AppendFormat(@"https://zvukogram.com/index.php?r=api/");
                data.AppendFormat("longtext&token=7b79dbc5ed7d3f5f0a51f32fb7e6ca23&email=cmn.nia@gmail.com&&text={0}", txtMsgSend.Text);
                data.AppendFormat("&voice={0}", voice);
                data.AppendFormat("&format=mp3");
                data.AppendFormat("&speed={0}", speed);
                data.AppendFormat("&pitch=1&emotion={0}", emotion);

                (int id, string status_or_error) info = VoiceOver.ChallengeRequestOne(data.ToString());

                // выводит ощибку
                if (info.id == 0) MessageBox.Show(messageBoxText: $"Статуса запроса {info.status_or_error}", 
                                                    caption: "Запрос на озвучку", 
                                                    MessageBoxButton.OK, 
                                                    MessageBoxImage.Warning);

                //if (info.id != 0) 
                //{
                //    Uri path_file = new Uri(info.status_or_error);

                //    InputOnlineFile pathFile = new InputOnlineFile(path_file);

                //    //InputOnlineFile pathFile = new InputOnlineFile(info.status_or_error);

                //    var curUser = usersList.SelectedItem as TelegramUser;

                //    client.SendMessage(txtMsgSend.Text, TargetSend.Text, curUser);

                //    MessageLog.SaveFile(client.Users);

                //    client.SendVoice(pathFile, TargetSend.Text);

                //    Debug.WriteLine("status = 1");
                //}

                // Запрос в процессе status = 0 вторая строка запрос
                if (info.status_or_error == "1" || info.status_or_error == "0")
                {
                    this.statBarText.Text = $"ID файла {info.id}, статус {info.status_or_error}";

                    data.Clear();

                    data.AppendFormat(@"https://zvukogram.com/index.php?r=api/");
                    data.Append("result&token=7b79dbc5ed7d3f5f0a51f32fb7e6ca23&email=cmn.nia@gmail.com&");
                    data.AppendFormat("id={0}", info.id);

                    (string path, string expansion) info2 = (string.Empty, string.Empty);

                    while (info2.path == null || info2.path == string.Empty)
                    {
                        info2 = VoiceOver.ChallengeRequestTwo(data.ToString());
                    }

                    var curUser = usersList.SelectedItem as TelegramUser;

                    InputOnlineFile pathFile = new InputOnlineFile(info2.path);

                    client.SendMessage(txtMsgSend.Text, TargetSend.Text, curUser);

                    MessageLog.SaveFile(client.Users);

                    client.SendVoice(pathFile, TargetSend.Text);
                }
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

                //FileDownload.SaveFile(client.InfoFiles);
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
            if (!txtMsgSend.IsFocused)
            {
                txtMsgSend.Text = "Напишите текс для отправки";
            }
           
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
    }
}

﻿using System;
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
        TelegramMessageClient client;

        [Obsolete]
        public MainWindow()
        {
            InitializeComponent();

            client = new TelegramMessageClient(this);

            usersList.ItemsSource = client.Users;
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
        }

        /// <summary>
        /// Выводи сообщение от пользователя на экран
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void user_Selected(object sender, SelectionChangedEventArgs e)
        //{
        //    TelegramUser c = (TelegramUser)usersList.SelectedItem;

        //    concreteUser_Nick.Text = c.Nick;

        //    concreteUser_Message.Text = c.Messages[c.IndexLastMessege];
        //}
    }
}

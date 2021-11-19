using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using Telegram.Bot.Types.InputFiles;
using System.Diagnostics;

namespace TelegramBot_WPF
{
    /// <summary>
    /// Отправляет и получает данные на сайт https://zvukogram.com
    /// </summary>
    public class HttpClientVoice
    {
        const string basUrl_1 = @"https://zvukogram.com/index.php?r=api/longtext";

        const string basUrl_2 = @"https://zvukogram.com/index.php?r=api/result";

        const string token = @"&token=7b79dbc5ed7d3f5f0a51f32fb7e6ca23&email=cmn.nia@gmail.com&";

        #region help
        //'format' - формат результирующего файла, по умолчанию = mp3, доступные значения ( 'mp3', 'wav', 'ogg')
        //'speed' - скорость воспроизведения, по умолчанию 1, (диапазон от 0.1 до 2.0) 
        //'pitch'- высота голоса, по умолчанию 0, (диапазон от -20 до 20)    
        //'emotion' - эмоциональный окрас голоса, по умолчанию  'good', доступные значения( 'good',  'evil', 'neutral'). 

        //private const string url = @"https://zvukogram.com/index.php?r=api/longtext";

        //private const string result = @"https://zvukogram.com/index.php?r=api/result";

        //private const string token = "token=7b79dbc5ed7d3f5f0a51f32fb7e6ca23&email=cmn.nia@gmail.com&voice=Владимир&text=";

        //string text = $"Текст который будет озвучен";
        //string format = "&format=ogg";
        //string speed = "&speed=1";
        //string pitch = "&pitch=0.8";
        //string emotion = "&emotion=good]";

        //string data = @"token=7b79dbc5ed7d3f5f0a51f32fb7e6ca23&email=cmn.nia@gmail.com&voice=Владимир&text=""Текст который будет озвучен""&format=mp3&speed=0.5&pitch=0.8&emotion=evil]";

        //string data = @"https://zvukogram.com/index.php?r=api/longtext&token=7b79dbc5ed7d3f5f0a51f32fb7e6ca23&email=cmn.nia@gmail.com&text=""Текст который будет озвучен""&voice=Владимир&format=ogg&speed=1&pitch=0.8&emotion=good";
        #endregion

        /// <summary>
        /// Отправляет данные на сайт для постановка задачи на озвучку текста.
        /// </summary>
        /// <param name="data">Режим озвучки</param>
        /// <returns>"id" - уникальный идентификатор озвучки, 
        /// "status" - текущий статус озвучки. Доступны 3 значения:
        ///     0  - в процессе
        ///     1  - завершен успешно
        ///     -1 - ошибка </returns>
        public static (int, int) Request(string data)
        {
            CookieContainer cookies = new CookieContainer();

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(basUrl_1);

            req.Method = "POST";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.CookieContainer = cookies;
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:17.0) Gecko/20100101 Firefox/17.0";
            req.ContentType = "application/x-www-form-urlencoded;";

            using (var requestStream = req.GetRequestStream())

            using (StreamWriter sw = new StreamWriter(requestStream))
            {
                sw.Write(token + data);
            }

            using (var responseStream = req.GetResponse().GetResponseStream())

            using (StreamReader sr = new StreamReader(responseStream))
            {
                var result = sr.ReadToEnd();//ответ

                int id = Convert.ToInt32(JObject.Parse(result)["id"].ToString());
                int status =Convert.ToInt32(JObject.Parse(result)["status"]);

                return (id, status);
            }
        }

        /// <summary>
        /// Отправляет запрос, чтобы узнать результат
        /// </summary>
        /// <param name="data">Если результать обработки удачный возвращает InputOnlineFile</param>
        /// <returns>Путь к файлу для скачивания</returns>
        public static (string path, string format) Request_2(string data)
        {
            CookieContainer cookies = new CookieContainer();

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(basUrl_2);

            req.Method = "POST";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.CookieContainer = cookies;
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:17.0) Gecko/20100101 Firefox/17.0";
            req.ContentType = "application/x-www-form-urlencoded;";

            using (var requestStream = req.GetRequestStream())
            using (var sw = new StreamWriter(requestStream))
            {
                sw.Write(token + data);
            }

            string path = string.Empty;
            string format = string.Empty;

            using (var responseStream = req.GetResponse().GetResponseStream())

            using (StreamReader sr = new StreamReader(responseStream))
            {
                var result = sr.ReadToEnd();//ответ

                var id = JObject.Parse(result)["id"];

                var status = JObject.Parse(result)["status"];

                if ((int)status == 0)
                {
                    Thread.Sleep(1000);
                    Request_2(data);
                    Debug.WriteLine($"Статус {status}");
                }
                if ((int)status == 1)
                {
                    path = Convert.ToString(JObject.Parse(result)["file"]);

                    format = Convert.ToString(JObject.Parse(result)["format"]);
                }
            }
            return (path, format);
        }

    }

}

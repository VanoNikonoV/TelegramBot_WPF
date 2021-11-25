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
using System.Net.Http;

namespace TelegramBot_WPF
{
    /// <summary>
    /// Озвучка текста онлайн реалистичными голосами https://zvukogram.com/index.php?r=api/
    /// </summary>
    public static class VoiceOver
    {
        #region help
        //'format' - формат результирующего файла, по умолчанию = mp3, доступные значения ( 'mp3', 'wav', 'ogg')
        //'speed' - скорость воспроизведения, по умолчанию 1, (диапазон от 0.1 до 2.0) 
        //'pitch'- высота голоса, по умолчанию 0, (диапазон от -20 до 20)    
        //'emotion' - эмоциональный окрас голоса, по умолчанию  'good', доступные значения( 'good',  'evil', 'neutral'). 

        //https://zvukogram.com/index.php?r=api/longtext - Данный метод не имеет ограничений и позволяет озвучивать
        //текста до 1000 000 символов. Озвучка происходит в порядке очереди (первый пришел, первый обработан) и может
        //занимать от 1 до нескольких минут в зависимости от длинны текста.

        //После создания задачи и получения идентификатора (id), необходим отправить запроса
        //на https://zvukogram.com/index.php?r=api/result, чтобы узнать результат.
        #endregion

        private static readonly HttpClient client = new HttpClient();

        //public VoiceOver(string baseAdress)
        //{
        //    client.BaseAddress = new Uri(baseAdress);
        //}
        public static (int id, string status) ChallengeRequestOne(string request)
        {
            Task<(int id, string status)> info = Task.Run(() => RequestOne(request));

            (int id, string status) temp = info.Result;

            return (temp.id, temp.status);
        }

        public static (string path, string format) ChallengeRequestTwo(string request)
        {
            Task<(string path, string format)> info = Task.Run(() => RequestTwo(request));

            (string path, string format) temp = info.Result;

            return (temp.path, temp.format);
        }

        /// <summary>
        /// Отправляет запрос для постановка задачи на озвучку текста.
        /// </summary>
        /// <param name="request">Параметры запроса</param>
        /// <returns>"id" - уникальный идентификатор озвучки, 
        ///          "status" - текущий статус озвучки. 
        ///     Доступны 3 значения:
        ///     0  - в процессе
        ///     1  - завершен успешно
        ///    -1  - ошибка </returns>
        static async Task<(int id, string status)> RequestOne(string request)
        {
                HttpResponseMessage response = (await client.GetAsync(request)).EnsureSuccessStatusCode();

                using (var streamResponse = await response.Content.ReadAsStreamAsync())

                using (StreamReader sr = new StreamReader(streamResponse))
                {
                    var result = sr.ReadToEnd();//ответ

                    int id = Convert.ToInt32(JObject.Parse(result)["id"].ToString());

                    string status = Convert.ToString(JObject.Parse(result)["status"]);

                    string error = Convert.ToString(JObject.Parse(result)["error"]);

                    if (status == "-1")
                    {
                        return (0, error);
                    }
                    return (id, status);
                }
            
        }

        /// <summary>
        /// Отправляет запрос, чтобы узнать результат озвучки и путь к файлу
        /// </summary>
        /// <param name="request">Параметры запроса</param>
        /// <returns>
        /// path - путь к файлу
        /// format - формат файла
        /// </returns>
        static async Task<(string path, string format)> RequestTwo(string request)
        {
            HttpResponseMessage response = (await client.GetAsync(request)).EnsureSuccessStatusCode();

            string path = string.Empty;
            string format = string.Empty;

            using (var streamResponse = await response.Content.ReadAsStreamAsync())

            using (StreamReader sr = new StreamReader(streamResponse))
            {
                var result = sr.ReadToEnd();

                int status = Convert.ToInt32(JObject.Parse(result)["status"]);

                if (status == 0)
                {
                    Thread.Sleep(6000);
                    await RequestTwo(request);
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

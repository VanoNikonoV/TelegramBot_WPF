using Newtonsoft.Json;

namespace TelegramBot_WPF
{
    /// <summary>
    /// Информация о скаченных файлах
    /// </summary>
    public class InfoFiles
    {
        /// <summary>
        /// Номер записи
        /// </summary>
        [JsonProperty("number")]
        public int Number { get; set; }
        /// <summary>
        /// ID скаченого файла
        /// </summary>
        [JsonProperty("fileId")]
        public string FileId { get; }
        /// <summary>
        /// Имя скаченого файла
        /// </summary>
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        public InfoFiles(string fileId, string fileName, int count) => 
                        (this.FileId,   this.FileName,  this.Number) = (fileId, fileName, count);


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int Number { get; set; }
        /// <summary>
        /// ID скаченого файла
        /// </summary>
        public string FileId { get; }
        /// <summary>
        /// Имя скаченого файла
        /// </summary>
        public string FileName { get; set; }

        public InfoFiles(string fileId, string fileName, int count) => 
                        (this.FileId,   this.FileName,  this.Number) = (fileId, fileName, count);


    }
}

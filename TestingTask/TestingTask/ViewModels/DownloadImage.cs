using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TestingTask.ViewModels
{
    class DownloadImage
    {
        public ImageSource Image { get; set; }
        public string Url { get; set; }
        public double Progress { get; set; }
        public string Status { get; set; }

        public ICommand DownloadCommand { get; }
        public ICommand PauseCommand { get; }

        private CancellationTokenSource canselTokenSource;


        public DownloadImage()
        {
            
        }
        public async Task Download()
        {
            Status = "Загрузка...";
            try
            {
                ImageSource result =  // Начало загрузки...
                Image = result;
                Status = "Готово";
            }
            catch (OperationCanceledException)
            {
                Status = "Отменено";
            }
            catch(Exception ex)
            {
                Status = "Ошибка: " + ex.Message;
            }
        }
    }
}

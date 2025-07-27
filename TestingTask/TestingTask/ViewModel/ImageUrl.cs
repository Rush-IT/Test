using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TestingTask.Model;

namespace TestingTask.ViewModel
{
    public class ImageUrl
    {
        private string url = "";
        public string Url
        {
            get => url;
            set { url = value; }
        }
        //Загруженное изобаржение
        private ImageSource? image;
        public ImageSource? Image { get => image; set { image = value; OnPropertyChanged(nameof(Image)); } }

        //Статус загрузки
        private string status = "Ожидание";
        public string Status { get => status; set { status = value; OnPropertyChanged(nameof(Status)); } }

        //Прогресс загрузки
        private double progress;
        public double Progress { get => progress; set { progress = value; OnPropertyChanged(nameof(Progress)); } }

        //Загружается ли в данный момент изобажение
        public bool IsDownloading { get; set; }


        private CancellationTokenSource? cts;
        private readonly DownloadImageService downloadService;

        public ImageUrl()
        {
            downloadService = new DownloadImageService();
        }

        //Начало загрузки
        public async Task StartDownload()
        {
            Status = "Загрузка...";
            IsDownloading = true;
            Progress = 0;
            Image = null;
            cts = new CancellationTokenSource();
            try
            {
                var img = await downloadService.DownloadImageAsync(Url,
                    (p) => Progress = p, cts.Token);

                Image = img;
                Status = "Готово";
            }
            catch (OperationCanceledException)
            {
                Status = "Остановлено";
            }
            catch (Exception ex)
            {
                Status = "Ошибка: " + ex.Message;
            }
            finally
            {
                IsDownloading = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

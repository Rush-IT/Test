using System.ComponentModel;
using System.Windows.Media;
using TestingTask.Model;

namespace TestingTask.ViewModel
{
    public class ImageUrl : INotifyPropertyChanged
    {
        //Команды начала и отмены загрузки
        public RelayCommand StartCommand { get; }
        public RelayCommand CancelCommand { get; }

        private string url = "";
        public string Url
        {
            get => url;
            set { url = value; OnPropertyChanged(nameof(Url)); (StartCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
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
            StartCommand = new RelayCommand(async _ => await StartDownload(), _ => !IsDownloading && !string.IsNullOrWhiteSpace(Url));
            CancelCommand = new RelayCommand(_ => CancelDownload(), _ => IsDownloading);

        }

        //Начало загрузки
        public async Task StartDownload()
        {
            Status = "Загрузка...";
            IsDownloading = true;
            Progress = 0;
            Image = null;
            (StartCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
            cts = new CancellationTokenSource();
            try
            {
                var img = await downloadService.DownloadImageAsync(Url, progress => Progress = progress, cts.Token);

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
                (StartCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
            
        }
        public void CancelDownload()
        {
            cts?.Cancel();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestingTask.Model;

namespace TestingTask.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ImageUrl> Downloads { get; }
        public RelayCommand StartAllCommand { get; }
        private double overallProgress;
        private string overallStatus = "Ожидание";

        public double OverallProgress
        {
            get => overallProgress;
            set { overallProgress = value; OnPropertyChanged(nameof(OverallProgress)); }
        }
        public string OverallStatus
        {
            get => overallStatus;
            set { overallStatus = value; OnPropertyChanged(nameof(OverallStatus)); }
        }

        public MainViewModel()
        {
            Downloads = new ObservableCollection<ImageUrl>
        {
            new ImageUrl(),
            new ImageUrl(),
            new ImageUrl(),
        };

            foreach (var item in Downloads)
            {
                item.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(item.Progress) || e.PropertyName == nameof(item.Status))
                        UpdateOverallStatus();
                };
            }
            StartAllCommand = new RelayCommand(_ => StartAll(), _ => Downloads.Any(d => !d.IsDownloading && !string.IsNullOrWhiteSpace(d.Url)));
        }

        private void UpdateOverallStatus()
        {
            OverallProgress = Downloads.Average(d => d.Progress);
            int active = Downloads.Count(d => d.IsDownloading);
            if (active == 0) OverallStatus = "Ожидание";
            else OverallStatus = $"Загружается: {active}";
        }

        private void StartAll()
        {
            foreach (var d in Downloads)
            {
                if (!d.IsDownloading && !string.IsNullOrWhiteSpace(d.Url))
                    _ = d.StartDownload();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

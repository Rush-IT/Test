using System.IO;
using System.Net.Http;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TestingTask.Model
{
    class DownloadImageService
    {
        public async Task<ImageSource> DownloadImageAsync(string url, Action<double> reportProgress, CancellationToken token)
        {
            //HTTP запрос
            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);

            //Проверка ответа
            response.EnsureSuccessStatusCode();

            //Чтение потока
            var total = response.Content.Headers.ContentLength ?? -1L;
            using var stream = await response.Content.ReadAsStreamAsync(token);

            var ms = new MemoryStream();
            var buffer = new byte[8192];
            long read = 0;
            int n;
            while ((n = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), token)) > 0)
            {
                ms.Write(buffer, 0, n);
                read += n;

                //Обновление прогресса (Если null то прогресс не обновляется)
                reportProgress?.Invoke(total > 0 ? read * 100.0 / total : 0);
            }
            ms.Seek(0, SeekOrigin.Begin);

            //Создание изображения
            var decoder = BitmapDecoder.Create(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            var image = new WriteableBitmap(decoder.Frames[0]);

            //Заморозка для безопасного извлечения
            image.Freeze();

            return image;

            // Отмена загрузки происходит через токен в ViewModel
        }
    }
}

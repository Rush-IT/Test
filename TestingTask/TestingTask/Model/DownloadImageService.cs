using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
            using var responce = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);

            //Проверка ответа
            responce.EnsureSuccessStatusCode();

            //Чтение потока
            var total = responce.Content.Headers.ContentLength ?? -1L;
            using var stream = await responce.Content.ReadAsStreamAsync(token);

            var memStream = new MemoryStream();
            var buffer = new byte[16384];
            long read = 0;
            int n;
            while ((n = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), token)) > 0)
            {
                memStream.Write(buffer, 0, n);
                read += n;

                //Обновление прогресса (Если null то прогресс не обновляется)
                reportProgress?.Invoke(total > 0 ? read * 100.0 / total : 0);
            }
            memStream.Seek(0, SeekOrigin.Begin);

            //Создание изображения
            var decoder = BitmapDecoder.Create(memStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            var image = new WriteableBitmap(decoder.Frames[0]);

            //Заморозка для безопасного извлечения
            image.Freeze();

            return image;

            // Отмена загрузки происходит через токен в ViewModel
        }
    }
}

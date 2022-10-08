using MvcStartApp.Models.Db;

namespace MvcStartApp.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly ILogRepository _repo;

        /// <summary>
        ///  Middleware-компонент должен иметь конструктор, принимающий RequestDelegate
        /// </summary>
        public LoggingMiddleware(RequestDelegate next)  //, LogRepository repo
        {
            _next = next;
         //   _repo = repo;
        }

        private async Task LogDb(HttpContext context, ILogRepository _repo)
        {
            Request newRequest = new Request();
            newRequest.Url = context.Request.Host.Value + context.Request.Path;
            await _repo.AddRequest(newRequest);
        }

        private void LogConsole(HttpContext context)
        {
            // Для логирования данных о запросе используем свойста объекта HttpContext
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now}]: New request to http://{context.Request.Host.Value + context.Request.Path}");
        }

        private async Task LogFile(HttpContext context)
        {
            // Строка для публикации в лог
            string logMessage = $"[{DateTime.Now}]: New request to http://{context.Request.Host.Value + context.Request.Path}{Environment.NewLine}";

            // Путь до лога (опять-таки, используем свойства IWebHostEnvironment)
            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "RequestLog.txt");

            // Используем асинхронную запись в файл
            await File.AppendAllTextAsync(logFilePath, logMessage);
        }

        /// <summary>
        ///  Необходимо реализовать метод Invoke  или InvokeAsync
        /// </summary>
        public async Task InvokeAsync(HttpContext context, ILogRepository repo)
        {
            LogConsole(context);
            await LogFile(context);
            await LogDb(context, repo);

            // Передача запроса далее по конвейеру
            await _next.Invoke(context);
        }
    }
}

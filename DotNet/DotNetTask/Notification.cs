using System;

namespace DotNetTask
{
    public class Notification
    {
        /// <summary>
        /// Идентификатор встречи, к которой устанавливается напоминание.
        /// </summary>
        public string MeetingId { get; }

        /// <summary>
        /// Таймер.
        /// </summary>
        private NotificationTimer _timer;

        /// <summary>
        /// Создание напоминания.
        /// </summary>
        /// <param name="meetingId"></param>
        /// <param name="notifyAt"></param>
        /// <param name="message"></param>
        public Notification(string meetingId, DateTime notifyAt, string message)
        {
            this.MeetingId = meetingId;

            SetupTimer(notifyAt, message);
        }
        
        /// <summary>
        /// Устанавилвает новое время напоминания.
        /// </summary>
        /// <param name="notifyAt">Новое время напоминания.</param>
        /// <param name="message">Сообщение.</param>
        public void ResetNotification(DateTime notifyAt, string message)
        {
            this.SetupTimer(notifyAt, message);
        }

        /// <summary>
        /// Установка таймера.
        /// </summary>
        /// <param name="notifyAt">Новое время напоминания.</param>
        /// <param name="message">Сообщение.</param>
        private void SetupTimer(DateTime notifyAt, string message)
        {
            var timespan = (notifyAt - DateTime.Now).TotalMilliseconds;
            if (timespan <= 0)
            {
                throw new Exception("Время напоминания не может быть позже текущего времени.");
            }

            _timer = new NotificationTimer
            {
                Interval = timespan,
                AutoReset = false,
                Message = message
            };
            _timer.Elapsed += Notify;
            _timer.Start();
        }

        /// <summary>
        /// Отмена напоминания.
        /// </summary>
        public void StopNotification()
        {
            _timer.Stop();
            _timer.Dispose();
        }

        /// <summary>
        /// Вывод напоминания.
        /// </summary>
        private void Notify(object timer, System.Timers.ElapsedEventArgs e)
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"\nНапоминание:\n{((NotificationTimer)timer).Message}\n");
            Console.BackgroundColor = ConsoleColor.Black;

            StopNotification();
        }
    }
}

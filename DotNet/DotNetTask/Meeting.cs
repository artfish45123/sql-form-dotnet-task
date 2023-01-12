using System;

namespace DotNetTask
{
    public class Meeting
    {
        /// <summary>
        /// Идентификатор встречи.
        /// </summary>
        public string MeetingId { get; private set; }

        /// <summary>
        /// Название встречи.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Время начала.
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// Время окончания.
        /// </summary>
        public DateTime EndTime { get; private set; }

        /// <summary>
        /// Время напоминания.
        /// </summary>
        public DateTime? NotifyAt { get; private set; }

        /// <summary>
        /// Создание встречи.
        /// </summary>
        /// <param name="name">Название встречи.</param>
        /// <param name="startTime">Время начала.</param>
        /// <param name="endTime">Время окончания.</param>
        /// <param name="notifyBeforeSeconds">За сколько секунд до начала встречи установить напоминание.</param>
        public Meeting(string name, DateTime startTime, DateTime endTime, uint? notifyBeforeSeconds = null)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Не указано название встречи.");
            }

            if (startTime < DateTime.Now)
            {
                throw new ArgumentException("Время начала встречи может быть только в будушем.");
            }

            if (endTime < startTime)
            {
                throw new ArgumentException("Время окончания встречи должно быть позже времени начала.");
            }

            this.MeetingId = Guid.NewGuid().ToString();
            this.Name = name;
            this.StartTime = startTime;
            this.EndTime = endTime;

            SetNewNotificationTime(notifyBeforeSeconds);
        }

        /// <summary>
        /// Выводит данные о встрече в текстовом формате.
        /// </summary>
        public override string ToString()
        {
            return $@"Название: {this.Name}
Время начала: {this.StartTime}
Время окончания: {this.EndTime}
Время напоминания: {(this.NotifyAt.HasValue ? this.NotifyAt.Value.ToString() : "Не установлено")}";
        }

        /// <summary>
        /// Устанавливает новое время напоминания.
        /// </summary>
        /// <param name="notifyBeforeSeconds">За сколько секунд до начала встречи установить напоминание.</param>
        public void SetNewNotificationTime(uint? notifyBeforeSeconds)
        {
            this.NotifyAt = notifyBeforeSeconds == null
                ? this.NotifyAt = null
                : this.StartTime.AddSeconds(-notifyBeforeSeconds.Value);
        }
    }
}

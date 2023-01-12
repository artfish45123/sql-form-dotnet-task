using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DotNetTask
{
    public static class CommonHelper
    {
        /// <summary>
        /// Диалог создания встречи.
        /// </summary>
        /// <returns></returns>
        public static Meeting GetNewMeetingInfoViaUserInput()
        {
            Console.WriteLine("Введите название встречи:");
            string name = Console.ReadLine().Trim();

            if (String.IsNullOrEmpty(name))
            {
                throw new Exception("Название встречи не может быть пустым.");
            }

            Console.WriteLine("Введите время начала встречи, например: 20.01.2023 10:00:");
            string startTimeString = Console.ReadLine().Trim();
            DateTime startTime = TryGetParsedTimeOrThrowException(startTimeString);

            Console.WriteLine("Введите время окончания встречи, например: 20.01.2023 11:00:");
            string endTimeString = Console.ReadLine().Trim();
            DateTime endTime = TryGetParsedTimeOrThrowException(endTimeString);

            Console.WriteLine("За сколько минут до начала напомнить о встрече?\nОставьте пустым, если не нужно напоминать:");
            string notifyBeforeMinutesString = Console.ReadLine().Trim();
            uint? notifyBeforeMinutes;
            if (String.IsNullOrEmpty(notifyBeforeMinutesString))
            {
                notifyBeforeMinutes = null;
            }
            else
            {
                notifyBeforeMinutes = UInt32.Parse(notifyBeforeMinutesString);
            }

            return new Meeting(name, startTime, endTime, notifyBeforeMinutes*60);
        }

        /// <summary>
        /// Проверяет, пересекается ли встреча по времени с остальными.
        /// </summary>
        /// <param name="newMeeting">Новая встреча.</param>
        /// <param name="existingMeetings">Список существующих встреч.</param>
        /// <returns></returns>
        public static bool IsMeetingIntersectsOthers(Meeting newMeeting, List<Meeting> existingMeetings)
        {
            return existingMeetings.Any(
                 item => newMeeting.StartTime < item.EndTime
                 && item.StartTime < newMeeting.EndTime);
        }

        /// <summary>
        /// Создает напоминание из объекта встречи.
        /// </summary>
        /// <param name="meeting">Объект встречи.</param>
        /// <returns></returns>
        public static Notification CreateNotificationFromMeeting(Meeting meeting)
        {
            if (meeting.NotifyAt == null)
            {
                return null;
            }

            string message = $"Встреча \"{meeting.Name}\" с {meeting.StartTime} до {meeting.EndTime}.";

            var notification = new Notification(meeting.MeetingId, meeting.NotifyAt.Value, message);

            return notification;
        }

        /// <summary>
        /// Выводит список встреч в текстовом формате.
        /// </summary>
        /// <param name="meetings">Список встреч.</param>
        /// <param name="numerate">Если true - встречи будут пронумерованы.</param>
        public static string GetMeetingsInText(List<Meeting> meetings, bool numerate = false)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < meetings.Count; i++)
            {
                stringBuilder.Append($"{(numerate ? $"({i + 1}) " : "")}{meetings[i].ToString()}\n\n");
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Экспортирует данные о встречах в файл.
        /// </summary>
        /// <param name="meetings">Список встреч.</param>
        /// <param name="fileName">Имя файла.</param>
        public static void ExportMeetingsToFile(List<Meeting> meetings, string fileName)
        {
            var meetingsToOutput = meetings.OrderBy(item => item.StartTime).ToList();

            var meetingsText = GetMeetingsInText(meetingsToOutput);

            using (StreamWriter outputFile = new StreamWriter(fileName))
            {
                outputFile.Write(meetingsText);
            }
        }

        private static DateTime TryGetParsedTimeOrThrowException(string startTimeString)
        {
            DateTime time;
            bool parsedTime = DateTime.TryParse(startTimeString, out time);
            if (!parsedTime)
            {
                throw new Exception($"Не получилось разспознать время: {startTimeString}");
            }

            return time;
        }
    }
}

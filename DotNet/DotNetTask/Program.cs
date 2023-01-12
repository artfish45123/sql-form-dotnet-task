using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetTask
{
    class Program
    {
        private const string OutputFileName = "MeetingList.txt";

        private static List<Meeting> _meetingList = new List<Meeting>();
        private static List<Notification> _notificationList = new List<Notification>();

        static void Main(string[] args)
        {
            Console.WriteLine("Загрузить тестовые данные о встречах?");
            Console.WriteLine("y - да, любая другая клавиша - нет.");

            var userInput = ReadKeyFromUserInput();
            if (userInput == 'y')
            {
                (_meetingList, _notificationList) = TestDataHelper.GenerateTestMeetingsAndNotifications();
                Console.WriteLine("Тестовые встречи загружены.");
            }

            RenderMainMenu();
            userInput = ReadKeyFromUserInput();

            while (userInput != UserMenuKeyConstants.Quit)
            {
                try
                {
                    switch (userInput)
                    {
                        case UserMenuKeyConstants.ShowMeetings:
                            ShowMeetings();
                            break;

                        case UserMenuKeyConstants.CreateMeeting:
                            CreateNewMeeting();
                            break;

                        case UserMenuKeyConstants.ChangeNotificationTime:
                            ChangeNotificationTime();
                            break;

                        case UserMenuKeyConstants.ExportToFile:
                            ExportToFile();
                            break;

                        default:
                            Console.WriteLine("Команда не распознана, пожалуйста, повторите ввод");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Произошла ошибка: {0}\nПожалуйста, попробуйте еще раз.", e.Message);
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                finally
                {
                    RenderMainMenu();
                    userInput = ReadKeyFromUserInput();
                }
            }            
        }

        private static void RenderMainMenu()
        {
            Console.WriteLine("\nВыберите пункт меню или нажмите q для выхода:");
            Console.WriteLine($"{UserMenuKeyConstants.ShowMeetings} - показать сохраненные встречи");
            Console.WriteLine($"{UserMenuKeyConstants.CreateMeeting} - создать встречу");
            Console.WriteLine($"{UserMenuKeyConstants.ChangeNotificationTime} - изменить время напоминания для встречи");
            Console.WriteLine($"{UserMenuKeyConstants.ExportToFile} - экспортировать встречи в файл\n");
        }

        private static char ReadKeyFromUserInput()
        {
            return Console.ReadKey(true).KeyChar;
        }

        private static void ShowMeetings()
        {
            if (_meetingList.Count == 0)
            {
                Console.WriteLine("Сохранённые встречи отсутствуют.");
                return;
            }

            string meetingsText = CommonHelper.GetMeetingsInText(_meetingList);
            Console.WriteLine(meetingsText);
            Console.WriteLine($"Встреч всего: {_meetingList.Count}.");
        }

        private static void CreateNewMeeting()
        {
            var newMeeting = CommonHelper.GetNewMeetingInfoViaUserInput();

            if (CommonHelper.IsMeetingIntersectsOthers(newMeeting, _meetingList))
            {
                throw new Exception("Новое мероприятие пересекается с существующим(и).");
            }

            _meetingList.Add(newMeeting);
            Console.WriteLine($"Встреча \"{newMeeting.Name}\" добавлена.");

            SetNotification(newMeeting);
        }

        private static void ChangeNotificationTime()
        {
            if (_meetingList.Count == 0)
            {
                Console.WriteLine("Сохранённые встречи отсутствуют. Пожалуйста, добавьте встречу.");
                return;
            }

            string meetingsText = CommonHelper.GetMeetingsInText(_meetingList, numerate: true);
            Console.WriteLine(meetingsText);

            Console.WriteLine($"\nВведите номер встречи, время напоминания которой хотите изменить:");
            string meetingNumberString = Console.ReadLine().Trim();
            int meetingNumber;
            if(!Int32.TryParse(meetingNumberString, out meetingNumber))
            {
                throw new Exception("Номер не распознан.");
            }
            if (meetingNumber > _meetingList.Count)
            {
                throw new Exception("Не существует встречи с таким номером.");
            }

            meetingNumber--;
            if (meetingNumber < 0)
            {
                throw new Exception("Номер не может быть отрицательным или равным нулю.");
            }
            
            var existingMeeting = _meetingList[meetingNumber];

            Console.WriteLine($"Начало встречи \"{existingMeeting.Name}\" в {existingMeeting.StartTime}");
            Console.WriteLine("За сколько минут напомнить о встрече? Оставьте пустым и нажмите Enter, чтобы отменить напоминание.");
            Console.WriteLine($"Чтобы отменить редактирование, нажмите {UserMenuKeyConstants.CancelEditing}");

            string notifyBeforeMinutesString = Console.ReadLine().Trim();
            if (notifyBeforeMinutesString == UserMenuKeyConstants.CancelEditing.ToString())
            {
                Console.WriteLine("Действие отменено.");
                return;
            }

            if (String.IsNullOrEmpty(notifyBeforeMinutesString))
            {
                existingMeeting.SetNewNotificationTime(null);
                RemoveNotificationIfExists(existingMeeting);

                Console.WriteLine("Напоминание отменено.");
                return;
            }

            int notifyBeforeMinutes;
            if (!Int32.TryParse(notifyBeforeMinutesString, out notifyBeforeMinutes))
            {
                throw new Exception("Введенное значение должно быть целым числом.");
            }
            if (notifyBeforeMinutes < 0)
            {
                throw new Exception("Введенное значение должно быть положительным числом.");
            }

            existingMeeting.SetNewNotificationTime((uint?)notifyBeforeMinutes * 60);
            SetNotification(existingMeeting);
        }

        private static void ExportToFile()
        {
            if (_meetingList.Count == 0)
            {
                Console.WriteLine("Встречи отсутствуют.");
                return;
            }

            CommonHelper.ExportMeetingsToFile(_meetingList, OutputFileName);
            Console.WriteLine($"Данные о встречах сохранены в файл {OutputFileName}.");
        }

        private static void RemoveNotificationIfExists(Meeting meeting)
        {
            var existingNotification = _notificationList.SingleOrDefault(item => item.MeetingId == meeting.MeetingId);

            if (existingNotification != null)
            {
                existingNotification.StopNotification();
                _notificationList.Remove(existingNotification);
            }
        }

        private static void SetNotification(Meeting meeting)
        {
            RemoveNotificationIfExists(meeting);

            if (meeting.NotifyAt != null)
            {
                var newNotification = CommonHelper.CreateNotificationFromMeeting(meeting);
                if (newNotification != null)
                {
                    _notificationList.Add(newNotification);
                    Console.WriteLine($"Напоминание для встречи \"{meeting.Name}\" добавлено.");
                }
            }
        }
    }
}

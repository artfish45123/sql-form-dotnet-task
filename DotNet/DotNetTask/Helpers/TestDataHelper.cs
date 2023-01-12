using System;
using System.Collections.Generic;

namespace DotNetTask
{
    public static class TestDataHelper
    {
        private const int MeetingCount = 5;
        private const int MeetingDurationSeconds = 3;
        private const int MeetingIntervalSeconds = 2;
        private const int FirstMeetingOffsetSeconds = 5;

        public static (List<Meeting>, List<Notification>) GenerateTestMeetingsAndNotifications()
        {
            var meetingList = new List<Meeting>();

            DateTime latestStart = DateTime.Now.AddSeconds(FirstMeetingOffsetSeconds);
            DateTime latestEnd = latestStart.AddSeconds(MeetingDurationSeconds);

            for (int i = 0; i < MeetingCount; i++)
            {
                var newMeeting = new Meeting($"Тестовая встреча {i + 1}", latestStart, latestEnd, 1);

                meetingList.Add(newMeeting);

                latestStart = latestEnd.AddSeconds(MeetingIntervalSeconds);
                latestEnd = latestStart.AddSeconds(MeetingDurationSeconds);
            }

            meetingList.Shuffle();

            var notificationList = new List<Notification>();
            foreach (var meeting in meetingList)
            {
                var newNotification = CommonHelper.CreateNotificationFromMeeting(meeting);
                if (newNotification != null)
                {
                    notificationList.Add(newNotification);
                }
            }

            return (meetingList, notificationList);
        }
    }
}

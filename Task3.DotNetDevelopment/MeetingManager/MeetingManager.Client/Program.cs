
using MeetingManager.Models;
using MeetingManager.Services;
using MeetingManager.Services.Repositories;
using System;
using System.Threading.Tasks;

var meetingRepository = new MeetingRepository();
var notificationService = new NotificationService(meetingRepository);
var manager = new MeetingService(meetingRepository, notificationService);

while (true)
{
  Console.Clear();
  Console.WriteLine("1. Добавить встречу");
  Console.WriteLine("2. Просмотреть все встречи");
  Console.WriteLine("3. Обновить встречу");
  Console.WriteLine("4. Удалить встречу");
  Console.WriteLine("5. Просмотреть встречи на день");
  Console.WriteLine("6. Просмотреть встречу по ID");
  Console.WriteLine("7. Экспорт встреч в файл");
  Console.WriteLine("8. Выход");

  var choice = Console.ReadLine();

  try
  {
    switch (choice)
    {
      case "1":
        AddMeeting(manager);
        break;
      case "2":
        ViewMeetings(manager);
        break;
      case "3":
        UpdateMeeting(manager);
        break;
      case "4":
        DeleteMeeting(manager);
        break;
      case "5":
        ViewMeetingsByDate(manager);
        break;
      case "6":
        ViewMeeting(manager);
        break;
      case "7":
        await ExportMeetings(manager);
        break;
      case "8":
        return;
    }
  }
  catch (Exception ex)
  {
    Console.WriteLine($"Ошибка: {ex.Message}");
    Console.ReadKey();
  }
}

static void AddMeeting(MeetingService manager)
{
  var meeting = new Meeting();

  Console.Write("Название встречи: ");
  meeting.Title = Console.ReadLine();

  Console.Write("Дата и время начала (dd.MM.yyyy HH:mm): ");
  meeting.StartTime = DateTime.ParseExact(Console.ReadLine(), "dd.MM.yyyy HH:mm", null);

  Console.Write("Дата и время окончания (dd.MM.yyyy HH:mm): ");
  meeting.EndTime = DateTime.ParseExact(Console.ReadLine(), "dd.MM.yyyy HH:mm", null);

  Console.Write("Время напоминания (dd.MM.yyyy HH:mm или Enter чтобы пропустить): ");
  var notificationInput = Console.ReadLine();
  if (!string.IsNullOrEmpty(notificationInput))
    meeting.NotificationTime = DateTime.ParseExact(notificationInput, "dd.MM.yyyy HH:mm", null);

  manager.AddMeeting(meeting);
  Console.WriteLine("Встреча добавлена!");
  Console.ReadKey();
}

static void ViewMeetingsByDate(MeetingService manager)
{
  Console.Write("Введите дату (dd.MM.yyyy): ");
  var date = DateTime.ParseExact(Console.ReadLine(), "dd.MM.yyyy", null);

  var meetings = manager.GetMeetingsByDate(date);

  Console.WriteLine($"\nВстречи на {date:dd.MM.yyyy}:");
  foreach (var meeting in meetings)
  {
    Console.WriteLine(meeting);
  }
  Console.ReadKey();
}

static void ViewMeeting(MeetingService manager)
{
  Console.Write("Введите ID встречи для поиска: ");
  var id = int.Parse(Console.ReadLine());
  var meeting = manager.GetMeetingById(id);

  Console.WriteLine($"\nВстречи с ID {id}:");
  Console.WriteLine(meeting);

  Console.ReadKey();
}

static void ViewMeetings(MeetingService manager)
{
  var meetings = manager.GetMeetings();

  Console.WriteLine($"\nВсе встречи:");
  foreach (var meeting in meetings)
  {
    Console.WriteLine(meeting);
  }
  Console.ReadKey();
}

static async Task ExportMeetings(MeetingService manager)
{
  Console.Write("Введите дату (dd.MM.yyyy): ");
  var date = DateTime.ParseExact(Console.ReadLine(), "dd.MM.yyyy", null);

  Console.Write("Введите путь к файлу: ");
  var path = Console.ReadLine();

  await manager.ExportToFile(date, path);
  Console.WriteLine("Экспорт завершен!");
  Console.ReadKey();
}

static void DeleteMeeting(MeetingService manager)
{
  Console.Write("Введите ID встречи для удаления: ");
  var id = int.Parse(Console.ReadLine());

  manager.RemoveMeeting(id);
  Console.WriteLine("Встреча удалена!");
  Console.ReadKey();
}

static void UpdateMeeting(MeetingService manager)
{
  Console.Write("Введите ID встречи для изменения: ");
  var id = int.Parse(Console.ReadLine());

  var meeting = new Meeting() { Id = id };

  Console.Write("Название встречи: ");
  meeting.Title = Console.ReadLine();

  Console.Write("Дата и время начала (dd.MM.yyyy HH:mm): ");
  meeting.StartTime = DateTime.ParseExact(Console.ReadLine(), "dd.MM.yyyy HH:mm", null);

  Console.Write("Дата и время окончания (dd.MM.yyyy HH:mm): ");
  meeting.EndTime = DateTime.ParseExact(Console.ReadLine(), "dd.MM.yyyy HH:mm", null);

  Console.Write("Время напоминания (dd.MM.yyyy HH:mm или Enter чтобы пропустить): ");
  var notificationInput = Console.ReadLine();
  if (!string.IsNullOrEmpty(notificationInput))
  {
    meeting.NotificationTime = DateTime.ParseExact(notificationInput, "dd.MM.yyyy HH:mm", null);
    meeting.NotificationShown = false;
  }  
    
  manager.UpdateMeeting(meeting);
  Console.WriteLine("Встреча обновлена!");
  Console.ReadKey();
}


using MeetingManager.Models;
using MeetingManager.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingManager.Services
{
  /// <summary>
  /// Сервис для работы со встречами.
  /// </summary>
  public class MeetingService
  {
    #region Поля и свойства

    /// <summary>
    /// Репозиторий для работы со встречами.
    /// </summary>
    private readonly IMeetingRepository _repository;

    /// <summary>
    /// Сервис таймера.
    /// </summary>
    private readonly INotificationService _notificationService;

    #endregion 

    #region Методы 

    /// <summary>
    /// Добавить встречу.
    /// </summary>
    /// <param name="meeting">Встреча.</param>
    /// <exception cref="ArgumentException">Генерируется при неверных данных о встрече.</exception>
    public void AddMeeting(Meeting meeting)
    {
      ValidateMeeting(meeting);

      _repository.Add(meeting);
    }

    /// <summary>
    /// Обновить встречу.
    /// </summary>
    /// <param name="meeting">Встреча.</param>
    /// <exception cref="ArgumentException">Генерируется при неверных данных о встрече.</exception>
    public void UpdateMeeting(Meeting meeting)
    {
      var existingMeeting = _repository.GetById(meeting.Id);
      if (existingMeeting == null)
        throw new ArgumentException($"Встреча с ID {meeting.Id} не найдена.");

      ValidateMeeting(meeting);

      if (DateTime.Now > meeting.StartTime)
        throw new ArgumentException("Встреча уже началась или прошла.");

      _repository.Update(meeting);
    }

    /// <summary>
    /// Удалить встречу.
    /// </summary>
    /// <param name="id">Идентификатор встречи.</param>
    public void RemoveMeeting(int id)
    {
      _repository.Remove(id);
    }

    /// <summary>
    /// Получить встречи по дате.
    /// </summary>
    /// <param name="date">Дата.</param>
    /// <returns>Встречи.</returns>
    public List<Meeting> GetMeetingsByDate(DateTime date)
    {
      return _repository.GetByDate(date).ToList();
    }

    /// <summary>
    /// Получить встречe по ID.
    /// </summary>
    /// <param name="id">Идентификатор.</param>
    /// <returns>Встреча.</returns>
    public Meeting GetMeetingById(int id)
    {
      return _repository.GetById(id);
    }

    /// <summary>
    /// Получить встречи.
    /// </summary>
    /// <returns>Встречи.</returns>
    public List<Meeting> GetMeetings()
    {
      return _repository.GetAll().ToList();
    }

    /// <summary>
    /// Экспортировать встречи по дате.
    /// </summary>
    /// <param name="date">Дата.</param>
    /// <param name="filePath">Путь к файлу.</param>
    /// <returns></returns>
    public async Task ExportToFile(DateTime date, string filePath)
    {
      var dailyMeetings = GetMeetingsByDate(date);
      await File.WriteAllLinesAsync(filePath, dailyMeetings.Select(m => m.ToString()));
    }

    /// <summary>
    /// Валидация встреч.
    /// </summary>
    /// <param name="meeting">Встреча.</param>
    /// <exception cref="ArgumentException"></exception>
    private void ValidateMeeting(Meeting meeting)
    {
      if (meeting.StartTime < DateTime.Now)
        throw new ArgumentException("Встреча должна быть запланирована на будущее время");

      if (meeting.EndTime < meeting.StartTime)
        throw new ArgumentException("Неверно указано время конца встречи");

      if (meeting.NotificationTime > meeting.StartTime)
        throw new ArgumentException("Напоминание должно быть до начала встречи.");

      if (_repository.HasTimeConflict(meeting))
        throw new ArgumentException("Встречи не должны пересекаться");
    }

    #endregion

    #region Конструкторы

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="repository">Репозиторий встреч.</param>
    /// <param name="notificationService">Сервис таймера.</param>
    public MeetingService(IMeetingRepository repository,INotificationService notificationService)
    {
      _repository = repository;
      _notificationService = notificationService;

      _notificationService.Start();
    }

    #endregion
  }
}


using MeetingManager.Models;
using MeetingManager.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeetingManager.Services.Repositories
{
  /// <summary>
  /// Репозиторий для работы со встречами.
  /// </summary>
  public class MeetingRepository : IMeetingRepository
  {
    #region Поля и свойства

    /// <summary>
    /// Встречи.
    /// </summary>
    private readonly List<Meeting> _meetings = new List<Meeting>();

    /// <summary>
    /// Идентификатор встречи. 
    /// </summary>
    private int _nextId = 1;

    #endregion

    #region Методы

    /// <summary>
    /// Добавить новую встречу.
    /// </summary>
    /// <param name="meeting">Встреча.</param>
    /// <exception cref="ArgumentNullException">Генерируется, если передана null-ссылка</exception>
    public void Add(Meeting meeting)
    {
      if (meeting == null)
        throw new ArgumentNullException(nameof(meeting));

      meeting.Id = _nextId++;
      _meetings.Add(meeting);
    }

    /// <summary>
    /// Обновить встречу. 
    /// </summary>
    /// <param name="meeting">Встреча.</param>
    /// <exception cref="ArgumentNullException">Генерируется, если передана null-ссылка</exception>
    /// <exception cref="KeyNotFoundException">Генерируется, если встреча не найдена</exception>
    public void Update(Meeting meeting)
    {
      if (meeting == null)
        throw new ArgumentNullException(nameof(meeting));

      var existingMeeting = _meetings.FirstOrDefault(m => m.Id == meeting.Id);
      if (existingMeeting == null)
        throw new KeyNotFoundException($"Встреча с ID {meeting.Id} не найдена.");

      existingMeeting.Title = meeting.Title;
      existingMeeting.StartTime = meeting.StartTime;
      existingMeeting.EndTime = meeting.EndTime;
      existingMeeting.NotificationTime = meeting.NotificationTime;
    }

    /// <summary>
    /// Удавить встречу.
    /// </summary>
    /// <param name="id">Идентификатор встречи.</param>
    /// <exception cref="KeyNotFoundException">Генерируется, если встреча не найдена</exception>
    public void Remove(int id)
    {
      var meeting = _meetings.FirstOrDefault(m => m.Id == id);
      if (meeting == null)
        throw new KeyNotFoundException($"Встреча с ID {id} не найдена.");

      if (meeting != null)
      {
        _meetings.Remove(meeting);
      }
    }

    /// <summary>
    /// Получить встречу по ID.
    /// </summary>
    /// <param name="id">идентификатор встречи.</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">Генерируется, если встреча не найдена</exception>
    public Meeting GetById(int id)
    {
      var meeting = _meetings.FirstOrDefault(m => m.Id == id);
      if (meeting == null)
        throw new KeyNotFoundException($"Встреча с ID {id} не найдена.");

      return meeting;
    }

    /// <summary>
    /// Получить все встречи.
    /// </summary>
    /// <returns>Встречи.</returns>
    public IEnumerable<Meeting> GetAll()
    {
      return _meetings.OrderBy(m => m.StartTime).ToList();
    }

    /// <summary>
    /// Получить встречу по дате.
    /// </summary>
    /// <param name="date">Дата.</param>
    /// <returns>Встреча.</returns>
    /// <exception cref="ArgumentNullException">Генерируется, если не найдены встречи.</exception>
    public IEnumerable<Meeting> GetByDate(DateTime date)
    {
      var meetings = _meetings
        .Where(m => m.StartTime.Date == date.Date)
        .OrderBy(m => m.StartTime)
        .ToList();
      if (meetings == null)
        throw new ArgumentNullException(nameof(meetings));

      return meetings;
    }

    /// <summary>
    /// Получить предстоящие встречи. 
    /// </summary>
    /// <param name="currentTime">Дата.</param>
    /// <returns>Встречи.</returns>
    /// <exception cref="ArgumentNullException">Генерируется, если не найдены встречи.</exception>
    public IEnumerable<Meeting> GetUpcomingNotifications(DateTime currentTime)
    {
      var meetings = _meetings
        .Where(m => m.NotificationTime.HasValue &&
                    !m.NotificationShown &&
                    currentTime >= m.NotificationTime.Value &&
                    currentTime < m.StartTime);
      if (meetings == null)
        throw new ArgumentNullException(nameof(meetings));

      return meetings;
    }

    /// <summary>
    /// Проверить пересечение встречь в одно время.
    /// </summary>
    /// <param name="meeting">Встреча.</param>
    /// <returns>True если встречи пересекаются, иначе False.</returns>
    public bool HasTimeConflict(Meeting meeting)
    {
      return _meetings
        .Any(m => m.Id != meeting.Id &&
                  meeting.StartTime < m.EndTime &&
                  m.StartTime < meeting.EndTime);
    }

    #endregion
  }
}

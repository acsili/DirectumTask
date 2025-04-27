
using MeetingManager.Models;
using System;
using System.Collections.Generic;

namespace MeetingManager.Services.Interfaces
{
  /// <summary>
  /// Интерфейс репозитория для работы со встречами.
  /// </summary>
  public interface IMeetingRepository : IRepository<Meeting>
  {
    /// <summary>
    /// Проверить пересечение встречь в одно время.
    /// </summary>
    /// <param name="meeting">Встреча.</param>
    /// <returns>True если встречи пересекаются, иначе False.</returns>
    bool HasTimeConflict(Meeting meeting);

    /// <summary>
    /// Получить встречу по дате.
    /// </summary>
    /// <param name="date">Дата.</param>
    /// <returns>Встречи.</returns>
    IEnumerable<Meeting> GetByDate(DateTime date);

    /// <summary>
    /// Получить предстоящие встречи. 
    /// </summary>
    /// <param name="currentTime">Дата.</param>
    /// <returns>Встречи.</returns>
    IEnumerable<Meeting> GetUpcomingNotifications(DateTime currentTime);
  }
}

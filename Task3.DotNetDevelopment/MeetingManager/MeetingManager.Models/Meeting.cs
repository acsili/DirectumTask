
using System;

namespace MeetingManager.Models
{
  /// <summary>
  /// Встреча.
  /// </summary>
  public class Meeting
  {
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Начало встречи.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Конец встречи.
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Время напоминания.
    /// </summary>
    public DateTime? NotificationTime { get; set; }

    public bool NotificationShown { get; set; }

    public override string ToString()
    {
      var notification = NotificationTime.HasValue
          ? $" (напоминание в {NotificationTime:HH:mm})"
          : string.Empty;
      return $"{Id}: {StartTime:HH:mm} - {EndTime:HH:mm} | {Title}{notification}";
    }
  }
}

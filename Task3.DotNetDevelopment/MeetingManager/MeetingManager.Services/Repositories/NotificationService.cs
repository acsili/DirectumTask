using MeetingManager.Services.Interfaces;
using System;
using System.Threading;

namespace MeetingManager.Services.Repositories
{
  /// <summary>
  /// Сервис для отправки уведомлений о встречах.
  /// </summary>
  public class NotificationService : INotificationService, IDisposable
  {

    #region Поля и свойства

    /// <summary>
    /// Репозиторий встреч.
    /// </summary>
    private readonly IMeetingRepository _repository;

    /// <summary>
    /// Таймер.
    /// </summary>
    private Timer _timer;

    /// <summary>
    /// Освобождена ли память.
    /// </summary>
    private bool _disposed;

    #endregion 

    #region INotificationService

    /// <summary>
    /// Запустить сервис уведомлений.
    /// </summary>
    public void Start()
    {
      _timer = new Timer(CheckNotifications, null, 0, 30000);
    }

    /// <summary>
    /// Остановить сервис уведомлений.
    /// </summary>
    public void Stop()
    {
      _timer?.Dispose();
    }

    #endregion 

    #region Методы

    /// <summary>
    /// Проверить уведомления.
    /// </summary>
    private void CheckNotifications(object state)
    {
      var meetingsToNotify = _repository.GetUpcomingNotifications(DateTime.Now);

      foreach (var meeting in meetingsToNotify)
      {
        Console.WriteLine($"\nНАПОМИНАНИЕ: Встреча '{meeting.Title}' начнется в {meeting.StartTime:HH:mm}");
        meeting.NotificationShown = true;
        _repository.Update(meeting);
      }
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Освобождение ресурсов.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (_disposed) return;

      if (disposing)
      {
        _timer?.Dispose();
      }

      _disposed = true;
    }

    #endregion

    #region Конструкторы

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="repository">Репозиторий встреч.</param>
    public NotificationService(IMeetingRepository repository)
    {
      _repository = repository; ;
    }

    #endregion
  }
}

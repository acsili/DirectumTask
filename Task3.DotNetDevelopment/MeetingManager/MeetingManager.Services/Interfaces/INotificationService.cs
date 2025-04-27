using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingManager.Services.Interfaces
{
  /// <summary>
  /// Интерфейс сервиса уведомлений.
  /// </summary>
  public interface INotificationService : IDisposable
  {
    /// <summary>
    /// Запустить сервис уведомлений.
    /// </summary>
    void Start();

    /// <summary>
    /// Остановить сервис уведомлений.
    /// </summary>
    void Stop();
  }
}


using MeetingManager.Models;
using System.Collections.Generic;

namespace MeetingManager.Services.Interfaces
{
  /// <summary>
  /// Общий репозиторий.
  /// </summary>
  /// <typeparam name="T">Общий тип.</typeparam>
  public interface IRepository<T> where T : class
  {
    /// <summary>
    /// Добавить.
    /// </summary>
    /// <param name="item">Объект.</param>
    void Add(T item);

    /// <summary>
    /// Обновить.
    /// </summary>
    /// <param name="item">Объект.</param>
    void Update(T item);

    /// <summary>
    /// Удалить.
    /// </summary>
    /// <param name="id">Идентификатор.</param>
    void Remove(int id);

    /// <summary>
    /// Получить по ID.
    /// </summary>
    /// <param name="id">Идентификатор.</param>
    /// <returns>Общий тип.</returns>
    T GetById(int id);

    /// <summary>
    /// Получить все элементы.
    /// </summary>
    /// <returns>Список.</returns>
    IEnumerable<T> GetAll();
  }
}

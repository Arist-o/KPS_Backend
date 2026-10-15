using Catalog.Domain.Entity;
using Shared.Common.Interfaces;

namespace Catalog.Application.Common.Interfaces;

/// <summary>
/// Unit of Work для модуля Catalog.
/// Успадковує базові методи транзакцій від IBaseUnitOfWork (з Shared).
/// Додає репозиторії, специфічні для Catalog.
/// </summary>
public interface IUnitOfWork : IBaseUnitOfWork
{
    IRepository<Answer> Answers { get; }

    IRepository<Discipline> Disciplines { get; }

    IRepository<Question> Questions { get; }

    IRepository<Topic> Topics { get; }
}

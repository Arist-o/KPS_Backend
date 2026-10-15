using Shared.Common.Interfaces;
using Users.Domain.Entities;

namespace Users.Application.Common.Interfaces;

/// <summary>
/// Unit of Work для модуля Users.
/// Успадковує базові методи транзакцій від IBaseUnitOfWork (з Shared).
/// Додає репозиторії, специфічні для Users.
/// </summary>
public interface IUnitOfWork : IBaseUnitOfWork
{
    IRepository<ApplicationUser> Users { get; }
}

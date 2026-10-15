using MediatR;

namespace Shared.CQRS;

/// <summary>Маркерний інтерфейс — позначає команди для TransactionBehavior.</summary>
public interface IBaseCommand;

/// <summary>Команда без значення результату.</summary>
public interface ICommand : IRequest, IBaseCommand;

/// <summary>Команда з типізованим результатом.</summary>
public interface ICommand<out TResponse> : IRequest<TResponse>, IBaseCommand;

/// <summary>Запит (query) — лише читання, без транзакцій.</summary>
public interface IQuery<out TResponse> : IRequest<TResponse>;

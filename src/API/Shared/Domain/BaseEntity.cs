namespace Shared.Domain;

/// <summary>
/// Базова доменна сутність. Id генерується автоматично та не може бути змінений ззовні (DDD-Lite).
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
}

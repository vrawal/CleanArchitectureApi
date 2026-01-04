using MediatR;

namespace CleanArchitectureApi.Domain.Common;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
    Guid EventId { get; }
}


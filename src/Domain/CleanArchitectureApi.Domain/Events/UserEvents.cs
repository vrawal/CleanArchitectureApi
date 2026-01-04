using CleanArchitectureApi.Domain.Common;
using CleanArchitectureApi.Domain.Entities;

namespace CleanArchitectureApi.Domain.Events;

public class UserCreatedEvent : BaseDomainEvent
{
    public User User { get; }

    public UserCreatedEvent(User user)
    {
        User = user;
    }
}

public class UserProfileUpdatedEvent : BaseDomainEvent
{
    public User User { get; }

    public UserProfileUpdatedEvent(User user)
    {
        User = user;
    }
}

public class UserEmailConfirmedEvent : BaseDomainEvent
{
    public User User { get; }

    public UserEmailConfirmedEvent(User user)
    {
        User = user;
    }
}

public class UserDeactivatedEvent : BaseDomainEvent
{
    public User User { get; }

    public UserDeactivatedEvent(User user)
    {
        User = user;
    }
}

public class UserActivatedEvent : BaseDomainEvent
{
    public User User { get; }

    public UserActivatedEvent(User user)
    {
        User = user;
    }
}


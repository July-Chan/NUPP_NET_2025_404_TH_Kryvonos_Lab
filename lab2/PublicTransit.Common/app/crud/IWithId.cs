using System;

namespace PublicTransit.Common.App.Crud
{
    public interface IWithId
    {
        Guid Id { get; }
    }
}

using System;

namespace MarketingSystem.Common.Interfaces
{
    public interface IEntity
    {
        Guid Id { get; set; }

        DateTime CreatedDate { get; set; }

        DateTime UpdatedDate { get; set; }
    }
}

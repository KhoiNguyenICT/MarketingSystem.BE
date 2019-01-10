using System;
using System.Collections.Generic;
using System.Text;
using MarketingSystem.Common.Interfaces;

namespace MarketingSystem.Model.Entities
{
    public class BaseEntity: IEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}

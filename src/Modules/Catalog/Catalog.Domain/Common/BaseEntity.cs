using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Domain.Common
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}

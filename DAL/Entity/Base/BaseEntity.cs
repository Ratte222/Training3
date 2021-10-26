using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Entity.Base
{
    public class BaseEntity<T>
    {
        public T Id { get; set; }
    }
}

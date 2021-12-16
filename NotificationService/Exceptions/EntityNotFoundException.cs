using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public string Property { get; protected set; }
        public EntityNotFoundException(string message, string prop = "") : base(message)
        {
            Property = prop;
        }
    }
}

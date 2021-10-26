using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliaryLib.Extensions
{
    public static class GuidExtensions
    {
        public static string ToNumericString(this Guid guid)
        {
            return guid.ToString("N");
        }
    }
}

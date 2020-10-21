using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Extensions
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty(this Guid? item)
        {
            return !item.HasValue || item.Value == Guid.Empty;
        }

        public static bool NotNullNorEmpty(this Guid? item)
        {
            return item.HasValue && item.Value != Guid.Empty;
        }

        public static bool NotNull(this object item)
        {
            return item != null;
        }

        public static bool IsEqualTo(this Guid item, Guid comareItem)
        {
            return item.Equals(comareItem);
        }
    }
}

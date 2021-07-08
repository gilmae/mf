using System;
using System.Collections.Generic;
using System.Linq;

namespace mf
{
    public static class IEnumerableExtensions
    {
        public static T[] Append<T>(this T[] arr, T obj)
        {
            if (obj == null)
            {
                return arr;
            }
            if (arr == null)
            {
                return new T[] { obj };
            }
            var newArr = new T[arr.Length + 1];
            arr.CopyTo(newArr, 0);
            newArr[arr.Length] = obj;
            return newArr;
        }
    }
}

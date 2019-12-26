using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataGenerator.Extensions
{
    public static class RandomExtensions
    {
        public static T Random<T>(this IList<T> list)
        {
            if (list == null || list.Count < 1)
                return default(T);

            var index = RandomGenerator.Current.Next(list.Count);
            return list[index];
        }

        public static IEnumerable<T> Random<T>(this IList<T> list, int count)
        {
            if (list == null || list.Count < 1 || count < 1)
                yield break;

            for (int i = 0; i < count; i++)
                yield return Random(list);
        }

        public static T Random<T>(this IList<T> list, Func<T, int> weightSelector)
        {
            if (list == null || list.Count < 1)
                return default(T);

            if (weightSelector == null)
                return list.Random();

            int totalWeight = 0;
            var selected = default(T);

            foreach (var data in list)
            {
                int weight = weightSelector(data);
                int r = RandomGenerator.Current.Next(totalWeight + weight);

                if (r >= totalWeight)
                    selected = data;

                totalWeight += weight;
            }

            return selected;
        }

        public static IEnumerable<T> Random<T>(this IList<T> list, Func<T, int> weightSelector, int count)
        {
            if (list == null || list.Count < 1 || count < 1)
                yield break;

            for (int i = 0; i < count; i++)
                yield return Random(list, weightSelector);
        }


        public static int Next(this RandomNumberGenerator generator)
        {
            var buffer = new byte[4];
            generator.GetBytes(buffer);

            return BitConverter.ToInt32(buffer, 0);
        }

    }
}

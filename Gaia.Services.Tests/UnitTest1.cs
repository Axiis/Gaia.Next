using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Gaia.Services.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            HackerRank01.x(null);
        }
    }

    public class HackerRank01
    {
        public static void x(string[] args)
        {
            var test = new[] { 5, 4, 3, 4, 5 };

            var result = Solve(test);

            Console.WriteLine(result);
        }

        public static int Left(int[] array, int index)
        {
            if (index == 0) return 0;
            else
            {
                for(int cnt = index-1; cnt >=0; cnt--)
                {
                    if (array[cnt] > array[index])
                        return cnt + 1;
                }
                return 0;
            }
        }

        public static int Right(int[] array, int index)
        {
            if (index == array.Length - 1) return 0;
            else
            {
                for(int cnt = index + 1; cnt < array.Length; cnt++)
                {
                    if (array[cnt] > array[index])
                        return cnt + 1;
                }
                return 0;
            }
        }

        public static int IndexProduct(int[] array, int index) => Left(array, index) * Right(array, index);

        /// <summary>
        /// This is a very short/concise way of writing this
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int Solve(int[] array) => Enumerable.Range(0, array.Length).Max(index => IndexProduct(array, index));


        /// <summary>
        /// This is equivalent to "Solve" above
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int Solve2(int[] array)
        {
            int product = 0;
            for(int cnt=0;cnt<array.Length;cnt++)
            {
                var tempProduct = IndexProduct(array, cnt);

                if (tempProduct > product)
                    product = tempProduct;
            }

            return product;
        }
    }
}

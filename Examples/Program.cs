using System;
using FakerCore;

namespace Examples
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var faker = new Faker();

            int trueCount = 0;
            int falseCount = 0;
            
            for (var i = 0; i < 1000; i++)
            {

                if (faker.Create<bool>())
                    trueCount += 1;
                else
                    falseCount += 1;
            }

            //Console.Write($"TRUE:  {trueCount}\nFALSE: {falseCount}");
            Console.Write($"INT: {faker.Create<int>()}\nLONG: {faker.Create<long>()}");

            
        }
    }
}
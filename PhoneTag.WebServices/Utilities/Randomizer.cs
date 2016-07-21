using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhoneTag.WebServices.Utilities
{
    /// <summary>
    /// An adapter class for the system's random method, to make sure we don't re-generate a seed every
    /// time we use a random method.
    /// </summary>
    public static class Randomizer
    {
        private static Random s_Random;

        static Randomizer()
        {
            s_Random = new Random(DateTime.Now.Second * DateTime.Now.Minute + DateTime.Now.Millisecond);
        }

        public static int Range()
        {
            return s_Random.Next();
        }

        public static int Range(int i_Max)
        {
            return s_Random.Next(i_Max);
        }

        public static int Range(int i_Min, int i_Max)
        {
            return s_Random.Next(i_Min, i_Max);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{

    public static class Tags
    {
        public static string Enemy { get { return "Enemy"; } private set { } }
        public static string Player { get { return "Player"; } private set { } }
    }

    public static class Array
    {

        // Fisher–Yates shuffle
        public static T[] ShuffleArray<T>(T[] array, int seed)
        {
            System.Random prng = new System.Random(seed);

            // Loop through all elements of the array except the last one.
            for (int i = 0; i < array.Length - 1; i++)
            {
                int randomIndex = prng.Next(i, array.Length);
                T orig = array[i];
                array[i] = array[randomIndex];
                array[randomIndex] = orig;
            }

            return array;
        }
    }

}

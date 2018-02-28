using UnityEngine;

namespace AaronMeaney.BusStop.Utilities
{
    /// <summary>
    /// Utility class that generates random names
    /// </summary>
    public static class RandomNameGenerator
    {
        private static System.Random randomNumberGenerator = new System.Random(100);

        /// <summary>
        /// The file containing the list of first names
        /// </summary>
        public static TextAsset firstNamesFile = Resources.Load<TextAsset>("first_names");

        /// <summary>
        /// The file containing the list of last names
        /// </summary>
        public static TextAsset lastNamesFile = Resources.Load<TextAsset>("last_names");

        private static readonly string[] firstNames = GetArrayFromFile(firstNamesFile);
        private static readonly string[] lastNames = GetArrayFromFile(lastNamesFile);

        /// <summary>
        /// Returns the lines of a file as a string array
        /// </summary>
        /// <param name="file">The TextAsset of the file to read</param>
        /// <returns>The file line contents split up into an array</returns>
        private static string[] GetArrayFromFile(TextAsset file)
        {
            return file.text.Split("\n"[0]);
        }

        /// <summary>
        /// Returns a random first name from a list of names
        /// </summary>
        /// <returns>Random first name</returns>
        public static string GetFirstName()
        {
            int randomIndex = randomNumberGenerator.Next(0, firstNames.Length - 1);
            return firstNames[randomIndex];
        }

        /// <summary>
        /// Returns a random last name from a list of names
        /// </summary>
        /// <returns>Random last name</returns>
        public static string GetLastName()
        {
            int randomIndex = randomNumberGenerator.Next(0, lastNames.Length - 1);
            return lastNames[randomIndex];
        }
    }
}
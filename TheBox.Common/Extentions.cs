using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System
{
    public static class Extentions
    {
        /// <summary>
        /// Safely trims a string without causing an exception if the string is null.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string SafeTrim(this string input)
        {
            if (input == null)
            {
                return input;
            }
            return input.Trim();
        }

        /// <summary>
        /// Removes the non alpha numeric.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string RemoveNonAlphaNumeric(this string input)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            return rgx.Replace(input, " ");
        }
    }
}

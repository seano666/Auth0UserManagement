using System;
using System.Collections.Generic;
using System.Text;

namespace Auth0UserManagement
{
    class Utility
    {
        private static Random Random = new Random();

        #region private methods
        private static string RandomNumber() =>
            Random.Next().ToString();

        private static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor((double)((0x1a * Random.NextDouble()) + 0x41))));
                builder.Append(ch);
            }
            return (!lowerCase ? builder.ToString() : builder.ToString().ToLower());
        }
        #endregion

        #region public methods

        public static string RandomPassword()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(4, true));
            builder.Append(RandomNumber());
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }

        #endregion
    }
}

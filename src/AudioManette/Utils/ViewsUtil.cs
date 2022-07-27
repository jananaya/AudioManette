namespace AudioManette.Utils
{
    internal class ViewsUtil
    {
        public static string Displace(string str)
        {
            char[] strAsChars = str.ToCharArray();
            char p = ' ';
            char q = strAsChars[0];

            for (int i = 0; i < str.Length; i++)
            {
                bool itsEven = i % 2 == 0;

                if (i == str.Length - 1)
                {
                    strAsChars[0] = itsEven ? q : p;

                    break;
                }

                if (itsEven)
                {
                    p = strAsChars[i + 1];
                    strAsChars[i + 1] = q;
                }
                else
                {
                    q = strAsChars[i + 1];
                    strAsChars[i + 1] = p;
                }
            }

            return new string(strAsChars);
        }
    }
}

namespace ArrayUtility
{
    public class MyRandom
    {
        private static Random rnd = new Random();
        public static string RandomStringLower(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz123456789";
            string result = "";
            for (int i = 0; i < length; i++)
            {
                result += chars[rnd.Next(chars.Length)];
            }
            return result;
        }
    }
}

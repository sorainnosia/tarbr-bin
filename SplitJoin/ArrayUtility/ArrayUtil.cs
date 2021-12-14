namespace ArrayUtility
{
    public class ArrayUtil
    {
        public static T[] AddArray<T>(T[] test, T item)
        {
            T[] result = new T[test == null ? 1 : test.Length + 1];
            for (int i = 0; i < (test == null ? 0 : test?.Length); i++) result[i] = test[i];
            result[result.Length - 1] = item;
            return result;
        }
        public static T[] AddArray<T>(T test, T[] item)
        {
            T[] result = new T[item == null ? 1 : item.Length + 1];
            result[0] = test;
            for (int i = 0; i < (item == null ? 0 : item.Length); i++) result[i + 1] = item[i];
            return result;
        }
        public static T[] AddArray<T>(T[] test, T[] item)
        {
            T[] result = new T[test == null ? (item == null ? 0 : item.Length) : test.Length + (item == null ? 0 : item.Length)];
            for (int i = 0; i < (test == null ? 0 : test?.Length); i++) result[i] = test[i];
            for (int i = 0; i < (item == null ? 0 : item?.Length); i++) result[(test == null ? 0 : test.Length) + i] = item[i];
            return result;
        }

        public static bool ArrayContains<T>(T[] test, T item)
        {
            if (test == null) return false;
            for (int i = 0; i < test.Length; i++)
            {
                if (test[i] == null && item == null) return true;
                if (test[i].ToString().ToUpper().Equals(item.ToString().ToUpper())) return true;
            }
            return false;
        }
        public static U[] Select<T, U>(IEnumerable<T> items, Func<T, U> func)
        {
            U[] result = new U[0];
            foreach (T i in items)
            {
                U r = func(i);
                if (r != null)
                    result = AddArray(result, r);
            }
            return result;
        }

        public static U[] SelectMany<T, U>(IEnumerable<T> items, Func<T, U[]> func)
        {
            U[] result = new U[0];
            foreach (T i in items)
            {
                U[] r = func(i);
                if (r != null)
                    result = AddArray(result, r);
            }
            return result;
        }
    }
}
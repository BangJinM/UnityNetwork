namespace US.Common
{
    public class Singleton<T> where T : class, new()
    {
        public static T instance;

        public static T GetInstance()
        {
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }
    }
}

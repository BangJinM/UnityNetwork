namespace US
{
    public class Singleton<T> where T : Singleton<T>, new()
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

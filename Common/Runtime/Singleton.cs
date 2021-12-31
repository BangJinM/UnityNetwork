namespace US
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        protected static T mInstance = null;

        public static T Instance
        {
            get
            {
                {
                    if (mInstance == null)
                    {
                        mInstance = new T();
                    }
                    return mInstance;
                }
            }
        }

    }
}

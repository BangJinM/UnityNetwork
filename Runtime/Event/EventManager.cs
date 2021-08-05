using System;
using System.Collections.Generic;


namespace US
{
    public class Registrations<T> : IRegistrations
    {
        public Action<T> OnEvent = obj => { };
    }
    public class EventManager : Singleton<EventManager>, IEventManager
    {
        private Dictionary<Type, IRegistrations> mEventRegistrations;

        public void Send<T>() where T : new()
        {
            var e = new T();
            Send<T>(e);
        }
        public void Send<T>(T e)
        {
            var type = typeof(T);
            IRegistrations eventRegistrations;

            if (mEventRegistrations.TryGetValue(type, out eventRegistrations))
            {
                (eventRegistrations as Registrations<T>)?.OnEvent.Invoke(e);
            }
        }
        public void Register<T>(Action<T> onEvent)
        {
            var type = typeof(T);
            IRegistrations eventRegistrations;

            if (mEventRegistrations.TryGetValue(type, out eventRegistrations))
            {

            }
            else
            {
                eventRegistrations = new Registrations<T>();
                mEventRegistrations.Add(type, eventRegistrations);
            }
            (eventRegistrations as Registrations<T>).OnEvent += onEvent;
        }
        public void UnRegister<T>(Action<T> onEvent)
        {
            var type = typeof(T);
            IRegistrations eventRegistrations;

            if (mEventRegistrations.TryGetValue(type, out eventRegistrations))
            {
                (eventRegistrations as Registrations<T>).OnEvent -= onEvent;
            }
        }

        public void Init()
        {
            mEventRegistrations = new Dictionary<Type, IRegistrations>();
        }

        public void Destroy()
        {
            mEventRegistrations.Clear();
        }
    }
}

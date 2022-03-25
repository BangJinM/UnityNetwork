using System;
namespace US
{
    [Serializable]
    public class Reference
    {
        public int refCount = 0;

        protected Reference() { refCount = 0; }

        public bool IsUnused() { return refCount <= 0; }

        public void Retain() { refCount++; }

        public void Release() { refCount--; }

        public void ReleaseAll() { refCount = 0; }

        public int GetRefCount() { return refCount; }
    }
}

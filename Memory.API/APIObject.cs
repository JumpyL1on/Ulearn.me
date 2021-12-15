using System;

namespace Memory.API
{
    public class APIObject : IDisposable
    {
        public int AllocatedValue { get; }
		public bool IsDisposed { get; private set; } = false;
        public APIObject(int value)
        {
            AllocatedValue = value;
            MagicAPI.Allocate(value);
        }
		
        ~APIObject()
        {
            Dispose(false);
        }
		
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
		
        protected virtual void Dispose(bool fromDisposeMethod)
        {
            if (!IsDisposed)
            {
                MagicAPI.Free(AllocatedValue);
                IsDisposed = true;
            }
        }
    }
}
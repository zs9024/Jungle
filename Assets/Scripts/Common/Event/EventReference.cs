using System;


namespace CustomEvent
{
    class EventReference
    {
        private bool isOnce;
        private Delegate listener;
        private WeakReference weakReference;

        public EventReference(Delegate listener, bool useWeakReference, bool isOnce)
        {
            if (useWeakReference)
            {
                this.weakReference = new WeakReference(listener);  //短弱引用
            }
            else
            {
                this.listener = listener;
            }
            this.isOnce = isOnce;
        }

        public bool IsOnce
        {
            get
            {
                return this.isOnce;
            }
        }

        public Delegate Listener
        {
            get
            {
                if (this.listener != null)
                {
                    return this.listener;
                }
                if (this.weakReference != null)
                {
                    return (this.weakReference.Target as Delegate);
                }
                return null;
            }
        }
    }
}




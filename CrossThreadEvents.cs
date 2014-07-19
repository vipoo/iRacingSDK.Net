using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace iRacingSDK
{
    public class CrossThreadEvents<T>
    {
        event Action<T> evnt;

        Dictionary<Action<T>, Action<T>> evntDelegates = new Dictionary<Action<T>, Action<T>>();

        public void Invoke(T t)
        {
            if (evnt != null)
                evnt(t);
        }

        public event Action<T> Event 
        {
            add
            {
                var context = SynchronizationContext.Current;
                Action<T> newDelgate;

                if (context != null)
                    newDelgate = (d) => context.Send(i => value(d), null);
                else
                    newDelgate = value;

                evntDelegates.Add(value, newDelgate);
                evnt += newDelgate;
            }

            remove
            {
                var context = SynchronizationContext.Current;

                var delgate = evntDelegates[value];
                evntDelegates.Remove(value);

                evnt -= delgate;
            }
        }
    }

    public class CrossThreadEvents
    {
        event Action evnt;

        Dictionary<Action, Action> evntDelegates = new Dictionary<Action, Action>();

        public void Invoke()
        {
            if (evnt != null)
                evnt();
        }
        public event Action Event
        {
            add
            {
                var context = SynchronizationContext.Current;
                Action newDelgate;

                if (context != null)
                    newDelgate = () => context.Send(i => value(), null);
                else
                    newDelgate = value;

                evntDelegates.Add(value, newDelgate);
                evnt += newDelgate;
            }

            remove
            {
                var context = SynchronizationContext.Current;

                var delgate = evntDelegates[value];
                evntDelegates.Remove(value);

                evnt -= delgate;
            }
        }
    }

}

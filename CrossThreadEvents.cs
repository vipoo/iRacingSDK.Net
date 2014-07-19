﻿// This file is part of iRacingSDK.
//
// Copyright 2014 Dean Netherton
// https://github.com/vipoo/iRacingSDK.Net
//
// iRacingSDK is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// iRacingSDK is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with iRacingSDK.  If not, see <http://www.gnu.org/licenses/>.


using System;
using System.Collections.Generic;
using System.Threading;

namespace iRacingSDK
{
    internal class CrossThreadEvents<T>
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

    internal class CrossThreadEvents
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

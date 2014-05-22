// This file is part of iRacingSDK.
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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace iRacingSDK
{
    public delegate void DataSampleEventHandler(DataSample data);

    public partial class iRacing
    {
        static DataSampleEventHandler newData;
        static Dictionary<DataSampleEventHandler, DataSampleEventHandler> newDataDelegates = new Dictionary<DataSampleEventHandler, DataSampleEventHandler>();

        public static event DataSampleEventHandler NewData
        {
            add
            {
                var context = SynchronizationContext.Current;
                DataSampleEventHandler newDelgate;

                if (context != null)
                    newDelgate = (d) => context.Send(i => value(d), null);
                else
                    newDelgate = value;

                newDataDelegates.Add(value, newDelgate);
                newData += newDelgate;
            }
            remove
            {
                var context = SynchronizationContext.Current;

                var delgate = newDataDelegates[value];
                newDataDelegates.Remove(value);

                newData -= delgate;
            }
        }

        static Task backListener;
        static bool requestCancel;

        public static void StartListening()
        {
            if( backListener != null )
                throw new Exception("Already listening to iRacing data");

            requestCancel = false;
            
            backListener = new Task(Listen);
            backListener.Start();
        }

        public static void StopListening()
        {
            var bl = backListener;

            if (backListener == null)
                throw new Exception("Not currently listening to iRacing data");

            requestCancel = true;
            bl.Wait(500);
        }

        static void Listen()
        {
            try
            {
                foreach (var d in iRacing.GetDataFeed())
                {
                    if (requestCancel)
                        return;

                    if (newData != null)
                        newData(d);
                }
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message, "DEBUG");
                Trace.WriteLine(e.StackTrace, "DEBUG");
                throw e;
            }
            finally
            {
                backListener = null;
            }
        }
    }
}

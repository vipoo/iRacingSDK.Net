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
    public class iRacingEvents : IDisposable
    {
        readonly iRacingConnection instance = new iRacingConnection();
        readonly CrossThreadEvents<DataSample> newData = new CrossThreadEvents<DataSample>();
        readonly CrossThreadEvents<DataSample> newSessionData = new CrossThreadEvents<DataSample>();
        readonly CrossThreadEvents connected = new CrossThreadEvents();
        readonly CrossThreadEvents disconnected = new CrossThreadEvents();

        Task backListener;
        bool requestCancel;

        public event Action Connected
        {
            add { connected.Event += value; }
            remove { connected.Event -= value; }
        }

        public event Action Disconnected
        {
            add { disconnected.Event += value; }
            remove { disconnected.Event -= value; }
        }

        public event Action<DataSample> NewData
        {
            add { newData.Event += value; }
            remove { newData.Event -= value; }
        }

        public event Action<DataSample> NewSessionData
        {
            add { newSessionData.Event += value; }
            remove { newSessionData.Event -= value; }
        }

        public void StartListening()
        {
            if( backListener != null )
                throw new Exception("Already listening to iRacing data");

            requestCancel = false;
            
            backListener = new Task(Listen);
            backListener.Start();
        }

        public void StopListening()
        {
            var bl = backListener;

            if (backListener == null)
                throw new Exception("Not currently listening to iRacing data");

            requestCancel = true;
            bl.Wait(500);
        }

        void Listen()
        {
            var isConnected = false;
            var isDisconnected = true;
            var lastSessionInfoUpdate = -1;
            
            try
            {
                foreach (var d in instance.GetDataFeed(logging: false))
                {
                    if (requestCancel)
                        return;

                    if (!isConnected && d.IsConnected)
                    {
                        isConnected = true;
                        isDisconnected = false;
                        connected.Invoke();
                    }

                    if (!isDisconnected && !d.IsConnected)
                    {
                        isConnected = false;
                        isDisconnected = true;
                        disconnected.Invoke();
                    }

                    if( d.IsConnected)
                        newData.Invoke(d);

                    if (d.IsConnected && d.SessionData.InfoUpdate != lastSessionInfoUpdate)
                    {
                        lastSessionInfoUpdate = d.SessionData.InfoUpdate;
                        newSessionData.Invoke(d);
                    }
                }
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message, "DEBUG");
                Trace.WriteLine(e.StackTrace, "DEBUG");
            }
            finally
            {
                backListener = null;
            }
        }

        public void Dispose()
        {
            StopListening();
        }
    }
}

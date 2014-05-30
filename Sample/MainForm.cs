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

using iRacingSDK;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sample
{
    public partial class MainForm : Form
    {
        LogMessages logMessages;

        public MainForm()
        {
            InitializeComponent();

            logMessages = new LogMessages();
            Trace.Listeners.Add(new MyListener(logMessages.TraceMessage));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new EventSample().Show();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            logMessages.StartOperation( () => {

                FastLap lastFastestLap = null;

                Trace.WriteLine("Moving to start of race");
                iRacing.Replay.MoveToStartOfRace();
                Trace.WriteLine("Watching for fastest laps");

                foreach (var data in iRacing.GetDataFeed().AtSpeed(16).WithFastestLaps())
                {
                    if (lastFastestLap != data.Telemetry.FastestLap)
                        Trace.WriteLine(string.Format("{0} - {1}", data.Telemetry.FastestLap.Driver.UserName, data.Telemetry.FastestLap.Time));

                    lastFastestLap = data.Telemetry.FastestLap;
                }
            });
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            logMessages.Close();
        }

        System.Collections.Concurrent.ConcurrentQueue<string> traceMessages = new System.Collections.Concurrent.ConcurrentQueue<string>();

        private void iRacingInstance_Click(object sender, EventArgs e)
        {
            logMessages.StartOperation(() =>
            {
                //Can use both event and enumerable accesses with instance types.
                var instance1 = new iRacingInstance();
                instance1.NewData += instance1_NewData;
                instance1.StartListening();

                var iracingInstance = new iRacingInstance();

                var start = DateTime.Now;
                foreach (var data in iracingInstance.GetDataFeed())
                {
                    if (DateTime.Now - start > TimeSpan.FromSeconds(1))
                        break;

                    traceMessages.Enqueue(string.Format("Enumerable Data Tick {0}", data.Telemetry.TickCount));
                }

                instance1.StopListening();

                foreach( var m in traceMessages)
                    Trace.WriteLine(m);
            });
        }

        void instance1_NewData(DataSample data)
        {
            traceMessages.Enqueue(string.Format("Event Data Tick {0}", data.Telemetry.TickCount));
        }
    }
}

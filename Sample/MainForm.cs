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

        private void fastestLapButton_Click(object sender, EventArgs e)
        {
            logMessages.StartOperation(SampleFastestLap.Sample);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            logMessages.Close();
        }

        private void iRacingInstance_Click(object sender, EventArgs e)
        {
            logMessages.StartOperation(SampleEnumerableAndEventAccess.Sample);
        }

        private void TotalDistances_click(object sender, EventArgs e)
        {
            logMessages.StartOperation(SampleTotalDistane.Sample);
        }

        private void LapSectors_click(object sender, EventArgs e)
        {
            logMessages.StartOperation(SampleLapSector.Sample);
        }

        private void TakeUntilAfter_click(object sender, EventArgs e)
        {
            logMessages.StartOperation(SampleTakeUntilAfter.Sample);
        }

        private void RaceEnd_Click(object sender, EventArgs e)
        {
            logMessages.StartOperation(SampleRaceEnd.Sample);
        }

        private void WithFinishingStatuses_Click(object sender, EventArgs e)
        {
            logMessages.StartOperation(SampleWithFinishingStatuses.Sample);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            logMessages.StartOperation(SampleQueuedDataFeed.Sample);
        }

        private void ReplayMove_Click(object sender, EventArgs e)
        {
            logMessages.StartOperation(ReplayMove.Sample);
        }

        private void FindIncidents_Click(object sender, EventArgs e)
        {
            logMessages.StartOperation(SampleFindIncidents.Sample);
        }

        private void InspectDataSamples_Click(object sender, EventArgs e)
        {
            logMessages.StartOperation(SampleTelemetryAccess.Sample);
        }

        private void PitCommands_Click(object sender, EventArgs e)
        {
            logMessages.StartOperation(SamplePitCommands.Sample);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void IRacingDataStreamConnection_Click(object sender, EventArgs e)
        {
            logMessages.StartOperation(SampleIRacingDataStreamConnection.Sample);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            logMessages.StartOperation(SampleConventionalEventData.Sample);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            logMessages.StartOperation(SampleCarAndSessionState.Sample);

        }

        private void button16_Click(object sender, EventArgs e)
        {
            logMessages.StartOperation(SamplePerformanceMeasurement.Sample);
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button17_Click(object sender, EventArgs e)
        {
            DateTime targetPosition = dateTimePicker_RacePosition.Value;
            int targetframe = 60 * (targetPosition.Hour * 3600 + targetPosition.Minute*60 +targetPosition.Second);

            var iracing = new iRacingConnection();

            iRacing.Replay.MoveToFrame(targetframe);

            Trace.WriteLine("Moved to Frame");
        }
    }
}

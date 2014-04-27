
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
using System.Linq;

namespace iRacingSDK
{
	public partial class Telemetry : Dictionary<string, object>
	{
		internal SessionData SessionData { get; set; }


		public System.Double 	SessionTime				{ get { return (System.Double)		this["SessionTime"]; 			} }

		public System.Int32 	SessionNum				{ get { return (System.Int32)		this["SessionNum"]; 			} }

		public iRacingSDK.SessionState 	SessionState				{ get { return (iRacingSDK.SessionState)		this["SessionState"]; 			} }

		public System.Int32 	SessionUniqueID				{ get { return (System.Int32)		this["SessionUniqueID"]; 			} }

        public iRacingSDK.SessionFlags SessionFlags { get { return (iRacingSDK.SessionFlags)(System.Int32)this["SessionFlags"]; } }

		public System.Double 	SessionTimeRemain				{ get { return (System.Double)		this["SessionTimeRemain"]; 			} }

		public System.Int32 	SessionLapsRemain				{ get { return (System.Int32)		this["SessionLapsRemain"]; 			} }

		public System.Int32 	RadioTransmitCarIdx				{ get { return (System.Int32)		this["RadioTransmitCarIdx"]; 			} }

		public System.Boolean 	DriverMarker				{ get { return (System.Boolean)		this["DriverMarker"]; 			} }

		public System.Boolean 	IsReplayPlaying				{ get { return (System.Boolean)		this["IsReplayPlaying"]; 			} }

		public System.Int32 	ReplayFrameNum				{ get { return (System.Int32)		this["ReplayFrameNum"]; 			} }

		public System.Int32 	ReplayFrameNumEnd				{ get { return (System.Int32)		this["ReplayFrameNumEnd"]; 			} }

		public System.Single 	FrameRate				{ get { return (System.Single)		this["FrameRate"]; 			} }

		public System.Single 	CpuUsageBG				{ get { return (System.Single)		this["CpuUsageBG"]; 			} }

		public System.Int32[] 	CarIdxLap				{ get { return (System.Int32[])		this["CarIdxLap"]; 			} }

		public System.Single[] 	CarIdxLapDistPct				{ get { return (System.Single[])		this["CarIdxLapDistPct"]; 			} }

		public System.Int32[] 	CarIdxTrackSurface				{ get { return (System.Int32[])		this["CarIdxTrackSurface"]; 			} }

		public System.Boolean[] 	CarIdxOnPitRoad				{ get { return (System.Boolean[])		this["CarIdxOnPitRoad"]; 			} }

		public System.Boolean 	OnPitRoad				{ get { return (System.Boolean)		this["OnPitRoad"]; 			} }

		public System.Single[] 	CarIdxSteer				{ get { return (System.Single[])		this["CarIdxSteer"]; 			} }

		public System.Single[] 	CarIdxRPM				{ get { return (System.Single[])		this["CarIdxRPM"]; 			} }

		public System.Int32[] 	CarIdxGear				{ get { return (System.Int32[])		this["CarIdxGear"]; 			} }

		public System.Single 	SteeringWheelAngle				{ get { return (System.Single)		this["SteeringWheelAngle"]; 			} }

		public System.Single 	Throttle				{ get { return (System.Single)		this["Throttle"]; 			} }

		public System.Single 	Brake				{ get { return (System.Single)		this["Brake"]; 			} }

		public System.Single 	Clutch				{ get { return (System.Single)		this["Clutch"]; 			} }

		public System.Int32 	Gear				{ get { return (System.Int32)		this["Gear"]; 			} }

		public System.Single 	RPM				{ get { return (System.Single)		this["RPM"]; 			} }

		public System.Int32 	Lap				{ get { return (System.Int32)		this["Lap"]; 			} }

		public System.Single 	LapDist				{ get { return (System.Single)		this["LapDist"]; 			} }

		public System.Single 	LapDistPct				{ get { return (System.Single)		this["LapDistPct"]; 			} }

		public System.Int32 	RaceLaps				{ get { return (System.Int32)		this["RaceLaps"]; 			} }

		public System.Int32 	LapBestLap				{ get { return (System.Int32)		this["LapBestLap"]; 			} }

		public System.Single 	LapBestLapTime				{ get { return (System.Single)		this["LapBestLapTime"]; 			} }

		public System.Single 	LapLastLapTime				{ get { return (System.Single)		this["LapLastLapTime"]; 			} }

		public System.Single 	LapCurrentLapTime				{ get { return (System.Single)		this["LapCurrentLapTime"]; 			} }

		public System.Single 	LapDeltaToBestLap				{ get { return (System.Single)		this["LapDeltaToBestLap"]; 			} }

		public System.Single 	LapDeltaToBestLap_DD				{ get { return (System.Single)		this["LapDeltaToBestLap_DD"]; 			} }

		public System.Boolean 	LapDeltaToBestLap_OK				{ get { return (System.Boolean)		this["LapDeltaToBestLap_OK"]; 			} }

		public System.Single 	LapDeltaToOptimalLap				{ get { return (System.Single)		this["LapDeltaToOptimalLap"]; 			} }

		public System.Single 	LapDeltaToOptimalLap_DD				{ get { return (System.Single)		this["LapDeltaToOptimalLap_DD"]; 			} }

		public System.Boolean 	LapDeltaToOptimalLap_OK				{ get { return (System.Boolean)		this["LapDeltaToOptimalLap_OK"]; 			} }

		public System.Single 	LapDeltaToSessionBestLap				{ get { return (System.Single)		this["LapDeltaToSessionBestLap"]; 			} }

		public System.Single 	LapDeltaToSessionBestLap_DD				{ get { return (System.Single)		this["LapDeltaToSessionBestLap_DD"]; 			} }

		public System.Boolean 	LapDeltaToSessionBestLap_OK				{ get { return (System.Boolean)		this["LapDeltaToSessionBestLap_OK"]; 			} }

		public System.Single 	LapDeltaToSessionOptimalLap				{ get { return (System.Single)		this["LapDeltaToSessionOptimalLap"]; 			} }

		public System.Single 	LapDeltaToSessionOptimalLap_DD				{ get { return (System.Single)		this["LapDeltaToSessionOptimalLap_DD"]; 			} }

		public System.Boolean 	LapDeltaToSessionOptimalLap_OK				{ get { return (System.Boolean)		this["LapDeltaToSessionOptimalLap_OK"]; 			} }

		public System.Single 	LapDeltaToSessionLastlLap				{ get { return (System.Single)		this["LapDeltaToSessionLastlLap"]; 			} }

		public System.Single 	LapDeltaToSessionLastlLap_DD				{ get { return (System.Single)		this["LapDeltaToSessionLastlLap_DD"]; 			} }

		public System.Boolean 	LapDeltaToSessionLastlLap_OK				{ get { return (System.Boolean)		this["LapDeltaToSessionLastlLap_OK"]; 			} }

		public System.Single 	LongAccel				{ get { return (System.Single)		this["LongAccel"]; 			} }

		public System.Single 	LatAccel				{ get { return (System.Single)		this["LatAccel"]; 			} }

		public System.Single 	VertAccel				{ get { return (System.Single)		this["VertAccel"]; 			} }

		public System.Single 	RollRate				{ get { return (System.Single)		this["RollRate"]; 			} }

		public System.Single 	PitchRate				{ get { return (System.Single)		this["PitchRate"]; 			} }

		public System.Single 	YawRate				{ get { return (System.Single)		this["YawRate"]; 			} }

		public System.Single 	Speed				{ get { return (System.Single)		this["Speed"]; 			} }

		public System.Single 	VelocityX				{ get { return (System.Single)		this["VelocityX"]; 			} }

		public System.Single 	VelocityY				{ get { return (System.Single)		this["VelocityY"]; 			} }

		public System.Single 	VelocityZ				{ get { return (System.Single)		this["VelocityZ"]; 			} }

		public System.Single 	Yaw				{ get { return (System.Single)		this["Yaw"]; 			} }

		public System.Single 	Pitch				{ get { return (System.Single)		this["Pitch"]; 			} }

		public System.Single 	Roll				{ get { return (System.Single)		this["Roll"]; 			} }

		public System.Single 	PitRepairLeft				{ get { return (System.Single)		this["PitRepairLeft"]; 			} }

		public System.Single 	PitOptRepairLeft				{ get { return (System.Single)		this["PitOptRepairLeft"]; 			} }

		public System.Int32 	CamCarIdx				{ get { return (System.Int32)		this["CamCarIdx"]; 			} }

		public System.Int32 	CamCameraNumber				{ get { return (System.Int32)		this["CamCameraNumber"]; 			} }

		public System.Int32 	CamGroupNumber				{ get { return (System.Int32)		this["CamGroupNumber"]; 			} }

		public System.Int32 	CamCameraState				{ get { return (System.Int32)		this["CamCameraState"]; 			} }

		public System.Boolean 	IsOnTrack				{ get { return (System.Boolean)		this["IsOnTrack"]; 			} }

		public System.Boolean 	IsInGarage				{ get { return (System.Boolean)		this["IsInGarage"]; 			} }

		public System.Single 	SteeringWheelTorque				{ get { return (System.Single)		this["SteeringWheelTorque"]; 			} }

		public System.Single 	SteeringWheelPctTorque				{ get { return (System.Single)		this["SteeringWheelPctTorque"]; 			} }

		public System.Single 	SteeringWheelPctTorqueSign				{ get { return (System.Single)		this["SteeringWheelPctTorqueSign"]; 			} }

		public System.Single 	SteeringWheelPctTorqueSignStops				{ get { return (System.Single)		this["SteeringWheelPctTorqueSignStops"]; 			} }

		public System.Single 	SteeringWheelPctDamper				{ get { return (System.Single)		this["SteeringWheelPctDamper"]; 			} }

		public System.Single 	SteeringWheelAngleMax				{ get { return (System.Single)		this["SteeringWheelAngleMax"]; 			} }

		public System.Single 	ShiftIndicatorPct				{ get { return (System.Single)		this["ShiftIndicatorPct"]; 			} }

		public System.Single 	ShiftPowerPct				{ get { return (System.Single)		this["ShiftPowerPct"]; 			} }

		public System.Single 	ShiftGrindRPM				{ get { return (System.Single)		this["ShiftGrindRPM"]; 			} }

		public System.Single 	ThrottleRaw				{ get { return (System.Single)		this["ThrottleRaw"]; 			} }

		public System.Single 	BrakeRaw				{ get { return (System.Single)		this["BrakeRaw"]; 			} }

		public System.Int32 	EngineWarnings				{ get { return (System.Int32)		this["EngineWarnings"]; 			} }

		public System.Single 	FuelLevel				{ get { return (System.Single)		this["FuelLevel"]; 			} }

		public System.Single 	FuelLevelPct				{ get { return (System.Single)		this["FuelLevelPct"]; 			} }

		public System.Int32 	ReplayPlaySpeed				{ get { return (System.Int32)		this["ReplayPlaySpeed"]; 			} }

		public System.Boolean 	ReplayPlaySlowMotion				{ get { return (System.Boolean)		this["ReplayPlaySlowMotion"]; 			} }

		public System.Double 	ReplaySessionTime				{ get { return (System.Double)		this["ReplaySessionTime"]; 			} }

		public System.Int32 	ReplaySessionNum				{ get { return (System.Int32)		this["ReplaySessionNum"]; 			} }

		public System.Single 	dcBrakeBias				{ get { return (System.Single)		this["dcBrakeBias"]; 			} }

		public System.Single 	WaterTemp				{ get { return (System.Single)		this["WaterTemp"]; 			} }

		public System.Single 	WaterLevel				{ get { return (System.Single)		this["WaterLevel"]; 			} }

		public System.Single 	FuelPress				{ get { return (System.Single)		this["FuelPress"]; 			} }

		public System.Single 	OilTemp				{ get { return (System.Single)		this["OilTemp"]; 			} }

		public System.Single 	OilPress				{ get { return (System.Single)		this["OilPress"]; 			} }

		public System.Single 	OilLevel				{ get { return (System.Single)		this["OilLevel"]; 			} }

		public System.Single 	Voltage				{ get { return (System.Single)		this["Voltage"]; 			} }

		public System.Single 	ManifoldPress				{ get { return (System.Single)		this["ManifoldPress"]; 			} }

		public System.Single 	RRcoldPressure				{ get { return (System.Single)		this["RRcoldPressure"]; 			} }

		public System.Single 	RRtempCL				{ get { return (System.Single)		this["RRtempCL"]; 			} }

		public System.Single 	RRtempCM				{ get { return (System.Single)		this["RRtempCM"]; 			} }

		public System.Single 	RRtempCR				{ get { return (System.Single)		this["RRtempCR"]; 			} }

		public System.Single 	RRwearL				{ get { return (System.Single)		this["RRwearL"]; 			} }

		public System.Single 	RRwearM				{ get { return (System.Single)		this["RRwearM"]; 			} }

		public System.Single 	RRwearR				{ get { return (System.Single)		this["RRwearR"]; 			} }

		public System.Single 	LRcoldPressure				{ get { return (System.Single)		this["LRcoldPressure"]; 			} }

		public System.Single 	LRtempCL				{ get { return (System.Single)		this["LRtempCL"]; 			} }

		public System.Single 	LRtempCM				{ get { return (System.Single)		this["LRtempCM"]; 			} }

		public System.Single 	LRtempCR				{ get { return (System.Single)		this["LRtempCR"]; 			} }

		public System.Single 	LRwearL				{ get { return (System.Single)		this["LRwearL"]; 			} }

		public System.Single 	LRwearM				{ get { return (System.Single)		this["LRwearM"]; 			} }

		public System.Single 	LRwearR				{ get { return (System.Single)		this["LRwearR"]; 			} }

		public System.Single 	RFcoldPressure				{ get { return (System.Single)		this["RFcoldPressure"]; 			} }

		public System.Single 	RFtempCL				{ get { return (System.Single)		this["RFtempCL"]; 			} }

		public System.Single 	RFtempCM				{ get { return (System.Single)		this["RFtempCM"]; 			} }

		public System.Single 	RFtempCR				{ get { return (System.Single)		this["RFtempCR"]; 			} }

		public System.Single 	RFwearL				{ get { return (System.Single)		this["RFwearL"]; 			} }

		public System.Single 	RFwearM				{ get { return (System.Single)		this["RFwearM"]; 			} }

		public System.Single 	RFwearR				{ get { return (System.Single)		this["RFwearR"]; 			} }

		public System.Single 	LFcoldPressure				{ get { return (System.Single)		this["LFcoldPressure"]; 			} }

		public System.Single 	LFtempCL				{ get { return (System.Single)		this["LFtempCL"]; 			} }

		public System.Single 	LFtempCM				{ get { return (System.Single)		this["LFtempCM"]; 			} }

		public System.Single 	LFtempCR				{ get { return (System.Single)		this["LFtempCR"]; 			} }

		public System.Single 	LFwearL				{ get { return (System.Single)		this["LFwearL"]; 			} }

		public System.Single 	LFwearM				{ get { return (System.Single)		this["LFwearM"]; 			} }

		public System.Single 	LFwearR				{ get { return (System.Single)		this["LFwearR"]; 			} }

		public System.Single 	RRshockDefl				{ get { return (System.Single)		this["RRshockDefl"]; 			} }

		public System.Single 	LRshockDefl				{ get { return (System.Single)		this["LRshockDefl"]; 			} }

		public System.Single 	RFshockDefl				{ get { return (System.Single)		this["RFshockDefl"]; 			} }

		public System.Single 	LFshockDefl				{ get { return (System.Single)		this["LFshockDefl"]; 			} }

		public System.Int32 	TickCount				{ get { return (System.Int32)		this["TickCount"]; 			} }

	}
}

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

namespace iRacingSDK
{
    public enum SessionFlags : uint
    {
        // global flags
        checkered = 0x00000001,
        white = 0x00000002,
        green = 0x00000004,
        yellow = 0x00000008,
        red = 0x00000010,
        blue = 0x00000020,
        debris = 0x00000040,
        crossed = 0x00000080,
        yellowWaving = 0x00000100,
        oneLapToGreen = 0x00000200,
        greenHeld = 0x00000400,
        tenToGo = 0x00000800,
        fiveToGo = 0x00001000,
        randomWaving = 0x00002000,
        caution = 0x00004000,
        cautionWaving = 0x00008000,

        // drivers black flags
        black = 0x00010000,
        disqualify = 0x00020000,
        servicible = 0x00040000, // car is allowed service (not a flag)
        furled = 0x00080000,
        repair = 0x00100000,

        // start lights
        startHidden = 0x10000000,
        startReady = 0x20000000,
        startSet = 0x40000000,
        startGo = 0x80000000,
    };

	/// <summary>
	/// Remote controll the sim by sending these windows messages
	/// camera and replay commands only work when you are out of your car, 
	/// pit commands only work when in your car
	/// </summary>
	enum BroadcastMessage
	{
		/// <summary>
		/// The camera switch position : car position, group, camera
		/// </summary>
		CameraSwitchPos = 0,

		/// <summary>
		/// The camera switch number : driver #, group, camera
		/// </summary>
		CameraSwitchNum,

		/// <summary>
		/// The state of the camera set: CameraState, unused, unused
		/// </summary>
		CameraSetState,

		/// <summary>
		/// The replay set play speed: speed, slowMotion, unused
		/// </summary>
		ReplaySetPlaySpeed, 

		/// <summary>
		/// The replay set play position: ReplayPositionMode, Frame Number (high, low)
		/// </summary>
		ReplaySetPlayPosition,

		/// <summary>
		/// The replay search : ReplaySearchMode, unused, unused
		/// </summary>
		ReplaySearch,

		/// <summary>
		/// The state of the replay set : ReplayStateMode, unused, unused
		/// </summary>
		ReplaySetState,

		/// <summary>
		/// The reload textures : ReloadTexturesMode, carIdx, unused
		/// </summary>
		ReloadTextures,

		/// <summary>
		/// The chat comand : ChatCommandMode, subCommand, unused
		/// </summary>
		ChatComand,

		/// <summary>
		/// The pit command : PitCommandMode, parameter
		/// </summary>
		PitCommand
	};
}

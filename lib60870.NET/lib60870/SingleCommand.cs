/*
 *  Copyright 2016 MZ Automation GmbH
 *
 *  This file is part of lib60870.NET
 *
 *  lib60870.NET is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  lib60870.NET is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with lib60870.NET.  If not, see <http://www.gnu.org/licenses/>.
 *
 *  See COPYING file for the complete license text.
 */

using System;

namespace lib60870
{
	public class SingleCommand : InformationObject
	{

		private byte sco;

		public SingleCommand (int ioa, bool command, bool selectCommand, int qu) : base(ioa)
		{
			sco = (byte) ((qu & 0x1f) * 4);

			if (command) sco |= 0x01;

			if (selectCommand)
				sco |= 0x80;
		}

		public SingleCommand (ConnectionParameters parameters, byte[] msg, int startIndex) :
			base(parameters, msg, startIndex)
		{
			startIndex += parameters.SizeOfIOA; /* skip IOA */

			sco = msg [startIndex++];
		}

		public override void Encode(Frame frame, ConnectionParameters parameters) {
			base.Encode(frame, parameters);

			frame.SetNextByte (sco);
		}

		public int QU {
			get {
				return ((sco & 0x7c) / 4);
			}
		}

		/// <summary>
		/// Gets the state (off - false / on - true) of this command
		/// </summary>
		/// <value><c>true</c> if on; otherwise, <c>false</c>.</value>
		public bool State {
			get {
				return ((sco & 0x01) == 0x01);
			}
		}

		/// <summary>
		/// Indicates if the command is a select or an execute command
		/// </summary>
		/// <value><c>true</c> if select; execute, <c>false</c>.</value>
		public bool Select {
			get {
				return ((sco & 0x80) == 0x80);
			}
		}

		public override string ToString ()
		{
			return string.Format ("[SingleCommand: QU={0}, State={1}, Select={2}]", QU, State, Select);
		}
		
	}

	public class SingleCommandWithCP56Time2a : SingleCommand
	{
		private CP56Time2a timestamp;

		public SingleCommandWithCP56Time2a (int ioa, bool command, bool selectCommand, int qu, CP56Time2a timestamp) : 
			base(ioa, command, selectCommand, qu)
		{
			this.timestamp = timestamp;
		}

		public SingleCommandWithCP56Time2a (ConnectionParameters parameters, byte[] msg, int startIndex) :
			base(parameters, msg, startIndex)
		{
			startIndex += parameters.SizeOfIOA + 1; /* skip IOA + SCQ*/

			timestamp = new CP56Time2a (msg, startIndex);
		}

		public override void Encode(Frame frame, ConnectionParameters parameters) {
			base.Encode(frame, parameters);

			frame.AppendBytes (timestamp.GetEncodedValue ());
		}

		public CP56Time2a Timestamp {
			get {
				return timestamp;
			}
		}


	}

	public class DoubleCommand : InformationObject
	{
		public static int OFF = 1;
		public static int ON = 2;

		private byte dcq;

		public DoubleCommand (int ioa, int command, bool select, int quality) : base(ioa)
		{
			dcq = (byte) (command & 0x03);
			dcq += (byte)((quality & 0x1f) * 4);

			if (select)
				dcq |= 0x80;
		}

		public DoubleCommand (ConnectionParameters parameters, byte[] msg, int startIndex) :
			base(parameters, msg, startIndex)
		{
			startIndex += parameters.SizeOfIOA; /* skip IOA */

			dcq = msg [startIndex++];
		}

		public override void Encode(Frame frame, ConnectionParameters parameters) {
			base.Encode(frame, parameters);

			frame.SetNextByte (dcq);
		}

		public int QU {
			get {
				return ((dcq & 0x7c) / 4);
			}
		}

		public int State {
			get {
				return (dcq & 0x03);
			}
		}

		public bool Select {
			get {
				return ((dcq & 0x80) == 0x80);
			}
		}
	}

	public class DoubleCommandWithCP56Time2a : DoubleCommand
	{
		private CP56Time2a timestamp;

		public DoubleCommandWithCP56Time2a (int ioa, int command, bool select, int quality, CP56Time2a timestamp) : 
			base(ioa, command, select, quality)
		{
			this.timestamp = timestamp;
		}

		public DoubleCommandWithCP56Time2a (ConnectionParameters parameters, byte[] msg, int startIndex) :
		base(parameters, msg, startIndex)
		{
			startIndex += parameters.SizeOfIOA + 1; /* skip IOA + DCQ*/

			timestamp = new CP56Time2a (msg, startIndex);
		}

		public override void Encode(Frame frame, ConnectionParameters parameters) {
			base.Encode(frame, parameters);

			frame.AppendBytes (timestamp.GetEncodedValue ());
		}

		public CP56Time2a Timestamp {
			get {
				return timestamp;
			}
		}
	}


	public class StepCommand : DoubleCommand 
	{
		public static int LOWER = 1;
		public static int HIGHER = 2;

		public StepCommand (int ioa, int command, bool select, int quality) : base(ioa, command, select, quality)
		{
		}

		public StepCommand (ConnectionParameters parameters, byte[] msg, int startIndex) :
			base(parameters, msg, startIndex)
		{
		}
	}



}


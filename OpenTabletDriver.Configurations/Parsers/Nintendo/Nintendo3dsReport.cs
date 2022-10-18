using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using OpenTabletDriver.Plugin.Tablet;

namespace OpenTabletDriver.Configurations.Parsers.Nintendo
{
    public struct Nintendo3dsReport : ITabletReport
    {
        internal Nintendo3dsReport(byte[] report)
        {
            Raw = report;

            Position = new Vector2
            {
                X = BitConverter.ToUInt16(report, 0),
                Y = BitConverter.ToUInt16(report, 2)
            };
            Pressure = (Position.X == 0 && Position.Y == 0) ? 0u : 1u;
            PenButtons = new bool[0];
        }

        public byte[] Raw { set; get; }
        public Vector2 Position { set; get; }
        public uint Pressure { set; get; }
        public bool[] PenButtons { get; set; }
    }
}

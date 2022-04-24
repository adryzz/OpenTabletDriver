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
                X = Unsafe.ReadUnaligned<ushort>(ref report[0]) | (report[1] << 16),
                Y = Unsafe.ReadUnaligned<ushort>(ref report[2]) | (report[3] << 16)
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

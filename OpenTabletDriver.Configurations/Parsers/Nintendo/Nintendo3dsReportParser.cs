using System.Numerics;
using OpenTabletDriver.Plugin.Tablet;

namespace OpenTabletDriver.Configurations.Parsers.Nintendo
{
    public class Nintendo3dsReportParser : IReportParser<IDeviceReport>
    {
        static Vector2 oldPos = Vector2.Zero;
        public IDeviceReport Parse(byte[] data)
        {
            var report = new Nintendo3dsReport(data);
            if (report.Position == Vector2.Zero)
            {
                report.Position = oldPos;
            }

            oldPos = report.Position;

            return report;
        }
    }
}

using System;
using System.Buffers;
using System.IO;
using System.Threading;
using OpenTabletDriver.Plugin.Devices;

namespace OpenTabletDriver.Devices.Nintendo3ds
{
    public class Nintendo3dsInterface : IDeviceEndpoint
    {
        public unsafe Nintendo3dsInterface(string devicePath)
        {
            DevicePath = devicePath;
        }

        public int ProductID { get; private set; } = 0;

        public int VendorID { get; private set; } = 0;

        public int InputReportLength { get; private set; } = 4;

        public int OutputReportLength { get; private set; } = 4;

        public int FeatureReportLength => 0; // requires parsing report descriptor to determine feature report length

        public string Manufacturer { get; private set; } = "Nintendo";

        public string ProductName { get; private set; } = "Nintendo 3DS XL";

        public string FriendlyName => ProductName;

        public string SerialNumber { get; private set; } = "SPR-001";

        public string DevicePath { get; }

        public bool CanOpen => true;

        public unsafe string GetDeviceString(byte index) => "null";

        public IDeviceEndpointStream Open()
        {
            return new Nintendo3dsInterfaceStream(new WeakReference<Nintendo3dsInterface>(this));
        }
    }
}

using System;
using System.Collections.Generic;

namespace OpenTabletDriver.Plugin.Devices
{
    public interface ILegacyDeviceHub
    {
        public bool TryGetDevice(string path, out IDeviceEndpoint endpoint);

        public bool CanEnumeratePorts { get; }

        public string[] EnumeratePorts();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Devices;

namespace OpenTabletDriver.Devices.Nintendo3ds
{
    [DeviceHub]
    public class Nintendo3dsDeviceHub : IDeviceHub
    {
        public Nintendo3dsDeviceHub()
        {
            DevicesChanged?.Invoke(this, new DevicesChangedEventArgs(null, GetDevices()));
        }

        public event EventHandler<DevicesChangedEventArgs> DevicesChanged;

        public IEnumerable<IDeviceEndpoint> GetDevices()
        {
            var v = new List<Nintendo3dsInterface>();
            v.Add(new Nintendo3dsInterface("tcp://10.0.0.5:5000"));
            return v;
        }
    }
}

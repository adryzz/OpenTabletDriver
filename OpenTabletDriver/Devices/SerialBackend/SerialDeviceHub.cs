using System.IO.Ports;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Devices;

namespace OpenTabletDriver.Devices.SerialBackend
{
    [DeviceHub]
    public class SerialDevoceHub : ILegacyDeviceHub
    {
        public SerialDevoceHub()
        {
        }

        public bool CanEnumeratePorts => true;

        public string[] EnumeratePorts() => SerialPort.GetPortNames();

        public bool TryGetDevice(string path, out IDeviceEndpoint endpoint)
        {
            try
            {
                endpoint = new SerialInterface(path);
                return true;
            }
            catch
            {
                endpoint = null;
                return false;
            }
        }
    }
}

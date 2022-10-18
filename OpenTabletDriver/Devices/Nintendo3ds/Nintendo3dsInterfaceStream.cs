using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using OpenTabletDriver.Devices.Nintendo3ds;
using OpenTabletDriver.Plugin.Devices;

namespace OpenTabletDriver.Devices.Nintendo3ds
{
    public unsafe class Nintendo3dsInterfaceStream : IDeviceEndpointStream
    {
        private readonly WeakReference<Nintendo3dsInterface> parentInterface;

        private int reportSize = 4;

        private Socket streamSocket;

        public Nintendo3dsInterfaceStream(WeakReference<Nintendo3dsInterface> parentInterface)
        {
            this.parentInterface = parentInterface;

            if (!parentInterface.TryGetTarget(out var dsInterface))
                throw new InvalidOperationException("Weak reference to parent interface is unexpectedly invalid");

            reportSize = dsInterface.OutputReportLength;
            Uri uri = new Uri(dsInterface.DevicePath);
            IPEndPoint endpoint = new IPEndPoint(new IPAddress(new byte[] {10, 0, 0, 5}), uri.Port);
            streamSocket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            streamSocket.NoDelay = true;
            streamSocket.ExclusiveAddressUse = true;
            streamSocket.ReceiveBufferSize = 4;
            streamSocket.Connect(endpoint);
        }

        public byte[] Read()
        {
            byte[] buf = new byte[reportSize];
            streamSocket.Receive(buf);
            //Console.WriteLine(BitConverter.ToString(buf).Replace("-",""));
            return buf;
        }

        public void Write(byte[] buffer)
        {
            streamSocket.Send(buffer);
        }

        public unsafe void GetFeature(byte[] buffer)
        {
        }

        public void SetFeature(byte[] buffer)
        {
        }

        public void Dispose()
        {
            streamSocket.Dispose();
        }
    }
}

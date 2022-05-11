using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Components;
using OpenTabletDriver.Plugin.Devices;

#nullable enable

namespace OpenTabletDriver.Devices
{
    public class RootHub : ICompositeDeviceHub, IDeviceHub
    {
        public RootHub(IDeviceHubsProvider hubsProvider)
        {
            internalHubs = hubsProvider.DeviceHubs.ToHashSet();
            hubs = new HashSet<IDeviceHub>(internalHubs);

            internalLegacyHubs = hubsProvider.LegacyDeviceHubs.ToHashSet();
            legacyHubs = new HashSet<ILegacyDeviceHub>(internalLegacyHubs);
            ForceEnumeration();

            foreach (var hub in hubs)
            {
                HookDeviceNotification(hub);
            }

            foreach (var hub in hubs)
            {
                HookDeviceNotification(hub);
            }

            Log.Write(nameof(RootHub), $"Initialized internal child hubs: {string.Join(", ", hubs.Select(h => h.GetType().Name))}", LogLevel.Debug);
        }

        private readonly object syncObject = new();
        private readonly HashSet<IDeviceHub> internalHubs;
        private readonly HashSet<IDeviceHub> hubs;

        private readonly HashSet<ILegacyDeviceHub> internalLegacyHubs;
        private readonly HashSet<ILegacyDeviceHub> legacyHubs;

        private List<IDeviceEndpoint>? oldEndpoints;
        private readonly List<IDeviceEndpoint> endpoints = new();
        private long version;
        private int currentlyDebouncing;
        private IServiceProvider? serviceProvider;

        public event EventHandler<DevicesChangedEventArgs>? DevicesChanged;

        public IEnumerable<IDeviceHub> DeviceHubs => hubs;

        public IEnumerable<ILegacyDeviceHub> LegacyDeviceHubs => legacyHubs;

        public IEnumerable<string> legacyPortNames;

        public IEnumerable<string> LegacyPortNames => legacyPortNames;

        public static RootHub WithProvider(IServiceProvider provider)
        {
            return new RootHub(provider.GetRequiredService<IDeviceHubsProvider>())
                .RegisterServiceProvider(provider);
        }

        public IEnumerable<IDeviceEndpoint> GetDevices()
        {
            return endpoints;
        }

        public void ConnectDeviceHub<T>() where T : IDeviceHub
        {
            if (serviceProvider == null)
                throw new InvalidOperationException("Cannot instantiate device hubs without a service provider");

            ConnectDeviceHub(ActivatorUtilities.CreateInstance<T>(serviceProvider!));
        }

        public void ConnectDeviceHub(IDeviceHub rootHub)
        {
            Log.Write(nameof(RootHub), $"Connecting hub: {rootHub.GetType().Name}", LogLevel.Debug);
            if (hubs.Add(rootHub))
            {
                CommitHubChange();
                HookDeviceNotification(rootHub);
            }
            else
            {
                Log.Write(nameof(RootHub), $"Connection failed, {rootHub.GetType().Name} is already connected", LogLevel.Debug);
            }
        }

        public void DisconnectDeviceHub<T>() where T : IDeviceHub
        {
            foreach (var hub in DeviceHubs.Where(static t => t.GetType().IsAssignableTo(typeof(T))))
            {
                DisconnectDeviceHub(hub);
            }
        }

        public void DisconnectDeviceHub(IDeviceHub rootHub)
        {
            Log.Write(nameof(RootHub), $"Disconnecting hub: {rootHub.GetType().Name}", LogLevel.Debug);
            if (hubs.Remove(rootHub))
            {
                CommitHubChange();
                UnhookDeviceNotification(rootHub);
            }
            else
            {
                Log.Write(nameof(RootHub), $"Disconnection failed, {rootHub.GetType().Name} is not a connected hub", LogLevel.Debug);
            }
        }

        private async void OnDevicesChanged(object? sender, DevicesChangedEventArgs eventArgs)
        {
            var lastVersion = Interlocked.Increment(ref version);
            if (Interlocked.Increment(ref currentlyDebouncing) == 1)
            {
                // This event is the first of a potential sequence of events, copy old endpoint list
                oldEndpoints = new List<IDeviceEndpoint>(endpoints);
            }

            lock (syncObject)
            {
                endpoints.RemoveAll(e => eventArgs.Removals.Contains(e, DevicesChangedEventArgs.Comparer));
                endpoints.AddRange(eventArgs.Additions);
            }

            await Task.Delay(20);

            if (version == lastVersion)
            {
                Log.Write(nameof(RootHub), "Invoking DevicesChanged", LogLevel.Debug);
                DevicesChanged?.Invoke(this, new DevicesChangedEventArgs(oldEndpoints, endpoints));
            }
            else
            {
                Log.Write(nameof(RootHub), $"Debounced {sender?.GetType().Name}'s DevicesChanged event");
            }

            Interlocked.Decrement(ref currentlyDebouncing);
        }

        private void HookDeviceNotification(IDeviceHub rootHub)
        {
            rootHub.DevicesChanged += HandleHubDeviceNotification;
        }

        private void UnhookDeviceNotification(IDeviceHub rootHub)
        {
            rootHub.DevicesChanged -= HandleHubDeviceNotification;
        }

        private void HandleHubDeviceNotification(object? sender, DevicesChangedEventArgs eventArgs)
        {
            if (eventArgs.Changes.Any())
            {
                Log.Debug(sender?.GetType().Name, $"Changes: {eventArgs.Changes.Count()}, Add: {eventArgs.Additions.Count()}, Remove: {eventArgs.Removals.Count()}");
                OnDevicesChanged(sender, eventArgs);
            }
        }

        private void CommitHubChange()
        {
            oldEndpoints = new List<IDeviceEndpoint>(endpoints);
            ForceEnumeration();
            DevicesChanged?.Invoke(this, new DevicesChangedEventArgs(endpoints, oldEndpoints));
            oldEndpoints = null;
        }

        private void ForceEnumeration()
        {
            endpoints.Clear();
            endpoints.AddRange(hubs.SelectMany(h => h.GetDevices()));
            //legacyPortNames = legacyHubs.Where(h => h.CanEnumeratePorts).SelectMany(h => h.EnumeratePorts());
        }

        private RootHub RegisterServiceProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            return this;
        }
    }
}

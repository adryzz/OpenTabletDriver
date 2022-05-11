using System;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using Microsoft.VisualStudio.Threading;
using OpenTabletDriver.Devices;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.UX.Controls.Generic;

namespace OpenTabletDriver.UX.Windows.Tablet
{
    public class LegacyTabletConnectionWindow : DesktopForm
    {
        public LegacyTabletConnectionWindow()
            : base(Application.Instance.MainForm)
        {
            Title = "Connect legacy tablet...";
            Icon = App.Logo.WithSize(App.Logo.Size);
            ClientSize = new Size(300, 250);

            connectButton = new Button
            {
                Text = "Connect",
            };

            connectButton.Click += async (_, _) => await App.Driver.Instance.ConnectLegacyTablet(new Uri(devicePathText.Text), tablet.SelectedItem, reconnectBox.Checked.Value);

            devicePathText = new ComboBox();

            tablet = new DropDown<TabletConfiguration>();

            tablet.DataStore = App.Driver.Instance.GetSupportedTablets().Result.Where(x => x.Attributes.ContainsKey("isLegacy"));

            devicePathText.DataStore = App.Driver.Instance.GetLegacyPorts().Result;

            devicePathGroup = new Group("Device path", devicePathText, Orientation.Vertical, false);

            tabletGroup = new Group("Tablet", tablet, Orientation.Vertical, false);

            reconnectBox = new CheckBox
            {
                Text = "Remember tablet"
            };

            Content = new StackLayout
            {
                Padding = 5,
                Spacing = 5,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Items =
                {
                    devicePathGroup,
                    tabletGroup,
                    reconnectBox,
                    connectButton
                }
            };
        }

        private readonly ComboBox devicePathText;
        private readonly CheckBox reconnectBox;
        private readonly Button connectButton;

        private readonly DropDown<TabletConfiguration> tablet;

        private readonly Group devicePathGroup, tabletGroup, portTypeGroup;
    }
}

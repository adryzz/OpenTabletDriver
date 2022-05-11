using Eto.Drawing;
using Eto.Forms;
using Microsoft.VisualStudio.Threading;
using OpenTabletDriver.Devices;
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
            ClientSize = new Size(300, 300);

            connectButton = new Button
            {
                Text = "Connect",
            };

            /*connectButton.Click += async (_, _) => await Connect(devicePathText.Text,
                (s) => deviceStringText.Text = s,
                (e) => MessageBox.Show($"Error: {e.Message}", MessageBoxType.Error),
                () => MessageBox.Show(OperationTimedOut)
            );*/

            devicePathText = new ComboBox();

            tablet = new DropDown<string>();

            devicePathText.DataStore = App.Driver.Instance.GetLegacyPorts().Result;

            devicePathGroup = new Group("Device path", devicePathText, Orientation.Vertical, false);

            tabletGroup = new Group("Tablet", tablet, Orientation.Vertical, false);

            portType = new EnumDropDown<LegacyHubType>();

            portTypeGroup = new Group("Port type", portType, Orientation.Vertical, false);

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
                    portTypeGroup,
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

        private readonly DropDown<string> tablet;

        private readonly EnumDropDown<LegacyHubType> portType;

        private readonly Group devicePathGroup, tabletGroup, portTypeGroup;
    }
}

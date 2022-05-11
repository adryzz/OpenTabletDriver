using Eto.Drawing;
using Eto.Forms;
using OpenTabletDriver.Desktop.Contracts;
using OpenTabletDriver.UX.Components;
using OpenTabletDriver.Devices;

namespace OpenTabletDriver.UX.Windows
{
    public class LegacyTabletConnectionWindow : DesktopForm
    {
        private readonly IDriverDaemon _daemon;
        private readonly App _app;

        public LegacyTabletConnectionWindow(IDriverDaemon daemon, App app)
        {
            _daemon = daemon;
            _app = app;

            Title = "Connect legacy tablet...";

            //Icon = App.Logo.WithSize(App.Logo.Size);
            ClientSize = new Size(300, 250);

            var connectButton = new Button
            {
                Text = "Connect",
            };

            /*connectButton.Click += async (_, _) => await Connect(devicePathText.Text,
                (s) => deviceStringText.Text = s,
                (e) => MessageBox.Show($"Error: {e.Message}", MessageBoxType.Error),
                () => MessageBox.Show(OperationTimedOut)
            );*/

            devicePathText = new ComboBox();

            tablet = new DropDown();

            // Orientation.Vertical
            devicePathGroup = new GroupBox
            {
                Text = "Device path",
                Content = devicePathText
            };

            // Orientation.Vertical
            tabletGroup = new GroupBox
            {
                Text = "Tablet",
                Content = tablet
            };



            portType = new EnumDropDown<LegacyHubType>();

            // Orientation.Vertical
            portTypeGroup = new GroupBox
            {
                Text = "Port type",
                Content = portType
            };

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

        private readonly DropDown tablet;

        private readonly GroupBox devicePathGroup, tabletGroup, portTypeGroup;

        private readonly EnumDropDown<LegacyHubType> portType;
    }
}

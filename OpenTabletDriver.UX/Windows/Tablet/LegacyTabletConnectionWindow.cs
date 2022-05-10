using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Eto.Drawing;
using Eto.Forms;
using OpenTabletDriver.Desktop;
using OpenTabletDriver.Desktop.Contracts;
using OpenTabletDriver.Desktop.RPC;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Timing;
using OpenTabletDriver.UX.Controls.Generic;
using OpenTabletDriver.UX.Tools;

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

            tablet = new DropDown<string>();

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

        private readonly DropDown<string> tablet;

        private readonly Group devicePathGroup, tabletGroup;
    }
}
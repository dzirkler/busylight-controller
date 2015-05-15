using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
//using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Plenom.Components.Busylight.Sdk;
using System.Drawing;

namespace BusyLightController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();
        int TimerCount = 0;

        private BusylightColor GetBusyLightColor(Color color)
        {
            BusylightColor blColor = new BusylightColor();

            blColor.RedRgbValue = ConvertByteToPercent(color.R);
            blColor.GreenRgbValue = ConvertByteToPercent(color.G);
            blColor.BlueRgbValue = ConvertByteToPercent(color.B);

            return blColor;
        }

        private int ConvertByteToPercent(byte value)
        {
            // Cast to Float
            // divide by max byte (as floating point division)
            // multiply by 100 to convert to percentage
            // and cast back to int, to drop the decimals
            return (int)(((float)value / 255f) * 100);
        }

        private Color ColorOff
        {
            get
            {
                // Off
                return Color.FromArgb(0, 0, 0);
            }
        }

        private Color ColorAway
        {
            get
            {
                // Off (default is yellow, commented below)
                return ColorOff;
                //return Color.FromArgb(64, 64, 0);

            }
        }

        private Color ColorFree
        {
            get
            {
                // Green
                return Color.FromArgb(0, 127, 0);
            }
        }

        private Color ColorDND
        {
            get
            {
                // Purple
                return Color.FromArgb(128, 0, 64);
            }
        }

        private Color ColorBusy
        {
            get
            {
                // Red
                return Color.FromArgb(128, 0, 0);
            }
        }

        private Color ColorInACall
        {
            get
            {
                // Red (should be pulsed)
                return Color.FromArgb(128, 0, 0);
            }
        }

        private Color ColorIncomingCall
        {
            get
            {
                // Blue (should be pulsed)
                return Color.FromArgb(0, 0, 128);
            }
        }

        private PulseSequence GetPulse(BusylightColor color)
        {
            // One Bright flash every four measures
            PulseSequence seq = new PulseSequence();
            seq.Color = color;
            seq.Step1 = 50;
            seq.Step2 = 50;
            seq.Step3 = 50;
            seq.Step4 = 100;
            seq.Step5 = 50;
            seq.Step6 = 50;
            seq.Step7 = 50;
            seq.Step8 = 100;

            return seq;
        }

        private void SetBusyLightColor(Color color)
        {
            var ctlr = new BusylightLyncController();
            ctlr.Light(GetBusyLightColor(color));
            SetColorChip(color);
        }

        private void SetBusyLightPulse(Color color)
        {
            var ctlr = new BusylightLyncController();
            ctlr.Pulse(GetPulse(GetBusyLightColor(color)));
            SetColorChip(color, true);
        }

        private void SetColorChip(Color color)
        {
            SetColorChip(color, false);
        }

        private void SetColorChip(Color color, bool pulse)
        {
            var clr = new System.Windows.Media.Color();
            clr.A = 255;
            clr.R = color.R;
            clr.G = color.G;
            clr.B = color.B;

            ColorChip.Color = clr;

            //if (false) //pulse)
            //{
            //    for (int x = 0; x < 10; x++)
            //    {
            //        System.Threading.Thread.Sleep(1000);
            //        clr.A = 128;
            //        ColorChip.Color = clr;

            //        System.Threading.Thread.Sleep(500);
            //        clr.A = 255;
            //        ColorChip.Color = clr;

            //    }
            //}

        }

        private void UpdateBusyLight()
        {
            string status = StatusesBox.SelectedItem.ToString();

            switch (status)
            {
                case "Free":
                    SetBusyLightColor(ColorFree);
                    break;
                case "Away":
                    SetBusyLightColor(ColorAway);
                    break;
                case "Busy":
                    SetBusyLightColor(ColorBusy);
                    break;
                case "Do Not Disturb":
                    SetBusyLightColor(ColorDND);
                    break;
                case "In A Call":
                    SetBusyLightPulse(ColorInACall);
                    break;
                case "Incoming Call":
                    SetBusyLightPulse(ColorIncomingCall);
                    break;
                case "Off":
                default:
                    SetBusyLightColor(ColorOff);
                    break;
            }

        }

        private void StatusesBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateBusyLight();
        }

        private void StatusesBox_Loaded(object sender, RoutedEventArgs e)
        {
            var statusesBox = (ListBox)sender;

            List<string> statuses = new List<string>() { "Off", "Free", "Away", "Busy", "Do Not Disturb", "In A Call", "Incoming Call" };
            statusesBox.ItemsSource = statuses;
            statusesBox.SelectedIndex = 0;
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (TimerCount % 30 == 0)
            {
                UpdateBusyLight();
                TimerCount = 0;
            }

            CountDownItem.Content = string.Format("Countdown to refresh: {0}s", (30 - TimerCount));
            TimerCount++;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Tick += Timer_Tick;

            if ((bool)EnforceStatus.IsChecked)
            {
                StartTimer();
            }
        }

        private void StartTimer()
        {
            TimerCount = 0;
            Timer.Start();
        }

        private void EnforceStatus_Checked(object sender, RoutedEventArgs e)
        {
            StartTimer();
        }

        private void EnforceStatus_Unchecked(object sender, RoutedEventArgs e)
        {
            Timer.Stop();
            CountDownItem.Content = String.Empty;
        }

        private void CountDownItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (new ColorMixer()).ShowDialog();
        }

    }
}

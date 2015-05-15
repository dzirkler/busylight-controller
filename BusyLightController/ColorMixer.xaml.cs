using Plenom.Components.Busylight.Sdk;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BusyLightController
{
    /// <summary>
    /// Interaction logic for ColorMixer.xaml
    /// </summary>
    public partial class ColorMixer : Window
    {

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

        private void SetBusyLightColor(Color color)
        {
            var ctlr = new BusylightLyncController();
            ctlr.Light(GetBusyLightColor(color));
        }

        public ColorMixer()
        {
            InitializeComponent();
        }

        private void ColorPickerStandard_SelectedColorChanged(object sender, ColorPicker.EventArgs<Color> e)
        {
            SetBusyLightColor(e.Value);

        }
    }
}

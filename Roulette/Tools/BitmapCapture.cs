using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Roulette.Tools
{
    class BitmapCapture
    {
        public static Bitmap GetWindowCapture(Control control)
        {
            try
            {
                Graphics g1 = control.CreateGraphics();
                Bitmap myImage = new Bitmap(control.Width, control.Height, g1);
                Graphics g2 = Graphics.FromImage(myImage);
                IntPtr dc1 = g1.GetHdc();
                IntPtr dc2 = g2.GetHdc();
                WinApi.BitBlt(dc2, 0, 0, control.Width, control.Height, dc1, 0, 0, 0x00CC0020);
                g1.ReleaseHdc(dc1);
                g2.ReleaseHdc(dc2);
                g1.Dispose();
                g2.Dispose();
                return myImage;
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}

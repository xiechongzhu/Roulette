using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roulette.Tools
{
    class ImageOperator
    {
        public static Color GetImageRgb(Bitmap image, Int32 x, Int32 y)
        {
            Color color = image.GetPixel(x, y);
            return color;
        }
    }
}

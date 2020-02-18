using Roulette.Config;
using Roulette.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Roulette.Gamer
{
    public class GamerAG : GamerBase
    {
        public GamerAG(MainForm mainForm) : base(mainForm)
        {
        }

        protected override void ReEnter()
        {
            BrowserClick(0, 995, 607);
            TablePoints tablePoints = Tables.GetInstance().GetTablePoints(setting.tableName);
            BrowserClick(10000, tablePoints.reEnterPoint.X, tablePoints.reEnterPoint.Y);
        }

        protected override void Exit()
        {
            TablePoints tablePoints = Tables.GetInstance().GetTablePoints(setting.tableName);
            BrowserClick(0, tablePoints.exitPoint.X, tablePoints.exitPoint.Y);
        }

        protected override bool isStartImage(Bitmap image)
        {
            //用开局的"准备下注"黄色字体判断
            Color color;
            List<Color> colorList = new List<Color>();
            //389,216 ===>255,255,1
            color = ImageOperator.GetImageRgb(image, 479, 288);
            colorList.Add(color);
            //448,215 ===> 255,251,5
            color = ImageOperator.GetImageRgb(image, 508, 286);
            colorList.Add(color);
            //430,220 ===> 246,247,0
            color = ImageOperator.GetImageRgb(image, 598, 288);
            colorList.Add(color);

            foreach (Color item in colorList)
            {
                Int32 r = item.R;
                Int32 g = item.G;
                Int32 b = item.B;
                if (r < 220 || g < 220 || b > 50)
                {
                    return false;
                }
            }
            if(resultHistory == null)
            {
                resultHistory = FirstGetResultHistory(image);
                Log(String.Format("当前统计:红={0} 黑={1} 绿={2}", resultHistory.CountRed, resultHistory.CountBlack, resultHistory.CountGreen));
            }
            return true;
        }

        protected override bool isEndImage(Bitmap image, out GameResult gameResult)
        {
            gameResult = GameResult.RESULT_UNKNOW;
            Color colorTop = ImageOperator.GetImageRgb(image, 140, 70);
            if(colorTop.R < 250)
            {
                return false;
            }
            Color color1 = ImageOperator.GetImageRgb(image, 103, 206);
            Color color2 = ImageOperator.GetImageRgb(image, 136, 205);
            Color color3 = ImageOperator.GetImageRgb(image, 146, 179);

            if (color1.R < 10 && color1.G < 10 && color1.B < 10 && color2.R < 10 && color2.G < 10 && color2.B < 10
                && ColorInRect(image, Color.FromArgb(255, 255, 255), new Rectangle(81, 147, 70, 70)))
            {
                gameResult = GameResult.RESULT_BLACK;
                return true;
            }
            else if(color1.R > 215 && color2.R > 215)
            {
                gameResult = GameResult.RESULT_RED;
                return true;
            }
            else if(color1.G > 150 && color2.G > 150)
            {
                gameResult = GameResult.RESULT_GREEN;
                return true;
            }
            return false;
        }

        protected override bool isOutRoom(Bitmap image)
        {
            //240 93 41
            Color color1 = ImageOperator.GetImageRgb(image, 17, 94);
            //0 145 189
            Color color2 = ImageOperator.GetImageRgb(image, 41, 73);
            //0 79 0
            Color color3 = ImageOperator.GetImageRgb(image, 860, 87);
            if (color1.R > 230 && color1.G < 100 && color1.B < 50
                && color2.R < 10 && color2.G < 150 && color2.B < 200
                && color3.R > 10 && color3.G < 70 && color3.B > 10)
            {
                return true;
            }
            return false;
        }

        protected override ResultHistory FirstGetResultHistory(Bitmap image)
        {
            ResultHistory resultHistory = new ResultHistory();
            List<Point> pointList = new List<Point>();

            int startX = 80, startY = 551;
            for(int col = 0; col < 10; col++)
            {
                for(int row = 0; row < 6; row++)
                {
                    Color color = ImageOperator.GetImageRgb(image, startX + (16 * col), startY + (16 * row));
                    if (color.R >= 200 && color.G < 40 && color.B < 40)
                    {
                        resultHistory.CountRed++;
                    }
                    else if (color.G >= 120 && color.R < 80 && color.B < 80)
                    {
                        resultHistory.CountGreen++;
                    }
                    else if (color.R < 30 && color.G < 30 && color.B < 30)
                    {
                        resultHistory.CountBlack++;
                    }
                }
            }
            return resultHistory;
        }

        protected override void CloseVedio()
        {
            BrowserClick(0, 805, 619);
        }

        protected override bool isVedioOn(Bitmap image)
        {
            Color color = ImageOperator.GetImageRgb(image, 805, 621);
            if(color.R < 50 && color.G > 120 && color.B < 5)
            {
                return true;
            }
            return false;
        }

        protected bool ColorInRect(Bitmap bitmap, Color color, Rectangle rectangle)
        {
            for(int col = 0; col < rectangle.Width; ++col)
            {
                for(int row = 0; row < rectangle.Height; ++row)
                {
                    Color tempColor = ImageOperator.GetImageRgb(bitmap, rectangle.X + col, rectangle.Y + row);
                    if(Math.Abs(color.R - tempColor.R) < 10 && Math.Abs(color.G - tempColor.G) < 10 && Math.Abs(color.B - tempColor.B) < 10)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

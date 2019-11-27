using Roulette.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roulette.Gamer
{
    class GamerAG : GamerBase
    {
        protected Image currentImage;
        public GamerAG(MainForm mainForm) : base(mainForm)
        {
        }

        protected override Tuple<GameState, GameResult> InternalParseImage(Image image)
        {
            currentImage = (Image)image.Clone();
            GameState gameState = GameState.GAME_UNKNOW;
            GameResult gameResult = GameResult.RESULT_UNKNOW;
            if (isStartImage(image))
            {
                gameState = GameState.GAME_START;
                gameResult = GameResult.RESULT_UNKNOW;
            }
            else if (isEndImage(image, out gameResult))
            {
                AddResult(gameResult);
                gameState = GameState.GAME_END;
            }

            image.Dispose();
            return new Tuple<GameState, GameResult>(gameState, gameResult);
        }

        private bool isStartImage(Image image)
        {
            //用开局的"准备下注"黄色字体判断
            Color color;
            List<Color> colorList = new List<Color>();
            //389,216 ===>255,255,1
            color = ImageOperator.GetImageRgb(image, 479, 284);
            colorList.Add(color);
            //448,215 ===> 255,251,5
            color = ImageOperator.GetImageRgb(image, 507, 284);
            colorList.Add(color);
            //430,220 ===> 246,247,0
            color = ImageOperator.GetImageRgb(image, 597, 290);
            colorList.Add(color);

            foreach (Color item in colorList)
            {
                Int32 r = item.R;
                Int32 g = item.G;
                Int32 b = item.B;
                if (r < 220 || g < 220 || b > 20)
                {
                    return false;
                }
            }
            if(resultHistory == null)
            {
                resultHistory = FirstGetResultHistory(image);
            }
            return true;
        }

        private bool isEndImage(Image image, out GameResult gameResult)
        {
            gameResult = GameResult.RESULT_UNKNOW;
            Color color1 = ImageOperator.GetImageRgb(image, 100, 200);
            Color color2 = ImageOperator.GetImageRgb(image, 133, 198);

            if (color1.R < 10 && color1.G < 10 && color1.B < 10 && color2.R < 10 && color2.G < 10 && color2.B < 10)
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

        protected override bool isOutRoom()
        {
            if(currentImage == null)
            {
                return false;
            }
            List<Color> colors = new List<Color>
            {
                ImageOperator.GetImageRgb(currentImage, 179, 249),
                ImageOperator.GetImageRgb(currentImage, 350, 249),
                ImageOperator.GetImageRgb(currentImage, 267, 382),
                ImageOperator.GetImageRgb(currentImage, 267, 382)
            };
            foreach (Color color in colors)
            {
                if(color.R > 40 && color.G > 40 && color.B > 40)
                {
                    return false;
                }
            }
            return true;
        }

        protected override ResultHistory FirstGetResultHistory(Image image)
        {
            ResultHistory resultHistory = new ResultHistory();
            List<Point> pointList = new List<Point>();

            int startX = 80, startY = 551;
            for(int col = 0; col < 10; col++)
            {
                for(int row = 0; row < 6; row++)
                {
                    Color color = ImageOperator.GetImageRgb(image, startX + (16 * col), startY + (16 * row));
                    if(color.R >= 200 && color.G < 30 && color.B < 30)
                    {
                        resultHistory.CountRed++;
                    }
                    else if(color.G >= 120 && color.R < 70 && color.B < 70)
                    {
                        resultHistory.CountGreen++;
                    }
                    else if(color.R < 30 && color.G < 30 && color.B < 30)
                    {
                        resultHistory.CountBlack++;
                    }
                }
            }
            return resultHistory;
        }
    }
}

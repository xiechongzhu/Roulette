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
        public GamerAG(MainForm mainForm) : base(mainForm)
        {
        }

        protected override Tuple<GameState, GameResult> InternalParseImage(Image image)
        {
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

        protected override bool isOutRoom(Image image)
        {
            List<Color> colors = new List<Color>
            {
                ImageOperator.GetImageRgb(image, 179, 249),
                ImageOperator.GetImageRgb(image, 350, 249),
                ImageOperator.GetImageRgb(image, 267, 382),
                ImageOperator.GetImageRgb(image, 267, 382)
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
            return resultHistory;
        }
    }
}

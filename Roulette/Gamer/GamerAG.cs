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

        public override Tuple<GameState, GameResult> InternalParseImage(Image image)
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
                gameState = GameState.GAME_END;
            }

            image.Dispose();
            return new Tuple<GameState, GameResult>(gameState, gameResult);
        }

        private bool isStartImage(Image image)
        {
            return true;
        }

        private bool isEndImage(Image image, out GameResult gameResult)
        {
            gameResult = GameResult.RESULT_UNKNOW;
            return true;
        }
    }
}

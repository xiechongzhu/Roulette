using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roulette
{
    public enum GameResult
    {
        RESULT_UNKNOW,
        RESULT_RED,
        RESULT_BLACK,
        RESULT_GREEN
    }

    public enum GameState
    {
        GAME_START,     //新的一局开始，可以开始下注了
        GAME_END        //本局结束
    }

    public enum SupplierType
    {
        SUPPLIER_UNKNOW,
        SUPPLIER_AG
    }

    public class ResultHistory
    {
        public int CountBlack;
        public int CountRed;
        public int CountGreen;
    }

    public class SaveInfoItem
    {
        public int CountBlack;
        public int CountRed;
        public int CountGreen;
        public int diff;
        public int win;
        public int current_win;
        public int current_diff;
        public GameResult bet;
        public GameResult result;
        public bool isWin;
    }
}

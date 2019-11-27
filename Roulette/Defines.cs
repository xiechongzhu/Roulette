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
        GAME_UNKNOW,    //未知
        GAME_GOING,     //正在进行
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
        public UInt64 CountBlack;
        public UInt64 CountRed;
        public UInt64 CountGreen;
    }
}

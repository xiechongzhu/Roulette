using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roulette.Gamer
{
    abstract class GamerBase
    {
        protected StreamWriter logWriter;
        protected GameResult gameResult = GameResult.RESULT_UNKNOW;
        protected GameState gameState = GameState.GAME_UNKNOW;
        MainForm mainForm;
        protected bool isRunning = false;
        protected ResultHistory resultHistory;

        protected abstract Tuple<GameState, GameResult> InternalParseImage(Image image);

        public GamerBase(MainForm mainForm)
        {
            this.mainForm = mainForm;
            String strLogDir = AppDomain.CurrentDomain.BaseDirectory + "Log";
            Directory.CreateDirectory(strLogDir);
            logWriter = new StreamWriter(strLogDir + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log", true);
        }

        public void Start()
        {
            isRunning = true;
            Log("开始运行");
        }

        public void Stop()
        {
            isRunning = false;
            gameResult = GameResult.RESULT_UNKNOW;
            gameState = GameState.GAME_UNKNOW;
            resultHistory = null;
            Log("停止运行");
        }

        protected void BrowserClick(Int32 x, Int32 y)
        {
            mainForm.BrowserClick(x, y);
        }

        protected void SendLog(String logString)
        {
            mainForm.addLog(logString);
        }

        delegate Tuple<GameState, GameResult> SendImageDelegate(Image image);
        public void ParseImage(Image image)
        {
            SendImageDelegate d = new SendImageDelegate(InternalParseImage);
            d.BeginInvoke(image, SendImageDataCallBack, null);
        }

        protected void SendImageDataCallBack(IAsyncResult result)
        {
            if(isOutRoom())
            {
                Log("被踢出房间，准备重新进入");
            }
        }

        protected void AddResult(GameResult gameResult)
        {
            if(resultHistory == null)
            {
                return;
            }
            switch(gameResult)
            {
                case GameResult.RESULT_BLACK:
                    resultHistory.CountBlack += 1;
                    break;
                case GameResult.RESULT_RED:
                    resultHistory.CountRed += 1;
                    break;
                case GameResult.RESULT_GREEN:
                    resultHistory.CountGreen += 1;
                    break;
                default:
                    break;
            }
        }

        abstract protected bool isOutRoom();
        abstract protected ResultHistory FirstGetResultHistory(Image image);
        protected void Log(String strLog)
        {
            mainForm.addLog(strLog);
            logWriter.WriteLine("[" + DateTime.Now.ToString("u") + "]" + strLog);
            logWriter.Flush();
        }
    }
}

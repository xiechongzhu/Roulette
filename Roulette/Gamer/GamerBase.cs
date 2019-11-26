using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roulette.Gamer
{
    abstract class GamerBase
    {
        protected GameResult gameResult = GameResult.RESULT_UNKNOW;
        protected GameState gameState = GameState.GAME_UNKNOW;
        MainForm mainForm;
        bool isRunning = false;

        public abstract Tuple<GameState, GameResult> InternalParseImage(Image image);

        public GamerBase(MainForm mainForm)
        {
            this.mainForm = mainForm;
        }

        public void Start()
        {
            isRunning = true;
        }

        public void Stop()
        {
            isRunning = false;
            gameResult = GameResult.RESULT_UNKNOW;
            gameState = GameState.GAME_UNKNOW;
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

        }
    }
}

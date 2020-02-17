using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Roulette.Gamer
{
    public abstract class GamerBase
    {
        protected Setting setting;
        protected StreamWriter logWriter;
        protected GameResult gameResult = GameResult.RESULT_UNKNOW;
        protected GameState gameState = GameState.GAME_END;
        protected bool isPlayerOut = false;
        MainForm mainForm;
        protected bool isRunning = false;
        protected ResultHistory resultHistory;
        protected bool isInGamming = false;
        protected GameResult currentBet;
        List<SaveInfoItem> saveInfoList = new List<SaveInfoItem>();
        private int win = 0;

        public bool IsRunning
        {
            get { return isRunning; }
        }

        protected Tuple<GameState, GameResult> InternalParseImage(Bitmap image)
        {
            /*if(isVedioOn(image))
            {
                CloseVedio();
            }*/

            if (isStartImage(image) && gameState == GameState.GAME_END && !isPlayerOut)
            {
                Log("新的一局开始了");
                gameState = GameState.GAME_START;
                gameResult = GameResult.RESULT_UNKNOW;
                int diff = CalcDiff(resultHistory.CountRed, resultHistory.CountBlack, resultHistory.CountGreen, false);
                if (diff >= setting.diff && !isInGamming)
                {
                    Log(String.Format("当前差值(不包括绿色)={0},差值大于等于设定值,开始下注", CalcDiff(resultHistory.CountRed, resultHistory.CountBlack, resultHistory.CountGreen, false)));
                    isInGamming = true;
                }
                else if(win >= setting.win && isInGamming)
                {
                    Log(String.Format("已经赢了{0}把,停止下注", win));
                    isInGamming = false;
                    resultHistory = null;
                    win = 0;
                }
                if(isInGamming)
                {
                    if(resultHistory.CountRed > resultHistory.CountBlack)
                    {
                        currentBet = GameResult.RESULT_BLACK;
                    }
                    else
                    {
                        currentBet = GameResult.RESULT_RED;
                    }
                    Log(String.Format("当前相差值(包括绿色):{0},押:{1}", CalcDiff(resultHistory.CountRed, resultHistory.CountBlack, resultHistory.CountGreen, true), GameResultToString(currentBet)));
                }
                else
                {
                    Log(String.Format("当前差值(不包括绿色)={0},未达到下注条件，本局不下注", diff));
                }
                Exit();
            }
            else if (isEndImage(image, out gameResult) && gameState == GameState.GAME_START && !isPlayerOut)
            {
                Log("本局结束，结果为:" + GameResultToString(gameResult));
                AddResult(gameResult);
                if (resultHistory != null)
                {
                    Log(String.Format("当前统计:红={0} 黑={1} 绿={2}", resultHistory.CountRed, resultHistory.CountBlack, resultHistory.CountGreen));
                }
                gameState = GameState.GAME_END;
                if (isInGamming)
                {
                    Log(String.Format("本局结束，结果为:{0}, 押注:{1}, {2}了", GameResultToString(gameResult),
                        GameResultToString(currentBet), currentBet == gameResult ? "赢" : "输"));
                    win += (currentBet == gameResult ? 1 : -1);
                    saveInfoList.Add(new SaveInfoItem
                    {
                        CountBlack = resultHistory.CountBlack,
                        CountRed = resultHistory.CountRed,
                        CountGreen = resultHistory.CountGreen,
                        diff = setting.diff,
                        win = setting.win,
                        current_win = win,
                        current_diff = CalcDiff(resultHistory.CountRed, resultHistory.CountBlack, resultHistory.CountGreen, true),
                        bet = currentBet,
                        result = gameResult,
                        isWin = (currentBet == gameResult)
                    });
                }
            }
            else if (isOutRoom(image) && !isPlayerOut)
            {
                isPlayerOut = true;
                ReEnter();
            }

            image.Dispose();
            return new Tuple<GameState, GameResult>(gameState, gameResult);
        }

        protected String GameResultToString(GameResult result)
        {
            switch (result)
            {
                case GameResult.RESULT_BLACK:
                    return "黑";
                case GameResult.RESULT_GREEN:
                    return "绿";
                case GameResult.RESULT_RED:
                    return "红";
                default:
                    return "未知";
            }
        }

        public GamerBase(MainForm mainForm)
        {
            this.mainForm = mainForm;
        }

        public void SetSetting(Setting setting)
        {
            this.setting = setting;
        }

        public void Start()
        {
            String strLogDir = AppDomain.CurrentDomain.BaseDirectory + "Log";
            Directory.CreateDirectory(strLogDir);
            logWriter = new StreamWriter(strLogDir + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log", true);
            setting = Setting.Load();
            if(setting == null)
            {
                Log("加载配置文件失败");
                return;
            }
            isRunning = true;
            win = 0;
            Log("开始运行");
        }

        public void Stop()
        {
            isRunning = false;
            gameResult = GameResult.RESULT_UNKNOW;
            gameState = GameState.GAME_END;
            resultHistory = null;
            SaveBetResults();
            Log("停止运行");
        }

        protected delegate void BrowserClickDelegate(int waitMs, Int32 x, Int32 y);

        protected void InternalBrowserClick(int waitMs, Int32 x, Int32 y)
        {
            Thread.Sleep(waitMs);
            isPlayerOut = false;
            mainForm.BrowserClick(x, y);
        }

        protected void BrowserClick(int waitMs, Int32 x, Int32 y)
        {
            BrowserClickDelegate browserClickDeleggate = new BrowserClickDelegate(InternalBrowserClick);
            browserClickDeleggate.BeginInvoke(waitMs, x, y, null, null);
        }

        protected void SendLog(String logString)
        {
            mainForm.addLog(logString);
        }

        delegate Tuple<GameState, GameResult> ParseImageDelegete(Bitmap image);
        public void ParseImage(Bitmap image)
        {
            ParseImageDelegete parseImageDelegete = new ParseImageDelegete(InternalParseImage);
            parseImageDelegete.BeginInvoke(image, null, null);
            //InternalParseImage(image);
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

        abstract protected bool isOutRoom(Bitmap image);
        abstract protected ResultHistory FirstGetResultHistory(Bitmap image);
        protected void Log(String strLog)
        {
            mainForm.addLog(strLog);
            logWriter.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]" + strLog);
            logWriter.Flush();
        }

        protected abstract bool isStartImage(Bitmap image);
        protected abstract bool isEndImage(Bitmap image, out GameResult gameResult);
        protected abstract bool isVedioOn(Bitmap image);
        protected abstract void ReEnter();
        protected abstract void Exit();
        protected abstract void CloseVedio();

        protected void SaveBetResults()
        {
            int i = 0;
            String exportExcelPath = String.Format(@".\Log\{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss"));
            FileStream fs = new FileStream(exportExcelPath, FileMode.Create, FileAccess.Write);
            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = workbook.CreateSheet("Sheet1") as HSSFSheet;
            IRow headRow = (IRow)sheet.CreateRow(i++);
            headRow.CreateCell(0).SetCellValue("黑总数");
            headRow.CreateCell(1).SetCellValue("红总数");
            headRow.CreateCell(2).SetCellValue("绿总数");
            headRow.CreateCell(3).SetCellValue("预设差值(不包括绿色)");
            headRow.CreateCell(4).SetCellValue("预设利润");
            headRow.CreateCell(5).SetCellValue("当前利润");
            headRow.CreateCell(6).SetCellValue("差值(包括绿色)");
            headRow.CreateCell(7).SetCellValue("当前下注");
            headRow.CreateCell(8).SetCellValue("本局结果");
            headRow.CreateCell(9).SetCellValue("下注结果");

            foreach(var info in saveInfoList)
            {
                IRow row = (IRow)sheet.CreateRow(i++);
                row.CreateCell(0).SetCellValue(info.CountBlack);
                row.CreateCell(1).SetCellValue(info.CountRed);
                row.CreateCell(2).SetCellValue(info.CountGreen);
                row.CreateCell(3).SetCellValue(info.diff);
                row.CreateCell(4).SetCellValue(info.win);
                row.CreateCell(5).SetCellValue(info.current_win);
                row.CreateCell(6).SetCellValue(info.current_diff);
                row.CreateCell(7).SetCellValue(GameResultToString(info.bet));
                row.CreateCell(8).SetCellValue(GameResultToString(info.result));
                row.CreateCell(9).SetCellValue(info.isWin ? "赢":"输");
            }

            workbook.Write(fs);
            fs.Close();
        }

        protected int CalcDiff(int red, int black, int green, bool withGreen)
        {
            if (withGreen)
            {
                if (black > red)
                {
                    return black + green - red;
                }
                else if (red > black)
                {
                    return red + green - black;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return Math.Abs(red - black);
            }
        }
    }
}

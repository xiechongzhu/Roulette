using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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

        protected Tuple<GameState, GameResult> InternalParseImage(Image image)
        {
            if (isStartImage(image) && gameState == GameState.GAME_END && !isPlayerOut)
            {
                Log("新的一局开始了");
                gameState = GameState.GAME_START;
                gameResult = GameResult.RESULT_UNKNOW;
                int dif = Math.Abs(resultHistory.CountRed - resultHistory.CountBlack);
                if (dif >= setting.beginDiffer && !isInGamming)
                {
                    Log("差值大于设定值,开始下注");
                    isInGamming = true;
                }
                else if(dif < setting.endDiffer && isInGamming)
                {
                    Log("差值小于设定值,停止下注");
                    isInGamming = false;
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
                    Log(String.Format("当前相差值:{0},押:{1}", dif, GameResultToString(currentBet)));
                }
                else
                {
                    Log("未达到下注条件，本局不下注");
                }
            }
            else if (isEndImage(image, out gameResult) && gameState == GameState.GAME_START && !isPlayerOut)
            {
                Log("本局结束，结果为:" + GameResultToString(gameResult));
                AddResult(gameResult);
                Log(String.Format("当前统计:红={0} 黑={1} 绿={2}", resultHistory.CountRed, resultHistory.CountBlack, resultHistory.CountGreen));
                gameState = GameState.GAME_END;
                if (isInGamming)
                {
                    Log(String.Format("本局结束，结果为:{0}, 押注:{1}, {2}了", GameResultToString(gameResult),
                        GameResultToString(currentBet), currentBet == gameResult ? "赢" : "输"));
                    saveInfoList.Add(new SaveInfoItem
                    {
                        CountBlack = resultHistory.CountBlack,
                        CountRed = resultHistory.CountRed,
                        CountGreen = resultHistory.CountGreen,
                        difMax = setting.beginDiffer,
                        difMin = setting.endDiffer,
                        bet = currentBet,
                        result = gameResult,
                        isWin = (currentBet == gameResult)
                    });
                }
            }
            else if (isOutRoom(image) && !isPlayerOut)
            {
                Log("被踢出房间");
                isPlayerOut = true;
                ReEnter();
                gameState = GameState.GAME_START;
                isPlayerOut = false;
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
            InternalParseImage(image);
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

        abstract protected bool isOutRoom(Image image);
        abstract protected ResultHistory FirstGetResultHistory(Image image);
        protected void Log(String strLog)
        {
            mainForm.addLog(strLog);
            logWriter.WriteLine("[" + DateTime.Now.ToString("u") + "]" + strLog);
            logWriter.Flush();
        }

        protected abstract bool isStartImage(Image image);
        protected abstract bool isEndImage(Image image, out GameResult gameResult);
        protected abstract void ReEnter();

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
            headRow.CreateCell(3).SetCellValue("开始下注差值");
            headRow.CreateCell(4).SetCellValue("停止下注差值");
            headRow.CreateCell(5).SetCellValue("当前差值");
            headRow.CreateCell(6).SetCellValue("当前下注");
            headRow.CreateCell(7).SetCellValue("本局结果");
            headRow.CreateCell(8).SetCellValue("下注结果");

            foreach(var info in saveInfoList)
            {
                IRow row = (IRow)sheet.CreateRow(i++);
                row.CreateCell(0).SetCellValue(info.CountBlack);
                row.CreateCell(1).SetCellValue(info.CountRed);
                row.CreateCell(2).SetCellValue(info.CountGreen);
                row.CreateCell(3).SetCellValue(info.difMax);
                row.CreateCell(4).SetCellValue(info.difMin);
                row.CreateCell(5).SetCellValue(Math.Abs(info.CountBlack - info.CountRed));
                row.CreateCell(6).SetCellValue(GameResultToString(info.bet));
                row.CreateCell(7).SetCellValue(GameResultToString(info.result));
                row.CreateCell(8).SetCellValue(info.bet == info.result ? "赢":"输");
            }

            workbook.Write(fs);
            fs.Close();
        }
    }
}

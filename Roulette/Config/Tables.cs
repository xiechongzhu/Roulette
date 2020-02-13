using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roulette.Config
{
    public struct TablePoints
    {
        public Point exitPoint;
        public Point reEnterPoint;
    }

    public class Tables
    {
        private Tables()
        {
            tablePoints["C8"] = new TablePoints
            {
                exitPoint = new Point(33, 245),
                reEnterPoint = new Point(276, 557)
            };
            tablePoints["B23"] = new TablePoints
            {
                exitPoint = new Point(33, 274),
                reEnterPoint = new Point(782, 563)
            };
        }

        private static Tables __instance = new Tables();

        public static Tables GetInstance()
        {
            return __instance;
        }

        public TablePoints GetTablePoints(String strTable)
        {
            return tablePoints[strTable];
        }

        public Dictionary<String, TablePoints> tablePoints = new Dictionary<string, TablePoints>();
    }
}

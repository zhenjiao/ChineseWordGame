using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChineseWordGame.Common
{
    public class Constants
    {
        public static readonly string[] ChineseNumbers = new string[TotalLevelCount]
                                                    {
                                                        "一",
                                                        "二",
                                                        "三",
                                                        "四",
                                                        "五",
                                                        "六",
                                                        "七",
                                                        "八",
                                                    };



        public const int BaseBarForEachLevel = 3;
        public const int BaseScoreOfQuestion = 10;
        public const int TotalLevelCount = 8;
        public const int MaxScore = 2200;
    }
}

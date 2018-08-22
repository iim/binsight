using System.Globalization;

namespace APKInsight.Logic.Analysis.Data
{
    public class UseCaseResult
    {
        public int Id;
        public string Filename;
        public int InFileLoc;
        public string Result;
        public string ResultLabel;
        public string ResultMode;

        public bool Rule3StaticKey
            =>
                Result.Contains("StaticLabel") ||
                Result.Contains("StaticVale");

        public bool Rule4StaticSalt
            =>
                Result.Contains("StaticArray") ||
                Result.Contains("StaticArrayFill");

        public bool Rule6StaticSeed
            =>
                Result.Contains("StaticArray") ||
                Result.Contains("StaticArrayFill");

        public bool Rule5LessThan1000Iterations
        {
            get
            {
                int iter = -1;
                if (int.TryParse(Result, out iter))
                {
                    return iter <= 1000;
                }
                return false;
            }
        }

        public bool Rule2StaticIV
            =>
                Result.Contains("StaticArrayFill") ||
                Result.Contains("StaticArray");

        public bool IsModeNotFound
            =>
                (Result.StartsWith("NOTFOUND") && !Result.StartsWith("NOTFOUND-INVOKE:LJAVA/SECURITY/")) ||
                Result.StartsWith("KUJYTDYRDT764T5H8FHGD3G6") ||
                Result.StartsWith("TRUE") ||
                Result == "DEADCODE" ||
                Result.StartsWith("FALSE") ||
                Result.StartsWith("0.0") ||
                Result.StartsWith("1.0") ||
                Result.StartsWith("2.0") ||
                Result.StartsWith("%S/ECB/%S") ||
                Result.StartsWith("_A+M-A=P?A>P<S%3") ||
                Result.StartsWith("DSN IS NULL") ||
                Result.StartsWith("MAESOK") ||
                Result.StartsWith(@"\S+") ||
                Result.StartsWith("&FEATURES=") ||
                Result.StartsWith("UTF8") ||
                Result.StartsWith("UTF-8") ||
                Result.StartsWith("!") ||
                Result.StartsWith("HIGNDLPS") ||
                Result.StartsWith("PAYLOAD") ||
                Result.StartsWith("<AP") ||
                Result.StartsWith("H@2") ||
                Result.StartsWith("BC") ||
                Result.StartsWith("%S/%S/%S") ||
                Result.StartsWith("%S/%S/%S") ||
                string.IsNullOrEmpty(Result) ||

                // Special cases of obfuscation
                Result.StartsWith("ACABEACBB") ||

                Result.StartsWith(@"SERVICES.XML") ||
                Result.StartsWith(
                    @"]*4\U000FH]*4\U000FH]\'!\U0008Z\\/;\U001F{\U001D\'1UJ\U00007%\U000FF]*0\U0008\'\U0019+") ||
                Result.StartsWith(
                    @"]*4\U000FH]*4\U000FH]\'!\U0008Z\\/;\U001F{\U001D\'1UJ\U00007%\U000FF]:\'\U0012Y\U001E+1\U001EZ\\%0\U0002") ||
                Result.StartsWith(@"6\U000B\U0006") ||
                Result.StartsWith(@"6\U000B\U0006\U001EM\U0017") ||
                Result.StartsWith(@"4\'9\U001E)>+;\U001C}\U001AT") ||
                Result.StartsWith(@"6+7\U000EN") ||
                Result.StartsWith(
                    @"A\U0014\U0005\U0004YI\U0014(\U0013Y}\U0015\U000C\U000FJM\U0000\N\U0004N|\U0011\U0006\U000F") ||
                Result.StartsWith(@"L=:") ||
                Result.StartsWith(
                    @"\U000EEB\U0006M\U000EEB\U0006M\U000EHW\U0001_\U000F@M\U0016^NHG\\OSXS\U0006C\U000EUQ\U001B\\MDG\U0017_\U000FJF\U000B") ||
                Result.StartsWith(@"EDP\U0017HD") ||
                Result.StartsWith(
                    @"\U000EEB\U0006M\U000EEB\U0006M\U000EHW\U0001_\U000F@M\U0016^NHG\\OSXS\U0006C\U000EEF\U0001\U0002JDZ") ||
                Result.StartsWith(@"EDP") ||
                Result.StartsWith(@"GHO\U0017\U000CMDM\U0015XI\U001B") ||
                Result.StartsWith(@"EDA\U0007K") ||
                Result.StartsWith(@" EV\U0003)\U0008E[\U0014)\U001CD\U007F\U0008:") ||
                Result.StartsWith(@"-LI") ||
                Result.StartsWith(@"\\S+") ||
                // These are standars, consider them in future work.
                Result.StartsWith(@"2.16.840.1.113733.1.9.2") ||
                Result.StartsWith(@"2.16.840.1.113733.1.9.7") ||
                Result.StartsWith(@"2.16.840.1.113733.1.9.5") ||
                Result.StartsWith(@"1.3.6.1.4.1.22554") ||
                Result.StartsWith(@"1.2.643.2.2.9") ||
                Result.StartsWith(@"1.2.643.2.2.21") ||
                Result.StartsWith(@"1.2.643.2.2.20") ||
                Result.StartsWith(@"1.2.643.2.2.19") ||
                Result.StartsWith(@"1.2.643.2.2.4") ||
                Result.StartsWith(@"1.2.643.2.2.3") ||
                Result.StartsWith(@"1.2.643.2.2.30.1") ||
                Result.StartsWith(@"1.2.643.2.2.32.2") ||
                Result.StartsWith(@"1.2.643.2.2.32.3") ||
                Result.StartsWith(@"1.2.643.2.2.32.4") ||
                Result.StartsWith(@"1.2.643.2.2.32.5") ||
                Result.StartsWith(@"1.2.643.2.2.33.1") ||
                Result.StartsWith(@"1.2.643.2.2.33.2") ||
                Result.StartsWith(@"1.2.643.2.2.33.3") ||
                Result.StartsWith(@"1.2.643.2.2.35.1") ||
                Result.StartsWith(@"1.2.643.2.2.35.2") ||
                Result.StartsWith(@"1.2.643.2.2.35.3") ||
                Result.StartsWith(@"1.2.643.2.2.36.0") ||
                Result.StartsWith(@"1.2.643.2.2.36.1") ||
                Result.StartsWith(@"0.4.0.127.0.7") ||
                Result.StartsWith(@"1.3.6.1.5.5.8.1") ||
                Result.StartsWith(@"1.2.410.200004.1.4") ||
                Result.StartsWith(@"1.2.410.200004.7.1.1.1") ||
                Result.StartsWith(@"2.16.840.1.113730.1.1") ||
                Result.StartsWith(@"2.16.840.1.113730.1.2") ||
                Result.StartsWith(@"2.16.840.1.113730.1.3") ||
                Result.StartsWith(@"2.16.840.1.113730.1.4") ||
                Result.StartsWith(@"2.16.840.1.113730.1.7") ||
                Result.StartsWith(@"2.16.840.1.113730.1.8") ||
                Result.StartsWith(@"2.16.840.1.113730.1.12") ||
                Result.StartsWith(@"2.16.840.1.113730.1.13") ||
                Result.StartsWith(@"2.16.840.1.113733.1.6.3") ||
                Result.StartsWith(@"2.16.840.1.113733.1.6.15") ||
                Result.StartsWith(@"2.16.840.1.113719.1.9.4.1") ||
                Result.StartsWith(@"1.2.840.113533.7.65.0") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.2.1") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.2.2") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.2.3") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.2.4") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.1") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.2") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.3") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.4") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.5") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.6") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.7") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.21") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.22") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.23") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.24") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.25") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.26") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.27") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.41") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.42") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.43") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.44") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.45") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.46") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.1.47") ||
                Result.StartsWith(@"2.16.840.1.101.3.4.3") ||
                Result.StartsWith(@"1.2.392.200011.61.1.1.1.2") ||
                Result.StartsWith(@"1.2.392.200011.61.1.1.1.3") ||
                Result.StartsWith(@"1.2.392.200011.61.1.1.1.4") ||
                Result.StartsWith(@"1.2.392.200011.61.1.1.3.2") ||
                Result.StartsWith(@"1.2.392.200011.61.1.1.3.3") ||
                Result.StartsWith(@"1.2.392.200011.61.1.1.3.4") ||
                Result.StartsWith(@"1.3.14.3.2.2") ||
                Result.StartsWith(@"1.3.14.3.2.3") ||
                Result.StartsWith(@"1.3.14.3.2.4") ||
                Result.StartsWith(@"1.3.14.3.2.6") ||
                Result.StartsWith(@"1.3.14.3.2.7") ||
                Result.StartsWith(@"1.3.14.3.2.8") ||
                Result.StartsWith(@"1.3.14.3.2.9") ||
                Result.StartsWith(@"1.3.14.3.2.17") ||
                Result.StartsWith(@"1.3.14.3.2.26") ||
                Result.StartsWith(@"1.3.14.3.2.27") ||
                Result.StartsWith(@"1.3.14.3.2.29") ||
                Result.StartsWith(@"1.3.14.7.2.1.1") ||
                Result.StartsWith(@"1.2.840.113549.1.1.1") ||
                Result.StartsWith(@"1.2.840.113549.1.1.2") ||
                Result.StartsWith(@"1.2.840.113549.1.1.3") ||
                Result.StartsWith(@"1.2.840.113549.1.1.4") ||
                Result.StartsWith(@"1.2.840.113549.1.1.5") ||
                Result.StartsWith(@"1.2.840.113549.1.1.6") ||
                Result.StartsWith(@"1.2.840.113549.1.1.7") ||
                Result.StartsWith(@"1.2.840.113549.1.1.8") ||
                Result.StartsWith(@"1.2.840.113549.1.1.9") ||
                Result.StartsWith(@"1.2.840.113549.1.1.10") ||
                Result.StartsWith(@"1.2.840.113549.1.1.11") ||
                Result.StartsWith(@"1.2.840.113549.1.1.12") ||
                Result.StartsWith(@"1.2.840.113549.1.1.13") ||
                Result.StartsWith(@"1.2.840.113549.1.1.14") ||
                Result.StartsWith(@"1.2.840.113549.1.3.1") ||
                Result.StartsWith(@"1.2.840.113549.1.5.1") ||
                Result.StartsWith(@"1.2.840.113549.1.5.4") ||
                Result.StartsWith(@"1.2.840.113549.1.5.3") ||
                Result.StartsWith(@"1.2.840.113549.1.5.6") ||
                Result.StartsWith(@"1.2.840.113549.1.5.10") ||
                Result.StartsWith(@"1.2.840.113549.1.5.11") ||
                Result.StartsWith(@"1.2.840.113549.1.5.13") ||
                Result.StartsWith(@"1.2.840.113549.1.5.12") ||
                Result.StartsWith(@"1.2.840.113549.3.7") ||
                Result.StartsWith(@"1.2.840.113549.3.2") ||
                Result.StartsWith(@"1.2.840.113549.2.2") ||
                Result.StartsWith(@"1.2.840.113549.2.4") ||
                Result.StartsWith(@"1.2.840.113549.2.5") ||
                Result.StartsWith(@"1.2.840.113549.2.7") ||
                Result.StartsWith(@"1.2.840.113549.2.8") ||
                Result.StartsWith(@"1.2.840.113549.2.9") ||
                Result.StartsWith(@"1.2.840.113549.2.10") ||
                Result.StartsWith(@"1.2.840.113549.2.11") ||
                Result.StartsWith(@"1.2.840.113549.1.7.1") ||
                Result.StartsWith(@"1.2.840.113549.1.7.2") ||
                Result.StartsWith(@"1.2.840.113549.1.7.3") ||
                Result.StartsWith(@"1.2.840.113549.1.7.4") ||
                Result.StartsWith(@"1.2.840.113549.1.7.5") ||
                Result.StartsWith(@"1.2.840.113549.1.7.6") ||
                Result.StartsWith(@"1.2.840.113549.1.9.1") ||
                Result.StartsWith(@"1.2.840.113549.1.9.2") ||
                Result.StartsWith(@"1.2.840.113549.1.9.3") ||
                Result.StartsWith(@"1.2.840.113549.1.9.4") ||
                Result.StartsWith(@"1.2.840.113549.1.9.5") ||
                Result.StartsWith(@"1.2.840.113549.1.9.6") ||
                Result.StartsWith(@"1.2.840.113549.1.9.7") ||
                Result.StartsWith(@"1.2.840.113549.1.9.8") ||
                Result.StartsWith(@"1.2.840.113549.1.9.9") ||
                Result.StartsWith(@"1.2.840.113549.1.9.13") ||
                Result.StartsWith(@"1.2.840.113549.1.9.14") ||
                Result.StartsWith(@"1.2.840.113549.1.9.15") ||
                Result.StartsWith(@"1.2.840.113549.1.9.20") ||
                Result.StartsWith(@"1.2.840.113549.1.9.21") ||
                Result.StartsWith(@"1.2.840.113549.1.9.22.1") ||
                Result.StartsWith(@"1.2.840.113549.1.9.22.2") ||
                Result.StartsWith(@"1.2.840.113549.1.9.23.1") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.3.9") ||
                Result.StartsWith(@"1.2.840.113549.1.9.15.1") ||
                Result.StartsWith(@"1.2.840.113549.1.9.15.2") ||
                Result.StartsWith(@"1.2.840.113549.1.9.15.3") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.1.2") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.1.4") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.1.9") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.1.23") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.6.1") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.6.2") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.6.3") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.6.4") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.6.5") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.6.6") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.2.1") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.2.4") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.2.11") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.2.12") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.2.47") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.2.7") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.2.14") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.2.15") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.2.16") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.2.17") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.2.18") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.2.19") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.2.20") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.2.21") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.2.22") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.2.23") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.2.24") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.2.25") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.2.26") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.2.27") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.5.1") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.5.2") ||
                Result.StartsWith(@"1.2.840.113549.1.12.10.1.1") ||
                Result.StartsWith(@"1.2.840.113549.1.12.10.1.2") ||
                Result.StartsWith(@"1.2.840.113549.1.12.10.1.3") ||
                Result.StartsWith(@"1.2.840.113549.1.12.10.1.4") ||
                Result.StartsWith(@"1.2.840.113549.1.12.10.1.5") ||
                Result.StartsWith(@"1.2.840.113549.1.12.10.1.6") ||
                Result.StartsWith(@"1.2.840.113549.1.12.1.1") ||
                Result.StartsWith(@"1.2.840.113549.1.12.1.2") ||
                Result.StartsWith(@"1.2.840.113549.1.12.1.3") ||
                Result.StartsWith(@"1.2.840.113549.1.12.1.4") ||
                Result.StartsWith(@"1.2.840.113549.1.12.1.5") ||
                Result.StartsWith(@"1.2.840.113549.1.12.1.6") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.3.6") ||
                Result.StartsWith(@"1.2.840.113549.1.9.16.3.7") ||
                Result.StartsWith(@"1.3.132.0") ||
                Result.StartsWith(@"1.3.36.3.2.1") ||
                Result.StartsWith(@"1.3.36.3.2.2") ||
                Result.StartsWith(@"1.3.36.3.2.3") ||
                Result.StartsWith(@"1.3.36.3.3.1.2") ||
                Result.StartsWith(@"1.3.36.3.3.1.3") ||
                Result.StartsWith(@"1.3.36.3.3.1.4") ||
                Result.StartsWith(@"1.3.36.3.3.2") ||
                Result.StartsWith(@"1.3.36.3.3.2.8") ||
                Result.StartsWith(@"2.5.29.9") ||
                Result.StartsWith(@"2.5.29.14") ||
                Result.StartsWith(@"2.5.29.15") ||
                Result.StartsWith(@"2.5.29.16") ||
                Result.StartsWith(@"2.5.29.17") ||
                Result.StartsWith(@"2.5.29.18") ||
                Result.StartsWith(@"2.5.29.19") ||
                Result.StartsWith(@"2.5.29.20") ||
                Result.StartsWith(@"2.5.29.21") ||
                Result.StartsWith(@"2.5.29.23") ||
                Result.StartsWith(@"2.5.29.24") ||
                Result.StartsWith(@"2.5.29.27") ||
                Result.StartsWith(@"2.5.29.28") ||
                Result.StartsWith(@"2.5.29.29") ||
                Result.StartsWith(@"2.5.29.30") ||
                Result.StartsWith(@"2.5.29.31") ||
                Result.StartsWith(@"2.5.29.32") ||
                Result.StartsWith(@"2.5.29.33") ||
                Result.StartsWith(@"2.5.29.35") ||
                Result.StartsWith(@"2.5.29.36") ||
                Result.StartsWith(@"2.5.29.37") ||
                Result.StartsWith(@"2.5.29.46") ||
                Result.StartsWith(@"2.5.29.54") ||
                Result.StartsWith(@"1.3.6.1.5.5.7.1.1") ||
                Result.StartsWith(@"1.3.6.1.5.5.7.1.11") ||
                Result.StartsWith(@"1.3.6.1.5.5.7.1.12") ||
                Result.StartsWith(@"1.3.6.1.5.5.7.1.2") ||
                Result.StartsWith(@"1.3.6.1.5.5.7.1.3") ||
                Result.StartsWith(@"1.3.6.1.5.5.7.1.4") ||
                Result.StartsWith(@"2.5.29.56") ||
                Result.StartsWith(@"2.5.29.55") ||
                Result.StartsWith(@"2.5.4.6") ||
                Result.StartsWith(@"2.5.4.10") ||
                Result.StartsWith(@"2.5.4.11") ||
                Result.StartsWith(@"2.5.4.12") ||
                Result.StartsWith(@"2.5.4.3") ||
                Result.StartsWith(@"2.5.4.5") ||
                Result.StartsWith(@"2.5.4.9") ||
                Result.StartsWith(@"2.5.4.7") ||
                Result.StartsWith(@"2.5.4.8") ||
                Result.StartsWith(@"2.5.4.4") ||
                Result.StartsWith(@"2.5.4.42") ||
                Result.StartsWith(@"2.5.4.43") ||
                Result.StartsWith(@"2.5.4.44") ||
                Result.StartsWith(@"2.5.4.45") ||
                Result.StartsWith(@"2.5.4.15") ||
                Result.StartsWith(@"2.5.4.17") ||
                Result.StartsWith(@"2.5.4.46") ||
                Result.StartsWith(@"2.5.4.65") ||
                Result.StartsWith(@"1.3.6.1.5.5.7.9.1") ||
                Result.StartsWith(@"1.3.6.1.5.5.7.9.2") ||
                Result.StartsWith(@"1.3.6.1.5.5.7.9.3") ||
                Result.StartsWith(@"1.3.6.1.5.5.7.9.4") ||
                Result.StartsWith(@"1.3.6.1.5.5.7.9.5") ||
                Result.StartsWith(@"1.3.36.8.3.14") ||
                Result.StartsWith(@"2.5.4.16") ||
                Result.StartsWith(@"2.5.4.54") ||
                Result.StartsWith(@"0.9.2342.19200300.100.1.25") ||
                Result.StartsWith(@"0.9.2342.19200300.100.1.1") ||
                Result.StartsWith(@"BADLY FORMATED DIRECTORY STRING") ||
                Result.StartsWith(@"2.5.4.20") ||
                Result.StartsWith(@"2.5.4.41") ||
                Result.StartsWith(@"2.5.8.1.1") ||
                Result.StartsWith(@"1.3.6.1.5.5.7") ||
                Result.StartsWith(@"1.2.840.10045.1.1") ||
                Result.StartsWith(@"1.2.840.10045.1.2") ||
                Result.StartsWith(@"1.2.840.10045.1.2.3.1") ||
                Result.StartsWith(@"1.2.840.10045.1.2.3.2") ||
                Result.StartsWith(@"1.2.840.10045.1.2.3.3") ||
                Result.StartsWith(@"1.2.840.10045.4.1") ||
                Result.StartsWith(@"1.2.840.10045.2.1") ||
                Result.StartsWith(@"1.2.840.10045.4.3") ||
                Result.StartsWith(@"1.2.840.10045.3.0.1") ||
                Result.StartsWith(@"1.2.840.10045.3.0.2") ||
                Result.StartsWith(@"1.2.840.10045.3.0.3") ||
                Result.StartsWith(@"1.2.840.10045.3.0.4") ||
                Result.StartsWith(@"1.2.840.10045.3.0.5") ||
                Result.StartsWith(@"1.2.840.10045.3.0.6") ||
                Result.StartsWith(@"1.2.840.10045.3.0.7") ||
                Result.StartsWith(@"1.2.840.10045.3.0.8") ||
                Result.StartsWith(@"1.2.840.10045.3.0.9") ||
                Result.StartsWith(@"1.2.840.10045.3.0.10") ||
                Result.StartsWith(@"1.2.840.10045.3.0.11") ||
                Result.StartsWith(@"1.2.840.10045.3.0.12") ||
                Result.StartsWith(@"1.2.840.10045.3.0.13") ||
                Result.StartsWith(@"1.2.840.10045.3.0.14") ||
                Result.StartsWith(@"1.2.840.10045.3.0.15") ||
                Result.StartsWith(@"1.2.840.10045.3.0.16") ||
                Result.StartsWith(@"1.2.840.10045.3.0.17") ||
                Result.StartsWith(@"1.2.840.10045.3.0.18") ||
                Result.StartsWith(@"1.2.840.10045.3.0.19") ||
                Result.StartsWith(@"1.2.840.10045.3.0.20") ||
                Result.StartsWith(@"1.2.840.10045.3.1.1") ||
                Result.StartsWith(@"1.2.840.10045.3.1.2") ||
                Result.StartsWith(@"1.2.840.10045.3.1.3") ||
                Result.StartsWith(@"1.2.840.10045.3.1.4") ||
                Result.StartsWith(@"1.2.840.10045.3.1.5") ||
                Result.StartsWith(@"1.2.840.10045.3.1.6") ||
                Result.StartsWith(@"1.2.840.10045.3.1.7") ||
                Result.StartsWith(@"1.2.840.10046.2.1") ||
                Result.StartsWith(@"1.2.840.10040.4.1") ||
                Result.StartsWith(@"1.2.840.10040.4.3") ||
                Result.StartsWith(@"1.3.133.16.840.63.0") ||
                Result.StartsWith(@"1.2.840.10046.3") ||
                Result.StartsWith(@"1.3.6.1.4.1.311.20.2.3") ||
                Result.StartsWith(@"1.3.6.1.4.1.311.25.1") ||
                Result.StartsWith(@"1.3.6.1.5.2.2") ||
                Result.StartsWith(@"&") ||
                Result.StartsWith(@".PART") ||
                Result.StartsWith(@"%\U000FX<L\U0004") ||
                Result.StartsWith(@"\'\U0005S\U001AG/\U0004\U007F\U001CK)\U0004D\U0015G&\U0013L\U000BG4\U001AH\U0017") ||
                Result.StartsWith(@"%\U000FX") ||
                Result.StartsWith(@"N.J-IN.J-IN#_*{O+E=Z\U000E#OWK\U00133[-GN>Y0X\R/O<{O!N ") ||
                Result.StartsWith(@"N.J-IN.J-IN#_*{O+E=Z\U000E#OWK\U00133[-GN.N*&\N/R") ||
                Result.StartsWith(@"\""\U0007F\NO(\U0008B\U0010\\2\U0019O\U001C^P") ||
                Result.StartsWith(@"%/I") ||
                Result.StartsWith(@"K\U5B8C\U945B\UFF43") ||
                Result.StartsWith(@"\U666F\U65CD\UFF31") ||
                Result.StartsWith(@"\U5BA7\U65CD\UFF31") ||
                Result.StartsWith(@"K\U5B8C\U65AC\UFF43") ||
                Result.StartsWith(@"\U6E4D\U8A2CO\U001C[A\U0006D8LA\U0001N \U52A8\U5BA7\U89A9\U5BED") ||
                Result.StartsWith(@"K\U6644\U65AC\UFF43") ||
                Result.StartsWith(@"\U8982\U5B8C\U65AC\U4EAF\UFF12") ||
                Result.StartsWith(@"\U6E4D\U8A2CO\U001C[\U757E\U625A`<Q\U65E6\U4EBC\U528B\U5B9F\U89EB\U5BA7") ||
                Result.StartsWith(@"A\U95C1\U59E0Y") ||
                Result.StartsWith(@"\U52C1\U5B8C\U65AC\U4EAF\UFF12") ||
                Result.StartsWith(@"A\U7D1A\U6774Y") ||
                Result.StartsWith(@"\'#G<(-/E>|\TP") ||
                Result.StartsWith(
                    @"R~N\U001FLVZN\U001D9V\U000E\U001BAJ$X\U0013N>$~OO?Y{IK>P\U000FO\U001B9US\U001CJ1WX\U001B\U0018IP\U000F\U001BN?%\U000F\U001BI;\""\U000F\U001D\U001FKW\U000ENI") ||
                Result.StartsWith(
                    @"$|O\U001DLR\U007FHM0P\U000F\U001B\U001BM\""\U000C\U001AN>Q\U007FIN9X\T\U0013I;X\U000E\U0013\U0018<PX\U001AM=URMHI\'\U007FJ\U001F9V|\U001BLJV{\U0012\U001A0QY\U0012\U001DK\""\U0008J\U001DIV{\U0013JJX}M\U001F=") ||
                Result.StartsWith(
                    @"W}\U001EH=V\U007FO\U001DMPR\U0012\U001B<T\T\U001D\U0018<Q\U000CMLMX\U007FO`=QS\U001F\U001B:\""\U000CMMKQ\U007FOJKQ\U000BOO:Y\U000BI\U001D> RO\U001C9V\T\U0018\U001FK%\U000B\U001DM9W\U000CMMM%Y\U0012O9R}\U001BL;TSM\U001C9WY\U001CNKS\U000E\U001C\U001D<\'\U000E\U001E\U001CN#~\U0012`9SXN\U0018>T\U007FNL<\'|\U001C`:R{\U001BM;U|M\U001C?Q~\U0012`<T}\U001CI") ||
                Result.StartsWith(
                    @"\'\U000F\U001F\U001AL$S\U001E`?R\U000B\U0019M:W\U000BH\U001B8T\U000EJ\U001B;S\T\U0018LI$R\U001AO0\'~OKJT\U000C\U0012I8S\U000FIAL#\U007F\U0018IKX|\U001BH8%\U000C\U001C\U001A") ||
                Result.StartsWith(
                    @"XZ\U001ELLU\U000BN\U001C?R{O`?Q\U000E\U0012N9SZHJLV~NO>$ZMI;\'S\U001D\U001F: XNH;Y\U0008MN;PZ\U001BL< \U000F\U001DKM$R\U001CK") ||
                Result.StartsWith(@"T{NINVSI\U001C8%\U000C\U001ENIY") ||
                Result.StartsWith(
                    @" ZHL?V\U000C\U001AN8T|\U001D\U001F<X\U000CJHM\""\U007F\U001B\U001A9W\U000EM\U001D0R{M\U001A?\""\U000F\U001E`IY|\U001AH< }\U0012\U0018MQ~\U001EKK\'\U000FI\U001BKS\U000CN\U001A>PR\U0019`:PY\U001F\U001D>U\U000E\U001CJ8S{HHIXR\U0019\U001F<%|\U001BN: Z\U0012\U001C=XX\U001D\U001BL\'\T\U001B\U001A8W\TI\U001C9QYHIJP{MM8S\U000E\U001EAIX{MM0WSNL8YR\U001F\U001C") ||
                Result.StartsWith(
                    @"\U6E4D\U8A2CO\U001C[A\U0006D8LA\U0000J/IA\U0001N (\U0000$OYM\U000F8R)|\U0004.\U000B-M\U0019>\U000B\U52F9\U5BCE\U8982\U5B8C") ||
                Result.StartsWith(
                    @"\U6E4D\U8A2C\U007F+A\U0011&NYL$\U0019\U000B\U0015G\U0000.\U000B\U0012M\U0018\U52EA\U5BED\U89BA\U5BCE") ||
                Result.StartsWith(
                    @"\U6E4D\U8A2CA8~\U0000\U7555\U623B\U76DDL$\U0019\U528B\U5B9F\U768C\U65E6\U4EBC\U7503\U0018F\U00058D0L\U8982\U5B8C") ||
                Result.StartsWith(@"R|\U001E\U0018>#Y\U001DO<T\U000B\U001FJ;Q") ||
                Result.StartsWith(
                    @"\U6E4D\U8A2C\U007F+A\U0011&NYL$\U0019\U7534\U6249C\U00043\U65AC\U4EAF\U52A8\U5BA7\U89A9\U5BED") ||
                Result.StartsWith(@"\U6E4D\U8A2CO\U001C[\U757E\U625A`<Q\U52C1\U5B8C\U89C8\U5B9F") ||
                Result.StartsWith(
                    @" X\U0013J9XZ\U0013L?\""}M\U001BLQX\U001EH>XX\U001AJ9T\U0008\U001E\U001DNV|\U0013LIV}H\U001FMVY\U0019L;%\U000E\U001D") ||
                Result.StartsWith(
                    @"\U6E4D\U8A2CA8~\U0000\U7555\U623B\U76DD;%\U000FX\U52F9\U5BCE\U76E5\U65CD\U4EDD\U7571I\U000F.Y6A\U0005\U89A9\U5BED") ||
                Result.StartsWith(@"\U6E4D\U8A2C\U007F+A\U0011&NYL$\U0019\U7534\U6249C\U00043\U528B\U5B9F\U89EB\U5BA7") ||
                Result.StartsWith(
                    @" Z\U0012J9S\U000CJO=T\U000C\U0019\U001BIV}\U001A\U001B=\""}JL> \U000ENO;P\U000C\U001FIN#\U0008\U001D`IY\U000CMK1R\U000CO\U001AK |I\U0018LVZJOJRR\U0019L") ||
                Result.StartsWith(
                    @"XXHH:$}\U001E\U0018LW|IMN%X\U001AJ9U\U007F\U001C\U0018M\""\U000B\U0012J=S\U000BH\U001B1%}\U001EINQ\U000F\U0019KIT\U000EIAIU|\U001BM9\""\U000FHHLQ\U000E\U0012\U0018NW\U000F\U0018\U001B0WY\U001AJN$\U000CJO8\""YI\U001A:TR\U0013\U001C>%|\U001F\U0018=%~\U001D\U001D>V\U000CH\U001F<#YI\U001C0$R\U001DL0QRIN;#\U000CIAM$\U000CJIKY\U000BIMKPY\U0012`LW\U007FMK") ||
                Result.StartsWith(
                    @"\U6E4D\U8A2C\U007F+A\U0011&NYL$\U0019\U000B\U0015G\U0000.\U000B\U0013I\U0017+\U000B\U0012M\U0018JJ7LA\U000FE+Q\U0011>N=(\U0015/S-(\U52C1\U5B8C\U89C8\U5B9F") ||
                Result.StartsWith(
                    @"#Y\U001A\U001FIPZMH?SR\U001EK8$\U000C\U001E\U001C9R}\U001BKJ\""S\U001C`=\'\U000FHO; \U007F\U0019\U001DN\'\U000E\U001DK9P{\U001FI;X\U000EJ\U00180#\U0008\U0019M8%\U0008\U001B\U001D") ||
                Result.StartsWith(
                    @"$Y\U001EM1W\U000C\U001B\U0018MW~HK=Y}\U001BHNSRIO=V|N\U0018NV~MOI#XOIM${\U001E\U001B:XYI\U001A:SR\U001DI0X\U007F\U0019KIP~\U0013J") ||
                Result.StartsWith(
                    @"Y\U000F\U0019\U001BNU\U007F\U0012\U0018MP\U0008\U0013\U001B>$\U000CNK>R\U000B\U001D`MQX\U0018\U001FIQZ\U0019OL$|\U001CN=W\U0008\U0013\U001CMWRM\U001FMS|\U0018\U0018>X\U000F\U001BK;\'\U000B\U001BI9RRH\U001B=W\U007FHL8R}M\U0018:") ||
                Result.StartsWith(
                    @"S\U0008\U0018MMP|\U0012\U001B:W\U000C\U001BH9T}\U001CHN$|\U0012\U001D8R\U000F\U0013\U001DLY\U000B\U001C\U001D<P~\U001AOKS\TOM?#R\U001A\U001CL \U000B\U0019N>%RNMM$Y\U001A\U001ALR}JI8V|\U001FOK%\U007F\U0012\U001C0VS\U001CA?\'}J\U001F? |\U001DHIQ\U000F\U0018\U001FNXZ\U0012\U001F;W~N\U001CK\'\U007F\U001EH?YZNONQ\U000CN\U001F;WSN\U001F:VYNJI\""}\U001CIJWX\U001BA") ||
                Result.StartsWith(@"4T$HY4T$HY4Y1OK5Q+XJTY!\U0012[II5HW4Q+XJTY!CHNR)U[5R") ||
                Result.StartsWith(@"4T$HY4T$HY4Y1OK5Q+XJTY!\U0012[II5HW4Z$JYD`0^TRSKL]V") ||
                Result.StartsWith(@"4T$HY4T$HY4Y1OK5Q+XJTY!\U0012[II5HW4Q+XJTY!CHNR)U[5` Q") ||
                Result.StartsWith(@"6=H\U0011\U0015YU\U0002UV;@\U0010~TRSEW}B=H\U0011\U00156") ||
                Result.StartsWith(@"4T$HY4T$HY4Y1OK5Q+XJTY!\U0012[II5HW4Q+XJTY!CHIY3]L~>5YU") ||
                Result.StartsWith(@"4T$HY4T$HY4Y1OK5Q+XJTY!\U0012[II5HW4Q+XJTY!CHIY3]L~>\'UV") ||
                Result.StartsWith(@"6=H\U0011\U0015YU\U0002UV;B\U0016}\U0018KB\U000CJYOUEW}B=H\U0011\U00156") ||
                Result.StartsWith(@"6=H\U0011\U0015^^\U0001\U001CJHQELJRF\U0004H};[\U0000E\U00156=H\U0011") ||
                Result.StartsWith(@"6=H\U0011\U0015^^\U0001\U001CHNR\TU{;[\U0000E\U00156=H\U0011") ||
                Result.StartsWith(@"4T$HY4T$HY4Y1OK5Q+XJTY!\U0012[II5HW4Z$JYD`7UNZD \U0012H~}") ||
                Result.StartsWith(@"IC\U0004") ||
                Result.StartsWith(@"W\U007F$X\U0018KB") ||
                Result.StartsWith(@"\U5BDD\U6597\UFF5F") ||
                Result.StartsWith(@"W\U007F$X\U0018KE\'PQX0\U000EYA!") ||
                Result.StartsWith(@"\U0011\U79D1\U9435\UFF26") ||
                Result.StartsWith(@"\U6E37\U8A76\U5FDBL}V\U6597\U4EB3\U529C\U8F31\U5177\U9460\U7984\U944C") ||
                Result.StartsWith(@"\U0011\U661E\U65C2\UFF26") ||
                Result.StartsWith(@"_U\'I_") ||
                Result.StartsWith(@"\U0011\U5BD6\U65C2\UFF26") ||
                Result.StartsWith(@"\U6615\U6597\UFF5F") ||
                Result.StartsWith(@"\U0011\U517C\U9435\UFF26") ||
                Result.StartsWith(@"\U6E37\U8A76\U0004R\\I\U007F") ||
                Result.StartsWith(@"\\U+YJZD \U001CHNR)U[;[ E\U0002") ||
                Result.StartsWith(@"\\U+YJZD \U001CHIY3]L~0\U000EYA!") ||
                Result.StartsWith(@"\U6E37\U8A76\U5FDB\U65BB\U4ECE\U8F12\U5175\U0004R\\I\U007F") ||
                Result.StartsWith(
                    @"Y\'\U0006~\U000E_$\U0004\U0008\U000F](|\U0005\U0001*Q\U0006\U000B\U000E)#\U0004\U007F\U0000XV\U0003}\U000BY V\U007FY.\""U\U000E\N^!}YZ)&T\U000BY#R\U0006\U000BZ") ||
                Result.StartsWith(@"((\U0006}\U000C\""TV\U0005\U000BZU\U0006\U000C\U000C)UP\U000F\N") ||
                Result.StartsWith(
                    @"/(}Y\U000C+Q\U0007\U000F\U000E_)\U0006\N\NZ(U\U000B\U0000)TPZ\U000B/$\U0007\U000B\U000B)!\U0004X\U000B-\""P\U0004\U0000*!\U0001\U0004Y^#}Z\U000B.#WY\U000B^)\U0001\U007F}") ||
                Result.StartsWith(
                    @"/R\U0000\U000EY-R\U0003\U000C|#\'\U0004\R\U000B_ \U0000\U0008\U0001+$\U0007\U0008|-#\U0003\N\U0000XVT\U000C\U000C-\""VZ\R\""$\U0000\U000C~)$\U0000~\R)$\U0003\U000F\R.%T\R\U0001") ||
                Result.StartsWith(
                    @".%WZ\R\""T}\U000E\U000C/U\U0003\U0008}-)WX\U0001.\""|\U007F\U000C_RR\U007F{Z(U\R\U0008# PX\U000FY!\U0003\U007FY+&V\NZXVQ\U007F\N^$\U0000\U000FZZ&R\U0004|^UW\R\U000F#UT~{^%QZ\U000C^)W\U007FZ#Q\U0000Y~_UW\U0004|.!\U0007\U0005{X V\T\TX V~\U0008*\""W\RZ#QP\U000E\U0008-%W\U000B\RY\""\U0001Z\RX!W\U0005\U0000]$|}\RXUT\U0004{(\""P\U000B\U0000+%R\U000F\U000E.!T\U0005{Z)W\U000C\NY%\U0007ZY-\""\U0001Y\U000F\""!|~{#$\U0007\T\U000E*$\U0001Y\U000E/S\U0000\U000F\U0001_T\U0004\R\U000EX\""P~\N-%Q~\U000E-RS\U000EYYUQZ\U000F. \U0006\U0005{]&}\U0005\U000B\""QP\U0004\U000C^U\U0007}Z]V|\U0005~)#S\N\U000B/(\U0003\R\T]R}\U0005\U0008^Q}\U000B\U000EZS\U0004Z\U0001*#|\U007FZ/%\U0000X}-\'}\U0004|") ||
                Result.StartsWith(
                    @"\U6E37\U8A76\U755A\U622C\U5154\U946B\U79D1\U9435\U5E4A\U4FE5\U5B43\U6200\U0015YU\U659C\U4EE6") ||
                Result.StartsWith(
                    @"\U6E37\U8A76\U756DVYM\U750F\U6255\U76B8\U5154\U946B\U79D1\U9435\U65BB\U4ECE\U52BB\U5BD6\U65C2\U6710\UFF34\U8FCF\U56CE\U5B83\U65BB\U7D5EQQ3]\U7510\U79DA\U9460\U89A6\U5BFA") ||
                Result.StartsWith(
                    @"\U6E37\U8A76\U5FDB\U5B6B\U7B1E\U4E29\U8F19\U5120}V\U007FB*U\\\U79DA\U9460\U89A6\U5BFARZF$\U7AD3\U7510\U5177\U9460\U52E5\U5BFA\U76BC\U5BDD\U6597") ||
                Result.StartsWith(
                    @"VY\U000CYN|Y\U0007}|Z^\U0007[SJX.U\U007F\""GU~YJU\U0003}YHS\U0007W_LW\""OSZW\U0000}YTY\U0007}IX?\U0016F{}?P\U000F\NZ[") ||
                Result.StartsWith(@"-#\U0007\U000BZ\""%QX\U000B") ||
                Result.StartsWith(
                    @"VY\U000C~QQQ\U000B~_PA-WQ\\)2\U000CZZA\U0000ZYZ_\U0006}I#Q\U0008UQYS\""W{ZA\U0000}N+}\U0014R\U00170T|QQT`UH_QR)\TSR_\U0013NZWH\'U\TV(\U000E\U000C`/?#W\U0017NA4{A]V\U0012U^\U007FE\U0004\N{({QP`YD\U001DN\U007FZI-KRI&\U0015UKMA\U0014RH\""G\U000CYOZJ\U0003SPL}\U001FVL(H\U0003Z|.U\U000BHZMV\U001CO\\C]V]QX\U007F\U0002E\U0001M]U\RZK@\U001D]PMI!}\U000C*V\U0001\U0005}Z}") ||
                Result.StartsWith(@"+ Q\N\U0001.\'\U0003\U007F\U000C+Q\U0003\U0005Z]U\U0006\N\U000B#)\U0004\T\U000B") ||
                Result.StartsWith(@")TW\T|(QQXY))V~~") ||
                Result.StartsWith(
                    @"*V\U0003\N\U000F* \U0001\U0004\U0000^U\U0004YY(TV\T\U000E#TQX\U0001-&V\U0008~Z&Q~\U0008") ||
                Result.StartsWith(
                    @"+(U~\N_SP\TY*\'}\N\T)(S\U000BZ\""!U}\U000F/)}\U007F\N^TRZ\U0008_#\U0000\TY]$VX})#R~\U000E-&}\U007FYZRTZ\U000E+RP}\U000B") ||
                Result.StartsWith(@".$\U0001\U0004\U0000(\'\U0003\U0004\U000C+UW\U000E\U000B") ||
                Result.StartsWith(
                    @"VY\U000C~QQQ\U000B~_PA-WQ\\)2\U000CZZA\U0000ZYZ_\U0006}I#Q\U0008UQYS\""W{ZA\U0000}NQV\U000FL\TKS\""~\U000BIE=V\U007FLQVPW") ||
                Result.StartsWith(
                    @"\U6E37\U8A76\U756DVYMQ\U755A\U622C\U76BC\U5177\U9460\U4E77\U529C\U5BFE\U659C\U673C\UFF49\U8FE8\U56E6\U5BDD\U6597\U7D23VYMQ\U756D\U79FD\U9448\U89F8\U5BD6") ||
                Result.StartsWith(
                    @"VY\U000CYN|Y\U0007}|Z^\U0007[SJX.U\U007F\""GU~YJU\U0003}YHS\U0007W_LW\""OSZW\U0000}YTY\U0007}IX;\U0008IS4G.WYSU1H}PR\U0004X[NZ1D@L[!H}WJ5L{VA\U0001H\NYR\U0016UHCD\T~BTS\TD}LX2L\U0013YV\U0012EWQCR\U000EH}#\T\U0017\U000EC_ \U0008MQA\U000EL\U000E.U3NK~)0HNUE\U0011D{CH\U0003L\U007FSY\U0000KKO`\U0013NNCS|\T@I(\U0008N\U0008NRSVL(Z\U001FJBS;$R\RSJ3T\U0013JR+U\U0000K{6]J(Y\U0016OAWS\U0013YI+A}JBIY\U0014U\\UU7VVK)PY\U007FY\""\U001DYKT?\U000B[\U0001.`SE^J\'!FIK_5_PBINFBWYJ\U000C@UW\U0017R\U0000K~\U001FPL)R&\NVBS\TD|0;2H\NCU?Y@AQ=\U000FHT` U\U007FJV=L_BS\U000FX\U0017S&/\TWVE\U001D[\U0008Z_\U000EHPZJPPPYENXN/U\U001F\U000FU\\S7NLY]\'FVHJ\U000FP@BG=\U000CPLSJ{JBAU\U000ELT_SP[KV<^SU@\U0004[UYQ\U0004Y{|W\U0000~YW\U007FRQ\U007FU{\U0016\U000B~4}(QPK\'VK@^F0TUH`NHQJD-ITWQ&XRYR\""PH|;}\TQ^U\U0017V\NAB+U\U000FV\\\U001DR\U0000^&\U0016M[+_\TR\U000ESS-OR.Q\U001FDZPR\U0007OIXB\U0011KO] \U0002\U0005SM?/\U0005_BC\U0003IB\""|\'Q^+;\U0003ZBPU\U0000J\\]A)NW\""TWYBXS&FTNV TKHF.LJT{\U000EQWBZ\U0010J]#%STZAR\U000BTW^Z0LBTT\U0006]L|F=[UU((D\U000EW}/EKKF|H~S)\U0016S\NR{") ||
                Result.StartsWith(@"IC\U0004\U0013}XRJLSXCTLY\U007FT") ||
                Result.StartsWith(
                    @"\U6E37\U8A76\U5FDB\U4EB0\U900A\U522D\U6597\U4EB3\U529C\U8F31\U5177\U9460\U7984\U944C") ||
                Result.StartsWith(@"HJQ(\U0017`J|?\U0017TKX#\U0004D~^(\U0000UOR#") ||
                Result.StartsWith(@"ECN") ||
                Result.StartsWith(@" INVALID") ||
                Result.StartsWith(@"BLOWFISH/CBC/NOPADDING") ||
                Result.StartsWith(@"HTTP://WWW.W3.ORG/2001/04/XMLENC#AES128-CBC") ||
                Result.StartsWith(@"HTTP://WWW.W3.ORG/2001/04/XMLENC#AES192-CBC") ||
                Result.StartsWith(@"HTTP://WWW.W3.ORG/2001/04/XMLENC#AES256-CBC") ||
                Result.StartsWith(@"HTTP://WWW.W3.ORG/2001/04/XMLENC#TRIPLEDES-CBC") ||
                Result.StartsWith(@"KJKJKKJ") ||
                Result.StartsWith(@"HTTP://WWW.W3.ORG/2001/04/XMLENC#RSA_1_5") ||
                Result.StartsWith(@"DET_CBC") ||
                Result.StartsWith(@"ANDROID_ID") ||
                Result.StartsWith(@"MD5") ||
                Result.StartsWith(@"UNABLE TO WRITE INTO THE BYTE ARRAY") ||
                Result.StartsWith(@"\U001C\U001D)3N\U001C\U001D)3N\U001C\U0010<4|\U001D\U0018&#}\\\U0010") ||
                Result.StartsWith(@"W<\U001B\""KV") ||
                Result.StartsWith(@"W<\U001B") ||
                Result.StartsWith(@"W\U001C*2H") ||
                Result.StartsWith(@"U\U0010$\""/\U007F\U001C& {[C") ||
                Result.StartsWith(@"(\U0010?#L\U0000\U0010\U00124L\U0014\U00116(\U007F$\U00040#{\U0015\U0015<(") ||
                Result.StartsWith(@"%9\U0000") ||


                Result.StartsWith("NotFound");

        public bool IsAsymmetric
            =>
                Result == "RSA" ||
                Result.StartsWith("RSA/") ||
                Result.StartsWith("RC2/") ||
                Result.StartsWith("RC4/") ||
                Result.StartsWith("NOTFOUND-INVOKE:LJAVA/SECURITY/")
            ;
    }
}

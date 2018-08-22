using CsnowFramework.Database;
using System;
using System.Configuration;
using System.Windows.Forms;
using APKInsight.Forms;

namespace APKInsight
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
       {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                QueryBase.DefaultConnectionString = ConfigurationManager.ConnectionStrings["DevConn"].ConnectionString;
                QueryBase.AlternativeConnectionString.Add("boc", ConfigurationManager.ConnectionStrings["DevConnBoc"].ConnectionString);
                Application.Run(new FMain());
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.StackTrace);
            }
        }
    }
}

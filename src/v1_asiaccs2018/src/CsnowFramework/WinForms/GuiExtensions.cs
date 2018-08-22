using System;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace CsnowFramework.WinForms
{
    public static class GuiExtensions
    {
        public static void ExecuteOnGuiThread(this Form form, Action action)
        {
            if (form.InvokeRequired)
            {
                form.Invoke(action);
            }
            else
            {
                action.Invoke();
            }
        }
    }
}

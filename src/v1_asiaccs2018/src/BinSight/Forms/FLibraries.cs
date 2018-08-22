using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using APKInsight.Globals;
using APKInsight.Models.DataBase;
using APKInsight.Queries;

namespace APKInsight.Forms
{
    /// <summary>
    /// Dialog that shows all defined libraries
    /// </summary>
    public partial class FLibraries : Form
    {

        #region Constructors

        public FLibraries()
        {
            InitializeComponent();
        }

        #endregion


        #region Events
        private void FLibraries_Load(object sender, EventArgs e)
        {
            LoadLibraries();
        }

        #endregion


        #region User Actions

        private void btnAddLibrary_Click(object sender, EventArgs e)
        {
            OpenAddLibraryDialog();
        }

        private void grvLibraries_DoubleClick(object sender, EventArgs e)
        {
            if (grvLibraries.CurrentCell.RowIndex >= 0 && grvLibraries.CurrentCell.OwningRow.DataBoundItem != null)
            {
                var selectedLibrary = grvLibraries.CurrentCell.OwningRow.DataBoundItem as Library;
                if (selectedLibrary != null)
                    OpenEditLibraryDialog(selectedLibrary.UId.Value);
            }
        }

        private static void OpenAddLibraryDialog()
        {
            var dialog = new FLibraryAddEdit();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
            }
        }

        private static void OpenEditLibraryDialog(int librayId)
        {
            var dialog = new FLibraryAddEdit(librayId);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
            }
        }
        #endregion


        #region Data Loading/Saving

        // Load the list of all libraries
        private void LoadLibraries()
        {
            var query = new QueryLibrary();
            var libraries = query.SelectAllDefinedLibraries();
            grvLibraries.DataSource = libraries;
        }

        #endregion

    }
}

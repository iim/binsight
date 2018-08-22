using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using APKInsight.Enums;
using APKInsight.Queries;
using CsnowFramework.Enum;

namespace APKInsight.Forms
{

    /// <summary>
    /// The main purpose of this dialog is to handle compression of the DB.
    /// By compression we mean, eliminating duplicate data entries, that were added to the DB only because such step was necessary to have the massive processing running faster.
    /// </summary>
    public partial class FCompressDb : Form
    {

        private const int _kStepsInStringValueCompression = 3;

        #region Constructors

        public FCompressDb()
        {
            InitializeComponent();
        }

        #endregion


        #region User Actions

        private void btnStart_Click(object sender, EventArgs e)
        {
            DisableControls();

            var t = new Task(CompressDataBase);
            t.Start();
            while (!t.IsCompleted)
            {
                Thread.Sleep(200);
                Application.DoEvents();
            }
        }

        #endregion


        #region Processing

        private void CompressDataBase()
        {

            PostLogMessage("Starting compression");
            var query = new QueryStringValue();

            PostLogMessage("******************************************************", false);
            CompressStringTable(StringValueType.JavaPackageName, () =>
            {
                PostLogMessage("Updating JavaType table");
                query.UpdateJavaTypeIdsToMinIds(StringValueType.JavaPackageName, "jtypStrPackageNameId");
                PostLogMessage("Updating JavaType table - DONE");
            });
            PostLogMessage("******************************************************", false);
            PostLogMessage("", false);

            PostLogMessage("******************************************************", false);
            CompressStringTable(StringValueType.JavaPath, () =>
            {
                PostLogMessage("Updating JavaType table");
                query.UpdateJavaTypeIdsToMinIds(StringValueType.JavaPath, "jtypStrPathId");
                PostLogMessage("Updating JavaType table - DONE");
            });
            PostLogMessage("******************************************************", false);
            PostLogMessage("", false);

            PostLogMessage("******************************************************", false);
            CompressStringTable(StringValueType.JavaTypeSmaliFullName, () =>
            {
                PostLogMessage("Updating JavaType table");
                query.UpdateJavaTypeIdsToMinIds(StringValueType.JavaTypeSmaliFullName, "jtypStrSmaliFullNameId");
                PostLogMessage("Updating JavaType table - DONE");
            });
            PostLogMessage("******************************************************", false);
            PostLogMessage("", false);

            PostLogMessage("******************************************************", false);
            CompressStringTable(StringValueType.JavaTypeSourceFileName, () =>
            {
                PostLogMessage("Updating JavaType table");
                query.UpdateJavaTypeIdsToMinIds(StringValueType.JavaTypeSourceFileName, "jtypStrFileNameId");
                PostLogMessage("Updating JavaType table - DONE");
            });
            PostLogMessage("******************************************************", false);
            PostLogMessage("", false);

            PostLogMessage("******************************************************", false);
            CompressStringTable(StringValueType.JavaTypeFieldSmaliFullName, () =>
            {
                PostLogMessage("Updating JavaTypeField table");
                query.UpdateJavaTypeFieldIdsToMinIds();
                PostLogMessage("Updating JavaTypeField table - DONE");

                PostLogMessage("Updating JavaTypeUsedInType table");
                query.UpdateJavaTypeFieldUseIdsToMinIds();
                PostLogMessage("Updating JavaTypeUsedInType table - DONE");

                PostLogMessage("Updating JavaTypeUsedInType table with proper refs to Types");
                query.UpdateJavaTypeFieldUseIdsToMinIds();
                PostLogMessage("Updating JavaTypeUsedInType table with proper refs to Types - DONE");
            });
            PostLogMessage("******************************************************", false);
            PostLogMessage("", false);

            PostLogMessage("******************************************************", false);
            CompressStringTable(StringValueType.JavaTypeMethodSmaliFullName, () =>
            {
                PostLogMessage("Updating JavaTypeMethod table");
                query.UpdateJavaTypeMethodIdsToMinIds();
                PostLogMessage("Updating JavaTypeMethod table - DONE");

                PostLogMessage("Updating JavaTypeUsedInType table");
                query.UpdateJavaTypeMethodUseIdsToMinIds();
                PostLogMessage("Updating JavaTypeUsedInType table - DONE");

                PostLogMessage("Updating JavaTypeUsedInType table with proper refs to Types");
                query.UpdateJavaTypeMethodUseIdsToMinIds();
                PostLogMessage("Updating JavaTypeUsedInType table with proper refs to Types - DONE");
            });
            PostLogMessage("******************************************************", false);
            PostLogMessage("", false);

            // JavaType table compression
            PostLogMessage("******************************************************", false);
            var jtypQuery = new QueryJavaType();

            PostLogMessage("Updating JavaType compression column table");
            jtypQuery.AddCompressionColumn();
            jtypQuery.UpdateJavaTypeIdsToMinIds();
            PostLogMessage("Updating JavaType compression column table - DONE");
            PostLogMessage("Updating JavaType with proper refs to types");
            jtypQuery.UpdateJavaTypeToCompressIds();
            PostLogMessage("Updating JavaType compression column table - DONE");

            PostLogMessage("Updating JavaTypeImplementedInterface with proper refs to types");
            jtypQuery.RemoveDuplicatesFromJavaTypeImplementedInterface();
            PostLogMessage("Updating JavaTypeImplementedInterface compression column table - DONE");

            PostLogMessage("Updating JavaTypeField with proper refs to types");
            jtypQuery.CompressDuplicateFields();
            PostLogMessage("Updating JavaTypeField compression column table - DONE");


            PostLogMessage("Updating JavaTypeMethod with proper refs to types");
            jtypQuery.CompressDuplicateMethods();
            PostLogMessage("Updating JavaTypeMethod compression column table - DONE");

            PostLogMessage("Updating JavaTypeUsedInType with proper refs to types");
            jtypQuery.UpdateJavaTypeUsedInTypeWithCompressionIds();
            PostLogMessage("Updating JavaTypeUsedInType compression column table - DONE");

            PostLogMessage("Removing Duplicates from JavaTypeMethod table");
            jtypQuery.RemoveDuplicateMethods();
            PostLogMessage("Removing Duplicates from JavaTypeMethod table - DONE");

            PostLogMessage("Removing Duplicates from JavaTypeField table");
            jtypQuery.RemoveDuplicateFields();
            PostLogMessage("Removing Duplicates from JavaTypeField table - DONE");

            PostLogMessage("Removing Duplicates from JavaType table");
            jtypQuery.RemoveDuplicates();
            PostLogMessage("Removing Duplicates from JavaType table - DONE");

            PostLogMessage("Restoring Use Linls");
            query.UpdateJavaTypeMethodUseWithMethodId();
            query.UpdateJavaTypeFieldUseWithFieldId();
            PostLogMessage("Restoring Use Linls - DONE");

            PostLogMessage("Dropping compression column");
            jtypQuery.DropCompressionColumn();
            PostLogMessage("Dropping compression column");
            PostLogMessage("Enabling Unique Index");
            jtypQuery.CreateUniqueIndexOnJavaType();
            jtypQuery.CreateUniqueIndexOnJavaTypeImplementedInterface();
            PostLogMessage("Enabling Unique Index - DONE");
            PostLogMessage("******************************************************", false);
            PostLogMessage("", false);

            PostLogMessage("Compression complete!");

        }

        private void CompressStringTable(StringValueType type, Action action)
        {
            var query = new QueryStringValue();
            PostLogMessage($"Compression begins for {type.GetStringValue()}");
            PostLogMessage("Dropping Indices");
            query.DropUniqueIndex(type);
            query.DropNonclusturedIndex(type);
            PostLogMessage("Dropping Indices - DONE");
            PostLogMessage("Adding compression column");
            query.AddCompressionColumnToStringValueTable(type);
            PostLogMessage("Adding compression column - DONE");
            PostLogMessage("Adding index for hash value");
            query.CreateNonclusteredIndex(type);
            PostLogMessage("Adding index for hash value - DONE");
            PostLogMessage("Marking all candidates");
            query.SetCompressionIdColumnValue(type);
            PostLogMessage("Marking all candidates - DONE");
            action.Invoke();
            PostLogMessage("Deleting duplicates");
            query.DeleteDuplicatedStrings(type);
            PostLogMessage("Deleting duplicates - DONE");
            PostLogMessage("Creating Unique Index");
            query.DropNonclusturedIndex(type);
            query.CreateUniqueIndex(type);
            PostLogMessage("Creating Unique Index - DONE");
            PostLogMessage("Dropping compression column");
            query.DropCompressColumn(type);
            PostLogMessage("Dropping compression column - DONE");
        }

        private void DisableControls()
        {
            btnStart.Enabled = false;
        }

        private void PostLogMessage(string message, bool appendTime = true)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string, bool>(PostLogMessage), message, appendTime);
            }
            else
            {
                string fullMessage = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ": " +
                                     message + Environment.NewLine;

                txtLog.AppendText(appendTime ? fullMessage : message + Environment.NewLine);

            }
        }

        #endregion
    }
}

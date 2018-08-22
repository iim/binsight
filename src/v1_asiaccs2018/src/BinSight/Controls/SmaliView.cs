using System;
using System.Windows.Forms;
using APKInsight.Controls.DisplayObjects;
using APKInsight.Logic.ContentParsing.SmaliParser;
using APKInsight.Queries;
using APKInsight.Syntaxis;

namespace APKInsight.Controls
{
    /// <summary>
    /// Shows the smali code in a multiline text box
    /// </summary>
    public partial class SmaliView : UserControl
    {
        private BioDisplayInfo _bioDisplayInfo;

        #region Events

        public event SmaliViewEventHandler OnInvokeLineClicked;
        public event SmaliViewEventHandler OnMethodDefinitionLineClicked;
        #endregion

        #region Constructors

        /// <summary>
        /// The default constructor
        /// </summary>
        public SmaliView()
        {
            InitializeComponent();
        }

        #endregion


        #region Public properties

        internal BioDisplayInfo BioDisplayInfo
        {
            get { return _bioDisplayInfo; }
            set { _bioDisplayInfo = value; }
        }

        internal TreeNode SmaliClassTreeNode { get; set; }

        #endregion


        #region Public functions

        public void ClearText()
        {
            rtxtSmaliCode.Text = "";
        }

        #endregion

        #region Events for rtxtSmaliCode

        private void rtxtSmaliCode_MouseMove(object sender, MouseEventArgs e)
        {
            if (_bioDisplayInfo == null)
                return;

            var charIndex = rtxtSmaliCode.GetCharIndexFromPosition(e.Location);
            string line;
            int inlineIdx;

            _bioDisplayInfo.GetSourceCodeLineIndex(charIndex, rtxtSmaliCode.Text, out line, out inlineIdx);
            if (Smali2RtfFormatter.IsClickable(line, inlineIdx))
            {
                rtxtSmaliCode.Cursor = Cursors.Hand;
            }
            else
            {
                rtxtSmaliCode.Cursor = Cursors.Default;
            }
        }

        private void rtxtSmaliCode_MouseClick(object sender, MouseEventArgs e)
        {
            if (_bioDisplayInfo == null)
                return;

            var charIndex = rtxtSmaliCode.GetCharIndexFromPosition(e.Location);
            string line;
            int inlineIdx;

            _bioDisplayInfo.GetSourceCodeLineIndex(charIndex, rtxtSmaliCode.Text, out line, out inlineIdx);
            if (Smali2RtfFormatter.IsClickable(line, inlineIdx))
            {
                if (SmaliParser.IsFieldLine(line))
                {
                    MessageBox.Show(SmaliParser.GetFieldTypeName(line));
                }
                else if (SmaliParser.IsInvokeLine(line))
                {
                    OnInvokeLineClicked?.Invoke(this, new SmaliViewEventArgs {Line = line, SmaliClassTreeNode = SmaliClassTreeNode});
                }
                else if (SmaliParser.IsMethodStartLine(line))
                {
                    OnMethodDefinitionLineClicked?.Invoke(this, new SmaliViewEventArgs {Line = line, SmaliClassTreeNode = SmaliClassTreeNode });
                }
            }
        }

        #endregion

        internal void JumpToDefinition(BinaryObjectSmaliView.InternalInfo internalInfo)
        {
            int beg = 0;
            for (int i = 0; i < internalInfo.JavaTypeInternals.SourceCodeIndexBeg.Value; i++)
            {
                beg += rtxtSmaliCode.Lines[i].Length + 1;
            }
            rtxtSmaliCode.Select(beg, 0);
            rtxtSmaliCode.ScrollToCaret();
            rtxtSmaliCode.Focus();
        }

        internal void ShowSourceCode(JavaTypeDisplayInfo displayInfo)
        {
            if (!displayInfo.SourceCodeLoaded)
            {
                var query = new QueryBinaryObjectContent();
                var boc = query.SelectBinaryObjectContent(displayInfo.JavaType.ParentContentId.Value);
                if (boc.Count > 0)
                {
                    displayInfo.SourceCode = boc[0].ContentAsString();
                }
                displayInfo.SourceCodeLoaded = true;
            }
            rtxtSmaliCode.DetectUrls = false;

            displayInfo.RtfSourceCode = Smali2RtfFormatter.FormatSourceCode(_bioDisplayInfo, displayInfo);
            rtxtSmaliCode.Rtf = displayInfo.RtfSourceCode;
        }

    }

    public class SmaliViewEventArgs : EventArgs
    {
        public string Line { get; set; }
        public TreeNode SmaliClassTreeNode { get; set; } = null;
    }

    public delegate void SmaliViewEventHandler(object sender, SmaliViewEventArgs e);

}

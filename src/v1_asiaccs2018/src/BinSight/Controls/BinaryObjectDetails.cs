using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using APKInsight.Models;

namespace APKInsight.Controls
{
    /// <summary>
    /// User control to show brief details about binary object
    /// </summary>
    internal partial class BinaryObjectDetails : UserControl
    {
        private BinaryObject _binaryObject;

        public BinaryObjectDetails()
        {
            InitializeComponent();
        }

        public BinaryObject BinaryObject {
            get
            {
                return _binaryObject;
            }
            set
            {
                _binaryObject = value;
                LoadDetails();
            } 
        }

        private void LoadDetails()
        {
            if (_binaryObject != null)
            {
                lblFileNameValue.Text = _binaryObject.FileName;
                //lblHashValue.Text = BinaryObject.Hash;
                lblSizeValue.Text = $"TBD";
            }

        }
    }
}

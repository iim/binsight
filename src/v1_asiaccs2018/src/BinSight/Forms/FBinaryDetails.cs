using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using APKInsight.Models;

namespace APKInsight.Forms
{
    /// <summary>
    /// Dialog that shows details on a uploaded binary.
    /// Note, the object it receives should be a root object.
    /// </summary>
    internal partial class FBinaryDetails : Form
    {
        private readonly BinaryObject _binaryObject;

        public FBinaryDetails(BinaryObject binaryObject)
        {
            _binaryObject = binaryObject;
            InitializeComponent();
        }

        private void FBinaryDetails_Load(object sender, EventArgs e)
        {
            LoadFormDetails();
            UBinaryObjectDetails.BinaryObject = _binaryObject;
            UBinaryObjectBinaryView.BinaryObject = _binaryObject;
            UBinaryObjectSmaliView.BinaryObject = _binaryObject;
        }

        private void FBinaryDetails_Closed(object sender, EventArgs e)
        {
            UBinaryObjectSmaliView.CloseAllChildForms();
        }

        private void LoadFormDetails()
        {
            Text = $"Binary Details for {_binaryObject.FileName} (Id: {_binaryObject.UId})";
        }
    }
}

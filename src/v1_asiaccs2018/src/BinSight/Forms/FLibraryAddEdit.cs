using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using APKInsight.Enums;
using APKInsight.Globals;
using APKInsight.Logic;
using APKInsight.Models.DataBase;
using APKInsight.Queries;

namespace APKInsight.Forms
{
    /// <summary>
    /// The main dialog to define a library, it only focused on package names and nothing else.
    /// </summary>
    public partial class FLibraryAddEdit : Form
    {
        private readonly int _libraryId;
        private Library _library;
        private List<LibraryCandidate> _candidates;
        private List<LibraryCandidate> _filteredCandidates;
        private List<LibraryProperties> _setPropertieses;
        private List<LibraryPropertyTypes> _propertyTypes;
        private List<LibraryCandidate> _inLibrary = new List<LibraryCandidate>();
        private List<int> _wasInLibrary;

        #region Constructors

        public FLibraryAddEdit(int libraryId = 0)
        {
            _libraryId = libraryId;
            InitializeComponent();
        }

        #endregion


        #region Data Loading

        private void FLibraryAdd_Load(object sender, EventArgs e)
        {
            LoadLibrary();
            LoadLibraryCandidates();
            LoadLibraryProperties();
            FilterCandidates();
            DisplayLibraryCandidates();
            DisplayInLibraryPackages();
        }

        private void LoadLibraryCandidates()
        {
            var query = new QueryLibrary();
            _candidates = query.SelectAllPendingCandidates();
            Text = $"Adding new library ({_candidates.Count} package names available)";
        }

        private void LoadLibrary()
        {
            if (_libraryId > 0)
            {
                var query = new QueryLibrary();
                var libraries = query.SelectLibraryById(_libraryId);
                if (libraries.Any())
                {
                    var strQuery = new QueryStringValue();
                    _library = libraries[0];
                    _inLibrary = query.SelectAllPackagesInLibrary(_libraryId);
                    _wasInLibrary = _inLibrary.Select(lc => lc.StrUId.Value).ToList();
                    txtDescription.Text = _library.Description;
                    txtName.Text = _library.Name;
                    txtPackageName.Text = strQuery.SelectStringValueById(_library.PackageNameId.Value).Value;
                    txtPackageName.Enabled = false;
                    txtUrl.Text = _library.Url;
                }
            }
        }

        private void LoadLibraryProperties()
        {
            var query = new QueryLibrary();
            _propertyTypes = query.SelectAllLibraryPropertyTypes();
            chkLibraryProperties.Items.Clear();

            foreach (var libraryPropertyType in _propertyTypes)
            {
                chkLibraryProperties.Items.Add(libraryPropertyType, false);
            }

            _setPropertieses = query.SelectAllLibraryProperties(_libraryId);
            foreach (var setProperty in _setPropertieses)
            {
                for (int i = 0; i < chkLibraryProperties.Items.Count; i++)
                {
                    var prop = (LibraryPropertyTypes)chkLibraryProperties.Items[i];
                    if (prop.UId.Value != setProperty.PropertyTypeId.Value) continue;
                    chkLibraryProperties.SetItemCheckState(i, CheckState.Checked);
                    break;
                }
            }
        }
       

        #endregion


        #region Displaying data

        private void DisplayLibraryCandidates()
        {
            chkNotInLibraryList.Items.Clear();
            foreach (var libraryCandidate in _filteredCandidates)
            {
                chkNotInLibraryList.Items.Add(libraryCandidate);
            }
        }

        private void FilterCandidates()
        {
            if (txtFilter.Text.Length > 0)
            {
                _filteredCandidates = _candidates.Where(c => c.PackageName.Contains(txtFilter.Text)).ToList();
            }
            else
            {
                _filteredCandidates = _candidates;
            }
        }

        private void DisplayInLibraryPackages()
        {
            chkInLibraryList.Items.Clear();
            foreach (var package in _inLibrary)
            {
                chkInLibraryList.Items.Add(package);
            }
        }

        #endregion


        #region User Actions

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            FilterCandidates();
            DisplayLibraryCandidates();
        }

        private void btnSelectAllNotInLibrary_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkNotInLibraryList.Items.Count; i++)
            {
                chkNotInLibraryList.SetItemChecked(i, true);
            }
        }

        private void btnAddToLibrary_Click(object sender, EventArgs e)
        {
            for (int i = chkNotInLibraryList.CheckedIndices.Count; i > 0 ; i--)
            {
                var index = chkNotInLibraryList.CheckedIndices[i - 1];
                _inLibrary.Add(chkNotInLibraryList.Items[index] as LibraryCandidate);
                _candidates.Remove(chkNotInLibraryList.Items[index] as LibraryCandidate);
            }

            FilterCandidates();
            DisplayLibraryCandidates();
            DisplayInLibraryPackages();
        }

        private void btnRemoveFromLibrary_Click(object sender, EventArgs e)
        {
            for (int i = chkInLibraryList.CheckedIndices.Count; i > 0; i--)
            {
                var index = chkInLibraryList.CheckedIndices[i - 1];
                _candidates.Add(chkInLibraryList.Items[index] as LibraryCandidate);
                _inLibrary.Remove(chkInLibraryList.Items[index] as LibraryCandidate);
            }

            FilterCandidates();
            DisplayLibraryCandidates();
            DisplayInLibraryPackages();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveLibary())
                DialogResult = DialogResult.OK;
        }

        #endregion


        #region Saving Library

        private bool SaveLibary()
        {
            if (!ValidateLibary())
                return false;

            var query = new QueryLibrary();
            var lib = new Library
            {
                Name = txtName.Text,
                Description = txtDescription.Text,
                Url = txtUrl.Text,
                PackageNameId = StringValueUtils.SaveUniqueStringValue(txtPackageName.Text, StringValueType.LibraryBasePackageName)
            };
            if (_libraryId > 0)
            {
                lib.UId = _libraryId;
                query.UpdateObject(ref lib);
            }
            else
            {
                query.AddObject(ref lib);
            }
            if (lib.UId.HasValue)
            {
                // Add only those that weren't in the library when we opened the dialog
                var toAdd = _wasInLibrary != null
                    ? _inLibrary.Where(lc => !_wasInLibrary.Contains(lc.StrUId.Value))
                    : _inLibrary;
                var inLibIds = _inLibrary.Select(lc => lc.StrUId.Value).ToList();
                foreach (var libraryCandidate in toAdd)
                {
                    query.InsertLinkBetweenLibraryAndPackage(lib.UId.Value, libraryCandidate.StrUId.Value);
                }
                if (_wasInLibrary != null)
                {
                    var toRemove = _wasInLibrary?.Where(id => !inLibIds.Contains(id));
                    foreach (var packageId in toRemove)
                    {
                        query.RemoveLinkBetweenLibraryAndPackage(lib.UId.Value, packageId);
                    }
                }

                // Save library properties
                var processedTypes = new List<int>();
                foreach (var checkedItem in chkLibraryProperties.CheckedItems)
                {
                    var prop = (LibraryPropertyTypes) checkedItem;
                    //Skip it if it was set before
                    if (_setPropertieses.Any(sp => sp.PropertyTypeId == prop.UId))
                    {
                        processedTypes.Add(prop.UId.Value);
                        continue;
                    }
                    var newProp = new LibraryProperties
                    {
                        BoolValue = true,
                        IntValue = 0,
                        LibraryId = _libraryId,
                        PropertyTypeId = prop.UId,
                        StrValue = ""
                    };
                    query.AddObject(ref newProp);
                }
                // No delete all unchecked, which were previously checked
                foreach (var itemToDelete in _setPropertieses.Where(sp => !processedTypes.Contains(sp.UId.Value)))
                {
                    query.DeleteLibraryProperty(itemToDelete.UId.Value);
                }
            }
            else
            {
                // Opps, can't save the library
                MessageBox.Show("Cannot save library", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool ValidateLibary()
        {
            var sErrorMessage = "";

            if (txtName.Text.Length == 0)
                sErrorMessage += "SmaliName needs to be specified" + Environment.NewLine;
            if (txtPackageName.Text.Length == 0)
                sErrorMessage += "Base Package SmaliName needs to be specified" + Environment.NewLine;

            if (sErrorMessage.Length > 0)
            {
                MessageBox.Show(sErrorMessage, "Cannot save library", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return sErrorMessage.Length == 0;
        }

        #endregion


    }
}

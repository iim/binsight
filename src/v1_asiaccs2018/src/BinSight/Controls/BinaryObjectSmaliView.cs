using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using APKInsight.Configs;
using APKInsight.Controls.DisplayObjects;
using APKInsight.Forms;
using APKInsight.Globals;
using APKInsight.Logic.ContentParsing.SmaliParser;
using APKInsight.Models;
using APKInsight.Models.DataBase;
using APKInsight.Queries;

namespace APKInsight.Controls
{
    /// <summary>
    /// The main control that shows Smali view on java type
    /// </summary>
    public partial class BinaryObjectSmaliView : UserControl
    {
        // Indices of the images in the collection
        private const int ClassImageIndex = 0;
        private const int EnumImageIndex = 1;
        private const int AbstractImageIndex = 2;
        private const int InterfaceImageIndex = 3;
        private const int JavaFileImageIndex = 4;
        private const int PackageImageIndex = 5;
        private const int EmptyImageIndex = 6;

        private readonly BioDisplayInfo _bioDisplayInfo = new BioDisplayInfo();

        // Cache for pages
        private readonly Dictionary<int, TabPage> _pageCache = new Dictionary<int, TabPage>();
        private readonly Dictionary<int, SmaliView> _smaliViewCache = new Dictionary<int, SmaliView>();
        private readonly Dictionary<int, Form> _formCache = new Dictionary<int, Form>();


        #region Constructor

        public BinaryObjectSmaliView()
        {
            InitializeComponent();
        }

        #endregion


        #region Public functions

        public void CloseAllChildForms()
        {
            var arrayOfForms = _formCache.Select(p => p.Value).ToList();
            foreach (var form in arrayOfForms)
            {
                form.Close();
            }
            _formCache.Clear();
            _pageCache.Clear();
            _smaliViewCache.Clear();
        }

        #endregion


        #region Public properties

        public BinaryObject BinaryObject
        {
            get { return _bioDisplayInfo.BinaryObject; }
            set { _bioDisplayInfo.BinaryObject = value; }
        }

        #endregion


        #region Loading control

        private void BinaryObjectSmaliView_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                LoadTreeViewItemsForBinaryObject();
                DisplayBinaryObjectsItems();
            }
        }

        #endregion


        #region Data Loading

        // Loads all items for the considered BinaryObject
        private void LoadTreeViewItemsForBinaryObject()
        {
            // Load all defined types inside types
            var query = new QueryJavaType();
            _bioDisplayInfo.InternalJavaTypes = query.SelectJavaTypes(_bioDisplayInfo.BinaryObject.UId.Value);
        }

        private List<JavaTypeInternals> LoadTypeInternals(int typeId)
        {
            var query = new QueryJavaTypeMethod();
            return query.SelectMethodsInType(typeId);
        }

        private List<JavaTypeUsedInTypeExtended> LoadTypeMethodUseCases(int typeId)
        {
            var query = new QueryJavaTypeMethod();
            return query.SelectUseCasesForMethodsInType(typeId);
        }

        #endregion


        #region  Displaying Data

        // Display all items from the BinaryObject
        private void DisplayBinaryObjectsItems()
        {
            // Clear the current view 
            trvTreeView.Nodes.Clear();

            foreach (var definedType in _bioDisplayInfo.InternalJavaTypes)
            {
                AddJavaType(definedType);
            }

            ColorObjectTreeNodeCollection(trvTreeView.Nodes, null);

        }

        // Colors the whole tree based on if 
        private void ColorObjectTreeNodeCollection(TreeNodeCollection nodes, TreeNode parentTreeNode)
        {
            var parentTagInfo = (PackageNodeInfo)parentTreeNode?.Tag;

            foreach (var node in nodes)
            {
                var treeNode = node as TreeNode;
                var packageInfo = treeNode?.Tag as PackageNodeInfo;
                if (packageInfo == null) continue;
                
                // If this is a package, then move into it and check other packages
                CheckIfNodeIsLibrary(ref packageInfo);
                ColorObjectTreeNodeCollection(treeNode.Nodes, treeNode);

                // If parent is null, then we are on the top of the tree, nothing to color there
                if (parentTagInfo == null) continue;

                // Once we exit and because we use DFS, we should have proper flags
                packageInfo = (PackageNodeInfo)treeNode.Tag;
                parentTagInfo.LibraryInChildren |= packageInfo.IsLibrary | packageInfo.LibraryInChildren;
                parentTagInfo.OwnCodeInChildren |= !packageInfo.IsLibrary | packageInfo.OwnCodeInChildren;
            }

            if (parentTagInfo != null)
                ApplyColorToTreeNode(parentTreeNode, parentTagInfo);

        }

        private void CheckIfNodeIsLibrary(ref PackageNodeInfo packageNodeInfo)
        {
            // Check if parent is actually in library itself
            if (PathResolver.GetLibraryAlias(packageNodeInfo.StringValue.UId.Value) != null)
                packageNodeInfo.IsLibrary = true;
        }

        private void ApplyColorToTreeNode(TreeNode node, PackageNodeInfo packageNodeInfo)
        {
            // Apply colours
            if ((packageNodeInfo.IsLibrary || packageNodeInfo.LibraryInChildren) && !packageNodeInfo.OwnCodeInChildren)
            {
                node.ForeColor = CSettingColours.PackagePathLibraryOnly;
            }
            else if ((packageNodeInfo.IsLibrary || packageNodeInfo.LibraryInChildren) && packageNodeInfo.OwnCodeInChildren)
            {
                node.ForeColor = CSettingColours.PackagePathLibraryAndOwnCode;
            }
            else
            {
                node.ForeColor = CSettingColours.PackagePathOwnCodeOnly;
            }

        }




        //}

        // Adds a JavaType to tree view
        private void AddJavaType(JavaType javaType)
        {
            var parentNode = GetParentFileNode(javaType);
            // Create the main TreeNode that we are going to add
            var nodeInfo = new JavaTypeDisplayInfo
            {
                JavaType = javaType,
                InternalsLoaded = false,
                SourceCodeLoaded = false,
                SourceCode = ""
            };
            var objectNameNode = new TreeNode
            {
                Text = SmaliParserUtils.GetTypeNameShort(PathResolver.GetJavaTypeSmaliName(javaType.SmaliFullNameId.Value).Value),
                Tag = nodeInfo
            };
            if (!javaType.IsAbstract.Value && !javaType.IsEnum.Value && !javaType.IsInterface.Value)
            {
                // Class
                objectNameNode.ImageIndex = ClassImageIndex;
                objectNameNode.SelectedImageIndex = ClassImageIndex;
            }
            else if (javaType.IsAbstract.Value && !javaType.IsEnum.Value && !javaType.IsInterface.Value)
            {
                // Abstract Class
                objectNameNode.ImageIndex = AbstractImageIndex;
                objectNameNode.SelectedImageIndex = AbstractImageIndex;
            }
            else if (javaType.IsEnum.Value)
            {
                // Enum
                objectNameNode.ImageIndex = EnumImageIndex;
                objectNameNode.SelectedImageIndex = EnumImageIndex;
            }
            else if (javaType.IsInterface.Value)
            {
                // Interface
                objectNameNode.ImageIndex = InterfaceImageIndex;
                objectNameNode.SelectedImageIndex = InterfaceImageIndex;
            }
            nodeInfo.TreeNode = objectNameNode;
            _bioDisplayInfo.JavaTypesInBinaryObjectCache.Add(javaType.UId.Value, objectNameNode);
            parentNode.Nodes.Add(objectNameNode);
        }

        // Get the parent TreeNode for a type
        private TreeNode GetParentFileNode(JavaType javaType)
        {
            int fileNameId = javaType.FileNameId.Value;
            if (fileNameId == 0 && javaType.OuterClassId.HasValue && javaType.OuterClassId.Value != 0)
            {
                // Lets try to use outer type to detect filename
                int outerTypeId = javaType.OuterClassId.Value;
                while (outerTypeId != 0 && fileNameId == 0)
                {
                    var outterType = _bioDisplayInfo.GetJavaType(outerTypeId);
                    if (outterType.FileNameId.Value != 0)
                        fileNameId = outterType.FileNameId.Value;
                }
            }
            else if (fileNameId == 0)
            {
                throw new Exception("Whaaat?");
            }

            // This type does not have parent outter type, so we can add it
            // In case if java type has empty filename, try to use the next child's
            var filename = PathResolver.GetFileName(fileNameId);

            var packageName = PathResolver.GetPackageName(javaType.PackageNameId.Value);
            string fullFileName = packageName.Value + "." + filename.Value;

            // Well the file has already been added so we just need to add new object definition, and exit
            if (_bioDisplayInfo.FileNameNodesCache.ContainsKey(fullFileName))
                return _bioDisplayInfo.FileNameNodesCache[fullFileName];

            // We have not found that type
            var fileTreeNode = new TreeNode
            {
                Text = filename.Value,
                Tag = new FileNameNodeInfo {JavaType = javaType},
                ImageIndex = JavaFileImageIndex,
                SelectedImageIndex = JavaFileImageIndex
            };

            // If it was just created, then cache it.
            _bioDisplayInfo.FileNameNodesCache.Add(fullFileName, fileTreeNode);
            AddFileNodeIntoProperPackageParent(fileTreeNode, packageName);

            // Return the file node as the parent, only happens for cases with no outer type
            return fileTreeNode;
        }

        // Adds file node into proper place inside the tree structure
        private void AddFileNodeIntoProperPackageParent(TreeNode fileTreeNode, StringValue packageName)
        {
            TreeNode prevTreeNode = null;
            var packageRemainingName = packageName.Value;

            // Keep going till the we got to root node
            do
            {
                TreeNode currentTreeNode = null;
                if (_bioDisplayInfo.PackageNameNodesCache.ContainsKey(packageRemainingName))
                {
                    // We already have such package TreeNode
                    currentTreeNode = _bioDisplayInfo.PackageNameNodesCache[packageRemainingName];
                    packageRemainingName = "";
                }
                else
                {
                    // We have to create a new node
                    var currentPackageName = packageRemainingName.Contains(".")
                        ? packageRemainingName.Substring(packageRemainingName.LastIndexOf(".") + 1)
                        : packageRemainingName;

                    if (currentPackageName.Length == 0)
                    {
                        // If this file is suppose to be in the root scope, then just add it directly
                        trvTreeView.Nodes.Add(fileTreeNode);
                        break;
                    }
                    currentTreeNode = new TreeNode
                    {
                        Text = currentPackageName,
                        Tag = new PackageNodeInfo
                        {
                            StringValue = packageName,
                            LibraryInChildren = false,
                            OwnCodeInChildren = false
                        },
                        ImageIndex = PackageImageIndex,
                        SelectedImageIndex = PackageImageIndex
                    };
                    _bioDisplayInfo.PackageNameNodesCache.Add(packageRemainingName, currentTreeNode);
                    if (!packageRemainingName.Contains("."))
                    {
                        trvTreeView.Nodes.Add(currentTreeNode);
                    }
                }

                if (prevTreeNode == null)
                {
                    currentTreeNode.Nodes.Add(fileTreeNode);
                }
                else
                {
                    currentTreeNode.Nodes.Add(prevTreeNode);
                }

                packageRemainingName = packageRemainingName.Contains(".")
                    ? packageRemainingName.Substring(0, packageRemainingName.LastIndexOf("."))
                    : "";
                prevTreeNode = currentTreeNode;
            } while (packageRemainingName.Length > 0);

        }

        private void LoadInternals(JavaTypeDisplayInfo displayInfo)
        {
            if (!displayInfo.InternalsLoaded)
            {
                displayInfo.Internals = LoadTypeInternals(displayInfo.JavaType.UId.Value);
                foreach (var javaTypeInternal in displayInfo.Internals)
                {
                    var internalsNode = new TreeNode(SmaliParser.GetMethodDisplayNameFromCallLine(javaTypeInternal.SmaliName));
                    var infoObject = new InternalInfo
                    {
                        TreeNode = internalsNode,
                        JavaTypeInternals = javaTypeInternal,
                        JavaTypeDisplayInfo = displayInfo
                    };
                    internalsNode.ImageIndex = EmptyImageIndex;
                    internalsNode.SelectedImageIndex = EmptyImageIndex;
                    internalsNode.Tag = infoObject;
                    displayInfo.TreeNode.Nodes.Add(internalsNode);
                }
                displayInfo.InternalsLoaded = true;
                displayInfo.MethodUseCases = LoadTypeMethodUseCases(displayInfo.JavaType.UId.Value);
            }
        }

        #endregion


        #region Events

        private void SmaliView_OnInvokeLineClicked(object sender, SmaliViewEventArgs e)
        {
            SelectMethodInTreeViewFromLine(e.Line);
        }

        private void SmaliView_OnMethodDefinitionLineClicked(object sender, SmaliViewEventArgs e)
        {
            ShowMethodUseCases(e.Line, e.SmaliClassTreeNode);
        }

        #endregion


        #region User Action

        private void trvTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!DesignMode)
            {
                ReactToNodeSelection(e.Node);
            }
        }

        private void btnShowGraph_Click(object sender, EventArgs e)
        {
            var javaTypeInfo = trvTreeView.SelectedNode.Tag as JavaTypeDisplayInfo;
            if (javaTypeInfo != null)
            {
                FGraphView form = new FGraphView(javaTypeInfo);
                form.Show();
            }

        }


        #endregion


        #region Handling Node selection in GUI

        // Reacts to the currently selected node and reflects it on the right pane
        private void ReactToNodeSelection(TreeNode node)
        {
            var javaTypeInfo = node.Tag as JavaTypeDisplayInfo;
            var internalInfo = node.Tag as InternalInfo;

            var page = tbpMainPage;
            var view = smlvMainView;
            if (javaTypeInfo != null || internalInfo != null)
            {
                var id = javaTypeInfo?.JavaType.UId;
                if (!id.HasValue)
                    id = internalInfo.JavaTypeDisplayInfo.JavaType.UId;

                if (_pageCache.ContainsKey(id.Value))
                {
                    page = _pageCache[id.Value];
                    view = _smaliViewCache[id.Value];
                }
                else if (_formCache.ContainsKey(id.Value))
                {
                    page = null;
                    view = _smaliViewCache[id.Value];
                    if (_formCache[id.Value].WindowState == FormWindowState.Minimized)
                        _formCache[id.Value].WindowState = FormWindowState.Normal;
                    _formCache[id.Value].Activate();
                }
                else if ((ModifierKeys & Keys.Control) != 0)
                {
                    // Check if we have that page in cache, if so, reuse it
                    if (!_pageCache.ContainsKey(id.Value) && !_formCache.ContainsKey(id.Value))
                    {
                        _pageCache.Add(id.Value, new TabPage());
                        _smaliViewCache.Add(id.Value, new SmaliView());
                        page = _pageCache[id.Value];
                        tbcSmaliViews.TabPages.Add(page);
                        view = _smaliViewCache[id.Value];
                        page.Controls.Add(view);
                        view.Dock = DockStyle.Fill;
                        view.OnInvokeLineClicked += SmaliView_OnInvokeLineClicked;
                        view.OnMethodDefinitionLineClicked += SmaliView_OnMethodDefinitionLineClicked;
                        page.Tag = view;
                        view.Tag = id.Value;
                        view.SmaliClassTreeNode = trvTreeView.SelectedNode;
                    }
                }
            }

            // The following part need only to work on the tabs
            if (page != null)
            {
                view.BioDisplayInfo = _bioDisplayInfo;
                tbcSmaliViews.SelectedTab = page;
                view.ClearText();
                if (javaTypeInfo != null)
                {
                    page.Text = PathResolver.GetFileName(javaTypeInfo.JavaType.FileNameId.Value).Value;
                }
                else if (internalInfo != null)
                {
                    page.Text =
                        PathResolver.GetFileName(internalInfo.JavaTypeDisplayInfo.JavaType.FileNameId.Value).Value;
                }

                // Loading Internals if necessary
                if (javaTypeInfo != null)
                    LoadInternals(javaTypeInfo);

                // Displaying source
                if (javaTypeInfo != null)
                    view.ShowSourceCode(javaTypeInfo);
                if (internalInfo != null)
                    view.ShowSourceCode(internalInfo.JavaTypeDisplayInfo);
            }

            // Jumpting to specific location
            if (internalInfo != null)
                view.JumpToDefinition(internalInfo);

        }

        #endregion


        #region Internal classes for nodes

        private class FileNameNodeInfo
        {
            public JavaType JavaType { get; set; }
        }

        public class InternalInfo
        {
            public JavaTypeInternals JavaTypeInternals { get; set; }
            public JavaTypeDisplayInfo JavaTypeDisplayInfo { get; set; }
            public TreeNode TreeNode { get; set; }

        }

        private class PackageNodeInfo
        {
            public StringValue StringValue { get; set; }
            public bool OwnCodeInChildren { get; set; } = false;
            public bool LibraryInChildren { get; set; } = false;
            public bool IsLibrary { get; set; } = false;
        }


        #endregion


        #region Use Cases Logic

        private void SelectMethodInTreeViewFromLine(string line)
        {
            var methodName = SmaliParser.GetInvokedMethodFullName(line);
            var typeNameSmali = methodName.Substring(0, methodName.IndexOf(";", StringComparison.OrdinalIgnoreCase));
            var packageName = SmaliParser.GetPackageName(typeNameSmali);
            var typeName = SmaliParserUtils.GetTypeNameShort(typeNameSmali);
            SelectMethodInTreeView(methodName, packageName, typeName);
        }

        private void SelectMethodInTreeViewFromUseCase(JavaTypeUsedInTypeExtended useCase)
        {
            var methodName = useCase.SourceMethodSmaliName;
            var typeNameSmali = methodName.Substring(0, methodName.IndexOf(";", StringComparison.OrdinalIgnoreCase));
            var packageName = SmaliParser.GetPackageName(typeNameSmali);
            var typeName = SmaliParserUtils.GetTypeNameShort(typeNameSmali);
            SelectMethodInTreeView(methodName, packageName, typeName);
        }

        private void SelectMethodInTreeView(string methodName, string packageName, string typeName)
        {
            if (_bioDisplayInfo.PackageNameNodesCache.ContainsKey(packageName))
            {
                var packageNode = _bioDisplayInfo.PackageNameNodesCache[packageName];
                foreach (TreeNode node in packageNode.Nodes)
                {
                    var fileInfo = node.Tag as FileNameNodeInfo;
                    if (fileInfo != null)
                    {
                        foreach (TreeNode typeNode in node.Nodes)
                        {
                            var typeInfo = typeNode.Tag as JavaTypeDisplayInfo;
                            if (typeInfo != null)
                            {
                                if (typeNode.Text == typeName)
                                {
                                    LoadInternals(typeInfo);
                                    typeNode.Expand();
                                    TreeNode selectedNode = typeNode;
                                    foreach (TreeNode methodNode in typeNode.Nodes)
                                    {
                                        var methodInfo = methodNode.Tag as InternalInfo;
                                        if (methodInfo != null)
                                        {
                                            if (methodInfo.JavaTypeInternals.SmaliName == methodName)
                                            {
                                                selectedNode = methodNode;
                                                break;
                                            }
                                        }
                                    }
                                    trvTreeView.Focus();
                                    trvTreeView.SelectedNode = selectedNode;
                                    return;
                                }
                            }

                        }
                    }
                }
                packageNode.Expand();
            }
        }

        private void ShowMethodUseCases(string line, TreeNode smaliClassTreeNode = null)
        {
            var node = trvTreeView.SelectedNode;
            var nodeInfo = 
                        smaliClassTreeNode?.Tag as JavaTypeDisplayInfo ??
                        node.Tag as JavaTypeDisplayInfo;
            if (nodeInfo == null)
                nodeInfo = node.Parent.Tag as JavaTypeDisplayInfo;
            var typeName = PathResolver.GetJavaTypeSmaliName(nodeInfo.JavaType.SmaliFullNameId.Value);
            var methodName = SmaliParser.GetFullMethodName(line, typeName.Value);
            var methodRecord = nodeInfo.Internals.FirstOrDefault(m => m.SmaliName == methodName);
            if (methodRecord != null)
            {
                ContextMenuStrip menu = null;
                if (nodeInfo.MethodMenus.ContainsKey(methodRecord.UId.Value))
                {
                    menu = nodeInfo.MethodMenus[methodRecord.UId.Value];
                    if (menu == null)
                        nodeInfo.MethodMenus.Remove(methodRecord.UId.Value);
                }
                if (menu == null)
                {

                    var useCases = nodeInfo.MethodUseCases.Where(uc => uc.DestinationMethodId == methodRecord.UId).ToList();
                    if (useCases.Any())
                    {
                        menu = new ContextMenuStrip();
                        foreach (var javaTypeUsedInType in useCases)
                        {
                            var item = new ToolStripMenuItem
                            {
                                Text = javaTypeUsedInType.SourceMethodSmaliName,
                                Tag = javaTypeUsedInType
                            };
                            item.Click += UseCaseItem_Click;
                            menu.Items.Add(item);
                        }

                    }
                    nodeInfo.MethodMenus.Add(methodRecord.UId.Value, menu);
                }

                trvTreeView.ContextMenuStrip = menu;
                menu?.Show(MousePosition);
            }
        }

        private void UseCaseItem_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            var useCase = item.Tag as JavaTypeUsedInTypeExtended;
            SelectMethodInTreeViewFromUseCase(useCase);
        }

        #endregion


        #region In Form showing Smali logic

        private void btnOpenInForm_Click(object sender, EventArgs e)
        {
            if (tbcSmaliViews.SelectedTab.Equals(tbpMainPage))
                return;

            var page = tbcSmaliViews.SelectedTab;
            var view = page.Tag as SmaliView;
            var id = (int) view.Tag;
            if (_formCache.ContainsKey(id))
            {
                _formCache[id].Show();
                return;
            }
            var form = new Form
            {
                Text = page.Text,
                Tag = view
            };
            tbcSmaliViews.TabPages.Remove(page);
            _pageCache.Remove(id);
            page.Tag = null;
            page.Controls.Remove(view);
            form.Controls.Add(view);
            view.Dock = DockStyle.Fill;
            _formCache.Add(id, form);
            form.FormClosed += SmaliViewFormClosed;
            form.Show();
        }

        private void SmaliViewFormClosed(object sender, FormClosedEventArgs e)
        {
            var form = sender as Form;
            var view = form.Tag as SmaliView;
            form.FormClosed -= SmaliViewFormClosed;
            view.OnInvokeLineClicked -= SmaliView_OnInvokeLineClicked;
            view.OnMethodDefinitionLineClicked -= SmaliView_OnMethodDefinitionLineClicked;
            var id = (int)view.Tag;
            _formCache.Remove(id);
            _smaliViewCache.Remove(id);
        }

        private void tbcSmaliViews_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOpenInForm.Enabled = tbcSmaliViews.SelectedTab != null && !tbcSmaliViews.SelectedTab.Equals(tbpMainPage);
        }

        #endregion

    }
}

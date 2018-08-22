using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using APKInsight.Globals;
using APKInsight.Models;
using APKInsight.Models.DataBase;
using APKInsight.Queries;

namespace APKInsight.Controls
{
    internal partial class BinaryObjectBinaryView : UserControl
    {
        private BinaryObject _binaryObject;
        private List<BinaryObject> _internalBinaryObjects;
        private TreeNode _rooTreeNode;
        private Dictionary<int, TreeNode> _directories;
        private int _currentPath = -1;

        public BinaryObjectBinaryView()
        {
            InitializeComponent();
        }

        public BinaryObject BinaryObject
        {
            get
            {
                return _binaryObject;
            }
            set
            {
                _binaryObject = value;
                LoadRootDirectoryObjects();
            }
        }

        private void LoadRootDirectoryObjects()
        {
            _rooTreeNode = new TreeNode("/");
            trvTreeView.Nodes.Clear();
            trvTreeView.Nodes.Add(_rooTreeNode);
            if (_binaryObject != null)
            {
                var query = new QueryBinaryObject();
                _internalBinaryObjects = query.SelectBinaryObject(new BinaryObject
                {
                    ParentApkId = _binaryObject.UId
                });

                _directories = new Dictionary<int, TreeNode>();

                foreach (var internalBinaryObject in _internalBinaryObjects)
                {
                    //Get Parent Dir
                    TreeNode parentDir = null;
                    TreeNode currentDirectory = null;

                    var pathId = internalBinaryObject.PathId.Value;
                    do
                    {
                        if (pathId == 0)
                        {
                            if (parentDir == null)
                                parentDir = _rooTreeNode;
                        }
                        else if (_directories.ContainsKey(pathId))
                        {
                            if (parentDir == null)
                                parentDir = _directories[pathId];
                            if (currentDirectory != null)
                            {
                                _directories[pathId].Nodes.Add(currentDirectory);
                            }
                            currentDirectory = _directories[pathId];
                            // Since we found that dir in cache, we can safely exit
                            pathId = 0;
                        }
                        else
                        {
                            var bioPath = PathResolver.GetPath(pathId);
                            TreeNode newDirectory = new TreeNode { Text = bioPath.Name, Tag = bioPath };
                            _directories.Add(bioPath.UId.Value, newDirectory);
                            if (parentDir == null)
                                parentDir = newDirectory;
                            if (currentDirectory != null)
                                newDirectory.Nodes.Add(currentDirectory);
                            currentDirectory = newDirectory;
                            pathId = bioPath.ParentPathId.Value;
                            if (pathId == 0)
                                _rooTreeNode.Nodes.Add(currentDirectory);
                        }

                    } while (pathId != 0);
                }
                _rooTreeNode.Expand();

            }
        }

        private void trvTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var node = e.Node;
            var pathInfo = node.Tag as BinaryObjectPath;
            int pathId = pathInfo?.UId ?? 0;
            if (_currentPath == pathId)
                return;
            _currentPath = pathId;
            var bios = _internalBinaryObjects.Where(bio => bio.PathId == pathId).ToList();
            lstDirectoryInternals.Items.Clear();
            foreach (var bio in bios)
            {
                lstDirectoryInternals.Items.Add(new ListViewItem
                {
                    Text = bio.FileName,
                    Tag = bio
                });
            }
        }
    }
}

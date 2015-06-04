using System;
using System.Collections.Generic;
using System.Text;

namespace Kelner.Algorithm
{
    public class TreeNode
    {
        
        public TreeNodeCollection _children;
        private TreeAttribute _attribute;

        public TreeNode(TreeAttribute attribute)
        {
            if (attribute != null && attribute.PossibleValues != null)
            {
                _children = new TreeNodeCollection();
                for (int i = 0; i < attribute.PossibleValues.Count; i++)
                    _children.Add(null);
            }
            else
            {
                _children = new TreeNodeCollection();
                _children.Add(null);
            }
            _attribute = attribute;
        }

        public void AddTreeNode(TreeNode treeNode, string ValueName)
        {
            int index = _attribute.indexValue(ValueName);
            _children[index] = treeNode;
        }

        public int ChildrenCount
        {
            get
            {
                return _children.Count;
            }
        }

        public TreeNode GetChildAt(int index)
        {
            return _children[index];
        }

        public TreeAttribute Attribute
        {
            get
            {
                return _attribute;
            }
        }

        public TreeNode GetChildByBranchName(string branchName)
        {
            int index = _attribute.indexValue(branchName);
            return _children[index];
        }
    }
}

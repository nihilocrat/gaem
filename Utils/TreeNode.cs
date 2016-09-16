using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// a tree data structure that's generally useful for all sorts of things
// bidirectional: each node knows its parent as well as its children
//
// To pass around the whole tree, pass around the root TreeNode
// to know that you have the whole tree and weren't given a branch or leaf, just check IsRoot
public class TreeNode
{
    // whatever data is contained in this node; put whatever you want in here and unpack it
    public object data;

    // children isn't intended to be modified directly, use "myChild.parent = myParentNode;" instead
    protected List<TreeNode> _children = new List<TreeNode>();
    public List<TreeNode> children
    {
        get
        {
            return _children;
        }
        protected set
        {
            _children = value;
        }
    }

    // TODO : in order for this to be "properly" serializable, make _parent non-nullable
    //    see: http://blogs.unity3d.com/2014/06/24/serialization-in-unity/
    // use the "parent" property like you do with Unity transforms; "myChild.parent = myParentNode;"
    
    protected TreeNode _parent = null;
    public TreeNode parent
    {
        get
        {
            return _parent;
        }
        set
        {
            var oldParent = _parent;
            
            if(oldParent != null)
            {
                oldParent.children.Remove(this);
            }
            
            _parent = value;
            
            if(_parent != null)
            {
                _parent.children.Add(this);
            }
        }
    }
    
    /*
    protected List<TreeNode> _parents = new List<TreeNode>();
    
    public List<TreeNode> GetParents()
    {
        return _parents;
    }

    public void AddParent(TreeNode newParent)
    {
        if (newParent != null && !_parents.Contains(newParent))
        {
            _parents.Add(newParent);
            if (!newParent.children.Contains(this))
            {
                newParent.children.Add(this);
            }
        }
    }
    */
    public void Disconnect(TreeNode oldParent = null)
    {
        if (oldParent == null)
        {
            parent.children.Remove(this);
            parent = null;
            /*
            for (int i = 0; i < _parents.Count; i++)
            {
                _parents[i].children.Remove(this);
            }
            _parents.Clear();
            */
        }
        else
        {
            oldParent.children.Remove(this);
            //_parents.Remove(oldParent);
            parent = null;
        }
    }

    public bool isRoot
    {
        get
        {
            return parent == null;
        } //_parents.Count == 0; }
    }

    public bool isLeaf
    {
        get { return children == null || children.Count == 0; }
    }

    public TreeNode() { }

    public TreeNode(TreeNode parent)
    {
        //AddParent(parent);
        this.parent = parent;
    }

    // quickly convert to a flat list, useful if you don't care about order
    public List<TreeNode> Flatten(List<TreeNode> nodes = null)
    {
        if (nodes == null)
        {
            nodes = new List<TreeNode>();
        }

        // always ignore the root node
        // some abilities could have multiple start points, so the root will always be "empty"
        if (!this.isRoot && !nodes.Contains(this))
        {
            nodes.Add(this);
        }

        for (int i = 0; i < children.Count; i++)
        {
            var c = children[i];

            if (!nodes.Contains(c))
            {
                c.Flatten(nodes);
            }
        }

        return nodes;
    }

    public List<TreeNode> GetLeaves()
    {
        var nodes = new List<TreeNode>();

        if (this.isLeaf)
        {
            nodes.Add(this);
        }
        else
        {
            foreach (var c in children)
            {
                nodes.AddRange(c.GetLeaves());
            }
        }

        return nodes;
    }
}

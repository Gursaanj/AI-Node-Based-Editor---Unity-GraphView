using System.Collections.Generic;
using UnityEngine;

namespace Gbt
{
    public abstract class CompositeNode : Node
    {
       [SerializeField] protected List<Node> _children = new List<Node>();

       public List<Node> Children
       {
           get => _children;
           set => _children = value;
       }

       public override Node Clone()
       {
           CompositeNode node = Instantiate(this);
           node.Children = _children.ConvertAll(child => child.Clone());
           return node;
       }
       
    }
}

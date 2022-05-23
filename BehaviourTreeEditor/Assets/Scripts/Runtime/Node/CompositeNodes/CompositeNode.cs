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
    }
}

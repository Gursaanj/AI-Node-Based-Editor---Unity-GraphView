
namespace Gbt
{
    /// <summary>
    /// Executes the Child Nodes within the given sequence.
    /// If one of the children reaches the failure state, then this node will fail and exit
    /// If all the children reaches the success state, then this node will succeed and exit
    /// </summary>
    public class SequencerNode : CompositeNode
    {
        private int _currentChildIndex;
        
        protected override void OnStart()
        {
            _currentChildIndex = 0;
        }

        protected override void OnStop()
        {
            
        }

        protected override State OnUpdate()
        {
            Node childNode = _children[_currentChildIndex];

            switch (childNode.Update())
            {
                case State.Running:
                    return State.Running;
                case State.Failure:
                    return State.Failure;
                case State.Success:
                    _currentChildIndex++;
                    break;
            }

            return _currentChildIndex == _children.Count ? State.Success : State.Running;
        }
    }
}

namespace Gbt
{
    /// <summary>
    /// Inverts ChildNode's State Result
    /// </summary>
    public class InverterNode : DecoratorNode
    {
        public override string InspectorName { get; protected set; } = "Inverter Node";
        
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            State childState = Child.Update();

            switch (childState)
            {
                case State.Running:
                    return State.Running;
                case State.Failure:
                    return State.Success;
                case State.Success:
                    return State.Failure;
                default:
                    return State.Running;
            }
        }
    }
}

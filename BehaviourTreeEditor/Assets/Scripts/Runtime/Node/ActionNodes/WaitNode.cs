using UnityEngine;

namespace Gbt
{
    /// <summary>
    /// Adds delay (in seconds) for a wait action
    /// </summary>
    public class WaitNode : ActionNode
    {
        [Tooltip("The length of delay in seconds")]
        public float duration = 1.0f;
        
        private float _startTime;
        
        public override string InspectorName { get; protected set; } = "Wait Node";

        public override void InjectData(object data)
        {
            if (data is float givenDuration)
            {
                if (givenDuration < 0.0f)
                {
                    Debug.LogError("Unable to set duration to negative amount of time");
                }
                else
                {
                    duration = givenDuration;
                }
            }
        }

        protected override void OnStart()
        {
            if (duration < 0.0f)
            {
                _hasValidConditions = false;
                Debug.LogWarning("WaitNode Duration cannot be less than 0.0 seconds, returning with Failure");
                return;
            }
            
            _startTime = Time.time;
        }

        protected override void OnStop()
        {
            
        }

        protected override State OnUpdate()
        {
            if (Time.time - _startTime > duration)
            {
                return State.Success;
            }

            return State.Running;
        }
    }
}

using UnityEngine;

namespace Gbt
{
    /// <summary>
    /// Debugs given message at given LogType
    /// </summary>
    public class DebugNode : ActionNode
    {
        public string message;
        public LogType type= LogType.Log;
        
        protected override void OnStart()
        {
            LogMessage("OnStart");
        }

        protected override void OnStop()
        {
            LogMessage("OnStop");
        }

        protected override State OnUpdate()
        {
            LogMessage("OnUpdate");
            return State.Success;
        }

        private void LogMessage(string prefix)
        {
            switch (type)
            {
                case LogType.Log:
                    Debug.Log($"{prefix}: {message}");
                    break;
                case LogType.Exception:
                case LogType.Warning:
                    Debug.LogWarning($"{prefix}: {message}");
                    break;
                case LogType.Assert:
                    Debug.LogAssertion($"{prefix}: {message}");
                    break;
                case LogType.Error:
                    Debug.LogError($"{prefix}: {message}");
                    break;
            }
        }
    }
}

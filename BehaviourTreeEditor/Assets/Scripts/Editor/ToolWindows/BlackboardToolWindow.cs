using System;
using UnityEditor.Experimental.GraphView;

namespace Gbt
{
    public class BlackboardToolWindow : GraphViewBlackboardWindow
    {
        private Action _onWindowClose;

        public Action OnWindowClose
        {
            set => _onWindowClose = value;
        }

        private void OnDestroy()
        {
            _onWindowClose?.Invoke();
        }
    }
}

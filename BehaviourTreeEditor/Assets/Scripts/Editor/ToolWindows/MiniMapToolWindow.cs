using System;
using UnityEditor.Experimental.GraphView;

namespace Gbt
{
    public class MiniMapToolWindow : GraphViewMinimapWindow
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

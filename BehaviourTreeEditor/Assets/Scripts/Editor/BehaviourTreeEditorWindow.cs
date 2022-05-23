using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gbt
{
    public class BehaviourTreeEditorWindow : EditorWindow
    {
        private BehaviourTreeView _treeView;
        private InspectorView _inspectorView;
        
        
        [MenuItem("Gbt/BehaviourTreeEditor/Editor")]
        public static void OpenWindow()
        {
            BehaviourTreeEditorWindow wnd = GetWindow<BehaviourTreeEditorWindow>();
            wnd.titleContent = new GUIContent("BehaviourTreeEditorWindow");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/BehaviourTreeEditorWindow.uxml");
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/BehaviourTreeEditorWindow.uss");
            root.styleSheets.Add(styleSheet);

            _treeView = root.Q<BehaviourTreeView>();
            _inspectorView = root.Q<InspectorView>();
            
            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            BehaviourTree tree = Selection.activeObject as BehaviourTree;

            if (tree != null)
            {
                _treeView.PopulateView(tree);
            }
        }
    }
}
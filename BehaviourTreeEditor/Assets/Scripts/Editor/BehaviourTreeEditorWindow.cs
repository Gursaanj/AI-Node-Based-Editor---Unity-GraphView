using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gbt
{
    public class BehaviourTreeEditorWindow : GraphViewEditorWindow
    {
        private BehaviourTreeView _treeView;
        private InspectorView _inspectorView;
        private Label _treeViewTitle;

        private GraphViewBlackboardWindow _blackboardWindow;

        private static string _treeName = string.Empty;
        
        
        [MenuItem("Gbt/BehaviourTreeEditor/Editor")]
        public static void OpenWindow()
        {
            BehaviourTreeEditorWindow wnd = GetWindow<BehaviourTreeEditorWindow>();
            wnd.titleContent = new GUIContent("BehaviourTreeEditorWindow");
            wnd.Show();
        }
        
        [OnOpenAsset(OnOpenAssetAttributeMode.Execute)]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (!(Selection.activeObject is BehaviourTree))
            {
                return false;
            }

            _treeName = Selection.activeObject.name;
            OpenWindow();
            return true;
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

            //Set Tree's title
            _treeViewTitle = rootVisualElement.Q<Label>("Node-View-Label");
            SetTreeName(_treeName);
            
            _treeView = root.Q<BehaviourTreeView>();
            _inspectorView = root.Q<InspectorView>();
            _treeView.OnNodeSelected = OnNodeSelectionChanged;

            _blackboardWindow = CreateInstance<GraphViewBlackboardWindow>();
            _blackboardWindow.SelectGraphViewFromWindow(this, _treeView);
            _blackboardWindow.Show();

            OnSelectionChange();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
        
        private void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        private void OnSelectionChange()
        {
            BehaviourTree tree = Selection.activeObject as BehaviourTree;

            if (tree == null)
            {
                if (Selection.activeGameObject != null)
                {
                    BehaviourTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();

                    if (runner != null)
                    {
                        tree = runner.Tree;
                        SetTreeName(tree.name);
                    }
                }
            }
            else
            {
                SetTreeName(tree.name);
            }

            if (tree != null && _treeView != null)
            {
                if (Application.isPlaying)
                {
                    _treeView.PopulateView(tree);
                }
                else
                {
                    //Ensure that the asset is ready to be inspected before populating editor
                    if (AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
                    {
                        _treeView.PopulateView(tree);
                    }
                }
            }
        }

        private void OnNodeSelectionChanged(NodeView node)
        {
            _inspectorView.UpdateSelection(node);
        }

        private void OnInspectorUpdate()
        {
            if (_treeView != null)
            {
                _treeView.UpdateNodeStates();
            }
        }

        private void SetTreeName(string treeName)
        {
            _treeViewTitle.text = treeName;
        }
    }
    
    //Todo: Currently, my selection.activeGameObject needs to be on the TreeRunner when in edit mode, going into play mode, for the cloning to actually take place! Must change so cloning always take place
}
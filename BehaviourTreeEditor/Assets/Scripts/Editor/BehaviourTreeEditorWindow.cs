using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gbt
{
    public class BehaviourTreeEditorWindow : GraphViewEditorWindow
    {
        private BehaviourTreeView _treeView;
        private InspectorView _inspectorView;
        private Label _treeViewTitle;
        private ToolbarToggle _blackBoardToggle;
        private ToolbarToggle _miniMapToggle;

        private BlackboardToolWindow _blackboardWindow;
        private MiniMapToolWindow _miniMapWindow;

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

            _blackBoardToggle = rootVisualElement.Q<ToolbarToggle>("Blackboard-Toggle");
            _blackBoardToggle.RegisterValueChangedCallback(OnBlackboardTogglePressed);

            _miniMapToggle = rootVisualElement.Q<ToolbarToggle>("MiniMap-Toggle");
            _miniMapToggle.RegisterValueChangedCallback(OnMiniMapTogglePressed);
            
            OnSelectionChange();
        }

        private void OnBlackboardTogglePressed(ChangeEvent<bool> toggleChangeEvent)
        {
            if (toggleChangeEvent.newValue)
            {
                _blackboardWindow = CreateInstance<BlackboardToolWindow>();
                _blackboardWindow.SelectGraphViewFromWindow(this, _treeView);
                _blackboardWindow.OnWindowClose = OnGraphViewToolWindowIndependentlyClosed<BlackboardToolWindow>;
                _blackboardWindow.Show();
            }
            else
            {
                _blackboardWindow.Close();
            }
        }

        private void OnMiniMapTogglePressed(ChangeEvent<bool> toggleChangeEvent)
        {
            if (toggleChangeEvent.newValue)
            {
                _miniMapWindow = CreateInstance<MiniMapToolWindow>();
                _miniMapWindow.SelectGraphViewFromWindow(this, _treeView);
                _miniMapWindow.OnWindowClose = OnGraphViewToolWindowIndependentlyClosed<MiniMapToolWindow>;
                _miniMapWindow.Show();
            }
            else
            {
                if (_miniMapWindow != null)
                {
                    _miniMapWindow.Close();
                }
            }
        }

        private void OnGraphViewToolWindowIndependentlyClosed<T>() where T : GraphViewToolWindow
        {
            ToolbarToggle toggle = new ToolbarToggle();
            Type toolWindowType = typeof(T);

            if (toolWindowType == typeof(BlackboardToolWindow))
            {
                toggle = _blackBoardToggle;
            }
            else if (toolWindowType == typeof(MiniMapToolWindow))
            {
                toggle = _miniMapToggle;
            }

            if (toggle.value)
            {
                toggle.value = false;
            }
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
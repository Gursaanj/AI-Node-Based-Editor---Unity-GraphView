using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gbt
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits>
        {
            
        }

        private Editor _editor;
        
        public InspectorView()
        {
            
        }

        public void UpdateSelection(NodeView nodeView)
        {
            Clear();
            UnityEngine.Object.DestroyImmediate(_editor);

            _editor = Editor.CreateEditor(nodeView.node);
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (_editor.target != null)
                {
                    _editor.OnInspectorGUI();
                }
            });
            Add(container);
        }

        public void UpdateSelection(StickyNote note)
        {
            Clear();
            UnityEngine.Object.DestroyImmediate(_editor);
            
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (note == null)
                {
                    return;
                }

                note.title = EditorGUILayout.TextField("Title", note.title);
                
                GUILayout.Label("Contents");
                note.contents = GUILayout.TextArea(note.contents, EditorStyles.textArea,
                    GUILayout.Height(EditorGUIUtility.singleLineHeight * 3));
                note.theme = (StickyNoteTheme) EditorGUILayout.EnumPopup("Theme", note.theme);
            });
            Add(container);
        }
    }
}

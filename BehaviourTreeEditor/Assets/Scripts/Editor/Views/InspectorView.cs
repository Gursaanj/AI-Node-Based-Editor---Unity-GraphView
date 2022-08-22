using Unity.VisualScripting;
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

        public void UpdateSelection(GraphElement element)
        {
            Clear();
            UnityEngine.Object.DestroyImmediate(_editor);

            IMGUIContainer container = new IMGUIContainer();
            switch (element)
            {
                case NodeView nodeView :
                    container = CreateContainer(nodeView);
                    break;
                case StickyNote stickyNote:
                    container = CreateContainer(stickyNote);
                    break;
            }

            Add(container);
        }

        private IMGUIContainer CreateContainer(NodeView nodeView)
        {
            _editor = Editor.CreateEditor(nodeView.node);
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (_editor.target != null)
                {
                    _editor.OnInspectorGUI();
                }
            });

            return container;
        }

        private IMGUIContainer CreateContainer(StickyNote note)
        {
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
                note.fontSize = (StickyNoteFontSize) EditorGUILayout.EnumPopup("Font Size", note.fontSize);
            });
            return container;
        }
    }
}

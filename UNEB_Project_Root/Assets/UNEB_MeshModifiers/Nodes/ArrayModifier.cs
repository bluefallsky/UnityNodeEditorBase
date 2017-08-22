using UnityEngine;
using UnityEditor;

namespace UNEB.MeshModifiers {
    public class ArrayModifier : Node {
        public override string name { get { return "Array Modifier"; } }

        private int _count;

        public override void Init() {
            base.Init();
            AddInput("Input");
            AddOutput("Ouput");

            FitKnobs();

            //Fit the int field
            bodyRect.height += 20f;
        }

        public override void OnBodyGUI() {
            _count = EditorGUILayout.IntField("Count", _count);
        }

        public override void OnNewInputConnection(NodeInput addedInput) {
            Debug.Log("New input: " + addedInput.name);
        }

        public override void OnInputConnectionRemoved(NodeInput removedInput) {
            Debug.Log("New input: " + removedInput.name);
        }
    }
}

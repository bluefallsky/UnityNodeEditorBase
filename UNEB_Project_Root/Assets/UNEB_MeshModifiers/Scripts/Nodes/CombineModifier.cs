using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace UNEB.MeshModifiers {
    public class CombineModifier : MeshModifierNode {
        public override void Init() {
            base.Init();

            name = "CombineModifier";
            AddInput("Input");
            AddOutput("Ouput");

            FitKnobs();

            //Fit the int field
        }

        public override void OnNewInputConnection(NodeInput addedInput) {
            //_meshOutput = Modify(addedInput.GetValue<Mesh[]>());
        }

        public override void OnInputConnectionRemoved(NodeInput removedInput) {
            //_meshOutput = null;
        }

        public override List<Model> Modify(List<Model> input) {
            return new List<Model>() { Model.CombineModels(input) };
        }
    }
}

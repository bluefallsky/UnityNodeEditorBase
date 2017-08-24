using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNEB.MeshModifiers {
    public class OutputMesh : MeshModifierNode {


        public override void Init() {
            base.Init();
            name = "Output";
            AddInput("Input");
        }

        public override List<Model> Modify(List<Model> input) {
            for (int i = 0; i < input.Count; i++) {
                input[i].mesh.RecalculateBounds();
            }
            return input;
        }
    }
}
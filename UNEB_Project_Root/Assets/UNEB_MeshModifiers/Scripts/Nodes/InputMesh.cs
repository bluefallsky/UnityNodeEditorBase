using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNEB.MeshModifiers {
    public class InputMesh : Node {


        public override void Init() {
            base.Init();
            name = "Input";
            AddOutput("Output");
        }
    }
}
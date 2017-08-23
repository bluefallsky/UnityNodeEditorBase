using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNEB.MeshModifiers {
    public class InputMesh : Node {

        public override string name { get { return "Input"; } }

        public override void Init() {
            base.Init();
            AddOutput("Output");
        }
    }
}
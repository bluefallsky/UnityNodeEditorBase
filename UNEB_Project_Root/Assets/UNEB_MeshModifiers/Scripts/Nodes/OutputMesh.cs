﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNEB.MeshModifiers {
    public class OutputMesh : MeshModifierNode {

        public override string name { get { return "Output"; } }

        public override void Init() {
            base.Init();
            AddInput("Input");
        }

        public override List<Model> Modify(List<Model> input) {
            return input;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNEB.MeshModifiers {
    public abstract class MeshModifierNode : Node {

        public abstract List<Model> Modify(List<Model> input);
    }
}

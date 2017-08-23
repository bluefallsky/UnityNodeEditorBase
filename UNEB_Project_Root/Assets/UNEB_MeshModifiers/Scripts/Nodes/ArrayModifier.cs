using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace UNEB.MeshModifiers {
    public class ArrayModifier : MeshModifierNode {
        public override string name { get { return "Array Modifier"; } }

        public int count;
        public Vector3 posOffset;
        public Vector3 rotOffset;
        public Mesh[] meshOutput;

        public override void Init() {
            base.Init();
            AddInput("Input");
            AddOutput("Ouput");

            FitKnobs();

            //Fit the int field
            bodyRect.height += 22*4;
        }

        public override void OnBodyGUI() {
            count = EditorGUILayout.IntField("Count", count);
            posOffset = EditorGUILayout.Vector3Field("Offset", posOffset);
            rotOffset = EditorGUILayout.Vector3Field("Rotation", rotOffset);
        }

        public override void OnNewInputConnection(NodeInput addedInput) {
            //_meshOutput = Modify(addedInput.GetValue<Mesh[]>());
        }

        public override void OnInputConnectionRemoved(NodeInput removedInput) {
            //_meshOutput = null;
        }

        public override List<Model> Modify(List<Model> input) {
            List<Model> output = new List<Model>();

            /*if (spline) {
                if (fit == Fit.Spline && updateOnSpline) spline.OnRefresh.AddListenerOnce(OnSplineRefreshEvent);
                else spline.OnRefresh.RemoveListener(OnSplineRefreshEvent);
            }*/

            //int count = GetCount(inputMesh);

            for (int i = 0; i < input.Count; i++) {

                CombineInstance[] submeshCombines = new CombineInstance[count];
                for (int k = 0; k < count; k++) {
                    Vector3 pos = posOffset * k;
                    Vector3 rot = rotOffset * k;
                    Vector3 scale = Vector3.one;
                    /*if (fit != Fit.Count && scaleToFit) {
                        float segmentWidth = GetSegmentWidth(inputMesh); //eg 4
                        float lengthToFit = GetLength(); //eg 11.12041
                        float fitScale = lengthToFit / (segmentWidth * count); //eg 1.390052
                        Vector3 fitAxis = posOffset / segmentWidth; //eg (0,0,1)
                        Vector3 fitScaleOffset = fitAxis * fitScale; //eg (0,0,1.390052)

                        scale.x *= Mathf.Lerp(1f, fitScaleOffset.x, fitAxis.x);
                        scale.y *= Mathf.Lerp(1f, fitScaleOffset.y, fitAxis.y);
                        scale.z *= Mathf.Lerp(1f, fitScaleOffset.z, fitAxis.z);
                        pos.x *= Mathf.Lerp(1f, fitScaleOffset.x, fitAxis.x);
                        pos.y *= Mathf.Lerp(1f, fitScaleOffset.y, fitAxis.y);
                        pos.z *= Mathf.Lerp(1f, fitScaleOffset.z, fitAxis.z);

                    }*/
                    //scale += (scaleOffset * k);
                    submeshCombines[k] = new CombineInstance();
                    submeshCombines[k].mesh = input[i].mesh;
                    submeshCombines[k].transform = Matrix4x4.TRS(pos, Quaternion.Euler(rot), scale);
                }
                Mesh arrayedSubmesh = new Mesh();
                arrayedSubmesh.CombineMeshes(submeshCombines, true);
                arrayedSubmesh.RecalculateBounds();
                output.Add(new Model(arrayedSubmesh,input[i].materials));
            }
            return output;
        }
    }
}

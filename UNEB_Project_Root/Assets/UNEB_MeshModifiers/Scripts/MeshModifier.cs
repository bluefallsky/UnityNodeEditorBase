using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UNEB;

namespace UNEB.MeshModifiers {
    public class MeshModifier : MonoBehaviour {

        public NodeGraph graph;
        public Transform template;
        public bool singleMesh;

        [ContextMenu("Generate")]
        private void Generate() {
            ClearMesh();

            List<InputMesh> inputNodes = graph.nodes.FindAll(x => x.GetType() == typeof(InputMesh)).ConvertAll(x => x as InputMesh);
            List<Model> models = ModelsFromGameObjects(template, singleMesh);

            for (int i = 0; i < inputNodes.Count; i++) {
                InputMesh inputNode = inputNodes[i];
                MeshModifierNode currentNode = inputNode.GetOutput(0).GetInput(0).ParentNode as MeshModifierNode;
                while(currentNode != null) {
                    models = currentNode.Modify(models);
                    if (currentNode.OutputCount == 0) break;
                    currentNode = currentNode.GetOutput(0).GetInput(0).ParentNode as MeshModifierNode;
                }
            }
            if (models.Count == 1) ShowModel(gameObject, models[0]);
            else ShowModels(models);
        }

        private void ShowModel(GameObject target, Model model) {
            MeshFilter filter = target.AddComponent<MeshFilter>();
            filter.mesh = model.mesh;
            MeshRenderer renderer = target.AddComponent<MeshRenderer>();
            renderer.materials = model.materials;
        }
        private void ShowModels(List<Model> models) {
            for (int i = 0; i < models.Count; i++) {
                GameObject go = new GameObject("mesh output "+i);
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                ShowModel(go, models[i]);
            }
        }


        private List<Model> ModelsFromGameObjects(Transform root, bool singleMesh) {
            List<Model> output = new List<Model>();

            MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++) {
                MeshRenderer renderer = renderers[i];
                MeshFilter filter = renderer.GetComponent<MeshFilter>();
                if (filter == null) continue;
                Mesh mesh = BakeMeshTransform(root, filter);
                Material[] materials = renderer.sharedMaterials;
                output.Add(new Model(mesh,materials));
            }
            if (singleMesh) {
                output = new List<Model>() { Model.CombineModels(output) };
            }
            return output;
        }

        private Mesh BakeMeshTransform(Transform root, MeshFilter filter) {
            Mesh mesh = filter.sharedMesh.Copy();
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector4> tangent = new List<Vector4>();
            mesh.GetVertices(verts);
            mesh.GetNormals(norms);
            mesh.GetTangents(tangent);

            for (int i = 0; i < verts.Count; i++) {
                //Vert position
                Vector3 world = filter.transform.TransformPoint(verts[i]); //Transform from filter local to world
                Vector3 local = root.InverseTransformPoint(world); //Transform from world to root local 
                verts[i] = local;

                //Vert normal
                world = filter.transform.TransformDirection(norms[i]);
                local = root.InverseTransformDirection(world);
                norms[i] = local;

                //Vert tangent
                float w = tangent[i].w;
                world = filter.transform.TransformDirection(tangent[i]);
                local = root.InverseTransformDirection(world);
                tangent[i] = new Vector4(local.x,local.y,local.z,w);
            }
            mesh.SetVertices(verts);
            mesh.SetNormals(norms);
            mesh.SetTangents(tangent);
            return mesh;
        }

        private void ClearMesh() {
            int children = transform.childCount;
            for (int i = children - 1; i >= 0; i--) {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            DestroyImmediate(GetComponent<MeshRenderer>());
            DestroyImmediate(GetComponent<MeshFilter>());
        }
    }

    public static class ExtensionMethods {
        /// <summary> Returns a copy of the mesh </summary>
        public static Mesh Copy(this Mesh mesh) {
            return new Mesh() {
                vertices = mesh.vertices,
                triangles = mesh.triangles,
                uv = mesh.uv,
                uv2 = mesh.uv2,
                normals = mesh.normals,
                tangents = mesh.tangents,
                colors = mesh.colors
            };
        }

        /// <summary> Returns a copy of meshes </summary>
        public static Mesh[] Copy(this Mesh[] mesh) {
            Mesh[] meshes = new Mesh[mesh.Length];
            for (int i = 0; i < meshes.Length; i++) {
                meshes[i] = new Mesh() {
                    vertices = mesh[i].vertices,
                    triangles = mesh[i].triangles,
                    uv = mesh[i].uv,
                    uv2 = mesh[i].uv2,
                    normals = mesh[i].normals,
                    tangents = mesh[i].tangents,
                    colors = mesh[i].colors
                };
            }
            return meshes;
        }
    }
}

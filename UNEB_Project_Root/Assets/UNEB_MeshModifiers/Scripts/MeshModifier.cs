using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UNEB;

namespace UNEB.MeshModifiers {
    public class MeshModifier : MonoBehaviour {

        public MeshModifierGraph graph;
        public Transform template;
        public bool singleMesh;

        [ContextMenu("Generate")]
        private void Generate() {
            ClearMesh();

            List<InputMesh> inputNodes = graph.nodes.FindAll(x => x.GetType() == typeof(InputMesh)).ConvertAll(x => x as InputMesh);
            List<Mesh> meshes = MeshFromGameObject(template, singleMesh);

            for (int i = 0; i < inputNodes.Count; i++) {
                InputMesh inputNode = inputNodes[i];
                MeshModifierNode currentNode = inputNode.GetOutput(0).GetInput(0).ParentNode as MeshModifierNode;
                while(currentNode != null) {
                    meshes = currentNode.Modify(meshes);
                    if (currentNode.OutputCount == 0) break;
                    currentNode = currentNode.GetOutput(0).GetInput(0).ParentNode as MeshModifierNode;
                }
            }
            if (meshes.Count == 1) {
                MeshFilter filter = gameObject.AddComponent<MeshFilter>();
                filter.mesh = meshes[0];
                MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
            }

        }

        private List<Mesh> MeshFromGameObject(Transform root, bool singleMesh) {
            List<Mesh> output = new List<Mesh>();

            MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++) {
                MeshRenderer renderer = renderers[i];
                MeshFilter filter = renderer.GetComponent<MeshFilter>();
                if (filter == null) continue;
                output.Add(BakeMeshTransform(root, filter));
            }
            if (singleMesh) {
                output = new List<Mesh>() { CombineMeshes(output) };
            }
            return output;
        }

        private Mesh BakeMeshTransform(Transform root, MeshFilter filter) {
            Mesh mesh = filter.sharedMesh.Copy();
            List<Vector3> verts = new List<Vector3>();
            mesh.GetVertices(verts);
            for (int i = 0; i < verts.Count; i++) {
                //Transform from filter local to world
                Vector3 world = filter.transform.TransformPoint(mesh.vertices[i]);
                //Transform from world to root local
                Vector3 local = root.InverseTransformPoint(world);
                verts[i] = local;
            }
            mesh.SetVertices(verts);
            return mesh;
        }

        private Mesh CombineMeshes(List<Mesh> meshes) {
            CombineInstance[] combines = meshes.ConvertAll(x => new CombineInstance() {
                mesh = x,
                transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one)
            }).ToArray();
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combines);
            return mesh;
        }

        private void ClearMesh() {
            int children = transform.childCount;
            for (int i = children - 1; i > 0; i--) {
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

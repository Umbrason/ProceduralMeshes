using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ProceduralMeshes
{
    public class DynamicMesh
    {
        public string name;
        private List<Vector3> vertices;
        public List<Vector3> Vertices { get { return vertices ??= new List<Vector3>(); } set { vertices = value; } }
        private List<int> indices;
        public List<int> Indices { get { return indices ??= new List<int>(); } set { indices = value; } }
        private List<Vector3> normals;
        public List<Vector3> Normals { get { return normals ??= new List<Vector3>(); } set { normals = value; } }

        private List<Color> colors;
        public List<Color> Colors { get { return colors ??= new List<Color>(); } set { colors = value; } }

        private List<Vector2> uv0;
        public List<Vector2> UV0 { get { return uv0 ??= new List<Vector2>(); } set { uv0 = value; } }

        private List<Vector2> uv1;
        public List<Vector2> UV1 { get { return uv1 ??= new List<Vector2>(); } set { uv1 = value; } }

        private List<Vector2> uv2;
        public List<Vector2> UV2 { get { return uv2 ??= new List<Vector2>(); } set { uv2 = value; } }

        private List<Vector2> uv3;
        public List<Vector2> UV3 { get { return uv3 ??= new List<Vector2>(); } set { uv3 = value; } }

        private List<Vector2> uv4;
        public List<Vector2> UV4 { get { return uv4 ??= uv4 = new List<Vector2>(); } set { uv4 = value; } }

        private List<BoneWeight> boneWeights;
        public List<BoneWeight> BoneWeights { get { return boneWeights ??= new List<BoneWeight>(); } set { boneWeights = value; } }

        private List<Matrix4x4> bindPoses;
        public List<Matrix4x4> BindPoses { get { return bindPoses ??= new List<Matrix4x4>(); ; } set { bindPoses = value; } }

        public MeshTopology topology;


        public DynamicMesh(string name = "", IEnumerable<Vector3> vertices = null, IEnumerable<int> indices = null, IEnumerable<Vector3> normals = null, IEnumerable<Color> colors = null, IEnumerable<Vector2> uv0 = null, IEnumerable<Vector2> uv1 = null, IEnumerable<Vector2> uv2 = null, IEnumerable<Vector2> uv3 = null, IEnumerable<Vector2> uv4 = null, IEnumerable<BoneWeight> boneWeights = null, IEnumerable<Matrix4x4> bindPoses = null, MeshTopology topology = MeshTopology.Triangles)
        {
            this.name = name;
            this.vertices = vertices?.ToList();
            this.indices = indices?.ToList();
            this.normals = normals?.ToList();
            this.colors = colors?.ToList();
            this.uv0 = uv0?.ToList();
            this.uv1 = uv1?.ToList();
            this.uv2 = uv2?.ToList();
            this.uv3 = uv3?.ToList();
            this.uv4 = uv4?.ToList();
            this.boneWeights = boneWeights?.ToList();
            this.bindPoses = bindPoses?.ToList();
            this.topology = topology;
        }
        public Mesh ToMesh()
        {
            Mesh m = new Mesh() { name = name, vertices = Vertices.ToArray(), normals = Normals.ToArray(), colors = Colors.ToArray(), uv = UV0.ToArray(), uv2 = UV1.ToArray(), uv3 = UV2.ToArray(), uv4 = UV3.ToArray(), uv5 = UV4.ToArray(), boneWeights = BoneWeights.ToArray(), bindposes = BindPoses.ToArray() };
            m.SetIndices(Indices.ToArray(), topology, 0);
            return m;
        }

        public void AppendMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale) => AppendMesh(mesh, Matrix4x4.TRS(position, rotation, scale));
        public void AppendMesh(Mesh mesh, Matrix4x4? TRS = null)
        {
            for (int s = 0; s < mesh.subMeshCount; s++)
            {
                UnityEngine.Rendering.SubMeshDescriptor subMeshDesc = mesh.GetSubMesh(s);
                AppendMesh(subMeshDesc.baseVertex, subMeshDesc.vertexCount, subMeshDesc.indexStart, subMeshDesc.indexCount, TRS ??= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one),
                mesh.vertices,
                mesh.GetIndices(s),
                mesh.normals,
                mesh.colors,
                mesh.uv,
                mesh.uv2,
                mesh.uv3,
                mesh.uv4,
                mesh.uv5,
                mesh.boneWeights
                );
                if (subMeshDesc.topology != topology)
                {
                    Debug.LogError($"trying to append mesh with different topology type. expected {topology} but found {subMeshDesc.topology}");
                    continue;
                }
            }
        }
        public void AppendMesh(DynamicMesh mesh, Vector3 position, Quaternion rotation, Vector3 scale) => AppendMesh(mesh, Matrix4x4.TRS(position, rotation, scale));
        public void AppendMesh(DynamicMesh mesh, Matrix4x4? TRS = null)
        {
            if (this.topology != mesh.topology)
            {
                Debug.LogError($"trying to append mesh with different topology type. expected {this.topology} but found {mesh.topology}");
                return;
            }
            AppendMesh(0, mesh.Vertices.Count, 0, mesh.Indices.Count, TRS ??= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one),
                mesh.Vertices,
                mesh.Indices,
                mesh.Normals,
                mesh.Colors,
                mesh.UV0,
                mesh.UV1,
                mesh.UV2,
                mesh.UV3,
                mesh.UV4,
                mesh.BoneWeights
            );
        }

        private void AppendMesh(int baseVertex, int vertexCount, int baseIndex, int indexCount, Matrix4x4 TRS, IList<Vector3> meshVertices, IList<int> meshIndices, IList<Vector3> meshNormals, IList<Color> meshColors, IList<Vector2> meshUV0, IList<Vector2> meshUV1, IList<Vector2> meshUV2, IList<Vector2> meshUV3, IList<Vector2> meshUV4, IList<BoneWeight> meshBoneWeights)
        {
            int indexOffset = vertices.Count;
            for (int i = 0; i < vertexCount; i++)
            {
                var index = i + baseVertex;
                Vertices.Add(meshVertices?.Count > index ? TRS * meshVertices[index] : default);
                Normals.Add(meshNormals?.Count > index ? TRS.rotation * meshNormals[index] : default);
                Colors.Add(meshColors?.Count > index ? meshColors[index] : default);
                UV0.Add(meshUV0?.Count > index ? meshUV0[index] : default);
                UV1.Add(meshUV1?.Count > index ? meshUV1[index] : default);
                UV2.Add(meshUV2?.Count > index ? meshUV2[index] : default);
                UV3.Add(meshUV3?.Count > index ? meshUV3[index] : default);
                UV4.Add(meshUV4?.Count > index ? meshUV4[index] : default);
                BoneWeights.Add(meshBoneWeights?.Count > index ? meshBoneWeights[index] : default);
            }
            for (int i = 0; i < indexCount; i++)
                Indices.Add(meshIndices[i + baseIndex] + indexOffset);
        }
    }
}

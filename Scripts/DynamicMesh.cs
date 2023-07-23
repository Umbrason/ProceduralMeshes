using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ProceduralMeshes
{
    public class DynamicMesh
    {
        public string name;
        public List<Vector3> Vertices { get; private set; } = new();
        public List<int> Indices { get; private set; } = new();
        public List<Vector3> Normals { get; private set; } = new();
        public List<Color> Colors { get; private set; } = new();
        public List<Vector2> UV0 { get; private set; } = new();
        public List<Vector2> UV1 { get; private set; } = new();
        public List<Vector2> UV2 { get; private set; } = new();
        public List<Vector2> UV3 { get; private set; } = new();
        public List<Vector2> UV4 { get; private set; } = new();
        public List<BoneWeight> BoneWeights { get; private set; } = new();
        public List<Matrix4x4> BindPoses { get; private set; } = new();

        public MeshTopology Topology;

        public DynamicMesh(string name = "", IEnumerable<Vector3> vertices = null, IEnumerable<int> indices = null, IEnumerable<Vector3> normals = null, IEnumerable<Color> colors = null, IEnumerable<Vector2> uv0 = null, IEnumerable<Vector2> uv1 = null, IEnumerable<Vector2> uv2 = null, IEnumerable<Vector2> uv3 = null, IEnumerable<Vector2> uv4 = null, IEnumerable<BoneWeight> boneWeights = null, IEnumerable<Matrix4x4> bindPoses = null, MeshTopology topology = MeshTopology.Triangles)
        {
            this.name = name;
            this.Vertices = vertices?.ToList() ?? new();
            this.Indices = indices?.ToList() ?? new();
            this.Normals = normals?.ToList() ?? new();
            this.Colors = colors?.ToList() ?? new();
            this.UV0 = uv0?.ToList() ?? new();
            this.UV1 = uv1?.ToList() ?? new();
            this.UV2 = uv2?.ToList() ?? new();
            this.UV3 = uv3?.ToList() ?? new();
            this.UV4 = uv4?.ToList() ?? new();
            this.BoneWeights = boneWeights?.ToList() ?? new();
            this.BindPoses = bindPoses?.ToList() ?? new();
            this.Topology = topology;
        }
        public Mesh ToMesh()
        {
            Mesh m = new Mesh() { name = name, vertices = Vertices.ToArray(), normals = Normals.ToArray(), colors = Colors.ToArray(), uv = UV0.ToArray(), uv2 = UV1.ToArray(), uv3 = UV2.ToArray(), uv4 = UV3.ToArray(), uv5 = UV4.ToArray(), boneWeights = BoneWeights.ToArray(), bindposes = BindPoses.ToArray() };
            m.SetIndices(Indices.ToArray(), Topology, 0);
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
                if (subMeshDesc.topology != Topology)
                {
                    Debug.LogError($"trying to append mesh with different topology type. expected {Topology} but found {subMeshDesc.topology}");
                    continue;
                }
            }
        }
        public void AppendMesh(DynamicMesh mesh, Vector3 position, Quaternion rotation, Vector3 scale) => AppendMesh(mesh, Matrix4x4.TRS(position, rotation, scale));
        public void AppendMesh(DynamicMesh mesh, Matrix4x4? TRS = null)
        {
            if (this.Topology != mesh.Topology)
            {
                Debug.LogError($"trying to append mesh with different topology type. expected {this.Topology} but found {mesh.Topology}");
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
            int indexOffset = Vertices.Count;
            for (int i = 0; i < vertexCount; i++)
            {
                var index = i + baseVertex;
                Vertices.Add(meshVertices?.Count > index ? TRS * ((Vector4)meshVertices[index] + new Vector4(0, 0, 0, 1)) : default);
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

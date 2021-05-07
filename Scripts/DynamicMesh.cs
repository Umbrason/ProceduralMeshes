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
        public List<Vector3> Vertices { get { if (vertices == null) vertices = new List<Vector3>(); return vertices; } set { vertices = value; } }
        private List<int> indices;
        public List<int> Indices { get { if (indices == null) indices = new List<int>(); return indices; } set { indices = value; } }
        private List<Vector3> normals;
        public List<Vector3> Normals { get { if (normals == null) normals = new List<Vector3>(); return normals; } set { normals = value; } }

        private List<Color> colors;
        public List<Color> Colors { get { if (colors == null) colors = new List<Color>(); return colors; } set { colors = value; } }

        private List<Vector2> uv0;
        public List<Vector2> UV0 { get { if (uv0 == null) uv0 = new List<Vector2>(); return uv0; } set { uv0 = value; } }

        private List<Vector2> uv1;
        public List<Vector2> UV1 { get { if (uv1 == null) uv1 = new List<Vector2>(); return uv1; } set { uv1 = value; } }

        private List<Vector2> uv2;
        public List<Vector2> UV2 { get { if (uv2 == null) uv2 = new List<Vector2>(); return uv2; } set { uv2 = value; } }


        private List<Vector2> uv3;
        public List<Vector2> UV3 { get { if (uv3 == null) uv3 = new List<Vector2>(); return uv3; } set { uv3 = value; } }

        private List<Vector2> uv4;
        public List<Vector2> UV4 { get { if (uv4 == null) uv4 = new List<Vector2>(); return uv4; } set { uv4 = value; } }

        private List<BoneWeight> boneWeights;
        public List<BoneWeight> BoneWeights { get { if (boneWeights == null) boneWeights = new List<BoneWeight>(); return boneWeights; } set { boneWeights = value; } }

        private List<Matrix4x4> bindPoses;
        public List<Matrix4x4> BindPoses { get { if (bindPoses == null) bindPoses = new List<Matrix4x4>(); return bindPoses; } set { bindPoses = value; } }

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
        public void AppendMesh(Mesh mesh, Matrix4x4 TRS)
        {
            for (int s = 0; s < mesh.subMeshCount; s++)
            {
                UnityEngine.Rendering.SubMeshDescriptor subMeshDesc = mesh.GetSubMesh(s);
                if (subMeshDesc.topology != topology)
                    continue;

                int indexOffset = vertices.Count;
                for (int i = 0; i < subMeshDesc.vertexCount; i++)
                {
                    Vertices.Add(TRS * mesh.vertices[i + subMeshDesc.baseVertex]);
                    Normals.Add(TRS.rotation * mesh.normals[i + subMeshDesc.baseVertex]);
                    Colors.Add(mesh.colors[i + subMeshDesc.baseVertex]);
                    uv0.Add(mesh.uv[i]);
                    uv1.Add(mesh.uv2[i]);
                    uv2.Add(mesh.uv3[i]);
                    uv3.Add(mesh.uv4[i]);
                    uv4.Add(mesh.uv5[i]);
                    boneWeights.Add(mesh.boneWeights[i]);
                }
                for (int i = 0; i < subMeshDesc.indexCount; i++)
                    Indices.Add(mesh.triangles[i + subMeshDesc.indexStart] + indexOffset);
            }
        }
    }
}
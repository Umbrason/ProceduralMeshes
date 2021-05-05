using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ProceduralMeshes
{
    public struct DynamicMesh
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

        public MeshTopology topology;


        public DynamicMesh(string name = "", IEnumerable<Vector3> vertices = null, IEnumerable<int> indices = null, IEnumerable<Vector3> normals = null, IEnumerable<Color> colors = null, MeshTopology topology = MeshTopology.Triangles)
        {
            this.name = name;
            this.vertices = vertices?.ToList();
            this.indices = indices?.ToList();
            this.normals = normals?.ToList();
            this.colors = colors?.ToList();
            this.topology = topology;
        }
        public Mesh ToMesh()
        {
            Mesh m = new Mesh() { name = name, vertices = Vertices.ToArray(), normals = Normals.ToArray(), colors = Colors.ToArray() };
            m.SetIndices(Indices.ToArray(), topology, 0);
            return m;
        }

        public void AddMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale) => AddMesh(mesh, Matrix4x4.TRS(position, rotation, scale));
        public void AddMesh(Mesh mesh, Matrix4x4 TRS)
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
                }
                for (int i = 0; i < subMeshDesc.indexCount; i++)
                    Indices.Add(mesh.triangles[i + subMeshDesc.indexStart] + indexOffset);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProceduralMeshes
{
    [RequireComponent(typeof(MeshFilter))]
    [ExecuteAlways]
    public class LineMeshRenderer : MonoBehaviour
    {
        internal Mesh lineMesh;
        public Material material;
        public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode;
        private MeshFilter filter;
        public MeshFilter MeshFilter
        {
            get
            {
                if (filter == null)
                    filter = GetComponent<MeshFilter>();
                return filter;
            }
        }
        public Vector3 offset;

        public void Update()
        {
            if (!lineMesh)
                GenerateLineMesh();
            if (!lineMesh)
                return;
            Graphics.DrawMesh(lineMesh, transform.localToWorldMatrix * Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one), material, gameObject.layer, null, 0, null, shadowCastingMode, true);
        }

        internal virtual void GenerateLineMesh()
        {
            if (!MeshFilter?.sharedMesh)
                return;
            Mesh mesh = MeshFilter.sharedMesh;
            DynamicMesh dm = new DynamicMesh("LineMesh", vertices: mesh.vertices, normals: mesh.normals, topology: MeshTopology.Lines);
            for (int i = 0; i < mesh.triangles.Length / 3; i++)
            {
                dm.Indices.Add(mesh.triangles[i * 3 + 0]);
                dm.Indices.Add(mesh.triangles[i * 3 + 1]);
                dm.Indices.Add(mesh.triangles[i * 3 + 1]);
                dm.Indices.Add(mesh.triangles[i * 3 + 2]);
                dm.Indices.Add(mesh.triangles[i * 3 + 2]);
                dm.Indices.Add(mesh.triangles[i * 3 + 0]);
            }
            lineMesh = dm.ToMesh();
        }
    }
}
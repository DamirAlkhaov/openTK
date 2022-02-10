using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using Assimp;
using Assimp.Configs;

namespace Dengine.importer
{
    public class Loader : IDisposable
    {
        List<int> vaos = new List<int>();
        List<int> vbos = new List<int>();
        

        public Loader()
        {

        }

        public RawModel LoadModel(float[] positions, float[] uvs, float[] normals, int[] indices)
        {
            int vaoID = CreateVAO();
            BindIndices(indices);
            StoreAttributes(0, 3, positions);
            StoreAttributes(1, 3, normals);
            StoreAttributes(2, 2, uvs);
            GL.BindVertexArray(0);
            return new RawModel(vaoID, indices.Length);
        }

        public RawModel LoadModel(float[] positions, float[] uvs, float[] normals)
        {
            int vaoID = CreateVAO();
            StoreAttributes(0, 3, positions);
            StoreAttributes(1, 3, normals);
            StoreAttributes(2, 2, uvs);
            GL.BindVertexArray(0);
            return new RawModel(vaoID, positions.Length / 3);
        }

        public RawModel LoadFromFile(string path)
        {
            AssimpContext importer = new AssimpContext();
            NormalSmoothingAngleConfig config = new NormalSmoothingAngleConfig(66.0f);
            importer.SetConfig(config);

            LogStream logstream = new LogStream(delegate (String msg, String userData) {
            });
            logstream.Attach();

            Scene scene = importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);

            List<float> positions = new List<float>();
            List<float> uvs = new List<float>();
            List<float> normals = new List<float>();
            List<int> indices = new List<int>();


            foreach (Mesh m in scene.Meshes)
            {
                foreach (Vector3D vert in m.Vertices)
                {
                    positions.Add(vert.X);
                    positions.Add(vert.Y);
                    positions.Add(vert.Z);
                }

                foreach (Vector3D uv in m.TextureCoordinateChannels[0])
                {
                    uvs.Add(uv[0]);
                    uvs.Add(-uv[1]);
                }

                foreach (Vector3D norm in m.Normals)
                {
                    normals.Add(norm.X);
                    normals.Add(norm.Y);
                    normals.Add(norm.Z);
                }

                foreach (Face f in m.Faces)
                {
                    if (f.IndexCount != 3)
                    {
                        indices.Add(0);
                        indices.Add(0);
                        indices.Add(0);
                        continue;
                    }

                    indices.Add(f.Indices[0]);
                    indices.Add(f.Indices[1]);
                    indices.Add(f.Indices[2]);
                }
            }

            importer.Dispose();

            return LoadModel(positions.ToArray(), uvs.ToArray(), normals.ToArray(), indices.ToArray());
        }

        /*
        public RawModel LoadOBJ(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            LoadResult result = objLoader.Load(stream);

            List<float> positions = new List<float>();
            List<float> uvs = new List<float>();
            List<float> normals = new List<float>();
            List<int> indices = new List<int>();
            foreach (ObjLoader.Loader.Data.VertexData.Vertex vert in result.Vertices)
            {
                positions.Add(vert.X);
                positions.Add(vert.Y);
                positions.Add(vert.Z);
            }

            foreach (ObjLoader.Loader.Data.VertexData.Texture tex in result.Textures)
            {
                uvs.Add(tex.X);
                uvs.Add(tex.Y);
            }

            foreach (ObjLoader.Loader.Data.VertexData.Normal nrm in result.Normals)
            {
                normals.Add(nrm.X);
                normals.Add(nrm.Y);
                normals.Add(nrm.Z);
            }

            foreach (ObjLoader.Loader.Data.Elements.Group grp in result.Groups)
            {
                foreach (ObjLoader.Loader.Data.Elements.Face face in grp.Faces)
                {
                    for (int i = 0; i < face.Count; i++)
                    {
                        ObjLoader.Loader.Data.Elements.FaceVertex faceVertex = face[i];
                        indices.Add(faceVertex.VertexIndex);
                    }
                }
            }

            return LoadModel(positions.ToArray(), uvs.ToArray(), normals.ToArray(), indices.ToArray());
        }
        */

        int CreateVAO()
        {
            int vao = GL.GenVertexArray();
            vaos.Add(vao);
            GL.BindVertexArray(vao);
            return vao;
        }

        void BindIndices(int[] indices)
        {
            int vboID = GL.GenBuffer();
            vbos.Add(vboID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vboID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);
        }

        void StoreAttributes(int attr, int coords, float[] data)
        {
            int vbo = GL.GenBuffer();
            vbos.Add(vbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(attr);
            GL.VertexAttribPointer(attr, coords, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Dispose()
        {
            foreach (int vao in vaos)
            {
                GL.DeleteVertexArray(vao);
            }

            foreach (int vbo in vbos)
            {
                GL.DeleteBuffer(vbo);
            }
        }
    }
}

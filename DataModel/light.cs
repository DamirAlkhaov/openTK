using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Dengine.importer;

namespace Dengine.DataModel
{
    class light
    {
        //***
        public Vector3 partSize { get; private set; }
        public Vector3 partPosition { get; private set; }
        //*** get and set data ^^^

        public int VertexArrayObject { get; private set; }
        public Shader shader;
        private Loader _loader = new Loader();
        public int vertexCount { get; private set; }

        public light(Vector3 size, Vector3 position)
        {
            partSize = size;

            partPosition = position;

            RawModel _rawmodel = _loader.LoadFromFile("stock/cube.obj");
            
            VertexArrayObject = _rawmodel.VAO;
            vertexCount = _rawmodel.VertexCount;
            

            shader = new Shader("shaders/shader.vert", "shaders/light.frag");
            shader.Use();

            GL.BindVertexArray(VertexArrayObject);
        }

        public void render(camera _camera)
        {
            shader.Use();
            GL.BindVertexArray(VertexArrayObject);
            Matrix4 model = Matrix4.Identity;
            model *= Matrix4.CreateScale(partSize.X, partSize.Y, partSize.Z);
            //model *= Matrix4.CreateTranslation(_camera.Position.X, _camera.Position.Y, _camera.Position.Z);
            model *= Matrix4.CreateTranslation(partPosition.X, partPosition.Y, partPosition.Z);
            shader.SetMatrix4("model", model);
            shader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            shader.SetMatrix4("view", _camera.GetViewMatrix());

            GL.DrawElements(PrimitiveType.Triangles, vertexCount, DrawElementsType.UnsignedInt, 0);
        }
    }
}
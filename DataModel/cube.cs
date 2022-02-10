using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Dengine.importer;

namespace Dengine.DataModel
{
    class cube
    {
        //***
        public Vector3 partSize { get; private set; }
        public Vector3 partPosition { get; private set; }
        //*** get and set data ^^^

        public int VertexArrayObject { get; private set; }
        public Texture Texture { get; private set; }
        public Shader shader;
        private Loader _loader = new Loader();
        public int vertexCount { get; private set; }

        public cube()
        {
            RawModel _rawmodel = _loader.LoadFromFile("stock/cube.obj");
            
            VertexArrayObject = _rawmodel.VAO;
            vertexCount = _rawmodel.VertexCount;
            

            shader = new Shader("shaders/shader.vert", "shaders/shader.frag");
            shader.Use();

            GL.BindVertexArray(VertexArrayObject);
        }

        public void render(camera _camera, Vector3 lightPos, Vector3 size, Vector3 position, Vector3 world_Color)
        {
            GL.BindVertexArray(VertexArrayObject);
            Matrix4 model = Matrix4.Identity;
            model *= Matrix4.CreateScale(size.X, size.Y, size.Z);
            model *= Matrix4.CreateTranslation(position.X, position.Y, position.Z);
            shader.SetMatrix4("model", model);
            shader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            shader.SetMatrix4("view", _camera.GetViewMatrix());
            shader.SetVector3("viewPos", _camera.Position);

            // Here we specify to the shaders what textures they should refer to when we want to get the positions.
            shader.SetInt("material.diffuse", 1);
            shader.SetFloat("material.shininess", 2f);

            shader.SetVector3("light.position", lightPos);
            shader.SetVector3("light.worldColor", world_Color);
            shader.SetVector3("light.ambient", new Vector3(0.4f));
            shader.SetVector3("light.diffuse", new Vector3(0.5f));
            shader.SetVector3("light.specular", new Vector3(0.05f));
            GL.DrawElements(PrimitiveType.Triangles, vertexCount, DrawElementsType.UnsignedInt, 0);
        }
    }
}
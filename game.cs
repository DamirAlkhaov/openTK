using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using Dengine.DataModel;
using System.Collections.Generic;

namespace Dengine
{
    public class game : GameWindow
    {
        cube part0;

        public Color4 world_Color = new Color4(0.7f, 0.7f, 0.9f, 1f);
        public Vector3 world_Color1 = new Vector3(0.7f, 0.7f, 0.9f);
        private Shader shader;
        private Shader lampshader;
        Random rnd = new Random();
        private camera _camera;
        private bool _firstMove = true;
        private Vector2 _lastPos;
        float cameraSpeed;
        float sensitivity = 0.25f;

        //part class stuff
        private Vector3 newsize = new Vector3(1f, 1f, 1f);
        private Vector3 partposition = new Vector3(5f, 5f, 0f);
        bool touched = false;

        //lighting
        private Vector3 lightPos = new Vector3(5f, 10f, -5f);
        private Vector3 lightsize = new Vector3(0.5f, 0.5f, 0.5f);
        private light light0;

        //list
        //Part cube = new Part();
        List<CustomPart> customparts = new List<CustomPart>();
        List<Vector3> cubesize = new List<Vector3>();
        List<Vector3> cubeposition = new List<Vector3>();


        public game(int width, int height, string title) : base(width, height, new GraphicsMode(new ColorFormat(8, 8, 8, 0), 24, 8, 4), title)
        {

        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {

            if (!Focused) // check to see if the window is focused
            {
                return;
            }

            var input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }



            if (input.IsKeyDown(Key.LControl))
            {
                cameraSpeed = 1f;
            }
            else
            {
                cameraSpeed = 15f;
            }

            if (input.IsKeyDown(Key.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Key.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Key.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Key.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Key.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Key.LShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
            }



            // Get the mouse state
            var mouse = Mouse.GetState();

            if (_firstMove) // this bool variable is initially set to true
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {

                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);
                if (mouse.IsButtonDown(MouseButton.Right))
                {
                    CursorGrabbed = true;
                    CursorVisible = false;

                    _camera.Yaw += deltaX * sensitivity;
                    _camera.Pitch -= deltaY * sensitivity;
                }
                else
                {
                    CursorGrabbed = false;
                    CursorVisible = true;
                }

            }

            if (input.IsKeyDown(Key.O) && !touched)
            {
                touched = true;
                Instantiate();
            }
            if (input.IsKeyUp(Key.O) && touched)
            {
                touched = false;
            }

            if (input.IsKeyDown(Key.K) && !touched)
            {
                touched = true;
                deleteAll();
            }
            if (input.IsKeyUp(Key.K) && touched)
            {
                touched = false;
            }


            if (input.IsKeyDown(Key.LAlt) && !touched)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }
            if (input.IsKeyUp(Key.LAlt) && touched)
            {
                touched = false;
            }

            if (input.IsKeyDown(Key.RAlt) && !touched)
            {
                GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
            }
            if (input.IsKeyUp(Key.RAlt) && touched)
            {
                touched = false;
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(world_Color);
            GL.Enable(EnableCap.DepthTest);
            
            GL.Enable(EnableCap.CullFace);

            Initialization();

            Console.WriteLine(this.Icon);

            shader = new Shader("shaders/shader.vert", "shaders/shader.frag");
            shader.Use();

            lampshader = new Shader("shaders/shader.vert", "shaders/light.frag");
            lampshader.Use();

            customparts.Add(new CustomPart(newsize, "Resources/Few1Tex.png", partposition, "Resources/few.obj"));
            customparts.Add(new CustomPart(newsize, "Resources/rubyfedora.png", partposition, "Resources/fedora.obj"));
            customparts.Add(new CustomPart(newsize, "Resources/scarf.png", partposition, "Resources/scarf.obj"));


            light0 = new light(lightsize, lightPos);
            _camera = new camera(Vector3.UnitZ * 3, Width / (float)Height);

            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
            GL.DeleteProgram(shader.Handle);
            GL.DeleteProgram(lampshader.Handle);
            base.OnUnload(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            this.Title = "D-Engine | FPS: " + Math.Ceiling(this.RenderFrequency / 10) * 10;

            //part class
            foreach (CustomPart item in customparts)
            {
                item.render(_camera, lightPos, world_Color1);
            }
            //cubes
            for(int i = 0; i < cubesize.Count; i++) { 
                part0.render(_camera, lightPos, cubesize[i], cubeposition[i], world_Color1);
            }

            //light0 cube
            light0.render(_camera);
            SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected void Instantiate()
        {
            for (int i = 0; i < 101; i++)
            {
                newsize.X = rnd.Next(1, 4);
                newsize.Y = rnd.Next(1, 4);
                newsize.Z = rnd.Next(1, 4);
                partposition.X = rnd.Next(-50, 51);
                partposition.Y = rnd.Next(-50, 51);
                partposition.Z = rnd.Next(-50, 51);
                cubesize.Add(newsize);
                cubeposition.Add(partposition);
            }

        }

        protected void deleteAll()
        {
            cubesize.Clear();
            cubeposition.Clear();
        }

        protected void Initialization()
        {
            part0 = new cube();
        }

    }
}

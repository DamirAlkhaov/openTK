

namespace Dengine.importer
{
    class TexturedModel
    {
        public readonly RawModel Model;
        public readonly Texture Texture;

        public TexturedModel(RawModel model, Texture texture)
        {
            Model = model;
            Texture = texture;
        }
    }
}

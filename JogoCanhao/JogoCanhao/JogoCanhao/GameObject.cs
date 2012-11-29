using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace JogoCanhao
{
    public abstract class GameObject
    {
        public Vector2 Posicao { get; set; }
        public Texture2D Imagem { get; set; }
        public SoundEffect Som { get; set; }
    }
}

using System;
using Microsoft.Xna.Framework;

namespace JogoCanhao
{
    public class BolaCanhao : GameObject
    {
        public const int INCREMENTO = 6;
        public float Radianos { get; set; }

        internal void CalcularProximaPosicao()
        {
            float x = Posicao.X + INCREMENTO * (float)Math.Cos(Radianos);
            float y = Posicao.Y - INCREMENTO * (float)Math.Sin(Radianos);
            Posicao = new Vector2(x, y);
        }
    }
}

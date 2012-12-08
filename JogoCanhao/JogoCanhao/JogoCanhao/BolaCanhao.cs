using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JogoCanhao
{
    public class BolaCanhao : GameObject
    {
        public const int INCREMENTO = 4;
        const float LIMITE_TELA = 20;
        public float Radianos { get; set; }
        public double TempoInicial { get; private set; }
        public Vector2 PosicaoInicial { get; set; }

        public BolaCanhao(GameTime gameTime)
        {
            TempoInicial = gameTime.TotalGameTime.TotalMilliseconds;
        }

        internal void CalcularProximaPosicao(GraphicsDevice tela, GameTime gameTime)
        {
            const float GRAVIDADE = 9.8f;
            const float VELOCIDADE = 120;
            double t = (gameTime.TotalGameTime.TotalMilliseconds - TempoInicial) / 1000;
            float x = PosicaoInicial.X + (float)(VELOCIDADE * Math.Cos(Radianos) * t);
            float y = tela.Viewport.Height - (float)(VELOCIDADE * Math.Sin(Radianos) * t) + (float)((GRAVIDADE * Math.Pow(t, 2)) / 2) - (tela.Viewport.Height - PosicaoInicial.Y);
            Posicao = new Vector2(x, y);
        }
    }
}

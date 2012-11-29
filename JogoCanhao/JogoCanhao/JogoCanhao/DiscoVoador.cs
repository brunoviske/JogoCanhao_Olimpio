using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JogoCanhao
{
    public class DiscoVoador : GameObject
    {
		public Enuns.DirecaoDisco Direcao { get; set; }
        static DiscoVoador _Instancia = null;
        public int Dano = 0;

        public bool isToExplode = false;
        public Texture2D sprite;
        public Rectangle sourceRect;
        public Vector2 origin;
        
        float timer = 0f;
        float interval = 60f;
        int spriteHeight = 126;
        int spriteWidth = 200;
        int currentFrame = 1;
        int lastFrame = 13;
        int x0 = 0;
        int y0 = 0;
        public bool fallen = false;
        struct Queda
        {
            public float Y_inicial { get; private set; }
            public bool EmQueda { get; private set; }
            public TimeSpan TempoInicial { get; private set; }

            /// <summary>
            /// Pontos por mili segundo
            /// </summary>
            const float VELOCIDADE_QUEDA = 0.4f;

            internal void IniciarQueda(float y, TimeSpan tempo)
            {
                EmQueda = true;
                Y_inicial = y;
                TempoInicial = tempo;
            }

            internal float CalcularNovoY(TimeSpan time)
            {
                double t = time.TotalMilliseconds - TempoInicial.TotalMilliseconds;
                return Y_inicial + VELOCIDADE_QUEDA * (float)t;
            }
        }

        Queda _Queda;
        public bool Destruido
        {
            get
            {
                return _Queda.EmQueda;
            }
        }

        private DiscoVoador()
        {
            _Queda = new Queda();
            Dano = 0;
        }


        public static DiscoVoador Instancia
        {
            get
            {
                if (_Instancia == null)
                {
                    _Instancia = new DiscoVoador();
                }
                return _Instancia;
            }
        }

        internal void Cair(GameTime gameTime)
        {
            _Queda.IniciarQueda(Posicao.Y, gameTime.TotalGameTime);
        }

        internal void VerificarMovimento(KeyboardState keyState, Canhao canhao, GraphicsDevice graphicsDevice, GameTime gameTime)
        {
            if (_Queda.EmQueda)
            {
                Cair(gameTime, graphicsDevice);
            }
            else
            {
                CapturarMovimentoUsuario(keyState, canhao, graphicsDevice);
            }
        }

        private void Cair(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            if (Posicao.Y < graphicsDevice.Viewport.Height - Imagem.Height)
            {
                float y = _Queda.CalcularNovoY(gameTime.TotalGameTime);
                Posicao = new Vector2(Posicao.X, y);
            }
            else
            {
                fallen = true;
            }
        }

        private void CapturarMovimentoUsuario(KeyboardState keyState, Canhao canhao, GraphicsDevice graphicsDevice)
        {
            Rectangle tela = graphicsDevice.Viewport.Bounds;
            float limiteAltura = tela.Height - Imagem.Height - canhao.Imagem.Height - (tela.Height - canhao.Posicao.Y);
            float horizontal = 0;
            float vertical = 0;
            int incremento = BolaCanhao.INCREMENTO / 2;
            if (keyState.IsKeyDown(Keys.Right) && Posicao.X < tela.Width - Imagem.Width)
            {
                horizontal = incremento;
            }
            else if (keyState.IsKeyDown(Keys.Left) && Posicao.X >= 0)
            {
                horizontal = -incremento;
            }
            else if (keyState.IsKeyDown(Keys.Up) && Posicao.Y >= 0)
            {
                vertical = -incremento;
            }
            else if (keyState.IsKeyDown(Keys.Down) && Posicao.Y < limiteAltura)
            {
                vertical = incremento;
            }
            Posicao = new Vector2(Posicao.X + horizontal, Posicao.Y + vertical);
        }
        //----
        public void updateExplosao(GameTime gameTime)
        {
            if (currentFrame == lastFrame)
            {
                currentFrame = 0;
                isToExplode = false;
                x0 = 0;
                y0 = 0;
            }

            if (isToExplode)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (timer > interval)
                {
                    currentFrame++;
                    x0 += spriteWidth;
                    timer = 0f;
                }

                sourceRect = new Rectangle(x0, y0, spriteWidth, (int)spriteHeight);
                origin = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);

                if (x0 >= spriteWidth * 3)
                {
                    x0 = 0;
                    y0 += (int)spriteHeight;
                }


            }
        }
    }
}

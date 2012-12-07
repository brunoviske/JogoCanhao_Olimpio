using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JogoCanhao
{
    public class DiscoVoador : GameObject
    {
        static DiscoVoador _Instancia = null;
        public int Dano = 0;

        public bool isToExplode = false;
        public Texture2D sprite;
        public Rectangle sourceRect;
        public Vector2 origin;
        public Enuns.DirecaoDisco Direcao;

        /// <summary>
        /// Aceleracao do disco voador em pixel/s^2
        /// </summary>
        public const float ACELERACAO = 1f;

        float timer = 0f;
        float interval = 60f;
        int spriteHeight = 126;
        int spriteWidth = 200;
        int currentFrame = 1;
        int lastFrame = 13;
        int x0 = 0;
        int y0 = 0;
        float velocidadeInicial = 0;
        float totalMiliseconds = 0;
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
                CapturarMovimentoUsuario(keyState, canhao, graphicsDevice, gameTime);
            }
            totalMiliseconds = (float)gameTime.TotalGameTime.TotalMilliseconds;
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

        private void CapturarMovimentoUsuario(KeyboardState keyState, Canhao canhao, GraphicsDevice graphicsDevice, GameTime time)
        {
            SetDirecao(keyState);
            bool houveMovimento = keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.Up);

            if (houveMovimento)
            {
                if (velocidadeInicial < 3.5)
                {
                    velocidadeInicial = velocidadeInicial + ACELERACAO * ((float)time.TotalGameTime.TotalMilliseconds - totalMiliseconds) / 1000;
                }
            }
            else
            {
                velocidadeInicial = velocidadeInicial - ACELERACAO * ((float)time.TotalGameTime.TotalMilliseconds - totalMiliseconds) / 1000;
                if (velocidadeInicial < 0)
                {
                    velocidadeInicial = 0;
                    return;
                }
            }

            Rectangle tela = graphicsDevice.Viewport.Bounds;
            float limiteAltura = tela.Height - Imagem.Height - canhao.Imagem.Height - (tela.Height - canhao.Posicao.Y);

            bool ultrapassarTela = (Direcao == Enuns.DirecaoDisco.Baixo && Posicao.Y >= limiteAltura)
                || (Direcao == Enuns.DirecaoDisco.Cima && Posicao.Y < 0)
                || (Direcao == Enuns.DirecaoDisco.Direita && Posicao.X >= tela.Width - Imagem.Width)
                || (Direcao == Enuns.DirecaoDisco.Esquerda && Posicao.X < 0);

            if (ultrapassarTela)
            {
                velocidadeInicial = 0;
            }
            else
            {
                if (Direcao == Enuns.DirecaoDisco.Baixo)
                {
                    Posicao = new Vector2(Posicao.X, Posicao.Y + velocidadeInicial);
                }
                else if (Direcao == Enuns.DirecaoDisco.Cima)
                {
                    Posicao = new Vector2(Posicao.X, Posicao.Y - velocidadeInicial);
                }
                else if (Direcao == Enuns.DirecaoDisco.Direita)
                {
                    Posicao = new Vector2(Posicao.X + velocidadeInicial, Posicao.Y);
                }
                else if (Direcao == Enuns.DirecaoDisco.Esquerda)
                {
                    Posicao = new Vector2(Posicao.X - velocidadeInicial, Posicao.Y);
                }
            }
        }

        private void SetDirecao(KeyboardState keyState)
        {
            if (keyState.IsKeyDown(Keys.Right))
                Direcao = Enuns.DirecaoDisco.Direita;
            else if (keyState.IsKeyDown(Keys.Left))
                Direcao = Enuns.DirecaoDisco.Esquerda;
            else if (keyState.IsKeyDown(Keys.Down))
                Direcao = Enuns.DirecaoDisco.Baixo;
            else if (keyState.IsKeyDown(Keys.Up))
                Direcao = Enuns.DirecaoDisco.Cima;
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

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JogoCanhao
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Declaração das Constantes
        const int COMPENSACAO = 20;
        #endregion

        #region Declaração das Propriedades
        Rectangle tela;

        public float AnguloCanhao { get; set; }
        private KeyboardState _currentKeyboardState;
        private List<BolaCanhao> bolasCanhao = null;
        double miliSegundoUltimoTiro = 0;
        bool isGameOver = false;
        Texture2D vida;
        #endregion

        #region Declaração dos objetos
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Canhao canhao = null;
        Cenario cenario = null;
        DiscoVoador disco = null;
        Roda roda = null;
        #endregion

        #region Métodos XNA Game
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            canhao = new Canhao();
            disco = DiscoVoador.Instancia;
            cenario = new Cenario();
            roda = new Roda();
            bolasCanhao = new List<BolaCanhao>();

            canhao.Imagem = Content.Load<Texture2D>(@"images/canhao");
            canhao.Posicao = new Vector2(25, 453);
            canhao.Som = Content.Load<SoundEffect>(@"sounds/tiro");

            roda.Imagem = Content.Load<Texture2D>(@"images/roda");
            roda.Posicao = new Vector2(23, 405);

            cenario.Imagem = Content.Load<Texture2D>(@"images/cenario1");
            vida = Content.Load<Texture2D>(@"images/barrahp");
            cenario.Posicao = new Vector2(0, 0);

            disco.Imagem = Content.Load<Texture2D>(@"images/disco");
            disco.Som = Content.Load<SoundEffect>(@"sounds/hit");
            disco.Posicao = new Vector2(0, 0);
            disco.sprite = Content.Load<Texture2D>(@"images/explosao");

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (!disco.fallen)
            {
                MoverDiscoVoador(gameTime);
                ControlarCanhao(gameTime);
                ControlarBolasCanhao(gameTime);
                disco.updateExplosao(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (!disco.fallen)
            {
                spriteBatch.Draw(cenario.Imagem, cenario.Posicao, Color.White);

                //Desenho da barrinha de vida
                spriteBatch.Draw(vida, new Rectangle(690 + disco.Dano, 10, 100 - disco.Dano, 40), Color.Red);

                spriteBatch.Draw(canhao.Imagem, canhao.Posicao, null, Color.White, -canhao.Radianos, new Vector2(0, canhao.Imagem.Height), 1.0f, SpriteEffects.None, 0f);

                spriteBatch.Draw(roda.Imagem, roda.Posicao, Color.White);

                spriteBatch.Draw(disco.Imagem, disco.Posicao, Color.White);

                spriteBatch.Draw(disco.sprite, new Vector2(disco.Posicao.X + 60, disco.Posicao.Y + 60), disco.sourceRect, Color.White, 0f, disco.origin, 1.0f, SpriteEffects.None, 0);
                foreach (var bola in bolasCanhao)
                {
                    spriteBatch.Draw(bola.Imagem, bola.Posicao, Color.White);
                }
            }
            else
            {
                spriteBatch.Draw(cenario.Imagem, cenario.Posicao, Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion

        #region Métodos Auxiliares
        private void ControlarCanhao(GameTime gameTime)
        {
            if (canhao.NaMira(GraphicsDevice))
            {
                if (!disco.Destruido && (gameTime.TotalGameTime.TotalMilliseconds - miliSegundoUltimoTiro) > 300)
                {
                    BolaCanhao b = canhao.Disparar(Content);
                    bolasCanhao.Add(b);
                    miliSegundoUltimoTiro = gameTime.TotalGameTime.TotalMilliseconds;
                }
            }
            else
            {
                canhao.CorrigirCoordenada(gameTime);
            }
        }

        private void MoverDiscoVoador(GameTime gameTime)
        {
            tela = GraphicsDevice.Viewport.Bounds;
            float limiteAltura = tela.Height - disco.Imagem.Height - canhao.Imagem.Height - (tela.Height - canhao.Posicao.Y);
            _currentKeyboardState = Keyboard.GetState();
            disco.VerificarMovimento(_currentKeyboardState, canhao, GraphicsDevice, gameTime);
        }

        private void ControlarBolasCanhao(GameTime gameTime)
        {
            for (int i = 0; i < bolasCanhao.Count; i++)
            {
                bolasCanhao[i].CalcularProximaPosicao();
                if (bolasCanhao[i].Posicao.Y < 0 || bolasCanhao[i].Posicao.X > GraphicsDevice.Viewport.Bounds.Width)
                {
                    bolasCanhao.RemoveAt(i);
                }
            }

            for (int i = 0; i < bolasCanhao.Count; i++)
            {
                Rectangle rBola = new Rectangle((int)bolasCanhao[i].Posicao.X, (int)bolasCanhao[i].Posicao.Y, (int)bolasCanhao[i].Imagem.Width, (int)bolasCanhao[i].Imagem.Height);
                Rectangle rDisco = new Rectangle((int)disco.Posicao.X + COMPENSACAO, (int)disco.Posicao.Y + COMPENSACAO, (int)disco.Imagem.Width - COMPENSACAO, (int)disco.Imagem.Height - COMPENSACAO);

                //colisao com a nave
                if (rBola.Intersects(rDisco))
                {
                    disco.Som.Play();
                    disco.isToExplode = true;
                    bolasCanhao.RemoveAt(i);
                    disco.Dano += 10;
                    if (disco.Dano >= 100)
                    {
                        cenario.Imagem = Content.Load<Texture2D>(@"images/gameover");
                        canhao.Imagem = Content.Load<Texture2D>(@"images/null");
                        roda.Imagem = Content.Load<Texture2D>(@"images/null");
                        disco.Cair(gameTime);
                        if (disco.fallen == true)
                        {

                            disco.Imagem = Content.Load<Texture2D>(@"images/null");
                            disco.sprite = Content.Load<Texture2D>(@"images/null");
                        }
                    }
                }
            }
        }
        #endregion
    }
}
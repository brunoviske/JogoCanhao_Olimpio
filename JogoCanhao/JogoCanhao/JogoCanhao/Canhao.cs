using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace JogoCanhao
{
    public class Canhao : GameObject
    {
        class InformacaoDiscoVoador
        {
            private InformacaoDiscoVoador() { }

            public float meiaLargura;
            public float meiaAltura;
            public float xDiscoVoador;
            public float yDiscoVoador;
            public float catetoOposto;
            public float catetoAdvacente;

            /// <summary>
            /// A hipotenusa do triângulo retângulo
            /// </summary>
            public float distancia;

            public static InformacaoDiscoVoador GetInformacao(Canhao canhao)
            {
                InformacaoDiscoVoador inf = new InformacaoDiscoVoador();
                DiscoVoador discoVoador = DiscoVoador.Instancia;
                inf.meiaLargura = discoVoador.Imagem.Height * 1.0f / 2;
                inf.meiaAltura = discoVoador.Imagem.Width * 1.0f / 2;
                inf.yDiscoVoador = discoVoador.Posicao.Y + inf.meiaLargura;
                inf.xDiscoVoador = discoVoador.Posicao.X + inf.meiaAltura;
                inf.catetoOposto = canhao.Posicao.Y - inf.yDiscoVoador;
                inf.catetoAdvacente = inf.xDiscoVoador - canhao.Posicao.X;
                inf.distancia = (float)Math.Sqrt(Math.Pow(inf.catetoOposto, 2) + Math.Pow(inf.catetoAdvacente, 2));
                return inf;
            }
        }

        const float CORRECAO_ANGULO = 43;
        const float MARGEM_ERRO = 5;
        float _anguloReal;
        public float Angulo
        {
            get
            {
                return _anguloReal;
            }
            set
            {
                if ((value + CORRECAO_ANGULO) >= 0 && (value + CORRECAO_ANGULO) <= 90)
                {
                    _anguloReal = value;
                }
            }
        }
        public float Radianos
        {
            get
            {
                return ConverterGrauEmRadiano(Angulo);
            }
        }

        /// <summary>
        /// Angulo atual somado ao angulo do canhao na imagem. Angulo real da haste do canhao.
        /// </summary>
        float AnguloCorrigido
        {
            get
            {
                return Angulo + CORRECAO_ANGULO;
            }
        }

        /// <summary>
        /// Radianos da propriedade AnguloCorrigido
        /// </summary>
        float RadianosCorrigido
        {
            get
            {
                return ConverterGrauEmRadiano(AnguloCorrigido);
            }
        }

        public bool NaMira(GraphicsDevice graphicsDevice)
        {
            //Calcula a distância entre a base do canhão e o centro do disco voador
            InformacaoDiscoVoador inf = InformacaoDiscoVoador.GetInformacao(this);
            float x = Posicao.X + inf.distancia * (float)Math.Cos(RadianosCorrigido);
            float y = Posicao.Y - inf.distancia * (float)Math.Sin(RadianosCorrigido);
            return Math.Abs(x - inf.xDiscoVoador) <= MARGEM_ERRO && Math.Abs(y - inf.yDiscoVoador) <= MARGEM_ERRO;
        }

        public BolaCanhao Disparar(ContentManager content)
        {
            BolaCanhao b = new BolaCanhao();
            b.Imagem = content.Load<Texture2D>(@"images/bolacanhao");
            b.Posicao = CalcularPosicaoBola(b);
            b.Radianos = RadianosCorrigido;
            Som.Play();
            return b;
        }

        float ConverterGrauEmRadiano(float grau)
        {
            return grau * (float)Math.PI / 180;
        }

        private Vector2 CalcularPosicaoBola(BolaCanhao b)
        {
            Vector2 vetor = new Vector2();
            float hipotenusa = (float)Math.Sqrt(Math.Pow(Imagem.Width, 2) + Math.Pow(Imagem.Height, 2));
            vetor.X = Posicao.X + hipotenusa * (float)Math.Cos(RadianosCorrigido);
            vetor.Y = Posicao.Y - hipotenusa * (float)Math.Sin(RadianosCorrigido);

            if (AnguloCorrigido > 45)
            {
                vetor.Y -= (b.Imagem.Height * 1.0f / 2) * (float)Math.Sin(Radianos);
            }
            else if (AnguloCorrigido < 45)
            {
                vetor.X += (b.Imagem.Width * 1.0f / 2) * (float)Math.Sin(Radianos);
            }
            return vetor;
        }

        double miliSegundoUltimaAcao = 0;

        internal void CorrigirCoordenada(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds - miliSegundoUltimaAcao > 200)
            {
                const float velocidade = 1.2f;
                InformacaoDiscoVoador inf = InformacaoDiscoVoador.GetInformacao(this);
                float seno = inf.catetoOposto / inf.distancia;
                float radiano = (float)Math.Asin(seno);
                if (radiano > RadianosCorrigido)
                {
                    Angulo += velocidade;
                }
                else if (radiano < RadianosCorrigido)
                {
                    Angulo -= velocidade;
                }
                miliSegundoUltimaAcao = gameTime.TotalGameTime.TotalMilliseconds;
            }
        }
    }
}

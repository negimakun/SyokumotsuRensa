using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace ClickMove
{
    class Renderer
    {
        private ContentManager contentManager; //コンテンツ管理者
        private GraphicsDevice graphicsDevice; //グラフィック機器
        private SpriteBatch spriteBatch; //スプライト一括描画用オブジェクト
        private RenderTarget2D target2D;//2Dレンダーターゲット

        //複数画像管理用変数の宣言と生成
        private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="content">Game1クラスのコンテンツ管理者</param>
        /// <param name="graphics">Game1クラスのグラフィック機器</param>
        public Renderer(ContentManager content, GraphicsDevice graphics)
        {
            contentManager = content;
            graphicsDevice = graphics;
            spriteBatch = new SpriteBatch(graphicsDevice);
        }

        /// <summary>
        /// 画像の読み込み
        /// </summary>
        /// <param name="assetName">アセット名（ファイルの名前）</param>
        /// <param name="filepath">画像へのファイルパス</param>
        public void LoadContent(string assetName, string filepath = "./")
        {
            //すでにキー（assetName：アセット名）が登録されているとき
            if (textures.ContainsKey(assetName))
            {
#if DEBUG //DEBUGモードの時のみ下記エラー分をコンソールへ表示
                Console.WriteLine(assetName + "はすでに読み込まれています。\n プログラムを確認してください。");
#endif

                //それ以上読み込まないのでここで終了
                return;
            }
            //画像の読み込みとDictionaryへアセット名と画像を登録
            textures.Add(assetName, contentManager.Load<Texture2D>(filepath + assetName));

        }

        /// <summary>
        /// アンロード
        /// </summary>
        public void Unload()
        {
            textures.Clear();//Dictionaryの情報をクリア
        }

        /// <summary>
        /// 描画開始
        /// </summary>
        public void Begin()
        {
            spriteBatch.Begin();
        }

        /// <summary>
        /// 描画終了
        /// </summary>
        public void End()
        {
            spriteBatch.End();
        }

        /// <summary>
        /// 画像の描画（画像サイズはそのまま）
        /// </summary>
        /// <param name="assetName">アセット名</param>
        /// <param name="position">位置</param>
        /// <param name="alpha">透明値（1.0f：不透明 0.0f：透明）</param>
        public void DrawTexture(string assetName, Vector2 position, float alpha = 1.0f)
        {
            //デバッグモードの時のみ、画像描画前のアセット名チェック
            Debug.Assert(
                textures.ContainsKey(assetName),
                "描画時にアセット名の指定を間違えたか、画像の読み込み自体できていません");

            spriteBatch.Draw(textures[assetName], position, Color.White * alpha);
        }

        public void Draw(Texture2D texture2D, Rectangle rect)
        {
            spriteBatch.Draw(texture2D, rect, Color.White);
        }

        public void Draw(Texture2D texture2D, Rectangle rect, Color color)
        {
            spriteBatch.Draw(texture2D, rect, color);
        }

        /// <summary>
        /// 画像の描画（画像を指定範囲内だけ描画）
        /// </summary>
        /// <param name="assetName">アセット名</param>
        /// <param name="position">位置</param>
        /// <param name="rect">指定範囲</param>
        /// <param name="alpha">透明値</param>
        public void DrawTexture(string assetName, Vector2 position, Rectangle rect, float alpha = 1.0f)
        {
            //デバッグモードの時のみ、画像描画前のアセット名チェック
            Debug.Assert(
                textures.ContainsKey(assetName),
                "描画時にアセット名の指定を間違えたか、画像の読み込み自体できていません");

            spriteBatch.Draw(
                textures[assetName], //テクスチャ
                position,            //位置
                rect,                //指定範囲（矩形で指定：左上の座標、幅、高さ）
                Color.White * alpha);//透明値
        }


        /// <summary>
        /// 数字の描画（整数のみ）
        /// </summary>
        /// <param name="aseetName">数字画像の名前</param>
        /// <param name="position">位置</param>
        /// <param name="number">表示したい整数値</param>
        /// <param name="alpha">透明値</param>
        public void DrawNumber(string aseetName, Vector2 position, int number, float alpha = 1.0f)
        {
            //デバッグモードのときのみ、画像描画前のアセット名チェック
            Debug.Assert(
                textures.ContainsKey(aseetName),
         "描画時にアセット名の指定を間違えたか、" + "画像の読み込み自体できていません");

            //マイナスの数は0
            if (number < 0)
            {
                number = 0;
            }

            int width = 32;//画像横幅

            //数字を文字列化し、１文字ずつ取り出す
            foreach (var n in number.ToString())
            {
                //数字のテクスチャが数字１つにつき幅３２高さ６４
                //文字と文字を引き算し、整数値を取得している
                spriteBatch.Draw(
                    textures[aseetName], position, new Rectangle((n - '0') * width, 0, width, 64),
                    Color.White);

                //１文字描画したら１桁分右にずらす
                position.X += width;

            }
        }


        /// <summary>
        /// 数字の描画（実数、小数点以下は２桁表示）
        /// </summary>
        /// <param name="aseetaName">数字画像の名前</param>
        /// <param name="position">位置</param>
        /// <param name="numbrt">表示したい実数値</param>
        /// <param name="alpha">透明値</param>
        public void DrawNumber(string assetName, Vector2 position, float number, float alpha = 1.0f)
        {
            //マイナスは０へ
            if (number < 0.0f)
            {
                number = 0.0f;
            }

            int width = 32;//数字画像１つ分の横幅
            //少数部分は２桁まで、整数部が１桁のときは０で埋める
            foreach (var n in number.ToString("00.00"))
            {
                //少数の「.」か？
                if (n == '.')
                {
                    spriteBatch.Draw(textures[assetName], position,
                        new Rectangle(10 * width, 0, width, 64),//ピリオドは１０番目
                       Color.White * alpha);
                }
                else
                {
                    //数字の描画
                    spriteBatch.Draw(textures[assetName], position,
                        new Rectangle((n - '0') * width, 0, width, 64), Color.White * alpha);
                }

                //１文字分描画したら１桁分右にずらす
                position.X += width;
            }

        }

        public void LoadContent(string assetName, Texture2D texture)
        {
            if (textures.ContainsKey(assetName))
            {
#if DEBUG
                Console.WriteLine(assetName + "はすでに読み込まれています。/n" +
                    "プログラムを確認してください。");
#endif
                return;
            }
            textures.Add(assetName, texture);
        }

        public void DrawTexture(string assetName, Vector2 position,
            Rectangle? rect, float rotate,
            Vector2 rotatePosition, Vector2 scale,
            SpriteEffects effects = SpriteEffects.None,
            float depth = 0.0f, float alpha = 1.0f)
        {
            spriteBatch.Draw(textures[assetName],
                position, rect, Color.White * alpha, rotate, rotatePosition
                , scale, effects, depth);
        }

        public void DrawTexture(string assetName, Vector2 position, Color color,
            float alpha = 1.0f)
        {
            Debug.Assert(textures.ContainsKey(assetName), "描画時にアセット名の指定を間違えたか、" +
                "画像の読み込み自体できていません");

            spriteBatch.Draw(textures[assetName], position, color * alpha);
        }



        /// <summary>
        /// レンダーターゲットの初期化
        /// テクスチャサイズの設定
        /// </summary>
        /// <param name="width">テクスチャ横幅</param>
        /// <param name="height">テクスチャ縦幅</param>
        public void InitializeRenderTarget(int width, int height)
        {
            target2D = new RenderTarget2D(graphicsDevice, width, height);
        }



        /// <summary>
        /// レンダーターゲット先への描画開始
        /// </summary>
        public void BeginRenderTarget()
        {
            //描画するレンダーターゲットを設定
            graphicsDevice.SetRenderTarget(target2D);
            //描画先のレンダーターゲットの内容を消す
            graphicsDevice.Clear(Color.Transparent);
            //レンダーターゲットへの描画開始
            spriteBatch.Begin();
        }


        /// <summary>
        /// レンダーターゲットへの描画終了
        /// </summary>
        public void EndRenderTarget()
        {
            //レンダーターゲットへの描画終了
            spriteBatch.End();
            //描画を画面に変える
            graphicsDevice.SetRenderTarget(null);
        }


        /// <summary>
        /// レンダーターゲットのテクスチャを描画
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rect"></param>
        /// <param name="rotate"></param>
        /// <param name="color"></param>
        /// <param name="effects"></param>
        /// <param name="depth"></param>
        public void DrawRenderTargetTexture(
            Vector2 position,
            Rectangle? rect,//nullを受けられるよう「？」で
            float rotate,
            Vector2 rotatePosition,
            float scale,
            Color color,
            SpriteEffects effects = SpriteEffects.None,
            float depth = 0.0f
            )
        {
            spriteBatch.Draw(
                target2D,
                position,
                rect,
                color,
                rotate,
                rotatePosition,
                scale,
                effects,
                depth
                );
        }


        /// <summary>
        /// 描画開始
        /// </summary>
        /// <param name="sortMode">ソートモード</param>
        /// <param name="blendState">合成状態</param>
        public void Begin(SpriteSortMode sortMode, BlendState blendState)
        {
            spriteBatch.Begin(sortMode, blendState);
        }


    }
}

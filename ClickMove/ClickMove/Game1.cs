// このファイルで必要なライブラリのnamespaceを指定
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using ClickMove.CSV;
using ClickMove.Music;

/// <summary>
/// プロジェクト名がnamespaceとなります
/// </summary>
namespace ClickMove
{
    /// <summary>
    /// ゲームの基盤となるメインのクラス
    /// 親クラスはXNA.FrameworkのGameクラス
    /// </summary>
    public class Game1 : Game
    {
        // フィールド（このクラスの情報を記述）
        private GraphicsDeviceManager graphicsDeviceManager;//グラフィックスデバイスを管理するオブジェクト
        private SpriteBatch spriteBatch;//画像をスクリーン上に描画するためのオブジェクト

        private GameDevice gameDevice;

        Renderer renderer;

        private BGMLoader bgmLoader;

        Player player;
        List<Player> players;

        Wall wall;
        List<Wall> walls;

        EnemyLevel1 Top;
        EnemyLevel1 Bottom;
        EnemyLevel1 Right;
        EnemyLevel1 Left;
        List<EnemyLevel1> eL1List;

        List<Unchi> unchis;

        Camp camp;
        
        /// <summary>
        /// コンストラクタ
        /// （new で実体生成された際、一番最初に一回呼び出される）
        /// </summary>
        public Game1()
        {
            //グラフィックスデバイス管理者の実体生成
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            //コンテンツデータ（リソースデータ）のルートフォルダは"Contentに設定
            Content.RootDirectory = "Content";

            IsMouseVisible = true;


            graphicsDeviceManager.PreferredBackBufferWidth = Screen.ScreenWidth;
            graphicsDeviceManager.PreferredBackBufferHeight = Screen.ScreenHeight;
        }

        /// <summary>
        /// 初期化処理（起動時、コンストラクタの後に1度だけ呼ばれる）
        /// </summary>
        protected override void Initialize()
        {
            // この下にロジックを記述
            gameDevice = GameDevice.Instance(Content, GraphicsDevice);

            bgmLoader = new BGMLoader(new string[,] { { "gamePlay", "./Sound/" } });


            CSVReader csvReader = new CSVReader();
            csvReader.Read("spawn.csv");



            unchis = new List<Unchi>();
            player = new Player();
            camp = new Camp();

            players = new List<Player>();
            players.Add(player);

            wall = new Wall(new Vector2(700, 200), new Rectangle(0, 0, 10 * 50, 1 * 50));
            walls = new List<Wall>();
            walls.Add(wall);
            walls.Add(new Wall(new Vector2(500, 400), new Rectangle(0, 0, 2 * 50, 5 * 50)));
            walls.Add(new Wall(new Vector2(700, 800), new Rectangle(0, 0, 8 * 50, 1 * 50)));
            walls.Add(new Wall(new Vector2(1200, 400), new Rectangle(0, 0, 2 * 50, 5 * 50)));

            foreach (var wa in walls)
            {
                wa.Initialize();
            }

            Top = new EnemyLevel1(Direction.TOP, camp, players, walls, 0, unchis);
            Bottom = new EnemyLevel1(Direction.BOTTOM, camp, players, walls, 1, unchis);
            Right = new EnemyLevel1(Direction.RIGHT, camp, players, walls, 5, unchis);
            Left = new EnemyLevel1(Direction.LEFT, camp, players, walls, 9, unchis);

            eL1List = new List<EnemyLevel1>();
            eL1List.Add(Top);
            eL1List.Add(Bottom);
            eL1List.Add(Right);
            eL1List.Add(Left);

            EnemyCSVParser parser = new EnemyCSVParser(camp, players, walls,unchis);
            var dataList = parser.Parse("spawn.csv", "./");
            foreach (var data in dataList)
            {
                eL1List.Add(data);
            }
            

            foreach (var el1 in eL1List)
            {
                el1.Initialze();
            }


            // この上にロジックを記述
            base.Initialize();// 親クラスの初期化処理呼び出し。絶対に消すな！！
        }

        /// <summary>
        /// コンテンツデータ（リソースデータ）の読み込み処理
        /// （起動時、１度だけ呼ばれる）
        /// </summary>
        protected override void LoadContent()
        {
            // 画像を描画するために、スプライトバッチオブジェクトの実体生成
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // この下にロジックを記述
            renderer = new Renderer(Content, GraphicsDevice);
            renderer.LoadContent("backpi");
            renderer.LoadContent("clearor");
            renderer.LoadContent("clearpu");
            renderer.LoadContent("waku");
            renderer.LoadContent("1000");
            renderer.LoadContent("chicken");
            renderer.LoadContent("glass");
            renderer.LoadContent("house");
            renderer.LoadContent("number");
            renderer.LoadContent("pig");
            renderer.LoadContent("tile");
            renderer.LoadContent("unchi");
            renderer.LoadContent("wolf");

            // この上にロジックを記述
        }

        /// <summary>
        /// コンテンツの解放処理
        /// （コンテンツ管理者以外で読み込んだコンテンツデータを解放）
        /// </summary>
        protected override void UnloadContent()
        {
            // この下にロジックを記述


            // この上にロジックを記述
        }

        /// <summary>
        /// 更新処理
        /// （1/60秒の１フレーム分の更新内容を記述。音再生はここで行う）
        /// </summary>
        /// <param name="gameTime">現在のゲーム時間を提供するオブジェクト</param>
        protected override void Update(GameTime gameTime)
        {
            // ゲーム終了処理（ゲームパッドのBackボタンかキーボードのエスケープボタンが押されたら終了）
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) ||
                 (Keyboard.GetState().IsKeyDown(Keys.Escape)))
            {
                Exit();
            }

            // この下に更新ロジックを記述

            Input.Update();
            foreach (var pl in players)
            {
                pl.Update();
            }

            if (Input.IsMouseRButtonDown())
            {
                players.Add(new Player());
            }

            foreach (var el1 in eL1List)
            {
                el1.Update();
            }

            foreach (var wa in walls)
            {
                wa.Update();
            }

            foreach (var un in unchis)
            {
                un.Update();
            }

            for (int i = eL1List.Count - 1; i >= 0; i--)
            {
                if (eL1List[i].moveEndFlag)
                {
                    eL1List.RemoveAt(i);
                }
                else if (eL1List[i].stuffMAXFlag)
                {
                    eL1List.RemoveAt(i);
                }
            }

            for (int i = unchis.Count - 1; i >= 0  ; i--)
            {
                if (unchis[i].iswwwFlag)
                {
                    unchis.RemoveAt(i);
                }
            }

            // この上にロジックを記述
            base.Update(gameTime); // 親クラスの更新処理呼び出し。絶対に消すな！！
        }

        /// <summary>
        /// 描画処理
        /// </summary>
        /// <param name="gameTime">現在のゲーム時間を提供するオブジェクト</param>
        protected override void Draw(GameTime gameTime)
        {
            // 画面クリア時の色を設定
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // この下に描画ロジックを記述
            renderer.Begin();

            //仮マップ
            for (int i = 0; i < Screen.ScreenWidth / 50+50; i++)
            {
                for (int j = 0; j < Screen.ScreenHeight / 50+50; j++)
                {
                    renderer.DrawTexture("tile", new Vector2(i * 50, j * 50));
                }
            }


            foreach (var pl in players)
            {
                pl.Draw(renderer);
            }

            camp.Draw(renderer);

            foreach (var el1 in eL1List)
            {
                el1.Draw(renderer);
            }


            foreach (var un in unchis)
            {
                un.Draw(renderer);
            }

            //仮選択位置
            if (!Input.IsMouseLButton())
            {
                renderer.DrawTexture("clearor", new Vector2((int)(Input.MousePosition.X / 50) * 50, (int)(Input.MousePosition.Y / 50) * 50));
            }
            else
            {
                renderer.DrawTexture("clearpu", new Vector2((int)(Input.MousePosition.X / 50) * 50, (int)(Input.MousePosition.Y / 50) * 50));
            }

            //仮壁
            foreach (var wa in walls)
            {
                wa.Draw(renderer);
            }

            //仮UI位置
            renderer.DrawTexture("1000", Vector2.Zero, new Rectangle(0, 0, 300, Screen.ScreenHeight));

            renderer.End();

            //この上にロジックを記述
            base.Draw(gameTime); // 親クラスの更新処理呼び出し。絶対に消すな！！
        }
    }
}

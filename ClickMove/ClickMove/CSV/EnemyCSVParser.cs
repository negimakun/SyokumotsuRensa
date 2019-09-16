using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace ClickMove.CSV
{
    /// <summary>
    /// ゲームオブジェクトがまとめられたCSVデータの解析
    /// </summary>
    class EnemyCSVParser
    {
        private CSVReader csvReader;//CSV読込用オブジェクト
        private List<EnemyLevel1> enemys;//ゲームオブジェクトのリスト
        Camp baseCamp;
        List<Player> playerList;
        List<Wall> wallList;
        List<Unchi> unchiList;

        //デリケート宣言（メソッドを変数に保存するための型宣言)
        //戻り値の型がGameObject，引数はList<string>のメソッドを
        //保存できるiFunction型を宣言
        private delegate EnemyLevel1 iFunction(List<string> data);

        //文字列とiFunction型をディクショナリで保存
        private Dictionary<string, iFunction> functionTable;

        //private IGameObjectMediator mediator;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EnemyCSVParser(Camp camp, List<Player> players, List<Wall> walls, List<Unchi> unchis)
        {
            baseCamp = camp;
            playerList = players;
            wallList = walls;
            unchiList = unchis;
            //this.mediator = mediator;

            //CSV読込の実体生成
            csvReader = new CSVReader();

            //ゲームオブジェクトリストの実体生成
            enemys = new List<EnemyLevel1>();

            //文字列とメソッドを保存するディクショナリの実体生成
            functionTable = new Dictionary<string, iFunction>();

            //ディクショナリにデータを追加
            //文字列はクラス名の文字列と実行用のメソッド名
            functionTable.Add("EnemyLevel1", NewEnemyLevel1);
            //functionTable.Add("SlidingBlock", NewSlidingBlock);
            //functionTable.Add("Block", NewBlock);
            //functionTable.Add("ChaseEnemy", NewChaseEnemy);
            //functionTable.Add("JumpingEnemy", NewJumpingEnemy);
            //functionTable.Add("HPEnemy", NewHPEnemy);
            //functionTable.Add("NeerEnemy", NewNeerEnemy);
        }

        /// <summary>
        /// 解析
        /// 後でListの中身をEnemyという親オブジェクトに変更
        /// </summary>
        /// <param name="filename">CSVファイル名（拡張子も必要）</param>
        /// <param name="path">ゲームオブジェクトリスト</param>
        /// <returns></returns>
        public List<EnemyLevel1> Parse(string filename, string path = "./")
        {
            //リストをクリア
            enemys.Clear();

            //CSV読込
            csvReader.Read(filename, path);
            //List<string[]>でcsvデータを取得
            var data = csvReader.GetData();

            //1行ごと解析
            foreach (var line in data)
            {
                //1列目が#の時はコメント行として次へ
                if (line[0] == "#")
                {
                    continue;
                }
                //1列目が空文字だった場合もコメント行として次へ
                if (line[0] == "")
                {
                    continue;
                }

                //空白文字削除処理
                var temp = line.ToList();//配列からListに変換
                temp.RemoveAll(s => s == "");

                //ゲームオブジェクトリストに解析後作られたゲームオブジェクトを追加
                enemys.Add(functionTable[line[0]](temp));
            }

            return enemys;
        }

        private EnemyLevel1 NewEnemyLevel1(List<string> data)
        {
            //読み込んだ行の列数が正しいかチェック
            Debug.Assert(
                (data.Count == 3),
                "CSVデータを確認してください。");
            if (data[1] == "RIGHT")
            {
                return new EnemyLevel1(Direction.RIGHT, baseCamp, playerList, wallList, int.Parse(data[2]), unchiList);
            }
            else if (data[1] == "LEFT")
            {
                return new EnemyLevel1(Direction.LEFT, baseCamp, playerList, wallList, int.Parse(data[2]), unchiList);
            }
            else if (data[1] == "TOP")
            {
                return new EnemyLevel1(Direction.TOP, baseCamp, playerList, wallList, int.Parse(data[2]), unchiList);
            }
            else if (data[1] == "BOTTOM")
            {
                return new EnemyLevel1(Direction.BOTTOM, baseCamp, playerList, wallList, int.Parse(data[2]), unchiList);
            }

            return null;
        }



        ///// <summary>
        ///// 移動ブロックの解析を生成
        ///// </summary>
        ///// <param name="data">移動ブロック用データリスト</param>
        ///// <returns>移動ブロックオブジェクト</returns>
        //private SlidingBlock NewSlidingBlock(List<string> data)
        //{
        //    //読み込んだ行の列数が正しいかチェック
        //    Debug.Assert(
        //        (data.Count == 5) || (data.Count == 6) || (data.Count == 7) || (data.Count == 8),
        //        "CSVデータを確認してください。");

        //    if (data.Count == 5)//移動なし版
        //    {
        //        return new SlidingBlock(
        //            new Vector2(float.Parse(data[1]), float.Parse(data[2])) * 32,
        //            new Vector2(float.Parse(data[3]), float.Parse(data[4])) * 32,
        //            GameDevice.Instance());
        //    }
        //    else if (data.Count == 6)//移動なしIDあり版
        //    {
        //        SlidingBlock slidingBlock = new SlidingBlock(
        //            new Vector2(float.Parse(data[1]), float.Parse(data[2])) * 32,
        //            new Vector2(float.Parse(data[3]), float.Parse(data[4])) * 32,
        //            GameDevice.Instance());

        //        slidingBlock.SetID(stringToGameObjectID_Enum(data[5]));

        //        return slidingBlock;
        //    }
        //    else if (data.Count == 7)//移動量あり版
        //    {
        //        return new SlidingBlock(
        //            new Vector2(float.Parse(data[1]), float.Parse(data[2])) * 32,
        //            new Vector2(float.Parse(data[3]), float.Parse(data[4])) * 32,
        //            new Vector2(float.Parse(data[5]), float.Parse(data[6])),
        //            GameDevice.Instance());
        //    }
        //    else if (data.Count == 8)//移動量ありIDあり版
        //    {
        //        SlidingBlock slidingBlock = new SlidingBlock(
        //            new Vector2(float.Parse(data[1]), float.Parse(data[2])) * 32,
        //            new Vector2(float.Parse(data[3]), float.Parse(data[4])) * 32,
        //            new Vector2(float.Parse(data[5]), float.Parse(data[6])),
        //            GameDevice.Instance());

        //        slidingBlock.SetID(stringToGameObjectID_Enum(data[7]));

        //        return slidingBlock;
        //    }

        //    return null;
        //}

        ///// <summary>
        ///// 通常ブロックの解析と生成
        ///// </summary>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //private Block NewBlock(List<string> data)
        //{
        //    Debug.Assert(
        //        (data.Count == 3) || (data.Count == 4),
        //        "CSVデータを確認してください");
        //    if (data.Count == 3)//IDなし版
        //    {
        //        return new Block(
        //        new Vector2(float.Parse(data[1]), float.Parse(data[2])) * 32,
        //        GameDevice.Instance());
        //    }
        //    else if (data.Count == 4)//IDあり版
        //    {
        //        Block block = new Block(
        //        new Vector2(float.Parse(data[1]), float.Parse(data[2])) * 32,
        //        GameDevice.Instance());

        //        block.SetID(stringToGameObjectID_Enum(data[3]));

        //        return block;
        //    }

        //    return null;
        //}

        ///// <summary>
        ///// ChaseEnemyの解析と追加
        ///// </summary>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //private ChaseEnemy NewChaseEnemy(List<string> data)
        //{
        //    Debug.Assert(
        //        (data.Count == 3) || (data.Count == 4),
        //        "CSVデータを確認してください");

        //    if (data.Count == 3)//IDなし版
        //    {
        //        return new ChaseEnemy(
        //        new Vector2(float.Parse(data[1]), float.Parse(data[2])) * 32,
        //        GameDevice.Instance(),
        //        mediator);
        //    }
        //    else if (data.Count == 4)//IDあり版
        //    {
        //        ChaseEnemy chaseEnemy = new ChaseEnemy(
        //        new Vector2(float.Parse(data[1]), float.Parse(data[2])) * 32,
        //        GameDevice.Instance(),
        //        mediator);

        //        chaseEnemy.SetID(stringToGameObjectID_Enum(data[3]));

        //        return chaseEnemy;
        //    }

        //    return null;
        //}

        ///// <summary>
        ///// JumpingEnemyの解析と追加
        ///// </summary>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //private JumpingEnemy NewJumpingEnemy(List<string> data)
        //{
        //    Debug.Assert(
        //        (data.Count == 3) || (data.Count == 4),
        //        "CSVデータを確認してください");
        //    if (data.Count == 3)//IDなし版
        //    {
        //        return new JumpingEnemy(
        //        new Vector2(float.Parse(data[1]), float.Parse(data[2])) * 32,
        //        GameDevice.Instance(),
        //        mediator);
        //    }
        //    else if (data.Count == 4)//IDあり版
        //    {
        //        JumpingEnemy jumpingEnemy = new JumpingEnemy(
        //        new Vector2(float.Parse(data[1]), float.Parse(data[2])) * 32,
        //        GameDevice.Instance(),
        //        mediator);

        //        jumpingEnemy.SetID(stringToGameObjectID_Enum(data[3]));

        //        return jumpingEnemy;
        //    }

        //    return null;
        //}

        ///// <summary>
        ///// HPEnemyの解析と追加
        ///// </summary>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //private HPEnemy NewHPEnemy(List<string> data)
        //{
        //    Debug.Assert(
        //        (data.Count == 3) || (data.Count == 4),
        //        "CSVデータを確認してください");

        //    if (data.Count == 3)//IDなし版
        //    {
        //        return new HPEnemy(
        //        new Vector2(float.Parse(data[1]), float.Parse(data[2])) * 32,
        //        GameDevice.Instance(),
        //        mediator);
        //    }
        //    else if (data.Count == 4)//IDあり版
        //    {
        //        HPEnemy hpEnemy = new HPEnemy(
        //        new Vector2(float.Parse(data[1]), float.Parse(data[2])) * 32,
        //        GameDevice.Instance(),
        //        mediator);

        //        hpEnemy.SetID(stringToGameObjectID_Enum(data[3]));

        //        return hpEnemy;
        //    }

        //    return null;
        //}

        ///// <summary>
        ///// NeerEnemyの解析と追加
        ///// </summary>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //private NeerEnemy NewNeerEnemy(List<string> data)
        //{
        //    Debug.Assert(
        //        (data.Count == 3) || (data.Count == 4),
        //        "CSVデータを確認してください");
        //    if (data.Count == 3)//IDなし版
        //    {
        //        return new NeerEnemy(
        //        new Vector2(float.Parse(data[1]), float.Parse(data[2])) * 32,
        //        GameDevice.Instance(),
        //        mediator);
        //    }
        //    else if (data.Count == 4)//IDあり版
        //    {
        //        NeerEnemy neerEnemy = new NeerEnemy(
        //        new Vector2(float.Parse(data[1]), float.Parse(data[2])) * 32,
        //        GameDevice.Instance(),
        //        mediator);

        //        neerEnemy.SetID(stringToGameObjectID_Enum(data[3]));

        //        return neerEnemy;
        //    }

        //    return null;
        //}

        //private GameObjectID stringToGameObjectID_Enum(string stringID)
        //{
        //    GameObjectID id;
        //    Debug.Assert(
        //        Enum.TryParse(stringID, false, out id) &&
        //        Enum.IsDefined(typeof(GameObjectID), id),
        //        "CSVデータのIDと列挙型の名前が合いません");

        //    return (GameObjectID)Enum.Parse(typeof(GameObjectID), stringID);
        //}
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickMove.CSV
{
    /// <summary>
    /// CSVファイル読み込みクラス
    /// </summary>
    class CSVReader
    {

        private List<string[]> stringData;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CSVReader()
        {
            stringData = new List<string[]>();
        }

        /// <summary>
        /// CSVファイルの読み込み
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="path"></param>
        public void Read(string filename, string path = "./")
        {
            //リストのクリア
            Clear();

            //例外処理
            try
            {
                //csvファイルを開く
                using (var sr = new System.IO.StreamReader(@"Content/" + path + filename))
                {
                    //ストリームの末尾まで繰り返す
                    while (!sr.EndOfStream)
                    {
                        //1行読み込む
                        var line = sr.ReadLine();
                        //カンマごとに分けて配列に格納する
                        var values = line.Split(',');//文字のカンマ

                        //リストに読み込んだ1行を追加
                        stringData.Add(values);
#if DEBUG
                        //出力する
                        foreach (var v in values)
                        {
                            System.Console.Write("{0}", v);
                        }
                        System.Console.WriteLine();
#endif
                    }

                }
            }
            catch (System.Exception e)
            {
                //ファイルオープンが失敗したとき
                System.Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// リスト内をクリア
        /// </summary>
        public void Clear()
        {
            stringData.Clear();
        }

        /// <summary>
        /// stringDataを取得するメソッド
        /// </summary>
        /// <returns></returns>
        public List<string[]> GetData()
        {
            return stringData;
        }

        /// <summary>
        /// stringの2次元配列を返すメソッド
        /// </summary>
        /// <returns></returns>
        public string[][] GetArrayData()
        {
            return stringData.ToArray();
        }

        /// <summary>
        /// int型のジャグ配列として取得
        /// </summary>
        /// <returns></returns>
        public int[][] GetIntData()
        {
            //文字の2次元配列の取得
            var data = GetArrayData();

            int row = data.Count();//行の数の取得

            int[][] intData = new int[row][];

            for (int i = 0; i < row; i++)
            {
                int col = data[i].Count();//列の数の取得
                intData[i] = new int[col];//列の実体生成
            }

            //整数に変換し、コピーする
            for (int y = 0; y < row; y++)
            {
                for (int x = 0; x < intData[y].Count(); x++)
                {
                    intData[y][x] = int.Parse(data[y][x]);
                }
            }

            return intData;
        }

        /// <summary>
        /// string型の多次元配列として取得
        /// </summary>
        /// <returns></returns>
        public string[,] GetStringMatrix()
        {
            var data = GetArrayData();
            int row = data.Count();//行の取得
            int col = data[0].Count();//列の数がどの行でも同じとし、数を取得

            string[,] result = new string[row, col];//多次元配列を生成

            for (int y = 0; y < row; y++)
            {
                for (int x = 0; x < col; x++)
                {
                    result[y, x] = data[y][x];
                }
            }

            return result;
        }

        /// <summary>
        /// int型の多次元配列として取得
        /// </summary>
        /// <returns></returns>
        public int[,] GetIntMatrix()
        {
            var data = GetIntData();
            int row = data.Count();
            int col = data[0].Count();

            int[,] result = new int[row, col];//多次元配列を生成

            for (int y = 0; y < row; y++)
            {
                for (int x = 0; x < col; x++)
                {
                    result[y, x] = data[y][x];
                }
            }
            return result;
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickMove
{
    class EnemyLevel1
    {
        public Camp baseCamp;

        private float enemyMoveTime; //一マス当たりの移動時間の計算用小数
        private float moveTimeSet;//移動する時間は何秒？

        private Vector2 enemySpawnPos; //スポーン位置
        public Vector2 enemyMovePos; //移動量
        public Vector2 enemyHeadPos; //向かう場所
        private Vector2 enemyLimit; //移動量の限界値
        public Vector2 enemyMasu; //マスの位置
        public Vector2 enemyPos; //ポジション
        public Direction direction;
        private readonly int UIWidth = 300;

        public int eatTime = 3 * 60;

        public readonly int TextureSize = 50;
        public List<Wall> walls;
        public List<Player> players;
        public List<Unchi> unchis;
        public int targetPlayerNom;
        public int colWallNum;

        int stuff;//満腹いくつ？
        public bool stuffMAXFlag = false; //満足して帰ったらtrue

        public float spawnTime;

        bool neerGlassEaterFlag = false;
        bool glassEatTargetFlag = false;
        public bool moveEndFlag = false;

        public bool avoidFlag = true;
        int nowCnt;

        float collisionCoolTime = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction">向かう方向</param>
        /// <param name="camp">キャンプ、拠点</param>
        /// <param name="players">草食獣のリスト</param>
        /// <param name="walls">壁のリスト</param>
        /// <param name="spawnTimeSet">どのタイミングで出てくるかの設定</param>
        public EnemyLevel1(Direction direction, Camp camp, List<Player> players, List<Wall> walls, float spawnTimeSet, List<Unchi> unchis)
        {
            this.direction = direction;
            baseCamp = camp;
            this.players = players;
            this.walls = walls;
            this.unchis = unchis;
            spawnTime = spawnTimeSet;
        }


        public void Initialze()
        {
            stuff = 1;
            neerGlassEaterFlag = false;
            glassEatTargetFlag = false;
            switch (direction)
            {
                case Direction.LEFT:
                    enemySpawnPos = new Vector2((int)(Screen.ScreenWidth / TextureSize) * TextureSize, (int)((Screen.ScreenHeight / 2) / TextureSize) * TextureSize);
                    break;
                case Direction.RIGHT:
                    enemySpawnPos = new Vector2((int)(UIWidth / TextureSize) * TextureSize - TextureSize, (int)((Screen.ScreenHeight / 2) / TextureSize) * TextureSize);
                    break;
                case Direction.BOTTOM:
                    enemySpawnPos = new Vector2((int)(((Screen.ScreenWidth - UIWidth) / 2 + UIWidth) / TextureSize) * TextureSize, -TextureSize);
                    break;
                case Direction.TOP:
                    enemySpawnPos = new Vector2((int)(((Screen.ScreenWidth - UIWidth) / 2 + UIWidth) / TextureSize) * TextureSize, Screen.ScreenHeight);
                    break;
                default:
                    break;
            }
            enemyMovePos = enemySpawnPos;
            enemyPos = enemySpawnPos;

            moveTimeSet = 1 * walls.Count;
        }

        public void Update()
        {
            if (spawnTime > 0)//スポーンしないとき
            {
                spawnTime -= 1.0f / 60.0f;
                return;
            }

            if (avoidFlag)
            {
                nowCnt = 0;
            }

            if (collisionCoolTime <= 0)
            {
                collisionCoolTime--;
            }

            if (stuff <= 0 && eatTime > 0)//満腹で食べきってないとき
            {
                eatTime -= 1;
            }
            else if (eatTime == 0)
            {
                unchis.Add(new Unchi(enemyPos));
                eatTime--;
            }
            else if (stuff <= 0 && eatTime <= 0)
            {
                MoveToSpawn();
            }

            //if (moveEndFlag)
            //{
            //    //ここに草食獣のストックを減らす処理
            //   // Player.stock--;???
            //}
            NeerGlassEater();//近くに草食動物がいるかどうか


            foreach (var wall in walls)//壁
            {
                if (eatTime <= 0 || stuff <= 0)
                {
                    break;
                }
                if (!Collision.WallXEnemy(wall, this) && avoidFlag /*&&*/)
                {
                    if (stuff > 0 && !moveEndFlag)//壁に当たってないとき//満腹でなくて移動が終わってないとき
                    {
                        if (!neerGlassEaterFlag)//いないとき
                        {
                            MoveToCamp();//真ん中に向かう
                        }
                        else
                        {
                            MoveToGE();
                        }
                    }

                    nowCnt++;
                }
                else if (neerGlassEaterFlag)
                {
                    MoveToGE();
                }
                else//当たっているとき
                {
                    colWallNum = nowCnt;
                    WallAvoid();
                }

            }

            Console.WriteLine(neerGlassEaterFlag);

            enemyMasu = new Vector2(enemyPos.X / TextureSize, enemyPos.Y / TextureSize);
        }

        public void WallAvoid()
        {
            if (collisionCoolTime <= 0)
            {
                float Down;
                float Right;
                //walls[colWallNum]; 当たってる壁
                if (avoidFlag)
                {
                    direction = Collision.WallXEnemyDirection(walls[colWallNum], this);
                    avoidFlag = false;
                }

                Vector2 moveWall;

                Vector2 center = new Vector2(enemyPos.X + TextureSize / 2, enemyPos.Y + TextureSize / 2);
                center.X = center.X - walls[colWallNum].position.X;
                center.Y = center.Y - walls[colWallNum].position.Y;

                enemyMoveTime = 50 / moveTimeSet / 60.0f;
                switch (direction)
                {
                    case Direction.RIGHT:
                        Down = walls[colWallNum].rectangle.Height - center.Y;
                        if (Down/*下側の距離*/ < center.Y && Down > 0)//下の方が距離が短いとき
                        {
                            enemyMovePos += new Vector2(0, enemyMoveTime);
                        }
                        else if (Down >= center.Y && 0 < center.Y)
                        {
                            enemyMovePos += new Vector2(0, -enemyMoveTime);
                        }

                        moveWall = new Vector2(walls[colWallNum].rectangle.Width, 0);
                        if (Down < 0 || center.Y < 0)
                        {
                            enemyMovePos += new Vector2(enemyMoveTime, 0);
                        }

                        if (enemyMovePos.X - walls[colWallNum].position.X > moveWall.X)
                        {
                            avoidFlag = true;
                            collisionCoolTime = 10;
                        }
                        enemyPos = new Vector2((int)(enemyMovePos.X / TextureSize) * TextureSize,
                       (int)(enemyMovePos.Y / TextureSize) * TextureSize);
                        break;

                    case Direction.LEFT:
                        Down = walls[colWallNum].rectangle.Height - center.Y;
                        if (Down/*下側の距離*/ < center.Y && Down > 0)
                        {
                            enemyMovePos += new Vector2(0, enemyMoveTime);
                        }
                        else if (Down >= center.Y && 0 < center.Y)
                        {
                            enemyMovePos += new Vector2(0, -enemyMoveTime);
                        }

                        moveWall = new Vector2(-walls[colWallNum].rectangle.Width, 0);
                        if (Down < 0 || center.Y < 0)
                        {
                            enemyMovePos += new Vector2(-enemyMoveTime, 0);
                        }

                        if (enemyPos.X - walls[colWallNum].position.X < moveWall.X)
                        {
                            avoidFlag = true;
                            collisionCoolTime = 10;
                        }
                        enemyPos = new Vector2((int)(enemyMovePos.X / TextureSize) * TextureSize,
                       (int)(enemyMovePos.Y / TextureSize) * TextureSize);
                        break;

                    case Direction.TOP:
                        Right = walls[colWallNum].rectangle.Width - center.X;
                        if (Right/*右側の距離*/ < center.X/*左側の距離*/ && Right > 0)
                        {
                            enemyMovePos += new Vector2(enemyMoveTime, 0);
                        }
                        else if (Right >= center.X && 0 < center.X)
                        {
                            enemyMovePos += new Vector2(-enemyMoveTime, 0);
                        }

                        moveWall = new Vector2(0, -walls[colWallNum].rectangle.Height);

                        if (Right < 0 || center.X < 0)
                        {
                            enemyMovePos += new Vector2(0, -enemyMoveTime);
                        }

                        if (enemyPos.Y - walls[colWallNum].position.Y < moveWall.Y)
                        {
                            avoidFlag = true;
                            collisionCoolTime = 10;
                        }
                        enemyPos = new Vector2((int)(enemyMovePos.X / TextureSize) * TextureSize,
                       (int)(enemyMovePos.Y / TextureSize) * TextureSize);
                        break;

                    case Direction.BOTTOM:

                        Right = walls[colWallNum].rectangle.Width - center.X;
                        if (Right/*右側の距離*/ < center.X/*左側の距離*/ && Right > 0)
                        {
                            enemyMovePos += new Vector2(enemyMoveTime, 0);
                        }
                        else if (Right >= center.X && 0 < center.X)
                        {
                            enemyMovePos += new Vector2(-enemyMoveTime, 0);
                        }

                        moveWall = new Vector2(0, walls[colWallNum].rectangle.Height);

                        if (Right < 0 || center.X < 0)
                        {
                            enemyMovePos += new Vector2(0, enemyMoveTime);
                        }

                        if (enemyPos.Y - walls[colWallNum].position.Y > moveWall.Y)
                        {
                            avoidFlag = true;
                            collisionCoolTime = 10;
                        }
                        enemyPos = new Vector2((int)(enemyMovePos.X / TextureSize) * TextureSize,
                       (int)(enemyMovePos.Y / TextureSize) * TextureSize);
                        break;

                    case Direction.NULL:
                        break;
                    default:
                        break;
                }
            }
        }

        public void NeerGlassEater()
        {
            int nowCount = 0;
            if (players != null)
            {
                foreach (var ge in players)
                {
                    if (Math.Abs(Vector2.Distance(enemyPos, ge.rendPos)) <= 4 * TextureSize && !ge.isDeadFlag)
                    {
                        neerGlassEaterFlag = true;
                        enemyHeadPos = ge.rendPos;
                        targetPlayerNom = nowCount;
                        glassEatTargetFlag = true;
                    }
                    else if(!glassEatTargetFlag || ge.isDeadFlag)
                    {
                        neerGlassEaterFlag = false;
                    }
                    nowCount++;
                }
            }
        }

        public void Draw(Renderer renderer)
        {
            renderer.DrawTexture("wolf", enemyPos);
        }



        public void MoveToCamp()
        {
            //拠点移動
            enemyHeadPos = new Vector2((int)((baseCamp.campPos.X + TextureSize) / TextureSize)/*何マス目か*/ * TextureSize,
                (int)((baseCamp.campPos.Y + TextureSize) / TextureSize) * TextureSize);
            enemyLimit = new Vector2((int)(enemyHeadPos.X - enemyPos.X) / TextureSize,
                (int)(enemyHeadPos.Y - enemyPos.Y) / TextureSize);

            //時間 ＝ フレーム　一マス辺りの時間　移動マス
            if (Math.Abs(enemyLimit.X) < Math.Abs(enemyLimit.Y)) enemyMoveTime = 60 * moveTimeSet * Math.Abs(enemyLimit.Y);
            else enemyMoveTime = 60 * moveTimeSet * Math.Abs(enemyLimit.X);

            if (enemyPos != enemyHeadPos)
            {
                if (Math.Abs(enemyLimit.X) > Math.Abs((enemyMovePos.X / TextureSize) - (enemyPos.X / TextureSize)))
                {
                    enemyMovePos += new Vector2((enemyHeadPos.X - enemyPos.X)/*何マス離れてるか*/ / (enemyMoveTime/*60f×秒数*/), 0);
                }
                if (Math.Abs(enemyLimit.Y) > Math.Abs((enemyMovePos.Y / TextureSize) - (enemyPos.Y / TextureSize)))
                {
                    enemyMovePos += new Vector2(0, (enemyHeadPos.Y - enemyPos.Y) / (enemyMoveTime/*×秒数*/));
                }

                enemyPos = new Vector2((int)(enemyMovePos.X / TextureSize) * TextureSize,
                    (int)(enemyMovePos.Y / TextureSize) * TextureSize);
            }
            else
            {
                moveEndFlag = true;
            }
        }

        public void MoveToGE()
        {
            enemyLimit = new Vector2((int)(enemyHeadPos.X - enemyPos.X) / TextureSize,
                   (int)(enemyHeadPos.Y - enemyPos.Y) / TextureSize);

            //時間 ＝ フレーム　一マス辺りの時間　移動マス
            if (Math.Abs(enemyLimit.X) < Math.Abs(enemyLimit.Y)) enemyMoveTime = 60 * 2 * Math.Abs(enemyLimit.Y);
            else enemyMoveTime = 60 * 2 * Math.Abs(enemyLimit.X);

            if (enemyPos != enemyHeadPos)
            {
                if (Math.Abs(enemyLimit.X) >= Math.Abs((enemyMovePos.X / TextureSize) - (enemyPos.X / TextureSize)))
                {
                    enemyMovePos += new Vector2((enemyHeadPos.X - enemyPos.X)/*何マス離れてるか*/ / (enemyMoveTime/*60f×秒数*/), 0);
                }
                if (Math.Abs(enemyLimit.Y) >= Math.Abs((enemyMovePos.Y / TextureSize) - (enemyPos.Y / TextureSize)))
                {
                    enemyMovePos += new Vector2(0, (enemyHeadPos.Y - enemyPos.Y) / (enemyMoveTime/*×秒数*/));
                }

                enemyPos = new Vector2((int)(enemyMovePos.X / TextureSize) * TextureSize,
                    (int)(enemyMovePos.Y / TextureSize) * TextureSize);
            }
            else
            {
                if (players != null)
                {
                    players[targetPlayerNom].isDeadFlag = true;
                    stuff -= players[targetPlayerNom].stuff;
                }
            }
        }

        public void MoveToSpawn()
        {
            enemyHeadPos = enemySpawnPos;
            enemyLimit = new Vector2((int)(enemyHeadPos.X - enemyPos.X) / TextureSize,
                  (int)(enemyHeadPos.Y - enemyPos.Y) / TextureSize);

            //時間 ＝ フレーム　一マス辺りの時間　移動マス
            if (Math.Abs(enemyLimit.X) < Math.Abs(enemyLimit.Y)) enemyMoveTime = 60 * Math.Abs(enemyLimit.Y);
            else enemyMoveTime = 60 * Math.Abs(enemyLimit.X);

            if (enemyPos != enemyHeadPos)
            {
                if (Math.Abs(enemyLimit.X) > Math.Abs((enemyMovePos.X / TextureSize) - (enemyPos.X / TextureSize)))
                {
                    enemyMovePos += new Vector2((enemyHeadPos.X - enemyPos.X)/*何マス離れてるか*/ / (enemyMoveTime/*60f×秒数*/), 0);
                }
                if (Math.Abs(enemyLimit.Y) > Math.Abs((enemyMovePos.Y / TextureSize) - (enemyPos.Y / TextureSize)))
                {
                    enemyMovePos += new Vector2(0, (enemyHeadPos.Y - enemyPos.Y) / (enemyMoveTime/*×秒数*/));
                }

                enemyPos = new Vector2((int)(enemyMovePos.X / TextureSize) * TextureSize,
                    (int)(enemyMovePos.Y / TextureSize) * TextureSize);
            }
            else
            {
                stuffMAXFlag = true;
            }
        }
    }
}

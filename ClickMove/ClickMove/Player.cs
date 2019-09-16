using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickMove
{
    class Player
    {

        Vector2 mouseClickPosition;

        Vector2 mousePosition;
        Vector2 clickPosition;
        Vector2 movePosition;
        public Vector2 rendPos;
        Vector2 limit;
        public Vector2 plMasu;

        bool clickFlag = false;
        public bool isDeadFlag = false;

        float time;

        public int stuff;//肉食が食べた時にたまる満腹度

        readonly int TextureSize = 50;

        public Player()
        {
            stuff = 1;
        }


        public void Update()
        {
            if (Input.IsMouseLButtonDown())
            {
                mouseClickPosition = new Vector2((int)(Input.MousePosition.X / TextureSize)/*何マス目か*/ * TextureSize,
                    (int)(Input.MousePosition.Y / TextureSize) * TextureSize);

                if (rendPos == mouseClickPosition || clickFlag)
                {
                    if (!clickFlag)
                    {
                        clickPosition = new Vector2((int)(Input.MousePosition.X / TextureSize)/*何マス目か*/ * TextureSize,
                            (int)(Input.MousePosition.Y / TextureSize) * TextureSize);
                        movePosition = clickPosition;
                        clickFlag = true;
                    }
                    else
                    {
                        mousePosition = new Vector2((int)(Input.MousePosition.X / TextureSize)/*何マス目か*/ * TextureSize,
                            (int)(Input.MousePosition.Y / TextureSize) * TextureSize);
                        clickFlag = false;
                        limit = new Vector2((int)(mousePosition.X - clickPosition.X) / TextureSize,
                            (int)(mousePosition.Y - clickPosition.Y) / TextureSize);
                        if (limit.X < 0) limit.X++;
                        if (limit.Y < 0) limit.Y++;

                        //時間 ＝ フレーム　一マス辺りの時間　移動マス
                        if (Math.Abs(limit.X) < Math.Abs(limit.Y)) time = 60 / 10 * Math.Abs(limit.Y);
                        else time = 60 / 10 * Math.Abs(limit.X);
                    }
                }
            }

            if (rendPos != mousePosition && !clickFlag)
            {
                if (Math.Abs(limit.X) >= Math.Abs((movePosition.X / TextureSize) - (clickPosition.X / TextureSize)))
                {
                    movePosition += new Vector2((mousePosition.X - clickPosition.X)/*何マス離れてるか*/ / (time/*60f×秒数*/), 0);
                }
                if (Math.Abs(limit.Y) >= Math.Abs((movePosition.Y / TextureSize) - (clickPosition.Y / TextureSize)))
                {
                    movePosition += new Vector2(0, (mousePosition.Y - clickPosition.Y) / (time/*×秒数*/));
                }

                rendPos = new Vector2((int)(movePosition.X / TextureSize) * TextureSize,
                    (int)(movePosition.Y / TextureSize) * TextureSize);
            }

            plMasu = new Vector2((int)rendPos.X / TextureSize, (int)rendPos.Y / TextureSize);
        }

        public void Draw(Renderer renderer)
        {
            renderer.DrawTexture("backpi", mousePosition);
            if (!isDeadFlag)
            {
                renderer.DrawTexture("chicken", rendPos);
            }
        }
    }
}

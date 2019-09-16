using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickMove
{
    class Unchi
    {
        Vector2 position;
        float deleteTime;

        public bool iswwwFlag = false;

        public Unchi(Vector2 pos)
        {
            position = pos;
            deleteTime = 3;
        }

        public void Update()
        {
            if (deleteTime > 0)
            {
                deleteTime -= 1.0f / 60.0f;
            }
            else
            {
                //草生える処理

                iswwwFlag = true;
            }
        }

        public void Draw(Renderer renderer)
        {
            renderer.DrawTexture("unchi", position);
        }
    }
}

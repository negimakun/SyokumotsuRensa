using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickMove
{
    class Camp
    {
        public Vector2 campPos;

        public Camp()
        {
            campPos = new Vector2(800, 400);
        }

        public void Draw(Renderer renderer)
        {
            renderer.DrawTexture("house", campPos);
        }
    }
}

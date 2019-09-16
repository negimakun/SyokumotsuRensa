using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickMove
{
    class Wall
    {
        public Rectangle rectangle;

        public Vector2 position;
        public Vector2 scaleSize;

        int masu;

        public readonly int TextureSize = 50;
        Direction direction;

        public Wall(Vector2 pos,Rectangle rect)
        {
            position = pos;
            rectangle = rect;
        }

        public void Initialize()
        {
            if (rectangle.Width < rectangle.Height)
            {
                direction = Direction.RIGHT;
                masu = rectangle.Width / TextureSize;
            }
            else
            {
                direction = Direction.BOTTOM;
                masu = rectangle.Height / TextureSize;
            }
        }

        public void Update()
        {

        }

        public void Draw(Renderer renderer)
        {
            for (int i = 0; i < masu; i++)
            {
                //if (direction == Direction.RIGHT)
                //{
                //    renderer.DrawTexture("waku", new Vector2(i * position.X,position.Y));
                //}
                //else if(direction == Direction.BOTTOM)
                //{
                //    renderer.DrawTexture("waku", new Vector2(position.X, i * position.Y));
                //}

                renderer.DrawTexture("waku", position, rectangle);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using Microsoft.Xna.Framework;

namespace CollisionExample.Collisions
{
    /// <summary>
    /// A struct representing rectangluar bounds
    /// </summary>
    public struct BoundingRectangle
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public float Left => X;
        public float Right => X+Width;

        public float Top => Y;
        public float Bottom => Y+Height;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public BoundingRectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public BoundingRectangle(Vector2 pos, float width, float height)
        {
            X = pos.X;
            Y = pos.Y;
            Width = width;
            Height = height;
        }
        /// <summary>
        /// tests for collison between boundingrectangles
        /// </summary>
        /// <param name="other">other bounding rectangles testing collide with</param>
        /// <returns>true if collision, false if not</returns>
        public bool CollidesWith(BoundingRectangle other)
        {
            return CollisionHelper.Collides(this, other);
        }

        /// <summary>
        /// tets collsion between circle and rectangle
        /// </summary>
        /// <param name="other">circle</param>
        /// <returns></returns>
        public bool CollidesWith(BoundingCircle other)
        {
            return CollisionHelper.Collides(other, this);
        }
    }
}
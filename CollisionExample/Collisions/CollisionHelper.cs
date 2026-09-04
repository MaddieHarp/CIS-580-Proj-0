using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace CollisionExample.Collisions
{
    /// <summary>
    /// A struct representing circular bounds
    /// </summary>
    public static class CollisionHelper
    {
        /// <summary>
        /// detects collision between two bounding circles
        /// </summary>
        /// <param name="a">first bounding circle</param>
        /// <param name="b">second bounding circle</param>
        /// <returns>true for collision false if not</returns>
        public static bool Collides(BoundingCircle a, BoundingCircle b)
        {
            return Math.Pow(a.Radius + b.Radius, 2) >=
                Math.Pow(a.Center.X - b.Center.X, 2) + 
                Math.Pow(a.Center.Y - b.Center.Y, 2);
            
        }
        /// <summary>
        /// detects between two bvoundingrectangles
        /// </summary>
        /// <param name="a">rect 1</param>
        /// <param name="b">rect 2</param>
        /// <returns></returns>
        public static bool Collides(BoundingRectangle a, BoundingRectangle b)
        {
            return !(a.Right < b.Left || a.Left > b.Right 
            || a.Top > b.Bottom || a.Bottom < b.Top);
            
        }
        /// <summary>
        /// detects collisions between rectangle and a circle
        /// </summary>
        /// <param name="c">circle</param>
        /// <param name="r">rectangle</param>
        /// <returns>true if collide false if not</returns>
        public static bool Collides(BoundingCircle c, BoundingRectangle r)
        {
            float nearestX = MathHelper.Clamp(c.Center.X, r.Left, r.Right);
            float nearestY = MathHelper.Clamp(c.Center.Y, r.Top, r.Bottom);
            return Math.Pow(c.Radius, 2) >= Math.Pow(c.Center.X - nearestX, 2) + Math.Pow(c.Center.Y - nearestY, 2);

        }

        /// <summary>
        /// detects collisions between rectangle and a circle
        /// </summary>
        /// <param name="c">circle</param>
        /// <param name="r">rectangle</param>
        /// <returns>true if collide false if not</returns>
        public static bool Collides(BoundingRectangle r, BoundingCircle c)
        {
            float nearestX = MathHelper.Clamp(c.Center.X, r.Left, r.Right);
            float nearestY = MathHelper.Clamp(c.Center.Y, r.Top, r.Bottom);
            return Math.Pow(c.Radius, 2) >= Math.Pow(c.Center.X - nearestX, 2) + Math.Pow(c.Center.Y - nearestY, 2);

        }
    }
}
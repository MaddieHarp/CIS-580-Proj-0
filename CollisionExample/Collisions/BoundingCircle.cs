using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using Microsoft.Xna.Framework;

namespace CollisionExample.Collisions
{
    /// <summary>
    /// A struct representing circular bounds
    /// </summary>
    public struct BoundingCircle
    {
        /// <summary>
        /// center of the bounding circle
        /// </summary>
        public Vector2 Center;
        /// <summary>
        /// radius of the bounding circle
        /// </summary>
        public float Radius;

        /// <summary>
        /// constructs new bounding circle
        /// </summary>
        /// <param name="center">center</param>
        /// <param name="radius">radius</param>
        public BoundingCircle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }
        /// <summary>
        /// tests for collison between boundingcircles
        /// </summary>
        /// <param name="other">other bounding circle tewsting collide with</param>
        /// <returns>true if collision, false if not</returns>
        public bool CollidesWith(BoundingCircle other)
        {
            return CollisionHelper.Collides(this, other);
        }
        /// <summary>
        /// tets collsion between circle and rectangle
        /// </summary>
        /// <param name="other">rectangle</param>
        /// <returns></returns>
        public bool CollidesWith(BoundingRectangle other)
        {
            return CollisionHelper.Collides(this, other);
        }
    }
}
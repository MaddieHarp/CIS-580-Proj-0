using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionExample
{
    public class particleEffect : ParticleSystem
    {
        public particleEffect(Game game, int maxParticles) : base(game, maxParticles) 
        { 
        }

        protected override void InitializeConstants()
        {
            textureFilename = "particle"; 
            minNumParticles = 1000;
            maxNumParticles = 2000;
            
            blendState = BlendState.AlphaBlend; 
        }

        protected override void InitializeParticle(ref Particle p, Vector2 where)
        {
            Vector2 velocity = RandomHelper.NextDirection() * RandomHelper.NextFloat(50f, 250f); 
            Vector2 acceleration = new Vector2(0, 150f); 
            float lifetime = RandomHelper.NextFloat(0.5f, 1.5f); 
            float scale = RandomHelper.NextFloat(0.2f, 0.8f); 
            float rotation = RandomHelper.NextFloat(0, MathHelper.TwoPi); 
            float angularVelocity = RandomHelper.NextFloat(-2f, 2f);
            p.Initialize(where, velocity, acceleration, Color.Red, lifetime, scale, rotation, angularVelocity);
        }
        public void Explode(Vector2 where)
        {
            AddParticles(where); 
        }
    }
}
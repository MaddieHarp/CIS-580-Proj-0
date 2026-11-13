using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace JumpJump;

/// <summary>
/// A class representing a crate
/// </summary>
public class Crate 
{
    // The game this crate belongs to
    Game game;

    // The VertexBuffer of crate vertices
    VertexBuffer vertexBuffer;

    // The IndexBuffer defining the Crate's triangles
    IndexBuffer indexBuffer;

    // The effect to render the crate with
    BasicEffect effect;

    // The texture to apply to the crate
    Texture2D texture;

    public Vector3 Position { get; set; } = Vector3.Zero;

        /// <summary>
    /// Initializes the vertex of the cube
    /// </summary>
    public void InitializeVertices() 
    {
        var vertexData = new VertexPositionNormalTexture[] { 
            // Front Face
            new VertexPositionNormalTexture() { Position = new Vector3(-1.0f, -1.0f, -1.0f), TextureCoordinate = new Vector2(0.0f, 1.0f), Normal = Vector3.Forward },
            new VertexPositionNormalTexture() { Position = new Vector3(-1.0f,  1.0f, -1.0f), TextureCoordinate = new Vector2(0.0f, 0.0f), Normal = Vector3.Forward },
            new VertexPositionNormalTexture() { Position = new Vector3( 1.0f,  1.0f, -1.0f), TextureCoordinate = new Vector2(1.0f, 0.0f), Normal = Vector3.Forward },
            new VertexPositionNormalTexture() { Position = new Vector3( 1.0f, -1.0f, -1.0f), TextureCoordinate = new Vector2(1.0f, 1.0f), Normal = Vector3.Forward },

            // Back Face
            new VertexPositionNormalTexture() { Position = new Vector3(-1.0f, -1.0f, 1.0f), TextureCoordinate = new Vector2(1.0f, 1.0f), Normal = Vector3.Backward },
            new VertexPositionNormalTexture() { Position = new Vector3( 1.0f, -1.0f, 1.0f), TextureCoordinate = new Vector2(0.0f, 1.0f), Normal = Vector3.Forward },
            new VertexPositionNormalTexture() { Position = new Vector3( 1.0f,  1.0f, 1.0f), TextureCoordinate = new Vector2(0.0f, 0.0f), Normal = Vector3.Forward },
            new VertexPositionNormalTexture() { Position = new Vector3(-1.0f,  1.0f, 1.0f), TextureCoordinate = new Vector2(1.0f, 0.0f), Normal = Vector3.Forward },

            // Top Face
            new VertexPositionNormalTexture() { Position = new Vector3(-1.0f, 1.0f, -1.0f), TextureCoordinate = new Vector2(0.0f, 1.0f), Normal = Vector3.Up },
            new VertexPositionNormalTexture() { Position = new Vector3(-1.0f, 1.0f,  1.0f), TextureCoordinate = new Vector2(0.0f, 0.0f), Normal = Vector3.Up },
            new VertexPositionNormalTexture() { Position = new Vector3( 1.0f, 1.0f,  1.0f), TextureCoordinate = new Vector2(1.0f, 0.0f), Normal = Vector3.Up },
            new VertexPositionNormalTexture() { Position = new Vector3( 1.0f, 1.0f, -1.0f), TextureCoordinate = new Vector2(1.0f, 1.0f), Normal = Vector3.Up },

            // Bottom Face
            new VertexPositionNormalTexture() { Position = new Vector3(-1.0f, -1.0f, -1.0f), TextureCoordinate = new Vector2(1.0f, 1.0f), Normal = Vector3.Down },
            new VertexPositionNormalTexture() { Position = new Vector3( 1.0f, -1.0f, -1.0f), TextureCoordinate = new Vector2(0.0f, 1.0f), Normal = Vector3.Down },
            new VertexPositionNormalTexture() { Position = new Vector3( 1.0f, -1.0f,  1.0f), TextureCoordinate = new Vector2(0.0f, 0.0f), Normal = Vector3.Down },
            new VertexPositionNormalTexture() { Position = new Vector3(-1.0f, -1.0f,  1.0f), TextureCoordinate = new Vector2(1.0f, 0.0f), Normal = Vector3.Down },

            // Left Face
            new VertexPositionNormalTexture() { Position = new Vector3(-1.0f, -1.0f,  1.0f), TextureCoordinate = new Vector2(0.0f, 1.0f), Normal = Vector3.Left },
            new VertexPositionNormalTexture() { Position = new Vector3(-1.0f,  1.0f,  1.0f), TextureCoordinate = new Vector2(0.0f, 0.0f), Normal = Vector3.Left },
            new VertexPositionNormalTexture() { Position = new Vector3(-1.0f,  1.0f, -1.0f), TextureCoordinate = new Vector2(1.0f, 0.0f), Normal = Vector3.Left },
            new VertexPositionNormalTexture() { Position = new Vector3(-1.0f, -1.0f, -1.0f), TextureCoordinate = new Vector2(1.0f, 1.0f), Normal = Vector3.Left },

            // Right Face
            new VertexPositionNormalTexture() { Position = new Vector3( 1.0f, -1.0f, -1.0f), TextureCoordinate = new Vector2(0.0f, 1.0f), Normal = Vector3.Right },
            new VertexPositionNormalTexture() { Position = new Vector3( 1.0f,  1.0f, -1.0f), TextureCoordinate = new Vector2(0.0f, 0.0f), Normal = Vector3.Right },
            new VertexPositionNormalTexture() { Position = new Vector3( 1.0f,  1.0f,  1.0f), TextureCoordinate = new Vector2(1.0f, 0.0f), Normal = Vector3.Right },
            new VertexPositionNormalTexture() { Position = new Vector3( 1.0f, -1.0f,  1.0f), TextureCoordinate = new Vector2(1.0f, 1.0f), Normal = Vector3.Right },
        };
        vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionNormalTexture), vertexData.Length, BufferUsage.None);
        vertexBuffer.SetData<VertexPositionNormalTexture>(vertexData);
    }

        /// <summary>
    /// Initializes the Index Buffer
    /// </summary>
    public void InitializeIndices()
    {
        var indexData = new short[]
        {
            // Front face
            0, 2, 1,
            0, 3, 2,

            // Back face 
            4, 6, 5, 
            4, 7, 6,

            // Top face
            8, 10, 9,
            8, 11, 10,

            // Bottom face 
            12, 14, 13,
            12, 15, 14,

            // Left face 
            16, 18, 17,
            16, 19, 18,

            // Right face 
            20, 22, 21,
            20, 23, 22
        };
        indexBuffer = new IndexBuffer(game.GraphicsDevice, IndexElementSize.SixteenBits, indexData.Length, BufferUsage.None);
        indexBuffer.SetData<short>(indexData);
    }

        /// <summary>
    /// Initializes the BasicEffect to render our crate
    /// </summary>
    void InitializeEffect()
    {
        effect = new BasicEffect(game.GraphicsDevice);
        effect.World = Matrix.CreateScale(2.0f) 
             * Matrix.CreateTranslation(Position);
        effect.View = Matrix.CreateLookAt(
            new Vector3(8, 9, 12), // The camera position
            new Vector3(0, 0, 0), // The camera target,
            Vector3.Up            // The camera up vector
        );
        effect.Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4,                         // The field-of-view 
            game.GraphicsDevice.Viewport.AspectRatio,   // The aspect ratio
            0.1f, // The near plane distance 
            100.0f // The far plane distance
        );
        effect.EnableDefaultLighting();
        effect.TextureEnabled = true; 
        effect.Texture = texture;
    }

        /// <summary>
    /// Draws the crate
    /// </summary>
public void Draw()
{
    effect.World = Matrix.CreateScale(2.0f)
                 * Matrix.CreateTranslation(Position);

    effect.CurrentTechnique.Passes[0].Apply();

    game.GraphicsDevice.SetVertexBuffer(vertexBuffer);
    game.GraphicsDevice.Indices = indexBuffer;
    game.GraphicsDevice.DrawIndexedPrimitives(
        PrimitiveType.TriangleList,
        0,
        0,
        12
    );
}
        /// <summary>
    /// Creates a new crate instance
    /// </summary>
    /// <param name="game">The game this crate belongs to</param>
    /// <param name="type">The type of crate to use</param>
    public Crate(Game game, CrateType type)
    {
        this.game = game;
        this.texture = game.Content.Load<Texture2D>($"crate{(int)type}_diffuse");
        InitializeVertices();
        InitializeIndices();
        InitializeEffect();
    }


}


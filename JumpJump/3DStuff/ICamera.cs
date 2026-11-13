using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JumpJump;
/// <summary>
/// An interface defining a camera
/// </summary>
public interface ICamera
{
    /// <summary>
    /// The view matrix
    /// </summary>
    Matrix View { get; }

    /// <summary>
    /// The projection matrix
    /// </summary>
    Matrix Projection { get; }
}
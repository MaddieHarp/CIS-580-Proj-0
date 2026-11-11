namespace JumpJump;

/// <summary>
/// Implemented by screens that can write the player's progress to the save file.
/// </summary>
public interface ISaveableScreen
{
    void SaveGame();
}
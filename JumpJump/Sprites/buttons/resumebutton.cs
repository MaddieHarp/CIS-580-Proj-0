using System;
using Microsoft.Xna.Framework;

namespace JumpJump
{
    public class ResumeButton : titlebutton
    {
        private readonly Action onClick;

        public ResumeButton(string assetName, Vector2 position, Action onClick, float scale = 1f)
            : base(assetName, position, scale)
        {
            this.onClick = onClick;
        }

        protected override void OnClick() => onClick();
    }
}
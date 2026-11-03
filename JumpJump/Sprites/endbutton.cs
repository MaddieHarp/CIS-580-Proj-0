using System;
using Microsoft.Xna.Framework;

namespace JumpJump
{
    public class EndButton : titlebutton
    {
        private readonly Action onClick;

        public EndButton(string assetName, Vector2 position, Action onClick, float scale = 1f)
            : base(assetName, position, scale)
        {
            this.onClick = onClick;
        }

        protected override void OnClick() => onClick();
    }
}
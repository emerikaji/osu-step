using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace osu.Game.Rulesets.Step.Skinning.Argon
{
    internal partial class ArgonMineNotePiece : ArgonNotePiece
    {
        protected override bool ShowBottomHighlight => false;

        protected override Drawable CreateIcon() => new SpriteIcon
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Y = 3,
            Icon = FontAwesome.Solid.Times,
            Size = new Vector2(18),
        };
    }
}

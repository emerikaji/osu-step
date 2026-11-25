using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Step.Skinning.Default
{
    internal partial class DefaultMineNotePiece : CompositeDrawable
    {
        public const float NOTE_HEIGHT = 12;

        public DefaultMineNotePiece()
        {
            RelativeSizeAxes = Axes.X;
            Height = NOTE_HEIGHT;

            CornerRadius = 5;
            Masking = true;

            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = new Color4(255, 50, 50, 120),
                Radius = 8,
            };

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.DarkRed,
                },
                new Box
                {
                    RelativeSizeAxes = Axes.X,
                    Height = NOTE_HEIGHT / 2,
                    Alpha = 0.1f,
                    Colour = Color4.DarkRed,
                },
                new SpriteIcon
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = FontAwesome.Solid.Times,
                    Size = new Vector2(NOTE_HEIGHT - 2),
                    Colour = Color4.White,
                }
            };
        }
    }
}

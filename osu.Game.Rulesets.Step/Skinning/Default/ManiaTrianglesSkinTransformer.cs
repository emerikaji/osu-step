// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Step.Beatmaps;
using osu.Game.Skinning;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Step.Skinning.Default
{
    public class ManiaTrianglesSkinTransformer : SkinTransformer
    {
        private readonly StepBeatmap beatmap;

        public ManiaTrianglesSkinTransformer(ISkin skin, IBeatmap beatmap)
            : base(skin)
        {
            this.beatmap = (StepBeatmap)beatmap;
        }

        private static readonly Color4 colour_even = new(6, 84, 0, 255);
        private static readonly Color4 colour_odd = new(94, 0, 57, 255);
        private static readonly Color4 colour_special = new(0, 48, 63, 255);

        private static Color4 getTrianglesSnapColour(int divisor) => divisor switch
        {
            1 => Color4.Crimson,
            2 => colour_special,
            3 => Color4.Purple,
            4 => Color4.Gold,
            6 => colour_odd,
            8 => Color4.DarkOrange,
            12 or 16 => Color4.Teal,
            _ => colour_even,
        };

        public override IBindable<TValue>? GetConfig<TLookup, TValue>(TLookup lookup)
        {
            if (lookup is BeatSnapColourLookup snapLookup)
                return SkinUtils.As<TValue>(new Bindable<Color4>(getTrianglesSnapColour(snapLookup.Divisor)));

            if (lookup is ManiaSkinConfigurationLookup maniaLookup)
            {
                switch (maniaLookup.Lookup)
                {
                    case LegacyManiaSkinConfigurationLookups.ColumnBackgroundColour:
                        return SkinUtils.As<TValue>(new Bindable<Color4>(Color4.Gray));
                }
            }

            return base.GetConfig<TLookup, TValue>(lookup);
        }
    }
}

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Testing;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Step.Beatmaps;
using osu.Game.Screens.Play.HUD;
using osu.Game.Screens.Play.HUD.HitErrorMeters;
using osu.Game.Skinning;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Step.Skinning.Argon
{
    public class ManiaArgonSkinTransformer : SkinTransformer
    {
        private readonly ManiaBeatmap beatmap;

        public ManiaArgonSkinTransformer(ISkin skin, IBeatmap beatmap)
            : base(skin)
        {
            this.beatmap = (ManiaBeatmap)beatmap;
        }

        public override Drawable? GetDrawableComponent(ISkinComponentLookup lookup)
        {
            switch (lookup)
            {
                case GlobalSkinnableContainerLookup containerLookup:
                    // Only handle per ruleset defaults here.
                    if (containerLookup.Ruleset == null)
                        return base.GetDrawableComponent(lookup);

                    switch (containerLookup.Lookup)
                    {
                        case GlobalSkinnableContainers.MainHUDComponents:
                            return new DefaultSkinComponentsContainer(container =>
                            {
                                var leaderboard = container.OfType<DrawableGameplayLeaderboard>().FirstOrDefault();
                                var combo = container.ChildrenOfType<ArgonManiaComboCounter>().FirstOrDefault();
                                var spectatorList = container.OfType<SpectatorList>().FirstOrDefault();
                                var hitError = container.OfType<HitErrorMeter>().FirstOrDefault();
                                var hitError2 = container.OfType<HitErrorMeter>().LastOrDefault();

                                leaderboard?.Position = new Vector2(36, 115);

                                if (combo != null)
                                {
                                    combo.ShowLabel.Value = false;
                                    combo.Anchor = Anchor.TopCentre;
                                    combo.Origin = Anchor.Centre;
                                    combo.Y = 200;
                                }

                                spectatorList?.Position = new Vector2(36, -66);

                                if (hitError != null)
                                {
                                    hitError.Anchor = Anchor.CentreLeft;
                                    hitError.Origin = Anchor.CentreLeft;
                                }

                                if (hitError2 != null)
                                {
                                    hitError2.Anchor = Anchor.CentreRight;
                                    hitError2.Scale = new Vector2(-1, 1);
                                    // origin flipped to match scale above.
                                    hitError2.Origin = Anchor.CentreLeft;
                                }

                                foreach (var d in container.OfType<ISerialisableDrawable>())
                                    d.UsesFixedAnchor = true;
                            })
                            {
                                new DrawableGameplayLeaderboard(),
                                new ArgonManiaComboCounter(),
                                new SpectatorList
                                {
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft,
                                },
                                new BarHitErrorMeter(),
                                new BarHitErrorMeter(),
                            };
                    }

                    return null;

                case SkinComponentLookup<HitResult> resultComponent:
                    // This should eventually be moved to a skin setting, when supported.
                    if (Skin is ArgonProSkin && resultComponent.Component >= HitResult.Great)
                        return Drawable.Empty();

                    return new ArgonJudgementPiece(resultComponent.Component);

                case ManiaSkinComponentLookup maniaComponent:
                    // TODO: Once everything is finalised, consider throwing UnsupportedSkinComponentException on missing entries.
                    switch (maniaComponent.Component)
                    {
                        case ManiaSkinComponents.StageBackground:
                            return new ArgonStageBackground();

                        case ManiaSkinComponents.ColumnBackground:
                            return new ArgonColumnBackground();

                        case ManiaSkinComponents.HoldNoteBody:
                            return new ArgonHoldBodyPiece();

                        case ManiaSkinComponents.HoldNoteTail:
                            return new ArgonHoldNoteTailPiece();

                        case ManiaSkinComponents.HoldNoteHead:
                            return new ArgonHoldNoteHeadPiece();

                        case ManiaSkinComponents.Note:
                            return new ArgonNotePiece();

                        case ManiaSkinComponents.HitTarget:
                            return new ArgonHitTarget();

                        case ManiaSkinComponents.KeyArea:
                            return new ArgonKeyArea();

                        case ManiaSkinComponents.HitExplosion:
                            return new ArgonHitExplosion();
                    }

                    break;
            }

            return base.GetDrawableComponent(lookup);
        }

        private static readonly Color4 colour_special_column = new Color4(169, 106, 255, 255);
        private static readonly Color4 colour_yellow = new Color4(255, 197, 40, 255);
        private static readonly Color4 colour_orange = new Color4(252, 109, 1, 255);
        private static readonly Color4 colour_pink = new Color4(213, 35, 90, 255);
        private static readonly Color4 colour_purple = new Color4(203, 60, 236, 255);
        private static readonly Color4 colour_cyan = new Color4(72, 198, 255, 255);
        private static readonly Color4 colour_green = new Color4(100, 192, 92, 255);

        private static Color4 getArgonSnapColour(int divisor) => divisor switch
        {
            1 => colour_pink,
            2 => colour_cyan,
            3 => colour_purple,
            4 => colour_yellow,
            6 => colour_special_column,
            8 => colour_orange,
            _ => colour_green,
        };

        public override IBindable<TValue>? GetConfig<TLookup, TValue>(TLookup lookup)
        {
            if (lookup is BeatSnapColourLookup snapLookup)
                return SkinUtils.As<TValue>(new Bindable<Color4>(getArgonSnapColour(snapLookup.Divisor)));

            if (lookup is ManiaSkinConfigurationLookup maniaLookup)
            {
                int columnIndex = maniaLookup.ColumnIndex ?? 0;
                var stage = beatmap.GetStageForColumnIndex(columnIndex);

                switch (maniaLookup.Lookup)
                {
                    case LegacyManiaSkinConfigurationLookups.LeftColumnSpacing:
                    case LegacyManiaSkinConfigurationLookups.RightColumnSpacing:
                        return SkinUtils.As<TValue>(new Bindable<float>(1));

                    case LegacyManiaSkinConfigurationLookups.StagePaddingBottom:
                    case LegacyManiaSkinConfigurationLookups.StagePaddingTop:
                        return SkinUtils.As<TValue>(new Bindable<float>(30));

                    case LegacyManiaSkinConfigurationLookups.ColumnWidth:
                        bool isSpecialColumn = stage.IsSpecialColumn(columnIndex);

                        float width = 60 * (isSpecialColumn ? 2 : 1);

                        return SkinUtils.As<TValue>(new Bindable<float>(width));

                    case LegacyManiaSkinConfigurationLookups.ColumnBackgroundColour:
                        var colour = getColourForLayout(columnIndex, stage);

                        return SkinUtils.As<TValue>(new Bindable<Color4>(colour));
                }
            }

            return base.GetConfig<TLookup, TValue>(lookup);
        }

        private static Color4 getColourForLayout(int columnIndex, StageDefinition stage) => Color4.Gray;
    }
}

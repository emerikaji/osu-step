// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Testing;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Step.Scoring;
using osu.Game.Rulesets.Step.Skinning.Legacy;
using osu.Game.Rulesets.Step.UI;

namespace osu.Game.Rulesets.Step.Tests.Skinning
{
    public partial class TestSceneDrawableJudgement : ManiaSkinnableTestScene
    {
        public TestSceneDrawableJudgement()
        {
            var hitWindows = new ManiaHitWindows();

            foreach (HitResult result in Enum.GetValues(typeof(HitResult)).OfType<HitResult>().Skip(1))
            {
                if (hitWindows.IsHitResultAllowed(result))
                {
                    AddStep("Show " + result.GetDescription(), () =>
                    {
                        SetContents(_ =>
                        {
                            var drawableManiaJudgement = new DrawableManiaJudgement
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                            };

                            drawableManiaJudgement.Apply(new JudgementResult(new HitObject { StartTime = Time.Current }, new Judgement())
                            {
                                Type = result
                            }, null);

                            return drawableManiaJudgement;
                        });

                        // for test purposes, undo the Y adjustment related to the `ScorePosition` legacy positioning config value
                        // (see `LegacyManiaJudgementPiece.load()`).
                        // this prevents the judgements showing somewhere below or above the bounding box of the judgement.
                        foreach (var legacyPiece in this.ChildrenOfType<LegacyManiaJudgementPiece>())
                            legacyPiece.Y = 0;
                    });
                }
            }
        }
    }
}

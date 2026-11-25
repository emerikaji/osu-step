// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using NUnit.Framework;
using osu.Game.Rulesets.Step.Beatmaps;
using osu.Game.Rulesets.Step.Objects;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Step.Tests.Mods
{
    public partial class TestSceneManiaModAutoplay : ModTestScene
    {
        protected override Ruleset CreatePlayerRuleset() => new StepRuleset();

        [Test]
        public void TestPerfectScoreOnShortHoldNote()
        {
            CreateModTest(new ModTestData
            {
                Autoplay = true,
                CreateBeatmap = () => new StepBeatmap(new StageDefinition(1))
                {
                    HitObjects = new List<ManiaHitObject>
                    {
                        new HoldNote
                        {
                            StartTime = 100,
                            EndTime = 100,
                        },
                        new HoldNote
                        {
                            StartTime = 100.1,
                            EndTime = 150,
                        },
                    }
                },
                PassCondition = () => Player.ScoreProcessor.Combo.Value == 2
            });
        }
    }
}

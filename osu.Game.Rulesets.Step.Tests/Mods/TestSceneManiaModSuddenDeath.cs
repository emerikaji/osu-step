// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using NUnit.Framework;

namespace osu.Game.Rulesets.Step.Tests.Mods
{
    public partial class TestSceneManiaModSuddenDeath : ModFailConditionTestScene
    {
        protected override Ruleset CreatePlayerRuleset() => new StepRuleset();

        public TestSceneManiaModSuddenDeath()
            : base(new ManiaModSuddenDeath())
        {
        }

        [Test]
        public void TestGreatHit() => CreateModTest(new ModTestData
        {
            Mod = new ManiaModSuddenDeath(),
            PassCondition = () => ((ModFailConditionTestPlayer)Player).CheckFailed(false),
            Autoplay = false,
            CreateBeatmap = () => new Beatmap
            {
                HitObjects = new List<HitObject>
                {
                    new Note
                    {
                        StartTime = 1000,
                    }
                },
            },
            ReplayFrames = new List<ReplayFrame>
            {
                new ManiaReplayFrame(1020, ManiaAction.Key1),
                new ManiaReplayFrame(2000)
            }
        });

        [Test]
        public void TestBreakOnHoldNote() => CreateModTest(new ModTestData
        {
            Mod = new ManiaModSuddenDeath(),
            PassCondition = () => ((ModFailConditionTestPlayer)Player).CheckFailed(true) && Player.Results.Count == 2,
            Autoplay = false,
            CreateBeatmap = () => new Beatmap
            {
                HitObjects = new List<HitObject>
                {
                    new HoldNote
                    {
                        StartTime = 1000,
                        EndTime = 3000,
                    },
                },
            },
            ReplayFrames = new List<ReplayFrame>
            {
                new ManiaReplayFrame(1000, ManiaAction.Key1),
                new ManiaReplayFrame(2000)
            }
        });
    }
}

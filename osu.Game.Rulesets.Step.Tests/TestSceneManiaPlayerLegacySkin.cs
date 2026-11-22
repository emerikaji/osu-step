// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;

namespace osu.Game.Rulesets.Step.Tests
{
    public partial class TestSceneManiaPlayerLegacySkin : LegacySkinPlayerTestScene
    {
        protected override Ruleset CreatePlayerRuleset() => new StepRuleset();

        // play with a converted beatmap to allow dual stages mod to work.
        protected override IBeatmap CreateBeatmap(RulesetInfo ruleset) => new TestBeatmap(new RulesetInfo());

        protected override bool HasCustomSteps => true;

        [Test]
        public void TestSingleStage()
        {
            AddStep("Load single stage", LoadPlayer);
            AddUntilStep("player loaded", () => Player.IsLoaded && Player.Alpha == 1);
        }

        [Test]
        public void TestDualStage()
        {
            AddStep("Load dual stage", () => LoadPlayer(new Mod[] { new ManiaModDualStages() }));
            AddUntilStep("player loaded", () => Player.IsLoaded && Player.Alpha == 1);
        }
    }
}

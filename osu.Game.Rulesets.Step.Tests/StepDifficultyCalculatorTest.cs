// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Step.Difficulty;
using osu.Game.Rulesets.Step.Mods;
using osu.Game.Tests.Beatmaps;

namespace osu.Game.Rulesets.Step.Tests
{
    public class StepDifficultyCalculatorTest : DifficultyCalculatorTest
    {
        protected override string ResourceAssembly => "osu.Game.Rulesets.Step.Tests";

        [TestCase(2.3493769750220914d, 137, "diffcalc-test")]
        public void Test(double expectedStarRating, int expectedMaxCombo, string name)
            => base.Test(expectedStarRating, expectedMaxCombo, name);

        [TestCase(2.797245912537965d, 137, "diffcalc-test")]
        public void TestClockRateAdjusted(double expectedStarRating, int expectedMaxCombo, string name)
            => Test(expectedStarRating, expectedMaxCombo, name, new ManiaModDoubleTime());

        protected override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap) => new StepDifficultyCalculator(new StepRuleset().RulesetInfo, beatmap);

        protected override Ruleset CreateRuleset() => new StepRuleset();
    }
}

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Reflection;
using NUnit.Framework;

namespace osu.Game.Rulesets.Step.Tests
{
    public partial class TestSceneManiaHitObjectSamples : HitObjectSampleTest
    {
        protected override Ruleset CreatePlayerRuleset() => new StepRuleset();
        protected override IResourceStore<byte[]> RulesetResources => new DllResourceStore(Assembly.GetAssembly(typeof(TestSceneManiaHitObjectSamples)));

        /// <summary>
        /// Tests that when a normal sample bank is used, the normal hitsound will be looked up.
        /// </summary>
        [Test]
        public void TestManiaHitObjectNormalSampleBank()
        {
            const string expected_sample = "normal-hitnormal2";

            SetupSkins(expected_sample, expected_sample);

            CreateTestWithBeatmap("mania-hitobject-beatmap-normal-sample-bank.osu");

            AssertBeatmapLookup(expected_sample);
        }

        /// <summary>
        /// Tests that when a custom sample bank is used, layered hitsounds are not played
        /// (only the sample from the custom bank is looked up).
        /// </summary>
        [Test]
        public void TestManiaHitObjectCustomSampleBank()
        {
            const string expected_sample = "normal-hitwhistle2";
            const string unwanted_sample = "normal-hitnormal2";

            SetupSkins(expected_sample, unwanted_sample);

            CreateTestWithBeatmap("mania-hitobject-beatmap-custom-sample-bank.osu");

            AssertBeatmapLookup(expected_sample);
            AssertNoLookup(unwanted_sample);
        }

        [Test]
        public void TestConvertHitObjectCustomSampleBank()
        {
            const string beatmap_sample = "normal-hitwhistle2";
            const string user_skin_sample = "normal-hitnormal";

            SetupSkins(beatmap_sample, user_skin_sample);

            CreateTestWithBeatmap("convert-beatmap-custom-sample-bank.osu");

            AssertBeatmapLookup(beatmap_sample);
            AssertUserLookup(user_skin_sample);
        }
    }
}

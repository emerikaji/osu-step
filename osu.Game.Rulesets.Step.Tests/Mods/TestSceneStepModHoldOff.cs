// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Step.Beatmaps;
using osu.Game.Rulesets.Step.Mods;
using osu.Game.Rulesets.Step.Objects;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Step.Tests.Mods
{
    public partial class TestSceneStepModHoldOff : ModTestScene
    {
        protected override Ruleset CreatePlayerRuleset() => new StepRuleset();

        [Test]
        public void TestMapHasNoHoldNotes()
        {
            var testBeatmap = createModdedBeatmap();
            ClassicAssert.False(testBeatmap.HitObjects.OfType<HoldNote>().Any());
            ClassicAssert.False(testBeatmap.HitObjects.OfType<RollNote>().Any());
        }

        [Test]
        public void TestCorrectObjectCount()
        {
            // Ensure that the mod produces the expected number of objects when applied.

            var rawBeatmap = createRawBeatmap();
            var testBeatmap = createModdedBeatmap();

            // Both notes and hold notes account for at least one object
            int expectedObjectCount = rawBeatmap.HitObjects.Count;

            Assert.That(testBeatmap.HitObjects.Count == expectedObjectCount);
        }

        private static StepBeatmap createModdedBeatmap()
        {
            var beatmap = createRawBeatmap();
            var holdOffMod = new StepModHoldOff();

            foreach (var hitObject in beatmap.HitObjects)
                hitObject.ApplyDefaults(beatmap.ControlPointInfo, new BeatmapDifficulty());

            holdOffMod.ApplyToBeatmap(beatmap);

            return beatmap;
        }

        private static StepBeatmap createRawBeatmap()
        {
            var beatmap = new StepBeatmap(new StageDefinition(1));
            beatmap.ControlPointInfo.Add(0.0, new TimingControlPoint { BeatLength = 1000 }); // Set BPM to 60

            // Add test hit objects
            beatmap.HitObjects.Add(new Note { StartTime = 4000 });
            beatmap.HitObjects.Add(new Note { StartTime = 4500 });
            beatmap.HitObjects.Add(new HoldNote { StartTime = 0, EndTime = 125 }); // 1/8 note
            beatmap.HitObjects.Add(new HoldNote { StartTime = 0, EndTime = 250 }); // 1/4 note
            beatmap.HitObjects.Add(new HoldNote { StartTime = 0, EndTime = 500 }); // 1/2 note
            beatmap.HitObjects.Add(new HoldNote { StartTime = 0, EndTime = 1000 }); // 1/1 note
            beatmap.HitObjects.Add(new HoldNote { StartTime = 0, EndTime = 2000 }); // 2/1 note
            beatmap.HitObjects.Add(new RollNote { StartTime = 0, EndTime = 1000 }); // 2/1 note

            return beatmap;
        }
    }
}

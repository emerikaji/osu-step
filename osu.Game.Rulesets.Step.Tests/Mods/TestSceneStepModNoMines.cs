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
    public partial class TestSceneStepModNoMines : ModTestScene
    {
        protected override Ruleset CreatePlayerRuleset() => new StepRuleset();

        [Test]
        public void TestMapHasNoMines()
        {
            var testBeatmap = createModdedBeatmap();
            ClassicAssert.False(testBeatmap.HitObjects.OfType<MineNote>().Any());
        }

        [Test]
        public void TestOtherObjectsPreserved()
        {
            var rawBeatmap = createRawBeatmap();
            var testBeatmap = createModdedBeatmap();

            int expectedCount = rawBeatmap.HitObjects.Count(h => h is not MineNote);
            Assert.That(testBeatmap.HitObjects.Count, Is.EqualTo(expectedCount));
        }

        private static StepBeatmap createModdedBeatmap()
        {
            var beatmap = createRawBeatmap();
            var noMinesMod = new StepModNoMines();

            foreach (var hitObject in beatmap.HitObjects)
                hitObject.ApplyDefaults(beatmap.ControlPointInfo, new BeatmapDifficulty());

            noMinesMod.ApplyToBeatmap(beatmap);

            return beatmap;
        }

        private static StepBeatmap createRawBeatmap()
        {
            var beatmap = new StepBeatmap(new StageDefinition(4));
            beatmap.ControlPointInfo.Add(0.0, new TimingControlPoint { BeatLength = 1000 });

            beatmap.HitObjects.Add(new Note { StartTime = 1000, Column = 0 });
            beatmap.HitObjects.Add(new HoldNote { StartTime = 1500, EndTime = 2000, Column = 1 });
            beatmap.HitObjects.Add(new RollNote { StartTime = 2200, EndTime = 2800, Column = 2 });
            beatmap.HitObjects.Add(new MineNote { StartTime = 1200, Column = 3 });
            beatmap.HitObjects.Add(new MineNote { StartTime = 2100, Column = 1 });

            return beatmap;
        }
    }
}

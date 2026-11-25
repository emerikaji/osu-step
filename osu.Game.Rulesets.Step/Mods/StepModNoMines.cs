using System.Linq;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Step.Beatmaps;
using osu.Game.Rulesets.Step.Objects;

namespace osu.Game.Rulesets.Step.Mods
{
    public class StepModNoMines : Mod, IApplicableAfterBeatmapConversion
    {
        public override string Name => "No Mines";

        public override string Acronym => "NM";

        public override LocalisableString Description => @"Removes all mines from the beatmap.";

        public override IconUsage? Icon => FontAwesome.Solid.Ban;

        public override ModType Type => ModType.DifficultyReduction;

        public void ApplyToBeatmap(IBeatmap beatmap)
        {
            var maniaBeatmap = (StepBeatmap)beatmap;
            maniaBeatmap.HitObjects = maniaBeatmap.HitObjects.Where(h => h is not MineNote).ToList();
        }
    }
}

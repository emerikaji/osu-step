// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Step.Beatmaps;
using osu.Game.Rulesets.Step.Replays;

namespace osu.Game.Rulesets.Step.Mods
{
    public class ManiaModAutoplay : ModAutoplay
    {
        public override ModReplayData CreateReplayData(IBeatmap beatmap, IReadOnlyList<Mod> mods)
            => new ModReplayData(new StepAutoGenerator((StepBeatmap)beatmap).Generate(), new ModCreatedUser { Username = "osu!topus" });
    }
}

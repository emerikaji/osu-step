// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Step.Objects;
using osu.Game.Rulesets.Step.Objects.Drawables;

namespace osu.Game.Rulesets.Step.Tests.Skinning
{
    public partial class TestSceneNote : ManiaHitObjectTestScene
    {
        protected override DrawableManiaHitObject CreateHitObject()
        {
            var note = new Note();
            note.ApplyDefaults(new ControlPointInfo(), new BeatmapDifficulty());

            return new DrawableNote(note);
        }
    }
}

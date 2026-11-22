// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Rulesets.Step.Tests.Editor
{
    public partial class TestSceneHoldNotePlacementBlueprint : ManiaPlacementBlueprintTestScene
    {
        protected override DrawableHitObject CreateHitObject(HitObject hitObject) => new DrawableHoldNote((HoldNote)hitObject);
        protected override HitObjectPlacementBlueprint CreateBlueprint() => new HoldNotePlacementBlueprint();
    }
}

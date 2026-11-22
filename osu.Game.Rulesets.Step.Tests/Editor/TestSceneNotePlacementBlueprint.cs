// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using NUnit.Framework;

namespace osu.Game.Rulesets.Step.Tests.Editor
{
    public partial class TestSceneNotePlacementBlueprint : ManiaPlacementBlueprintTestScene
    {
        [SetUp]
        public void Setup() => Schedule(() =>
        {
            this.ChildrenOfType<HitObjectContainer>().ForEach(c => c.Clear());

            ResetPlacement();

            ((ScrollingTestContainer)HitObjectContainer).Direction = ScrollingDirection.Down;
        });

        [Test]
        public void TestPlaceBeforeCurrentTimeDownwards()
        {
            AddStep("seek to 200", () => HitObjectContainer.Dependencies.Get<EditorClock>().Seek(200));
            AddStep("move mouse before current time", () =>
            {
                var column = this.ChildrenOfType<Column>().Single();
                InputManager.MoveMouseTo(column.ScreenSpacePositionAtTime(100));
            });

            AddStep("click", () => InputManager.Click(MouseButton.Left));

            AddAssert("note start time < 200", () => getNote().StartTime < 200);
        }

        [Test]
        public void TestPlaceAfterCurrentTimeDownwards()
        {
            AddStep("seek to 200", () => HitObjectContainer.Dependencies.Get<EditorClock>().Seek(200));
            AddStep("move mouse after current time", () =>
            {
                var column = this.ChildrenOfType<Column>().Single();
                InputManager.MoveMouseTo(column.ScreenSpacePositionAtTime(300));
            });

            AddStep("click", () => InputManager.Click(MouseButton.Left));

            AddAssert("note start time > 200", () => getNote().StartTime > 200);
        }

        private Note getNote() => this.ChildrenOfType<DrawableNote>().FirstOrDefault()?.HitObject;

        protected override DrawableHitObject CreateHitObject(HitObject hitObject) => new DrawableNote((Note)hitObject);
        protected override HitObjectPlacementBlueprint CreateBlueprint() => new NotePlacementBlueprint();
    }
}

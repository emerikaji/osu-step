// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;

namespace osu.Game.Rulesets.Step.Tests.Editor
{
    [TestFixture]
    public partial class TestSceneEditor : EditorTestScene
    {
        private readonly Bindable<ManiaScrollingDirection> direction = new Bindable<ManiaScrollingDirection>();

        protected override Ruleset CreateEditorRuleset() => new StepRuleset();

        public TestSceneEditor()
        {
            AddStep("upwards scroll", () => direction.Value = ManiaScrollingDirection.Up);
            AddStep("downwards scroll", () => direction.Value = ManiaScrollingDirection.Down);
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (ManiaRulesetConfigManager)RulesetConfigs.GetConfigFor(Ruleset.Value.CreateInstance()).AsNonNull();
            config.BindWith(ManiaRulesetSetting.ScrollDirection, direction);
        }

        [Test]
        public void TestReloadOnBPMChange()
        {
            HitObjectComposer oldComposer = null!;

            AddStep("store composer", () => oldComposer = this.ChildrenOfType<HitObjectComposer>().Single());
            AddUntilStep("composer stored", () => oldComposer, () => Is.Not.Null);
            AddStep("switch to timing tab", () => InputManager.Key(Key.F3));
            AddUntilStep("wait for loaded", () => this.ChildrenOfType<DiscreteAdjustmentControl<double>>().ElementAtOrDefault(1), () => Is.Not.Null);
            AddStep("change timing point BPM", () =>
            {
                var bpmControl = this.ChildrenOfType<DiscreteAdjustmentControl<double>>().ElementAt(1);
                InputManager.MoveMouseTo(bpmControl);
                InputManager.Click(MouseButton.Left);
            });

            AddStep("switch back to composer", () => InputManager.Key(Key.F1));
            AddUntilStep("composer reloaded", () =>
            {
                var composer = this.ChildrenOfType<HitObjectComposer>().SingleOrDefault();
                return composer != null && composer != oldComposer;
            });

            AddStep("store composer", () => oldComposer = this.ChildrenOfType<HitObjectComposer>().Single());
            AddUntilStep("composer stored", () => oldComposer, () => Is.Not.Null);
            AddStep("undo", () => Editor.Undo());
            AddUntilStep("composer reloaded", () =>
            {
                var composer = this.ChildrenOfType<HitObjectComposer>().SingleOrDefault();
                return composer != null && composer != oldComposer;
            });
        }
    }
}

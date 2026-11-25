// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Extensions.ObjectExtensions;
using osu.Game.Rulesets.Step.Configuration;
using osu.Game.Rulesets.Step.UI;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Step.Tests
{
    public partial class TestSceneManiaPlayer : PlayerTestScene
    {
        protected override Ruleset CreatePlayerRuleset() => new StepRuleset();

        public override void SetUpSteps()
        {
            base.SetUpSteps();

            AddStep("change direction to down", () => changeDirectionTo(ManiaScrollingDirection.Down));
            AddStep("change direction to up", () => changeDirectionTo(ManiaScrollingDirection.Up));
        }

        private void changeDirectionTo(ManiaScrollingDirection direction)
        {
            var rulesetConfig = (ManiaRulesetConfigManager)RulesetConfigs.GetConfigFor(new StepRuleset()).AsNonNull();
            rulesetConfig.SetValue(ManiaRulesetSetting.ScrollDirection, direction);
        }
    }
}

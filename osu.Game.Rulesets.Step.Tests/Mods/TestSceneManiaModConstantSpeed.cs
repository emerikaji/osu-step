// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;

namespace osu.Game.Rulesets.Step.Tests.Mods
{
    public partial class TestSceneManiaModConstantSpeed : ModTestScene
    {
        protected override Ruleset CreatePlayerRuleset() => new StepRuleset();

        [Test]
        public void TestConstantScroll() => CreateModTest(new ModTestData
        {
            Mod = new ManiaModConstantSpeed(),
            PassCondition = () =>
            {
                var hitObject = Player.ChildrenOfType<DrawableManiaHitObject>().FirstOrDefault();
                return hitObject?.Dependencies.Get<IScrollingInfo>().Algorithm.Value is ConstantScrollAlgorithm;
            }
        });
    }
}

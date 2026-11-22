// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using NUnit.Framework;

namespace osu.Game.Rulesets.Step.Tests.Skinning
{
    public partial class TestScenePlayfield : ManiaSkinnableTestScene
    {
        [Cached]
        private ScoreProcessor scoreProcessor = new ScoreProcessor(new StepRuleset());

        private List<StageDefinition> stageDefinitions = new List<StageDefinition>();

        [Test]
        public void TestSingleStage()
        {
            AddStep("create stage", () =>
            {
                stageDefinitions = new List<StageDefinition>
                {
                    new StageDefinition(2)
                };

                SetContents(_ => new ManiaInputManager(new StepRuleset().RulesetInfo, 2)
                {
                    Child = new ManiaPlayfield(stageDefinitions)
                });
            });

            AddRepeatStep("perform hit", () => scoreProcessor.ApplyResult(new JudgementResult(new HitObject(), new Judgement()) { Type = HitResult.Perfect }), 20);
            AddStep("perform miss", () => scoreProcessor.ApplyResult(new JudgementResult(new HitObject(), new Judgement()) { Type = HitResult.Miss }));
        }

        [TestCase(2)]
        [TestCase(3)]
        [TestCase(5)]
        public void TestDualStages(int columnCount)
        {
            AddStep("create stage", () =>
            {
                stageDefinitions = new List<StageDefinition>
                {
                    new StageDefinition(columnCount),
                    new StageDefinition(columnCount)
                };

                SetContents(_ => new ManiaInputManager(new StepRuleset().RulesetInfo, (int)PlayfieldType.Dual + 2 * columnCount)
                {
                    Child = new ManiaPlayfield(stageDefinitions)
                    {
                        // bit of a hack to make sure the dual stages fit on screen without overlapping each other.
                        Size = new Vector2(1.5f),
                        Scale = new Vector2(1 / 1.5f)
                    }
                });
            });

            AddRepeatStep("perform hit", () => scoreProcessor.ApplyResult(new JudgementResult(new HitObject(), new Judgement()) { Type = HitResult.Perfect }), 20);
            AddStep("perform miss", () => scoreProcessor.ApplyResult(new JudgementResult(new HitObject(), new Judgement()) { Type = HitResult.Miss }));
        }

        protected override IBeatmap CreateBeatmapForSkinProvider()
        {
            var maniaBeatmap = (ManiaBeatmap)base.CreateBeatmapForSkinProvider();
            maniaBeatmap.Stages = stageDefinitions;
            return maniaBeatmap;
        }
    }
}

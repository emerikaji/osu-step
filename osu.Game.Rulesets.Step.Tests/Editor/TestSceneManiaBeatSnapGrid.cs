// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Step.Tests.Editor
{
    public partial class TestSceneManiaBeatSnapGrid : EditorClockTestScene
    {
        [Cached(typeof(IScrollingInfo))]
        private ScrollingTestContainer.TestScrollingInfo scrollingInfo = new ScrollingTestContainer.TestScrollingInfo();

        [Cached(typeof(EditorBeatmap))]
        private EditorBeatmap editorBeatmap = new EditorBeatmap(new ManiaBeatmap(new StageDefinition(2))
        {
            BeatmapInfo =
            {
                Ruleset = new StepRuleset().RulesetInfo
            }
        });

        private readonly ManiaBeatSnapGrid beatSnapGrid;

        public TestSceneManiaBeatSnapGrid()
        {
            editorBeatmap.ControlPointInfo.Add(0, new TimingControlPoint { BeatLength = 200 });
            editorBeatmap.ControlPointInfo.Add(10000, new TimingControlPoint { BeatLength = 200 });

            BeatDivisor.Value = 3;

            // Some sane defaults
            scrollingInfo.Algorithm.Algorithm = ScrollVisualisationMethod.Constant;
            scrollingInfo.Direction.Value = ScrollingDirection.Up;
            scrollingInfo.TimeRange.Value = 1000;

            Children = new Drawable[]
            {
                Playfield = new ManiaPlayfield(new List<StageDefinition>
                {
                    new StageDefinition(4),
                    new StageDefinition(3)
                })
                {
                    Clock = new FramedClock(new StopwatchClock())
                },
                new TestHitObjectComposer(Playfield)
                {
                    Child = beatSnapGrid = new ManiaBeatSnapGrid()
                }
            };
        }

        protected override bool OnMouseMove(MouseMoveEvent e)
        {
            // We're providing a constant scroll algorithm.
            float relativePosition = Playfield.Stages[0].HitObjectContainer.ToLocalSpace(e.ScreenSpaceMousePosition).Y / Playfield.Stages[0].HitObjectContainer.DrawHeight;
            double timeValue = scrollingInfo.TimeRange.Value * relativePosition;

            beatSnapGrid.SelectionTimeRange = (timeValue, timeValue);

            return true;
        }

        public ManiaPlayfield Playfield { get; }
    }

    public partial class TestHitObjectComposer : HitObjectComposer
    {
        public override Playfield Playfield { get; }
        public override ComposeBlueprintContainer BlueprintContainer => throw new NotImplementedException();
        public override IEnumerable<DrawableHitObject> HitObjects => Enumerable.Empty<DrawableHitObject>();
        public override bool CursorInPlacementArea => false;
        public override Bindable<TernaryState> SelectionNewComboState => throw new NotImplementedException();
        public override IReadOnlyDictionary<string, Bindable<TernaryState>> SelectionSampleStates => throw new NotImplementedException();
        public override IReadOnlyDictionary<string, Bindable<TernaryState>> SelectionBankStates => throw new NotImplementedException();
        public override IReadOnlyDictionary<string, Bindable<TernaryState>> SelectionAdditionBankStates => throw new NotImplementedException();
        public override Bindable<bool> AutoSelectionBankEnabled => throw new NotImplementedException();

        public TestHitObjectComposer(Playfield playfield)
            : base(new StepRuleset())
        {
            Playfield = playfield;
        }

        public Drawable Child
        {
            set => InternalChild = value;
        }
    }
}

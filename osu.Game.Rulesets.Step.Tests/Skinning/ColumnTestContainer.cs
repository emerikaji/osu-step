// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Step.Beatmaps;
using osu.Game.Rulesets.Step.UI;

namespace osu.Game.Rulesets.Step.Tests.Skinning
{
    /// <summary>
    /// A container to be used in a <see cref="ManiaSkinnableTestScene"/> to provide a resolvable <see cref="Column"/> dependency.
    /// </summary>
    public partial class ColumnTestContainer : Container
    {
        protected override Container<Drawable> Content => content;

        private readonly Container content;

        [Cached]
        private readonly Column column;

        [Cached]
        private readonly StageDefinition stageDefinition = new StageDefinition(5);

        public ColumnTestContainer(int column, ManiaAction action, bool showColumn = false)
        {
            InternalChildren = new[]
            {
                this.column = new Column(column, false)
                {
                    Action = { Value = action },
                    Alpha = showColumn ? 1 : 0
                },
                content = new ManiaInputManager(new StepRuleset().RulesetInfo, 4)
                {
                    RelativeSizeAxes = Axes.Both
                },
                this.column.TopLevelContainer.CreateProxy()
            };
        }
    }
}

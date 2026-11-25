// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Step.UI.Components;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Step.Tests.Skinning
{
    public partial class TestSceneStageBackground : ManiaSkinnableTestScene
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            SetContents(_ => new SkinnableDrawable(new StepSkinComponentLookup(StepSkinComponents.StageBackground),
                _ => new DefaultStageBackground())
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Width = 0.5f,
            });
        }
    }
}

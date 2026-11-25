// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Step.UI.Components;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Step.Tests.Skinning
{
    public partial class TestSceneColumnHitObjectArea : ManiaSkinnableTestScene
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            SetContents(_ => new FillFlowContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.8f),
                Direction = FillDirection.Horizontal,
                Children = new Drawable[]
                {
                    new ColumnTestContainer(0, ManiaAction.Key1)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = 0.5f,
                        Child = new ColumnHitObjectArea
                        {
                            RelativeSizeAxes = Axes.Both,
                            Child = new HitObjectContainer(),
                        }
                    },
                    new ColumnTestContainer(1, ManiaAction.Key2)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = 0.5f,
                        Child = new ColumnHitObjectArea
                        {
                            RelativeSizeAxes = Axes.Both,
                            Child = new HitObjectContainer(),
                        }
                    }
                }
            });
        }
    }
}

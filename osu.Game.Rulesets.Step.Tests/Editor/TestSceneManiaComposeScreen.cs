// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using NUnit.Framework;

namespace osu.Game.Rulesets.Step.Tests.Editor
{
    public partial class TestSceneManiaComposeScreen : EditorClockTestScene
    {
        [Resolved]
        private SkinManager skins { get; set; } = null!;

        [Cached]
        private EditorClipboard clipboard = new EditorClipboard();

        [SetUpSteps]
        public void SetUpSteps()
        {
            AddStep("setup compose screen", () =>
            {
                var beatmap = new ManiaBeatmap(new StageDefinition(4))
                {
                    BeatmapInfo = { Ruleset = new StepRuleset().RulesetInfo },
                };

                beatmap.ControlPointInfo.Add(0, new TimingControlPoint());

                var editorBeatmap = new EditorBeatmap(beatmap, new LegacyBeatmapSkin(beatmap.BeatmapInfo, null));

                Beatmap.Value = CreateWorkingBeatmap(editorBeatmap.PlayableBeatmap);

                Child = new DependencyProvidingContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    CachedDependencies = new (Type, object)[]
                    {
                        (typeof(EditorBeatmap), editorBeatmap),
                        (typeof(IBeatSnapProvider), editorBeatmap),
                        (typeof(OverlayColourProvider), new OverlayColourProvider(OverlayColourScheme.Green)),
                    },
                    Children = new Drawable[]
                    {
                        editorBeatmap,
                        new ComposeScreen { State = { Value = Visibility.Visible } },
                    }
                };
            });

            AddUntilStep("wait for composer", () => this.ChildrenOfType<HitObjectComposer>().SingleOrDefault()?.IsLoaded == true);
        }

        [Test]
        public void TestDefaultSkin()
        {
            AddStep("set default skin", () => skins.CurrentSkinInfo.Value = TrianglesSkin.CreateInfo().ToLiveUnmanaged());
        }

        [Test]
        public void TestLegacySkin()
        {
            AddStep("set legacy skin", () => skins.CurrentSkinInfo.Value = DefaultLegacySkin.CreateInfo().ToLiveUnmanaged());
        }
    }
}

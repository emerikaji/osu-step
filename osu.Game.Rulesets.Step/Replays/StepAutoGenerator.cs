// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Step.Beatmaps;
using osu.Game.Rulesets.Step.Objects;

namespace osu.Game.Rulesets.Step.Replays
{
    internal class StepAutoGenerator : AutoGenerator<ManiaReplayFrame>
    {
        public const double RELEASE_DELAY = 20;

        public new StepBeatmap Beatmap => (StepBeatmap)base.Beatmap;

        public StepAutoGenerator(StepBeatmap beatmap)
            : base(beatmap)
        {
        }

        protected override void GenerateFrames()
        {
            if (Beatmap.HitObjects.Count == 0)
                return;

            var pointGroups = generateActionPoints().GroupBy(a => a.Time).OrderBy(g => g.First().Time);

            var actions = new List<ManiaAction>();

            foreach (var group in pointGroups)
            {
                foreach (var point in group)
                {
                    switch (point)
                    {
                        case HitPoint:
                            actions.Add(ManiaAction.Key1 + point.Column);
                            break;

                        case ReleasePoint:
                            actions.Remove(ManiaAction.Key1 + point.Column);
                            break;
                    }
                }

                Frames.Add(new ManiaReplayFrame(group.First().Time, actions.ToArray()));
            }
        }

        private IEnumerable<IActionPoint> generateActionPoints()
        {
            for (int i = 0; i < Beatmap.HitObjects.Count; i++)
            {
                var currentObject = Beatmap.HitObjects[i];

                if (currentObject is MineNote)
                    continue;

                if (currentObject is RollNote roll)
                {
                    const double roll_tap_interval = 100;
                    const double roll_press_duration = 25;

                    for (double t = roll.StartTime; t < roll.EndTime - roll_press_duration; t += roll_tap_interval)
                    {
                        yield return new HitPoint { Time = t, Column = roll.Column };
                        yield return new ReleasePoint { Time = t + roll_press_duration, Column = roll.Column };
                    }

                    continue;
                }

                var nextObjectInColumn = GetNextObject(i); // Get the next object that requires pressing the same button
                double releaseTime = calculateReleaseTime(currentObject, nextObjectInColumn);

                yield return new HitPoint { Time = currentObject.StartTime, Column = currentObject.Column };

                yield return new ReleasePoint { Time = releaseTime, Column = currentObject.Column };
            }
        }

        private double calculateReleaseTime(HitObject currentObject, HitObject? nextObject)
        {
            double endTime = currentObject.GetEndTime();
            double releaseDelay = RELEASE_DELAY;

            if (currentObject is HoldNote hold)
            {
                if (hold.Duration > 0)
                    // hold note releases don't have to be timed exactly actually but that's what the code does why am I talking
                    return endTime;

                // Special case for super short hold notes
                releaseDelay = 1;
            }

            bool canDelayKeyUpFully = nextObject == null ||
                                      nextObject.StartTime > endTime + releaseDelay;

            return endTime + (canDelayKeyUpFully ? releaseDelay : (nextObject.AsNonNull().StartTime - endTime) * 0.9);
        }

        protected override HitObject? GetNextObject(int currentIndex)
        {
            int desiredColumn = Beatmap.HitObjects[currentIndex].Column;

            for (int i = currentIndex + 1; i < Beatmap.HitObjects.Count; i++)
            {
                if (Beatmap.HitObjects[i] is MineNote)
                    continue;

                if (Beatmap.HitObjects[i].Column == desiredColumn)
                    return Beatmap.HitObjects[i];
            }

            return null;
        }

        private interface IActionPoint
        {
            double Time { get; set; }
            int Column { get; set; }
        }

        private struct HitPoint : IActionPoint
        {
            public double Time { get; set; }
            public int Column { get; set; }
        }

        private struct ReleasePoint : IActionPoint
        {
            public double Time { get; set; }
            public int Column { get; set; }
        }
    }
}

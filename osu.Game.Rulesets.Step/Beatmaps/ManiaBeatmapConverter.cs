// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Legacy;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Legacy;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring.Legacy;
using osu.Game.Rulesets.Step.Beatmaps.Patterns;
using osu.Game.Rulesets.Step.Beatmaps.Patterns.Legacy;
using osu.Game.Rulesets.Step.Objects;
using osu.Game.Utils;
using osuTK;

namespace osu.Game.Rulesets.Step.Beatmaps
{
    public class ManiaBeatmapConverter : BeatmapConverter<ManiaHitObject>
    {
        /// <summary>
        /// Maximum number of previous notes to consider for density calculation.
        /// </summary>
        private const int max_notes_for_density = 7;

        /// <summary>
        /// The total number of columns.
        /// </summary>
        public int TotalColumns => TargetColumns * (Dual ? 2 : 1);

        /// <summary>
        /// The number of columns per-stage.
        /// </summary>
        public int TargetColumns;

        /// <summary>
        /// Whether to double the number of stages.
        /// </summary>
        public bool Dual;

        /// <summary>
        /// Whether the beatmap instantiated with is for the mania ruleset.
        /// </summary>
        public readonly bool IsForCurrentRuleset;

        /// <summary>
        /// Static object cache to add new statistics.
        /// </summary>
        public static readonly Dictionary<string, (int Rolls, int Mines)> ObjectCountsByHash = new Dictionary<string, (int, int)>();

        // Internal for testing purposes
        internal readonly LegacyRandom Random;

        private Pattern lastPattern = new Pattern();

        public ManiaBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : this(beatmap, LegacyBeatmapConversionDifficultyInfo.FromBeatmap(beatmap), ruleset)
        {
        }

        private static bool isManiaFormattedBeatmap(IBeatmap? beatmap)
        {
            if (beatmap?.HitObjects == null || beatmap.HitObjects.Count == 0)
                return false;

            foreach (var obj in beatmap.HitObjects)
            {
                if (obj is IHasPosition pos && !Precision.AlmostEquals(pos.Y, 192, 0.5f))
                    return false;
            }

            return true;
        }

        private ManiaBeatmapConverter(IBeatmap? beatmap, LegacyBeatmapConversionDifficultyInfo difficulty, Ruleset ruleset)
            : base(beatmap!, ruleset)
        {
            IsForCurrentRuleset = difficulty.SourceRuleset.Equals(ruleset.RulesetInfo) || isManiaFormattedBeatmap(beatmap);
            Random = new LegacyRandom((int)MathF.Round(difficulty.DrainRate + difficulty.CircleSize) * 20 + (int)(difficulty.OverallDifficulty * 41.2) + (int)MathF.Round(difficulty.ApproachRate));
            TargetColumns = getColumnCount(difficulty, IsForCurrentRuleset);

            if (IsForCurrentRuleset && TargetColumns > StepRuleset.MAX_STAGE_KEYS)
            {
                // If support for odd key counts above 10 (11 / 13 / 15 / 17K) is ever desired, this code needs to go.
                // For now, it's probably fine, as stable doesn't support them outside of manual .osu edits either.
                TargetColumns /= 2;
                Dual = true;
            }

            static int getColumnCount(LegacyBeatmapConversionDifficultyInfo difficulty, bool isForCurrentRuleset)
            {
                if (!isForCurrentRuleset && difficulty.SourceRuleset.ShortName != StepRuleset.SHORT_NAME) return 4;

                double roundedCircleSize = Math.Round(difficulty.CircleSize);
                return (int)Math.Max(1, roundedCircleSize);
            }
        }

        public static int GetColumnCount(LegacyBeatmapConversionDifficultyInfo difficulty, IReadOnlyCollection<Mod>? mods = null)
        {
            var converter = new ManiaBeatmapConverter(null, difficulty, new StepRuleset());

            if (mods != null)
            {
                foreach (var m in mods.OfType<IApplicableToBeatmapConverter>())
                    m.ApplyToBeatmapConverter(converter);
            }

            return converter.TotalColumns;
        }

        public override bool CanConvert() => Beatmap.HitObjects.All(h => h is IHasXPosition);

        protected override Beatmap<ManiaHitObject> CreateBeatmap()
        {
            ManiaBeatmap beatmap = new ManiaBeatmap(new StageDefinition(TargetColumns));

            if (Dual)
                beatmap.Stages.Add(new StageDefinition(TargetColumns));

            return beatmap;
        }

        protected override IEnumerable<ManiaHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap, CancellationToken cancellationToken)
        {
            LegacyHitObjectType legacyType;

            switch (original)
            {
                case ManiaHitObject maniaObj:
                {
                    yield return maniaObj;

                    yield break;
                }

                case IHasLegacyHitObjectType legacy:
                    legacyType = legacy.LegacyType & LegacyHitObjectType.ObjectTypes;
                    break;

                case IHasPath:
                    legacyType = LegacyHitObjectType.Slider;
                    break;

                case IHasDuration:
                    legacyType = IsForCurrentRuleset ? LegacyHitObjectType.Hold : LegacyHitObjectType.Spinner;
                    break;

                default:
                    legacyType = LegacyHitObjectType.Circle;
                    break;
            }

            double startTime = original.StartTime;
            double endTime = (original as IHasDuration)?.EndTime ?? startTime;
            Vector2 position = (original as IHasPosition)?.Position ?? Vector2.Zero;

            PatternGenerator conversion;

            switch (legacyType)
            {
                case LegacyHitObjectType.Circle:
                    if (IsForCurrentRuleset)
                    {
                        conversion = new PassThroughPatternGenerator(Random, original, beatmap, TotalColumns, lastPattern);
                        recordNote(startTime, position);
                    }
                    else
                    {
                        // Note: The density is used during the pattern generator constructor, and intentionally computed first.
                        computeDensity(startTime);
                        conversion = new HitCirclePatternGenerator(Random, original, beatmap, TotalColumns, lastPattern, lastTime, lastPosition, density, lastStair);
                        recordNote(startTime, position);
                    }

                    break;

                case LegacyHitObjectType.Slider:
                    if (IsForCurrentRuleset)
                    {
                        conversion = new PassThroughPatternGenerator(Random, original, beatmap, TotalColumns, lastPattern);
                        recordNote(original.StartTime, position);
                    }
                    else
                    {
                        var generator = new SliderPatternGenerator(Random, original, beatmap, TotalColumns, lastPattern);
                        conversion = generator;

                        for (int i = 0; i <= generator.SpanCount; i++)
                        {
                            double time = original.StartTime + generator.SegmentDuration * i;

                            recordNote(time, position);
                            computeDensity(time);
                        }
                    }

                    break;

                case LegacyHitObjectType.Spinner:
                    // Note: Some older mania-specific beatmaps can have spinners that are converted rather than passed through.
                    //       Newer beatmaps will usually use the "hold" hitobject type below.
                    conversion = new SpinnerPatternGenerator(Random, original, beatmap, TotalColumns, lastPattern);
                    recordNote(endTime, new Vector2(256, 192));
                    computeDensity(endTime);
                    break;

                case LegacyHitObjectType.Hold:
                    conversion = new PassThroughPatternGenerator(Random, original, beatmap, TotalColumns, lastPattern);
                    recordNote(endTime, position);
                    computeDensity(endTime);
                    break;

                default:
                    throw new ArgumentException($"Invalid legacy object type: {legacyType}", nameof(original));
            }

            foreach (var newPattern in conversion.Generate())
            {
                if (conversion is HitCirclePatternGenerator circleGenerator)
                    lastStair = circleGenerator.StairType;

                if (conversion is HitCirclePatternGenerator || conversion is SliderPatternGenerator)
                    lastPattern = newPattern;

                foreach (var obj in newPattern.HitObjects)
                    yield return obj;
            }
        }

        protected override Beatmap<ManiaHitObject> ConvertBeatmap(IBeatmap original, CancellationToken cancellationToken)
        {
            var converted = base.ConvertBeatmap(original, cancellationToken);

            // Only inject pattern-based mines on Mode: 0 (osu!standard) converts
            if (IsForCurrentRuleset || converted.HitObjects.Count == 0)
                return converted;

            var minesToAdd = new List<MineNote>();

            // Track occupied intervals per column so mines never overlap notes or holds
            var busyIntervals = new List<(double start, double end)>[TotalColumns];
            for (int c = 0; c < TotalColumns; c++)
                busyIntervals[c] = new List<(double, double)>();

            foreach (var obj in converted.HitObjects)
            {
                double end = obj.GetEndTime();
                busyIntervals[obj.Column].Add((obj.StartTime - 100, end + 100));
            }

            bool isColumnFree(int col, double time, double buffer = 95)
            {
                foreach (var (s, e) in busyIntervals[col])
                {
                    if (time >= s - buffer && time <= e + buffer)
                        return false;
                }

                return true;
            }

            void tryAddMine(int col, double time)
            {
                if (!isColumnFree(col, time))
                    return;

                minesToAdd.Add(new MineNote
                {
                    StartTime = time,
                    Column = col,
                });

                busyIntervals[col].Add((time - 90, time + 90));
            }

            // Group converted hit objects by StartTime
            var groups = converted.HitObjects
                                  .GroupBy(h => Math.Round(h.StartTime, 1))
                                  .OrderBy(g => g.Key)
                                  .ToList();

            for (int i = 0; i < groups.Count; i++)
            {
                var group = groups[i].ToList();
                double groupStart = group[0].StartTime;
                double groupMaxEnd = group.Max(h => h.GetEndTime());

                double prevGap = i > 0 ? groupStart - groups[i - 1].Max(h => h.GetEndTime()) : double.PositiveInfinity;
                double nextGap = i < groups.Count - 1 ? groups[i + 1].First().StartTime - groupMaxEnd : double.PositiveInfinity;

                // 1. Sudden Pause / Break Wall: dense phrase followed by a sudden >= 500ms pause
                if (prevGap < 220 && nextGap >= 500 && nextGap < double.PositiveInfinity)
                {
                    double wallTime = groupMaxEnd + 130;
                    for (int col = 0; col < TotalColumns; col++)
                        tryAddMine(col, wallTime);

                    continue;
                }

                // 2. Post-Hold Release Check: long hold (>= 350ms) followed by a gap >= 240ms
                foreach (var hold in group.OfType<HoldNote>())
                {
                    if (hold is RollNote || hold.Duration < 350)
                        continue;

                    double nextNoteTime = i < groups.Count - 1 ? groups[i + 1].First().StartTime : double.PositiveInfinity;
                    if (nextNoteTime - hold.EndTime >= 240)
                        tryAddMine(hold.Column, hold.EndTime + 120);
                }

                // 3. Intense / Kiai Section Trip-Ups: 1-2 mines in free columns during fast sections
                bool isKiai = original.ControlPointInfo.EffectPointAt(groupStart).KiaiMode;
                bool isDense = prevGap < 160 && nextGap < 160;

                if ((isKiai || isDense) && group.Count <= TotalColumns - 2 && Random.NextDouble() < 0.22)
                {
                    var freeCols = Enumerable.Range(0, TotalColumns)
                                             .Where(c => isColumnFree(c, groupStart, 110))
                                             .OrderBy(_ => Random.NextDouble())
                                             .ToList();

                    // Always leave at least 1-2 open lanes so the pattern stays playable
                    int maxMines = Math.Min(freeCols.Count - 1, TotalColumns >= 4 && Random.NextDouble() < 0.25 ? 2 : 1);

                    for (int m = 0; m < maxMines; m++)
                        tryAddMine(freeCols[m], groupStart);
                }
            }

            if (minesToAdd.Count > 0)
            {
                converted.HitObjects.AddRange(minesToAdd);
                converted.HitObjects.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));
            }

            if (!string.IsNullOrEmpty(original.BeatmapInfo?.Hash))
            {
                int rolls = converted.HitObjects.OfType<RollNote>().Count();
                int mines = converted.HitObjects.OfType<MineNote>().Count();
                ObjectCountsByHash[original.BeatmapInfo.Hash] = (rolls, mines);
            }

            return converted;
        }


        private readonly LimitedCapacityQueue<double> prevNoteTimes = new LimitedCapacityQueue<double>(max_notes_for_density);
        private double density = int.MaxValue;

        private void computeDensity(double newNoteTime)
        {
            prevNoteTimes.Enqueue(newNoteTime);

            if (prevNoteTimes.Count >= 2)
                density = (prevNoteTimes[^1] - prevNoteTimes[0]) / prevNoteTimes.Count;
        }

        private double lastTime;
        private Vector2 lastPosition;
        private PatternType lastStair = PatternType.Stair;

        private void recordNote(double time, Vector2 position)
        {
            lastTime = time;
            lastPosition = position;
        }
    }
}

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using NUnit.Framework;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Step.Beatmaps;
using osu.Game.Rulesets.Step.Objects;
using osu.Game.Tests.Beatmaps;

namespace osu.Game.Rulesets.Step.Tests
{
    [TestFixture]
    public class StepBeatmapConversionTest : BeatmapConversionTest<ManiaConvertMapping, ConvertValue>
    {
        private static string getBeatmapsDir([CallerFilePath] string path = "") => Path.Combine(Path.GetDirectoryName(path)!, "Resources", "Testing", "Beatmaps");

        [Test]
        [Explicit("Run once manually to regenerate *-expected-conversion.json files")]
        public void RegenerateSnapshots()
        {
            string beatmapsDir = getBeatmapsDir();

            string[] cases =
            {
                "basic",
                "zero-length-slider",
                "mania-specific-spinner",
                "20544",
                "100374",
                "1450162",
                "4869637",
                "1K", "2K", "3K", "4K", "5K", "6K", "7K", "8K", "9K", "10K"
            };

            foreach (string name in cases)
            {
                var beatmap = GetBeatmap(name);
                var converter = CreateRuleset().CreateBeatmapConverter(beatmap);

                var mappings = new List<ManiaConvertMapping>();

                converter.ObjectConverted += (original, converted) =>
                {
                    OnConversionGenerated(original, converted, converter);

                    var mapping = CreateConvertMapping(original);
                    mapping.StartTime = original.StartTime;
                    mapping.Objects.AddRange(converted.SelectMany(CreateConvertValue));
                    mapping.PostProcess();
                    mappings.Add(mapping);
                };

                converter.Convert();

                var wrapper = new
                {
                    Mappings = mappings.Select(m => new
                    {
                        m.RandomW,
                        m.RandomX,
                        m.RandomY,
                        m.RandomZ,
                        m.StartTime,
                        m.Objects,
                    })
                };

                string json = JsonConvert.SerializeObject(wrapper, Formatting.Indented);
                File.WriteAllText(Path.Combine(beatmapsDir, $"{name}-expected-conversion.json"), json);
            }
        }

        protected override string ResourceAssembly => "osu.Game.Rulesets.Step.Tests";

        [TestCase("basic")]
        [TestCase("zero-length-slider")]
        [TestCase("mania-specific-spinner")]
        [TestCase("20544")]
        [TestCase("100374")]
        [TestCase("1450162")]
        [TestCase("4869637")]
        [TestCase("1K")]
        [TestCase("2K")]
        [TestCase("3K")]
        [TestCase("4K")]
        [TestCase("5K")]
        [TestCase("6K")]
        [TestCase("7K")]
        [TestCase("8K")]
        [TestCase("9K")]
        [TestCase("10K")]
        // Circle size is clamped to 10 for custom rulesets. No chance of getting those to work for now.
        // [TestCase("11K")] <- See comment in `ManiaBeatmapConverter` ctor for disable reason.
        // [TestCase("12K")]
        // [TestCase("13K")] <- See comment in `ManiaBeatmapConverter` ctor for disable reason.
        // [TestCase("14K")]
        // [TestCase("15K")] <- See comment in `ManiaBeatmapConverter` ctor for disable reason.
        // [TestCase("16K")]
        // [TestCase("17K")] <- See comment in `ManiaBeatmapConverter` ctor for disable reason.
        // [TestCase("18K")]
        public void Test(string name) => base.Test(name);

        protected override IEnumerable<ConvertValue> CreateConvertValue(HitObject hitObject)
        {
            yield return new ConvertValue
            {
                StartTime = hitObject.StartTime,
                EndTime = hitObject.GetEndTime(),
                Column = ((ManiaHitObject)hitObject).Column
            };
        }

        private readonly Dictionary<HitObject, RngSnapshot> rngSnapshots = new Dictionary<HitObject, RngSnapshot>();

        protected override void OnConversionGenerated(HitObject original, IEnumerable<HitObject> result, IBeatmapConverter beatmapConverter)
        {
            base.OnConversionGenerated(original, result, beatmapConverter);

            rngSnapshots[original] = new RngSnapshot(beatmapConverter);
        }

        protected override ManiaConvertMapping CreateConvertMapping(HitObject source) => new ManiaConvertMapping(rngSnapshots[source]);

        protected override Ruleset CreateRuleset() => new StepRuleset();
    }

    public class RngSnapshot
    {
        public readonly uint RandomW;
        public readonly uint RandomX;
        public readonly uint RandomY;
        public readonly uint RandomZ;

        public RngSnapshot(IBeatmapConverter converter)
        {
            var maniaConverter = (StepBeatmapConverter)converter;
            RandomW = maniaConverter.Random.W;
            RandomX = maniaConverter.Random.X;
            RandomY = maniaConverter.Random.Y;
            RandomZ = maniaConverter.Random.Z;
        }
    }

    public class ManiaConvertMapping : ConvertMapping<ConvertValue>, IEquatable<ManiaConvertMapping>
    {
        public uint RandomW;
        public uint RandomX;
        public uint RandomY;
        public uint RandomZ;

        public ManiaConvertMapping()
        {
        }

        public ManiaConvertMapping(RngSnapshot snapshot)
        {
            RandomW = snapshot.RandomW;
            RandomX = snapshot.RandomX;
            RandomY = snapshot.RandomY;
            RandomZ = snapshot.RandomZ;
        }

        public override void PostProcess()
        {
            base.PostProcess();
            Objects.Sort();
        }

        public bool Equals(ManiaConvertMapping other) => other != null && RandomW == other.RandomW && RandomX == other.RandomX && RandomY == other.RandomY && RandomZ == other.RandomZ;
        public override bool Equals(ConvertMapping<ConvertValue> other) => base.Equals(other) && Equals(other as ManiaConvertMapping);
    }

    public struct ConvertValue : IEquatable<ConvertValue>, IComparable<ConvertValue>
    {
        /// <summary>
        /// A sane value to account for osu!stable using ints everwhere.
        /// </summary>
        private const float conversion_lenience = 2;

        public double StartTime;
        public double EndTime;
        public int Column;

        public bool Equals(ConvertValue other)
            => Precision.AlmostEquals(StartTime, other.StartTime, conversion_lenience)
               && Precision.AlmostEquals(EndTime, other.EndTime, conversion_lenience)
               && Column == other.Column;

        public int CompareTo(ConvertValue other)
        {
            int result = StartTime.CompareTo(other.StartTime);

            if (result != 0)
                return result;

            return Column.CompareTo(other.Column);
        }
    }
}

# osu!step

osu!step is a keyboard rhythm-game ruleset for osu!, inspired by StepMania and based on osu!mania. It builds on a mania-style playfield and adds StepMania-inspired holds, rolls, and mines.

Thank you for checking it out!

## Current status

The ruleset implements the same play modes as osu!mania, with visuals and features modified to resemble Stepmania. Holds include a 250 ms grace period for re-grabbing a released hold, and display `OK!` and `NG!` feedback. Mines trigger when pressed, breaking combo and displaying `BOOM!` as feedback. Rolls are similar to holds but require repeated key presses within 250ms.

During conversion, spinners become rolls, while mines are added in patterns based on the song and beatmap features. The `Hold Off` mod disables rolls alongside holds, and a new `No Mines` mod is added to disable mines post-conversion.

## Development

- `osu.Game.Rulesets.Step` is the ruleset library. It contains beatmap conversion, gameplay and input, hit objects, scoring and difficulty calculations, replay support, mods, and skinning. It targets .NET 10 and references `ppy.osu.Game`.
- `osu.Game.Rulesets.Step.Tests` is the NUnit test and visual-test project. It references the ruleset library and covers areas including beatmap conversion, scoring, input, mods, and skins.

Both projects were directly converted from the `ppy/osu` Mania ruleset so bugs and unused code may exist. To cope with this, files that were extensively modified were renamed with a Step prefix, while files still close to the source still have the Mania prefix. This is especially important for tests as, while they pass, they might not test for osu!step specific features.
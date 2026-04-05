// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Framework.Timing;
using osu.Framework.Utils;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Rulesets.Catch.Objects;
using osu.Game.Rulesets.Catch.Objects.Drawables;
using osu.Game.Rulesets.Catch.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Catch.Mods
{
    public class CatchModMagnetised : Mod, IUpdatableByPlayfield, IApplicableToDrawableRuleset<CatchHitObject>
    {
        public override string Name => "Magnetised";
        public override string Acronym => "MG";
        public override IconUsage? Icon => OsuIcon.ModMagnetised;
        public override ModType Type => ModType.Fun;
        public override LocalisableString Description => "No need to chase the circles – your cursor is a magnet!";
        public override double ScoreMultiplier => 0.5;
        public override Type[] IncompatibleMods => new[] { typeof(ModAutoplay) };
        private CatchPlayfield? catchPlayfield;

        [SettingSource("Attraction strength", "How strong the pull is.", 0)]
        public BindableFloat AttractionStrength { get; } = new BindableFloat(0.5f)
        {
            Precision = 0.05f,
            MinValue = 0.05f,
            MaxValue = 1.0f,
        };

        public void ApplyToDrawableRuleset(DrawableRuleset<CatchHitObject> drawableRuleset)
        {
            catchPlayfield = (CatchPlayfield)drawableRuleset.Playfield;
        }

        public void Update(Playfield playfield)
        {
            if (catchPlayfield == null) return;
            var catcherPos = catchPlayfield.Catcher.Position;

            foreach (var entry in playfield.HitObjectContainer.AliveEntries)
            {
                var drawable = entry.Value;
                handleDrawable(playfield.Clock, (DrawableCatchHitObject)drawable, catcherPos);
            }
        }

        private void handleDrawable(IFrameBasedClock clock, DrawableCatchHitObject hitObject, Vector2 destination)
        {
            if (hitObject.NestedHitObjects.Count > 0)
            {
                foreach (DrawableCatchHitObject nestedHitObject in hitObject.NestedHitObjects)
                {
                    handleDrawable(clock, nestedHitObject, destination);
                }
                return;
            }
            switch (hitObject)
            {
                case DrawableFruit fruit:
                    easeTo(clock, fruit, destination, false);
                    break;
                case DrawableDroplet droplet:
                    easeTo(clock, droplet, destination, false);
                    break;
                case DrawableBanana banana:
                    easeTo(clock, banana, destination, true);
                    break;
            }
        }

        private void easeTo(IFrameBasedClock clock, DrawableCatchHitObject hitObject, Vector2 destination, bool offsetObject)
        {
            double dampLength = Interpolation.Lerp(3000, 40, AttractionStrength.Value);

            float x = (float)Interpolation.DampContinuously(hitObject.Position.X, destination.X, dampLength, clock.ElapsedFrameTime);

            if (offsetObject)
            {
                hitObject.X = x;
                float xOffset = (float)Interpolation.DampContinuously(hitObject.HitObject.XOffset, destination.X, dampLength, clock.ElapsedFrameTime);
                hitObject.HitObject.XOffset = xOffset;
            }
            else
            {
                hitObject.X = x;
                hitObject.HitObject.X = x;
            }

            hitObject.Position = new Vector2(x, hitObject.Position.Y);
        }
    }
}

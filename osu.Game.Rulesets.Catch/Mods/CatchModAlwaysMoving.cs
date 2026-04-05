// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Rulesets.Catch.Objects;
using osu.Game.Rulesets.Catch.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Catch.Mods
{
    public partial class CatchModAlwaysMoving : Mod, IApplicableToDrawableRuleset<CatchHitObject>
    {
        public override string Name => "Always Moving";
        public override string Acronym => "AM";
        public override LocalisableString Description => "Once you start moving, you can't stop!";
        public override ModType Type => ModType.Fun;
        public override double ScoreMultiplier => 1;
        public override IconUsage? Icon => OsuIcon.ModMovingFast;
        public override Type[] IncompatibleMods => new[] { typeof(ModAutoplay), typeof(ModRelax) };

        public void ApplyToDrawableRuleset(DrawableRuleset<CatchHitObject> drawableRuleset)
        {
            var catchPlayfield = (CatchPlayfield)drawableRuleset.Playfield;
            catchPlayfield.CatcherArea.Add(new ForceMovementInputHelper(catchPlayfield.CatcherArea));
        }

        private partial class ForceMovementInputHelper : Drawable, IKeyBindingHandler<CatchAction>
        {
            private readonly CatcherArea catcherArea;
            public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;

            public ForceMovementInputHelper(CatcherArea catcherArea)
            {
                this.catcherArea = catcherArea;

                RelativeSizeAxes = Axes.Both;
            }

            public bool OnPressed(KeyBindingPressEvent<CatchAction> e)
            {
                if (e.Action == CatchAction.MoveLeft || e.Action == CatchAction.MoveRight)
                {
                    catcherArea.CurrentDirection = 0;
                }
                return false;
            }

            public void OnReleased(KeyBindingReleaseEvent<CatchAction> e)
            {
                switch (e.Action)
                {
                    case CatchAction.MoveLeft:
                        catcherArea.CurrentDirection--;
                        break;

                    case CatchAction.MoveRight:
                        catcherArea.CurrentDirection++;
                        break;
                }
            }
        }
    }
}

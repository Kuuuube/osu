// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Game.Graphics;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Taiko.Skinning.Legacy
{
    public partial class LegacyHit : LegacyCirclePiece
    {
        private readonly TaikoSkinComponents component;

        public LegacyHit(TaikoSkinComponents component)
        {
            this.component = component;
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            AccentColour = LegacyColourCompatibility.DisallowZeroAlpha(
                component == TaikoSkinComponents.CentreHit
                    ? colours.TaikoPink
                    : colours.TaikoBlue);
        }
    }
}

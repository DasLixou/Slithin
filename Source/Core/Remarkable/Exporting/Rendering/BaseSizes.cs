﻿namespace Slithin.Core.Remarkable.Rendering
{
    public enum BrushBaseSize
    {
        Small,
        Mid,
        Large
    }

    public struct BaseSizes
    {
        public const float Large = 2.125f;
        public const float Mid = 2.0f;
        public const float Small = 1.875f;

        public static float GetValue(BrushBaseSize size) 
            => size switch
            {
                BrushBaseSize.Small => Small,
                BrushBaseSize.Mid => Mid,
                BrushBaseSize.Large => Large,
                _ => Small,
            };

        public static BrushBaseSize Parse(float value) 
            => value switch
            {
                Small => BrushBaseSize.Small,
                Mid => BrushBaseSize.Mid,
                Large => BrushBaseSize.Large,
                _ => BrushBaseSize.Small,
            };
    }
}

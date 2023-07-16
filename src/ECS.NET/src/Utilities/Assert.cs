using System;
using System.Diagnostics;

namespace ECS.NET.Utilities
{
    // TODO: Make assertions more descriptive. Maybe add log codes.
    public static class Assert
    {
        [Conditional("DEBUG")]
        public static void True(bool condition)
        {
            if (!condition)
                throw new ArgumentException("Temporary message.", nameof(condition));
        }
    }
}
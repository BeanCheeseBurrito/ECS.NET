using ECS.NET.Utilities;

namespace ECS.NET.Unmanaged
{
    /// <summary>
    ///     Alternative to the <c>System.String</c> class that allows strings to be used in unmanaged structs
    /// </summary>
    public unsafe struct EcsString
    {
        private readonly char* _data;

        public EcsString(string str)
        {
            _data = Memory.Alloc<char>((ulong)str.Length);
        }
    }
}

// ReSharper disable CheckNamespace
#if NET6_0
namespace System.Runtime.CompilerServices
{
    [System.AttributeUsage( AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Delegate | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class NullableContextAttribute : Attribute
    {
        public readonly byte Flag;
        public NullableContextAttribute(byte flag)
        {
            Flag = flag;
        }
    }
}
#endif

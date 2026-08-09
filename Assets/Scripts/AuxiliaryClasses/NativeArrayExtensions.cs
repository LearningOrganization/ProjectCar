using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public static class NativeArrayExtensions
{
    public static unsafe ref T ExtractElementRef<T>(
        this NativeArray<T> array,
        int index
    )
    where T : unmanaged
    {
        return ref UnsafeUtility.ArrayElementAsRef<T>(
            array.GetUnsafePtr(),
            index
        );
    }
}
using System;
using System.Collections.Generic;

namespace PostEnot.DxHints
{
    public interface IBuffer<T> : IReadOnlyList<T>
    {
        public new T this[int index] { get; set; }

        public void EnsureCapacity(int capacity, bool preserveData = false);
        public void Grow(int growingStep, int maxLength = int.MaxValue, bool preserveData = false);
        public void Shrink(int newLength);
        public void ClearFrom(int fromIndex);
        public void Clear();
        public Span<T> Slice(int start, int length);
        public Span<T> AsSpan();
        public ReadOnlySpan<T> AsReadOnlySpan();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;

namespace PostEnot.DxHints
{
    public sealed class Buffer<T> : IReadOnlyList<T>, IBuffer<T>
    {
        public struct Enumerator : IEnumerator<T>
        {
            private readonly Buffer<T> _buffer;
            private readonly int _version;

            private int _index;
            private T _current;

            internal Enumerator(Buffer<T> buffer)
            {
                _buffer = buffer;
                _version = buffer._version;
                _index = -1;
                _current = default;
            }

            public readonly T Current
            {
                get
                {
                    if (_buffer._version != _version)
                    {
                        throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                    }
                    if (_index < 0 || _index >= _buffer.Count)
                    {
                        throw new InvalidOperationException("Enumeration not started or already finished.");
                    }
                    return _current;
                }
            }

            readonly object IEnumerator.Current => Current;

            public readonly void Dispose() { }

            public bool MoveNext()
            {
                if (_buffer._version != _version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }

                if (_index < _buffer.Count - 1)
                {
                    _index += 1;
                    _current = _buffer[_index];
                    return true;
                }

                _index = _buffer.Count;
                _current = default;
                return false;
            }

            public void Reset()
            {
                if (_buffer._version != _version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }
                _index = -1;
                _current = default;
            }
        }

        public Buffer(int size) => Array = new T[size];

        public T[] Array { get; private set; }
        public int Count => Array.Length;

        private int _version;

        public T this[int index]
        {
            get => Array[index];
            set => Array[index] = value;
        }

        public void EnsureCapacity(int capacity, bool preserveData = false)
        {
            if (capacity <= Array.Length)
            {
                return;
            }
            T[] array = new T[capacity];
            if (preserveData)
            {
                Array.CopyTo(array, 0);
            }
            Array = array;
            unchecked
            {
                _version += 1;
            }
        }

        public void Grow(int growingStep, int maxLength = int.MaxValue, bool preserveData = false)
        {
            if (growingStep <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(growingStep));
            }
            if (maxLength < Array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength));
            }
            long longLength = (long)Array.Length + growingStep;
            longLength = Math.Min(longLength, maxLength);
            int newLength = (int)longLength;
            T[] array = new T[newLength];
            if (preserveData)
            {
                System.Array.Copy(Array, array, newLength);
            }
            Array = array;
            unchecked
            {
                _version += 1;
            }
        }

        public void Shrink(int newLength)
        {
            if (newLength < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(newLength));
            }
            if (newLength > Array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(newLength));
            }
            Array = new T[newLength];
            unchecked
            {
                _version += 1;
            }
        }

        public void ClearFrom(int fromIndex)
        {
            for (int i = fromIndex; i < Array.Length; i += 1)
            {
                Array[i] = default;
            }
        }

        public void Clear()
        {
            for (int i = 0; i < Array.Length; i += 1)
            {
                Array[i] = default;
            }
        }

        public Span<T> AsSpan() => Array;

        public ReadOnlySpan<T> AsReadOnlySpan() => Array;

        public Span<T> Slice(int start, int length) => Array.AsSpan().Slice(start, length);

        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static implicit operator Span<T>(Buffer<T> buffer) => buffer.Array;

        public static implicit operator ReadOnlySpan<T>(Buffer<T> buffer) => buffer.Array;

        public static implicit operator T[](Buffer<T> buffer) => buffer.Array;
    }
}

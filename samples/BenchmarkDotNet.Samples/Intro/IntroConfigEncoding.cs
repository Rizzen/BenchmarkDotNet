﻿using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Samples.Algorithms;

namespace BenchmarkDotNet.Samples.Intro
{
    [EncodingAttribute.Unicode]
    public class IntroConfigEncoding
    {
        private const int N = 1002;
        private readonly ulong[] numbers;
        private readonly Random random = new Random(42);
        
        public IntroConfigEncoding()
        {
            numbers = new ulong[N];
            for (int i = 0; i < N; i++)
                numbers[i] = NextUInt64();
        }
        
        public ulong NextUInt64()
        {
            var buffer = new byte[sizeof(long)];
            random.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }

        [Benchmark]
        public double Foo()
        {
            int counter = 0;
            for (int i = 0; i < N / 2; i++)
                counter += BitCountHelper.PopCountParallel2(numbers[i],numbers[i+1]);
            return counter;
        }
    }
}
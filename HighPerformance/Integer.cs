using System;
using System.Collections.Generic;
using System.Text;

namespace HighPerformance
{
    public static class Integer
    {
        public static int Parse(string input)
        {
            return 0;
        }
        public static int Parse(string input, int startIndex, int length)
        {
            return 0;
        }
        unsafe public static int Parse(char* inputPtr, int length)
        {
            return 0;
        }
        unsafe public static int Parse(char* inputPtr, int startIndex, int length)
        {
            return 0;
        }
        private static void Copy<T>(T[] source, T[] destination) { }
        private static void Copy<T>(T[] source, int sourceStartIndex, T[] destination, int destinationStartIndex, int elementsCount) { }
        private static unsafe void Copy<T>(void* source, void* destination, int elementsCount) { }
        private static unsafe void Copy<T>(void* source, int sourceStartIndex, void* destination, int destinationStartIndex, int elementsCount) { }
        private static unsafe void Copy<T>(void* source, int sourceLength, T[] destination) { }
        private static unsafe void Copy<T>(void* source, int sourceStartIndex, T[] destination, int destinationStartIndex, int elementsCount) { }
    }
}

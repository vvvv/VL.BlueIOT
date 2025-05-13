#region License
/*
 * Ext.cs
 *
 * Some parts of this code are derived from Mono (http://www.mono-project.com):
 * - The GetStatusDescription method is derived from HttpListenerResponse.cs (System.Net)
 * - The MaybeUri method is derived from Uri.cs (System)
 * - The isPredefinedScheme method is derived from Uri.cs (System)
 *
 * The MIT License
 *
 * Copyright (c) 2001 Garrett Rooney
 * Copyright (c) 2003 Ian MacLean
 * Copyright (c) 2003 Ben Maurer
 * Copyright (c) 2003, 2005, 2009 Novell, Inc. (http://www.novell.com)
 * Copyright (c) 2009 Stephane Delcroix
 * Copyright (c) 2010-2025 sta.blockhead
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

#region Contributors
/*
 * Contributors:
 * - Liryna <liryna.stark@gmail.com>
 * - Nikola Kovacevic <nikolak@outlook.com>
 * - Chris Swiedler
 */
#endregion

//

using System.Collections.Specialized;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Linq;

namespace LsWebsocketClient
{
    /// <summary>
    /// Provides a set of static methods.
    /// </summary>
    public static class Ext
    {
        /// <summary>
        /// Determines whether the specified byte order is host (this computer
        /// architecture) byte order.
        /// </summary>
        /// <returns>
        /// <c>true</c> if <paramref name="order"/> is host byte order; otherwise,
        /// <c>false</c>.
        /// </returns>
        /// <param name="order">
        /// One of the <see cref="ByteOrder"/> enum values to test.
        /// </param>
        public static bool IsHostOrder(this ByteOrder order)
        {
            // true: !(true ^ true) or !(false ^ false)
            // false: !(true ^ false) or !(false ^ true)
            return !(BitConverter.IsLittleEndian ^ (order == ByteOrder.Little));
        }

        /// <summary>
        /// Converts the order of elements in the specified byte array to
        /// host (this computer architecture) byte order.
        /// </summary>
        /// <returns>
        ///   <para>
        ///   An array of <see cref="byte"/> converted from
        ///   <paramref name="source"/>.
        ///   </para>
        ///   <para>
        ///   <paramref name="source"/> if the number of elements in
        ///   it is less than 2 or <paramref name="sourceOrder"/> is
        ///   same as host byte order.
        ///   </para>
        /// </returns>
        /// <param name="source">
        /// An array of <see cref="byte"/> to convert.
        /// </param>
        /// <param name="sourceOrder">
        ///   <para>
        ///   One of the <see cref="ByteOrder"/> enum values.
        ///   </para>
        ///   <para>
        ///   It specifies the order of elements in <paramref name="source"/>.
        ///   </para>
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public static byte[] ToHostOrder(this byte[] source, ByteOrder sourceOrder)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (source.Length < 2)
                return source;

            if (sourceOrder.IsHostOrder())
                return source;

            return (byte[])source.Reverse();
        }

        internal static ushort ToUInt16(this byte[] source, ByteOrder sourceOrder)
        {
            var val = source.ToHostOrder(sourceOrder);

            return BitConverter.ToUInt16(val, 0);
        }

        internal static ulong ToUInt64(this byte[] source, ByteOrder sourceOrder)
        {
            var val = source.ToHostOrder(sourceOrder);

            return BitConverter.ToUInt64(val, 0);
        }

        internal static byte[] ToByteArray(this Stream stream)
        {
            stream.Position = 0;

            using (var buff = new MemoryStream())
            {
                stream.CopyTo(buff, 1024);
                buff.Close();

                return buff.ToArray();
            }
        }

        internal static byte[] ToByteArray(this ushort value, ByteOrder order)
        {
            var ret = BitConverter.GetBytes(value);

            if (!order.IsHostOrder())
                Array.Reverse(ret);

            return ret;
        }

        internal static byte[] ToByteArray(this ulong value, ByteOrder order)
        {
            var ret = BitConverter.GetBytes(value);

            if (!order.IsHostOrder())
                Array.Reverse(ret);

            return ret;
        }

        internal static byte[] ToByteArray(this uint value, ByteOrder order)
        {
            var ret = BitConverter.GetBytes(value);

            if (!order.IsHostOrder())
                Array.Reverse(ret);

            return ret;
        }


        /// <summary>
        /// Retrieves a sub-array from the specified array. A sub-array starts at
        /// the specified index in the array.
        /// </summary>
        /// <returns>
        /// An array of T that receives a sub-array.
        /// </returns>
        /// <param name="array">
        /// An array of T from which to retrieve a sub-array.
        /// </param>
        /// <param name="startIndex">
        /// An <see cref="int"/> that specifies the zero-based index in the array
        /// at which retrieving starts.
        /// </param>
        /// <param name="length">
        /// An <see cref="int"/> that specifies the number of elements to retrieve.
        /// </param>
        /// <typeparam name="T">
        /// The type of elements in the array.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <para>
        ///   <paramref name="startIndex"/> is less than zero.
        ///   </para>
        ///   <para>
        ///   -or-
        ///   </para>
        ///   <para>
        ///   <paramref name="startIndex"/> is greater than the end of the array.
        ///   </para>
        ///   <para>
        ///   -or-
        ///   </para>
        ///   <para>
        ///   <paramref name="length"/> is less than zero.
        ///   </para>
        ///   <para>
        ///   -or-
        ///   </para>
        ///   <para>
        ///   <paramref name="length"/> is greater than the number of elements from
        ///   <paramref name="startIndex"/> to the end of the array.
        ///   </para>
        /// </exception>
        public static T[] SubArray<T>(this T[] array, int startIndex, int length)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            var len = array.Length;

            if (len == 0)
            {
                if (startIndex != 0)
                    throw new ArgumentOutOfRangeException("startIndex");

                if (length != 0)
                    throw new ArgumentOutOfRangeException("length");

                return array;
            }

            if (startIndex < 0 || startIndex >= len)
                throw new ArgumentOutOfRangeException("startIndex");

            if (length < 0 || length > len - startIndex)
                throw new ArgumentOutOfRangeException("length");

            if (length == 0)
                return new T[0];

            if (length == len)
                return array;

            var ret = new T[length];

            Array.Copy(array, startIndex, ret, 0, length);

            return ret;
        }

        /// <summary>
        /// Retrieves a sub-array from the specified array. A sub-array starts at
        /// the specified index in the array.
        /// </summary>
        /// <returns>
        /// An array of T that receives a sub-array.
        /// </returns>
        /// <param name="array">
        /// An array of T from which to retrieve a sub-array.
        /// </param>
        /// <param name="startIndex">
        /// A <see cref="long"/> that specifies the zero-based index in the array
        /// at which retrieving starts.
        /// </param>
        /// <param name="length">
        /// A <see cref="long"/> that specifies the number of elements to retrieve.
        /// </param>
        /// <typeparam name="T">
        /// The type of elements in the array.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <para>
        ///   <paramref name="startIndex"/> is less than zero.
        ///   </para>
        ///   <para>
        ///   -or-
        ///   </para>
        ///   <para>
        ///   <paramref name="startIndex"/> is greater than the end of the array.
        ///   </para>
        ///   <para>
        ///   -or-
        ///   </para>
        ///   <para>
        ///   <paramref name="length"/> is less than zero.
        ///   </para>
        ///   <para>
        ///   -or-
        ///   </para>
        ///   <para>
        ///   <paramref name="length"/> is greater than the number of elements from
        ///   <paramref name="startIndex"/> to the end of the array.
        ///   </para>
        /// </exception>
        public static T[] SubArray<T>(this T[] array, long startIndex, long length)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            var len = array.LongLength;

            if (len == 0)
            {
                if (startIndex != 0)
                    throw new ArgumentOutOfRangeException("startIndex");

                if (length != 0)
                    throw new ArgumentOutOfRangeException("length");

                return array;
            }

            if (startIndex < 0 || startIndex >= len)
                throw new ArgumentOutOfRangeException("startIndex");

            if (length < 0 || length > len - startIndex)
                throw new ArgumentOutOfRangeException("length");

            if (length == 0)
                return new T[0];

            if (length == len)
                return array;

            var ret = new T[length];

            Array.Copy(array, startIndex, ret, 0, length);

            return ret;
        }

        public static T To<T>(this byte[] source, ByteOrder sourceOrder) where T : struct
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (source.Length == 0)
            {
                return default(T);
            }

            Type typeFromHandle = typeof(T);
            byte[] value = source.ToHostOrder(sourceOrder);
            return ((object)typeFromHandle == typeof(bool)) ? ((T)(object)BitConverter.ToBoolean(value, 0)) : (((object)typeFromHandle == typeof(char)) ? ((T)(object)BitConverter.ToChar(value, 0)) : (((object)typeFromHandle == typeof(double)) ? ((T)(object)BitConverter.ToDouble(value, 0)) : (((object)typeFromHandle == typeof(short)) ? ((T)(object)BitConverter.ToInt16(value, 0)) : (((object)typeFromHandle == typeof(int)) ? ((T)(object)BitConverter.ToInt32(value, 0)) : (((object)typeFromHandle == typeof(long)) ? ((T)(object)BitConverter.ToInt64(value, 0)) : (((object)typeFromHandle == typeof(float)) ? ((T)(object)BitConverter.ToSingle(value, 0)) : (((object)typeFromHandle == typeof(ushort)) ? ((T)(object)BitConverter.ToUInt16(value, 0)) : (((object)typeFromHandle == typeof(uint)) ? ((T)(object)BitConverter.ToUInt32(value, 0)) : (((object)typeFromHandle == typeof(ulong)) ? ((T)(object)BitConverter.ToUInt64(value, 0)) : default(T))))))))));
        }
    }
}

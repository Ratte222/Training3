using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace AuxiliaryLib.Extensions
{
    //https://github.com/khalidabuhakmeh/Compressing
    public static class Compression
    {
        public static async Task<byte[]> ToCompressedStringAsync(byte[] bytes,
                                                                                Func<Stream, Stream> createCompressionStream)
        {
            await using var input = new MemoryStream(bytes);

            return await ToCompressedStringAsync(input, createCompressionStream);
        }

        public static async Task<byte[]> ToCompressedStringAsync(Stream input,
                                                                                Func<Stream, Stream> createCompressionStream)
        {
            await using var output = new MemoryStream();
            await using var stream = createCompressionStream(output);

            await input.CopyToAsync(stream);
            await stream.FlushAsync();

            var result = output.ToArray();

            return result;
        }

        public static async Task<byte[]> FromCompressedStringAsync(byte[] bytes, Func<Stream, Stream> createDecompressionStream)
        {
            await using var input = new MemoryStream(bytes);
            return await FromCompressedStringAsync(input, createDecompressionStream);
        }

        public static async Task<byte[]> FromCompressedStringAsync(Stream input, Func<Stream, Stream> createDecompressionStream)
        {
            await using var output = new MemoryStream();
            await using var stream = createDecompressionStream(input);

            await stream.CopyToAsync(output);
            await output.FlushAsync();

            return output.ToArray();
        }

        public static async Task<string> ToGzipAsync(this string value, Encoding encoding, CompressionLevel level = CompressionLevel.Fastest)
            => Convert.ToBase64String((await ToCompressedStringAsync(encoding.GetBytes(value), s => new GZipStream(s, level))));



        public static async Task<string> ToBrotliAsync(this string value, Encoding encoding, CompressionLevel level = CompressionLevel.Fastest)
            => Convert.ToBase64String((await ToCompressedStringAsync(encoding.GetBytes(value), s => new BrotliStream(s, level))));



        public static async Task<string> FromGzipAsync(this string value, Encoding encoding)
            => encoding.GetString((await FromCompressedStringAsync(Convert.FromBase64String(value), s => new GZipStream(s, CompressionMode.Decompress))));

        public static async Task<string> FromBrotliAsync(this string value, Encoding encoding)
            => encoding.GetString((await FromCompressedStringAsync(Convert.FromBase64String(value), s => new BrotliStream(s, CompressionMode.Decompress))));
    }
    
}

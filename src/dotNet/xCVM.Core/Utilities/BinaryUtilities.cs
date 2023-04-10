namespace xCVM.Core.Utilities
{
    public static class BinaryUtilities
    {
        /// <summary>
        /// ASCII Chars.
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(params char[] chars)
        {
            byte[] bytes = new byte[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                bytes[i] = (byte)chars[i];
            }
            return bytes;
        }
    }
}

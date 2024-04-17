using System;

namespace SpatiumInteractive.Libraries.Unity.Platform.Dtos
{
    public class ReadBytesInCoroutineResult
    {
        public readonly bool successful;
        public readonly byte[] data;
        public readonly Exception reason;

        private ReadBytesInCoroutineResult(bool successful, byte[] data, Exception reason)
        {
            this.successful = successful;
            this.data = data;
            this.reason = reason;
        }

        public static ReadBytesInCoroutineResult Success(byte[] data)
        {
            return new ReadBytesInCoroutineResult(true, data, null);
        }

        public static ReadBytesInCoroutineResult Failure(Exception reason)
        {
            return new ReadBytesInCoroutineResult(false, null, reason);
        }

    }
}

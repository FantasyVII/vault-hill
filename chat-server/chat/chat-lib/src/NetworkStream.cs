using System.IO;

namespace ChatLib
{
    class NetworkStream
    {
        static MemoryStream wms;
        static BinaryWriter bw;

        static MemoryStream rms;
        static BinaryReader br;

        static bool writerPrepared = false;
        static bool readerPrepared = false;

        public static void BeginWrite()
        {
            if (writerPrepared)
                throw new System.Exception("BeginWrite was already called. Call EndWrite before calling BeginWrite again.");

            wms = new MemoryStream();
            bw = new BinaryWriter(wms);
            writerPrepared = true;
        }

        static void BeginWriteError()
        {
            if (!writerPrepared)
                throw new System.Exception("BeginWrite must be called before Write");
        }

        public static void Write(bool value) { BeginWriteError(); bw.Write(value); }
        public static void Write(byte value) { BeginWriteError(); bw.Write(value); }
        public static void Write(sbyte value) { BeginWriteError(); bw.Write(value); }
        public static void Write(char value) { BeginWriteError(); bw.Write(value); }
        public static void Write(short value) { BeginWriteError(); bw.Write(value); }
        public static void Write(ushort value) { BeginWriteError(); bw.Write(value); }
        public static void Write(int value) { BeginWriteError(); bw.Write(value); }
        public static void Write(uint value) { BeginWriteError(); bw.Write(value); }
        public static void Write(long value) { BeginWriteError(); bw.Write(value); }
        public static void Write(ulong value) { BeginWriteError(); bw.Write(value); }
        public static void Write(float value) { BeginWriteError(); bw.Write(value); }
        public static void Write(double value) { BeginWriteError(); bw.Write(value); }
        public static void Write(string value) { BeginWriteError(); bw.Write(value); }

        public static byte[] EndWrite()
        {
            if (!writerPrepared)
                throw new System.Exception("EndWrite cannot be called before BeginWrite.");

            writerPrepared = false;
            return wms.ToArray();
        }

        //-----------------------------------------------------------------------------------------------------------

        public static void BeginRead(byte[] buffer)
        {
            if (readerPrepared)
                throw new System.Exception("BeginRead was already called. Call EndRead before calling BeginRead again.");

            rms = new MemoryStream(buffer);
            br = new BinaryReader(rms);
            readerPrepared = true;
        }

        static void BeginReadError()
        {
            if (!readerPrepared)
                throw new System.Exception("BeginRead must be called before Read.");
        }

        public static bool ReadBoolean() { BeginReadError(); return br.ReadBoolean(); }
        public static byte ReadByte() { BeginReadError(); return br.ReadByte(); }
        public static sbyte ReadSByte() { BeginReadError(); return br.ReadSByte(); }
        public static char ReadChar() { BeginReadError(); return br.ReadChar(); }
        public static short ReadInt16() { BeginReadError(); return br.ReadInt16(); }
        public static ushort ReadUInt16() { BeginReadError(); return br.ReadUInt16(); }
        public static int ReadInt32() { BeginReadError(); return br.ReadInt32(); }
        public static uint ReadUInt32() { BeginReadError(); return br.ReadUInt32(); }
        public static long ReadInt64() { BeginReadError(); return br.ReadInt64(); }
        public static ulong ReadUInt64() { BeginReadError(); return br.ReadUInt64(); }
        public static float ReadSingle() { BeginReadError(); return br.ReadSingle(); }
        public static double ReadDouble() { BeginReadError(); return br.ReadDouble(); }
        public static string ReadString() { BeginReadError(); return br.ReadString(); }

        public static void EndRead()
        {
            if (!readerPrepared)
                throw new System.Exception("EndRead cannot be called before BeginRead.");

            readerPrepared = false;
            rms.Dispose();
            br.Dispose();
        }
    }
}
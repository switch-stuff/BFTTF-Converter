using System.IO;
using System.Linq;

namespace BFTTFConverter
{
    internal class Program
    {
        public static uint Key = 0x06186249;

        public static uint Flip(uint In)
        {
            return ((In & 0x000000ff) << 24) +
                   ((In & 0x0000ff00) <<  8) +
                   ((In & 0x00ff0000) >>  8) +
                   ((In & 0xff000000) >> 24);
        }

        public static byte[] Xor(BinaryReader Input, bool Encode)
        {
            var MemStrm = new MemoryStream();
            var Output  = new BinaryWriter(MemStrm);

            if (!Encode)
            {
                Input.BaseStream.Position += 8;
                for (int i = 0; i < ((int)Input.BaseStream.Length - 8) / 4; i++)
                {
                    Output.Write(Input.ReadUInt32() ^ Key);
                }
            }
            else
            {
                Output.Write(0x18029a7f ^ Key);
                Output.Write(Flip((uint)Input.BaseStream.Length) ^ Key);
                for (int i = 0; i < (int)Input.BaseStream.Length / 4; i++)
                {
                    Output.Write(Input.ReadUInt32() ^ Key);
                }
            }

            Output.Close();
            return MemStrm.ToArray();
        }

        private static void Main(string[] args)
        {
            var FileArgs = args[0].Split('.');
            if (FileArgs[1] == "bfttf")
            {
                var Rd = new BinaryReader(File.OpenRead(args[0]));
                File.WriteAllBytes($"{FileArgs[0]}.ttf", Xor(Rd, false));
            }
            else if (FileArgs[1] == "ttf")
            {
                var Rd = new BinaryReader(File.OpenRead(args[0]));
                File.WriteAllBytes($"{FileArgs[0]}.bfttf", Xor(Rd, true));
            }
        }
    }
}
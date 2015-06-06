using GameFormatReader.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIReadout
{
    public class DebugMenuModifier
    {
        public class EntryHeader
        {
            /* Actual Layout of Struct on Disk: */
            // 0x28/40 bytes long
            // 0x00 - string Name;
            // 0x20 - short SubOptionsCount;
            // 0x22 - short Padding; //Always 0x0000
            // 0x24 - int SubOptionsOffset;

            public string Name;
            public List<EntrySubOption> SubOptions { get; private set; }

            public EntryHeader()
            {
                SubOptions = new List<EntrySubOption>();
            }

            public void Load(EndianBinaryReader stream)
            {
                // Store the position of the Stream so we can reset to that position + size of our EntryHeader
                long streamPos = stream.BaseStream.Position;

                Name = Encoding.GetEncoding("shift-jis").GetString(stream.ReadBytesUntil(0));
                stream.BaseStream.Position = streamPos + 0x20;

                short subOptionCount = stream.ReadInt16();
                short padding = stream.ReadInt16();

                if (padding != 00) throw new Exception("Padding is not zero here. What gives?");

                Console.WriteLine("Header Name: {0} SubOptionCount: {1}", Name, subOptionCount);

                int subOptionOffset = stream.ReadInt32();
                stream.BaseStream.Position = subOptionOffset;

                for (int i = 0; i < subOptionCount; i++)
                {
                    EntrySubOption subOption = new EntrySubOption();
                    subOption.Load(stream);
                    SubOptions.Add(subOption);
                }

                // Reset the stream position to the start of the struct + size of struct since the sub-options move the stream header around a lot.
                streamPos += 0x28;
                stream.BaseStream.Position = streamPos;
            }

            public void Save(EndianBinaryWriter stream, ref long subEntryOffset)
            {
                // Write the 20 characters of the Name. Convert from shift-jis encoded to bytes, and then write the first 0x20 bytes.
                long streamPos = stream.BaseStream.Position;
                byte[] encodedName = Encoding.GetEncoding("shift-jis").GetBytes(Name);
                for(int i = 0; i < 0x1F; i++)
                {
                    if (i < encodedName.Length)
                        stream.Write((byte)encodedName[i]);
                    else
                        stream.Write((byte)0);
                }

                // Force a null terminator
                stream.Write((byte)0);

                // Write the number of sub-options and padding byte.
                stream.Write((short)SubOptions.Count);
                stream.Write((short)0); // Padding
                stream.Write((int)subEntryOffset); // Sub Option Offset

        
                stream.BaseStream.Position = subEntryOffset;

                for(int i = 0; i < SubOptions.Count; i++)
                {
                    EntrySubOption subOption = SubOptions[i];

                    subOption.Save(stream);

                    // Advance the sub-entry offset that the next one will write to
                    subEntryOffset += 0x2c;
                }

                stream.BaseStream.Position = streamPos + 0x28;
            }
        }

        public class EntrySubOption
        {
            /* Actual Layout of Struct on Disk: */
            // 0x2C/44 bytes long
            // 0x00 - string Name
            // 0x21 - string MapName
            // 0x29 - byte RoomNumber
            // 0x2A - byte SpawnPointId
            // 0x2B byte LoadedLayerId

            public string Name;
            public string MapName;
            public byte RoomNumber;
            public byte SpawnPointId;
            public byte LoadedLayerId; // Speculation, untested by Gamma is confident. I agree that it makes sense to have it.

            public void Load(EndianBinaryReader stream)
            {
                long streamPos = stream.BaseStream.Position;

                Name = Encoding.GetEncoding("shift-jis").GetString(stream.ReadBytesUntil(0));
                stream.BaseStream.Position = streamPos + 0x21;

                MapName = Encoding.GetEncoding("shift-jis").GetString(stream.ReadBytesUntil(0));
                stream.BaseStream.Position = streamPos + 0x29;

                RoomNumber = stream.ReadByte();
                SpawnPointId = stream.ReadByte();
                LoadedLayerId = stream.ReadByte();

                Console.WriteLine("SubOption Name: {0} MapName: {1} RoomNumber: {2} SpawnPointId: {3} LoadedLayerId: {4}", Name, MapName, RoomNumber, SpawnPointId, LoadedLayerId);
            }

            public void Save(EndianBinaryWriter stream)
            {
                // Write 20 bytes for Name (and then force null terminator) from shift-jis encoded.
                byte[] encodedName = Encoding.GetEncoding("shift-jis").GetBytes(Name);
                for(int i = 0; i < 0x20; i++)
                {
                    if (i < encodedName.Length)
                        stream.Write((byte)encodedName[i]);
                    else
                        stream.Write((byte)0);
                }

                stream.Write((byte)0); // Force null terminator

                byte[] encodedMapName = Encoding.GetEncoding("shift-jis").GetBytes(MapName);
                for(int i = 0; i < 0x7; i++)
                {
                    if (i < encodedMapName.Length)
                        stream.Write((byte)encodedMapName[i]);
                    else
                        stream.Write((byte)0);
                }

                stream.Write((byte)0); // Force null terminator.

                stream.Write(RoomNumber);
                stream.Write(SpawnPointId);
                stream.Write(LoadedLayerId);
            }
        }

        public List<EntryHeader> Entries { get; private set; }

        public DebugMenuModifier()
        {
            Entries = new List<EntryHeader>();
        }

        public void Load(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("File not specfied.");
            if(!File.Exists(fileName))
                throw new FileNotFoundException("File not found.");

            EndianBinaryReader reader = new EndianBinaryReader(new FileStream(fileName, FileMode.Open), Endian.Big);
            Load(reader);
        }

        public void Load(EndianBinaryReader stream)
        {
            if (stream == null || stream.BaseStream.Length == 0)
                throw new ArgumentException("Null or empty stream specified.");

            // Read the header
            byte entryCount = stream.ReadByte();
            byte padding = stream.ReadByte();
            stream.ReadByte();
            stream.ReadByte();

            // Offset to the first entry which is right after the header (ie: 0x08)
            int offsetToFirstEntry = stream.ReadInt32();

            // Load the individual options, then their sub options.
            stream.BaseStream.Position = offsetToFirstEntry;
            for(int o = 0; o < entryCount; o++)
            {
                EntryHeader entry = new EntryHeader();
                entry.Load(stream);

                Entries.Add(entry);
            }
        }

        public void Save(string fileName)
        {
            FileStream stream = File.Open(fileName, FileMode.OpenOrCreate);

            EndianBinaryWriter writer = new EndianBinaryWriter(stream, Endian.Big);
            Save(writer);
        }

        public void Save(EndianBinaryWriter stream)
        {
            // Save the Header 
            stream.Write((byte)Entries.Count);
            stream.Write((byte)0x5E); // Unknown
            stream.Write((byte)0); // Padding
            stream.Write((byte)0x61); // Unknown

            // Offset to the first entry
            stream.Write((int)stream.BaseStream.Position + 0x4);


            // Pre-allocate the offset to the sub-entries based on the size of the EntryHeader and the # of entryheaders.
            long subEntryOffsetStart = stream.BaseStream.Position + (Entries.Count * 0x28);

            // Write the options.
            for(int i = 0; i < Entries.Count; i++)
            {
                EntryHeader entry = Entries[i];
                entry.Save(stream, ref subEntryOffsetStart);
            }

            // Pad up to a 32 byte alignment Formula:
            // (x + (n-1)) & ~(n-1)
            long nextAligned = (stream.BaseStream.Length + 0x1F) & ~0x1F;

            long delta = nextAligned - stream.BaseStream.Length;
            stream.BaseStream.Position = stream.BaseStream.Length;
            stream.Write(new byte[delta]);
        }
    }
}

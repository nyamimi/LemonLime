﻿using LemonLime.ARM;
using LemonLime.Common;
using LemonLime.CTR.IO;

namespace LemonLime.CTR
{
    class Memory : IBus
    {
        private CPUHandler CPU;

        private IOHandler IO;

        private BootROM.ARM9 BootROM9;

        private CPUType Type;

        private byte[] DataTCM = new byte[0x00004000];

        public Memory()
        {
            BootROM9 = new BootROM.ARM9();

            IO = new IOHandler();
        }

        public void SetType(CPUType Type)
        {
            this.Type = Type;
        }

        public void SetHandler(CPUHandler CPU)
        {
            this.CPU = CPU;
        }

        public byte ReadUInt8(uint Address)
        {
            if (Address >= 0x10000000 && Address < 0x10000000 + 0x08000000)
            {
                IOData IOInfo = new IOData(CPU, Address, IOType.Read, 1);
                IO.Call(IOInfo);
                return IOInfo.Read8;
            }
            else if (Address >= 0xFFF00000 && Address < 0xFFF00000 + 0x00004000)
            {
                return DataTCM[Address - 0xFFF00000];
            }
            else if (Address >= 0xFFFF0000)
            {
                return BootROM9.BootROM[Address - 0xFFFF0000];
            }

            Logger.WriteInfo($"Read @ 0x{Address.ToString("X")}");

            return 0;
        }

        public ushort ReadUInt16(uint Address)
        {
            if (Address >= 0x10000000 && Address < 0x10000000 + 0x08000000)
            {
                IOData IOInfo = new IOData(CPU, Address, IOType.Read, 2);
                IO.Call(IOInfo);
                return IOInfo.Read16;
            }

            return (ushort)(ReadUInt8(Address) |
                (ReadUInt8(Address + 1) << 8));
        }

        public uint ReadUInt32(uint Address)
        {
            if (Address >= 0x10000000 && Address < 0x10000000 + 0x08000000)
            {
                IOData IOInfo = new IOData(CPU, Address, IOType.Read, 4);
                IO.Call(IOInfo);
                return IOInfo.Read32;
            }

            return (uint)(ReadUInt8(Address)   |
                (ReadUInt8(Address + 1) << 8)  |
                (ReadUInt8(Address + 2) << 16) |
                (ReadUInt8(Address + 3) << 24));
        }

        public void WriteUInt8(uint Address, byte Value)
        {
            if (Address >= 0x10000000 && Address < 0x10000000 + 0x08000000)
            {
                IO.Call(new IOData(CPU, Address, IOType.Write, 1, Value));
                return;
            }
            else if (Address >= 0xFFF00000 && Address < 0xFFF00000 + 0x00004000)
            {
                DataTCM[Address - 0xFFF00000] = Value;
                return;
            }

            Logger.WriteInfo($"Write @ 0x{Address.ToString("X")}, Value = {Value.ToString("X")}");
        }

        public void WriteUInt16(uint Address, ushort Value)
        {
            if (Address >= 0x10000000 && Address < 0x10000000 + 0x08000000)
            {
                IO.Call(new IOData(CPU, Address, IOType.Write, 2, 0, Value));
                return;
            }

            WriteUInt8(Address,     (byte)Value);
            WriteUInt8(Address + 1, (byte)(Value >> 8));
        }

        public void WriteUInt32(uint Address, uint Value)
        {
            if (Address >= 0x10000000 && Address < 0x10000000 + 0x08000000)
            {
                IO.Call(new IOData(CPU, Address, IOType.Write, 4, 0, 0, Value));
                return;
            }

            WriteUInt8(Address,     (byte)Value);
            WriteUInt8(Address + 1, (byte)(Value >> 8));
            WriteUInt8(Address + 2, (byte)(Value >> 16));
            WriteUInt8(Address + 3, (byte)(Value >> 24));
        }
    }
}

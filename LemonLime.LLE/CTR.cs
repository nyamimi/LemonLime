﻿namespace LemonLime.LLE
{
    public class CTR
    {
        private Memory      Memory;
        private CPU.Handler Handler;

        public CTR()
        {
            Memory  = new Memory();
            Handler = new CPU.Handler(Memory, Memory);

            CPU.Handler.EnableCpu(CPU.Type.Arm9, true); // Enable ARM9 CPU

            IO.Handler io = new IO.Handler();

            Map memmap = new Map(0x0, 0x0, null, io.Call);
        }

        public void Run()
        {
            Handler.Start();
        }
    }
}

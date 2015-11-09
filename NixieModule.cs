using System;
using Microsoft.SPOT.Hardware;

namespace Nixie
{
    public class NixieModule
    {
        private readonly int _sections;
        private readonly PWM _oePort;
        private readonly OutputPort _shiftPort;
        private readonly OutputPort _storePort;
        private readonly OutputPort _dataInPort;

        private ushort[] _data;

        public enum Colour
        {
            White = 0,
            Yellow = 1,
            Cyan = 2,
            Green = 3,
            Magenta = 4,
            Red = 5,
            Blue = 6,
            Black = 7
        };

        public enum Spacer
        {
            Off = 0,
            SingleQuote = 1,
            Dot = 2,
            Colon = 3
        }

        public enum Number
        {
            Off = 0,
            One = 1,
            Two = 2,
            Three = 4,
            Four = 8,
            Five = 16,
            Six = 32,
            Seven = 64,
            Eight = 128,
            Nine = 256,
            Zero = 512
        }

        public NixieModule(OutputPort dataInPort, OutputPort stopPort, OutputPort shiftPort, PWM oePort, int sections = 1)
        {
            _dataInPort = dataInPort;
            _storePort = stopPort;
            _shiftPort = shiftPort;
            _oePort = oePort;
            _sections = sections;
            _data = new ushort[sections];
            for (int i = 0; i < sections; i++)
            {
                SetBackgroundColour(Colour.White, i + 1);
                SetNumber(Number.Off, i + 1);
                SetSpacer(Spacer.Off, i + 1);
            }
        }

        private void Shift()
        {
            _shiftPort.Write(false);
            _shiftPort.Write(true);
        }

        private void Store()
        {
            _storePort.Write(false);
            _storePort.Write(true);
        }

        private void WriteSection(ushort data)
        {
            var d = data;
            for (int i = 0; i < 16; i++)
            {
                _dataInPort.Write((d & 0x8000) == 0x8000);
                Shift();
                d = (ushort)(d << 1);
            }
        }
        public void Display()
        {
            for (int i = 0; i < _sections; i++)
            {
                WriteSection(_data[i]);
                Store();
            }
        }

        public void SetNumber(Number number, int section = 1)
        {
            _data[section - 1] = (ushort)((_data[section - 1] & 0xFC00) | (ushort)number);
        }

        public void SetBackgroundColour(Colour colour, int section = 1)
        {
            _data[section - 1] = (ushort)((_data[section - 1] & 0x8FFF) | ((ushort)colour << 12));
        }

        public void SetSpacer(Spacer spacer, int section = 1)
        {
            _data[section - 1] = (ushort)((_data[section - 1] & 0xF3FF) | ((ushort)spacer << 10));
        }
    }
}

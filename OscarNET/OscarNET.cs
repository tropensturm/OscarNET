using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OscarNET
{
    public class Oscar
    {
        public StringBuilder Log { get; private set; }

        //# Reference: patent US4550247
        private static readonly Dictionary<int, int> CODES = new Dictionary<int, int>()
        {
            { 0b0100110, 0x0 },
            { 0b0101010, 0x1 },
            { 0b0110010, 0x2 },
            { 0b0110110, 0x3 },
            { 0b1011010, 0x4 },
            { 0b1010010, 0x5 },
            { 0b1010110, 0x6 },
            { 0b1001010, 0x7 },
            { 0b0100101, 0x8 },
            { 0b0101101, 0x9 },
            { 0b0101001, 0xA },
            { 0b0110101, 0xB },
            { 0b1011001, 0xC },
            { 0b1010101, 0xD },
            { 0b1001101, 0xE },
            { 0b1001001, 0xF },
            { 0b1111111, 0x10 }
        };

        private static readonly int CANDLINES_MINIMUMBLOCKSIZE = 20;

        public double Threshold { get; set; }
        public int Bandwidth { get; set; } = CANDLINES_MINIMUMBLOCKSIZE;
        public int[] ComputedLines { get; private set; }
        public int[] CandidateIndexes { get; private set; }
        public Pix BitmapPix { get; private set; }
        public List<Barcode> Barcodes { get; private set; }
        private int[] Symbols { get; set; }
        
        public Oscar(Pix pix)
        {
            Log = new StringBuilder();
            Barcodes = new List<Barcode>();
            BitmapPix = pix;

            //candidates = lines with at least 20% black
            Threshold = (255 * (BitmapPix.Stride - BitmapPix.Padding)) * Barcode.LUMINACE_FACTOR;
        }

        public Oscar(string symbols)
        {
            Log = new StringBuilder();
            Barcodes = new List<Barcode>();
            Symbols = symbols.Select(x => int.Parse(x.ToString())).ToArray();
        }

        public string Execute()
        {
            List<int[]> dataList = null;

            if (BitmapPix != null)
            {
                List<Barcode> blocks = GetBlocks();

                List<Barcode.Pulse[]> pulsesList = new List<Barcode.Pulse[]>(blocks.Count());
                foreach (var block in blocks)
                {
                    pulsesList.Add(block.ComputePulses());
                }

                List<int[]> nibblesList = new List<int[]>(pulsesList.Count());
                foreach (var pulses in pulsesList)
                {
                    var symbols = PulsesToSymbols(pulses);
                    nibblesList.Add(SymbolsToNibbles(symbols));
                }

                dataList = new List<int[]>(nibblesList.Count());
                foreach (var nibbles in nibblesList)
                {
                    var res = ParseNibbles(nibbles);
                    if (res != null)
                        dataList.Add(res);
                }
            }
            else if(Symbols != null)
            {
                var nibbles = SymbolsToNibbles(Symbols);

                dataList = new List<int[]>(nibbles.Count());
                var res = ParseNibbles(nibbles);
                if (res != null)
                    dataList.Add(res);
            }

            StringBuilder sb = new StringBuilder();
            foreach (var r in dataList)
            {
                if (r == null) continue;
                var hex = r.Select(x => x.ToString("x2")).Aggregate((a, b) => a + b);
                sb.AppendLine(hex);
            }

            return sb.ToString();
        }

        public List<Barcode> GetBlocks()
        {
            // calculate (color) value for each line and pick thoese that are below threshold
            ComputedLines = BitmapPix.EvaluateAllLines().ToArray();
            CandidateIndexes = Enumerable.
                                Range(0, ComputedLines.Count()).
                                Where(x => ComputedLines[x] < Threshold
                                ).ToArray();

            Barcodes = new List<Barcode>();

            while (CandidateIndexes.Count() > 0)
            {
                // 'take while' will abort when condition is not met
                var block = CandidateIndexes.TakeWhile((x, idx) =>
                            (idx > 0 && CandidateIndexes[idx - 1] == x - 1
                            || idx == 0
                            )).ToArray();

                if (block.Count() >= Bandwidth)
                {
                    var candline = Enumerable.Range(block.First(), block.Count()).Select(x => BitmapPix.GetIntLine(x)).ToList();
                    Barcode item = new Barcode(candline, BitmapPix.BytesPerPixel);
                    item.BitmapPadding = BitmapPix.Padding;
                    Barcodes.Add(item);
                }

                CandidateIndexes = CandidateIndexes.Except(block).ToArray();
            }

            return Barcodes;
        }

        public int[] SymbolsToNibbles(int[] symbols)
        {
            var symbolList = symbols.AsEnumerable();
            List<int> nibbles = new List<int>();

            while (symbolList.Count() > 0)
            {
                var bytes = symbolList.Take(7);
                symbolList = symbolList.Skip(7);
                int bit7 = 0;
                foreach (var b in bytes)
                {
                    bit7 = bit7 << 1;
                    bit7 += b;
                }

                var validCode = CODES.ContainsKey((byte)bit7);
                if(!validCode)
                {
                    if (symbolList.Count() < 2)
                        break;

                    Log.AppendLine("ERROR: code unknown " + bit7.ToString() + " - " + string.Join("", bytes));
                }
                else
                {
                    nibbles.Add(CODES[(byte)bit7]);
                }
            }

            return nibbles.ToArray();
        }

        public int[] PulsesToSymbols(Barcode.Pulse[] pulses)
        {
            // adjustment factors for "learning" norm array
            var factor_learn = 0d;
            double adjustment = 0;
           
            List<int> symbols = new List<int>();

            // the first bars (in every line) from oscar barcode exists to measure their width
            var currentsymbol = 1; // 1 == black 0 == white
            var norm = new double[] { (pulses[1].Length + pulses[3].Length + pulses[5].Length ) / 3.0d,
                                      (pulses[0].Length + pulses[2].Length + pulses[4].Length ) / 3.0d };

            foreach (var p in pulses)
            {
                var isBlack = p.IsBlack;
                var pulse = p.Length;
                var guess = 1;

                if (pulse < norm[currentsymbol] * 1.5)
                    guess = 1;
                else if (pulse > norm[currentsymbol] * 5)
                    guess = 8; // max is EOL marker + last bit == 1
                else
                    guess = 2;

                if (guess < 3)
                    adjustment = pulse / guess;

                norm[currentsymbol] = (norm[currentsymbol] + adjustment * factor_learn) / (1 + factor_learn);

                if (guess < 3)
                    foreach (var k in Enumerable.Range(0, guess))
                    {
                        symbols.Add(isBlack ? 1 : 0);
                    }
                else // EOL Marker
                    foreach (var k in Enumerable.Range(0, guess))
                    {
                        symbols.Add(1);
                    }

                currentsymbol ^= 1; // swap 1 to 0 and 0 to 1
            }

            return symbols.ToArray();
        }

        private static readonly string[] FUNCTION =
        {
            "GENERATE TONES",
            "CONTROL ARRAY",
            "RAM FIRMWARE",
            "DATA LINE3",
            "DATA LINE4",
            "DATA LINE5",
            "DATA LINE6",
            "DATA LINE7",
            "ENDFW"
        };

        private static readonly string[] MACHINETYPE =
        {
            "Atari400/800/1200",
            "Commodore",
            "TI99/4",
            "Timex 1000",
            "Timex 2000",
            "RADIO SHACK",
            "RS-232",
            "Commodore no loader",
            "Commodore no loader/no header"
        };

        public int[] ParseNibbles(int[] nibbles)
        {
            var rawlength = nibbles.Length;
            if (nibbles[0] != 0xD)
                throw new System.Exception("nibble is not starting with 0xD: " + nibbles[0].ToString());

            nibbles = nibbles.Skip(1).ToArray(); // remove 0xD

            var isValid = nibbles.Count() % 2 == 0; // ?

            List<int> bytes = new List<int>();
            for (int i = 0; i < nibbles.Length / 2; i++)
            {
                bytes.Add((nibbles[i * 2 + 1] << 4) + nibbles[i * 2]);
            }
            var linenumber = bytes[0];
            var functionbyte = bytes[1];

            string function = string.Empty;
            if(FUNCTION.Count() > functionbyte)
                function = FUNCTION[functionbyte];
            var length = bytes[2]; // == rawlength
            var diff = bytes.Last();

            if(function == FUNCTION[1])
            {
                Log.AppendLine("INFO: Function " + function);
                var machineType = MACHINETYPE[bytes[3] - 1];
                Log.AppendLine("INFO: Machine Type " + machineType);
            }

            var checksum = (int)(bytes.ToArray().Sum() & 0xFF) == 0;
            if (!checksum)
                Log.AppendLine("ERROR: checksum is not zero " + diff.ToString());

            if(function.StartsWith("DATA LINE"))
            {
                bytes.Remove(bytes.Last()); // remove the checksum diff
                return bytes.Skip(3).ToArray();
            }

            return null;
        }
    }
}

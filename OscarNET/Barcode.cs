using System;
using System.Collections.Generic;
using System.Linq;

namespace OscarNET
{
    public class Barcode
    {
        public class AVGLine
        {
            public int Index { get; set; }
            public int AVG { get; set; }
            public bool IsBlack { get; set; }
        }

        public class Pulse
        {
            public int Length { get; set; }
            public bool IsBlack { get; set; }
        }

        public static readonly double LUMINACE_FACTOR = 0.60d;

        public List<int[]> CandidateBlock { get; private set; } // horizontal lines
        public int Bits { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public int MidlineIndex { get; private set; }
        public int Radius { get; private set; }
        public int StartIndex { get; private set; }
        public int EndIndex { get; private set; }
        public int BitmapPadding { get; set; } = 0;
        public bool IsReversedPulse { get; private set; } = false;

        public AVGLine[] AvgLines { get; set; } // vertical lines
        public Pulse[] PulseList { get; set; }
        public int[] Symbols { get; set; }

        public Barcode(List<int[]> candidateBlock, int bits, double radius = 0.25d)
        {
            CandidateBlock = candidateBlock;
            Bits = bits;

            Height = CandidateBlock.Count();

            MidlineIndex = Height / 2;
            Width = CandidateBlock[0].Count();
            Radius = Convert.ToInt32(Math.Floor(Height * radius)); // 10 / 21; //2 / 6;
            StartIndex = MidlineIndex - Radius;
            EndIndex = MidlineIndex + Radius + 1;
        }

        /// <summary>
        /// vertical iteration sum each column and categorize as 
        /// black or white depending on local threshold
        /// </summary>
        /// <param name="startindex">(horizontal) startline</param>
        /// <param name="endindex">(horizontal) endline</param>
        /// <returns></returns>
        public AVGLine[] GetAVGLine()
        {
            List<AVGLine> result = new List<AVGLine>();

            var lines = Enumerable.
                            Range(StartIndex, EndIndex - StartIndex).
                            Select(x => CandidateBlock[x]).
                            ToArray();

            var range = EndIndex - StartIndex;
            var threshold = 255 * range * LUMINACE_FACTOR;

            int pos = 0;
            for (int x = 0; x < Width; x += Bits)
            {
                int sum = 0;
                for (int subX = 0; subX < Bits; ++subX)
                {
                    if (BitmapPadding != 0 && x + subX > Width)
                    {
                        sum += 255; // treat padding as white
                    }
                    else
                    {
                        for (int y = 0; y < range; ++y)
                        {
                            sum += lines[y][x + subX];
                        }
                    }
                }
                sum /= Bits;
                result.Add(new AVGLine() { Index = pos++, AVG = sum, IsBlack = sum < threshold });
            }

            return result.ToArray();
        }

        /// <summary>
        /// creates an array of pulses
        /// measuring the horizontal length of each "bar" and label it as white or black
        /// removes the first and the last bar when it is white
        /// reverses pulse result when eol marker is at the beginning
        /// </summary>
        public Pulse[] ComputePulses()
        {
            AvgLines = GetAVGLine();

            List<Pulse> pulses = new List<Pulse>();

            for (int i = 0; i < AvgLines.Length; ++i)
            {
                var pulse = new Pulse();
                pulse.IsBlack = AvgLines[i].IsBlack;

                var avg = AvgLines[i];
                if (avg.IsBlack)
                {
                    pulse.Length = AvgLines.
                                    Skip(i).
                                    TakeWhile(x =>
                                       x.IsBlack
                                    ).ToArray().Count();

                    pulses.Add(pulse);
                }
                else
                {
                    pulse.Length = AvgLines.
                                    Skip(i).
                                    TakeWhile(x =>
                                        !x.IsBlack
                                    ).ToArray().Count();

                    // we are dropping it when the first is a white pulse
                    if (pulses.Count() > 0)
                        pulses.Add(pulse);
                }

                i = i + pulse.Length - 1;
            }

            if (!pulses.Last().IsBlack)
            {
                pulses.Remove(pulses.Last());
            }

            // end marker in beginning?
            if (pulses.First().Length > pulses.Last().Length)
            {
                pulses.Reverse();
                IsReversedPulse = true;
            }

            PulseList = pulses.ToArray();

            return PulseList;
        }
    }

}

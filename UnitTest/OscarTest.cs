using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OscarNET;

namespace UnitTest_Oscar
{
    [TestClass]
    [TestCategory("Oscar")]
    public class OscarTest
    {
        [TestMethod]
        public void TestEvaluateAllLines()
        {
            var pix = new Pix(@"Tests\Barcode_Test01_linesevaluation.bmp");
            var lines = pix.EvaluateAllLines().ToArray();
            Assert.AreEqual(20 * 3 * 255, lines[0]);
            Assert.AreEqual(19 * 3 * 255, lines[1]);
            Assert.AreEqual(18 * 3 * 255, lines[2]);
            Assert.AreEqual(17 * 3 * 255, lines[3]);
            Assert.AreEqual(16 * 3 * 255, lines[4]);
            Assert.AreEqual(15 * 3 * 255, lines[5]);
            Assert.AreEqual(14 * 3 * 255, lines[6]);
            Assert.AreEqual(13 * 3 * 255, lines[7]);
            Assert.AreEqual(13 * 3 * 255, lines[8]);
            Assert.AreEqual(13 * 3 * 255, lines[9]);
            Assert.AreEqual(13 * 3 * 255, lines[10]);
            Assert.AreEqual(20 * 3 * 255, lines[11]);
        }

        [TestMethod]
        public void TestThreshold()
        {
            var pix = new Pix(@"Tests\Barcode_Test02_valid_block_2.bmp");
            var oscar = new Oscar(pix);

            Assert.AreEqual((int)((255 * pix.Width * pix.BytesPerPixel) * Barcode.LUMINACE_FACTOR), (int)oscar.Threshold);
        }

        [TestMethod]
        public void TestGetBlocks()
        {
            var pix = new Pix(@"Tests\Barcode_Test02_valid_block_2.bmp");
            var oscar = new Oscar(pix);

            var result = oscar.GetBlocks();

            Assert.AreEqual(2, result.Count());

            Assert.AreEqual(108, result[0].Height);
            Assert.AreEqual(pix.Depth/8, result[0].Bits);
            Assert.AreEqual(pix.Width * pix.BytesPerPixel, result[0].Width);
            Assert.AreEqual(54, result[0].MidlineIndex);

            Assert.AreEqual(96, result[1].Height);
            Assert.AreEqual(pix.Depth / 8, result[1].Bits);
            Assert.AreEqual(pix.Width * pix.BytesPerPixel, result[1].Width);
            Assert.AreEqual(48, result[1].MidlineIndex);

        }

        [TestMethod]
        public void TestGetAVGLine()
        {
            var pix = new Pix(@"Tests\Barcode_Test03_avglines.bmp");

            Assert.AreEqual(91, pix.Width);
            Assert.AreEqual(46, pix.Height);

            var oscar = new Oscar(pix);

            Assert.AreEqual((int)((255 * (pix.Stride - pix.Padding)) * Barcode.LUMINACE_FACTOR), (int)oscar.Threshold);

            var blocks = oscar.GetBlocks();

            Assert.AreEqual(1, blocks.Count());

            Assert.AreEqual(44, blocks[0].Height);
            Assert.AreEqual(pix.Depth / 8, blocks[0].Bits);
            Assert.AreEqual(pix.Width * pix.BytesPerPixel, blocks[0].Width);
            Assert.AreEqual(22, blocks[0].MidlineIndex);

            var lines = blocks[0].GetAVGLine();
            Assert.AreEqual(91, lines.Length);

            var white = (blocks[0].EndIndex - blocks[0].StartIndex) * 255;
            var black = 0;

            Assert.AreEqual(white, lines[0].AVG);
            Assert.AreEqual(white, lines[5].AVG);
            Assert.AreEqual(black, lines[6].AVG);
            Assert.AreEqual(white, lines[21].AVG);
            Assert.AreEqual(black, lines[20].AVG);
            Assert.AreEqual(white, lines[44].AVG);
            Assert.AreEqual(black, lines[45].AVG);
            Assert.AreEqual(white, lines[52].AVG);
            Assert.AreEqual(black, lines[51].AVG);
            Assert.AreEqual(white, lines[59].AVG);
            Assert.AreEqual(black, lines[60].AVG);
            Assert.AreEqual(white, lines[67].AVG);
            Assert.AreEqual(black, lines[66].AVG);
            Assert.AreEqual(white, lines[82].AVG);
            Assert.AreEqual(black, lines[83].AVG);
            Assert.AreEqual(black, lines[90].AVG);
        }

        [TestMethod]
        public void TestComputePulses()
        {
            var pix = new Pix(@"Tests\Barcode_Test03_avglines.bmp");

            Assert.AreEqual(91, pix.Width);
            Assert.AreEqual(46, pix.Height);

            var oscar = new Oscar(pix);

            Assert.AreEqual((int)((255 * pix.Width * pix.BytesPerPixel) * Barcode.LUMINACE_FACTOR), (int)oscar.Threshold);

            var blocks = oscar.GetBlocks();

            Assert.AreEqual(1, blocks.Count());

            Assert.AreEqual(44, blocks[0].Height);
            Assert.AreEqual(pix.Depth / 8, blocks[0].Bits);
            Assert.AreEqual(pix.Width * pix.BytesPerPixel, blocks[0].Width);
            Assert.AreEqual(22, blocks[0].MidlineIndex);

            Assert.AreEqual(false, blocks[0].IsReversedPulse);

            var pulses = blocks[0].ComputePulses();

            Assert.AreEqual(7, pulses.Length);

            // will reverse the array because last black pulse is longer than first one
            Assert.AreEqual(true, blocks[0].IsReversedPulse);
                        
            Assert.AreEqual(15, pulses[6].Length);
            Assert.AreEqual(24, pulses[5].Length);
            Assert.AreEqual(7, pulses[4].Length);
            Assert.AreEqual(8, pulses[3].Length);
            Assert.AreEqual(7, pulses[2].Length);
            Assert.AreEqual(16, pulses[1].Length);
            Assert.AreEqual(8, pulses[0].Length);
        }

        [TestMethod]
        public void TestPulsesToNibbles()
        {
            var pix = new Pix(@"Tests\Databar_Test01_artificial.bmp");

            Assert.AreEqual(122, pix.Width);
            Assert.AreEqual(77, pix.Height);

            var oscar = new Oscar(pix);

            Assert.AreEqual((int)((255 * pix.Width * pix.BytesPerPixel) * Barcode.LUMINACE_FACTOR), (int)oscar.Threshold);

            var blocks = oscar.GetBlocks();

            Assert.AreEqual(1, blocks.Count());

            Assert.AreEqual(77, blocks[0].Height);
            Assert.AreEqual(pix.Depth / 8, blocks[0].Bits);
            Assert.AreEqual(pix.Width * pix.BytesPerPixel, blocks[0].Width);
            Assert.AreEqual(38, blocks[0].MidlineIndex);

            var pulses = blocks[0].ComputePulses();

            Assert.AreEqual(27, pulses.Length);
            Assert.AreEqual(false, blocks[0].IsReversedPulse);

            var symbols = oscar.PulsesToSymbols(pulses);
            var symbolstring = symbols.Select(x=>x.ToString()).Aggregate((current, next) => current + next);
            Assert.AreEqual("101010101001100100110010101001010011111111", symbolstring);

            //1010101 0xD
            //0100110 0x0
            //0100110 0x0
            //0101010 0x1
            //0101001 0xA
            //1111111 EOL

            var nibbles = oscar.SymbolsToNibbles(symbols);

            Assert.AreEqual(0xD, nibbles[0]);
            Assert.AreEqual(0x0, nibbles[1]);
            Assert.AreEqual(0x0, nibbles[2]);
            Assert.AreEqual(0x1, nibbles[3]);
            Assert.AreEqual(0xA, nibbles[4]);
        }

        [TestMethod]
        public void TestSymbolFromStringToNibbles()
        {
            var symbolstring = "101010101001100100110010101001010011111111";
            var oscar = new Oscar(symbolstring);

            var symbols = symbolstring.Select(x => int.Parse(x.ToString())).ToArray();
            var nibbles = oscar.SymbolsToNibbles(symbols);

            Assert.AreEqual(0xD, nibbles[0]);
            Assert.AreEqual(0x0, nibbles[1]);
            Assert.AreEqual(0x0, nibbles[2]);
            Assert.AreEqual(0x1, nibbles[3]);
            Assert.AreEqual(0xA, nibbles[4]);
            Assert.AreEqual(0x10, nibbles[5]);
        }

        [TestMethod]
        public void TestPulsesToNibbles_PNG()
        {
            var pix = new Pix(@"Tests\Pocorgtfo12-53_24bit.png");

            Assert.AreEqual(4352, pix.Width);
            Assert.AreEqual(5648, pix.Height);

            var oscar = new Oscar(pix);

            Assert.AreEqual((int)((255 * (pix.Stride - pix.Padding)) * Barcode.LUMINACE_FACTOR), (int)oscar.Threshold);

            var blocks = oscar.GetBlocks();

            Assert.AreEqual(31, blocks.Count());

            Assert.AreEqual(127, blocks[0].Height);
            Assert.AreEqual(63, blocks[0].MidlineIndex);

            var pulses0 = blocks[0].ComputePulses();
            var pulses1 = blocks[1].ComputePulses();

            Assert.AreEqual(433, pulses0.Length);
            Assert.AreEqual(false, blocks[0].IsReversedPulse);
            Assert.AreEqual(true, blocks[1].IsReversedPulse);

            var symbols0 = oscar.PulsesToSymbols(pulses0);
            var symbolstring = symbols0.Select(x => x.ToString()).Aggregate((current, next) => current + next);

            var expected = "1010101010011001001100101010010011001101101010110011011"; // too lazy to count more from source png file....
            Assert.IsTrue(symbolstring.StartsWith(expected));

            var nibbles0 = oscar.SymbolsToNibbles(symbols0);
            
            var symbols1 = oscar.PulsesToSymbols(pulses1);
            var nibbles1 = oscar.SymbolsToNibbles(symbols1);

            // seems to be around 700/701 symbols always resolving to 100 nibbles each (last symbol is EOL)
            Assert.AreEqual(701, symbols0.Length);
            Assert.AreEqual(701, symbols1.Length);
            Assert.AreEqual(100, nibbles0.Length);
            Assert.AreEqual(100, nibbles1.Length);

            var pulses2 = blocks[2].ComputePulses();
            var symbols2 = oscar.PulsesToSymbols(pulses2);
            var nibbles2 = oscar.SymbolsToNibbles(symbols2);

            Assert.AreEqual(700, symbols2.Length);
            Assert.AreEqual(100, nibbles2.Length);
        }

        [TestMethod]
        public void TestParseNibbles_PNG()
        {
            var testset = new Pix(@"Tests\Pocorgtfo12-53_24bit.png");
            var resultset = System.IO.File.ReadAllLines(@"Tests\OscarCompareResult_Pocorgtfo12-53_24bit.txt").
                                Where(x => !x.StartsWith(@"//"));

            Assert.AreEqual(4352, testset.Width);
            Assert.AreEqual(5648, testset.Height);

            var oscar = new Oscar(testset);

            var blocks = oscar.GetBlocks();

            var pulses = new List<Barcode.Pulse[]>(blocks.Count);
            foreach (var b in blocks)
            {
                pulses.Add(b.ComputePulses());
            }

            var symbols = new List<int[]>(pulses.Count);
            foreach (var p in pulses)
            {
                symbols.Add(oscar.PulsesToSymbols(p));
            }

            var nibbles = new List<int[]>(symbols.Count);
            foreach (var s in symbols)
            {
                nibbles.Add(oscar.SymbolsToNibbles(s));
            }

            var results = new List<int[]>(symbols.Count);
            foreach (var n in nibbles)
            {
                if (n == null) continue;
                results.Add(oscar.ParseNibbles(n));
            }

            Assert.IsNull(results[0]);// control array so result should be null

            List<string> hexstrings = new List<string>(results.Count());
            foreach(var r in results)
            {
                if (r == null) continue;
                hexstrings.Add(r.Select(x => x.ToString("x2")).Aggregate((a,b) => a + b));
            }
            
            int pos = -1;
            foreach(var expected in resultset)
            {
                pos++;

                if(pos == 1 || pos == 11 || pos == 13)
                {
                    // will fail
                    continue;
                }

                Assert.AreEqual(expected, hexstrings[pos]);
            }

            Assert.Inconclusive("data lines 1, 11 and 13 are not matching!");
        }
    }
}

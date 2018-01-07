using System;
using System.Collections;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OscarNET;

namespace UnitTest_Oscar
{
    [TestClass]
    [TestCategory("Pixel")]
    public class PixelTest
    {
        [TestMethod]
        public void TestLineEdges_Padding3()
        {
            var testset = new Pix(@"Tests\Barcode_Test01_linesedges_100_101.bmp");
            var result_line1 = testset.GetLine(0);
            var result_line2 = testset.GetLine(1);
            var result_line3 = testset.GetLine(2);

            var l1s = result_line1.Take(3).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l1s, new byte[] { 104, 104, 104 }), "line1 start marker wrong");

            var l1e = result_line1.Skip(Math.Max(0, result_line1.Count() - 3)).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l1e, new byte[] { 32, 32, 32 }), "line1 end marker wrong");
            
            var l2s = result_line2.Take(3).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l2s, new byte[] { 108, 108, 108 }), "line2 start marker wrong");

            var l2e = result_line2.Skip(Math.Max(0, result_line2.Count() - 3)).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l2e, new byte[] { 17, 17, 17 }), "line2 end marker wrong");

            var l3s = result_line3.Take(3).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l3s, new byte[] { 60, 0, 180 }), "line3 start marker wrong");

            var l3e = result_line3.Skip(Math.Max(0, result_line3.Count() - 3)).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l3e, new byte[] { 180, 0, 60 }), "line3 end marker wrong");
        }

        [TestMethod]
        public void TestLineEdges_Padding2()
        {
            var testset = new Pix(@"Tests\Barcode_Test01_linesedges_2.bmp");
            var result_line1 = testset.GetLine(0);
            var result_line2 = testset.GetLine(1);
            var result_line3 = testset.GetLine(2);

            var l1s = result_line1.Take(3).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l1s, new byte[] { 104, 104, 104 }), "line1 start marker wrong");

            var l1e = result_line1.Skip(Math.Max(0, result_line1.Count() - 3)).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l1e, new byte[] { 32, 32, 32 }), "line1 end marker wrong");

            var l2s = result_line2.Take(3).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l2s, new byte[] { 108, 108, 108 }), "line2 start marker wrong");

            var l2e = result_line2.Skip(Math.Max(0, result_line2.Count() - 3)).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l2e, new byte[] { 17, 17, 17 }), "line2 end marker wrong");

            var l3s = result_line3.Take(3).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l3s, new byte[] { 60, 0, 180 }), "line3 start marker wrong");

            var l3e = result_line3.Skip(Math.Max(0, result_line3.Count() - 3)).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l3e, new byte[] { 180, 0, 60 }), "line3 end marker wrong");
        }

        [TestMethod]
        public void TestLineEdges_Padding1()
        {
            var testset = new Pix(@"Tests\Barcode_Test01_linesedges_3.bmp");
            var result_line1 = testset.GetLine(0);
            var result_line2 = testset.GetLine(1);
            var result_line3 = testset.GetLine(2);

            var l1s = result_line1.Take(3).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l1s, new byte[] { 104, 104, 104 }), "line1 start marker wrong");

            var l1e = result_line1.Skip(Math.Max(0, result_line1.Count() - 3)).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l1e, new byte[] { 32, 32, 32 }), "line1 end marker wrong");

            var l2s = result_line2.Take(3).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l2s, new byte[] { 108, 108, 108 }), "line2 start marker wrong");

            var l2e = result_line2.Skip(Math.Max(0, result_line2.Count() - 3)).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l2e, new byte[] { 17, 17, 17 }), "line2 end marker wrong");

            var l3s = result_line3.Take(3).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l3s, new byte[] { 60, 0, 180 }), "line3 start marker wrong");

            var l3e = result_line3.Skip(Math.Max(0, result_line3.Count() - 3)).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l3e, new byte[] { 180, 0, 60 }), "line3 end marker wrong");
        }

        [TestMethod]
        public void TestLines_Padding0()
        {
            var testset = new Pix(@"Tests\Barcode_Test01_linesevaluation.bmp");
            var result_line1 = testset.GetLine(0);
            var result_line2 = testset.GetLine(1);
            var result_line3 = testset.GetLine(2);

            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(result_line1, new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }));
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(result_line2, new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 0, 0, 0, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }));
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(result_line3, new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 0, 0, 0, 0, 0, 0, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }));
        }

        [TestMethod]
        public void TestLines_Padding0_PNG()
        {
            var testset = new Pix(@"Tests\Pocorgtfo12-53_24bit.png");
            var result_line1 = testset.GetLine(0);

            var expected = Enumerable.Range(0, testset.Stride).Select(x => (byte)(255)).ToArray();

            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(expected, result_line1));
        }

        [TestMethod]
        public void TestIntLineEdges_Padding3()
        {
            var testset = new Pix(@"Tests\Barcode_Test01_linesedges_100_101.bmp");
            var result_line1 = testset.GetIntLine(0);
            var result_line2 = testset.GetIntLine(1);
            var result_line3 = testset.GetIntLine(2);

            var l1s = result_line1.Take(3).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l1s, new int[] { 104, 104, 104 }), "line1 start marker wrong");

            var l1e = result_line1.Skip(Math.Max(0, result_line1.Count() - 3)).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l1e, new int[] { 32, 32, 32 }), "line1 end marker wrong");

            var l2s = result_line2.Take(3).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l2s, new int[] { 108, 108, 108 }), "line2 start marker wrong");

            var l2e = result_line2.Skip(Math.Max(0, result_line2.Count() - 3)).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l2e, new int[] { 17, 17, 17 }), "line2 end marker wrong");

            var l3s = result_line3.Take(3).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l3s, new int[] { 60, 0, 180 }), "line3 start marker wrong");

            var l3e = result_line3.Skip(Math.Max(0, result_line3.Count() - 3)).ToArray();
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(l3e, new int[] { 180, 0, 60 }), "line3 end marker wrong");
        }

        [TestMethod]
        public void TestSinglePixel()
        {
            var testset = new Pix(@"Tests\Barcode_Test01_linesedges_100_101.bmp");

            var result = testset.GetPixel(0, 0);
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(result, new byte[] { 104, 104, 104 }));

            result = testset.GetPixel(0, 1);
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(result, new byte[] { 108, 108, 108 }));

            result = testset.GetPixel(0, 2);
            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(result, new byte[] { 60, 0, 180 }));
        }

        [TestMethod]
        public void TestSinglePixel_PNG()
        {
            var testset = new Pix(@"Tests\Pocorgtfo12-53_24bit.png");
            var result = testset.GetPixel(0, 0);

            var expected = Enumerable.Range(0, testset.BytesPerPixel).Select(x => (byte)(255)).ToArray();

            Assert.IsTrue(StructuralComparisons.StructuralEqualityComparer.Equals(expected, result));
        }

        [TestMethod]
        public void TestSingleBit()
        {
            var testset = new Pix(@"Tests\Barcode_Test01_linesedges_100_101.bmp");

            var result = testset.GetBit(0, 0);
            Assert.AreEqual(104, (int)result);

            result = testset.GetBit(0, 1);
            Assert.AreEqual(108, (int)result);

            result = testset.GetBit(0, 2);
            Assert.AreEqual(60, (int)result);

            result = testset.GetBit(2, 2);
            Assert.AreEqual(180, (int)result);

            result = testset.GetBit((798*3) + 2, 2);
            Assert.AreEqual(60, (int)result);
        }

        [TestMethod]
        public void TestAttributes()
        {
            var testset = new Pix(@"Tests\Barcode_Test01_linesedges_100_101.bmp");

            var width = 799;
            var height = 12;
            var depth = 24;
            var padding = ((width * depth / 8) % 4) == 0 ? 0 : 4 - ((width * depth / 8) % 4);
            var stride = width * depth/8 + padding;

            Assert.AreEqual(width, testset.Width);
            Assert.AreEqual(height, testset.Height);
            Assert.AreEqual(depth, testset.Depth);
            Assert.AreEqual(stride, testset.Stride);
            Assert.AreEqual(padding, testset.Padding);
        }

        [TestMethod]
        public void TestAttributes_PNG()
        {
            var testset = new Pix(@"Tests\Pocorgtfo12-53_24bit.png");

            var width = 4352;
            var height = 5648;
            var depth = 24;
            var padding = ((width * 1) % 4) == 0 ? 0 : 4 - ((width * 1) % 4);
            var stride = width * depth / 8 + padding;

            Assert.AreEqual(width, testset.Width);
            Assert.AreEqual(height, testset.Height);
            Assert.AreEqual(depth, testset.Depth);
            Assert.AreEqual(stride, testset.Stride);
            Assert.AreEqual(padding, testset.Padding);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace OscarNET
{
    public class Pix
    {
        private Bitmap _image;
        private BitmapData _bitmapData;
        private int _width;
        private int _height;
        private int _length;

        public Bitmap Image { get => _image; private set => _image = value; }
        public int Width { get => _width; private set => _width = value; }
        public int Height { get => _height; private set => _height = value; }
        public int Stride { get; private set; }
        public int Depth { get; private set; }
        public int BytesPerPixel { get; private set; }
        public int Padding { get; private set; }
        public byte[] Pixels { get; private set; }

        public Pix(string filename)
        {
            _image = new Bitmap(filename);
            _width = _image.Width;
            _height = _image.Height;
            _length = _width * _height;

            Rectangle rect = new Rectangle(0, 0, _width, _height);

            try
            {
                _bitmapData = _image.LockBits(rect, ImageLockMode.ReadOnly, _image.PixelFormat);

                Depth = System.Drawing.Bitmap.GetPixelFormatSize(_bitmapData.PixelFormat);
                BytesPerPixel = Math.Max(1, Depth >> 3);

                Stride = _bitmapData.Stride; // bmp: BytesPerPixel * width + padding
                Padding = Math.Max(0, _bitmapData.Stride - (_width * BytesPerPixel)); //((_width * BytesPerPixel) % 4) == 0 ? 0 : 4 - ((_width * BytesPerPixel) % 4);
                
                //copy content to byte[]
                int size = Stride * Height;
                Pixels = new byte[size];
                Marshal.Copy(_bitmapData.Scan0, Pixels, 0, size);
            }
            finally
            {
                _image.UnlockBits(_bitmapData);
                _bitmapData = null;
            }
        }

        public byte[] GetPixel(int x, int y)
        {
            // correct with stride
            int i = y * Stride + x * BytesPerPixel;

            byte[] result = new byte[BytesPerPixel];
            Array.Copy(Pixels, y * Stride + x, result, 0, BytesPerPixel);

            return result;
        }

        public byte GetBit(int x, int y)
        {
            // correct with stride
            int i = y * Stride + x;

            return Pixels[i];
        }

        public byte[] GetLine(int y)
        {
            int size = Stride - Padding;
            int sol = y * Stride;

            byte[] result = new byte[size];
            Array.Copy(Pixels, sol, result, 0, size);

            return result;
        }

        public int[] GetIntLine(int y)
        {
            return Array.ConvertAll(GetLine(y), x => (int)x);
        }

        public IEnumerable<int> EvaluateAllLines()
        {
            var lines = Enumerable.
                            Range(0, Height).
                            Select(x => GetIntLine(x).Sum());

            return lines;
        }
    }
}

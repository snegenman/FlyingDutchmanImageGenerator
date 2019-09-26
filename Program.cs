using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace FlyingDutchman
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Fly test generator");
            Console.WriteLine("==================");
            Console.WriteLine("");
            if (args.Any(x => x.Contains("?")))
            {
                Console.WriteLine("FlyingDutchman.exe [parameters]");
                Console.WriteLine("? = This help");
                Console.WriteLine("p = pause at the end");
            }
            else
            {
                Console.WriteLine("Use 'FlyingDutchman.exe ?' for help");
            }

            Console.WriteLine("");

            try
            {
                System.IO.Directory.CreateDirectory("output");

                Random _random = new Random(DateTime.Now.Second);

                using (var image = new Image<Rgba32>(1000, 1000))
                {
                    var flyTypes = new FlyTypes();

                    var flyDrawer = new FlyDrawer(image, 5);

                    for (int i = 0; i < 100; i++)
                    {
                        var flyAdded = false;
                        while (!flyAdded)
                        {
                            int left = _random.Next(image.Width);
                            int top = _random.Next(image.Height);
                            int fly = _random.Next(0, flyTypes.Count);

                            if (!flyDrawer.DrawAreas.InAreas(left, top))
                            {
                                // point is in non of current areas or over the rim

                                var flyType = flyTypes[fly];

                                Console.Write($"{fly}");

                                flyDrawer.DrawFly(left, top, flyType);

                                flyAdded = true;
                                Console.Write(".");
                            }
                            else
                            {
                                Console.Write("X");
                            }
                        }
                    }

                    Console.WriteLine("");
                    Console.WriteLine($"Flies drawn: {flyDrawer.DrawAreas.Count}");

                    var fileName = $"output/test{DateTime.Now.ToString("yyyyMMddhhmmssfff")}";

                    image.Save(fileName + ".png");

                    var json = JsonConvert.SerializeObject(flyDrawer.DrawAreas);

                    File.WriteAllText(fileName + ".json", json);

                    Console.WriteLine($"Files generated: {fileName}.png, {fileName}.json");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            if (args.Any(x => x.ToLower() == "p"))
            {
                Console.WriteLine("Press a key to exit");
                Console.ReadKey();
            }
        }
    }

    public class FlyDrawer
    {
        Image<Rgba32> _image;

        public DrawAreas DrawAreas { get; private set; }

        /// <summary>
        /// Remember the draw areas on the canvas. Prevent overlap.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="offset"></param>
        public FlyDrawer(Image<Rgba32> image, int offset)
        {
            _image = image;
            DrawAreas = new DrawAreas(image.Width, image.Height, offset);
        }

        public void DrawFly(int left, int top, IFlyType flyType)
        {
            flyType.DrawFly(_image, left, top);

            DrawAreas.Add(new DrawArea(left, left+4, top, top+4, flyType.ToString()));
        }
    }

    public interface IFlyType
    {
        void DrawFly(Image<Rgba32> image, int left, int top);

    }
    public class FlyTypeSquare : IFlyType
    {
        public void DrawFly(Image<Rgba32> image, int left, int top)
        {
            image[left, top] = Rgba32.Red;
            image[left + 1, top] = Rgba32.Red;
            image[left + 2, top] = Rgba32.Red;
            image[left + 3, top] = Rgba32.Red;
            image[left + 4, top] = Rgba32.Red;

            image[left, top + 1] = Rgba32.Red;
            image[left + 4, top + 1] = Rgba32.Red;

            image[left, top + 2] = Rgba32.Red;
            image[left + 4, top + 2] = Rgba32.Red;

            image[left, top + 3] = Rgba32.Red;
            image[left + 4, top + 3] = Rgba32.Red;

            image[left, top + 4] = Rgba32.Red;
            image[left + 4, top + 4] = Rgba32.Red;

            image[left, top + 4] = Rgba32.Red;
            image[left + 1, top + 4] = Rgba32.Red;
            image[left + 2, top + 4] = Rgba32.Red;
            image[left + 3, top + 4] = Rgba32.Red;
            image[left + 4, top + 4] = Rgba32.Red;
        }
    }

    public class FlyTypePointToTop : IFlyType
    {
        public void DrawFly(Image<Rgba32> image, int left, int top)
        {
            image[left + 2, top] = Rgba32.Red;

            image[left + 2, top + 1] = Rgba32.Red;
            
            image[left + 1, top + 2] = Rgba32.Red;
            image[left + 2, top + 2] = Rgba32.Red;
            image[left + 3, top + 2] = Rgba32.Red;

            image[left + 1, top + 3] = Rgba32.Red;
            image[left + 2, top + 3] = Rgba32.Red;
            image[left + 3, top + 3] = Rgba32.Red;

            image[left + 0, top + 4] = Rgba32.Red;
            image[left + 1, top + 4] = Rgba32.Red;
            image[left + 3, top + 4] = Rgba32.Red;
            image[left + 4, top + 4] = Rgba32.Red;
        }

        public override string ToString()
        {
            return ("Top");
        }
    }

    public class FlyTypePointToLeft : IFlyType
    {
        public void DrawFly(Image<Rgba32> image, int left, int top)
        {
            image[left + 4, top] = Rgba32.Red;

            image[left + 2, top + 1] = Rgba32.Red;
            image[left + 3, top + 1] = Rgba32.Red;
            image[left + 4, top + 1] = Rgba32.Red;

            image[left, top + 2] = Rgba32.Red;
            image[left + 1, top + 2] = Rgba32.Red;
            image[left + 2, top + 2] = Rgba32.Red;
            image[left + 3, top + 2] = Rgba32.Red;

            image[left + 2, top + 3] = Rgba32.Red;
            image[left + 3, top + 3] = Rgba32.Red;
            image[left + 4, top + 3] = Rgba32.Red;

            image[left + 4, top + 4] = Rgba32.Red;
        }
        public override string ToString()
        {
            return ("Left");
        }
    }

    public class FlyTypePointToRight : IFlyType
    {
        public void DrawFly(Image<Rgba32> image, int left, int top)
        {
            image[left + 0, top] = Rgba32.Red;

            image[left + 0, top + 1] = Rgba32.Red;
            image[left + 1, top + 1] = Rgba32.Red;
            image[left + 2, top + 1] = Rgba32.Red;

            image[left + 1, top + 2] = Rgba32.Red;
            image[left + 2, top + 2] = Rgba32.Red;
            image[left + 3, top + 2] = Rgba32.Red;
            image[left + 4, top + 2] = Rgba32.Red;

            image[left + 0, top + 3] = Rgba32.Red;
            image[left + 1, top + 3] = Rgba32.Red;
            image[left + 2, top + 3] = Rgba32.Red;

            image[left + 0, top + 4] = Rgba32.Red;
        }
        public override string ToString()
        {
            return ("Right");
        }
    }

    public class FlyTypePointToBottom : IFlyType
    {
        public void DrawFly(Image<Rgba32> image, int left, int top)
        {
            image[left + 0, top] = Rgba32.Red;
            image[left + 1, top] = Rgba32.Red;
            image[left + 3, top] = Rgba32.Red;
            image[left + 4, top] = Rgba32.Red;

            image[left + 1, top + 1] = Rgba32.Red;
            image[left + 2, top + 1] = Rgba32.Red;
            image[left + 3, top + 1] = Rgba32.Red;

            image[left + 1, top + 2] = Rgba32.Red;
            image[left + 2, top + 2] = Rgba32.Red;
            image[left + 3, top + 2] = Rgba32.Red;

            image[left + 2, top + 3] = Rgba32.Red;

            image[left + 2, top + 4] = Rgba32.Red;
        }
        public override string ToString()
        {
            return ("Bottom");
        }
    }

    public class FlyTypes : List<IFlyType>
    {
        public FlyTypes()
        {
//            this.Add(new FlyTypeSquare());
            this.Add(new FlyTypePointToTop());
            this.Add(new FlyTypePointToLeft());
            this.Add(new FlyTypePointToBottom());
            this.Add(new FlyTypePointToRight());
        }
    }

    public class DrawAreas : List<DrawArea>
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        public int Offset { get; private set; }

        public DrawAreas(int width, int height, int offset)
        {
            Width = width;
            Height = height;
            Offset = offset;
        }

        /// <summary>
        /// Check if any area is overlapping
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="offSet"></param>
        /// <returns>True if point is at least in one area or of the rim</returns>
        public bool InAreas(int left, int top)
        {
            var result = (left <= 0)
                            || (left >= Width - Offset)
                            || (top <= 0)
                            || (top >= Height-Offset);

            foreach (var item in this)
            {
                result = result 
                            || item.InArea(left, top, Offset);
            }

            return result;
        }
    }

    public class DrawArea
    {
        public int Left { get; private set; }
        public int Right { get; private set; }
        public int Top { get; private set; }
        public int Bottom { get; private set; }
        public string Type { get; private set; }

        public DrawArea(int left, int right, int top, int bottom, string type)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
            Type = type;
        }

        /// <summary>
        /// Prevent overlap with extra offset
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="offSet"></param>
        /// <returns>Point is in area with consideration of the offset (to the right and to the bottom)</returns>
        public bool InArea(int left, int top, int offSet)
        {
            return (left >= Left)
                        && (left <= Right-offSet)
                        && (top >= Top)
                        && (top <= Bottom - offSet);
        }
    }
}

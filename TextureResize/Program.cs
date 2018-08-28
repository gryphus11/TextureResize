using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using TextureResize.Sources;
using System.Threading;

namespace TextureResize
{
    class Program
    {
        private static void Main(string[] args)
        {
            int width = 0;
            int height = 0;
            string[] arguments = null;

            InputArguments(args, out arguments, out width, out height);

            Resize(arguments, width, height);

            Console.WriteLine("모든 텍스쳐의 변환이 완료되었습니다. 엔터 키를 눌러서 종료.");
            Console.WriteLine("Made By Brunhild");
            while (Console.ReadKey().Key != ConsoleKey.Enter) ;
        }

        private static void InputArguments(string[] args, out string[] outArguments, out int outWidth, out int outHeight)
        {
            int width = 0;
            int height = 0;
            string[] arguments = null;

            if (args.Length == 0)
            {
                Console.Write("파일의 경로와 파일명까지 입력해 주세요. ',' 로 구분하여 경로 추가: ");

                string inputPath = Console.ReadLine();
                arguments = inputPath.Trim().Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string argument in arguments)
                {
                    Console.WriteLine(argument);
                }
            }
            else
            {
                arguments = args;
            }

            bool validValue = false;

            do
            {
                Console.Write("리사이징 텍스쳐의 Width를 입력해 주세요 : ");
                string widthString = Console.ReadLine();
                if (int.TryParse(widthString, out width))
                {
                    validValue = true;
                }
                else
                {
                    Console.WriteLine("올바른 입력값이 아닙니다.\n");
                }
            } while (!validValue);

            validValue = false;

            do
            {
                Console.Write("리사이징 텍스쳐의 Height를 입력해 주세요 : ");
                string heightString = Console.ReadLine();
                if (int.TryParse(heightString, out height))
                {
                    validValue = true;
                }
                else
                {
                    Console.WriteLine("올바른 입력값이 아닙니다.\n");
                }
            } while (!validValue);

            outArguments = arguments;
            outWidth = width;
            outHeight = height;
        }

        private static void Resize(string[] arguments, int width, int height)
        {
            List<TextureLoader> loaders = new List<TextureLoader>();
            for (int i = 0; i < arguments.Length; ++i)
            {
                TextureLoader loader = new TextureLoader();
                loader.ResizeTexture(arguments[i], width, height);
                loaders.Add(loader);
            }

            bool isDone = false;
            while (!isDone)
            {
                isDone = true;
                foreach (TextureLoader loader in loaders)
                {
                    if (!loader.isResizingDone)
                    {
                        isDone = false;
                        break;
                    }
                }
            }

            loaders.Clear();
            loaders = null;
        }
    }
}

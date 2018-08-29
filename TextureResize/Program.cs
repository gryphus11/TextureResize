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

            // 입력부. out result로 문자열의 배열, 가로, 세로의 크기를 받음
            InputArguments(args, out arguments, out width, out height);

            // 처리부. 문자열의 배열에 해당하는 파일이 존재시 사이즈를 변경
            Resize(arguments, width, height);

            Console.WriteLine("모든 텍스쳐의 변환이 완료되었습니다.");
            Console.WriteLine("[Texture Resize] Made By Brunhild");
            Console.WriteLine("Press Enter Key.....");
            while (Console.ReadKey().Key != ConsoleKey.Enter) ;
        }

        private static void InputArguments(string[] args, out string[] outArguments, out int outWidth, out int outHeight)
        {
            int width = 0;
            int height = 0;
            string[] arguments = null;
		
            // 유효한 값의 여부를 체크하는 부분
	    // 테스트 주석
            if (args.Length == 0)
            {
                Console.Write("파일의 경로와 파일명까지 입력해 주세요. ',' 로 구분하여 경로 추가: ");

                string inputPath = Console.ReadLine();
                arguments = inputPath.Trim().Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine("경로 입력값 : ");
                foreach (string argument in arguments)
                {
                    Console.WriteLine(argument);
                }
                Console.WriteLine("경로 입력 종료.");
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
            // 개별적인 리사이징을 수행하는 텍스쳐 로더의 리스트
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

            // 모든 리사이징 완료시 클리어
            loaders.Clear();
            loaders = null;
        }
    }
}

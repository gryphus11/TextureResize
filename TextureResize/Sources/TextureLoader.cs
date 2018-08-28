using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace TextureResize.Sources
{
    public class TextureLoader
    {
        public bool isResizingDone = false;

        private Bitmap LoadImage(string path)
        {
            Bitmap texture = null;
            if (File.Exists(path))
            {
                string fileName = Path.GetFileName(path);
                Console.WriteLine(fileName + "파일 읽기를 시작합니다.");
                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                texture = new Bitmap(stream);
                stream.Close();

                Console.WriteLine(path + " 읽기 완료! " + texture.Size + "\n");

                return texture;
            }
            else
            {
                Console.WriteLine(path + " 은(는) 유효하지 않습니다.");
            }

            return null;
        }

        private void ResizeImage2(string path, int width, int height)
        {
            if (File.Exists(path))
            {
                string directory = Path.GetDirectoryName(path);
                string fileName = Path.GetFileName(path);

                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                Image sourceImage = Image.FromStream(stream);
                Bitmap sourceTexture = new Bitmap(sourceImage, new Size(width, height));
                stream.Close();
                sourceTexture.Save(directory + "/new2_" + fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                sourceTexture.Dispose();
            }
            else
            {
                Console.WriteLine(path + " => 경로 또는 원본 파일을 찾을수 없습니다.\n");
            }
        }

        private void ResizeImage(string path, int width, int height)
        {
            Bitmap sourceTexture = LoadImage(path);
            string fileName = string.Empty;
            string directory = string.Empty;

            int targetWidth = width;
            int targetHeight = height;

            if (sourceTexture == null)
            {
                Console.WriteLine(path + " => 경로 또는 원본 파일을 찾을수 없습니다.\n");
                return;
            }

            directory = Path.GetDirectoryName(path);
            fileName = Path.GetFileName(path);

            int sourceWidth = sourceTexture.Width;
            int sourceHeight = sourceTexture.Height;

            if (targetHeight > sourceHeight)
            {
                targetHeight = sourceHeight;
            }

            if (targetWidth > sourceWidth)
            {
                targetWidth = sourceWidth;
            }

            if ((sourceWidth <= targetWidth) && (sourceHeight <= targetHeight))
            {
                Console.WriteLine(fileName + " 은(는) 리사이징 이 필요하지 않습니다.\n");
                return;
            }

            int totalTargetLength = targetWidth * targetHeight;

            int widthPixelRate = (sourceWidth / targetWidth);
            int heightPixelRate = (sourceHeight / targetHeight);

            int colorBlendedCount = 0;

            Bitmap newBitmap = new Bitmap(targetWidth, targetHeight);
            Console.WriteLine(fileName + " 의 색상 정보 평균을 처리중\n");

            int maxIndex = totalTargetLength - 1;
            float targetRate = 10.0f;

            for (int totalIndex = 0; totalIndex < totalTargetLength; ++totalIndex)
            {
                int startY = (totalIndex / targetWidth) * heightPixelRate;
                int startX = (totalIndex % targetWidth) * widthPixelRate;
                int endY = startY + heightPixelRate;
                int endX = startX + widthPixelRate;

                int alpha = 0;
                int red = 0;
                int green = 0;
                int blue = 0;

                for (int y = startY; y < endY; ++y)
                {
                    for (int x = startX; x < endX; ++x)
                    {
                        Color sourcePixel = sourceTexture.GetPixel(x, y);

                        alpha += sourcePixel.A;
                        red += sourcePixel.R;
                        green += sourcePixel.G;
                        blue += sourcePixel.B;

                        ++colorBlendedCount;
                    }
                }

                int avgAlpha = alpha / colorBlendedCount;
                int avgRed = red / colorBlendedCount;
                int avgGreen = green / colorBlendedCount;
                int avgBlue = blue / colorBlendedCount;

                Color averageColor = Color.FromArgb(avgAlpha, avgRed, avgGreen, avgBlue);

                colorBlendedCount = 0;

                newBitmap.SetPixel(totalIndex % targetWidth, totalIndex / targetWidth, averageColor);


                float percentage = ((float)totalIndex / maxIndex * 100.0f);
                if (percentage >= targetRate)
                {
                    targetRate += 10.0f;
                    Console.WriteLine(fileName + " " + percentage + "% 완료\n");
                }
            }
            Console.WriteLine("리사이징이 완료된 " + fileName + " 의 저장 시작\n");
            newBitmap.Save(directory + "/new_" + fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            newBitmap.Dispose();
            Console.WriteLine("저장 완료 : " + directory + "/new_" + fileName + "\n");
        }

        async public void ResizeTexture(string path, int width, int height)
        {
            Task task = Task.Run(() => { ResizeImage2(path, width, height); });
            Console.WriteLine(path + " [리사이징 시작]\n");
            await task;
            Console.WriteLine(path + " [리사이징 완료]\n");
            isResizingDone = true;
        }

    }
}

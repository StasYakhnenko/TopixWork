using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TopixCompresion
{
    //Main Builder
    class TopixBuilder
    {
        private string GetHexStringFromBinaryByte(byte[] binaryArray)
        {
            var data = binaryArray.Select(e => e.ToString());
            StringBuilder sb = new StringBuilder();
            foreach (var figure in data)
                sb.Append(figure);
            int value = Convert.ToInt16(sb.ToString(), 2);
            return value.ToString("X");
        }
        private byte[,] GetTwoDimensinalArrayFromBitmap(Bitmap bmp)
        {
            int resultWidth = (bmp.Width % 8 == 0) ? bmp.Width : bmp.Width - bmp.Width % 8 + 8;
            byte[,] resultArray = new byte[bmp.Height + 1, resultWidth];

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    resultArray[i+1,j] = ( (byte)bmp.GetPixel(j,i).R == 0) ? (byte)0 : (byte)1;
                }
            }
            return resultArray;
        }
        public string GetBitmapData(Bitmap bmp)
        {
            byte[,] bmpData = this.GetTwoDimensinalArrayFromBitmap(bmp);
            int line = 1;
            string resultData = "";

            byte[] l1Data = new byte[8];
            byte[] l2Data = new byte[8];
            byte[] l3Data = new byte[8];
            byte[] graphicData = new byte[8];

            bool isNotNullForOneElement = false;
            bool isNotNullForLine = false;
            StringBuilder outputData = new StringBuilder();
            string resultForLine = "";
            while (line<=bmp.Height)
            {
                isNotNullForLine = false;
                for (int i = 0; i <= bmp.Width; i += 8)
                {
                    //taking one byte from current position
                    for (int k = 0; k < 8; k++)
                    {
                        graphicData[k] = (byte) Math.Abs(bmpData[line, i + k] - bmpData[line - 1, i + k]);
                        if (graphicData[k] == 1)
                        {
                            isNotNullForOneElement = true;
                            isNotNullForLine = true;
                        }
                    }
                    if (isNotNullForOneElement)
                    {
                        int indexL1 = i / 256;
                        int indexL2 = i / 64;
                        int indexL3 = i / 8;

                        l1Data[indexL1] = 1;
                        l2Data[indexL2] = 1;
                        l3Data[indexL3] = 1;
                        outputData.Append(this.GetHexStringFromBinaryByte(graphicData) + " ");
                    }
                    graphicData = new byte[8];
                    isNotNullForOneElement = false;
                }
                if (isNotNullForLine)
                    resultForLine = this.GetHexStringFromBinaryByte(l1Data) + " " +
                                 this.GetHexStringFromBinaryByte(l2Data) + " " +
                                 this.GetHexStringFromBinaryByte(l3Data) + " " +
                                 outputData.ToString();
                else
                    resultForLine = "00 ";
                resultData += resultForLine + "\n";

                outputData.Clear();
                l1Data = new byte[8];
                l2Data = new byte[8];
                l3Data = new byte[8];
                line++;
            }

            return resultData;
        }
    }
}

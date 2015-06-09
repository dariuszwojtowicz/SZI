using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using xpidea.neuro.net.patterns;
using xpidea.neuro.net.backprop;
using xpidea.neuro.net;
using System.IO;
using System.Xml;
using System.Windows.Media.Imaging;

namespace Kelner.Algorithm
{
    class NeuralBackProp
    {
        static public bool IsTerminated;
        static public string global_font = "Gabriola";
        public Font font_sharp = new System.Drawing.Font(global_font, 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));

	
		public static int aMatrixDim = 10;
		public static byte aFirstChar =  (byte)'A';
		public static byte aLastChar = (byte)'z';
		public static int aCharsCount = 26; //liczba liter + 1
		public PatternsCollection trainingPatterns;
		public NeuralNet backpropNetwork;
		private System.ComponentModel.Container components = null;

		public NeuralBackProp()
		{

            trainingPatterns = CreateTrainingPatterns();
            backpropNetwork = new NeuralNet(new int[3] { aMatrixDim * aMatrixDim, (aMatrixDim * aMatrixDim + aCharsCount + 9) / 2, aCharsCount + 9 });

            if (backpropNetwork == null)
            {
                Console.WriteLine("*** Neural Network => Sieć nie została wygenerowana. :<");
                return;
            }
            if (trainingPatterns == null)
            {

                Console.WriteLine("*** Neural Network => Zbiory treningowe nie zostały wygenerowane. :<:<:<");
                return;
            }
            backpropNetwork.Train(trainingPatterns);
            Console.WriteLine("*** Neural Network => Sieć została wytrenowana!");

		}
		
		public PatternsCollection CreateTrainingPatterns()
		{			 
			PatternsCollection result = new PatternsCollection(aCharsCount + 9, aMatrixDim * aMatrixDim, aCharsCount + 9);
			for (int i= 0; i<aCharsCount; i++)
			{
                Console.Write("{0} - {1}", aFirstChar + i, Convert.ToChar(aFirstChar + i));
				double[] aBitMatrix = ImageToDoubleArray(Convert.ToChar(aFirstChar + i), aMatrixDim, 0);
				for (int j = 0; j<aMatrixDim * aMatrixDim; j++) 
					result[i].Input[j] = aBitMatrix[j];
				result[i].Output[i] = 1;
			}

            aFirstChar = 23;
            for (int i = 26; i < 35; i++)
            {
                Console.Write("{0} - {1}", i + aFirstChar, Convert.ToChar(aFirstChar + i));
                double[] aBitMatrix = ImageToDoubleArray(Convert.ToChar(aFirstChar +i), aMatrixDim, 0);
                for (int j = 0; j < aMatrixDim * aMatrixDim; j++)
                    result[i].Input[j] = aBitMatrix[j];
                result[i].Output[i] = 1;
            }
			return result;
		}

        public Bitmap LoadImage(string fileName, int w, int h)
        {

           
                using (Stream BitmapStream = System.IO.File.Open(fileName, System.IO.FileMode.Open))
                {
                    Image img = Image.FromStream(BitmapStream);
                    BitmapStream.Flush();
                    BitmapStream.Dispose();
                    if (w != 0 && h != 0)
                    {
                        return new Bitmap(img, w, h);
                    }
                    else
                    {
                        return new Bitmap(img);
                    }
                }
          
     
        }


		public double[] ImageToDoubleArray(char aChar, int aArrayDim, int aAddNoisePercent)
		{
            double[] result = new double[aArrayDim * aArrayDim];
            Size size;
            using (Graphics gr = Graphics.FromHwnd(IntPtr.Zero))
            {
                size = Size.Round(gr.MeasureString(aChar.ToString(), font_sharp));
            }

            Bitmap aSrc = LoadImage("..\\..\\Data\\ImageSet\\"+aChar+".bmp",size.Width, size.Height);
            Graphics bmp = Graphics.FromImage(aSrc);

			double xStep = (double)aSrc.Width/(double)aArrayDim;
			double yStep = (double)aSrc.Height/(double)aArrayDim;
			for (int i=0; i<aSrc.Width; i++)
				for (int j=0;j<aSrc.Height;j++)
				{
					int x = (int)((i/xStep));
					int y = (int)(j/yStep);
					Color c = aSrc.GetPixel(i,j);
					result[y*x+y]+=Math.Sqrt(c.R*c.R+c.B*c.B+c.G*c.G);
				}
			return  Scale(result);
		}

        public double[] CheckLetter(int aArrayDim, BitmapImage image)
		{
            double[] result = new double[aArrayDim * aArrayDim];

            Bitmap aSrc;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(image));
                enc.Save(outStream);
                aSrc = new System.Drawing.Bitmap(outStream);
            }
            Graphics bmp = Graphics.FromImage(aSrc);

			double xStep = (double)aSrc.Width/(double)aArrayDim;
			double yStep = (double)aSrc.Height/(double)aArrayDim;
			for (int i=0; i<aSrc.Width; i++)
				for (int j=0;j<aSrc.Height;j++)
				{
					int x = (int)((i/xStep));
					int y = (int)(j/yStep);
					Color c = aSrc.GetPixel(i,j);
					result[y*x+y]+=Math.Sqrt(c.R*c.R+c.B*c.B+c.G*c.G); //Convert to BW, I guess I can use B component of Alpha color space too...
				}
			return  Scale(result);
		}

		private double MaxOf(double[] src)
		{
			double res=double.NegativeInfinity;
			foreach (double d in src)
				if (d>res) res = d;
			return res;
		}

		private double[] Scale(double[] src)
		{
			double max = MaxOf(src);
			if (max!=0)
			{
				for(int i=0; i<src.Length; i++)
					src[i] = src[i]/max;
			}
			return src;					
		}
			
		public String ReadLetter(BitmapImage image)
		{			
				double[] aInput = CheckLetter(aMatrixDim, image);
                for (int i = 0; i < backpropNetwork.InputNodesCount; i++)
                {
                    backpropNetwork.InputNode(i).Value = aInput[i];
                }
				backpropNetwork.Run();
                return Convert.ToChar(aFirstChar + backpropNetwork.BestNodeIndex).ToString();
		}

    }
}

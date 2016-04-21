using System;
using System.Collections.Generic;
using System.Text;
using BddSharp.Kernel;

namespace Examples
{
    class Sudoku : IExampleForm
    {
        public Sudoku()
        {
            FddKernel.Setup();
        }

        public string Run(bool b1, bool b2, bool b3)
        {
            Fdd[] nodeArray = new Fdd[16];
            for (int i = 0; i < 16; i++)
            {
                nodeArray[i] = new Fdd(4);
            }
            Bdd result = new Bdd(true);

            result = Kernel.And(result, FddKernel.Xor(nodeArray[0], nodeArray[1]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[0], nodeArray[2]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[0], nodeArray[3]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[0], nodeArray[4]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[0], nodeArray[5]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[0], nodeArray[8]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[0], nodeArray[12]));

            result = Kernel.And(result, FddKernel.Xor(nodeArray[1], nodeArray[2]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[1], nodeArray[3]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[1], nodeArray[4]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[1], nodeArray[5]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[1], nodeArray[9]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[1], nodeArray[13]));

            result = Kernel.And(result, FddKernel.Xor(nodeArray[2], nodeArray[3]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[2], nodeArray[6]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[2], nodeArray[7]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[2], nodeArray[10]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[2], nodeArray[14]));

            result = Kernel.And(result, FddKernel.Xor(nodeArray[3], nodeArray[6]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[3], nodeArray[7]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[3], nodeArray[11]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[3], nodeArray[15]));

            result = Kernel.And(result, FddKernel.Xor(nodeArray[4], nodeArray[5]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[4], nodeArray[6]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[4], nodeArray[7]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[4], nodeArray[8]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[4], nodeArray[12]));

            result = Kernel.And(result, FddKernel.Xor(nodeArray[5], nodeArray[9]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[5], nodeArray[13]));

            result = Kernel.And(result, FddKernel.Xor(nodeArray[6], nodeArray[7]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[6], nodeArray[10]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[6], nodeArray[14]));

            result = Kernel.And(result, FddKernel.Xor(nodeArray[7], nodeArray[11]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[7], nodeArray[15]));

            result = Kernel.And(result, FddKernel.Xor(nodeArray[8], nodeArray[9]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[8], nodeArray[10]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[8], nodeArray[11]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[8], nodeArray[12]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[8], nodeArray[13]));

            result = Kernel.And(result, FddKernel.Xor(nodeArray[9], nodeArray[10]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[9], nodeArray[11]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[9], nodeArray[13]));

            result = Kernel.And(result, FddKernel.Xor(nodeArray[10], nodeArray[11]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[10], nodeArray[14]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[10], nodeArray[15]));

            result = Kernel.And(result, FddKernel.Xor(nodeArray[11], nodeArray[14]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[11], nodeArray[15]));

            result = Kernel.And(result, FddKernel.Xor(nodeArray[12], nodeArray[13]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[12], nodeArray[14]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[12], nodeArray[15]));

            result = Kernel.And(result, FddKernel.Xor(nodeArray[13], nodeArray[14]));
            result = Kernel.And(result, FddKernel.Xor(nodeArray[13], nodeArray[15]));

            result = Kernel.And(result, FddKernel.Xor(nodeArray[14], nodeArray[15]));

            long l = Kernel.SatCount(result);

            string s = PrintNice(FddKernel.AnySat(result, 3));

            return String.Format("Number of results: {0}{1}{2}", l, Environment.NewLine, s);
        }

        private string PrintNice(int[] iList)
        {
            string result = "";

            for (int i = 0; i < iList.Length; i++)
            {
                result += string.Format(" {0} ", iList[i]+1);

                if (i % 4 == 3)
                    result += Environment.NewLine;
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BddSharp.Kernel;
using BddSharp.Serializer;

/*
Sudoku layout is represented in the nodeArray. N[81]

The physical layout should be read like this

N[ 0]   N[ 1]   N[ 2]   |   N[ 3]   N[ 4]   N[ 5]   |   N[ 6]   N[ 7]   N[ 8]
N[ 9]   N[10]   N[11]   |   N[12]   N[13]   N[14]   |   N[15]   N[16]   N[17]
N[18]   N[19]   N[20]   |   N[  ]   N[  ]   N[  ]   |   N[  ]   N[  ]   N[  ]
------------------------+---------------------------+--------------------------
N[27]   N[  ]   N[  ]   |   N[  ]   N[  ]   N[  ]   |   N[  ]   N[  ]   N[  ]
N[36]   N[  ]   N[  ]   |   N[  ]   N[  ]   N[  ]   |   N[  ]   N[  ]   N[  ]
N[45]   N[  ]   N[  ]   |   N[  ]   N[  ]   N[  ]   |   N[  ]   N[  ]   N[  ]
------------------------+---------------------------+--------------------------
N[54]   N[  ]   N[  ]   |   N[  ]   N[  ]   N[  ]   |   N[  ]   N[  ]   N[  ]
N[63]   N[  ]   N[  ]   |   N[  ]   N[  ]   N[  ]   |   N[  ]   N[  ]   N[  ]
N[72]   N[  ]   N[  ]   |   N[  ]   N[  ]   N[  ]   |   N[  ]   N[  ]   N[80]

Any given node must differ from all other nodes in its collumn, all other nodes in its row
and all other nodes in its local grid. Ie. 0 have to differ from 1 2 9 10 11 18 19 20.

*/

namespace Sudoku
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void solve_Click(object sender, EventArgs e)
        {
            Kernel.Setup();
            Fdd[] nodeArray = new Fdd[81];
            for (int i = 0; i < 81; i++)
            {
                nodeArray[i] = new Fdd(9);
            }

            //vertical xors
            Bdd b = new Bdd(true);

            //horisontal xors
            Bdd a = new Bdd(true);
            for (int i = 0; i < 9; i++)
                for (int j = 1; j <= 8; j++)
                    for (int k = j; k < 9; k++)
                    {
                        a = Kernel.And(a, FddKernel.Xor(nodeArray[i * 9 + j], nodeArray[i * 9 + k]));
                        b = Kernel.And(b, FddKernel.Xor(nodeArray[i + j * 9], nodeArray[i + k * 9]));
                    }

            //own grid restriction all nodes just check 2 nodes on line below.
            Bdd c = new Bdd(true);
            for (int i = 0; i < 81; i++)
            {
                c = Kernel.And(c, FddKernel.Xor(nodeArray[i], nodeArray[getLeft(i)]));
                c = Kernel.And(c, FddKernel.Xor(nodeArray[i], nodeArray[getRight(i)]));
            }

            Bdd result = Kernel.And(Kernel.And(a, b), c);
            int[] foo = FddKernel.AnySat(result, 9);

            //BddSerializer.Serialize(result, "Sudoku");
        }

        //returns the left and right node verified for leftright bounds
        private int left(int i)                         // [x] [ ] [ ]
        {                                               // [ ] [y] [y]    
            return (i % 3 == 2) ? i + 7 : i + 10;       // e.g. x has to check y because these
        }                                               // are not horisontal nor vertical

        private int right(int i)
        {
            return (i % 3 == 0) ? i + 11 : i + 8;
        }

        private bool lastline(int i)
        {
            return i / 9 % 3 == 2;
        }

        private int getLeft(int i)
        {
            if (lastline(i))
                return left(i) - 18;        //-18 goes 2 lines up
            return left(i);
        }

        private int getRight(int i)
        {
            if (lastline(i))
                return right(i) - 18;
            return right(i);
        }
    }
}
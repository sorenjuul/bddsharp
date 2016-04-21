using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BddSharp.Kernel;
using BddSharp.Serializer;

namespace Queen_Problem
{
	public partial class Form1 : Form
	{
		static Bdd[,] X;
		static int N; // number of queens
		static Bdd queens;
		
		public Form1()
		{
			InitializeComponent();
		}

		static void BuildQueenProblem(int i, int j)
		{
			//BDD demonstration of the N-Queen chess problem.
			//-----------------------------------------------
			//The BDD variables correspond to a NxN chess board like:

			//   0    N      2N     ..  N*N-N
			//   1    N+1    2N+1   ..  N*N-N+1
			//   2    N+2    2N+2   ..  N*N-N+2
			//   ..   ..     ..     ..  ..
			//   N-1  2N-1   3N-1   ..  N*N-1

			// So for example a 4x4 is:

			//   0 4  8 12
			//   1 5  9 13
			//   2 6 10 14
			//   3 7 11 15

			// One solution is then that 2,4,11,13 should be true, meaning a queen
			// should be placed there:

			//   . X . .
			//   . . . X
			//   X . . .
			//   . . X .

			/* Build the requirements for all other fields than (i,j) assuming
			 that (i,j) has a queen */


			Bdd a = new Bdd(true);
			Bdd b = new Bdd(true);
			Bdd c = new Bdd(true);
			Bdd d = new Bdd(true);

			int k, l;

			/* No one in the same column */
			for (l = 0; l < N; l++)
			{
				if (l != j)
					a = Kernel.And(a, Kernel.Imp(X[i, j], Kernel.Not(X[i, l])));
			}

			/* No one in the same row */
			for (k = 0; k < N; k++)
			{
				if (k != i)
					b = Kernel.And(b, Kernel.Imp(X[i, j], Kernel.Not(X[k, j])));
			}

			/* No one in the same up-right diagonal */
			for (k = 0; k < N; k++)
			{
				int ll = k - i + j;
				if (ll >= 0 && ll < N)
				{
					if (k != i)
						c = Kernel.And(c, Kernel.Imp(X[i, j], Kernel.Not(X[k, ll])));
				}
			}

			/* No one in the same down-right diagonal */
			for (k = 0; k < N; k++)
			{
				int ll = i + j - k;
				if (ll >= 0 && ll < N)
				{
					if (k != i)
						d = Kernel.And(d, Kernel.Imp(X[i, j], Kernel.Not(X[k, ll])));
				}
			}

			queens = Kernel.And(queens, Kernel.And(a, Kernel.And(b, Kernel.And(c, d))));
		}

		private void button1_Click(object sender, EventArgs e)
		{
            this.Cursor = Cursors.WaitCursor;
            DateTime startSetup = DateTime.Now;
            Kernel.Setup();
            DateTime finishSetup = DateTime.Now;
            TimeSpan timeSetup = finishSetup - startSetup;
			try
			{
				N = int.Parse(textBox1.Text);
			}
			catch (FormatException)
			{
				MessageBox.Show("Input must be a integer");
				return;
			}
			DateTime start = DateTime.Now;
            SetupQueenProblem();
			DateTime finish = DateTime.Now;

			TimeSpan time = finish - start;

            textBox2.Text = "The time to calulate this problem: " + time + " Setup: " + timeSetup +
                            Environment.NewLine +
                            "There are " + Kernel.SatCount(queens) + " solutions." +
                            Environment.NewLine +
                            "Nodes in T are: " + Kernel.TCount();
            queens.Dispose();
            X = new Bdd[N,N];
            //Kernel.Done();
			//BddSerializer.Serialize(queens, "Queens");
			this.Cursor = Cursors.Arrow;
		}

        public static void SetupQueenProblem()
        {
            X = new Bdd[N, N];
			queens = new Bdd(true);
			/* Build variable array */
			for (int i = 0; i < N; i++)
			{
				for (int j = 0; j < N; j++)
				{
                    X[i, j] = new Bdd(i * N + j);
				}
			}

			/* Place a queen in each colum */
			for (int i = 0; i < N; i++)
			{
				Bdd q = new Bdd(false);
				for (int j = 0; j < N; j++)
				{
					q = Kernel.Or(q, X[i, j]);
				}
				queens = Kernel.And(queens, q);
			}

			/* Build requirements for each variable(field) */
			for (int i = 0; i < N; i++)
			{
				for (int j = 0; j < N; j++)
				{
					BuildQueenProblem(i, j);
				}
			}
		}
	}
}
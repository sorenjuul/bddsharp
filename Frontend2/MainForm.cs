using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BddSharp.AST;
using BddSharp.Parser;
using Bdd = System.Int32;
using System.IO;
using System.Threading;


namespace Frontend
{
    public partial class MainForm : Form
    {
        private IBoolExpr Ast = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void ASTButton_Click(object sender, EventArgs e)
        {


            //debugBox.Text = Ast.print();

        }

        private static void CategorizeVariables(string varlist)
        {
            varlist = varlist.Replace(" ", "");
            string[] varArray = varlist.Split(",".ToCharArray());

            foreach (string s in varArray)
            {
                BddSharp.AST.VarList.AddToDic(s);
            }
        }

        private void Execute_Click(object sender, EventArgs e)
        {
            BddSharp.AST.VarList.ResetDic();

            BoolParser parser = new BoolParser();
            Scanner sc = new Scanner();
            TextReader tr = new StringReader(ExpressionBox.Text);

            CategorizeVariables(VarsBox.Text);

            Ast = parser.Parse(sc.Scan(tr));
            BoolExpr.execute(Ast, "foo");


            if (ExpressionBox.Text == string.Empty)
            {
                MessageBox.Show("Error: No expression provided.");
            }
            else
            {
                Thread.Sleep(4000);
                System.Diagnostics.Process.Start("results\\foo.jpg", null);
            }
        }

        private void ImportBtn_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            string readFile = openFileDialog1.FileName.ToString();

            StreamReader sr = new StreamReader(readFile);
            string inStr = sr.ReadToEnd();
            sr.Close();
            ExpressionBox.Text = inStr;

        }

        private void getGraph_Click(object sender, EventArgs e)
        {

        }

        private void Debug_Click(object sender, EventArgs e)
        {
            BddSharp.AST.VarList.ResetDic();

            BoolParser parser = new BoolParser();
            Scanner sc = new Scanner();
            TextReader tr = new StringReader(ExpressionBox.Text);

            CategorizeVariables(VarsBox.Text);

            Ast = parser.Parse(sc.Scan(tr));
            DebugBox.Text = Ast.print();

        }
    }
}

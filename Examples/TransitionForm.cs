using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BddSharp.Kernel;

namespace Examples
{
    public partial class TransitionForm : Form
    {
        Dictionary<Bdd, string> test = new Dictionary<Bdd, string>();
        string system = string.Empty;
        string box;
        public TransitionForm() 
        {
            InitializeComponent();
        }

        private void RunTransitionSystem(string exampleName)
        {
            
            if (CheckInput())
            {
                //this.Cursor = Cursors.WaitCursor;
                DateTime startSetup = DateTime.Now;
                IExampleForm example = null;
                switch(exampleName)
                {
                    case "towers":
                        example = new TowersOfHanoi(int.Parse(textBox1.Text));
                        break;
                    case "milners":
                        example = new MilnersScheduler(int.Parse(textBox1.Text));
                        break;
                    case "cannibal":
                        example = new CannibalsAndMissionaries();
                        break;
                    case "knight":
                        example = new Knight(int.Parse(textBox1.Text));
                        break;
                    case "knightEasy":
                        example = new KnightEasy(int.Parse(textBox1.Text));
                        break;
                    case "queen":
                        example = new QueenProblem(int.Parse(textBox1.Text));
                        break;
                    case "fdd":
                        example = new FDDStatemachine();
                        break;
                    case "sudoku":
                        example = new Sudoku();
                        break;

                }
                string solutions = example.Run(checkBox1.Checked, checkBox2.Checked, checkBox3.Checked);
                DateTime finishSetup = DateTime.Now;
                TimeSpan duration = finishSetup - startSetup;
                box = "Duration: " + duration + Environment.NewLine + solutions;
                //this.Cursor = Cursors.Arrow;
            }
        }
        
        private void RunBtn_Click(object sender, EventArgs e)
        {
            system = "towers";
            Respond();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            system = "fdd";
            Respond();
        }

        private bool CheckInput()
        {
            int value;
            try
            {
                value = int.Parse(textBox1.Text);
                return true;
            }
            catch(Exception e)
            {
                MessageBox.Show("Input must be integer");
                return false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            system= "cannibal";
            Respond();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            system = "milners";
            Respond();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            system = "knight";
            Respond();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            system = "knightEasy"; 
            Respond();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            RunTransitionSystem(system);
        }


        private void Respond()
        {
            this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.button3.Enabled = false;
            this.button4.Enabled = false;
            this.button5.Enabled = false;
            this.button6.Enabled = false;
            this.button7.Enabled = false;
            this.RunBtn.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
            toolStripStatusLabel1.Text = "Working...";
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.button1.Enabled = true;
            this.button2.Enabled = true;
            this.button3.Enabled = true;
            this.button4.Enabled = true;
            this.button5.Enabled = true;
            this.button6.Enabled = true;
            this.button7.Enabled = true;
            this.RunBtn.Enabled = true;
            OutputBox.Text = box;
            toolStripStatusLabel1.Text = "Completed";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            system = "queen";
            Respond();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            system = "sudoku";
            Respond();
        }
    }
}

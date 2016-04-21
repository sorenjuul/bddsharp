using System;
using System.Collections.Generic;
using System.Text;

namespace BddSharp.Kernel
{
    /// <summary>
    /// Class for representing a single Fdd.
    /// </summary>
    public class Fdd
    {
        private int StartVar;

        /// <summary>
        /// Returns the number of bits the FDD can hold, notice that this is not always
        /// log 2 of its maxvalue, because FDD's has log 2 ceiling bits
        /// </summary>
        public int BitSize
        {
            get
            {
                return FddKernel.Size(this);
            }
        }
                
        /// <summary>
        /// Returns the Bdd with the lowest var contained in the FDD
        /// </summary>
        public Bdd Root
        {
            get
            {
                return new Bdd(StartVar);
            }
        }

        /// <summary>
        /// Constructor for the FDD.
        /// </summary>
        /// <param name="states">Number of different states an FDD can hold</param>
        public Fdd(int states)
        {
            this.StartVar = FddKernel.IthVar(states);
        }


        /// <summary>
        /// Returns the lowest var as an integer.
        /// </summary>
        public int Var
        {
            get
            {
                return StartVar;
            }
        }

        /// <summary>
        /// Returns a number that uniquely identify the FDD.
        /// The number is the same as the lowest var contained in the FDD.
        /// </summary>
        /// <returns>A hashcode for the Fdd</returns>
        public override int GetHashCode()
        {
            return Var;
        }

        /// <summary>
        /// Compares Fdd's based on their position in the FDD array
        /// </summary>
        /// <param name="fdd">The FDD to compare the current FDD to</param>
        /// <returns>true if the FDD's have the same index, else false</returns>
        public bool Equals(Fdd fdd)
        {
            return this.GetHashCode() == fdd.GetHashCode();       //GetHashCode is unique
        }

        /// <summary>
        /// Returns the var as a string.
        /// </summary>
        /// <returns>The var as a string</returns>
        public override string ToString()
        {
            return Var.ToString();
        }
    }
}

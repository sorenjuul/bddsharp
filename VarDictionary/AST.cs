using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using BDD = BddSharp.Kernel.Kernel;
using BddSharp.Kernel;

namespace BddSharp.AST
{
    /// <summary>
    /// Current language members
    /// EOF = End of File, VAR = Variable, DIS = Disjunction (|),
    /// CON = Conjunction (&), BIMP = Biimplication (<=>)
    /// POS = true, NEG = false, LPAR = left parateses, RPAR = right parateses
    /// NOT = not, INV_IMPL = inverse implication {0,1,0,0}, GREATER = greater than
    /// LESSER = lesser than, XOR = exclusive or, NAND = Not and {1,1,1,0},
    /// NOR = not or {1,0,0,0}
    /// </summary>
    public enum Kind { EOF, VAR, VARS, CON, DIS, BIMP, POS, NEG, LPAR, RPAR, NOT, INV_IMPL, IMPL, GREATER, LESSER, 
        XOR, NAND, NOR, FORALL, EXISTS, COMMA }

    public interface IBoolExpr
    {
        string print();
        Bdd execute();
    }

    public class BoolExpr    //Just a Factory Class
    {
        public static IBoolExpr CreateVariable(string var)
        {
            return new Var(var);
        }

        public static IBoolExpr CreateTrueVariable()
        {
            return new TrueBdd();
        }
        public static IBoolExpr CreateFalseVariable()
        {
            return new FalseBdd();
        }

        public static Bdd execute(IBoolExpr AST, string filename)
        {
            if (AST == null)
            {
                return null;        //Nothing written in textbox, or parser failed.
            }

            BDD.Setup();

            Bdd result = AST.execute();

            result.Serialize(filename, 12);

            return result;

        }
    }

    public class Bin : IBoolExpr
    {
        IBoolExpr left;
        IBoolExpr right;
        Kind op;

        public Bin(Kind op, IBoolExpr l, IBoolExpr r)
        {
            this.op = op;
            this.left = l;
            this.right = r;
        }

        public string print()
        {
            if (op == Kind.NOT)
                return op + "( " + right.print() + " )";
            else
               return op + "( " + left.print() + ", " + right.print() + " )";
        }

        public Bdd execute()
        {
            // Calls specific methods from Kernel
            Bdd l = left.execute();
            Bdd r = right.execute();
            Bdd result = new Bdd(false);

            switch (op)
            {
                case Kind.DIS:
                    result = BDD.Or(l, r); 
                    break;
                case Kind.CON:
                    result = BDD.And(l, r); 
                    break;
                case Kind.BIMP:
                    result = BDD.Equal(l, r);
                    break;
                case Kind.NAND:
                    result = BDD.Nand(l, r);
                    break;
                case Kind.XOR:
                    result = BDD.Xor(l, r);
                    break;
                case Kind.NOR:
                    result = BDD.Nor(l, r);
                    break;
                case Kind.NOT:
                    result = BDD.Not(r);
                    break;
            }
            l.Dispose();                //forced garbage collection
            r.Dispose();

            return result;
            
        }
    }

    public class Var : IBoolExpr
    {
        String varName;
        public Var(string var)
        {
            this.varName = var;
        }

        public string print()
        {
            return varName;
        }

        public Bdd execute()
        {
            return new Bdd(VarList.GetVar(varName));     //bdd in its constructor secure call of ithvar
        }
    }

    public class Forall : IBoolExpr
    {

        private List<Var> vars;
        private IBoolExpr body;

        public Forall(List<Var> Vars, IBoolExpr Body)
        {
            this.body = Body;
            this.vars = Vars;
        }

        private string PrintVars(List<Var> Vars)
        {
            StringBuilder sb = new StringBuilder("");
            Var[] tstarr = Vars.ToArray();
            for (int j = 0; j < tstarr.Length; j++)
            {
                if (j > 0) sb.Append(", ");
                sb.Append(tstarr[j].print());
            }
            return sb.ToString();
        }

        public void AddVar(string item)
        {
            Var v = new Var(item);
            vars.Add(v);
        }

        public string print()
        {
            return "ForAll" + "(" + PrintVars(vars) + ") . " + body.print();
        }

        public Bdd execute()
        {
            Bdd b = body.execute();
            Bdd result = new Bdd (false);

            foreach (Var v in vars)
            {
                //Bdd b2 = v.execute();
                //result = BDD.And(BDD.Restrict(b.Var, b2.Var, true), BDD.Restrict(b.Var, b2.Var, false));
                result = BDD.Or(result, BDD.ForAll(v.execute().Var, b));
            }
            b.Dispose();
            return result;
        }
    }

    public class Exists : IBoolExpr
    {

        private List<Var> vars;
        private IBoolExpr body;

        public Exists(List<Var> Vars, IBoolExpr Body)
        {
            this.body = Body;
            this.vars = Vars;
        }

        private string PrintVars(List<Var> Vars)
        {
            StringBuilder sb = new StringBuilder("");
            Var[] tstarr = Vars.ToArray();
            for (int j = 0; j < tstarr.Length; j++)
            {
                if (j > 0) sb.Append(", ");
                sb.Append(tstarr[j].print());
            }
            return sb.ToString();
        }

        public void AddVar(string item)
        {
            Var v = new Var(item);
            vars.Add(v);
        }

        public string print()
        {
            return "Exists" + "(" + PrintVars(vars) + ") . " + body.print();
        }

        public Bdd execute()
        {
            Bdd b = body.execute();
            Bdd result = new Bdd (false);

            foreach (Var v in vars)
            {
                //Bdd b2 = v.execute();
                //result = BDD.Or(BDD.Restrict(b.Var, b2.Var, true), BDD.Restrict(b.Var, b2.Var, false));
                result = BDD.Or(result, BDD.ForAll(v.execute().Var, b));
            }
            b.Dispose();

            return result;
        }
    }

    public class TrueBdd : IBoolExpr
    {
        public TrueBdd()
        {
        }

        public string print()
        {
            return "true";
        }

        public Bdd execute()
        {
            return new Bdd(true);
        }
    }

    public class FalseBdd : IBoolExpr
    {
        public FalseBdd()
        {
        }

        public string print()
        {
            return "false";
        }

        public Bdd execute()
        {
            return new Bdd(false);
        }

    }

}

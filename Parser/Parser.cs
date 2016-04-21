using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using BddSharp.AST;
using TokenStream = System.Collections.Generic.IEnumerator<BddSharp.Parser.Token>;

namespace BddSharp.Parser
{
    public class BoolParser
    {
        private List<Var> vars = new List<Var>();

        /// <summary>
        /// Returns the Scanner input in the form of an abstract syntax tree.
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public IBoolExpr Parse(TokenStream ts)
        {
            ts.MoveNext();
            IBoolExpr result = Exists(ts);
            //return result;
            switch (ts.Current.kind)
            {
                case Kind.EOF: return result;
                default: throw new ApplicationException("Parse error: " + ts.Current);
            }
        }

        IBoolExpr Exists(TokenStream ts)
        {
            return ExistsOpt(Forall(ts), ts);
        }

        IBoolExpr ExistsOpt(IBoolExpr inStr, TokenStream ts)
        {
            if (ts.Current.kind == Kind.EXISTS)
            {
                ts.MoveNext();
                if (ts.Current.kind != Kind.LPAR)
                {
                    throw new ApplicationException("Expected Kind.LPAR");
                }
                else
                {
                    return VarList(ts, Kind.EXISTS);
                }
            }
            else
            {
                return inStr;
            }
        }

        IBoolExpr Forall(TokenStream ts)
        {
            return ForallOpt(Disj(ts), ts);
        }

        IBoolExpr ForallOpt(IBoolExpr inStr, TokenStream ts)
        {
            if (ts.Current.kind == Kind.FORALL)
            {
                //vars.Clear();
                ts.MoveNext();
                if (ts.Current.kind != Kind.LPAR)
                {
                    throw new ApplicationException("Expected Kind.LPAR");
                }
                else
                {
                    return VarList(ts, Kind.FORALL);
                }
            }
            else
            {
                return inStr;
            }
        }

        private IBoolExpr VarList(TokenStream ts,  Kind k)
        {
            ts.MoveNext();
            if (ts.Current.kind == Kind.VAR)
            {
                try
                {
                    Var v = new Var(ts.Current.nString);
                    vars.Add(v);
                    ts.MoveNext();
                }
                catch (Exception ex)
                {
                    string err = ex.ToString();
                }
                if (ts.Current.kind != Kind.RPAR)
                {
                    if (ts.Current.kind != Kind.COMMA)
                    {
                        throw new ApplicationException("Expected Kind.COMMA");
                    }
                    else
                    {
                        return VarList(ts, k);
                    }
                }
                else
                {
                    
                    ts.MoveNext();
                    if (k == Kind.FORALL)
                    {
                        return ForallOpt(new Forall(VarsCopy(vars), Exists(ts)), ts);
                    }
                    else
                    {
                        return ExistsOpt(new Exists(VarsCopy(vars), Exists(ts)), ts);
                    }
                }
            }
            else
            {
                throw new ApplicationException("Expected Kind.VAR");
            }
        }

        /// <summary>
        /// Used in VarList to enable usage of several Exist's or Forall's in an expression.
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        private List<Var> VarsCopy(List<Var> lst)
        {
            List<Var> outLst = new List<Var>();

            foreach (Var v in lst)
            {
                outLst.Add(v);
            }
            lst.Clear();

            return outLst;
        }

        /// <summary>
        /// Disjunction (OR)
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        IBoolExpr Disj(TokenStream ts)
        {
            return DisjOpt(Nor(ts), ts);
        }

        IBoolExpr DisjOpt(IBoolExpr inStr, TokenStream ts)
        {
            if (ts.Current.kind == Kind.DIS)
            {
                ts.MoveNext();
                return DisjOpt(new Bin(Kind.DIS, inStr, Nor(ts)), ts);
            }
            else
            {
                return inStr;
            }
        }
        /// <summary>
        /// NOR !|
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        IBoolExpr Nor(TokenStream ts)
        {
            return NorOpt(Conj(ts), ts);
        }

        IBoolExpr NorOpt(IBoolExpr inStr, TokenStream ts)
        {
            if (ts.Current.kind == Kind.NOR)
            {
                ts.MoveNext();
                return NorOpt(new Bin(Kind.NOR, inStr, Conj(ts)), ts);
            }
            else
            {
                return inStr;
            }
        }

        /// <summary>
        /// Conjuction (AND)
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        IBoolExpr Conj(TokenStream ts)
        {
            return ConjOpt(Nand(ts), ts);
        }

        IBoolExpr ConjOpt(IBoolExpr inStr, TokenStream ts)
        {
            if (ts.Current.kind == Kind.CON)
            {
                ts.MoveNext();
                return ConjOpt(new Bin(Kind.CON, inStr, Nand(ts)), ts);
            }
            else
            {
                return inStr;
            }
        }
        /// <summary>
        /// NAND !&
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        IBoolExpr Nand(TokenStream ts)
        {
            return NandOpt(Bimp(ts), ts);
        }

        IBoolExpr NandOpt(IBoolExpr inStr, TokenStream ts)
        {
            if (ts.Current.kind == Kind.NAND)
            {
                ts.MoveNext();
                return NandOpt(new Bin(Kind.NAND, inStr, Bimp(ts)), ts);
            }
            else
            {
                return inStr;
            }
        }

        /// <summary>
        /// Bi-implication a.k.a <=>
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        IBoolExpr Bimp(TokenStream ts)
        {
            return BimpOpt(Impl(ts), ts);
        }

        IBoolExpr BimpOpt(IBoolExpr inStr, TokenStream ts)
        {
            if (ts.Current.kind == Kind.BIMP)
            {
                ts.MoveNext();
                return BimpOpt(new Bin(Kind.BIMP, inStr, Impl(ts)), ts);
            }
            else
            {
                return inStr;
            }
        }

        /// <summary>
        /// Implication =>
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        IBoolExpr Impl(TokenStream ts)
        {
            return ImplOpt(InvImpl(ts), ts);
        }

        IBoolExpr ImplOpt(IBoolExpr inStr, TokenStream ts)
        {
            if (ts.Current.kind == Kind.IMPL)
            {
                ts.MoveNext();
                return ImplOpt(new Bin(Kind.IMPL, inStr, InvImpl(ts)), ts);
            }
            else
            {
                return inStr;
            }
        }

        /// <summary>
        /// Inverse implication
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        IBoolExpr InvImpl(TokenStream ts)
        {
            return InvImplOpt(Xor(ts), ts);
        }

        IBoolExpr InvImplOpt(IBoolExpr inStr, TokenStream ts)
        {
            if (ts.Current.kind == Kind.INV_IMPL)
            {
                ts.MoveNext();
                return InvImplOpt(new Bin(Kind.INV_IMPL, inStr, Xor(ts)), ts);
            }
            else
            {
                return inStr;
            }
        }

        /// <summary>
        /// XOR !=
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        IBoolExpr Xor(TokenStream ts)
        {
            return XorOpt(Not(ts), ts);
        }

        IBoolExpr XorOpt(IBoolExpr inStr, TokenStream ts)
        {
            if (ts.Current.kind == Kind.XOR)
            {
                ts.MoveNext();
                return XorOpt(new Bin(Kind.XOR, inStr, Not(ts)), ts);
            }
            else
            {
                return inStr;
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        IBoolExpr Not(TokenStream ts)
        {
            return NotOpt(Prim(ts), ts);
        }

        IBoolExpr NotOpt(IBoolExpr inStr, TokenStream ts)
        {
            if (ts.Current.kind == Kind.NOT)
            {
                ts.MoveNext();
                return NotOpt(new Bin(Kind.NOT, null, Prim(ts)), ts);   
            }
            else
            {
                return inStr;
            }
        }

        /// <summary>
        /// PRIM
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        IBoolExpr Prim(TokenStream ts)
        {

            switch (ts.Current.kind)
            {
                case Kind.VAR:
                    string varName = ts.Current.nString.ToString();
                    ts.MoveNext();
                    return BoolExpr.CreateVariable(varName);
                case Kind.POS: ts.MoveNext(); return BoolExpr.CreateTrueVariable();
                case Kind.NEG: ts.MoveNext(); return BoolExpr.CreateFalseVariable();
                case Kind.LPAR: ts.MoveNext();
                    IBoolExpr ev = Exists(ts);
                    if (ts.Current.kind != Kind.RPAR)
                    {
                        throw new ApplicationException("Expected Kind.RPAR");
                    }
                    ts.MoveNext(); return ev;

                case Kind.RPAR: ts.MoveNext(); return null;
            }
            return null;
        }
    }
}

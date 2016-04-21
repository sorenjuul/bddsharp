using System;
using System.Collections.Generic;
using System.Text;
using BddSharp.AST;

namespace BddSharp.Parser
{

    public struct Token
    {
        public readonly AST.Kind kind;
        public readonly String nString;

        public Token(Kind k)
        {
            kind = k;
            nString = string.Empty;
        }
        //public Token(Char c)
        //{
        //    kind = Kind.POS;
        //    nString = string.Empty;
        //}

        public Token(string s)
        {
            kind = Kind.VAR;
            nString = s;
        }

        public static Token FromKind(Kind k)
        {
            return new Token(k);
        }

        public static Token FromString(string s)
        {
            return new Token(s);
        }
    }
}

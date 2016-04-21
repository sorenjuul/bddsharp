using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using BddSharp.AST;

namespace BddSharp.Parser
{
    public class Scanner
    {
        static string ScanVarName(TextReader reader)
        {
            StringBuilder sb = new StringBuilder();
            while (Char.IsLetterOrDigit((char)reader.Peek()))
                sb.Append((char)reader.Read());
            return sb.ToString();
        }

        /// <summary>
        /// Returns a token list for all the Kinds found in the in the input string.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public IEnumerator<Token> Scan(TextReader reader)
        {
            while (reader.Peek() != -1)
            {
                if (Char.IsWhiteSpace((Char)reader.Peek()))
                {
                    reader.Read();
                }
                else if (Char.IsLetter((Char)reader.Peek()))
                {
                    string VarName = ScanVarName(reader);

                    if (VarName.ToLower() == "exists")
                    {
                        yield return Token.FromKind(Kind.EXISTS);
                    }
                    else if (VarName.ToLower() == "forall")
                    {
                        yield return Token.FromKind(Kind.FORALL);
                    }
                    else
                    {
                        yield return Token.FromString(VarName);
                    }
                }
                else if (Char.Equals((Char)reader.Peek(), '<'))
                {
                    
                    reader.Read();
                    if (Char.Equals((Char)reader.Peek(), '='))
                    {
                        reader.Read();
                        yield return Token.FromKind(Kind.INV_IMPL);
                    }
                    else
                    {
                        yield return Token.FromKind(Kind.LESSER);
                    }
                }
                else if (Char.Equals((Char)reader.Peek(), '='))
                {
                    reader.Read();
                    if (Char.Equals((Char)reader.Peek(), '>'))
                    {
                        reader.Read();
                        yield return Token.FromKind(Kind.IMPL);
                    }
                    else
                    {
                        yield return Token.FromKind(Kind.BIMP);
                    }
                }
                else if (Char.Equals((Char)reader.Peek(), '!'))
                {
                    reader.Read();
                    char c = (Char)reader.Peek();
                    switch (c)
                    {
                        case '&': reader.Read(); yield return Token.FromKind(Kind.NAND); break;
                        case '|': reader.Read(); yield return Token.FromKind(Kind.NOR); break;
                        case '=': reader.Read(); yield return Token.FromKind(Kind.XOR); break;
                        default:
                            yield return Token.FromKind(Kind.NOT); break;
                    }
                }
                else
                {
                    char c = (char)reader.Read();
                    switch (c)
                    {
                        case '&': yield return Token.FromKind(Kind.CON); break;
                        case '>': yield return Token.FromKind(Kind.GREATER); break;
                        case '|': yield return Token.FromKind(Kind.DIS); break;
                        case '0': yield return Token.FromKind(Kind.NEG); break;
                        case '1': yield return Token.FromKind(Kind.POS); break;
                        case '(': yield return Token.FromKind(Kind.LPAR); break;
                        case ')': yield return Token.FromKind(Kind.RPAR); break;
                        case ',': yield return Token.FromKind(Kind.COMMA); break;
                        default:
                            throw new ApplicationException("Illegal Character: '" + c + "'!!");
                    }
                }
            }
            yield return Token.FromKind(Kind.EOF);
            reader.Close();
        }
    }
}

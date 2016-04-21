using System;
using System.Collections.Generic;
using System.Text;

namespace BddSharp.AST
{
    public static class VarList
    {
        //Will Contain Key Name and Var Number
        public static Dictionary<string, int> Vars = new Dictionary<string, int>();
        private static int max = 1;

        public static void AddToDic(string VarName)
        {
            if (!Vars.ContainsKey(VarName))
            {
                Vars.Add(VarName, max++);
            }
        }

        public static void ResetDic()
        {
            Vars.Clear();
            max = 1;
        }

        //gets Bdd of variable. Non existing variables are inserted into hashmap
        public static int GetVar(string VarName)
        {
            if (!Vars.ContainsKey(VarName))
            {
                AddToDic(VarName);
            }

            return Vars[VarName];
        }
    }
}

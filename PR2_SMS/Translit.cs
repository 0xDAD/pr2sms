using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace PR2_SMS
{
    class Translit
    {
        static Dictionary<char, string> dict;

        static  Translit()
        {
            dict = new Dictionary<char, string>();

            dict.Add('�',"A");
            dict.Add('�',"B");
            dict.Add('�',"V");
            dict.Add('�',"G");
            dict.Add('�',"D");
            dict.Add('�',"E");
            dict.Add('�',"E");
            dict.Add('�',"Zh");
            dict.Add('�',"Z");
            dict.Add('�',"I");
            dict.Add('�',"J");
            dict.Add('�',"K");
            dict.Add('�',"L");
            dict.Add('�',"M");
            dict.Add('�',"N");
            dict.Add('�',"O");
            dict.Add('�',"P");
            dict.Add('�',"R");
            dict.Add('�',"S");
            dict.Add('�',"T");
            dict.Add('�',"U");
            dict.Add('�',"F");
            dict.Add('�',"H");
            dict.Add('�',"C");
            dict.Add('�',"Ch");
            dict.Add('�',"Sh");
            dict.Add('�',"Sch");
            dict.Add('�',"'");
            dict.Add('�',"Y");
            dict.Add('�',"'");
            dict.Add('�',"E");
            dict.Add('�',"Yu");
            dict.Add('�',"Ya");

            dict.Add('�', "a");
            dict.Add('�', "b");
            dict.Add('�', "v");
            dict.Add('�', "g");
            dict.Add('�', "d");
            dict.Add('�', "e");
            dict.Add('�', "e");
            dict.Add('�', "zh");
            dict.Add('�', "z");
            dict.Add('�', "i");
            dict.Add('�', "j");
            dict.Add('�', "k");
            dict.Add('�', "l");
            dict.Add('�', "m");
            dict.Add('�', "n");
            dict.Add('�', "o");
            dict.Add('�', "p");
            dict.Add('�', "r");
            dict.Add('�', "s");
            dict.Add('�', "t");
            dict.Add('�', "u");
            dict.Add('�', "f");
            dict.Add('�', "h");
            dict.Add('�', "c");
            dict.Add('�', "ch");
            dict.Add('�', "sh");
            dict.Add('�', "sch");
            dict.Add('�', "'");
            dict.Add('�', "y");
            dict.Add('�', "'");
            dict.Add('�', "e");
            dict.Add('�', "yu");
            dict.Add('�', "ya");

           //dict.Add('\"', "\);
        }
        public static string Convert(string instr)
        {
            string str = string.Empty;            
            for (int i=0;i<instr.Length;i++)
            {
                if(dict.ContainsKey(instr[i]))
                {
                    str=str.Insert(str.Length, dict[instr[i]]);
                }
                else
                    str=str.Insert(str.Length, instr[i].ToString());
            }
            return str;
        }
    }
}

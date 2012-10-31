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

            dict.Add('À',"A");
            dict.Add('Á',"B");
            dict.Add('Â',"V");
            dict.Add('Ã',"G");
            dict.Add('Ä',"D");
            dict.Add('Å',"E");
            dict.Add('¨',"E");
            dict.Add('Æ',"Zh");
            dict.Add('Ç',"Z");
            dict.Add('È',"I");
            dict.Add('É',"J");
            dict.Add('Ê',"K");
            dict.Add('Ë',"L");
            dict.Add('Ì',"M");
            dict.Add('Í',"N");
            dict.Add('Î',"O");
            dict.Add('Ï',"P");
            dict.Add('Ð',"R");
            dict.Add('Ñ',"S");
            dict.Add('Ò',"T");
            dict.Add('Ó',"U");
            dict.Add('Ô',"F");
            dict.Add('Õ',"H");
            dict.Add('Ö',"C");
            dict.Add('×',"Ch");
            dict.Add('Ø',"Sh");
            dict.Add('Ù',"Sch");
            dict.Add('Ú',"'");
            dict.Add('Û',"Y");
            dict.Add('Ü',"'");
            dict.Add('Ý',"E");
            dict.Add('Þ',"Yu");
            dict.Add('ß',"Ya");

            dict.Add('à', "a");
            dict.Add('á', "b");
            dict.Add('â', "v");
            dict.Add('ã', "g");
            dict.Add('ä', "d");
            dict.Add('å', "e");
            dict.Add('¸', "e");
            dict.Add('æ', "zh");
            dict.Add('ç', "z");
            dict.Add('è', "i");
            dict.Add('é', "j");
            dict.Add('ê', "k");
            dict.Add('ë', "l");
            dict.Add('ì', "m");
            dict.Add('í', "n");
            dict.Add('î', "o");
            dict.Add('ï', "p");
            dict.Add('ð', "r");
            dict.Add('ñ', "s");
            dict.Add('ò', "t");
            dict.Add('ó', "u");
            dict.Add('ô', "f");
            dict.Add('õ', "h");
            dict.Add('ö', "c");
            dict.Add('÷', "ch");
            dict.Add('ø', "sh");
            dict.Add('ù', "sch");
            dict.Add('ú', "'");
            dict.Add('û', "y");
            dict.Add('ü', "'");
            dict.Add('ý', "e");
            dict.Add('þ', "yu");
            dict.Add('ÿ', "ya");

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

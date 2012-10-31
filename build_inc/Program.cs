using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace build_inc
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 0 && File.Exists(args[0]))
            {
                try
                {
                    FileStream fs = new FileStream(args[0], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader sr = new StreamReader(fs);
                    string source = sr.ReadToEnd();
                    fs.Close();
                    int sidx = source.IndexOf("[assembly: AssemblyVersion");
                    if (-1 == sidx)
                    {
                        Console.WriteLine("There is no proper string in file.");
                        fs.Close();
                        return -1;
                    }
                    int eidx = source.IndexOf("]", sidx);
                    if (-1 == sidx)
                    {
                        Console.WriteLine("There is no proper string in file.");
                        fs.Close();
                        return -1;
                    }
                   int fp =  source.IndexOf(".", sidx, eidx - sidx);
                   fp = source.IndexOf(".", fp+1, eidx - fp-1);
                   int lp = source.IndexOf(".", fp+1, eidx-fp-1);
                   String buildNum = source.Substring(fp+1, lp - fp-1);
                   source = source.Remove(fp + 1, lp - fp - 1);
                   int bn = Int32.Parse(buildNum);
                   bn++;
                   source = source.Insert(fp+1, bn.ToString());

                   fs = new FileStream(args[0], FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
                   StreamWriter sw = new StreamWriter(fs);
                   sw.Write(source);
                   sw.Close();

                }
                catch
                {

                }

            }
            else
            {
                Console.WriteLine("General Error: wrong command line argument");
                return -1;
            }
            return 0;
        }
        
    }
}

using System;
using System.Drawing;
using System.Threading;
using System.IO;
using Word = Microsoft.Office.Interop.Word;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace echoAutomatedSuite
{
    public class TextFileOps
    {
        public static void Open(string pth)
        {
            FileStream fs;

            if (File.Exists(pth))
            {
                File.Delete(pth);
                fs = File.Create(pth);
            }
            else
            {
                fs = File.Create(pth);
            }

            fs.Close();
            Thread.Sleep(50);
        }

        //Write to the console and text file
        //each case in the switch statement takes in a number (clrCode). This will determine the font, size, and color of the 
        //entry into the result file
        public static void Write(string pth, string inString, int clrCode)
        {
            StreamWriter sw;
            sw = File.AppendText(pth);

            switch(clrCode)
            {
                case 1:
                    //Write passed step (green)
                    sw.WriteLine("<span style=\"font-family:verdana;font-size:75%;color:#006400\" class=\"jqtree-title\">" + inString + "</span>");  //#006400
                    break;
                case -1:
                    //Write failed step (red)
                    sw.WriteLine("<span style=\"font-family:verdana;font-size:75%;color:#FF0000\" class=\"jqtree-title\">"  + inString + "</span>");  //#FF0000
                    break;
                case 2:
                    //Write condidional passed step ()
                    sw.WriteLine("<span style=\"font-family:verdana;font-size:75%;color:#000099\" class=\"jqtree-title\">" + inString + "</span>");  //#006400
                    break;
                case 5:
                    //Write failed step (red)
                    sw.WriteLine("<span style=\"font-family:verdana;font-size:75%;color:#3300FF\" class=\"jqtree-title\"><b>" + inString + "</b></span>");  //#FF0000
                    break;
                case 20:
                    //Announce checkpoint (large font)
                    sw.WriteLine("<span style=\"font-family:verdana;font-size:125%;color:#000000\" class=\"jqtree-title\">" + inString + "</span>");  //#FF0000
                    break;
                case 75:
                    //Write step info(bold)
                    sw.WriteLine("<span style=\"font-family:verdana;font-size:75%;color:#000000\" class=\"jqtree-title\"><b>" + inString + "</b></span>");   //#000000
                    break;
                case 80:
                    //Write step info
                    sw.WriteLine("<span style=\"font-family:verdana;font-size:75%;color:#000000\" class=\"jqtree-title\">" + inString + "</span>");   //#000000
                    break;
                case 90:
                    //Write step info (tab in - for argument list)
                    sw.WriteLine("<p style=\"font-family:verdana;font-size:75%;color:#000000\" class=\"jqtree-title\">" + inString + "</p>");
                    break;
                case 95:
                    //Write step info(bold)
                    sw.WriteLine("<p style=\"font-family:verdana;font-size:75%;color:#000000\"><b>" + inString + "</b></p>");   //#000000
                    break;
                case 100:
                    //Write header and footer tags with no info
                    sw.WriteLine(inString);
                    break;
                default:
                    //Standard font
                    sw.WriteLine("<p style=\"font-family:verdana;font-size:75%;color:#000000\">" + inString + "</p>");  //#000000
                    break; 
            }

            sw.Close();
            Thread.Sleep(75);
        }
    }
}

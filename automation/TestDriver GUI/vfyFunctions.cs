using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;
using NUnit.Framework;
using Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace echoAutomatedSuite

{
    public class Recorder
    {
        static int clrValue;

        public static void vfyNav(bool rslt, string fldName, string objType, string stpNum, string pth)
        {
            TextFileOps.Write(pth, "Searching for the " + fldName + objType + "........(" + stpNum + ")", 0);
        }

        public static void inputTextVerify(bool rslt, string fldName, string inString, string stpNum, string pth)
        {
            if (rslt == true)
            {
                TextFileOps.Write(pth, "The " + fldName + " table was found......", clrValue = 0);
                TextFileOps.Write(pth, "Succesfully input grades into the " + fldName + " table. \r\n", 1);
            }
            else if (rslt == false)
            {
                TextFileOps.Write(pth, "The " + fldName + " field was not found", clrValue = 0);
                TextFileOps.Write(pth, "Could not input text..........\r", clrValue = -1);
                clrValue = 0;
            }
        }

        public static void insertGradesVerify(bool rslt, string fldName, string inString, string stpNum, string pth)
        {
            if (rslt == true)
            {
                TextFileOps.Write(pth, "Succesfully input grades into the " + fldName + " field. \r\n", 1);
            }
            else if (rslt == false)
            {
                TextFileOps.Write(pth, "The " + fldName + " field was not found", clrValue = 0);
                TextFileOps.Write(pth, "Could not input text..........\r", clrValue = -1);
                clrValue = 0;
            }
        }

        public static void btnPressVerify(bool rslt, string btnName, string stpNum, string pth)
        {
            if (rslt == true)
            {
                TextFileOps.Write(pth, "\r\n", clrValue);
                //TextFileOps.Write(pth, "The " + btnName + " button was found: (" + stpNum + ")\r", clrValue = 0);
                TextFileOps.Write(pth, "Successfully pressed the '" + btnName + "' button\r\n", 1);
            }
            else if (rslt == false)
            {
                TextFileOps.Write(pth, "\r\n", clrValue);
                TextFileOps.Write(pth, "The " + btnName + " button was not found\r", clrValue = 0);

                TextFileOps.Write(pth, "Could not press button..........\r", clrValue = -1);
                clrValue = 0;
            }
        }

        public static void gradeVerify(bool rslt, string corrgrade, string pct, string upper, string lower, string dispgrade, string stdntName, string pth)
        {
            if (rslt == true)
            {
                TextFileOps.Write(pth, "PASS: The grade '" + dispgrade + "' was displayed in the grade table for student, '" + stdntName + "'. " +
                "The percentage was " + pct + "% and is between the upper bound, " + upper + ", and lower bound, " + lower + ", for a '" + corrgrade + "'.", 1);
            }
            else if (rslt == false)
            {
                TextFileOps.Write(pth, "FAIL: The grade '" + dispgrade + "' was displayed in the grade table. " +
                "The correct grade would be a " + corrgrade + " for a percentage of " + pct + ".", -1);
                clrValue = 0;
            }
        }

        public static void outcomeVerify(bool rslt, string itmMatch, string stdntName, string outcome, string earnScore, string totPoss, string failCalc, string pth)
        {
            if (rslt == true)
            {
                TextFileOps.Write(pth, "PASS: The percentage '" + itmMatch + "' was displayed for outcome " + outcome + " for student, '" + stdntName + "', and is correct. This was calculated from " +
                    earnScore + " points earned with " + totPoss + " possible points in the outcome", 1);
            }
            else if (rslt == false)
            {
                TextFileOps.Write(pth, "FAIL: The percentage '" + itmMatch + "' was displayed for outcome " + outcome + "  for student, '" + stdntName + "', and is incorrect. This was calculated from " +
                earnScore + " points earned with " + totPoss + " possible points. The correct percentage is " + failCalc + "%", -1);
                clrValue = 0;
            }
        }

        public static void pctVerify(bool rslt, string scrnPct, string stdntName, string vfyPct, string pth)
        {
            if (rslt == true)
            {
                TextFileOps.Write(pth, "<b>PASS: The percentage '" + scrnPct + "' was displayed for student , '" + stdntName + "'. This final calculated percentage was correct</b>", 1);
            }
            else if (rslt == false)
            {
                TextFileOps.Write(pth, "<b>FAIL: The percentage '" + scrnPct + "' was displayed for student , '" + stdntName + "'. This final calculated percentage was incorrect. " +
                    "The calculated grade from the testing application was calculated to : " + vfyPct + "</b>", -1);
                clrValue = 0;
            }
        }

        public static void weightWrite(string outcomeName, string weight, string pth)
        {
            TextFileOps.Write(pth, "The outcome '" + outcomeName + "' was calculated at " + weight + "% of the grade", 80);
            clrValue = 0;
        }

        public static void vfyLogin(bool rslt, string userName, string stpNum, string pth)
        {
            int clrIndex;

            clrIndex = 0;
            //TextFileOps.Write(pth, "<br />", clrIndex);
            TextFileOps.Write(pth, "Logging into Echo as " + userName + ": (" + stpNum + ")", 0);
            TextFileOps.Write(pth, "<br />", clrIndex);
        }

        public static void tblVerify(bool rslt, string inString, string offsetItem, string scrnName, string pth, string getNeg)
        {
            if (rslt == true && (getNeg != "N" ))
            {
                TextFileOps.Write(pth, "PASS: The table item " + '"' + inString + '"' + " is present on the " + scrnName + " screen.\r", 1);
                clrValue = 0;
            }
            else if (rslt == false && (getNeg != "N"))
            {
                TextFileOps.Write(pth, "FAIL: The table item " + '"' + inString + '"' + " is not present on the " + scrnName + " screen. The item present was '" + offsetItem + "'\r", -1);
                clrValue = 0;
            }
            else if (rslt == false && (getNeg == "N"))
            {
                TextFileOps.Write(pth, "PASS: The table item " + '"' + inString + '"' + " is not present on the " + scrnName + " screen and is not expected.\r", 1);
                clrValue = 0;
            }
            else if (rslt == true && (getNeg == "N"))
            {
                TextFileOps.Write(pth, "FAIL: The table item " + '"' + inString + '"' + " is present on the " + scrnName + " screen and is not expected.\r", -1);
                clrValue = 0;
            }
        }

        public static void btnVerify(bool rslt, string scrnName, string inString, string pth, string getNeg)
        {
            if (rslt == true && (getNeg != "N"))
            {
                TextFileOps.Write(pth, "PASS: The button " + '"' + inString + '"' + " is present on the " + scrnName + " screen.\r", 1);
                clrValue = 0;
            }
            else if (rslt == false && (getNeg != "N"))
            {
                TextFileOps.Write(pth, "FAIL: The button " + '"' + inString + '"' + " is not present on the " + scrnName + " screen.\r", -1);
                clrValue = 0;
            }
            else if (rslt == false && (getNeg == "N"))
            {
                TextFileOps.Write(pth, "PASS:  The button " + '"' + inString + '"' + " is not present on the " + scrnName + " screen and is not expected.\r", 1);
                clrValue = 0;
            }
            else if (rslt == true && (getNeg == "N"))
            {
                TextFileOps.Write(pth, "FAIL: The button " + '"' + inString + '"' + " is present on the " + scrnName + " screen and is not expected.\r", -1);
                clrValue = 0;
            }


        }

        public static void lnkVerify(bool rslt, string scrnName, string inString, string pth, string getNeg)
        {
            if (rslt == true && (getNeg != "N"))
            {
                TextFileOps.Write(pth, "PASS: The link " + '"' + inString + '"' + " is present on the "  + scrnName + " screen.\r", 1);
                clrValue = 0;
            }
            else if (rslt == false && (getNeg != "N"))
            {
                TextFileOps.Write(pth, "FAIL: The link " + '"' + inString + '"' + " is not present on the "  + scrnName + " screen.\r", -1);
                clrValue = 0;
            }
            else if (rslt == false && (getNeg == "N"))
            {
                TextFileOps.Write(pth, "PASS:  The link " + '"' + inString + '"' + " is not present on the " + scrnName + " screen and is not expected.\r", 1);
                clrValue = 0;
            }
            else if (rslt == true && (getNeg == "N"))
            {
                TextFileOps.Write(pth, "FAIL: The link " + '"' + inString + '"' + " is present on the " + scrnName + " screen and is not expected.\r", -1);
                clrValue = 0;
            }

        }

        public static void blockVerify(bool rslt, string scrnName, string inString, string pth, string getNeg)
        {
            if (rslt == true && (getNeg != "N"))
            {
                TextFileOps.Write(pth, "PASS: The block " + '"' + inString + '"' + " is present on the " + scrnName + " screen.\r", 1);
                clrValue = 0;
            }
            else if (rslt == false && (getNeg != "N"))
            {
                TextFileOps.Write(pth, "FAIL: The block " + '"' + inString + '"' + " is not present on the " + scrnName + " screen.\r", -1);
                clrValue = 0;
            }
            else if (rslt == false && (getNeg == "N"))
            {
                TextFileOps.Write(pth, "PASS: The block " + '"' + inString + '"' + " is not present on the " + scrnName + " screen and is not expected.\r", 1);
                clrValue = 0;
            }
            else if (rslt == true && (getNeg == "N"))
            {
                clrValue = -1;
                TextFileOps.Write(pth, "FAIL: The block " + '"' + inString + '"' + " is present on the " + scrnName + " screen and is not expected.\r", -1);
                clrValue = 0;
            }
        }

        public static void imgVerify(bool rslt, string scrnName, string inString, string pth, string getNeg)
        {
            if (rslt == true && (getNeg != "N"))
            {
                TextFileOps.Write(pth, "PASS: The image " + '"' + inString + '"' + " is present on the " + scrnName + " screen.\r", 1);
                clrValue = 0;
            }
            else if (rslt == false && (getNeg != "N"))
            {
                TextFileOps.Write(pth, "FAIL: The image " + '"' + inString + '"' + " is not present on the " + scrnName + " screen.\r", -1);
                clrValue = 0;
            }
            else if (rslt == false && (getNeg == "N"))
            {
                TextFileOps.Write(pth, "PASS: The image " + '"' + inString + '"' + " is not present on the " + scrnName + " screen and is not expected.\r", 1);
                clrValue = 1;
            }
            else if (rslt == true && (getNeg == "N"))
            {
                TextFileOps.Write(pth, "FAIL: The image " + '"' + inString + '"' + " is present on the " + scrnName + " screen and is not expected.\r", -1);
                clrValue = 0;
            }
        }

        public static void tabVerify(bool rslt, string scrnName, string inString, string pth, string getNeg)
        {
            if (rslt == true && (getNeg != "N"))
            {
                TextFileOps.Write(pth, "PASS: The tab " + '"' + inString + '"' + " is present on the " + scrnName + " screen.\r", 1);
                clrValue = 0;
            }
            else if (rslt == false && (getNeg != "N"))
            {
                TextFileOps.Write(pth, "FAIL: The tab " + '"' + inString + '"' + " is not present on the " + scrnName + " screen..\r", -1);
                clrValue = 0;
            }
            else if (rslt == false && (getNeg == "N"))
            {
                TextFileOps.Write(pth, "PASS: The tab " + '"' + inString + '"' + " is not present on the " + scrnName + " screen and is not expected.\r", 1);
                clrValue = 0;
            }
            else if (rslt == true && (getNeg == "N"))
            {
                TextFileOps.Write(pth, "FAIL: The tab " + '"' + inString + '"' + " is present on the " + scrnName + " screen and is not expected.\r", -1);
                clrValue = 0;
            }
        }

        public static void fldVerify(bool rslt, string scrnName, string inString, string pth, string getNeg)
        {
            if (rslt == true && (getNeg != "N"))
            {
                TextFileOps.Write(pth, "PASS: The field " + '"' + inString + '"' + " is present on the " + scrnName + " screen.\r", 1);
                clrValue = 0;
            }
            else if (rslt == false && (getNeg != "N"))
            {
                TextFileOps.Write(pth, "FAIL: The field " + '"' + inString + '"' + " is not present on the " + scrnName + " screen..\r", -1);
                clrValue = 0;
            }
            else if (rslt == false && (getNeg == "N"))
            {
                TextFileOps.Write(pth, "PASS: The field " + '"' + inString + '"' + " is not present on the " + scrnName + " screen and is not expected.\r", 1);
                clrValue = 0;
            }
            else if (rslt = true && (getNeg == "N"))
            {
                TextFileOps.Write(pth, "FAIL: The field " + '"' + inString + '"' + " is present on the " + scrnName + " screen and is not expected.\r", -1);
                clrValue = 0;
            }
        }

        public static void txtVerify(bool rslt, string scrnName, string inString, string pth, string getNeg)
        {
            if (rslt == true && (getNeg != "N"))
            {
                TextFileOps.Write(pth, "PASS: The text " + '"' + inString + '"' + " is present on the " + scrnName + " field...\r", 1);
                clrValue = 0;
            }
            else if (rslt == false && (getNeg != "N"))
            {
                TextFileOps.Write(pth, "FAIL: The text " + '"' + inString + '"' + " is not present on the " + scrnName + " field...\r", -1);
                clrValue = 0;
            }
            else if (rslt == false && (getNeg == "N"))
            {
                TextFileOps.Write(pth, "PASS: The text " + '"' + inString + '"' + " is not present on the " + scrnName + " field and is not expected.\r", 1);
                clrValue = 0;
            }
            else if (rslt = true && (getNeg == "N"))
            {
                TextFileOps.Write(pth, "FAIL: The text " + '"' + inString + '"' + " is present on the " + scrnName + " field and is not expected.\r", -1);
                clrValue = 0;
            }
        }

        public static void dropdownVerify(bool rslt, string scrnName, string inString, string pth, string getNeg)
        {
            if (rslt == true && (getNeg != "N"))
            {
                TextFileOps.Write(pth, "PASS: The dropdown " + '"' + inString + '"' + " is present in the " + scrnName + " list.\r", 1);
                clrValue = 0;
            }
            else if (rslt == false && (getNeg != "N"))
            {
                TextFileOps.Write(pth, "FAIL: The dropdown " + '"' + inString + '"' + " is not present on the " + scrnName + " list.\r", -1);
                clrValue = 0;
            }
            else if (rslt == false && (getNeg == "N"))
            {
                TextFileOps.Write(pth, "PASS: The dropdown " + '"' + inString + '"' + " is not present on the " + scrnName + " list and is not expected.\r", 1);
                clrValue = 0;
            }
            else if (rslt == true && (getNeg == "N"))
            {
                TextFileOps.Write(pth, "FAIL: The dropdown " + '"' + inString + '"' + " is present on the " + scrnName + " list and is not expected.\r", -1);
                clrValue = 0;
            }
        }

        public static void dropdownListVerify(bool rslt, string scrnName, string inString, string pth, string getNeg)
        {
            if (rslt == true && (getNeg != "N"))
            {
                TextFileOps.Write(pth, "PASS: The dropdown list item '" + '"' + inString + '"' + " is present in the " + scrnName + " dropdown.\r", 1);
                clrValue = 0;
            }
            else if (rslt == false && (getNeg != "N"))
            {
                TextFileOps.Write(pth, "FAIL: The dropdown list item '" + '"' + inString + '"' + " is not present on the " + scrnName + " dropdown.\r", -1);
                clrValue = 0;
            }
            else if (rslt == false && (getNeg == "N"))
            {
                TextFileOps.Write(pth, "PASS: The dropdown list item '" + '"' + inString + '"' + " is not present on the " + scrnName + " dropdown and is not expected.\r", 1);
                clrValue = 0;
            }
            else if (rslt == true && (getNeg == "N"))
            {
                TextFileOps.Write(pth, "FAIL: The dropdown list item '" + '"' + inString + '"' + " is present on the " + scrnName + " dropdown and is not expected.\r", -1);
                clrValue = 0;
            }
        }
    }
}


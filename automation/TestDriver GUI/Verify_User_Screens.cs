using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Data.Odbc;
using echoAutomatedSuite;
using Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using System.Windows.Forms;

namespace Verify_User_Screens_Firefox
{
    public class VerifyScreens
    {
        public static string testname;
        #region Constructor
        #endregion

        #region tstFirefox function
        public static void tstBrowser(string pth, string datSource, string steName, string tstName, int radnum, string baseURL, ref string[,] tstResult, out int fndExcep, out int fnlFail)
        {
            testname = tstName;
            int vfyFunc;
            int eleNum;
            int maxCols;
            int stpCount;
            int itmCount;
            int tstFail;
            char chr = Convert.ToChar(34);
            object[,] tmpArray;
            string[] outArray;
            string[] lstTest;
            string[] dbArray;
            string[,] dataArray;
            string[,] rsltArray;
            string argID;
            string functionID;
            string getNeg;
            string nmFunc;
            string step;
            string strCon;
            string strSQL;
            string tstID;
            string xlPath;
            ADODB.Connection objCon;
            ADODB.Recordset objRec;
            TestSuite lstObject;

            getNeg = "";
            tstFail = 0;
            fnlFail = 0;
            fndExcep = 0;
            DateTime tmp = DateTime.Now;                            //date-time at the time of test running
            vfyFunc = 0;
            rsltArray = new string[1, 6];

            //open browser based on selection on the GUI
            tstObject tstObj = new tstObject(radnum);

            //TextFileOps.Write(pth, "<li>", 100);

            switch (datSource)
            {
                case "DB":
                {    
                    //db connection string
                    strCon = "driver={MySQL ODBC 5.1 Driver};server=107.22.232.228;uid=qa_people;pwd=thehandcontrols;" +
                                "database=functional_test_data;option=3";

                    //database connections objects
                    objCon = new ADODB.Connection();
                    objRec = new ADODB.Recordset();
                    //open the connection to the database
                    objCon.Open(strCon);

                    //SQL to execute
                    strSQL = "SELECT id FROM test WHERE name = '" + tstName + "'";

                    //open recordset and get test id with SQL
                    objRec.Open(strSQL, objCon);

                    //set test id to a string variable
                    tmpArray = objRec.GetRows();
                    tstID = Convert.ToString(tmpArray[0, 0]);

                    //close the recordset
                    objRec.Close();

                    //SQL to execute
                    strSQL = "SELECT COUNT(*) FROM step WHERE test_id = '" + tstID + "'";

                    //open recordset and get the number of stepsto execute with SQL
                    objRec.Open(strSQL, objCon);

                    //get the count value from the recordset and convert to an int fpor use in the step loop
                    tmpArray = objRec.GetRows();
                    stpCount = Convert.ToInt32(tmpArray[0, 0]);

                    //close the recordset
                    objRec.Close();

                    //set up a for loop to run all steps in a test
                    for (int stp = 0; stp < stpCount; stp++)
                    {
                        if (stp == 23)
                            stp = 23;
                        step = "Step " + Convert.ToString(stp + 1);
                        //SQL to execute
                        strSQL = "SELECT function_id, argument_set_id FROM step WHERE (test_id = '" + tstID + "') AND (number = " + (stp + 1) + ")";

                        //open recordset and get all the ids necessary for this step
                        objRec.Open(strSQL, objCon);
                        tmpArray = objRec.GetRows();

                        //set the function id
                        functionID = Convert.ToString(tmpArray[0, 0]);

                        //set the hash string for the argument set
                        argID = Convert.ToString(tmpArray[1, 0]);

                        //close the recordset
                        objRec.Close();

                        //get the function name
                        strSQL = "SELECT function_name FROM function WHERE id = '" + functionID + "'";
                        objRec.Open(strSQL, objCon);
                        tmpArray = objRec.GetRows();

                        //set the function name variable
                        nmFunc = Convert.ToString(tmpArray[0, 0]);

                        //close the recordset
                        objRec.Close();

                        // get the number of argments to be set to the application
                        strSQL = "SELECT COUNT(*) FROM argument WHERE argument_set_id = '" + argID + "'";
                        objRec.Open(strSQL, objCon);
                        tmpArray = objRec.GetRows();

                        //set the itmCount varable
                        itmCount = Convert.ToInt32(tmpArray[0, 0]);

                        //close the recordset
                        objRec.Close();

                        if (itmCount > 0)
                        {
                            //set the array size for the inArray parameter of the drive function
                            dbArray = new string[itmCount];

                            //set the array with blank values
                            for (int x = 0; x < itmCount; x++)
                                dbArray[x] = String.Empty;

                            //get the argument data from the database and populate into the dbArray 
                            strSQL = "SELECT value FROM argument WHERE argument_set_id = '" + argID + "' ORDER BY seq ASC";
                            objRec.Open(strSQL, objCon);
                            tmpArray = objRec.GetRows();

                            //close the recordset
                            objRec.Close();

                            for (int stpData = 0; stpData < tmpArray.Length; stpData++)
                            {
                                if (stpData != tmpArray.Length)
                                    dbArray[stpData] = Convert.ToString(tmpArray[0, stpData]);
                            }
                        }
                        else
                        {
                            dbArray = new string[1];
                            dbArray[0] = "";
                        }

                        //call driveFunction to execute the step noted in dataArray[tstSuite. 0]
                        TextFileOps.Write(pth, "<li>", 100);

                        //initialize anm array to hold all of the test results that wiill be written to the results file
                        tstResult = new string[1, 9];

                        //pass the appropriate vars to the drivefunction
                        tstObj.driveFunction(tstObj, nmFunc, step, dbArray, baseURL, datSource, pth, ref getNeg, ref tstResult, out fndExcep, out tstFail);

                        for (int x = 0; x < tstResult.GetLength(0); x++)
                        {
                            TextFileOps.Write(pth, "<div>", 100);

                            if (fndExcep != -1)
                            {
                                if (tstResult[x, 0] == "verify")
                                {
                                    switch (tstResult[x, 1].Trim())
                                    {
                                        case "button":
                                            Recorder.btnVerify(Convert.ToBoolean(tstResult[x, 2]), tstResult[x, 3], tstResult[x, 4], pth, getNeg);
                                            break;
                                        case "dropdown":
                                            Recorder.dropdownListVerify(Convert.ToBoolean(tstResult[x, 2]), tstResult[x, 5], tstResult[x, 3], pth, getNeg);
                                            break;
                                        case "field":
                                            Recorder.fldVerify(Convert.ToBoolean(tstResult[x, 2]), tstResult[x, 3], tstResult[x, 5], pth, getNeg);
                                            break;
                                        case "grades":
                                            Recorder.gradeVerify(Convert.ToBoolean(tstResult[x, 2]), tstResult[x, 3], tstResult[x, 4], tstResult[x, 5], tstResult[x, 6], tstResult[x, 7], tstResult[x, 8], pth);
                                            break;
                                        case "image":
                                            Recorder.imgVerify(Convert.ToBoolean(tstResult[x, 2]), tstResult[x, 3], tstResult[x, 5], pth, getNeg);
                                            break;
                                        case "link":
                                            Recorder.lnkVerify(Convert.ToBoolean(tstResult[x, 2]), tstResult[x, 3], tstResult[x, 5], pth, getNeg);
                                            break;
                                        case "table":
                                            Recorder.tblVerify(Convert.ToBoolean(tstResult[x, 2]), tstResult[x, 3], tstResult[x, 4], tstResult[x, 5], pth, getNeg);
                                            break;
                                        case "text":
                                        case "field text":
                                            Recorder.txtVerify(Convert.ToBoolean(tstResult[x, 2]), tstResult[x, 3], tstResult[x, 5], pth, getNeg);
                                            break;
                                        case "outcome":
                                            Recorder.outcomeVerify(Convert.ToBoolean(tstResult[x, 2]), tstResult[x, 3], tstResult[x, 4], tstResult[x, 5], tstResult[x, 6], tstResult[x, 7], tstResult[x, 8], pth);
                                            break;
                                        case "percentage":
                                            Recorder.pctVerify(Convert.ToBoolean(tstResult[x, 2]), tstResult[x, 3], tstResult[x, 4], tstResult[x, 5], pth);
                                            break;

                                    }
                                }
                                else
                                {
                                    TextFileOps.Write(pth, tstResult[x, 1].ToString(), Convert.ToInt32(tstResult[x, 2]));
                                }

                            }
                            else
                            {
                                TextFileOps.Write(pth, "<div>", 100);
                                TextFileOps.Write(pth, "Exception Found.............(" + step + ")", 80);
                                TextFileOps.Write(pth, "<br />", 100);
                                TextFileOps.Write(pth, tstResult[x, 1].ToString(), -1);
                            }


                            TextFileOps.Write(pth, "</div>", 100);
                        }

                        tstResult = null;

                        TextFileOps.Write(pth, "<br />", 100);
                        TextFileOps.Write(pth, "</li>", 100);

                        if (tstFail == -1)
                            fnlFail = -1;

                        if (fndExcep == -1)
                        {
                            break;
                        }
                    }

                    break;
                }
                case "EX":
                {
                    //Initialize Firefox and construct the dataArray
                    //the out parameter is the maxColumns in what will be the fnlArray and hence the width of each item in the array  
                    xlPath = TestSuite.getXlPath(steName);

                    lstObject = new TestSuite();
                    lstObject.getTestListing(out lstTest, xlPath);

                    dataArray = tstObj.xlFunctions(tstObj, xlPath, tstName, "Master", out maxCols);

                    //set eleNum (number of elements) to 0
                    for (int runStep = 0; runStep < dataArray.GetLength(0); runStep++)
                    {
                        eleNum = 0;
                        for (int stpNum = 3; stpNum < dataArray.GetLength(1); stpNum++)
                        {
                            //Get the number of elements in this step data from dataArray
                            if (dataArray[runStep, stpNum] != null)
                            {
                                eleNum++;                           //increment elenum in the presence of a value
                            }                                       //in dataArray[tstSuite, stpNum]
                            else
                            {
                                break;
                            }

                        }

                        //initialize and set outArray with all data values in the dataArray line item
                        if (eleNum != 0)
                        {
                            outArray = new string[eleNum];      //set outArray to eleNum

                            //set the outarray with all data values or passing to driveFunction
                            for (int setArray = 0; setArray < eleNum; setArray++)
                            {
                                outArray[setArray] = dataArray[runStep, setArray + 3];
                            }
                        }
                        else
                        {
                            outArray = new string[1];
                            outArray[0] = "";
                        }

                        nmFunc = dataArray[runStep, 0];
                        step = dataArray[runStep, 2];
                        dbArray = outArray;

                        //call driveFunction to execute the step noted in dataArray[tstSuite. 0]
                        TextFileOps.Write(pth, "<li>", 100);

                        //initialize anm array to hold all of the test results that wiill be written to the results file
                        tstResult = new string[1, 9];

                        //pass the appropriate vars to the drivefunction
                        tstObj.driveFunction(tstObj, nmFunc, step, dbArray, baseURL, datSource, pth, ref getNeg, ref tstResult, out fndExcep, out tstFail);

                        for (int x = 0; x < tstResult.GetLength(0); x++)
                        {
                            TextFileOps.Write(pth, "<div>", 100);

                            if (fndExcep != -1)
                            {
                                if (tstResult[x, 0] == "verify")
                                    switch (tstResult[x, 1].Trim())
                                    {
                                        case "button":
                                            Recorder.btnVerify(Convert.ToBoolean(tstResult[x, 2]), tstResult[x, 3], tstResult[x, 4], pth, getNeg);
                                            break;
                                        case "dropdown":
                                            Recorder.dropdownListVerify(Convert.ToBoolean(tstResult[x, 2]), tstResult[x, 5], tstResult[x, 3], pth, getNeg);
                                            break;
                                        case "field":
                                            Recorder.fldVerify(Convert.ToBoolean(tstResult[x, 2]), tstResult[x, 3], tstResult[x, 5], pth, getNeg);
                                            break;
                                        case "grades":
                                            Recorder.gradeVerify(Convert.ToBoolean(tstResult[x, 2]), tstResult[x, 3], tstResult[x, 4], tstResult[x, 5], tstResult[x, 6], tstResult[x, 7], tstResult[x, 8], pth);
                                            break;
                                        case "image":
                                            Recorder.imgVerify(Convert.ToBoolean(tstResult[x, 2]), tstResult[x, 3], tstResult[x, 5], pth, getNeg);
                                            break;
                                        case "link":
                                            Recorder.lnkVerify(Convert.ToBoolean(tstResult[x, 2]), tstResult[x, 3], tstResult[x, 5], pth, getNeg);
                                            break;
                                        case "table":
                                            Recorder.tblVerify(Convert.ToBoolean(tstResult[x, 2]), tstResult[x, 3], tstResult[x, 4], tstResult[x, 5], pth, getNeg);
                                            break;
                                        case "text":
                                        case "field text":
                                            Recorder.txtVerify(Convert.ToBoolean(tstResult[x, 2]), tstResult[x, 3], tstResult[x, 5], pth, getNeg);
                                            break;
                                        case "outcome":
                                            Recorder.outcomeVerify(Convert.ToBoolean(tstResult[x, 2]), tstResult[x, 3], tstResult[x, 4], tstResult[x, 5], tstResult[x, 6], tstResult[x, 7], tstResult[x, 8], pth);
                                            break;
                                        case "percentage":
                                            Recorder.pctVerify(Convert.ToBoolean(tstResult[x, 2]), tstResult[x, 3], tstResult[x, 4], tstResult[x, 5], pth);
                                            break;
                                        case "weight":
                                            Recorder.weightWrite(tstResult[x, 2], tstResult[x, 3], pth);
                                            break;


                                    }
                                else
                                {
                                    TextFileOps.Write(pth, tstResult[x, 1].ToString(), Convert.ToInt32(tstResult[x, 2]));
                                }

                            }
                            else
                            {
                                TextFileOps.Write(pth, "<div>", 100);
                                TextFileOps.Write(pth, "Exception Found.............(" + step + ")", 80);
                                TextFileOps.Write(pth, tstResult[x, 1].ToString(), -1);
                            }

                            TextFileOps.Write(pth, "</div>", 100);
                        }

                        vfyFunc = 0;
                        tstResult = null;

                        TextFileOps.Write(pth, "<br />", 100);

                        //call driveFunction to execute the step noted in dataArray[tstSuite. 0]
                        TextFileOps.Write(pth, "</li>", 100);

                        if (tstFail == -1)
                            fnlFail = -1;

                        if (fndExcep == -1)
                        {
                            break;
                        }

                    }



                    break;
                }
            }

            TextFileOps.Write(pth, "</ul>", 100);

            switch (radnum)
            {
                case 1: 
                {
                    TextFileOps.Write(pth, "<li>", 80);
                    TextFileOps.Write(pth, "Closing Chrome..........\r\n", 80);
                    TextFileOps.Write(pth, "</li>", 80);
                    break;
                }
                case 2:
                    TextFileOps.Write(pth, "<li>", 80);
                    TextFileOps.Write(pth, "Closing Internet Explorer..........\r\n", 80);
                    TextFileOps.Write(pth, "</li>", 80);
                    break;
                {
                }
                default:
                    //close Firefox
                    TextFileOps.Write(pth, "<li>", 80);
                    TextFileOps.Write(pth, "Closing Firefox..........\r\n", 80);
                    TextFileOps.Write(pth, "</li>", 80);
                    break;
            }

            //exit chromedriver.exe if Chrome is the browser being used if niot here by an exception
            if (fndExcep != -1)
            {
                if (radnum == 1 || radnum == 2)
                    tstObj.Quit();
                else
                    tstObj.Close();
            }
            //put a couple newlines in the result file to seperate results
            for (int a = 0; a < 1; a++)
                TextFileOps.Write(pth, "\r\n", 0);

        }
        #endregion

        #region expectedStep
        private static int expectedStep(string inStep, string argID, ADODB.Connection objCon, ADODB.Recordset objRec)
        {
            object[,] tmpArray;
            int retVal;
            string strSQL;

            retVal = 0;

            switch (inStep)
            {
                case "Login":
                case "navLinks":
                case "addOutcome":
                    retVal = 2;
                    break;
                case "clkButton":
                case "inputText":
                    retVal = 4;
                    break;
                case "chkCheckbox":
                    retVal = 5;
                    break;
                case "selDropdown":
                    retVal = 6;
                    break;
                case "tblSelect":
                    retVal = 10;
                    break;
                case "vfyTableEntry":
                    strSQL = "SELECT COUNT(*) FROM argument WHERE argument_set_id = '" + argID + "'";

                    objRec.Open(strSQL, objCon);
                    tmpArray = objRec.GetRows();
                    retVal = Convert.ToInt32(tmpArray[0, 0]);
                    objRec.Close();
                    break;
            }
            return retVal;
        }
        #endregion
    }
        
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using NUnit;
using echoAutomatedSuite;
using Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace echoAutomatedSuite
{
    public class TestSuite
    {
        #region Public Functions

            #region Constructor
            public TestSuite()
            {

            }
            #endregion

            #region  getTestListing
            public void getTestListing(out string[] outArray, string tstPath)
            {
            
                Excel.Application xlApp;
                Excel.Workbook wrkBook;
                Excel.Worksheet wrkSheet;
                Process[] initXLProc;

                int numRows;

                //Get the Process Id of any Excel processes that are currently running 
                initXLProc = Process.GetProcessesByName("EXCEL");

                xlApp = new Excel.Application(); 
                wrkBook = xlApp.Workbooks.Open(tstPath);
                wrkSheet = wrkBook.Sheets["Master"];

                //Get the number of lines used in the data sheet
                numRows = wrkSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing).Row;

                //Set the outArray that will contain the test list
                outArray = new string[numRows - 1];

                for (int x = 0; x < numRows - 1; x++)
                {
                    outArray[x] = wrkSheet.Cells[x + 2, 1].Value;
                }

                //Kill the Excel process just opened
                killXLProc(initXLProc);
            }
            #endregion

            #region killXLProc
            public static void killXLProc(Process[] initXLProc)
            {
                Process[] allXLProc = Process.GetProcessesByName("Excel");
                int fndProc = 0;

                int procNum = initXLProc.Length;

                foreach (Process chkXLProc in allXLProc)
                {
                    for (int x = 0; x <= procNum - 1; x++)
                    {
                        if (chkXLProc.Id == initXLProc[x].Id)
                        {
                            fndProc = 1;
                            break;
                        }
                    }
                    if (fndProc != 1)
                    {
                        chkXLProc.Kill();
                    }
                    else
                    {
                        fndProc = 0;
                    }
                }
            }
            #endregion

            #region killProc
            public static void killProc(string inProc)
            {
                Process[] allProc = Process.GetProcessesByName(inProc);

                foreach (Process chkProc in allProc)
                {
                        chkProc.Kill();

                }
            }
            #endregion

            #region getXlPath
            public static string getXlPath(string inString)
            {
                string retString;
            
                //initialize variables
                retString = "";

                switch (inString)
                {

                    case "Verify User Screens - IE":
                    case "Verify User Screens - Firefox":
                    case "Verify User Screens - Chrome":
                        retString = Application.StartupPath + "\\Data Sheets\\Verify Users.xlsm";
                        break;
                    case "Gradebook Tests - Chrome":
                    case "Gradebook Tests - Firefox":
                    case "Gradebook Tests - IE":
                        retString = Application.StartupPath + "\\Data Sheets\\Verify Gradebook.xlsm";
                        break;
                    case "Peer Rubric - Autopopulate All Activities":
                        retString = Application.StartupPath + "\\Data Sheets\\Security Audit.xlsm";
                        break;
                    case "491 - Groups Admin":
                        retString = Application.StartupPath + "\\Data Sheets\\ECHODEV-491 Groups Admin.xlsm";
                        break;
                    case "Course Tests":
                        retString = Application.StartupPath + "\\Data Sheets\\Course Tests.xlsm";
                        break;
                    case "Event Tests":
                        retString = Application.StartupPath + "\\Data Sheets\\Event Tests.xlsm";
                        break;
                    case "Grades Tests":
                        retString = Application.StartupPath + "\\Data Sheets\\Grades Tests.xlsm";
                        break;
                    case "Groups Tests":
                        retString = Application.StartupPath + "\\Data Sheets\\Groups Tests.xlsm";
                        break;
                    case "People Tests":
                        retString = Application.StartupPath + "\\Data Sheets\\People Tests.xlsm";
                        break;
                    case "Library Tests":
                        retString = Application.StartupPath + "\\Data Sheets\\Library Tests.xlsm";
                        break;
                    case "Tools Tests":
                        retString = Application.StartupPath + "\\Data Sheets\\Tool Tests.xlsm";
                        break;
                    case "491 - Tests":
                        retString = Application.StartupPath + "\\Data Sheets\\491 Tests.xlsm";
                        break;
                    case "611 Automation":
                        retString = Application.StartupPath + "\\Data Sheets\\ECHODEV-611.xlsm";
                        break;
                    case "Experiemental Tests":
                        retString = Application.StartupPath + "\\Data Sheets\\Database Upload.xlsm";
                        break;
                    case "Smoke Tests":
                        retString = Application.StartupPath + "\\Data Sheets\\Smoke Tests.xlsm";
                        break;
                    case "Database Upload":
                        retString = Application.StartupPath + "\\Data Sheets\\Database Upload.xlsm";
                        break;
                }

                return retString;
            }
            #endregion

            #region getIndex
            public static string[] getIndex(string[] inArray)
            {
                int idx;
                int fndItem;
                string[] retArray;

                fndItem = 0;
                idx = 0;

                //get the index of the last array item that contains (not empty) data
                for (int x = 0; x < inArray.Length; x++)
                {
                    if (inArray[x] == String.Empty)
                    {
                        fndItem = 1;
                        idx = x;
                        break;
                    }

                    idx = x;
                }

                if (fndItem != 1)
                {
                    fndItem = 0;
                    idx = idx + 1;
                }

                retArray = new string[idx];

                //populate the return array
                for (int x = 0; x < idx; x++)
                {
                    retArray[x] = inArray[x];
                }

                return retArray;
            }
            #endregion

            #region getTableName
            public static string getTableName(string inString)
            {
                string tempstring;
                int strtString;
                string evalChar;
                char chr;

                strtString = 0;
                tempstring = "";

                for (int x = 0; x < inString.Length; x++)
                {
                    evalChar = inString.Substring(x, 1);
                    chr = Convert.ToChar(evalChar);

                    //check for apostrophe in inString
                    if ((int)chr == 39)               
                    {

                        if (strtString < 1)
                            strtString = 1;
                        else if (strtString == 1)
                            strtString = 2;
                    }

                    //write the tempstring if not the first apostrophe encountered
                    if (strtString == 1 && (int)chr != 39)
                        tempstring = tempstring + Convert.ToString(chr);
                    else if (strtString == 2)
                        break;
                }

                return tempstring;
            }
            #endregion

            #region genRandomGrade
            public static string genRandomGrade(int inGrade)
            {
                Random rndGrade = new Random();
                int tmpGrade;
                string grade;

                //get a random number between 55% of inGrade and inGrade + 1
                tmpGrade = rndGrade.Next(Convert.ToInt32(inGrade * .5), Convert.ToInt32(inGrade + (inGrade * .1)));

                if (tmpGrade > inGrade)
                    tmpGrade = 0;

                //convert the generated number to a string value to pass back
                grade = Convert.ToString(tmpGrade);
                return grade;
            } 
            #endregion

        #endregion

        #region Private Functions

            #region chkNegative
            public static int chkNegative(string inString)
            {
                int outVar;

                outVar = 0;

                switch (inString)
                {
                    case "content_table":
                    case "student_Panel":
                        outVar = 1;
                        break;

                    default:
                        outVar = 0;
                        break;
                }

                return outVar;
            }
            #endregion

            #region getXLData
            public static string[,] getXLData(string tstName, String xlPath, string shtName)
            {
                //This function will extract the data from the data sheets. First, we need get the list from the Master sheet by matching 
                //the first column to the test. 
                Excel.Application xlApp;
                Excel.Workbook wrkBook;
                Excel.Workbooks tmp;
                Excel.Worksheet wrkSheet;
                Process[] initXLProc;

                string[,] arrString;
                string[] tmpArray;
                int tstRows;            //the sheet row containing the test being processed
                int numRows;            //the number of used rows in the spreadsheet
                int numCols;            //the number of used columns row (tstRow)


                //Get the Process Id of any Excel processes that are currently running 
                initXLProc = Process.GetProcessesByName("EXCEL");

                //Setting the Excel objects
                xlApp = new Excel.Application();

                tmp = xlApp.Workbooks;
                wrkBook = tmp.Open(xlPath);
                wrkSheet = wrkBook.Sheets[shtName];

                //Get the number of lines used in the data sheet
                numRows = wrkSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing).Row;

                //Set the numCols variable
                tstRows = 0;
                numCols = 0;

                //Check for test name in the Master sheet
                for (int x = 2; x <= numRows; x++)
                {
                    if (wrkSheet.Cells[x, 1].Value == tstName)
                    {
                        //Set tstRow  = the row of the spreadsheet that matches the test
                        tstRows = x;

                        //Call numCols function to get the number of steps in the test
                        numCols = TestSuite.getCols(wrkSheet, "Master", x);

                        //Exit the for loop once the the test has been found and number of steps identified
                        break;
                    }
                }

                //set tmpArray to list of steps in the Master sheet and go get the values
                tmpArray = new string[numCols - 2];

                //set arrString to a two dimesional array that will contain the 
                arrString = new string[numCols - 2, 3];
                tmpArray = TestSuite.getArray(wrkSheet, tstRows, numCols);

                //Call array to split the test step and line number in the test sheet (Testname|X)
                arrString = TestSuite.splitArray(wrkBook, tmpArray);

                //kill the Excel process just opened
                TestSuite.killXLProc(initXLProc);

                //return the arrString value
                return arrString;
            }
            #endregion

            #region getTotDataCols
            public static int getTotDataCols(int ubound, string[,] inArray, string pthWorkBook, out int[] idxArray, out int totCols)
            {
                Excel.Application xlApp;
                Excel.Workbook wrkBook;
                Excel.Worksheet wrkSheet;
                Process[] initXLProc;

                string shtName;
                int idx;
                int maxCols;
                int currCols;

                //Initialize the idxArray
                idx = 0;
                idxArray = new int[ubound];

                //Initialize totCols, maxCols, currCols
                totCols = 0;
                maxCols = 0;
                currCols = 0;

                //This path will need to be changed....currently set to run on my local machine
                //pthWorkBook = "C:\\Users\\michael.kiewicz\\Documents\\Visual Studio 2010\\Projects\\TestDriver\\Data Sheets\\Verify Users.xlsx";

                //Get the Process Id of any Excel processes that are currently running 
                initXLProc = Process.GetProcessesByName("EXCEL");

                //Setting the Excel objects
                xlApp = new Excel.Application();
                wrkBook = xlApp.Workbooks.Open(pthWorkBook);

                for (int x = 0; x <= ubound - 1; x++)
                {
                    //Get the row number from the passed array
                    if (inArray[x, 1] != null && inArray[x, 1] != "")
                    {
                        //switch the sheet being processed [inArray is the step array passed in]
                        wrkSheet = wrkBook.Sheets[inArray[x, 0]];
                        shtName = inArray[x, 0];
                        int rowNum = Convert.ToInt32(inArray[x, 1]);

                        if (inArray[x, 0] == "vfyTableEntry")
                            shtName = inArray[x, 0];
                        currCols = getCols(wrkSheet, shtName, rowNum);
                    }

                    
                    //get the maxCols by comparing
                    if (currCols > maxCols)
                    {
                        maxCols = currCols;
                    }

                    totCols = totCols + currCols;
                    idxArray[idx] = currCols;
                    idx++;
                }

                //kill the Excel process just opened
                killXLProc(initXLProc);

                //the chkCheckbox function takes 5 args. The dataArray muct be at least this wide to prevent an exception
                if (maxCols < 10)
                {
                    maxCols = 10;
                }

                //return maxCols;
                return maxCols;
            }
            #endregion

            #region getArrayData
            //Function to handle itmArray of multiple dimensions
            public static string[,] getArrayData(string[,] stpNum, string pthWorkBook, int arrIndex)
            {
                Excel.Application xlApp;
                Excel.Workbook wrkBook;
                Excel.Worksheet wrkSheet;
                Process[] initXLProc;
                int fnd;
                string[,] outArray;

                //Initialize the couting variable (idx) and outArray
                fnd = 0;
                outArray = new string[stpNum.GetLength(0), arrIndex];

                //This path will need to be changed....currently set to run on my local machine
                //pthWorkBook = "C:\\Users\\michael.kiewicz\\Documents\\Visual Studio 2010\\Projects\\TestDriver\\Data Sheets\\Verify Users.xlsx";

                //Get the Process Id of any Excel processes that are currently running 
                initXLProc = Process.GetProcessesByName("EXCEL");

                //Setting the Excel objects
                xlApp = new Excel.Application();
                wrkBook = xlApp.Workbooks.Open(pthWorkBook);
                wrkSheet = wrkBook.Sheets[stpNum[0, 0]]; 
                

                for (int x = 0; x <= stpNum.GetLength(0) - 1; x++)
                {
                    //iterate through the sheet array in  the workbook object to find if a sheet exists
                    foreach(Excel.Worksheet fndSheet in wrkBook.Sheets)
                    {
                        if (fndSheet.Name.Equals(stpNum[x, 0]))
                        {
                            fnd = 1;            //found the sheet

                            if (fndSheet.Name == "selDropdown" )
                                fnd = 1;
                            break;
                        }
                    }

                    if (fnd == 1)
                    {
                        fnd = 0;
                        wrkSheet = wrkBook.Sheets[stpNum[x, 0]];

                        //Populate the fnlArray across checking for nulls as we go
                        for (int y = 0; y < arrIndex; y++)
                        {
                            if (wrkSheet.Cells[stpNum[x, 1], y + 1].Value != null)
                            {
                                outArray[x, y] = (wrkSheet.Cells[stpNum[x, 1], y + 1].Value).ToString();

                                /*if (y + 1 == arrIndex)
                                {
                                    //idx++;
                                }*/

                            }
                            else
                            {
                                outArray[x, y] = String.Empty;
                                //idx++;
                            }
                        }
                    }
                    else
                    {
                        fnd = 0;
                        //idx++;
                    }
                }

                //kill the Excel process just opened
                killXLProc(initXLProc);

                return outArray;
            }
            #endregion

            #region getCols
            private static int getCols(Excel.Worksheet wrkSheet, string shtName, int x)
            {
                int numCols = 0;
                if (shtName != "Master")
                {
                    //Once test is found scroll through and get the last column in preparation of populatuing the data array
                    for (int y = 30; y >= 1; y--)
                    {
                        //Check for the first row in the spreadsheet NOT containing data
                        if (wrkSheet.Cells[x, y].Value != null)
                        {
                            numCols = y;                        //the last Column that contains data
                            //endLoop = 2;                          //set the endloop variable to break out of the  for loops
                            break;                                  //break out of this loop
                        }
                    }
                }
                else
                {
                    for (int y = 400; y >= 1; y--)
                    {
                        //Check for the first row in the spreadsheet NOT containing data
                        if (wrkSheet.Cells[x, y].Value != null)
                        {
                            numCols = y;                        //the last Column that contains data
                            //endLoop = 2;                          //set the endloop variable to break out of the  for loops
                            break;                                  //break out of this loop
                        }
                    }
                }

                return numCols;
            }
            #endregion

            #region getArray
            private static string[] getArray(Excel.Worksheet wrkSheet, int numRows, int numCols)
            {
                //the string array that will hold all of the sheet data from the test data sheet (i.e. Login, nav sheets)
                string[] outArray;

                //initialize the output string array that will be sent back to the calling function
                outArray = new string[numCols - 2];

                for (int x = 0; x < numCols - 2; x++)
                {
                    outArray[x] = (string)wrkSheet.Cells[numRows, x + 3].Value;
                }
                return outArray;
            }
            #endregion

            #region splitArray
            private static string[,] splitArray(Excel.Workbook wrkBook, string[] inArray)
            {
                string[,] outArray;
                string currString;                                          //the current array element being processed  
                string tempString;                                          //placeholder for array 
                string ltr;                                                 //character checking for the '|' divider 
                int asciibyte;                                              //ascii code of ltr
                int strLen;                                                 //length of the string array element
                int y;

                outArray = new string[inArray.Length, 3];

                for (int x = 0; x <= inArray.Length - 1; x++)
                {
                    tempString = "";                                            //set tempString                                     
                    currString = inArray[x];                                    //set currsting to the incoming element
                    strLen = currString.Length;                                 //get the length of the array element being processed

                    for (y = 0; y <= strLen - 1; y++)
                    {
                        ltr = currString.Substring(y, 1);
                        asciibyte = System.Convert.ToInt16(ltr[0]);
                        if (asciibyte == 124)
                        {
                            outArray[x, 0] = tempString;
                            outArray[x, 1] = currString.Substring(y + 1, strLen - (y + 1));
                            tempString = "";
                            break;
                        }
                        else
                        {
                            tempString = tempString + ltr;
                        }
                    }

                    if (outArray[x, 1] == null)
                    {
                        outArray[x, 0] = tempString;
                        outArray[x, 1] = "";
                    }

                    outArray[x, 2] = "Step " + (x + 1);
                }

                return outArray;
            }
            #endregion

            #region getShtData
            private string[,] getShtData(Excel.Workbook wrkBook, string[,] inArray)
            {
                Excel.Worksheet wrkSheet;
                string[,] outArray;
                int arrLen;
                int lnNum;
                arrLen = inArray.Length;

                //Set the array to return
                outArray = new string[arrLen, 2];


                //Setting the Excel objects
                for (int x = 0; x <= inArray.Length; x++)
                {
                    //Open the work to process
                    wrkSheet = wrkBook.Sheets[inArray[x, 0]];

                    outArray[x, 0] = inArray[x, 0];
                    lnNum = Convert.ToInt32(inArray[x, 1][0]);
                    outArray[x, 1] = (string)wrkSheet.Cells[1, lnNum].Value;
                }

                return outArray;
            }
            #endregion

            #region convertString
            public static string convertString(string inString)
            {
                string retString;

                retString = "";

                switch (inString)
                {
                    case "addEvents_btn":
                        retString = "Add Events";
                        break;
                    case "AgendaPollsSubmitBtn":
                        retString = "Vote";
                        break;
                    case "Calender":
                        retString = "Calendar";
                        break;
                    case "CancelBtn":
                        retString = "Cancel Button";
                        break;
                    case "comboShowhideBtn_manageCourse":
                        retString = "Manage Course";
                        break;
                    case "content_table":
                        retString = "Open Courses";
                        break;
                    case "creategroup_Btn":
                        retString = "Create Group";
                        break;
                    case "custom_button":
                        retString = "Close";
                        break;
                    case "id('courses_view_table')":
                    case "courses_view_table":
                        retString = "Courses View";
                        break;
                    case "id('course_views')":
                        retString = "Enrollment";
                        break;
                    case "id('txt_group_title')":
                        retString = "Group Name";
                        break;
                    case "id('pplf_user_list')":
                    case "pplf_user_list":
                        retString = "Student list";
                        break;
                    case "id('manageCourse')":
                        retString = "Settings";
                        break;
                    case "id('edit-main-form-course-full-title')":
                        retString = "Full Title";
                        break;
                    case "id('edit-main-form-course-short-title')":
                        retString = "Short Title";
                        break;
                    case "id('edit-main-form-course-description')":
                        retString = "Description";
                        break;
                    case "id('edit-main-form-endrow-change-status-wrapper')/div/span":
                        retString = "Enrollment Change Status";
                        break;
                    case "//*[@id=\"edit-main-form-course-subject1-wrapper\"]/div/span":
                        retString = "first Subject";
                        break;
                    case "//*[@id=\"edit-main-form-course-period-wrapper\"]/div/span":
                        retString = "Period";
                        break;
                    case "//*[@id=\"edit-main-form-course-school-year-wrapper\"]/div/span":
                        retString = "School Year";
                        break;
                    case "//*[@id=\"edit-main-form-school-term-start-school-term-w-school-term-475199-wrapper\"]/div/span":
                        retString = "Fall Semester 2011";
                        break;
                    case "//*[@id=\"edit-main-form-school-term-start-school-term-w-school-term-491932-wrapper\"]/div/span":
                        retString = "Spring Semester 2012";
                        break;
                    case "//*[@id=\"edit-main-form-school-term-start-school-term-w-school-term-495334-wrapper\"]/div/span":
                        retString = "Summer 2011";
                        break;
                    case "//*[@id=\"edit-main-form-school-term-start-school-term-w-school-term-519945-wrapper\"]/div/span":
                        retString = "Spring 2012 ";
                        break;
                    case "id('ntlp-courses-activities-form')/div/table/tbody/tr/td/div/div/div/div/div/div/table/tbody/tr/td[4]/div/div/a/strong":
                        retString = "Add Activities";
                        break;
                    case "edit-main-save-close":
                        retString = "Save & Close";
                        break;
                    case "edit-main-save":
                        retString = "Save";
                        break;
                    case "events-add":
                        retString = "Add Event";
                        break;
                    case "events-body":
                        retString = "Events Block";
                        break;
                    case "form-item":
                        retString = "My Courses";
                        break;
                    case "gsites_block":
                        retString = "Google Sites";
                        break;
                    case "gmail_block":
                    case "Gmail":
                        retString = "Google Mail";
                        break;
                    case "gdocs_block":
                    case "Google Docs":
                        retString = "Google Docs";
                        break;
                    case "gapps-tipsy":
                        retString = "Google Apps Image";
                        break;
                    case "groups_table":
                        retString = "Group Table";
                        break;
                    case "GroupsAndEvents":
                        retString = "Events";
                        break;
                    case "HomePageFeatures":
                        retString = "New Tech Features";
                        break;
                    case "HomePageGroups":
                        retString = "Groups";
                        break;
                    case "HomePagePolls":
                        retString = "Polls";
                        break;
                    case "/html/body/div/div/div/div/div/form/div/div/fieldset/div[2]/div/div/div/div/div[2]/div/div[2]/div/div/div/div/div/table":
                        retString = "Student List";
                        break;
                    case "/html/body/table/tbody/tr/td/table[2]/tbody/tr[2]/td[2]/table/tbody/tr/td/div/div[2]/div/div/div[3]/div/div/div/div/div/div/div/div/div/div/div/div/table[2]":
                        retString = "Courses";
                        break;
                    case "/html/body/table/tbody/tr/td/table[2]/tbody/tr[2]/td[2]/table/tbody/tr/td/div/form/div/fieldset/div/div[3]/div/div/div/div/div/div/div/div/div/table[2]":
                        retString = "Select Course View";
                        break;
                    case "/html/body/table/tbody/tr/td/table[2]/tbody/tr[2]/td[2]/table/tbody/tr/td/div/form/div/div/div/div/fieldset/div[4]/div/div/div[2]/div/div/div/div/div/div/div/div/div/fieldset/table/tbody/tr/td[3]/div/a":
                        retString = "Mark All 100%";
                        break;
                    case "/html/body/table/tbody/tr/td/table[2]/tbody/tr[2]/td[2]/table/tbody/tr/td/div/div[2]/div/div/div/div/div/div/div/div/div/div/div[3]/div/div/div[2]/table":
                        retString = "Acivities Side";
                        break;
                    case "/html/body/table/tbody/tr[1]/td/table[2]/tbody/tr[1]/td/div[2]/div[1]/ul/li[2]/a":
                        retString = "All Groups";
                        break;
                    case "/html/body/table/tbody/tr/td/table[2]/tbody/tr[2]/td[2]/table/tbody/tr/td/div/div[2]/div/div/div[3]/div/div/table":
                        retString = "Group List";
                        break;
                    case "id('edit-main-info-duedate-hour-wrapper')/div/span":
                        retString = "Hour";
                        break;
                    case "id('edit-main-info-duedate-minute-wrapper')/div/span":
                        retString = "Minute";
                        break;
                    case "id('edit-main-info-duedate-meridian-wrapper')/div/span":
                        retString = "AM/PM";
                        break;
                    case "id('edit-main-info-activityname')":
                        retString = "Activity Name";
                        break;
                    case "logo":
                        retString = "NewTechNetwork logo";
                        break;
                    case "mailNameHome":
                        retString = "Google Apps";
                        break;
                    case "mycourses_block_contents":
                        retString = "My Courses";
                        break;
                    case "newListSelected":
                        retString = "Term dropdown";
                        break;
                    case "newListSelected newListSelHover":
                        retString = "Year dropdown";
                        break;
                    case "notification":
                        retString = "Notifications Image";
                        break;
                    case "notification-tipsy":
                        retString = "Notifications";
                        break;
                    case "pf_add_students-wrapper":
                        retString = "Select Instructors";
                        break;
                    case "Project-ActivitiesFilter":
                        retString = "Project Activities Filter";
                        break;
                    case "ProjectResources-Activities":
                        retString = "Project Filter";
                        break;
                    case "SearchBtn":
                        retString = "Search";
                        break;
                    case "settings-tipsy":
                        retString = "Settings";
                        break;
                    case "school_year_combo":
                        retString = "School Year";
                        break;
                    case "student-snapshot-form":
                        retString = "Student Snapshot";
                        break;
                    case "SubmitAssignmentBtn":
                        retString = "Create Course";
                        break;
                    case "tab-taking":
                        retString = "Taking";
                        break;
                    case "tab-school-snapshot":
                        retString = "School Snapshot";
                        break;
                    case "tab-teaching":
                        retString = "Teaching";
                        break;
                    case "term_selector_combo":
                        retString = "Term Selector";
                        break;
                    case "tools-school-selector":
                        retString = "School Selector";
                        break;
                    case "view view-ntlp-people-search view-id-ntlp_people_search view-display-id-default view-dom-id-1 views-processed":
                        retString = "Students";
                        break;
                    case "view-content":
                        retString = "Student";
                        break;
                    default:
                        retString = inString;
                        break;
                }

                return retString;
            }
            #endregion

            #region tblString
            public static string tblString(string tblName, string inString)
            {
                string retString;

                retString = "";

                switch (tblName)
                {
                    case "/html/body/table/tbody/tr/td/table[2]/tbody/tr[2]/td[2]/table/tbody/tr/td/div/div[2]/div/div/div/div/div/div/div/div/div/div/div[3]/div/div/div[2]/table":
                        retString = inString + "/ul/li/a";
                        break;
                    default:
                        retString = inString;
                        break;
                }   

                return retString;
            }
            #endregion

            #region getTestList
            public static string[] getTestList(string xlPth)
            {
                Excel.Application xlApp;
                Excel.Workbooks tmp;
                Excel.Workbook wrkBook;
                Excel.Worksheet wrkSheet;
                Process[] initXLProc;

                int numRows;
                string[] outArray;

                outArray = null;
                wrkBook = null;
                numRows = 0;

                //Get the Process Id of any Excel processes that are currently running 
                initXLProc = Process.GetProcessesByName("EXCEL");

                //Establish Excel objects
                xlApp = new Excel.Application();
                tmp = xlApp.Workbooks;

                if (File.Exists(xlPth))
                {
                    wrkBook = tmp.Open(xlPth);
                    wrkSheet = wrkBook.Sheets[1];

                    //Get the number of lines used in the data sheet
                    for (int x = 2; x < 10000; x++)
                    {
                        if (wrkSheet.Cells[x, 1].Value == null)
                        {
                            numRows = x - 1;
                            break;
                        }
                    }

                    //Instantiate outarray
                    outArray = new string[numRows - 1];

                    //populate outArray
                    for (int x = 2; x <= numRows; x++)
                    {
                        outArray[x - 2] = wrkSheet.Cells[x, 1].Value;
                    }

                    //kill the Excel process just opened
                    TestSuite.killXLProc(initXLProc);

                }
                else 
                {
                    MessageBox.Show("The Excel workbook at the path " + xlPth + " is not present",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

                return outArray;
            }
            #endregion

            #region trimString
            public static int trimString(string instring)
            {
                int retVal;
                int strLen;
                string thsChar;

                retVal = 0;
                strLen = instring.Length;

                for (int x = strLen; x > 0; x--)
                {
                    thsChar = instring.Substring(x - 1, 1);

                    if (thsChar == "/")
                    {
                        break;
                    }
                    else
                    {
                        retVal++;
                    }
                }
                    
                return retVal;
            }
            #endregion

            #region getTableEntry
            public static string getTableEntry(string instring, string schString, int strLen)
            {
                int fndItem;
                int charCount;
                string tempstring;
                string thsChar;

                charCount = 0;
                fndItem = 0;
                tempstring = "";

                if (instring.Length > schString.Length)
                {
                    for (int x = 0; charCount <= (instring.Length - strLen); x++)
                    {
                        tempstring = instring.Substring(x, strLen);
                        //set thsChar equal to the character being processed
                        thsChar = instring.Substring(x, 1);

                        //look for a backslash which indicate a carriage return or newline
                        if (thsChar == "\r" || thsChar == "\n")
                        {
                            if (tempstring == schString)
                            {
                                fndItem = 1;
                                break;
                            }
                            else
                            {
                                charCount = charCount + 1;
                            }
                        }
                        else
                        {
                            if (tempstring == schString)
                            {
                                fndItem = 1;
                                break;
                            }
                            else
                            {
                                charCount = charCount + 1;
                            }
                        }
                    }
                }
                else
                {
                    if (instring == schString)
                    {
                        fndItem = 1;
                        tempstring = instring;
                    }
                }

                if (fndItem == 1)
                    return tempstring;
                else
                {
                    return instring;
                }
                
            }
            #endregion

            #region setSlash
            public static string setSlash(string instring)
            {
                int strLen;
                string retString;
                string tempstring;
                string thsChar;

                tempstring = "";
                retString = "";
                strLen = instring.Length;

                for(int x = 0; x < strLen; x++)
                {
                    thsChar = instring.Substring(x, 1);

                    if (thsChar == "\r" || thsChar == "\n")
                    {
                        if (thsChar == "\r")
                            tempstring = tempstring + "\\r";
                        else if (thsChar == "\n")
                            tempstring = tempstring + "\\n";
                    }
                    else
                    {
                        tempstring = tempstring + thsChar;
                    }
                }

                retString = tempstring;
                return retString;
            }
            #endregion

        #endregion
    }
}


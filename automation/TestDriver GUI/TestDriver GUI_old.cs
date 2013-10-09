using ADODB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Web;
using Excel = Microsoft.Office.Interop.Excel;
using Verify_User_Screens_Chrome;
using Verify_User_Screens_Firefox;
using Verify_User_Screens_IE;
using echoAutomatedSuite;
using System.Collections;

namespace TestDriver_GUI
{
    public partial class TestDriverGUI : Form
    {
        object missing;
        
        DateTime currTime;
        string[] avblSuite;
        private int radnum;
        string datSource;
        public static string reg;

        #region Constructor
        public TestDriverGUI()
        {
            InitializeComponent();
        }
        #endregion

        #region Form1_Load
        public void Form1_Load(object sender, EventArgs e)
        {
            string[] lstSuite;
            string pth;
            string appPath ="";


            //form1.WindowState = FormWindowState.Maximized;
            //targetForm.FormBorderStyle = FormBorderStyle.None;
            //targetForm.TopMost = true;

            appPath = Application.StartupPath;
            missing = Type.Missing;


            if (rdoDB.Checked == true)
            {
                datSource = "DB";
                lstAvailableTest.Hide();
                label1.Hide();
                label3.Location = label1.Location;
                lstSelectedTest.Location = lstAvailableTest.Location;
                lstSelectedTest.Size = new Size(447, lstSelectedTest.Height);
            }
            else
                datSource = "EX";

            switch (datSource)
            {
                case "DB":
                    lstTestSuite.Items.Add("");
                    lstTestSuite.Items.AddRange(db_access("SELECT name FROM regression_suite ORDER BY name ASC"));
                    break;
                case "EX":
                    pth = appPath + "\\Suite List.txt";
                    lstSuite = File.ReadAllLines(pth);
                    lstTestSuite.Items.AddRange(lstSuite);
                    break;
            }

            //set the initial form field states
            lstAvailableTest.Text = "";
            lstSelectedTest.Text = "";
            btnSelect.Enabled = false;
            btnDeselect.Enabled = false;
            btnOK.Enabled = false;
            btnSlctAllAvailable.Enabled = false;
            btnDeslctAllAvailable.Enabled = false;
        }
        #endregion

        #region btnOK_Click
        public void btnOK_Click(object sender, EventArgs e)
        {
            int total;
            int clrIndex;
            int failed = 0;
            int fndExcep;
            object[] tmpArray;
            string[,] currList;
            string endTime;
            string hrs;
            string mins;
            string secs;
            string timeString;
            string dirPth;
            string suffFile;
            string slctString;
            string suiteName;
            string tmpID;
            string caseName;
            string fileName = "";
            string fldrString = "";
            string pth = "";
            Control.ControlCollection btnRadio;
            DateTime tmp;
            TimeSpan fnlTime;
            DateTime startTime;

            clrIndex  = 0;
            suffFile = "";
            btnRadio = slctBrowser.Controls;

            //disable all the controls on the form 
            lstTestSuite.Enabled = false;
            btnOK.Enabled = false;
            btnCancel.Enabled = false;
            btnDeslctAllAvailable.Enabled = false;
            btnSelect.Enabled = false;
            btnSlctAllAvailable.Enabled = false;
            lstAvailableTest.Enabled = false;
            lstSelectedTest.Enabled = false;
            lstCaseSelect.Enabled = false;
            lstTestSuite.Enabled = false;

            

            label11.Text = "Total Tests: ";
            label12.Text = "Passed Tests: ";
            label13.Text = "Failed Tests: ";


            //populate an array with the list of items currently in the Selected Items box
            currList = new string[lstCaseSelect.Items.Count, 2];
            total = lstCaseSelect.Items.Count;

            if (datSource != "DB")
            {
                for (int i = 0; i < lstCaseSelect.Items.Count; i++)
                {
                    slctString = lstCaseSelect.Items[i].ToString();
                    getVal(slctString, out suiteName, out caseName);

                    currList[i, 0] = suiteName;
                    currList[i, 1] = caseName;
                }
            }
            else
            {
                for (int i = 0; i < lstCaseSelect.Items.Count; i++)
                {
                    //set the test case variable
                    currList[i, 1] = lstCaseSelect.Items[i].ToString();

                    //get a one item array with the regression suite id from the test table
                    tmpArray= db_access("SELECT regression_suite_id FROM test WHERE name = '"
                                                        + currList[i, 1] + "'");
                    tmpID = Convert.ToString(tmpArray[0]);

                    tmpArray = db_access("SELECT name FROM regression_suite WHERE id = '" + tmpID + "'");
                    currList[i, 0] = Convert.ToString(tmpArray[0]);
                }

                //there's no way to 

            }

            //Setup the directory where the results file will be stored
            dirPth = Application.StartupPath + "\\Results";
            tmp = DateTime.Now;                                                     //date-time at the time of test running
            suffFile = tmp.Month.ToString() + tmp.Day.ToString() + tmp.Year.ToString().Substring(2,2) + tmp.Hour.ToString() + 
                tmp.Minute.ToString() + tmp.Second.ToString();

            //set the date-time value to a string
            fileName = "Result_" + suffFile + ".html";
            fldrString = "Result_" + suffFile;
            pth = dirPth + "\\" + fileName;

            //create a result text file  and write a beginning message
            TextFileOps.Open(pth);

            //start timer1 to calculate the complete test run
            timer1.Start();
            startTime = DateTime.Now;


            tstObject_FF.faillst = "";

            for (int tst = 0; tst < currList.GetLength(0); tst++ )
            {
                //get the current date/time value and set to a string
                currTime = DateTime.Now;
                suffFile = currTime.ToString("MMddhhmmss");

                lstCaseSelect.SetSelected(tst, true);

                txtSuiteRunning.Text = currList[tst, 0];
                txtTestRunning.Text = currList[tst, 1];

                timer2.Start();

                //run this test
                runFunction(currList[tst, 0], currList[tst, 1], btnRadio, pth, out fndExcep);

                //stop test timer
                timer2.Stop();

                //set the final time and format each element
                fnlTime = DateTime.Now - currTime;
                timeString = fnlTime.ToString();
                hrs = fnlTime.Hours.ToString();
                hrs = fmtTime(hrs);
                mins = fnlTime.Minutes.ToString();
                mins = fmtTime(mins);
                secs = fnlTime.Seconds.ToString();
                secs = fmtTime(secs);
                endTime = hrs + ":" + mins + ":" + secs;


                //Write the endTime to the test results
                TextFileOps.Write(pth, "This test run completed in: " + endTime + "\r\n", clrIndex = 0);
                TextFileOps.Write(pth, "", clrIndex);

                lstCaseSelect.SetSelected(tst, false);
            }

            //Send message to result file noting the end of the suite
            for (int a = 0; a < 1; a++)
                TextFileOps.Write(pth, "End Test Run.........." + "\r\n", clrIndex);




            TextFileOps.Write(pth, "Failed Tests: ", clrIndex);
            switch (radnum)
            {
                case 0:
                    TextFileOps.Write(pth, tstObject_FF.faillst + "\r\n", clrIndex);
                    foreach (char c in tstObject_FF.faillst)
                        if (c == ',') failed++;
                    break;
                case 1:
                    TextFileOps.Write(pth, tstObject_Chrome.faillst + "\r\n", clrIndex);
                    foreach (char c in tstObject_Chrome.faillst)
                        if (c == ',') failed++;
                    break;
                case 2:
                    TextFileOps.Write(pth, tstObject_IE.faillst + "\r\n", clrIndex);
                    foreach (char c in tstObject_IE.faillst)
                        if (c == ',') failed++;
                    break;
                default:
                    TextFileOps.Write(pth, tstObject_FF.faillst + "\r\n", clrIndex);
                    break;
            }


            label11.Text += total;
            label12.Text += (total - failed);
            label13.Text += failed;

            //write closing tags to result file
            TextFileOps.Write(pth, "</html>", clrIndex = 100);
            TextFileOps.Write(pth, "</body>", clrIndex = 100);

            //show the result file
            Process.Start(pth);

            //re-enable all the controls on the form 
            lstTestSuite.Enabled = true;
            btnOK.Enabled = true;
            btnCancel.Enabled = true;
            btnDeslctAllAvailable.Enabled = true;
            btnSelect.Enabled = true;
            btnSlctAllAvailable.Enabled = true;
            lstAvailableTest.Enabled = true;
            lstSelectedTest.Enabled = true;
            lstCaseSelect.Enabled = true;
            lstTestSuite.Enabled = true;

            //enable the test suite selector field at the conclusion of the test
            lstTestSuite.Enabled = true;

            //stop timer1 and post results to the GUI 
            timer1.Stop();
            currTime = DateTime.Now; 
            fnlTime = (currTime - startTime);
            hrs = fnlTime.Hours.ToString();
            hrs = fmtTime(hrs);
            mins = fnlTime.Minutes.ToString();
            mins = fmtTime(mins);
            secs = fnlTime.Seconds.ToString();
            secs = fmtTime(secs);

            endTime = hrs + ":" + mins + ":" + secs;
            lblTimer.Text = endTime;

            boxPerms();
        }
        #endregion

        #region fmtTime
        private string fmtTime(string instring)
        {
            int inInt;

            inInt = Convert.ToInt32(instring);
            if (inInt < 10)
            {
                instring = "0" + instring;
            }

            return instring;
        }
        #endregion

        #region btnCancel_Click
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region lstTestSuite_SelectedIndexChanged
        private void lstTestSuite_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] lstSuite;
            string pth;
            string tstSelected = "";

            //Clear items from the Available Test and the Selected Test listbox
            lstAvailableTest.Items.Clear();
            lstSelectedTest.Items.Clear();
            //the selected item in the list box
            tstSelected = lstTestSuite.SelectedItem.ToString();

            if (lstTestSuite.Text != "")
            {
                switch (datSource)
                {
                    case "DB":
                        object[] array = db_access("SELECT name FROM test WHERE regression_suite_id = " + "(SELECT id FROM regression_suite WHERE name = '" + tstSelected + "')");
                        for (int i = 0; i < array.Length; i++)
                        {
                            if(lstCaseSelect.Items.Contains(array[i]) == false)
                            {
                                lstSelectedTest.Items.Add(array[i]);
                            }
                        }
                        break;

                    case "EX":
                        pth = getSuitePath(tstSelected);

                        lstAvailableTest.Text = "";
                        lstSelectedTest.Text = "";
                        btnSelect.Enabled = true;
                        btnDeselect.Enabled = true;
                        btnOK.Enabled = true;
                        btnSlctAllAvailable.Enabled = true;
                        btnDeslctAllAvailable.Enabled = true;

                        lstSuite = File.ReadAllLines(pth);

                        lstAvailableTest.Items.AddRange(lstSuite);
                        break;
                }
                slctDataSource.Enabled = false;
            }
            else
            {
                lstAvailableTest.Text = "";
                lstSelectedTest.Text = "";
                btnSelect.Enabled = false;
                btnDeselect.Enabled = false;
                btnOK.Enabled = false;
                slctDataSource.Enabled = true;
            }
        }
        #endregion

        #region popListBox
        private string popListBox(string inSuite)
        {
            string retString = "";

            switch(inSuite)
            {
                case "Verify User Screens":
                    retString = "C:\\Users\\michael.kiewicz\\Documents\\Visual Studio 2010\\Projects\\WindowsFormsApplication1\\Verify User Screens.txt";
                    break;
            }

            return retString;
        }
        #endregion

        #region getTest
        public string getTest(string inString)
        {
            string outString;

            switch (inString)
            {
                case "Verify User Screens - IE":
                    outString = "VerifyScreens_IE";
                    break;
                case "Verify User Screens - Firefox":
                    outString = "VerifyScreens_Firefox";
                    break;
                case "Verify User Screens - Chrome":
                    outString = "VerifyUserScreens_Chrome";
                    break;
                default:
                    outString = "";
                    break;
            }
            return outString;
        }
        #endregion

        #region btnSelect_Click
        private void btnSelect_Click(object sender, EventArgs e)
        {
            ArrayList list = new ArrayList();
            //loop through the selectedTest listbox items
            for (int i = 0; i < lstSelectedTest.Items.Count; i++)
            {
                //add items to return list
                list.Add(lstSelectedTest.Items[i]);
                //check if item is selected
                if (lstSelectedTest.SelectedItems.Contains(lstSelectedTest.Items[i]))
                {   
                    //add item to caseSelected listbox
                    lstCaseSelect.Items.Add(lstSelectedTest.Items[i]);
                    //remove item from return list
                    list.Remove(lstSelectedTest.Items[i]);
                }
            }
            //clear selected return listbox and populate it with the return list
            lstSelectedTest.Items.Clear();
            lstSelectedTest.Items.AddRange(list.ToArray());

            //If the OK button is not enabled, enable it
            if (btnOK.Enabled != true)
                btnOK.Enabled = true;
        }
        #endregion

        #region btnDeselect_Click
        private void btnDeselect_Click(object sender, EventArgs e)
        {
            clkDeselectButton();

            if (lstCaseSelect.Items.Count == 0)
            {
                btnDeselect.Enabled = false;
                btnDeslctAllAvailable.Enabled = false;
            }
            else
            {
                btnDeselect.Enabled = true;
                btnDeslctAllAvailable.Enabled = true;
            }
        }
        #endregion

        #region runFunction
        private void runFunction(string steName, string tstName, Control.ControlCollection btnRadio, string pth, out int fndExcep)
        {
            int count;          //number of browser select contriols on the GUI form
            int numControl;      //the index of the selected radio button in the btnRadio Control Collection

            count = 0;          //initialize count to 0
            numControl = -1;     //initialize numControl to 0
            fndExcep = 0;       //initialize fndExcep to 0
            

            //get value of count from the btnRadio collection
            count = btnRadio.Count;

            for (int x = 0; x < count; x++)
            {
                RadioButton btnThis = (RadioButton)btnRadio[x];
                if (btnThis.Checked == true)
                {
                    numControl = x;
                    break;
                }
            }


            switch (numControl)
                {
                    case 0:
                        VerifyScreens_Firefox.tstFirefox(pth, datSource, steName, tstName, out fndExcep);
                        radnum = 0;
                        break;
                    case 1:
                        VerifyUserScreens_Chrome.tstChrome(pth, datSource, steName, tstName, out fndExcep);
                        radnum = 1;
                        break;
                    case 2:
                        VerifyScreens_IE.tstIE(pth, datSource, steName, tstName, out fndExcep);
                        radnum = 2;
                        break;
                    default:
                        VerifyScreens_Firefox.tstFirefox(pth, datSource, steName, tstName, out fndExcep);
                        break;
                }
        } 
        #endregion

        #region Form1_FormClosed
        private void Form1_FormClosed(Object sender, FormClosedEventArgs e)
        {
            TestSuite.killProc("WINWORD.EXE");
        }
        #endregion

        #region slctAllAvailable_Click
        private void slctAllAvailable_Click(object sender, EventArgs e)
        {
            string[] currList;

            //populate a string array with all the selected items in teh Available test box
            currList = new string[lstCaseSelect.Items.Count];

            //populate an array with the list of items currently in the Case Select box.
            //These will be sent to clkSelect as the original list 
            for (int i = 0; i < lstCaseSelect.Items.Count; i++)
                currList[i] =  lstCaseSelect.Items[i].ToString();

            //Select all items in the lstSelected listbox
            for (int i = 0; i < lstSelectedTest.Items.Count; i++ )
                lstSelectedTest.SetSelected(i, true);

            //Call clickSelectButton to populate the CaseSelect listbox 
            //clkSelectButton();

            //clear the SelectedTest listbox  
            lstSelectedTest.Items.Clear();

            btnDeslctAllAvailable.Enabled = true;
            btnSlctAllAvailable.Enabled = false;
            btnSelect.Enabled = false;
            //boxPerms();
        }
        #endregion

        #region slctAllSelected_Click
        private void slctAllSelected_Click(object sender, EventArgs e)
        {
            lstCaseSelect.Items.Clear();
            lstSelectedTest.SelectedItems.Clear();
            lstAvailableTest.SelectedItems.Clear();

            btnSlctAllAvailable.Enabled = true;
            btnDeslctAllAvailable.Enabled = false;
            btnSelect.Enabled = false;

            if (lstTestSuite.Enabled == false)
                lstTestSuite.Enabled = true;
            if (lstSelectedTest.Items[0].ToString() == "Regression Suite")
            {
                lstSelectedTest.Items.Clear();
                lstSelectedTest.Enabled = true;
            }

            //boxPerms();
        }
        #endregion

        #region boxPerms
        private void boxPerms()
        {
            //enable and disable buttons and checkboxes if the selection boxes go to or come from blank
            if (lstSelectedTest.Items.Count == 0)
            {
                btnDeslctAllAvailable.Enabled = false;
                btnDeselect.Enabled = false;
            }
            else
            {
                btnSlctAllAvailable.Enabled = true;
                btnDeslctAllAvailable.Enabled = true;
                btnDeselect.Enabled = true;
            }

            if (lstAvailableTest.Items.Count == 0)
            {
                btnSlctAllAvailable.Enabled = false;
                btnSelect.Enabled = false;
            }
            else
            {
                btnSlctAllAvailable.Enabled = true;
                btnSelect.Enabled = true;
            }
        }
        #endregion

        #region clkDeselectButton
        private void clkDeselectButton()
        {

            //see btnSelect_Click for comments
            ArrayList list = new ArrayList();
            ArrayList listgo = new ArrayList();
            for (int i = 0; i < lstCaseSelect.Items.Count; i++)
            {
                list.Add(lstCaseSelect.Items[i]);
                if (lstCaseSelect.SelectedItems.Contains(lstCaseSelect.Items[i]))
                {
                        listgo.Add(lstCaseSelect.Items[i]);
                        list.Remove(lstCaseSelect.Items[i]);
                }
            }

            lstCaseSelect.Items.Clear();
            lstCaseSelect.Items.AddRange(list.ToArray());


            //check listgo against what is in lstSelectedTests to verify non suite crossover
            string tstSelected = lstTestSuite.SelectedItem.ToString();

            if (tstSelected != "")
            {
                object[] array = db_access("SELECT name FROM test WHERE regression_suite_id = " + "(SELECT id FROM regression_suite WHERE name = '" + tstSelected + "')");
                for (int i = 0; i < array.Length; i++)
                {
                    if (array.Contains(listgo[i]))
                        lstSelectedTest.Items.Add(listgo[i]);
                }
            }
        }
        #endregion

        #region lstAvailableTest_SelectedIndexChanged
        private void lstAvailableTest_SelectedIndexChanged(object sender, EventArgs e)
        {
            int count;
            int fndItem;
            string tstSelected;
            string pth;
            string steName;
            string tstName;
            string[] newArray;
            string[] presArray;
            string[] currList;                              //The list of items currently in the Case Select box
            string[] tmpArray;                              //Temporary array containing the selected items. Used to match up case select

            lstSelectedTest.Items.Clear();

            count = 0;
            fndItem = 0;
            tstSelected = "";

            if (lstAvailableTest.SelectedItems.Count != 0)
            {
                switch (datSource)
                {
                    case "DB":
                        break;
                    case "EX":
                        tstSelected = lstAvailableTest.SelectedItem.ToString();
                pth = TestSuite.getXlPath(tstSelected);

                //Get the list of all tests to populate the Test Case listbox
                avblSuite = TestSuite.getTestList(pth);
                tmpArray = new string[avblSuite.Length];

                //Format the Avaliable test list
                if (lstCaseSelect.Items.Count > 0)
                {
                    currList = new string[lstCaseSelect.Items.Count];

                    for (int x = 0; x < tmpArray.Length; x++)
                    {
                        tmpArray[x] = "(" + lstAvailableTest.SelectedItem.ToString() + ") " + avblSuite[x];
                    }



                    //Get the number 
                    for (int x = 0; x < tmpArray.Length; x++)
                    {
                        for (int y = 0; y < lstCaseSelect.Items.Count; y++)
                        {
                            if (tmpArray[x] == (string)lstCaseSelect.Items[y].ToString())
                            {
                                count++;
                            }
                        }
                    }


                    if (count != 0)
                    {
                        presArray = new string[count];
                        count = 0;

                        for (int x = 0; x < tmpArray.Length; x++)
                        {
                            for (int y = 0; y < lstCaseSelect.Items.Count; y++)
                            {
                                if (tmpArray[x] == (string)lstCaseSelect.Items[y].ToString())
                                {
                                    getVal(tmpArray[x], out steName, out tstName);
                                    presArray[count] = tstName;
                                    count++;
                                }
                            }
                        }

                        //newArray will hold
                        count = 0;
                        newArray = new string[avblSuite.Length - presArray.Length];

                        for (int x = 0; x < avblSuite.Length; x++)
                        {
                            for (int y = 0; x < presArray.Length; y++)
                            {
                                if (avblSuite[x] == presArray[y])
                                {
                                    fndItem = 1;
                                    break;
                                }
                            }

                            if (fndItem != 1)
                            {
                                newArray[count] = avblSuite[x];
                                count++;
                            }

                            fndItem = 0;
                        }

                        //populate the newArray.
                        lstSelectedTest.Items.AddRange(newArray);
                    }
                    else
                    {
                        lstSelectedTest.Items.AddRange(avblSuite);
                    }
                }
            else
            {
                lstSelectedTest.Items.AddRange(avblSuite);
            }

                btnSlctAllAvailable.Enabled = true;
                btnSelect.Enabled = true;
                        break;
                }
                
            }
        }
        #endregion

        #region getSuitePath
        private string getSuitePath(string inString)
        {
            string pth;

            pth = "";

            switch (inString)
            {
                case "Verify Gradebook":
                    pth = Application.StartupPath + "\\Gradebook.txt";
                    break;
                case "Verify User Screens":
                    pth = Application.StartupPath + "\\Verify User Screens.txt";
                    break;
                case "Course Tests":
                    pth = Application.StartupPath + "\\Course Tests.txt";
                    break;
                case "Security Audit":
                    pth = Application.StartupPath + "\\Security Audit.txt";
                    break;
                case "ECHODEV - 491 Groups Administration":
                    pth = Application.StartupPath + "\\ECHODEV - 491 Groups Admin.txt";
                    break;
                case "Regression Tests":
                    pth = Application.StartupPath + "\\Regression Tests.txt";
                    break;
                case "ECHODEV - 611":
                    pth = Application.StartupPath + "\\611 Automation.txt";
                    break;
                default:
                    lstAvailableTest.Items.Clear();
                    break;
            }

            return pth;
        }
        #endregion

        #region getVal
        private void getVal(string inString, out string steName, out string caseName)
        {
            int strLen;
            int fndItem;
            int stString;
            string tempstring;
            string tmpSteName;
            string tmpCaseName;
            string char1;
            string char2;

            //instatiate variables
            fndItem = 0;
            stString = 0;
            tmpCaseName = "";
            tmpSteName = "";
            strLen = inString.Length;
            tempstring = "";

            for (int x = 0; x < strLen; x++)
            {
                char1 = inString.Substring(x, 1);
                char2 = inString.Substring(x + 1, 1);

                if (stString == 1)
                    tempstring = tempstring + Convert.ToString(char1);

                if (char1 == "(")
                    stString = 1;
                
                if ((char1 + char2) == ") ")
                {
                    tmpSteName = tempstring.Substring(0, tempstring.Length - 1);
                    tmpCaseName = inString.Substring(x + 2, strLen - (x + 2));
                    stString = 0;
                    fndItem = 1;
                    break;
                }
            }

            if (fndItem != 1)
            {
                caseName = inString;
                steName = "";
            }
            else
            {
                fndItem = 0;
                caseName = tmpCaseName;
                steName = tmpSteName;
            }
        }
        #endregion

        #region lstCaseSelect_SelectedIndexChanged
        private void lstCaseSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstCaseSelect.Items.Count != 0)
            {
                btnDeselect.Enabled = true;
                btnDeslctAllAvailable.Enabled = true;
            }
                
        }
        #endregion

        #region lstSelectedTest_SelectedIndexChanged
        private void lstSelectedTest_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSelectedTest.Items.Count != 0)
            {
                btnSelect.Enabled = true;
                btnSlctAllAvailable.Enabled = true;
            }
        }
        #endregion

        #region rdoDB_CheckedChanged
        private void rdoDB_CheckedChanged(object sender, EventArgs e)
        {            
            datSource = "DB";
            lstAvailableTest.Items.Clear();
            lstSelectedTest.Items.Clear();
            lstCaseSelect.Items.Clear();
            lstAvailableTest.Hide();
            label1.Hide();
            label3.Location = label1.Location;
            lstSelectedTest.Location = lstAvailableTest.Location;
            lstSelectedTest.Size = new Size(447, lstSelectedTest.Height);
            lstTestSuite.Items.Clear();
            rTest.Enabled = true;
            lstTestSuite.Items.Add("");
            lstTestSuite.Items.AddRange(db_access("SELECT name FROM regression_suite ORDER BY name ASC"));
        }
        #endregion

        #region rdoEX_CheckedChanged
        private void rdoEX_CheckedChanged(object sender, EventArgs e)
        {
            lstAvailableTest.Items.Clear();
            lstSelectedTest.Items.Clear();
            lstCaseSelect.Items.Clear();
            lstAvailableTest.Hide();
            lstAvailableTest.Show();
            label1.Show();
            label3.Location = new Point(167, 71); ;
            lstSelectedTest.Location = new Point(170, 87);
            lstSelectedTest.Size = new Size(289, lstSelectedTest.Height);


            string pth;
            string appPath;
            string[] lstSuite;

            datSource = "EX";

            appPath = Application.StartupPath;

            pth = appPath + "\\Suite List.txt";
            lstSuite = File.ReadAllLines(pth);
            lstTestSuite.Items.Clear();
            lstTestSuite.Items.AddRange(lstSuite);
            rTest.Enabled = false;

        }
        #endregion

        #region rTest_Click
        private void rTest_Click(object sender, EventArgs e)
        {
            /*reg = "";
            RegQ pop = new RegQ();
            pop.Show();*/


            lstTestSuite.Text = "";
            lstTestSuite.Enabled = false;
            lstSelectedTest.Items.Clear();
            lstSelectedTest.Items.Add("Regression Suite");
            lstSelectedTest.Enabled = false;

            lstCaseSelect.Items.Clear();
            lstCaseSelect.Items.AddRange(db_access("SELECT name FROM test WHERE isRegression <> ''"));

            //other buttons and fields states in here
            btnOK.Enabled = true;
            //btnDeselect.Enabled = true;
            btnDeslctAllAvailable.Enabled = true;
            btnSlctAllAvailable.Enabled = false;

            /*switch(reg)
            {
                case "Y":
                    {
                        datSource = "DB";
                        //Insert sql database call
                        //Add testNames to lstSelectedCases box
                        //Call SlctAllAvailable function
                        //Call buttonOKclick function
                        break;
                    }
                case "N":
                    {
                        break;
                    }
            }*/
        }
        #endregion

        #region thinArray
        private object[] thinArray(object[,] inArray)
        {
            int itmCount;
            object[] outArray; 

            //get the number of results in the second array dimension
            itmCount = inArray.Length;

            //initialize a new one dimensional array
            outArray = new object[itmCount];

            //populate the dataSuite array
            for (int x = 0; x < itmCount; x++)
                outArray[x] = inArray[0, x];

            //return the new array 
            return outArray;
        }
        #endregion

        #region db_access
        public object[] db_access(string strSQL)
        {
            ADODB.Connection objCon;
            ADODB.Recordset objRec;
            object[,] dataRows;
            object[] dataSuite;
            string strCon;

            objCon = new ADODB.Connection();
            objRec = new ADODB.Recordset();

            //establish the connection string and open the database connection  
            strCon = "driver={MySQL ODBC 5.1 Driver};server=107.22.232.228;uid=qa_people;pwd=thehandcontrols;" + 
                "database=functional_test_data;option=3";

            objCon.Open(strCon);

            //execute the SQL and return the recrodset of results
            objRec = objCon.Execute(strSQL, out missing, 0);

            //populate a two dinmensional object array with the results
            dataRows = objRec.GetRows();

            //get a onedimensional array that can be placed into the Test Suite dropdown
            dataSuite = thinArray(dataRows);

            //close the recordset
            objRec.Close();

            //close the database connection
            objCon.Close();

            return dataSuite;
        }
        #endregion
    }
}

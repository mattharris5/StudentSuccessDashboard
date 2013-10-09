using ADODB;
using System;
using System.Collections;
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
using Microsoft.VisualBasic;
using Microsoft.Office.Tools.Excel;
using Office = Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;
using VBA = Microsoft.Vbe.Interop;
using Verify_User_Screens_Firefox;
using echoAutomatedSuite;
namespace TestDriver_GUI
{
    public partial class TestDriverGUI : Form
    {
        object missing;
        string[] avblSuite;
        string[,] propArray;
        string[,] failArray;
        string[,] passArray;
        private int rtnIdx;
        //int appPause;
        int fndExcep;
        string datSource;
        public static string reg;
        DateTime currTime;
        ControlCollection defControls;
        ControlCollection okControls;

        #region Public Members

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
                string[] envSuite;
                string pth;
                string appPath = "";
            
                this.cmbIteration.SelectedItem = "1";
                appPath = Application.StartupPath;
                missing = Type.Missing;

                datSource = "EX";

                pth = appPath + "\\Suite List.txt";
                lstSuite = File.ReadAllLines(pth);
                lstTestSuite.Items.AddRange(lstSuite);

                pth = appPath + "\\Environments.txt";
                envSuite = File.ReadAllLines(pth);
                envSelector.Items.AddRange(envSuite);

                //set the initial form field states
                cmbProductBox.Text = cmbProductBox.Items[0].ToString();
                lstAvailableTest.Text = "";
                lstSelectedTest.Text = "";
                btnSelect.Enabled = false;
                btnDeselect.Enabled = false;
                btnPlay.Enabled = false;
                btnExtract.Enabled = false;
                btnSlctAllAvailable.Enabled = false;
                btnDeslctAllAvailable.Enabled = false;
            }
            #endregion

            #region btnOK_Click
            public void btnOK_Click(object sender, EventArgs e)
            {
                int total;
                int tstNum;
                int passTest;
                int failTest;
                int exitRes;
                int clrIndex;
                int tstFail;
                int typBrowser;
                int totTests;
                object[] tmpArray;
                string[,] currList;
                string[,] tstResult;
                string baseURL;
                string browser;
                string dataSource;
                string dirPth;
                string env;
                string suffFile;
                string slctString;
                string suiteName;
                string tmpID;
                string caseName;
                string xlPath;
                string profilePath;
                string fileName;
                string fldrString;
                string xlResName;
                string pth;
                string hours;
                string mins;
                string secs;

                Control.ControlCollection btnRadio;
                DialogResult runProd;
                DialogResult exitApp;
                DateTime tmp;
                DateTime strtIterTime;
                DateTime stpIterTime;
                DateTime strtTmpTime;
                DateTime fnlTmpTime;
                TimeSpan iterTime;
                TimeSpan tmpRunTime;
                TimeSpan fnlTime;
                DateTime startTime;

                exitRes = 0;
                xlResName = "";
                profilePath = "";
                dataSource = ""; 
                fileName = "";
                fldrString = "";
                pth = "";
                xlPath = Application.StartupPath + "\\Results\\Excel Results\\Excel Results Template\\TestResults.xlsm";
                passTest = 0;
                failTest = 0;
                tstNum = 0;
                failArray = null;
                passArray = null;
                propArray = cntrlArray();
                tstResult = new string[1, 9];

                if (lstCaseSelect.Items.Count == 0)
                {
                    //
                    MessageBox.Show("You must have Test Cases added to the Selected Test Cases Listbox", 
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    //the current state of the form. This will be used as a reference to reset
                    okControls = (ControlCollection)this.Controls;

                    //reset the form
                    resetForm(okControls, propArray);

                    rtnIdx = 1;

                    //select the current selection to fire the lstAvailableTest_SelectedIndexChanged event
                    if (datSource == "EX")
                        //select the current selection to fire the lstAvailableTest_Se;ectedINdexChanged event
                        lstAvailableTest_SelectedIndexChanged(lstAvailableTest, e);
                    else if (datSource == "DB")
                        lstTestSuite_SelectedIndexChanged(lstTestSuite.Text, e);
                }

                else if (envSelector.Text == "")
                {
                    MessageBox.Show("You must select an environment in which to to run tests",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    //the current state of the form. This will be used a s a reference to reset
                    okControls = (ControlCollection)this.Controls;

                    //reset the form
                    resetForm(okControls, propArray);

                    rtnIdx = 1;

                    //select the current selection to fire the lstAvailableTest_SelectedIndexChanged event
                    if (datSource == "EX")
                        lstAvailableTest_SelectedIndexChanged(lstAvailableTest, e);
                    else if (datSource == "DB")
                        lstTestSuite_SelectedIndexChanged(lstTestSuite.Text, e);
                }
                else
                {
                    clrIndex = 0;
                    suffFile = "";
                    runProd = DialogResult.Yes;
                    baseURL = "";
                    btnRadio = slctBrowser.Controls;

                    //disable all the controls on the form 
                    lstTestSuite.Enabled = false;
                    btnPlay.Enabled = false;
                    //btnPause.Enabled = false;
                    //btnStop.Enabled = false;
                    btnExtract.Enabled = false;
                    btnCancel.Enabled = false;
                    btnDeslctAllAvailable.Enabled = false;
                    btnSelect.Enabled = false;
                    btnSlctAllAvailable.Enabled = false;
                    lstAvailableTest.Enabled = false;
                    lstSelectedTest.Enabled = false;
                    lstCaseSelect.Enabled = false;
                    lstTestSuite.Enabled = false;

                    if (chkGenExcel.Checked == true)
                    {
                        do
                        {
                            xlResName = Interaction.InputBox("You have opted to generate a set of results in Excel. Please give the Excel spreadsheet a name.", "Excel Results Name",
                                "xlResults1");

                            if (xlResName == "")
                            {
                                exitApp = MessageBox.Show("Pressing 'Yes' will exit the application. Do you wish to do this?", "Exit Application", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                                if (exitApp == DialogResult.Yes)
                                {
                                    Thread.Sleep(250);
                                    exitRes = -1;
                                    break;
                                }
                            }
                        } while (xlResName == "");
                    }

                    if (exitRes != -1)
                    {
                        env = envSelector.Text;

                        //set the environment URL based on the envSelector
                        switch(cmbProductBox.Text)
                        {
                            case "Strive - SSD":
                            {
                                switch (envSelector.Text)
                                {
                                    case "Dev":
                                        baseURL = "https://covington.dev.studentsuccessdashboard.com/";
                                        break;
                                    case "UAT":
                                        baseURL = "http://uat.studentsuccessdashboard.com";
                                        break;
                                    case "Staging":
                                        baseURL = "https://covington.staging.studentsuccessdashboard.com/";
                                        break;
                                    case "Prod":
                                        baseURL = "https://covington.studentsuccessdashboard.com/";
                                        runProd = MessageBox.Show("You will be running in the Production environment. Are you sure you want to do this?",
                                        "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                        break;
                                }
                                break;
                            }
                        }

                        if (runProd == DialogResult.Yes)
                        {
                            //populate an array with the list of items currently in the Selected Items box
                            currList = new string[lstCaseSelect.Items.Count, 2];
                            total = lstCaseSelect.Items.Count;

                            if (datSource != "DB")
                            {
                                for (int i = 0; i < lstCaseSelect.Items.Count; i++)
                                {
                                    slctString = lstCaseSelect.Items[i].ToString();

                                    currList[i, 0] = lstAvailableTest.SelectedItem.ToString();
                                    currList[i, 1] = lstCaseSelect.Items[i].ToString();
                                    suiteName = currList[i, 0];
                                    caseName = currList[i, 1];
                                }
                            }
                            else
                            {
                                for (int i = 0; i < lstCaseSelect.Items.Count; i++)
                                {
                                    //set the test case variable
                                    currList[i, 1] = lstCaseSelect.Items[i].ToString();

                                    //get a one item array with the regression suite id from the test table
                                    tmpArray = db_access("SELECT regression_suite_id FROM test WHERE name = '"
                                                                        + currList[i, 1] + "'", ref fndExcep);
                                    tmpID = Convert.ToString(tmpArray[0]);

                                    tmpArray = db_access("SELECT name FROM regression_suite WHERE id = '" + tmpID + "'", ref fndExcep);
                                    currList[i, 0] = Convert.ToString(tmpArray[0]);
                                }
                            }

                            //Setup the directory where the results file will be stored
                            dirPth = Application.StartupPath + "\\Results";
                            tmp = DateTime.Now;                                                     //date-time at the time of test running
                            suffFile = tmp.Month.ToString() + tmp.Day.ToString() + tmp.Year.ToString().Substring(2, 2) + tmp.Hour.ToString() +
                                tmp.Minute.ToString() + tmp.Second.ToString();

                            //set the date-time value to a string
                            fileName = "Result_" + suffFile + ".html";
                            fldrString = "Result_" + suffFile;
                            pth = dirPth + "\\" + fileName;

                            //create a result text file  and write a beginning message
                            TextFileOps.Open(pth);

                            //write style tag 
                            TextFileOps.Write(pth, "<!DOCTYPE html>", 100);
                            TextFileOps.Write(pth, "<html>", 100);
                            TextFileOps.Write(pth, "<head>", 100);
                            TextFileOps.Write(pth, "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">", 100);
                            TextFileOps.Write(pth, "<script src=\"" + Application.StartupPath + "\\JQuery\\jquery-1.8.2.min.js\"></script>", 100);
                            TextFileOps.Write(pth, "<script src=\"" + Application.StartupPath + "\\JQuery\\tree.jquery.js\"></script>", 100);
                            TextFileOps.Write(pth, "<link rel=\"stylesheet\" src=\"" + Application.StartupPath + "\\JQuery\\jqtree.css\">", 100);

                            TextFileOps.Write(pth, "</head>", 100);
                            TextFileOps.Write(pth, "<body>", 100);
                            //start timer1 to calculate the complete test run
                            timer1.Start();
                            startTime = DateTime.Now;
                            switch(cmbProductBox.Text)
                            {
                                case "Strive - SSD":
                                {
                                    switch (baseURL)
                                    {
                                        case "https://covington.dev.studentsuccessdashboard.com/":
                                            TextFileOps.Write(pth, "Environment: Dev", 95);
                                            break;
                                        case "https://uat.studentsuccessdashboard.com/User":
                                            TextFileOps.Write(pth, "Environment: UAT", 95);
                                            break;
                                        case "https://covington.staging.studentsuccessdashboard.com/":
                                            TextFileOps.Write(pth, "Environment: Staging", 95);
                                            break;
                                        case "https://covington.studentsuccessdashboard.com/":
                                            TextFileOps.Write(pth, "Environment: Prod", 95);
                                            break;
                                    }

                                    break;
                                }
                            }
                            //get the browser name based onn the selection in the btnRadio group box
                            typBrowser = browserSelect(btnRadio);

                            //write the browser name to the  result file
                            switch (typBrowser)
                            {
                                case 0:
                                    browser = "Safari";
                                    TextFileOps.Write(pth, "Test Browser: Apple Safari", 0);
                                    TextFileOps.Write(pth, "Opening Safari.......", 0);
                                    //TextFileOps.Write(pth, "<br />", 0);
                                    break;
                                case 2:
                                    browser = "Chrome";
                                    TextFileOps.Write(pth, "Test Browser: Google Chrome", 0);
                                    TextFileOps.Write(pth, "Opening Chrome.......", 0);
                                    //TextFileOps.Write(pth, "<br />", 0);
                                    break;
                                case 1:
                                    browser = "Firefox";
                                    TextFileOps.Write(pth, "Test Browser: Mozilla Firefox", 0);
                                    TextFileOps.Write(pth, "Opening Firefox.......", 0);
                                    //TextFileOps.Write(pth, "<br />", 0);
                                    break;
                                case 3:
                                    browser = "Internet Explorer";
                                    TextFileOps.Write(pth, "Test Browser: MS Internet Explorer", 0);
                                    TextFileOps.Write(pth, "Opening Internet Explorer.......", 0);
                                    //TextFileOps.Write(pth, "<br />", 0);
                                    break;
                                default:
                                    browser = "Not Found";
                                    break;
                            }

                            //record the data source in the result file 
                            dataSource = "Excel";
                            TextFileOps.Write(pth, "Data Source: Excel<br/><br/>", 80);

                            /*if (rdoDB.Checked == true)
                            {
                                dataSource = "Database";
                                TextFileOps.Write(pth, "Data Source: Database<br/><br/>", 80);
                            }
                            else if (rdoEX.Checked == true)
                            {
                                dataSource = "Excel";
                                TextFileOps.Write(pth, "Data Source: Excel<br/><br/>", 80);
                            }*/

                            //list the tests for this result sheet 
                            TextFileOps.Write(pth, "Tests in this run:<br/>", 80);
                            for (int tstCount = 0; tstCount < lstCaseSelect.Items.Count; tstCount++)
                            {
                                TextFileOps.Write(pth, lstCaseSelect.Items[tstCount].ToString() + "<br/>", 80);
                            }

                            TextFileOps.Write(pth, "<br />", 0);
                            totTests = lstCaseSelect.Items.Count * Convert.ToInt32(cmbIteration.Text);

                            //runn the selected tests x number of iterations from the cmbIteration dropdown
                            for (int iter = 0; iter < Convert.ToInt32(cmbIteration.Text); iter++)
                            {
                                lblIterCount.Text = (iter + 1).ToString();

                                //get the iteration start time amd start the iteration timer
                                strtIterTime = DateTime.Now;
                                timer3.Start();

                                //write the iteration marker to the results
                                TextFileOps.Write(pth, "Iteration: " + Convert.ToString(iter + 1), 0);

                                //set the initial pass/fail counters 
                                lblTestPass.Text = passTest.ToString();
                                lblTestFail.Text = failTest.ToString();

                                //run the current test from the currList array
                                for (int tst = 0; tst < currList.GetLength(0); tst++)
                                {
                                    //set thee selected test in the lstCaseSelect list box to the currently running index 
                                    lstCaseSelect.SetSelected(tst, true);

                                    //Set the suite and currently running test into the currently running labels at the bottom of the GUI
                                    txtSuiteRunning.Text = currList[tst, 0];
                                    txtTestRunning.Text = currList[tst, 1];

                                    //set the initial test counter number, (0/[number of tests selected * iterations])
                                    lblTstRun.Text = tstNum + "/" + (lstCaseSelect.Items.Count * Convert.ToInt32(cmbIteration.Text)).ToString();

                                    //increment the test number for stats
                                    tstNum++;

                                    this.Refresh();

                                    //get the test start time value and start the test timer
                                    strtTmpTime = DateTime.Now;
                                    timer2.Start();
                                    TextFileOps.Write(pth, "<div id=\"Test Container\">", 100);
                                    TextFileOps.Write(pth, "<ul class=\"jqtree-tree\">", 100);
                                    TextFileOps.Write(pth, "<li class=\"jqtree-folder jqtree-closed\">", 100);
                                    TextFileOps.Write(pth, "<div>", 100);
                                    TextFileOps.Write(pth, "<span class=\"jqtree-title\" style=\"font-family:verdana;font-size:75%;color:#000000\"><b>" + currList[tst, 1] + "</b></span>", 100);
                                    TextFileOps.Write(pth, "</div>", 100);
                                    TextFileOps.Write(pth, "<ul>", 100);

                                    //run this test
                                    runFunction(currList[tst, 0], currList[tst, 1], typBrowser, datSource, pth, baseURL, ref tstResult, ref profilePath, out fndExcep, out tstFail);

                                    TextFileOps.Write(pth, "</ul>", 100);

                                    //stop test timer
                                    fnlTmpTime = DateTime.Now;
                                    timer2.Stop();

                                    //set the final time and format each element
                                    tmpRunTime = fnlTmpTime - strtTmpTime;

                                    //format the timespan into a string for output
                                    if (tmpRunTime.Hours < 10)
                                        hours = "0" + tmpRunTime.Hours.ToString();
                                    else
                                        hours = Convert.ToString(tmpRunTime.Hours);

                                    if (tmpRunTime.Minutes < 10)
                                        mins = "0" + tmpRunTime.Minutes.ToString();
                                    else
                                        mins = Convert.ToString(tmpRunTime.Minutes);

                                    if (tmpRunTime.Seconds < 10)
                                        secs = "0" + tmpRunTime.Seconds.ToString();
                                    else
                                        secs = Convert.ToString(tmpRunTime.Seconds);

                                    //write the automated test time into the results file 
                                    TextFileOps.Write(pth, "<li>", 100);
                                    TextFileOps.Write(pth, currList[tst, 1] + " automated test completed in " + hours + ":" + mins + ":" + secs, 0);
                                    TextFileOps.Write(pth, "</li>", 100);

                                    TextFileOps.Write(pth, "</li>", 100);
                                    TextFileOps.Write(pth, "</ul>", 100);
                                    TextFileOps.Write(pth, "</div>", 100);

                                    //line break
                                    TextFileOps.Write(pth, "<br />", 0);

                                    //unselect the currently running test 
                                    lstCaseSelect.SetSelected(tst, false);

                                    //set the GUI stats variables and field
                                    lblTstRun.Text = tstNum.ToString() + "/" + totTests.ToString();

                                    if (fndExcep == -1 || tstFail == -1)
                                    {
                                        failTest++;
                                        lblTestFail.Text = failTest.ToString();
                                        if (failTest > 0)
                                            lblTestFail.ForeColor = System.Drawing.Color.Red;
                                        else
                                            lblTestFail.ForeColor = System.Drawing.Color.Black;
                                    }
                                    else
                                    {
                                        passTest++;
                                        lblTestPass.Text = passTest.ToString();
                                        if (passTest > 0)
                                            lblTestPass.ForeColor = System.Drawing.Color.DarkGreen;
                                        else
                                            lblTestPass.ForeColor = System.Drawing.Color.Black;
                                    }

                                    //refresh the GUI
                                    this.Refresh();

                                    //append any failed tests to failArray if test fails. If test passes append the passArray                            
                                    if (fndExcep == -1 || tstFail == -1)
                                    {
                                        failArray = createArray((iter + 1).ToString(), currList[tst, 1], failArray);

                                        if (fndExcep == -1)
                                            fndExcep = 0;
                                        else
                                            tstFail = 0;
                                    }
                                    else
                                    {
                                        passArray = createArray((iter + 1).ToString(), currList[tst, 1], passArray);
                                    }
                                }

                                stpIterTime = DateTime.Now;
                                iterTime = stpIterTime - strtIterTime;
                                timer3.Stop();

                                //if there is only one iteration, there is no need to run the iteration timer
                                //else get the iteration info and post to the results file
                                if (Convert.ToInt32(cmbIteration.Text) > 1)
                                {
                                    //format the timespan into a string for output
                                    if (iterTime.Hours < 10)
                                        hours = "0" + iterTime.Hours.ToString();
                                    else
                                        hours = Convert.ToString(iterTime.Hours);

                                    if (iterTime.Minutes < 10)
                                        mins = "0" + iterTime.Minutes.ToString();
                                    else
                                        mins = Convert.ToString(iterTime.Minutes);

                                    if (iterTime.Seconds < 10)
                                        secs = "0" + iterTime.Seconds.ToString();
                                    else
                                        secs = Convert.ToString(iterTime.Seconds);

                                    TextFileOps.Write(pth, "Iteration " + Convert.ToString(iter + 1) + " completed in: " + hours + ":" + mins + ":" + secs + "\r\n", 75);

                                    if (iter != (Convert.ToInt32(cmbIteration.Text) - 1))
                                        TextFileOps.Write(pth, "<br />", 0);
                                }
                            }

                            //Send message to result file noting the end of the suite
                            for (int a = 0; a < 1; a++)
                                TextFileOps.Write(pth, "End Test Run.........." + "\r\n", 0);

                            //write the failed list of tests to the results file 
                            if (failArray != null)
                            {
                                TextFileOps.Write(pth, "Failed Tests: ", 75);
                                for (int x = 0; x < failArray.GetLength(0); x++)
                                {
                                    TextFileOps.Write(pth, "Iteration: " + failArray[x, 0] + "\t\t\t\tTest: " + failArray[x, 1] + "\r\n", -1);
                                    TextFileOps.Write(pth, "<br />", 80);
                                }
                            }
                            else
                            {
                                TextFileOps.Write(pth, "There were no failed tests in the run", 0);
                            }

                            //re-enable all the controls on the form 
                            lstTestSuite.Enabled = true;
                            btnPlay.Enabled = true;
                            btnExtract.Enabled = false;
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

                            //format the timespan into a string for output
                            if (fnlTime.Hours < 10)
                                hours = "0" + fnlTime.Hours.ToString();
                            else
                                hours = Convert.ToString(fnlTime.Hours);

                            if (fnlTime.Minutes < 10)
                                mins = "0" + fnlTime.Minutes.ToString();
                            else
                                mins = Convert.ToString(fnlTime.Minutes);

                            if (fnlTime.Seconds < 10)
                                secs = "0" + fnlTime.Seconds.ToString();
                            else
                                secs = Convert.ToString(fnlTime.Seconds);

                            lblTimer.Text = hours + ":" + mins + ":" + secs;

                            TextFileOps.Write(pth, "Total time for this automated test run: " + hours + ":" + mins + ":" + secs + "\r\n", 75);
                            TextFileOps.Write(pth, "<br />", clrIndex);

                            //write closing tags to result file
                            TextFileOps.Write(pth, "<script>", 100);
                            TextFileOps.Write(pth, "$(function()\n{\n\tvar data = [{label: 'node1',children: [{ label: 'child1' },{ label: 'child2' }]},{label: 'node2',children: " +
                                "[{ label: 'child3' }]}];\n\t$('#Test Container').tree({data: data}});\n);", 100);
                            TextFileOps.Write(pth, "</script>", 100);
                            TextFileOps.Write(pth, "</body>", clrIndex = 100);
                            TextFileOps.Write(pth, "</html>", clrIndex = 100);

                            //show the result file
                            Process.Start(pth);

                            if (chkGenExcel.Checked == true)
                            {
                                popExcelResults(xlPath, passArray, failArray, totTests, Convert.ToInt32(passTest), Convert.ToInt32(failTest), startTime, 
                                    lblTimer.Text, browser, env, dataSource, xlResName, pth);
                            }
                        }
                        else
                        {
                            //the current state of the form. This will be used a s a reference to reset
                            okControls = (ControlCollection)this.Controls;

                            //reset the form
                            resetForm(okControls, propArray);

                            rtnIdx = 1;

                            //select the current selection to fire the lstAvailableTest_SelectedIndexChanged event
                            if (datSource == "EX")
                                lstAvailableTest_SelectedIndexChanged(lstAvailableTest, e);
                            else if (datSource == "DB")
                                lstTestSuite_SelectedIndexChanged(lstTestSuite.Text, e);
                        }
                    }
                    else
                    {
                        this.Close();
                    }
                }


                //delete the profile directory in the temp directory
                if (profilePath != "") 
                    Directory.Delete(profilePath, true);

                btnExtract.Enabled = true;
            }
            #endregion

            #region envSelector_Changed
            private void envSelector_Changed(object sender, EventArgs e)
            {
                if (envSelector.Text != "")
                {
                    if (lstCaseSelect.Items.Count > 0)
                    {
                        btnPlay.Enabled = true;
                    }
                    label10.Font = new Font(label10.Font, FontStyle.Bold);
                }
                else
                {
                    if (lstCaseSelect.Items.Count > 0)
                    {
                        btnPlay.Enabled = false;
                    }
                    label10.Font = new Font(label10.Font, FontStyle.Regular);
                }
            }
            #endregion

            #region btnExtract_Click
            private void btnExtract_Click(object sender, EventArgs e)
            {
                object[] rtnList;
                string[] lstArray;
                string argID;
                string itmList;
                string strCon; 
                string steID;
                string thsFuncID;
                string tstID;
                int itmCount;
                int argNum;
                int numSteps;
                int numTests;
                ArrayList slctList;
                ADODB.Connection objCon;
                DialogResult valExtract;

                itmList = "";
                itmCount = lstCaseSelect.Items.Count;
                lstArray = new string[itmCount];
                strCon = "driver={MySQL ODBC 5.1 Driver};server=107.22.232.228;uid=qa_people;pwd=thehandcontrols;" +
                    "database=functional_test_data;option=3";

                objCon = new ADODB.Connection();

                //set lstArray to all items in the lstCaseSelect box
                for (int x = 0; x < itmCount; x++)
                    lstArray[x] = lstCaseSelect.Items[x].ToString();

                slctList = new ArrayList(itmCount);

                for (int x = 0; x < itmCount; x++)
                {
                    //get a carriage return delimited string of all entries in lstCaseSelect
                    itmList = itmList + lstArray[x] + "\r\n";

                    //add the items to slctList whiule iterating through the lstCaseSelect items
                    slctList.Add(lstCaseSelect.Items[x]);
                }

                //show an information message box with an escape option
                valExtract = MessageBox.Show("You will be extracting the following tests from the database \r\n\r\n" + itmList + "\r\nSelect Yes to continue. Select No to return to TestDriver ",
                        "Database Test Extractor", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (valExtract == DialogResult.Yes)
                {
                    for (int cnt = 0; cnt < itmCount; cnt++)
                    {
                        string strSQL;

                        //get the function ID aznd number of steps from the test table
                        strSQL = "SELECT id, number_of_steps FROM test WHERE name = '" + lstArray[cnt] + "'";
                        rtnList = db_access(strSQL, ref fndExcep);
                        tstID = rtnList[0].ToString();

                        if (rtnList[1].ToString() != "")
                        {
                            numSteps = Convert.ToInt32(rtnList[1]);
                        }
                        else
                        {
                            numSteps = 0;
                        }

                        //open a connection  to the database
                        objCon.Open(strCon);

                        //set a for loop with x + 1 being the current step number being processed
                        //renmove all steps that are not used in any other tests (recCount = 1)
                        for (int x = 0; x < numSteps; x++)
                        {
                            //get the function id and arg set id using the 
                            strSQL = "SELECT function_id, argument_set_id FROM step WHERE (test_id = '" + tstID + "' AND number = '" + (x + 1).ToString() + "')" ;
                            rtnList = db_access(strSQL, ref fndExcep);
                            thsFuncID = rtnList[0].ToString();
                            argID = rtnList[1].ToString();

                            strSQL = "DELETE FROM step WHERE argument_set_id = '" + argID + "' AND function_id = '" + thsFuncID +
                                    "' AND test_id = '" + tstID + "' AND number = '" + (x + 1).ToString() + "'";
                            objCon.Execute(strSQL, out missing, 0);

                            //if an argument set is no longer used, gert rid of it
                            strSQL = "SELECT COUNT(*) FROM step WHERE argument_set_id = '" + argID + "'";
                            rtnList = db_access(strSQL, ref fndExcep);
                            argNum = Convert.ToInt32(rtnList[0]);

                            if (argNum == 0)
                            {
                                strSQL = "DELETE FROM argument WHERE argument_set_id = '" + argID + "'";
                                objCon.Execute(strSQL, out missing, 0);
                            
                                //delete t
                                strSQL = "DELETE FROM argument_set WHERE id = '" + argID + "'";
                                objCon.Execute(strSQL, out missing, 0); 
                            }
                        }

                        //get the regression suite id from the test being extracted
                        strSQL= "SELECT regression_suite_id FROM test WHERE id = '" + tstID + "'";
                        rtnList = db_access(strSQL, ref fndExcep);
                        steID = rtnList[0].ToString();
                    
                        //delete the test from the test rable 
                        strSQL = "DELETE FROM test WHERE  id = '" + tstID + "'";
                        objCon.Execute(strSQL, out missing, 0);

                        //get the number of tests from in the regression suite. If delete the regression suite
                        strSQL = "SELECT COUNT(*) FROM test WHERE regression_suite_id = '" + steID + "'";
                        rtnList = db_access(strSQL, ref fndExcep);
                        numTests = Convert.ToInt32(rtnList[0]);
                    
                        //if there are no tests left in the database delete the suite
                        if (numTests == 0)
                        {
                            strSQL = "DELETE FROM regression_suite WHERE id = '" + steID + "'";
                            objCon.Execute(strSQL, out missing, 0);
                        }

                        //close the database connection
                        objCon.Close();

                        //get the list of 
                        for (int x = 0; x < slctList.Count; x++)
                        {
                            if (Convert.ToString(slctList[x]) == lstArray[cnt])
                            {
                                slctList.Remove(lstArray[cnt]);
                                break;
                            }
                        }
                    }

                    lstCaseSelect.Items.Clear();
                }
            }
            #endregion

            #region btnCancel_Click
            private void btnCancel_Click(object sender, EventArgs e)
            {
                this.Close();
            }
            #endregion

            #region btnUpdate_Click
            public void btnUpdate_Click(object sender, EventArgs e)
            {
                if (lstCaseSelect.SelectedItems.Count == 1)
                {
                    dbUpdater dbUpdater;

                    dbUpdater = new dbUpdater(lstCaseSelect.SelectedItem.ToString());
                    dbUpdater.Show();
                }
                else if (lstCaseSelect.SelectedItems.Count < 1) 
                {
                    MessageBox.Show("You need to have one test selected. Please select a test to update the step data for", "Step Updater", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    MessageBox.Show("You have more than one test selected. Please select only one test to update the step data for", "Step Updater", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            #endregion

            #region btnResetForm_Click
            private void btnResetForm_Click(object sender, EventArgs e)
            {
                lstAvailableTest.Items.Clear();
                lstSelectedTest.Items.Clear();
                lstCaseSelect.Items.Clear();
                lstTestSuite.SelectedIndex = 0;
                this.cmbIteration.SelectedItem = "1";
                //rdoDB.Checked = true;

                //the current state of the form. This will be used a s a reference to reset
                okControls = (ControlCollection)this.Controls;
                envSelector.SelectedIndex = envSelector.Items.IndexOf("");

                propArray = cntrlArray();

                //reset the form
                resetForm(okControls, propArray);

                rtnIdx = 2;

                //select the current selection to fire the lstAvailableTest_SelectedIndexChanged event
                if (datSource == "EX")
                    lstAvailableTest_SelectedIndexChanged(lstAvailableTest, e);

                rtnIdx = 0;

                //set the initial form field states
                cmbProductBox.SelectedIndex = 0;
                btnExtract.Enabled = true;
                lstAvailableTest.Text = "";
                lstSelectedTest.Text = "";
                btnSelect.Enabled = true;
                btnDeselect.Enabled = false;
                btnPlay.Enabled = false;
                btnExtract.Enabled = false;
                btnSlctAllAvailable.Enabled = false;
                btnDeslctAllAvailable.Enabled = false;
                lstSelectedTest.Enabled = true;

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
                lstCaseSelect.Items.Clear();
                lstSuite = new string[1];
                //the selected item in the list box
                tstSelected = lstTestSuite.SelectedItem.ToString();


                if (lstTestSuite.Text != "")
                {
                    switch (datSource)
                    {
                        /*case "DB":
                        {
                            object[] array = db_access("SELECT name FROM test WHERE regression_suite_id = " + "(SELECT id FROM regression_suite WHERE name = '" + tstSelected + "')", ref fndExcep);

                            if (fndExcep != -1)
                            {
                                for (int i = 0; i < array.Length; i++)
                                {
                                    if (lstCaseSelect.Items.Contains(array[i]) == false)
                                    {
                                        lstSelectedTest.Items.Add(array[i]);
                                    }
                                }
                            }
                            else
                            {
                                //pop a message box stating that no records were returned
                                MessageBox.Show("No record of any existing tests were returned from the database for the following suite:\r\n\r\n" + tstSelected +
                                "\r\n\r\nPlease check that there is data for this suite in the database", "No Database Records Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                                //reset the fndExcep variable. the message box has been popped and the form reloads ewaiting for a valid selection
                                fndExcep = 0;
                            }

                            break;
                        }*/
                        case "EX":
                        {


                            lstAvailableTest.Text = "";
                            lstSelectedTest.Text = "";
                            lstSuite[0] = lstTestSuite.Text;
                            lstAvailableTest.Items.AddRange(lstSuite);
                            lstAvailableTest.SelectedIndex = 0;
                            break;

                            /*pth = getSuitePath(tstSelected);

                            lstAvailableTest.Text = "";
                            lstSelectedTest.Text = "";

                            lstSuite = File.ReadAllLines(pth);

                            lstAvailableTest.Items.AddRange(lstSuite);
                            break;*/
                        }
                    }

                    if (rtnIdx == 0)
                    {
                       // slctDataSource.Enabled = false;
                    }
                    else if (rtnIdx == 2)
                    {
                    }
                    else
                    {
                        rtnIdx = 0;
                    }

                    btnSelect.Enabled = true;
                    lstSelectedTest.Enabled = true;
                    btnSlctAllAvailable.Enabled = true;
                }
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
                        btnDeselect.Enabled = true;
                        btnDeslctAllAvailable.Enabled = true;
                        btnExtract.Enabled = true;

                        //enable the OK button if the the Environment selector field is not blank 
                        if (envSelector.Text != "")
                            btnPlay.Enabled = true;

                        //remove item from return list
                        list.Remove(lstSelectedTest.Items[i]);
                    }
                }
                //clear selected return listbox and populate it with the return list
                lstSelectedTest.Items.Clear();
                lstSelectedTest.Items.AddRange(list.ToArray());
                if (lstSelectedTest.Items.Count == 0)
                {
                    btnSlctAllAvailable.Enabled = false;
                    btnSelect.Enabled = false;
                }
            }
            #endregion

            #region btnDeselect_Click
            private void btnDeselect_Click(object sender, EventArgs e)
            {
                string[] tmpArray;
                string[] fnlArray;
                string[] slctArray;
                string[] caseArray;
                string[] stringArray;
                string tstSelected;
                string pth;

                //see btnSelect_Click for comments
                ArrayList list = new ArrayList();
                ArrayList listgo = new ArrayList();

                fnlArray = null;
                caseArray = null;            

                switch (datSource)
                {
                    case "DB":
                        //check listgo against what is in lstSelectedTests to verify non suite crossover
                        string tstsSelected = lstTestSuite.SelectedItem.ToString();
                        //check if there is a value selected from the dropdown
                        if (tstsSelected != "")
                        {
                            //get the values from the currently selected suite
                            object[] array = db_access("SELECT name FROM test WHERE regression_suite_id = " +
                                "(SELECT id FROM regression_suite WHERE name = '" + tstsSelected + "')", ref fndExcep);

                            //set stringArray
                            stringArray = new string[array.Length];

                            //convert array to array of strings 
                            for (int idx = 0; idx < array.Length; idx++)
                                stringArray[idx] = (string)array[idx];

                            populateSelectBoxes(stringArray, out tmpArray, out slctArray, out caseArray, out fnlArray);

                            //clear the SelectedItems and CaseSelect boxes
                            lstSelectedTest.Items.Clear();
                            lstCaseSelect.Items.Clear();

                            for (int x = 0; x < fnlArray.Length; x++)
                                lstSelectedTest.Items.Add(fnlArray[x]);

                            for (int x = 0; x < caseArray.Length; x++)
                                lstCaseSelect.Items.Add(caseArray[x]);
                        }
                        break;
                    case "EX":
                        tstSelected = lstAvailableTest.SelectedItem.ToString();
                        pth = TestSuite.getXlPath(tstSelected);
                        //Get the list of all tests to populate the Test Case listbox
                        avblSuite = TestSuite.getTestList(pth);
                        list.Clear();
                        list.AddRange(avblSuite);
                        if (tstSelected != "")
                        {
                            populateSelectBoxes(avblSuite, out tmpArray, out slctArray, out caseArray, out fnlArray);
                            lstSelectedTest.Items.Clear();
                        }

                        //clear the SelectedItems and CaseSelect boxes
                        lstSelectedTest.Items.Clear();
                        lstCaseSelect.Items.Clear();

                        for (int x = 0; x < fnlArray.Length; x++)
                                lstSelectedTest.Items.Add(fnlArray[x]);

                        for (int x = 0; x < caseArray.Length; x++)
                            lstCaseSelect.Items.Add(caseArray[x]);

                        break;
                }
                if (lstSelectedTest.Items.Count != 0)
                {
                    btnSlctAllAvailable.Enabled = true;
                    btnSelect.Enabled = true;
                }
                if (lstCaseSelect.Items.Count == 0)
                {
                    btnDeselect.Enabled = false;
                    btnPlay.Enabled = false;
                    btnDeslctAllAvailable.Enabled = false;
                    btnExtract.Enabled = false;
                }
                else
                {
                    btnDeselect.Enabled = true;
                    btnDeslctAllAvailable.Enabled = true;
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
                for (int i = 0; i < lstSelectedTest.Items.Count; i++)
                {
                    lstCaseSelect.Items.Add(lstSelectedTest.Items[i]);
                    btnDeselect.Enabled = true;
                    btnPlay.Enabled = true;
                    btnExtract.Enabled = true;
                    btnDeslctAllAvailable.Enabled = true;
                }
                lstSelectedTest.Items.Clear();
                btnSlctAllAvailable.Enabled = false;
                btnSelect.Enabled = false;
            }
            #endregion

            #region slctAllSelected_Click
            private void slctAllSelected_Click(object sender, EventArgs e)
            {

                switch (datSource)
                {

                    case "DB":
                        //check listgo against what is in lstSelectedTests to verify non suite crossover
                        string tstSelected = lstTestSuite.SelectedItem.ToString();
                        //check if there is a value selected from the dropdown
                        if (tstSelected != "")
                        {
                            //get the values from the currently selected suite
                            object[] array = db_access("SELECT name FROM test WHERE regression_suite_id = " + "(SELECT id FROM regression_suite WHERE name = '" + tstSelected + "')", ref fndExcep);
                            //loop through and check the items to be deselected, and add the ones contained in the currently selected suite to the selectedcase list box   
                            for (int x = 0; x < lstCaseSelect.Items.Count; x++)
                            {
                                if (array.Contains(lstCaseSelect.Items[x]))
                                {
                                    lstSelectedTest.Items.Add(lstCaseSelect.Items[x]);
                                    btnSelect.Enabled = true;
                                }
                            }
                        }
                        break;
                    case "EX":
                        string pth;
                        tstSelected = lstAvailableTest.SelectedItem.ToString();
                        pth = TestSuite.getXlPath(tstSelected);
                        //Get the list of all tests to populate the Test Case listbox
                        ArrayList list = new ArrayList(TestSuite.getTestList(pth));
                        if (tstSelected != "")
                        {
                            for (int i = 0; i < lstCaseSelect.Items.Count; i++)
                            {
                                if (list.Contains(lstCaseSelect.Items[i]))
                                {
                                    lstSelectedTest.Items.Add(lstCaseSelect.Items[i]);
                                }
                            }
                        }
                        break;
                }
                lstCaseSelect.Items.Clear();
                btnDeslctAllAvailable.Enabled = false;
                btnDeselect.Enabled = false;
                if (lstSelectedTest.Items.Count > 0)
                {
                    btnSelect.Enabled = true;
                    btnSlctAllAvailable.Enabled = true;
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
                        //
                        case "DB":
                            break;
                        case "EX":
                        {
                            tstSelected = lstAvailableTest.SelectedItem.ToString();
                            pth = TestSuite.getXlPath(tstSelected);

                            //Get the list of all tests to populate the Test Case listbox
                            avblSuite = TestSuite.getTestList(pth);

                            if (avblSuite != null)
                            {
                                tmpArray = new string[avblSuite.Length];

                                //Format the Available test list
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
                            }

                            break;
                        }
                    }
                }
            }
            #endregion

            #region btnStepExtract_Click
            private void btnStepExtract_Click(object sender, EventArgs e)
            {
                int stpNum;
                int argNum;
                object[] rtnArray;
                string[] argSet;
                string tstID;
                string funcID;
                string funcName;
                string outstring;
                string pth;
                string argID;
                string tstName;
                string strSQL;

                //set the path to open the html file that will contain the step listing
                outstring = "";

                pth = Application.StartupPath + "\\Step List\\stepList.html";

                //open the step file
                TextFileOps.Open(pth);

                if (lstCaseSelect.Items.Count > 0)
                {
                    for (int i = 0; i < lstCaseSelect.Items.Count; i++)
                    {
                        tstName = Convert.ToString(lstCaseSelect.Items[i]);

                        //wite the test name to the step file
                        TextFileOps.Write(pth, "Test Name: " + tstName, 75);

                        //get the test ID from the database
                        strSQL = "SELECT id from test WHERE name = '" + tstName + "'";
                        rtnArray = db_access(strSQL, ref fndExcep);
                        tstID = Convert.ToString(rtnArray[0]);

                        //get the number of steps in the test
                        strSQL = "SELECT COUNT(*) FROM step WHERE test_id = '" + tstID + "'";
                        rtnArray = db_access(strSQL, ref fndExcep);
                        stpNum = Convert.ToInt32(rtnArray[0]);

                        //Write the number if steps to the result file
                        TextFileOps.Write(pth, "There are " + Convert.ToString(stpNum) + " steps in this test<br></br>", 80);

                        for (int x = 0; x < stpNum; x++)
                        {
                            //get the function ID and argument set ID
                            strSQL = "SELECT function_id, argument_set_id FROM step WHERE test_id = '" + tstID + "' AND number = " + (x + 1).ToString();
                            rtnArray = db_access(strSQL, ref fndExcep);
                            funcID = Convert.ToString(rtnArray[0]);
                            argID = Convert.ToString(rtnArray[1]);

                            //get the function name
                            strSQL = "SELECT function_name FROM function WHERE id = '" + funcID + "'";
                            rtnArray = db_access(strSQL, ref fndExcep);
                            funcName = Convert.ToString(rtnArray[0]);

                            //Write a line to the step listing
                            TextFileOps.Write(pth, "Step Number " + Convert.ToString(x + 1), 80);
                            TextFileOps.Write(pth, "Function Name: " + funcName, 80);

                            //get the argument set count to initialize an array to keep the argument set
                            strSQL = "SELECT COUNT(*) FROM argument WHERE argument_set_id = '" + argID + "'";
                            rtnArray = db_access(strSQL, ref fndExcep);
                            argNum = Convert.ToInt32(rtnArray[0]);

                            argSet = new string[argNum];

                            //get the argument set
                            strSQL = "SELECT value FROM argument WHERE argument_set_id = '" + argID + "' ORDER BY seq";
                            rtnArray = db_access(strSQL, ref fndExcep);

                            //populate an array with the step arguments
                            for (int y = 0; y < argNum; y++)
                            {
                                argSet[y] = Convert.ToString(rtnArray[y]);

                                outstring = "Argument " + Convert.ToString(y + 1) + ": " + argSet[y];
                                TextFileOps.Write(pth, outstring, 85);
                            }

                            //write a line break
                            TextFileOps.Write(pth, "<br></br>", 85);
                        }
                    }

                    //show the step listing file
                    Process.Start(pth);
                }
                else
                {
                    MessageBox.Show("There needs to be at least one test in the Selected Test Cases listbox. Please select a test", "Step Extractor Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            /*private void rdoDB_CheckedChanged(object sender, EventArgs e)
            {
                if (rdoDB.Checked == true)
                {
                    datSource = "DB";
                    cmbProductBox.Enabled = true;
                    lstAvailableTest.Items.Clear();
                    lstSelectedTest.Items.Clear();
                    lstCaseSelect.Items.Clear();
                    lstAvailableTest.Hide();
                    label3.Location = label1.Location;
                    label1.Hide();
                    lstSelectedTest.Location = lstAvailableTest.Location;
                    lstSelectedTest.Size = new Size((btnSelect.Location.X - 12) - lstSelectedTest.Location.X, lstSelectedTest.Height);
                    lstTestSuite.Items.Clear();
                    btnStepExtract.Enabled = true;
                    rTest.Enabled = true;
                    lstTestSuite.Items.Add("");
                    lstTestSuite.Items.AddRange(db_access("SELECT name FROM regression_suite ORDER BY name ASC", ref fndExcep));
                }
            }
            #endregion

            #region rdoEX_CheckedChanged
            private void rdoEX_CheckedChanged(object sender, EventArgs e)
            {
                Excel.Application xlApp;
                Excel.Workbook wrkBook;
                Excel.Workbooks tmp;
                Excel.Worksheet wrkSheet;
                Process[] initXLProc;
                int vfySel;
                string rawString;
                string product;
                string xlPath;

                xlPath = Application.StartupPath + "\\Data Sheets\\Database Upload.xlsm";

                if (rdoEX.Checked == true)
                {
                    //Get the Process Id of any Excel processes that are currently running 
                    initXLProc = Process.GetProcessesByName("EXCEL");

                    //Setting the Excel objects
                    xlApp = new Excel.Application();
                    tmp = xlApp.Workbooks;
                    wrkBook = tmp.Open(xlPath);
                    wrkSheet = wrkBook.Sheets["Master"];

                    //get the product name from the first test in the Database Upload sheet
                    rawString = wrkSheet.Cells[2, 3].Value;

                    //trim any spaces from the front or back of the rawString
                    product = rawString.Trim();

                    //kill the Excel process just opened
                    TestSuite.killXLProc(initXLProc);

                    //vewrify that what is in the spreadsheet is an actual selection in the product box 
                    vfySel = verifySelection(product);

                    if (vfySel == 1)
                    {
                        cmbProductBox.Text = product;

                        cmbProductBox.Enabled = false;
                        lstAvailableTest.Items.Clear();
                        lstSelectedTest.Items.Clear();
                        lstCaseSelect.Items.Clear();
                        lstAvailableTest.Hide();
                        lstAvailableTest.Show();
                        label1.Show();
                        label3.Location = new Point(label1.Location.X + 199, label1.Location.Y);
                        lstSelectedTest.Location = new Point(lstAvailableTest.Location.X + 199, lstAvailableTest.Location.Y);
                        lstSelectedTest.Size = new Size((btnSelect.Location.X - 12) - lstSelectedTest.Location.X, lstSelectedTest.Height);

                        string pth;
                        string appPath;
                        string[] lstSuite;

                        datSource = "EX";

                        appPath = Application.StartupPath;

                        pth = appPath + "\\Suite List.txt";
                        lstSuite = File.ReadAllLines(pth);
                        lstTestSuite.Items.Clear();
                        lstTestSuite.Items.AddRange(lstSuite);
                        btnStepExtract.Enabled = false;
                        rTest.Enabled = false;
                    }
                    else
                    {
                        MessageBox.Show("The product entry in line 2 of the database upload sheet either does not match any of the " + 
                        "selections in the product box or is blank.\r\n\r\nThe entry from the sheet is: " + product + "\r\n\r\n" + 
                        "Please check the entry in the sheet and correct it",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);


                        propArray = cntrlArray();

                        //the current state of the form. This will be used a s a reference to reset
                        okControls = (ControlCollection)this.Controls;

                        //reset the form
                        resetForm(okControls, propArray);

                        rtnIdx = 1;

                        rdoEX.Checked = false;
                        rdoDB.Checked = true;
                    }
                }
            }*/
            #endregion

            #region rTest_Click
            private void rTest_Click(object sender, EventArgs e)
            {
                /*reg = "";
                RegQ pop = new RegQ();
                pop.Show();*/

                //clear the test suite list box and populate with regression tests
                lstTestSuite.Items.Clear();
                lstTestSuite.Items.Add("Regression Suite");
                //lstTestSuite.Text = "Regression Suite";
                lstSelectedTest.Items.Clear();

                lstCaseSelect.Items.Clear();
                lstSelectedTest.Items.AddRange(db_access("SELECT name FROM test WHERE isRegression = 'Y'", ref fndExcep));

                //other buttons and fields states in here
                btnExtract.Enabled = true;

                //btnDeselect.Enabled = true;
                btnDeslctAllAvailable.Enabled = true;
                btnSlctAllAvailable.Enabled = false;
                btnPlay.Enabled = true;
            }
            #endregion

            #region cmbProductBox_Changed
            private void cmbProductBox_Changed(object sender, EventArgs e)
            {

            }
            #endregion

        #endregion

        #region Private Functions

            #region createArray
        private string[,] createArray(string iter, string tstName, string[,] inArray)
            {
                string[,] outArray;

                if (inArray == null)
                {
                    outArray = new string[1, 2];
                    outArray[0, 0] = iter;
                    outArray[0, 1] = tstName;
                }

                else
                {
                    outArray = new string[inArray.GetLength(0) + 1, 2];

                    for (int x = 0; x < outArray.GetLength(0) - 1; x++)
                    {
                        outArray[x, 0] = inArray[x, 0];
                        outArray[x, 1] = inArray[x, 1];
                    }

                    outArray[outArray.GetLength(0) - 1, 0] = iter;
                    outArray[outArray.GetLength(0) - 1, 1] = tstName;
                }
                return outArray;
            }
            #endregion

            #region popExcelResults
            private void popExcelResults(string xlPath, string[,] tstPassed, string[,] tstFailed, int totTests, int numPass, int numFail, DateTime dtRun, string totTime,
                string browser, string env, string dataSource, string xlResName, string hmtlResultPth)
            {
                bool filePres;
                string[] items;
                string cmbName;
                string resPath;
                int totSeconds;
                int maxRows;
                decimal avgTests;
                decimal passPct;

                Excel.Application xlApp;
                Excel.Workbook wrkBook;
                Excel.Workbook xlBook;
                Excel.Workbooks tmp;
                Excel.Worksheet wrkSheet;
                Excel.DropDowns xlDropDowns;
                Excel.DropDown xlDropDown;
                Excel.Range range;
                Process[] initXLProc;

                //Get the Process Id of any Excel processes that are currently running 
                initXLProc = Process.GetProcessesByName("EXCEL");

                //start a new instance of excel
                xlApp = new Excel.Application();

                try
                {
                    //instantiate variables
                    range = null;

                    //set the items list array
                    items = new string[4];
                    items[0] = "";
                    items[1] = "Provisional Pass";
                    items[2] = "Rerun Pass";
                    items[3] = "Failed Test";

                    //suppress any dialog boxes on save
                    xlApp.DisplayAlerts = false;

                    //set maxRows (larger value of failArray.GetLength(0) and passArray.GetLength(0))
                    if (passArray != null && failArray != null)
                    {
                        if (passArray.GetLength(0) >= failArray.GetLength(0))
                            maxRows = passArray.GetLength(0);
                        else
                            maxRows = failArray.GetLength(0);
                    }
                    else if (passArray == null)
                    {
                        maxRows = failArray.GetLength(0);
                    }
                    else if (failArray == null)
                    {
                        maxRows = passArray.GetLength(0);
                    }

                    tmp = xlApp.Workbooks;
                    resPath = Application.StartupPath + "\\Results\\Excel Results";
                    wrkBook = tmp.Open(xlPath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                    //check for duplicates. If present, delete the existing file
                    filePres = File.Exists(resPath + "\\" + xlResName + ".xlsm");

                    if (filePres == true)
                    {
                        File.Delete(resPath + "\\" + xlResName + ".xlsm");
                    }

                    //copy the template to the Excel Results directory
                    wrkBook.SaveCopyAs(resPath + "\\" + xlResName + ".xlsm");

                    //close the template
                    wrkBook.Close();

                    //open the copied workbook
                    xlBook = tmp.Open(resPath + "\\" + xlResName + ".xlsm", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                    ///These values are the header values in the results spreadsheet______________________________________________________
                    //set the wrksheet variable in the new spreadsheet
                    wrkSheet = xlBook.Sheets[1];

                    //Populate the list items in the dropdown 
                    xlDropDowns = ((Excel.DropDowns)(wrkSheet.DropDowns(Type.Missing)));

                    //Enter the Date/Time of Run
                    wrkSheet.Cells[2, 8].Value = dtRun.Month.ToString() + "/" + dtRun.Day.ToString() + "/" + dtRun.Year.ToString() +
                        " " + dtRun.Hour + ":" + dtRun.Minute.ToString();

                    //Enter the browser type
                    wrkSheet.Cells[2, 13].Value = browser;

                    //enter the environment run
                    wrkSheet.Cells[2, 16].Value = browser;

                    //enter the test run time
                    wrkSheet.Cells[3, 8].Value = totTime;

                    //enter the data source
                    wrkSheet.Cells[3, 13].Value = dataSource;

                    //enter the local results path
                    wrkSheet.Cells[4, 10].Value = hmtlResultPth;
                    //____________________________________________________________________________________________________________________


                    ///Run Stats__________________________________________________________________________________________________________
                    //put the total number of tests ran into the spreadsheet
                    wrkSheet.Cells[4, 2].Value = totTests.ToString();

                    // get the total seconds ran from the totTime string
                    totSeconds = getTotSeconds(totTime);

                    //get the average execution time for each test
                    avgTests = Convert.ToDecimal(totSeconds) / Convert.ToDecimal(totTests);
                    avgTests = Math.Round(avgTests, 2, MidpointRounding.AwayFromZero);

                    //put the avgTest time into the spreadsheet
                    wrkSheet.Cells[4, 5].Value = avgTests.ToString();

                    //put the initial number of passed tests into the spreadsheet
                    wrkSheet.Cells[6, 2].Value = numPass.ToString();
                    wrkSheet.Cells[6, 2].Font.Color = 0x009900;

                    //put the initial raw failed number from the automation into the spreadsheet
                    wrkSheet.Cells[5, 5].Value = numFail.ToString();

                    //put '0' in the expected failures fields
                    wrkSheet.Cells[5, 2].Value = "0";

                    //put the initial total failures into the spreadsheet
                    wrkSheet.Cells[6, 5].Value = numFail.ToString();
                    wrkSheet.Cells[6, 5].Font.Color = 0x0000FF;

                    //put the initial pass percentage in
                    passPct = (Convert.ToDecimal(numPass) / Convert.ToDecimal(totTests)) * 100;
                    passPct = Math.Round(passPct, 2, MidpointRounding.AwayFromZero);
                    wrkSheet.Cells[5, 6].Value = passPct + "%";

                    //place the initial pass and fail percentages into the (hidden from the user) chart cells
                    wrkSheet.Cells[3, 19].Value = passPct.ToString();
                    wrkSheet.Cells[3, 20].Value = Convert.ToDecimal(100.00 - Convert.ToDouble(passPct)).ToString();
                    //wrkSheet.Cells[4, 19].Value = numFail.ToString;

                    //____________________________________________________________________________________________________________________

                    //Passed Tests________________________________________________________________________________________________________
                    //Merge the A-E Columns for the passed tests, set the font color, and put in thew value of the passed test
                    if (passArray != null)
                    {
                        for (int x = 0; x < passArray.GetLength(0); x++)
                        {
                            wrkSheet.Range[wrkSheet.Cells[x + 9, 1], wrkSheet.Cells[x + 9, 6]].Merge();
                            wrkSheet.Cells[x + 9, 1].Font.Color = 0x009900;
                            wrkSheet.Cells[x + 9, 1].Value = passArray[x, 1];
                            //wrkSheet.Range[wrkSheet.Cells[x + 9, 1], wrkSheet.Cells[x + 9, 6]].Locked = true;
                        }
                    }
                    //____________________________________________________________________________________________________________________

                    //Failed Tests________________________________________________________________________________________________________
                    //Merge the G-K and M-Q Columns for the failed tests, set the font color, and put in thew value of the passed test
                    if (failArray != null)
                    {
                        for (int x = 0; x < failArray.GetLength(0); x++)
                        {
                            //merge cells to store the failed test
                            wrkSheet.Range[wrkSheet.Cells[x + 9, 7], wrkSheet.Cells[x + 9, 11]].Merge();

                            //change the font color to red
                            wrkSheet.Cells[x + 9, 7].Font.Color = 0x0000FF;

                            //insert the item
                            wrkSheet.Cells[x + 9, 7].Value = failArray[x, 1];

                            //set range for insert cell
                            range = wrkSheet.get_Range("L" + (x + 9).ToString() + ":L" + (x + 9).ToString());

                            //insert the dropdown into the cell
                            xlDropDown = xlDropDowns.Add((double)range.Left, (double)range.Top, (double)range.Width, (double)range.Height, true);

                            //set the name of the new dropdown
                            xlDropDown.Name = "expFail" + (x + 1).ToString();

                            //range.Dependents.

                            //set change event to macro
                            xlDropDown.OnAction = xlDropDown.Name + "_Change";

                            //assign dropdown name to cmbName 
                            cmbName = xlDropDown.Name;

                            //call function to write change macro for this box 
                            cmbWriteMacro(cmbName, xlApp, xlBook, wrkSheet);

                            //set Selected proerty to blank leading index
                            xlDropDown.Select(1);

                            //link the combo box to specific cell in excel
                            xlDropDown.LinkedCell = range.Address;

                            //add items to the dropdown
                            for (int i = 0; i < items.Length; i++)
                            {
                                xlDropDown.AddItem(items[i], Type.Missing);
                            }

                            //merge cells to store the failed test comments
                            wrkSheet.Range[wrkSheet.Cells[x + 9, 13], wrkSheet.Cells[x + 9, 17]].Merge();
                        }
                    }
                    //____________________________________________________________________________________________________________________

                    //save the workbook in its current locaton
                    xlBook.Save();

                    //bring the workbook to the screen
                    xlApp.Visible = true;

                    //kill the Excel process opened ayt the beginning of the function
                    //TestSuite.killXLProc(initXLProc);

                }
                catch (Exception e)
                {
                    //TestSuite.killXLProc(initXLProc);
                    string thsString = e.Message;

                    //kill the Excel process opened ayt the beginning of the function
                    TestSuite.killXLProc(initXLProc);

                    //Show the error
                    MessageBox.Show(e.Message);
                }
            }
            #endregion

            #region getTotSeconds
            private int getTotSeconds(string inString)
            {
                int endloop;
                int count;
                int outSecs;
                int strLen;
                int hours;
                int minutes;
                int seconds;
                string tempstring;
                string thsChar;

                endloop = 0;
                seconds = 0;
                minutes = 0;
                hours = 0;
                count = 0;
                tempstring = "";

                //get the instring length
                strLen = inString.Length;

                for (int x = 0; x < strLen; x++)
                {
                    thsChar = inString.Substring(x, 1);

                    //look for the colon delimiter that seperates each different time set 
                    if (thsChar == ":")
                    {
                        //increment count each time a colon is found 
                        count++;

                        //get integer representations of the time string sent in
                        switch (count)
                        {
                            case 1:         //hours
                                hours = Convert.ToInt32(tempstring);
                                break;
                            case 2:         //minutes and seconds
                                minutes = Convert.ToInt32(tempstring);
                                seconds = Convert.ToInt32(inString.Substring(x + 1, 2));
                                endloop = 1;
                                break;
                        }

                        //clear tempstring once a time set has been found and converted
                        tempstring = "";

                        //break the for loop once the seconds and minutes have been extracted
                        if (endloop == 1)
                            break;
                    }
                    else
                    {
                        //append a char to the current tempstring
                        tempstring = tempstring + thsChar;
                    }
                }

                //get the total number of seconds
                outSecs = 60 * ((hours * 60) + (minutes)) + seconds;
                return outSecs;
            }
            #endregion

            #region cmbWriteMacro
            private void cmbWriteMacro(string cmbName, Excel.Application xlApp, Excel.Workbook wrkBook, Excel.Worksheet wrkSheet)
            {
                StringBuilder sb;
                VBA.VBComponent xlModule;
                VBA.VBProject prj;
                string modName;
                int modExists;

                prj = wrkBook.VBProject;
                modExists = 0;

                sb = new StringBuilder();

                //build string with module code 
                sb.Append("Sub " + cmbName + "_Change()" + "\n");
                sb.Append("\t" + "Call lstBox_Update(\"" + cmbName + "\")" + "\n");
                sb.Append("End Sub");

                foreach (VBA.VBComponent comp in prj.VBComponents)
                {
                    modName = comp.Name;

                    if (modName == "Module2")
                    {
                        modExists = 1;
                        break;
                    }
                }

                //check to see if module already exists
                if (modExists != 1)
                {
                    //set an object for the new module to create
                    xlModule = wrkBook.VBProject.VBComponents.Add(VBA.vbext_ComponentType.vbext_ct_StdModule);
                }
                else
                {
                    xlModule = wrkBook.VBProject.VBComponents.Item("Module2");
                }

                //add the cmbbox macro to the spreadsheet
                xlModule.CodeModule.AddFromString(sb.ToString());
            }
            #endregion

            #region verifySelection
            private int verifySelection(string inString)
            {
                int vfyInt;
                int itmCount;
                string[] arrSels;

                vfyInt = 0;

                itmCount = cmbProductBox.Items.Count;
                arrSels = new string[itmCount];

                for (int x = 0; x < itmCount; x++)
                {
                    arrSels[x] = cmbProductBox.Items[x].ToString();
                }

                for (int x = 0; x < itmCount; x++)
                {
                    if (arrSels[x] == inString)
                    {
                        vfyInt = 1;
                        break;
                    }
                }
                return vfyInt;
            }
            #endregion

            #region cntrlArray
            private string[,] cntrlArray()
            {
                int idx;
                int arrLen;
                string[,] inArray;

                //set a collection of all controls in the state before any resets. This will be the default contriol collection
                //in case the form needs to be reset after pressing the OK button
                idx = 0;
                defControls = (ControlCollection)this.Controls;
                arrLen = defControls.Count;
                inArray = new string[arrLen, 2];

                //set an array to each control name and its current enabled state
                foreach (Control c in this.Controls)
                {

                    if (c.Name == "slctDataSource")
                    {
                        inArray[idx, 0] = c.Name;
                        inArray[idx, 1] = "True";
                        idx++;
                    }
                    else
                    {
                        inArray[idx, 0] = c.Name;
                        inArray[idx, 1] = c.Enabled.ToString();
                        idx++;
                    }
                }

                return inArray;
            }
            #endregion

            #region resetForm
            private void resetForm(ControlCollection okControls, string[,] inArray)
            {
                //reset the form using the propArray gathered before all the controls were disabled when the OK
                //button was pressed
                foreach (Control c in okControls)
                {
                    for (int x = 0; x < inArray.GetLength(0); x++)
                    {
                        if (inArray[x, 0] == c.Name)
                        {
                            c.Enabled = Convert.ToBoolean(inArray[x, 1]);
                        }
                    }
                }

                lstCaseSelect.Items.Clear();
            }
            #endregion

            #region runFunction
            private void runFunction(string steName, string tstName, int typBrowser, string datsource, string pth, string baseURL, ref string[,] tstResult, ref string profilePath, out int fndExcep, out int tstFail)
            {
                tstFail = 0;
                fndExcep = 0;       //initialize fndExcep to 0

                //call the tstbrowser function to launch the correct browser based on the selection in the browser group
                VerifyScreens.tstBrowser(pth, datSource, steName, tstName, typBrowser, baseURL, ref tstResult, ref profilePath, out fndExcep, out tstFail);
            }
            #endregion

            #region browserSelect
            private int browserSelect(Control.ControlCollection btnRadio)
            {
                int count;          //number of browser select contriols on the GUI form
                int numControl;      //the index of the selected radio button in the btnRadio Control Collection

                numControl = 0;
                count = btnRadio.Count;

                //get value of count from the btnRadio collection
                for (int x = 0; x < count; x++)
                {
                    RadioButton btnThis = (RadioButton)btnRadio[x];
                    if (btnThis.Checked == true)
                    {
                        numControl = x;
                        break;
                    }
                }
                return numControl;
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

            #region popListBox
            private string popListBox(string inSuite)
            {
                string retString = "";

                switch (inSuite)
                {
                    case "Verify User Screens":
                        retString = "C:\\Users\\michael.kiewicz\\Documents\\Visual Studio 2010\\Projects\\WindowsFormsApplication1\\Verify User Screens.txt";
                        break;
                }

                return retString;
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
                    case "Strive Regressions":
                        pth = Application.StartupPath + "\\Strive Regressions.txt";
                        break;
                    case "Database Upload":
                        pth = Application.StartupPath + "\\Database Upload.txt";
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

            #region thinArray
            private object[] thinArray(object[,] inArray)
            {
                int cnt;
                int xItmCount;
                int yItmCount;
                object[] outArray;

                cnt = 0;

                //get the number of results in the second array dimension
                xItmCount = inArray.GetLength(0);
                yItmCount = inArray.GetLength(1);

                //initialize a new one dimensional array
                outArray = new object[xItmCount * yItmCount];

                //populate the dataSuite array
                for (int x = 0; x < xItmCount; x++)
                {
                    for (int y = 0; y < yItmCount; y++)
                    {
                        outArray[cnt] = inArray[x, y];
                        cnt++;
                    }
                }

                //return the new array 
                return outArray;
            }
            #endregion

            #region db_access
            private object[] db_access(string strSQL, ref int fndExcep)
            {
                ADODB.Connection objCon;
                ADODB.Recordset objRec;
                object[,] dataRows;
                object[] dataSuite;
                string strCon;
                string tmpString;

                dataSuite = null;
                objCon = new ADODB.Connection();
                objRec = new ADODB.Recordset();
                try
                {
                    //establish the connection string and open the database connection  
                    strCon = "driver={MySQL ODBC 5.1 Driver};server=107.22.232.228;uid=qa_people;pwd=thehandcontrols;" +
                        "database=functional_test_data;option=3";

                    objCon.Open(strCon);

                    //execute the SQL and return the recrodset of results
                    objRec = objCon.Execute(strSQL, out missing, 0);

                    //populate a two dinmensional object array with the results
                    dataRows = objRec.GetRows();

                    //get a one dimensional array that can be placed into the Test Suite dropdown
                    dataSuite = thinArray(dataRows);

                    //close the recordset
                    objRec.Close();

                    //close the database connection
                    objCon.Close();
                }
                catch (Exception e)
                {
                    tmpString = e.Message;

                    //set the variable to ternibate the script
                    fndExcep = -1;

                }

                return dataSuite;
            }
            #endregion

            #region populateSelectBoxes
            private void populateSelectBoxes(string[] array, out string[] tmpArray, out string[] slctArray,
                out string[] caseArray, out string[] fnlArray)
            {
                int cnt;
                int fndItem;
                int fnlCount;
                int slctCnt;
                int caseCnt;

                fndItem = 0;
                cnt = 0;
                fnlCount = 0;
                slctCnt = 0;
                caseCnt = 0;

                fnlCount = lstSelectedTest.Items.Count + lstCaseSelect.SelectedItems.Count;

                tmpArray = new string[lstSelectedTest.Items.Count];
                slctArray = new string[lstCaseSelect.SelectedItems.Count];
                caseArray = new string[lstCaseSelect.Items.Count - lstCaseSelect.SelectedItems.Count];
                fnlArray = new string[fnlCount];


                for (int i = 0; i < lstCaseSelect.Items.Count; i++)
                {
                    if (lstCaseSelect.SelectedItems.Contains(lstCaseSelect.Items[i]))
                    {
                        slctArray[slctCnt] = lstCaseSelect.Items[i].ToString();
                        slctCnt++;
                    }
                    else
                    {
                        caseArray[caseCnt] = lstCaseSelect.Items[i].ToString();
                        caseCnt++;
                    }
                }

                for (int i = 0; i < lstSelectedTest.Items.Count; i++)
                {
                    tmpArray[i] = lstSelectedTest.Items[i].ToString();
                }

                //loop through and check the items to be deselected, and add the ones contained in the currently selected suite to the selectedcase list box   
                for (int x = 0; x <= array.Length; x++)
                {
                    for (int y = 0; y < tmpArray.Length; y++)
                    {
                        if (tmpArray[y] == array[x].ToString())
                        {
                            fnlArray[cnt] = tmpArray[y];
                            cnt++;
                            fndItem = 1;
                            break;
                        }
                    }

                    if (fndItem != 1)
                    {
                        for (int y = 0; y < slctArray.Length; y++)
                        {
                            if (slctArray[y] == array[x].ToString())
                            {
                                fnlArray[cnt] = slctArray[y];
                                cnt++;
                                fndItem = 1;
                                break;
                            }
                        }
                    }

                    if (fndItem == 1)
                    {
                        fndItem = 0;
                    }

                    if (cnt == fnlCount)
                    {
                        break;
                    }
                }

            }
            #endregion

        #endregion
    }
}
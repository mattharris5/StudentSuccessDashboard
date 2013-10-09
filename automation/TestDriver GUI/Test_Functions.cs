using Excel = Microsoft.Office.Interop.Excel;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Selenium;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Verify_User_Screens_Firefox;

#region echoAutomatedSuite namespce
namespace echoAutomatedSuite
{
    #region tstObject Class
    public class tstObject
    {
        public static string faillst;
        public int brwsrType;
        private IWebDriver driver;
        private string clkLink = "<br />";
        public string tmpString;
        #region Public Members

        #region Constructor
        public tstObject(int typNum)
        {

            brwsrType = typNum;

            switch (typNum)
            {
                //create a Chrome object
                case 1:
                {
                    var options = new ChromeOptions();

                    //set the startup options to start maximzed
                    options.AddArguments("start-maximized");

                    //start Chrome maximized
                    driver = new ChromeDriver(@Application.StartupPath, options);

                    //Wait 10 seconds for an item to appear
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(4));
                    break;
                }

                //create an IE object
                case 2:
                {
                    //var options = new InternetExplorerOptions();

                    //set the startup options to start maximzed
                    //options.ToCapabilities();

                    driver = new InternetExplorerDriver(@Application.StartupPath);

                    //maximize window
                    driver.Manage().Window.Maximize();

                    //Wait 4 seconds for an item to appear
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(4));

                    break;
                }
                default:
                {
                    FirefoxProfile profile = new FirefoxProfile();
                    profile.SetPreference("webdriver.firefox.profile", "cbufsusm.default");
                    profile.AcceptUntrustedCertificates = true;

                    driver = new FirefoxDriver(profile); //profile

                    //maximize window
                    driver.Manage().Window.Maximize();

                    //Wait 4 seconds for an item to appear
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(4));
                    break;
                }
            }
        }
        #endregion

        #region Close
        public void Close()
        {
            driver.Close();
        }
        #endregion

        #region Quit
        public void Quit()
        {
            driver.Quit();
        }
        #endregion

        #region Login
        public void Login(tstObject tstObj, string[] inArray, string baseURL, string stpNum, int browser, string pth, out int fndExcep, ref string[,] tstresult)
        {
            bool objPres;
            int isPres;
            string lnkLogout;
            string username;
            string password;
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            WebDriverWait wait;

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            //initialize variables
            fndExcep = 0;
            username = "";
            password = "";

            try
            {
                //Navite to the baseURL
                driver.Navigate().GoToUrl(baseURL + "/");

                if (browser == 2)
                {
                    lnkLogout = "/html/body/table/tbody/tr/td/table/tbody/tr/td/div/div[3]/table/tbody/tr/td[5]/div";
                    objPres = tstObj.IsElementPresent(By.XPath(lnkLogout));

                    if (objPres == true)
                    {
                        driver.FindElement(By.XPath(lnkLogout)).Click();
                        wait.Until(drv => driver.FindElement(By.Id("edit-name")).Displayed);
                    }
                }

                //assign variables from the incoming inArray from the database
                for (int x = 0; x < inArray.Length; x++)
                {
                    if (inArray[x] != "")
                    {
                        switch (x)
                        {
                            case 0:
                                username = inArray[x];

                                if (username == "teacher01@demo.newtechnetwork.org")
                                    objPres = true;
                                break;
                            case 1:
                                password = inArray[x];
                                break;
                        }
                    }
                }

                objPres = tstObj.IsElementPresent(By.XPath("id('edit-name')"));

                Thread.Sleep(250);
                
                //Clear the Username field
                driver.FindElement(By.XPath("id('edit-name')")).Clear();
                //Populate the Username field with the username parameter sent from the data sheet
                driver.FindElement(By.XPath("id('edit-name')")).SendKeys(username);

                Thread.Sleep(250);

                isPres = driver.FindElements(By.XPath("id('text')")).Count;

                if (isPres > 0)
                {
                    //click the password field. This will change the input type
                    driver.FindElement(By.XPath("id('text')")).Click();

                    js.ExecuteScript("swapInput()");
                }

                //Clear the password field
                driver.FindElement(By.XPath("id('edit-pass')")).Clear();

                //Populate the Password field with the password parameter sent from the data sheet
                driver.FindElement(By.XPath("id('edit-pass')")).SendKeys(password);
                

                //Click the Sign On button 
               // driver.FindElement(By.XPath("id('edit-pass')")).SendKeys("{TAB}");
                objPres = tstObj.IsElementPresent(By.Id("edit-submit"));
                driver.FindElement(By.Id("edit-submit")).Click();

                //Verify login was successful
                objPres = driver.FindElement(By.LinkText("Home")).Enabled;

                //log result to the result file
                tmpString = "Logging into Echo as " + username + ": (" + stpNum + ")";
                tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
            }
            catch (Exception e)
            {
                //Record failed result
                tmpString = e.Message;
                tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                //Record exception and begin exit process
                fndExcep = -1;

            }
        }
        #endregion

        #region Logout
        //Click the Log Out link
        public void Logout(string pth, bool objPres, string stpNum, ref string[,] tstresult)
        {
            int fndLink;
            DateTime startTime;
            DateTime currTime;
            TimeSpan waitTime;
            fndLink = 0;
            
            //get the time that control entered this function as a baseline
            startTime = DateTime.Now;

            do
            {
                //Check that the logout link is present on the screen
                objPres = driver.FindElement(By.XPath("/html/body/table/tbody/tr/td/table/tbody/tr/td/div/div[3]/table/tbody/tr/td[5]/div/a")).Enabled;

                //send control based on whether or not the logout link is present
                if (objPres == true)            //if the element is present
                {
                    //click the logout link
                    driver.FindElement(By.XPath("id('UserLogin')/table/tbody/tr/td[5]/div/a")).Click();

                    //set fndLink to 1 to break the do..while loop
                    fndLink = 1;
                }
                else                            //if the element is not present
                {
                    //set the current time
                    currTime = DateTime.Now;

                    //subtract the start time from the curent timer to get the amount of time waited
                    waitTime = currTime - startTime;

                    //check to see if the wait time is longer than 5 seconds. If not, do nothing
                    if (Convert.ToInt32(waitTime) >= 5000)
                    {
                        //set fndLink to -1 to denote that an element was not found and pop an error
                        fndLink = -1;
                    }

                }
            }
            while (fndLink < 0 && fndLink > 0);

            //add string to the test results list
            tmpString = "Logging out of Echo.........." + " (" + stpNum + ")";
            tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
        }
        #endregion

        #region addOutcome
        public void addOutcome(tstObject tstObj, bool objPres, string[] inArray, string stpNum, string pth, ref string[,] tstresult, out int fndExcep, out int tstFail)
        {
            int dataLen;
            int numRows;
            int dataCount;
            int fldNum;
            string[,] outcomeData;
            string outcome;
            string weight;
            string clkString;
            ReadOnlyCollection<IWebElement> selectData;

            dataLen = 0;
            fldNum = 0;
            dataCount = 0;
            tstFail = 0;
            fndExcep = 0;
            outcome = "";
            weight = "";

            //initialize variables
            string btnXpath1 = "id('ocLastRow')/td[1]/a";
            string btnXpath2 = "id('outcome_" + fldNum.ToString() + "')/td[1]/span[1]/div";
            string txtField = "id('txt_" + fldNum.ToString() + "')";

            for (int cnt = 0; cnt < inArray.Length; cnt++)
            {
                if (inArray[cnt] == "" || cnt == inArray.Length - 1)
                {
                    if (cnt == inArray.Length - 1)
                    {
                        cnt++;
                    }
                    dataLen = cnt / 2;
                    break;
                }
            }

            outcomeData = new string[dataLen, 2];

            for (int x = 0; x < inArray.Length; x++)
            {
                if (inArray[x] == "")
                    break;

                if (x % 2 == 0)
                    outcomeData[dataCount, 0] = inArray[x];
                else
                {
                    outcomeData[dataCount, 1] = inArray[x];
                    dataCount++;
                }
            }

            dataCount = 0;

            try
            {
                //check if object exists
                objPres = driver.FindElement(By.XPath(btnXpath1)).Enabled;

                //click the Add Outcome button
                driver.FindElement(By.XPath(btnXpath1)).Click();

                //add string to the test results list
                tmpString = "Clicking the Add Outcome button......"; ;
                tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);


                for (int x = 0; x < outcomeData.GetLength(0); x++)
                {
                    driver.FindElement(By.XPath(btnXpath2 + "/span")).Click();

                    //get the number of rows in the Select Outcome dropdown
                    numRows = driver.FindElements(By.XPath(btnXpath2 + "/ul/li")).Count;

                    selectData = driver.FindElements(By.XPath(btnXpath2 + "/ul/li"));

                    outcome = outcomeData[dataCount, 0];
                    weight = outcomeData[dataCount, 1];

                    for (int y = 0; y < numRows; y++)
                    {
                        if (y + 1 == 1)
                        {
                            clkString = btnXpath2 + "/ul/li/a";
                        }
                        else
                        {
                            clkString = btnXpath2 + "/ul/li[" + (y + 1).ToString() + "]/a";
                        }

                        if (outcome == selectData[y].Text)
                        {
                            //add string to the test results list
                            tmpString = "Found the " + outcome + " list item......";
                            tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //add string to the test results list
                            tmpString = "Clicking the <b>" + outcome + "</b> list item......";
                            tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                            
                            driver.FindElement(By.XPath(clkString)).Click();

                            //add string to the test results list
                            tmpString = "Putting the weight '<b>" + weight + "</b>' into the weight text field......";
                            tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            driver.FindElement(By.XPath(txtField)).SendKeys(weight);

                            dataCount++;
                            fldNum++;

                            btnXpath2 = "id('outcome_" + fldNum.ToString() + "')/td[1]/span[1]/div";
                            txtField = "id('txt_" + fldNum.ToString() + "')";

                            if ((x + 1) != outcomeData.GetLength(0))
                            {
                                //add string to the test results list
                                tmpString = "Clicking the Add Outcome button......";
                                tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                driver.FindElement(By.XPath(btnXpath1)).Click();
                            }

                            break;
                        }
                    }
                }

                //TextFileOps.Write(pth, "<br />", clrIndex);
            }
            catch (Exception e)
            {
                objPres = false;

                //add string to the test results list
                tmpString = e.Message;
                tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
            }
        }
        #endregion

        #region chkCheckbox
        public void chkCheckbox(tstObject tstObj, bool objPres, string[] inArray, string stpNum, string pth, ref string[,] tstresult, out int fndExcep)
        {
            IWebElement thsChkBox;
            bool check;
            string chkbxText;
            string chkBoxVal;
            string xpath;
            string chkMod;
            string modID;

            fndExcep = 0;

            //initialize variables
            objPres = true;
            xpath = "";
            chkbxText = "";
            chkBoxVal = "";
            chkMod = "";
            modID = "";

            //assign variables from the incoming inArray from the database
            for (int x = 0; x < inArray.Length; x++)
            {
                if (inArray[x] != "")
                {
                    switch (x)
                    {
                        case 0:
                            xpath = inArray[x];
                            break;
                        case 1:
                            chkbxText = inArray[x];
                            break;
                        case 2:
                            chkBoxVal = inArray[x];
                            break;
                        case 3:
                            chkMod = inArray[x];
                            break;
                        case 4:
                            modID = inArray[x];
                            break;
                    }
                }

            }

            if (stpNum == "Step 93")
                stpNum = "Step 93";

            try
            {
                //change frame if modal fields set in spreadsheet
                if (chkMod == "Y")
                {
                    driver.SwitchTo().DefaultContent();
                    driver.SwitchTo().Frame(modID);
                }
                else
                {
                    Thread.Sleep(1000);                         //sleep for one sec to register the click in 
                }                                               //case of two consecuticve clicks     

                objPres = driver.FindElement(By.XPath(xpath)).Displayed;
                check = driver.FindElement(By.XPath(xpath)).Selected;
                thsChkBox = driver.FindElement(By.XPath(xpath));

                //add string to the test results list
                tmpString = "Searching for the " + chkbxText + " checkbox........(" + stpNum + ")";
                tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                //Recorder.vfyNav(objPres, chkbxText, " checkbox", stpNum, pth);

                switch (chkBoxVal)
                {
                    case "T":
                        if (check != true)
                        {
                            thsChkBox.FindElement(By.XPath(xpath)).Click();
                        }
                        break;
                    case "F":
                        if (check == true)
                        {
                            thsChkBox.Click();
                        }
                        break;
                }

                if (chkMod == "Y")
                {
                    driver.SwitchTo().DefaultContent();
                    Thread.Sleep(750);
                }
            }
            catch (Exception e)
            {
                objPres = false;
                chkbxText = TestSuite.convertString(xpath);

                //add string to the test results list
                tmpString = e.Message;
                tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                fndExcep = -1;
            }

            //add string to the test results list
            tmpString = "Successfully checked " + chkbxText + " checkbox";
            tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
        }
        #endregion

        #region clkButton
        public void clkButton(tstObject tstObj, bool objPres, string[] inArray, string stpNum, string pth, ref string[,] tstresult, out int fndExcep)
        {
            string inString;
            string btnText;
            string chkMod;
            string modFrame;

            //wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            fndExcep = 0;

            //initialize variables
            objPres = true;
            inString = "";
            btnText = "";
            chkMod = "";
            modFrame = "";
            
            //assign variables from the incoming inArray from the database
            for (int x = 0; x < inArray.Length; x++)
            {
                if (inArray[x] != "")
                {
                    switch (x)
                    {
                        case 0:
                            inString = inArray[x];
                            break;
                        case 1:
                            btnText = inArray[x];
                            break;
                        case 2:
                            chkMod = inArray[x];
                            break;
                        case 3:
                            modFrame = inArray[x];
                            break;
                    }
                }

            }

            try
            {

                //sleep for a sec if pressing a Close button
                //this is for the Close dialog . Wait for it to appear
                if (btnText == "Close")
                    Thread.Sleep(1500);

                //log searching for the <btnText Button>
                //Recorder.vfyNav(objPres, btnText, " button", stpNum, pth);
                
                //add string to the test results list
                tmpString = "Searching for the " + btnText + " button........(" + stpNum + ")";
                tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                //change frame if modal fields set in spreadsheet
                if (chkMod == "Y")
                {
                    driver.SwitchTo().DefaultContent();
                    driver.SwitchTo().Frame(modFrame);
                }

                //check to see if object is present on the screen
                objPres = driver.FindElement(By.XPath(inString)).Enabled;

                if (objPres == true)
                    if (chkMod == "Y")
                        Thread.Sleep(1500);

                driver.FindElement(By.XPath(inString)).Click();

                //return the control from the modal to the screen
                if (chkMod == "Y")
                {
                    driver.SwitchTo().DefaultContent();
                    Thread.Sleep(750);
                }

                //add string to the test results list
                tmpString = "Successfully clicked the " + btnText + " button";
                tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
            }
            catch (Exception e)
            {
                //log the object present as false
                objPres = false;

                //add string to the test results list
                tmpString = e.Message;
                tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                //TextFileOps.Write(pth, "<br />", clrIndex);
                fndExcep = -1;
            }
        }
        #endregion

        #region inputText
        public void inputText(tstObject tstObj, bool objPres, string[] inArray, string stpNum, string pth, ref string[,] tstresult, out int fndExcep)
        {
            string fldName;
            string fldID;
            string txtInput;
            string chkMod;
            string modFrame;
            IWebElement thsField;

            //initialize variables
            fldName = "";
            fldID = "";
            txtInput = "";
            chkMod = "";
            modFrame = "";
            fndExcep = 0;
            objPres = true;

            //wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            fndExcep = 0;

            //assign variables from the incoming inArray from the database
            for (int x = 0; x < inArray.Length; x++)
            {
                if (inArray[x] != "")
                {
                    switch (x)
                    {
                        case 0:
                            fldID = inArray[x];
                            break;
                        case 1:
                            fldName = inArray[x];
                            break;
                        case 2:
                            txtInput = inArray[x];
                            break;
                        case 3:
                            chkMod = inArray[x];
                            break;
                        case 4:
                            modFrame = inArray[x];
                            break;
                    }
                }
            }

            try
            {
                if (chkMod == "Y")
                {
                    driver.SwitchTo().DefaultContent();
                    driver.SwitchTo().Frame(modFrame);
                    Thread.Sleep(750);
                }
                if (fldID == "URL")
                {
                    driver.Navigate().GoToUrl(txtInput);
                    //Thread.Sleep((int.Parse(chkMod) * 60) * 1000);
                }
                else
                {

                    objPres = driver.FindElement(By.XPath(fldID)).Enabled;

                    //add string to the test results list
                    tmpString = "Searching for the " + fldName + " text field........(" + stpNum + ")";
                    tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                    thsField = driver.FindElement(By.XPath(fldID));

                    if (inArray[0] != "/html/body/table/tbody/tr/td/table[2]/tbody/tr[2]/td[2]/table/tbody/tr/td/div/form/div/div/div/div/fieldset/div[4]/div/div/div[2]/div/div/div/div/div/div/div/div/div/fieldset/div[2]/div/div/table[2]/tbody/tr/td[5]/input")
                        thsField.Clear();
                    thsField.SendKeys(txtInput);
                }
            }
            catch (Exception e)
            {
                objPres = false;

                //add string to the test results list
                tmpString = e.Message;
                tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                fndExcep = -1;
            }

            //set the frame back to the default content
            if (chkMod == "Y")
            {
                driver.SwitchTo().DefaultContent();
                Thread.Sleep(750);
            }


            //set the correct result list objects based on the visibility of the text item 
            if (objPres == true)
            {   
                //add string to the test results list
                tmpString = "The " + fldName + " table was found......";
                tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                tmpString = "Succesfully input '" + txtInput + "' into the " + fldName + " text field. \r\n";
                tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                //TextFileOps.Write(pth, "Succesfully input grades into the " + fldName + " table. \r\n", 1);
            }
            else if (objPres == false)
            {
                //add string to the test results list
                tmpString = "The " + fldName + " field was not found";
                tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                //add string to the test results list
                tmpString = "Could not input text..........\r";
                tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
            }
        }
        #endregion

        #region insertGrades
        public void insertGrades(tstObject tstObj, bool objPres, string[] inArray, string stpNum, string pth, ref string[,] tstresult, out int fndExcep)
        {
            string tblPath;
            string setPath;
            string arrNumRows;
            string isRandom;
            string thsItem;
            string addString;
            string numCol;
            string tblType;
            int popNumRows;
            int tblRow;


            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            popNumRows = 0;
            addString = "";
            tblPath = inArray[0];
            setPath = "";
            numCol = inArray[1];
            arrNumRows = inArray[2];
            isRandom = inArray[3];
            tblType = inArray[4]; 

            fndExcep = 0;
            
            objPres = driver.FindElement(By.XPath(inArray[0])).Displayed;

            //add string to the test results list
            tmpString = "Searching for the Grades listing........(" + stpNum + ")";
            tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

            try
            {
                //check for a vertical table column to populate
                if (tblType != "Y")
                {
                    //find the nunber of grades to enter in a row
                    for (int cnt = inArray.Length - 1; cnt > 2; cnt--)
                    {
                        if (inArray[cnt] != "")
                        {
                            popNumRows = inArray.Length - (inArray.Length - (cnt - 4));
                            break;
                        }
                    }

                    for (int x = 1; x <= Convert.ToInt32(arrNumRows); x++)
                    {
                        //set the row number
                        if (x > 1)
                        {
                            tblRow = x;
                            addString = "[" + tblRow.ToString() + "]";
                        }

                        for (int y = 1; y <= popNumRows; y++)
                        {
                            //get the currenmt item to populate
                            thsItem = inArray[y + 4];

                            //set the path to populate the grade
                            if (y == 1)
                                setPath = inArray[0] + "/tbody/tr" + addString + "/td[" + (y + (Convert.ToInt32(numCol) - 1)).ToString() + "]/input";
                            else
                                setPath = inArray[0] + "/tbody/tr" + addString + "/td[" + (y + (Convert.ToInt32(numCol) - 1)).ToString() + "]/input";

                            //check for blank data or 'EX' (for excused from an outcome)
                            if (thsItem != "" && thsItem != "EX")
                            {
                                //if random, get the random number
                                if (isRandom == "Y")
                                {
                                    thsItem = TestSuite.genRandomGrade(Convert.ToInt32(thsItem));
                                }
                            }

                            //click the grade field    
                            driver.FindElement(By.XPath(setPath)).Click();

                            //clear the grade field    
                            driver.FindElement(By.XPath(setPath)).Clear();

                            //populate the grade
                            driver.FindElement(By.XPath(setPath)).SendKeys(thsItem);

                            js.ExecuteScript("submission_grade_tid('219615538','31004','10','125557', " + thsItem + ", '0')");

                            //tab out of the field
                            driver.FindElement(By.XPath(setPath)).SendKeys(OpenQA.Selenium.Keys.Tab);

                            Thread.Sleep(500);
                        }
                    }
                }
                else //if it is a vertical submission
                {
                    //get the number of rows to populate
                    popNumRows = driver.FindElements(By.XPath(inArray[0] + "/tbody/tr")).Count;

                    //scroll through the vertical table to populate grades
                    for (int x = 1; x <= popNumRows - 1; x++)
                    {
                        //get the item to populate
                        thsItem = inArray[x + 4];

                        //get the path to populate the grade
                        if (x == 1)
                            setPath = inArray[0] + "/tbody/tr/td/div/input";
                        else
                            setPath = inArray[0] + "/tbody/tr[" + x.ToString() + "]/td/div/input";

                        if (thsItem != "" && thsItem != "EX")
                        {
                            //is this a random calculated grade
                            if (isRandom == "Y")
                            {
                                thsItem = TestSuite.genRandomGrade(Convert.ToInt32(thsItem));
                            }
                        }

                        //click the grade field    
                        driver.FindElement(By.XPath(setPath)).Click();

                        //clear the grade field    
                        driver.FindElement(By.XPath(setPath)).Clear();

                        //populate the grade
                        driver.FindElement(By.XPath(setPath)).SendKeys(thsItem);

                        js.ExecuteScript("submission_grade_tid('219615538','31004','10','125557', " + thsItem + ", '0')");

                        //tab out of the field
                        //driver.FindElement(By.XPath(setPath)).SendKeys(OpenQA.Selenium.Keys.Tab);

                        //Thread.Sleep(1000);
                    }
                }

                //populate the results array with the results data from the function
                if (objPres == true)
                {
                    //add string to the test results list
                    tmpString = "Succesfully input grades. \r\n";
                    tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                    //TextFileOps.Write(pth, "Succesfully input grades into the " + fldName + " field. \r\n", 1);
                }
                else if (objPres == false)
                {
                    //add string to the test results list
                    tmpString = "The grade field was not found";
                    tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                    //TextFileOps.Write(pth, "The " + fldName + " field was not found", 0);
                    //TextFileOps.Write(pth, "Could not input text..........\r", -1);
                }
            }
            catch (Exception e)
            {
                objPres = false;

                //add string to the test results list
                tmpString = e.Message;
                tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                fndExcep = -1;
            }
        }
        #endregion

        #region navLinks
        //Click the link sent from the spreadsheet
        public void navLinks(tstObject tstObj, bool objPres, string[] inArray, string stpNum, string pth, ref string[,] tstresult, out int fndExcep)
        {
            string inString;
            string lnkText;

            //initilize variables
            fndExcep = 0;
            inString = "";
            lnkText = "";

            objPres = driver.FindElement(By.XPath(inArray[0])).Displayed;

            //assign variables from the incoming inArray from the database
            for (int x = 0; x < inArray.Length; x++)
            {
                if (inArray[x] != "")
                {
                    switch (x)
                    {
                        case 0:
                            inString = inArray[x];
                            break;
                        case 1:
                            lnkText = inArray[x];
                            break;
                    }
                }
            }

            //Format the link name
            //add string to the test results list
            tmpString = "Clicking the " + lnkText + " link..... (" + stpNum + ")";
            tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

            try
            {
                //switch statement governing how the app clicks on a link
                switch (inString)
                {
                    case "pf_add_students-wrapper":
                        driver.FindElement(By.Id(inString)).Click();
                        break;
                    default:
                        driver.FindElement(By.XPath(inString)).Click();
                        break;
                }
                
                tmpString = "Successfully clicked the " + lnkText + " link";
                tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
            }
            catch (Exception e)
            {
                //add string to the test results list
                tmpString = e.Message;
                tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                fndExcep = -1;
            }
        }
        #endregion

        #region selDropdown
        public void selDropDown(tstObject tstObj, bool objPres, string drpName, string lstTag, string suffix, string clk, string lstItem, string chkMod,
            string modID, string resString, string stpNum, string pth, ref string[,] tstresult, out int fndExcep)
        {
            //drpName - name of the droddown
            //lstTag - the tags that contain the actual list of items in the dropdown 
            //lstItem - the dropdown item being processed
            //chkMod - is dropodown in a modal window?
            //modID - modal frame id
            //pth - passed in - path to the result file
            //fndExcep - escape and  show result file if an exception is found

            ReadOnlyCollection<IWebElement> lnkList;
            int count;              //number of lst items in the dropdown
            int fndItem;
            string fndList;         //final xpath value 
            string sndKey;

            //set initial value for fndExcep
            fndExcep = 0;
            fndItem = 0;
            sndKey = "A";
            lnkList = null;

            try
            {
                //determine if need to aswitcvh to modal window -- chkMod = 'Y'
                if (chkMod == "Y")
                {
                    //switch to modal frame
                    driver.SwitchTo().DefaultContent();
                    driver.SwitchTo().Frame(modID);
                    Thread.Sleep(750);
                }

                //look to see if the dropdown object exists
                objPres = driver.FindElement(By.XPath(drpName)).Enabled;

                if (clk == "Y" || drpName == "/html/body/table/tbody/tr/td/table[2]/tbody/tr[2]/td[2]/table/tbody/tr" +
                    "/td/div/form/div/div/table/tbody/tr[2]/td[2]/div/div/div/div/div/table/tbody/tr[4]/td/div/div/div/span")
                {
                    driver.FindElement(By.XPath(drpName)).Click();

                    //Record result to result file
                    tmpString = "Searching for the " + resString + " dropdown........(" + stpNum + ")";
                    tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
            
                    //get count of items in the dropdown
                    count = driver.FindElements(By.XPath(lstTag)).Count;

                    //load all of the dropdown elements into a ReadOnly Collection of IWebElements
                    lnkList = driver.FindElements(By.XPath(lstTag));
                }
                else
                {
                    //Record result to result file
                    tmpString = "Searching for the " + resString + " dropdown........(" + stpNum + ")";
                    tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
            
                    //get count of items in the dropdown
                    count = driver.FindElements(By.XPath(lstTag)).Count;

                    //load all of the dropdown elements into a ReadOnly Collection of IWebElements
                    lnkList = driver.FindElements(By.XPath(lstTag));
                }

                //loop through the items in the list to find the item from the sheet to click
                for (int x = 0; x < count; x++)
                {

                    //append brackets to last tag if x > 0
                    //take lstTag var and pass as is if x = 0..no need for any brackets
                    if (x == 0)
                    {
                        fndList = lstTag;

                        //on the first pass thru, set the list to the first entry showing in the dropdown list
                        //initChar = lnkList[x].FindElement(By.XPath(fndList)).Text.Substring(0, 1);
                        //lnkList[x].FindElement(By.XPath(fndList)).SendKeys(initChar);
                    }
                    else
                    {
                        //append value based on value of x to lstTag
                        fndList = lstTag + "[" + (x).ToString() + "]";
                    }

                    //set the list by default to the first entry in the list by sernding an 'A' to the list



                    if (lnkList[x].Text == lstItem)
                    {
                        //increment one place on the list because the list of dropdown items and array list
                        //are not based the same. The array list is 0-based and the lnkList is not
                        if ((x + 1) == 1)
                            fndList = lstTag + suffix;
                        else
                            fndList = lstTag + "[" + (x + 1).ToString() + "]" + suffix;

                        //Record result
                        tmpString = "Found list item [" + lstItem + "]";
                        tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                        //Click the selected list item
                        lnkList[x].FindElement(By.XPath(fndList)).Click();
                        fndItem = 1;


                        //Record successful action in result file
                        tmpString = "Successfully selected item [" + lstItem + "]";
                        tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                        //TextFileOps.Write(pth, "Successfully selected item [" + lstItem + "]", 1);
                    }
                    else
                    {
                        switch (drpName)
                        {
                            case "/html/body/table/tbody/tr/td/table[2]/tbody/tr[2]/td[2]/table/tbody/tr" +
                                "/td/div/form/div/div/table/tbody/tr[2]/td[2]/div/div/div/div/div/table/tbody/tr[4]/td/div/div/div/span":

                                if (x == 0)
                                    driver.FindElement(By.LinkText(lnkList[x].Text)).SendKeys(sndKey);
                                else
                                {
                                    if (sndKey != lnkList[x].Text.Substring(0, 1))
                                    {
                                        sndKey = lnkList[x].Text.Substring(0, 1);
                                        driver.FindElement(By.LinkText(lnkList[x].Text)).SendKeys(sndKey);
                                    }
                                }

                                break;
                        }
                    }

                    if (fndItem == 1)
                    {
                        break;
                    }
                }

                if (fndItem != 1)
                {
                    //Record result
                    tmpString = "The item [" + lstItem + "] was not found";
                    tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                    //TextFileOps.Write(pth, "<br />", clrIndex);
                }

            }
            catch (Exception e)
            {
                //Record failed result
                tmpString = "Failed clicking the dropdown list box: " + e.Message;
                tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                //Record exception and begin exit process
                fndExcep = -1;
            }

            if (chkMod == "Y")
            {
                //Switch back to original frame from modal
                driver.SwitchTo().DefaultContent();
                Thread.Sleep(750);
            }
        }
        #endregion

        #region sendKeys
        public void sendKeys(tstObject tstObj, string[] inArray, string inStep, string pth, out int fndExcep, string stpNum)
        {
            fndExcep = 0;

            driver.FindElement(By.XPath(inArray[0])).SendKeys(inArray[1]);
        }
        #endregion

        #region switchTab
        public void switchTab(tstObject tstObj, string[] inArray, string inStep, string pth, out int fndExcep, string stpNum)
        {
            fndExcep = 0;

            driver.FindElement(By.XPath(inArray[1])).SendKeys(OpenQA.Selenium.Keys.Control + inArray[1]);
        }
        #endregion

        #region tblSelect
        public void tblSelect(tstObject tstObj, bool objPres, string[] inArray, string stpNum, string pth, ref string[,] tstresult, out int fndExcep)
        {
            //inArray contains all the data that has been imported from the spreadsheet. The data will parcelecd out
            //and placed into the respective values that each item corresponds to


            bool lstPath;
            int fndItem;
            int itmCount;
            int numRows;
            int trim;
            int strLen;
            int tblMove;
            string chkMod;
            string frmMod;
            string tblNav;
            string rowText;
            string fndPath;
            string tblId;
            string tblName;
            string tblTag;
            string tmpCount;
            string tmpString;
            string schCol;
            string schText;
            string slct;
            string suffix;
            string vis;
            Boolean found;

            found = false;
            lstPath = false;

            fndExcep = 0;
            fndItem = 0;
            strLen = 0;
            chkMod = "";
            fndPath = "";
            frmMod = "";
            rowText = "";
            schCol = "";
            schText = "";
            tblId = "";
            tblMove = 0;
            tblName = "";
            tblNav = "";
            tblTag = "";
            suffix = "";
            slct = "";
            vis = "";

            //assign variables from passed inArray
            try
            {
                for (int x = 0; x < inArray.Length; x++)
                {
                    if (inArray[x] != "")
                    {
                        switch (x)
                        {
                            case 0:
                                //the xpath down to the table level. Should end in /table
                                //the /tr, /td, and indexes in the xpath items will be appended
                                //by the application. the suffix variable will be appended
                                //to complete the xpath
                                tblId = inArray[x];
                                break;
                            case 1:
                                //the text being searched for
                                schText = inArray[x];
                                break;
                            case 2:
                                //the name of the table. Usually the window the table resides in. Used for result
                                tblName = inArray[x];
                                break;
                            case 3:
                                //the column of the table being searched
                                schCol = inArray[x];
                                break;
                            case 4:
                                //the column of the found row that an action will take place on
                                tblNav = inArray[x];
                                break;
                            case 5:
                                //check if a table is located in a modal window
                                chkMod = inArray[x];
                                break;
                            case 6:
                                //the id of the modal window
                                frmMod = inArray[x];
                                break;
                            case 7:
                                //additional string to perform an action
                                suffix = inArray[x];
                                if (suffix == "N/A")
                                    suffix = "";
                                break;
                            case 8:
                                //
                                trim = TestSuite.trimString(tblId);
                                if (inArray[x] != "Y")
                                    lstPath = false;
                                else
                                    lstPath = true;
                                break;
                            case 9:
                                //variable regualating whether or not a value will be selected of verified
                                slct = inArray[x];
                                if (slct == "X")
                                    vis = "N";
                                else if (slct.Length < 3)
                                {
                                    vis = "Y";
                                    slct = slct.Substring(0, 1);
                                }
                                else
                                {
                                    vis = slct.Substring(2, 1);
                                    slct = slct.Substring(0, 1);
                                }
                                break;
                        }
                    }
                }

                //set strLen to the search text length. This will be used in case of any suffix
                //entries in  the searched column
                strLen = schText.Length;

                //get number of pages in the table
                if (lstPath == true)
                {
                    //get the number of pages in the table if lstPath has a value
                    //Click on the 'last>>' link to go to the last table page 
                    driver.FindElement(By.LinkText("last »")).Click();

                    //extract the string value of the last page number 
                    tmpCount = driver.FindElement(By.ClassName("pager-current")).Text;

                    //convert to an int for use on a for loop
                    itmCount = Convert.ToInt32(tmpCount);

                    //click on the '<<first' link to return to the first page 
                    driver.FindElement(By.LinkText("« first")).Click();

                    //if there are less than three list items, set the itmCount variable == 3
                    if (itmCount < 3)
                    {
                        itmCount = 3;
                    }
                }
                else
                {
                    //set lstpath to 3. When processing 2 is subtracted to account for non-numerical links
                    //in the page list (next>>, <<previous, etc.)
                    itmCount = 3;
                }

                //Convert the loaded tblNav string variable to the tblMove integer
                if (tblNav != "")
                    tblMove = Convert.ToInt32(tblNav);

                //Check for modal input. If no modal input stop app for 1 sec
                if (chkMod == "Y")
                {
                    //switch frame
                    driver.SwitchTo().DefaultContent();
                    driver.SwitchTo().Frame(frmMod);
                    Thread.Sleep(750);
                }

                if (tblTag != "")
                {
                    tblId = tblId + tblTag;

                    //check to see that the table exists
                    objPres = driver.FindElement(By.XPath(tblId)).Enabled;
                }
                else
                {
                    //check to see that the table exists
                    objPres = driver.FindElement(By.XPath(tblId)).Enabled;
                }

                //add string to the test results list
                tmpString = "Searching for the " + tblName + " table........(" + stpNum + ")";
                tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);


                for (int tblCount = 0; tblCount < itmCount; tblCount++)
                {
                    numRows = driver.FindElements(By.XPath(tblId + "/tbody/tr")).Count;

                    //x is the row number currently being processed
                    for (int x = 0; x < numRows; x++)
                    {
                        try
                        {
                            //check if the table item is present and get the text of the searched item
                            if (x == 0)
                            {
                                if ((tblMove == 0 && schCol == "0") || (tblMove == 0 && schCol == "1"))
                                {
                                    //check if initial xpath object is present
                                    objPres = driver.FindElement(By.XPath(tblId + "/tbody/tr/td")).Enabled;

                                    //get the row text of the [0] item
                                    rowText = driver.FindElement(By.XPath(tblId + "/tbody/tr/td")).Text;
                                }
                                else
                                {
                                    //check if initial xpath object is present
                                    objPres = driver.FindElement(By.XPath(tblId + "/tbody/tr/td[" + schCol + "]")).Enabled;

                                    //get the row text of the [x] item
                                    rowText = driver.FindElement(By.XPath(tblId + "/tbody/tr/td[" + schCol + "]")).Text;
                                }
                            }
                            else
                            {
                                if (schCol == "1")
                                    rowText = driver.FindElement(By.XPath(tblId + "/tbody/tr[" + (x + 1).ToString() +
                                    "]/td")).Text;
                                else
                                    rowText = driver.FindElement(By.XPath(tblId + "/tbody/tr[" + (x + 1).ToString() +
                                    "]/td[" + schCol + "]")).Text;
                            }
                        }
                        catch
                        {
                            continue;
                        }

                        //see if the search string is in the table entry
                        rowText = TestSuite.getTableEntry(rowText.Trim(), schText.Trim(), schText.Trim().Length);

                        //get the row text from the beginning to the strLen and check if it matches the schText
                        //this is in case any suffix entries (ex: member count) are present
                        if (rowText == schText.Trim())
                        {

                            if (x == 0 && ((Convert.ToInt32(schCol) + (tblMove)) == 0 || (Convert.ToInt32(schCol) + (tblMove)) == 1))
                                fndPath = tblId + "/tbody/tr/td" + suffix;
                            else if (x == 0)
                                fndPath = tblId + "/tbody/tr/td[" + (Convert.ToInt32(schCol) + (tblMove)) + "]" + suffix;
                            else if ((Convert.ToInt32(schCol) + (tblMove)) == 0 || (Convert.ToInt32(schCol) + (tblMove)) == 1)
                                fndPath = tblId + "/tbody/tr[" + (x + 1) + "]/td" + suffix;
                            else
                                fndPath = tblId + "/tbody/tr[" + (x + 1) + "]/td[" + (Convert.ToInt32(schCol) + (tblMove)) + "]" + suffix;

                            //if the item is to be selected, enter this portion of the if statement
                            if (slct == "Y")
                            {
                                //set clkCell to a WebElement at the fndPath xpath
                                driver.FindElement(By.XPath(fndPath)).Click();

                                //Record the result
                                tmpString = "The item '" + schText + "' was found in the the " + tblName + " table";
                                tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                            }

                            //added code
                            else if (slct == "N")
                            {
                                Boolean view;
                                found = true;
                                view = driver.FindElement(By.XPath(fndPath)).Displayed;
                                if (view && vis == "Y")
                                {
                                    //Record the result
                                    tmpString = "The item at position(" + fndPath + ") was found in the the " + tblName + " table, and is visible";
                                    tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                }
                                else if (view && vis == "N")
                                {
                                    //Record the result
                                    tmpString = "The item at position(" + fndPath + ") was found in the the " + tblName + " table, and should not be visible";
                                    tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                }
                            }

                            //set fndItem to 1 to denaote that the itme has been found
                            fndItem = 1;
                            break;
                        }
                        //increment the fndPath string to to the next row before incrementing x in the for loop if the 
                        //current row doesn't match
                        else
                        {

                            fndPath = tblId + "/tbody/tr[" + (x + 1).ToString() + "]";
                        }
                    }


                    //if the item is not found and there are mutiple pages, select the next page 
                    if (fndItem != 1)
                    {
                        if (tblCount != itmCount)
                        {
                            //select the next page ion nthe table page list
                            driver.FindElement(By.LinkText((tblCount + 2).ToString())).Click();
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }


                //if the item was not found in the table, record the result in the result file
                if (fndItem != 1)
                {
                    if (slct == "X")
                    {
                        objPres = false;
                        //Record the result
                        tmpString = "The item '" + schText + "' was not found in the the " + tblName + " table";
                        tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                        //TextFileOps.Write(pth, "The item '" + schText + "' was not found in the the " + tblName + " table", 1);
                    }
                    else
                    {
                        //Set objPres to false to indicate that no itwm was found
                        objPres = false;

                        //Record the result
                        tmpString = "The item '" + schText + "' was not found in the the " + tblName + " table";
                        tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                        //set the fndExcep escape variable to leave the app
                        fndExcep = -1;
                    }
                }

                //if a modal table, return focus to the default window
                if (chkMod == "Y")
                {
                    driver.SwitchTo().DefaultContent();
                    Thread.Sleep(750);
                }
            }
            catch (Exception e)
            {
                if (slct == "N" && found == true)
                {
                    if (vis == "N")
                    {
                        //Record the result
                        tmpString = "The item at position(" + fndPath + ") was found in the the " + tblName + " table, and is not visible";
                        tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                    }
                    else if (vis == "Y")
                    {
                        //Record the result
                        tmpString = "The item at position(" + fndPath + ") was found in the the " + tblName + " table, and is not visible";
                        tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                    }
                }
                else
                {
                    if (slct == "X")
                    {
                        //Record the result
                        tmpString = "The item '" + schText + "' was not found in the the " + tblName + " table";
                        tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                    }
                    else
                    {
                        //Record the result
                        tmpString = e.Message;
                        arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                        //set the fndExcep escape variable to leave the app
                        fndExcep = -1;
                    }
                }
            }
        }
        #endregion

        #region Wait
        public void Wait(string inTime)
        {
            Thread.Sleep(Convert.ToInt32(inTime) * 1000);
        }
        #endregion

        #region vfyGrades
        public void vfyGrades(tstObject tstObj, bool objPres, string[] inArray, string stpNum, string pth, ref string[,] rsltArray, out int fndExcep, out int tstFail)
        {
            //this function will evaluate that the correct letter grade has been assigned based upon the percentage achieved by the student
            //only one student's grade can be evaluated with each step assigned in the spreadsheet or database
            bool objGrade;
            double grdPercent;
            int grdCount;
            string upperBound;
            string lowerBound;
            string corrGrade;
            string tmpPct;
            string tblGrade;
            string stdntName;
            string grade; 
            string xpath;
            string plsMinus;
            string[,] gradeArray;

            upperBound = "";
            lowerBound = "";
            tstFail = 0;
            corrGrade = "";
            xpath = "";
            grdCount = 0;
            fndExcep = 0;

            try
            {
                //set function variables 
                for (int x = 0; x < 2; x++)
                {
                    switch (x)
                    {
                        //the grade field xpath
                        case 0:
                            xpath = inArray[x];
                            break;
                        //flag for whether +/- grades are included
                        case 1:
                            plsMinus = inArray[x];
                            break;
                    }

                }

                //establish the size of the array based on whether or not +/- grades are included
                if (inArray[1] != "Y")
                {
                    gradeArray = new string[6, 2];
                }
                else
                {
                    gradeArray = new string[14, 2];
                }

                //set the array with the grade upper and lower bounds
                for (int x = 2; x < inArray.Length; x++)
                {
                    if (inArray[x] != "")
                    {
                        grade = getGrade(x);
                        gradeArray[grdCount, 0] = grade;
                        gradeArray[grdCount, 1] = inArray[x];
                        grdCount++;
                    }

                }

                //get the student's name
                stdntName = driver.FindElement(By.XPath(xpath + "/td[2]")).Text;

                //get the grade from the table
                tblGrade = driver.FindElement(By.XPath(xpath + "/td[3]")).Text;

                //get the grade percent from a straight copy of text from the Pct field and strip off the percent sign
                tmpPct = driver.FindElement(By.XPath(xpath + "/td[4]")).Text;
                tmpPct = tmpPct.Substring(0, tmpPct.Length - 1);

                //Convert the tmpPct to a double
                grdPercent = Convert.ToDouble(tmpPct);

                //verify the logged grade is the correct one based on the upper and lower bounds
                for (int x = 0; x < gradeArray.GetLength(0) - 1; x++)
                {
                    if (grdPercent > Convert.ToDouble(gradeArray[0, 1]))
                    {
                        corrGrade = gradeArray[0, 0];
                        upperBound = "110";
                        lowerBound = gradeArray[0, 1];
                        break;
                    }
                    else if (grdPercent <= Convert.ToDouble(gradeArray[x, 1]) && grdPercent >= Convert.ToDouble(gradeArray[x + 1, 1]))
                    {
                        corrGrade = gradeArray[x + 1, 0];
                        upperBound = gradeArray[x, 1];
                        lowerBound = gradeArray[x + 1, 1];
                        break;
                    }
                }

                //compare the table grade with the expected grade
                if (corrGrade == tblGrade)
                    objGrade = true;
                else
                    objGrade = false;

                //log result into the result srray
                rsltArray = arrayAppend("verify", "grades", objGrade.ToString(), corrGrade, tmpPct, upperBound, lowerBound, tblGrade, stdntName, rsltArray);
            }
            catch (Exception e)
            {
                objPres = false;

                //add string to the test results list
                tmpString = e.Message;
                rsltArray = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, rsltArray);

                fndExcep = -1;
            }
        }
        #endregion

        #region vfyTableEntry
        public void vfyTableEntry(tstObject tstObj, string tblPath, string xpath, string schCol, string mvCol, string vfySegment, string offsetItem,
            string getNeg, string isHeader, string thsTable, string schItem, string datsource, string stpNum, string pth, ref string retXpath, ref string[,]rsltArray, out int fndExcep, out int tstFail)
        {
            bool objPres;
            string tblEntry;
            int fndItem;
            string fndPath;
            int numRows;

            fndPath = "";
            objPres = true;
            tblEntry = "";
            numRows = 0;
            fndItem = 0;
            tstFail = 0;
            fndExcep = 0;

            try
            {
                //get the table Entry...this may need to be gleaned from the table entry using Fierbug as there may very well be other
                //things present (exclamation point, pencil icon, etc.) which may changer the text present
                objPres = tstObj.IsElementPresent(By.XPath(tblPath));

                //no need to get the number of rows unless the object is present
                if (objPres == true)
                {
                    numRows = driver.FindElements(By.XPath(tblPath + xpath)).Count;

                    if (numRows > 0)
                    {
                        for (int x = 1; x <= numRows; x++)
                        {
                            //check for a header item in the table and append /th tags
                            if (isHeader == "Y")
                            {
                                if (x == 1)
                                {
                                    tblEntry = driver.FindElement(By.XPath(tblPath + xpath + "/th[" + schCol + "]")).Text;
                                }
                                else
                                {
                                    tblEntry = driver.FindElement(By.XPath(tblPath + xpath + "[" + x.ToString() + "]/th[" + schCol + "]")).Text;
                                }
                            }
                            //if not a header append standard /tr (table row) tags
                            else
                            {
                                if (x == 1)
                                {
                                    //initial row condition. Straight copy of xpath variable
                                    fndPath = tblPath + xpath + "/td[" + schCol + "]";
                                    tblEntry = driver.FindElement(By.XPath(fndPath)).Text;
                                }
                                else
                                {
                                    //subsequent row condition. Append an index [x] to the xpath
                                    fndPath = tblPath + xpath + "[" + x.ToString() + "]/td[" + schCol + "]";
                                    tblEntry = driver.FindElement(By.XPath(fndPath)).Text;
                                }
                            }

                            if (datsource == "EX" && vfySegment != "Y")
                                tblEntry = TestSuite.setSlash(tblEntry);
                            else if (vfySegment == "Y")
                                tblEntry = TestSuite.getTableEntry(tblEntry.Trim(), schItem, schItem.Length);

                            //enter into this 
                            if (tblEntry.Trim() == schItem)
                            {
                                if (mvCol == "Y")
                                {
                                    retXpath = fndPath;
                                    break;
                                }

                                if (Convert.ToInt32(mvCol) >= 1)
                                {
                                    tblEntry = driver.FindElement(By.XPath(tblPath + xpath + "[" + x.ToString() + "]/td[" +
                                        (Convert.ToInt32(schCol) + Convert.ToInt32(mvCol)).ToString() + "]")).Text;
                                }
                                else
                                {
                                    offsetItem = schItem;
                                }

                                if (offsetItem == tblEntry.Trim())
                                {
                                    //write to results file that item was found in the offset column
                                    objPres = true;
                                    fndItem = 1;
                                    break;
                                }
                                else
                                {
                                    //set  objPres to false and keep scrolling if offset item doesn't match
                                    objPres = false;
                                    tstFail = -1;
                                    fndItem = 0;
                                    break;
                                }
                            }
                            else
                            {
                                if (x == numRows)
                                {
                                    //if the item  is supposed to be on the screen, but is not set the testfail variable to log the failed test
                                    //if getNeg is 'N' and the item is not present, set  variables to show this
                                    if (getNeg != "N")
                                    {
                                        tstFail = -1;
                                    }
                                }
                                fndItem = 0;
                                objPres = false;
                            }
                        }
                    }
                    else
                    {
                        if (getNeg == "N")
                        {
                            objPres = false;
                            fndItem = 0;
                        }
                        else
                        {
                            objPres = true;
                            tstFail = -1;
                        }
                    }
                }
                else
                {
                    if (getNeg == "N")
                    {
                        fndItem = 0;
                    }
                    else
                    {
                        fndItem = 1;
                        tstFail = -1;
                    }

                }
            }
            catch (Exception e)
            {
                TextFileOps.Write(pth, e.Message, -1);

                fndExcep = -1;
            }

            //if there is no offset on the table then use schItem to record results
            //if mvCol is > 0 then ise the offsetItem
            if (mvCol != "Y")
            {
                if (Convert.ToInt32(mvCol) == 0)
                {
                    rsltArray = arrayAppend("verify", "table", objPres.ToString(), schItem.Trim(), schItem.Trim(), thsTable, String.Empty, String.Empty, String.Empty, rsltArray);
                }
                else
                {
                    rsltArray = arrayAppend("verify", "table", objPres.ToString(), tblEntry.Trim(), offsetItem, thsTable, String.Empty, String.Empty, String.Empty, rsltArray);
                }

                fndItem = 0;
            }
        }
        #endregion

        #region vfyTooltip
        public void vfyTooltip(tstObject tstObj, string tblPath, ref string[,] rsltArray, out int fndExcep, out int tstFail)
        {
            bool objPres;
            string clkPath;
            string tempstring;
            string vfyString;
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            Actions builder = new Actions(driver);
            IWebElement thsObj;

            fndExcep = 0;
            tstFail = 0;

            try
            {
                tempstring = tblPath.Substring(1, tblPath.Length - 4);
                clkPath = tempstring + "[2]";

                driver.FindElement(By.XPath(clkPath)).Click();

                //strip off the last
                //set theObj to the object the tooltip is in
                thsObj = (IWebElement)driver.FindElement(By.XPath(tblPath));

                //move the app to the tooltip object
                Actions hoverOverTooltip = builder.MoveToElement(thsObj);

                //perform the hover
                hoverOverTooltip.Perform();

                vfyString = driver.FindElement(By.XPath(tblPath)).GetAttribute("onmouseover");

            }

            catch(Exception e)
            {
                objPres = false;

                //add string to the test results list
                tmpString = e.Message;
                rsltArray = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, rsltArray);

                fndExcep = -1;
            }
        }
        #endregion

        #region vfyField
        /*public void vfyField(tstObject tstObj, string itmPath, string vfyPath, string suffix, string getNeg, string lstVerify, string fldName,
            string fldType, string stpNum, string pth, out int fndExcep, out int tstFail)
        {
            bool objPres;
            string thsItem;
            int fndItem;
            string[] vfyList;
            ReadOnlyCollection<IWebElement> lstItems;

            //initialize variables
            objPres = true;
            tstFail = 0;
            fndExcep = 0;
            thsItem = "";
            fndItem = 0;
            lstItems = null;

            //populate vfyPath if it is blank
            if (itmPath != "" && vfyPath == "")
            {
                vfyPath = itmPath;
            }

            if (getNeg != "N")
                objPres = tstObj.IsElementPresent(By.XPath(itmPath));

            if (fldType != " text")
            {
                //get the number of list items in the field being verified 
                try
                {
                    lstItems = driver.FindElements(By.XPath(itmPath));
                    objPres = true;
                }
                catch
                {
                    objPres = false;
                    fndItem = 0;
                }

                if (getNeg != "N" && objPres == true)
                {
                    //instantiate an Array to hold all the list items
                    vfyList = new string[lstItems.Count];

                    for (int x = 0; x < lstItems.Count; x++)
                    {
                        vfyList[x] = lstItems[x].Text;
                    }

                    //Checking if the item is present
                    //TextFileOps.Write(pth, "<li>", 100);
                    TextFileOps.Write(pth, "Checking the " + fldName + " " + fldType + " item(s).....\r\n", 0);
                    //TextFileOps.Write(pth, "</li>", 100);

                    lstItems = driver.FindElements(By.XPath(vfyPath));

                    //search loop looking for the correct item

                    if (fldType != " image")
                        for (int x = 0; x < vfyList.Length; x++)
                        {
                            try
                            {
                                if (vfyList[x].Trim() == fldName)
                                {
                                    thsItem = vfyList[x];
                                    fndItem = 1;
                                }
                            }
                            catch (Exception e)
                            {
                                objPres = false;
                                //TextFileOps.Write(pth, "<li>", 100);
                                TextFileOps.Write(pth, e.Message, -1);
                                //TextFileOps.Write(pth, "</li>", 100);
                                fndExcep = -1;

                                faillst += Verify_User_Screens_Firefox.VerifyScreens.testname + "[" + stpNum + "],";
                            }

                            //if teh item has been found break out the searching loop else keep looking
                            if (fndItem == 1)
                                break;
                            else
                                fndItem = 0;
                        }
                }
            }
            else if (fldType == " text")
            {
                try
                {
                    thsItem = driver.FindElement(By.XPath(vfyPath)).Text.Trim();

                    if (thsItem == fldName)
                    {
                        fndItem = 1;
                    }
                    else
                    {
                        objPres = false;
                        fndItem = 0;
                    }
                }
                catch (Exception e)
                {
                    objPres = false;
                    //TextFileOps.Write(pth, "<li>", 100);
                    TextFileOps.Write(pth, e.Message, -1);
                    //TextFileOps.Write(pth, "</li>", 100);
                    fndExcep = -1;

                    faillst += Verify_User_Screens_Firefox.VerifyScreens.testname + "[" + stpNum + "],";
                }
            }

            if (getNeg == "N" && fndItem == 0)
            {
                objPres = false;
            }

            switch (fldType)
            {
                case " dropdown":
                    Recorder.dropdownListVerify(objPres, fldName, lstVerify, pth, getNeg);
                    break;
                case " field":
                    Recorder.fldVerify(objPres, lstVerify, fldName, pth, getNeg);
                    break;
                case " text":
                    Recorder.txtVerify(objPres, lstVerify, fldName, pth, getNeg);
                    break;
                case " button":
                    Recorder.btnVerify(objPres, fldName, lstVerify, pth, getNeg);
                    break;
                case " link":
                    Recorder.lnkVerify(objPres, lstVerify, fldName, pth, getNeg);
                    break;
            }
        }*/
        #endregion

        #region vfyObject
        public void vfyObject(tstObject tstObj, string itmPath, string suffix, string getNeg, string lstVerify, string fldName,
            string fldType, bool objPres, string stpNum, string pth, ref string[,] rsltArray, out int fndExcep, out int tstFail)
        {
            string vfyString;
            tstFail = 0;
            fndExcep = 0;
            vfyString = "";

            switch (fldType.Trim())
            {
                case "dropdown":
                    if (getNeg != "N")
                    {
                        vfyString = driver.FindElement(By.XPath(itmPath)).Text;
                    }

                    objPres = objVerify(vfyString, fldName, getNeg, ref tstFail);

                    rsltArray = arrayAppend("verify", "dropdown", objPres.ToString(), fldName.Trim(), vfyString, lstVerify, String.Empty, String.Empty, String.Empty, rsltArray);
                    break;
                case "field":
                    fldType = suffix;
                    switch (fldType)
                    {
                        case "Text":
                        {
                            //get the value attribute of a text field. The value attribute holds the text in a text field 
                            if (getNeg != "N")
                            {
                                vfyString = driver.FindElement(By.XPath(itmPath)).GetAttribute("value");
                            }

                            objPres = objVerify(vfyString, fldName, getNeg, ref tstFail);


                            rsltArray = arrayAppend("verify", "field text", objPres.ToString(), fldName.Trim(), vfyString, lstVerify, String.Empty, String.Empty, String.Empty, rsltArray);
                            break;
                        }
                    }


                    //TextFileOps.Write(pth, "<li>", 100);
                    //Recorder.fldVerify(objPres, lstVerify, fldName, pth, getNeg);
                    //TextFileOps.Write(pth, "</li>", 100);
                    break;
                case "text":
                    if (getNeg != "N")
                    {
                        vfyString = driver.FindElement(By.XPath(itmPath)).Text;
                    }

                    vfyString = TestSuite.getTableEntry(vfyString.Trim(), fldName, fldName.Length);

                    objPres = objVerify(vfyString, fldName, getNeg, ref tstFail);

                    rsltArray = arrayAppend("verify", "text", objPres.ToString(), lstVerify, vfyString, fldName.Trim(), String.Empty, String.Empty, String.Empty, rsltArray);
                    break;
                case "button":
                    if (getNeg != "N")
                    {
                        vfyString = driver.FindElement(By.XPath(itmPath)).Text;
                    }

                    objPres = objVerify(vfyString, fldName, getNeg, ref tstFail);

                    rsltArray = arrayAppend("verify", "button", objPres.ToString(), fldName.Trim(), vfyString, lstVerify, String.Empty, String.Empty, String.Empty, rsltArray);
                    break;
                case "link":
                    if (getNeg != "N")
                    {
                        vfyString = driver.FindElement(By.XPath(itmPath)).Text;
                    }
                    
                    rsltArray = arrayAppend("verify", "button", objPres.ToString(), fldName.Trim(), vfyString, String.Empty, String.Empty, String.Empty, String.Empty, rsltArray);
                    break;
                case "image":
                    if (getNeg != "N")
                    {
                        vfyString = driver.FindElement(By.XPath(itmPath)).Text;
                    }
                    //TextFileOps.Write(pth, "<li>", 100);
                    //rsltStatement[0, 1] = "image";
                    Recorder.imgVerify(objPres, fldName, lstVerify, pth, getNeg);
                    //TextFileOps.Write(pth, "</li>", 100);
                    break;
            }
        }
        #endregion

        #region vfyOutcome
        private void vfyOutcome(tstObject tstObj, string itmPath, string[] inArray, string stdntName, bool objPres, string stpNum, string pth, ref string[,] arrOutcome, ref string[,] rsltArray, out int fndExcep, out int tstFail)
        {
            bool itmMatch;
            decimal d;
            decimal rounded;
            double pctOutcome;
            double fnlPct;
            int b; 
            int earnScore;
            int totPoss;
            int cntOutcome;
            int cnt;
            int tmpCnt;
            int numRows;
            int numActivities;
            int numOutcomes;
            string[,] outcomeArray;
            string[,] scores;
            string hdrPath;
            string hdrText;
            string schPath;
            string schItem;
            string nameColumn;
            string vfyPercentage;

            outcomeArray = null;
            pctOutcome = 0.00;
            b = 0;
            earnScore = 0;
            totPoss = 0;
            tmpCnt = 1;
            cntOutcome = 0;
            fndExcep = 0;
            tstFail = 0;

            try
            {
                //get the number of weights in brought in from the inArray
                for (int x = 2; x < 9; x++)
                {
                    if (inArray[x] != "")
                    {
                        cntOutcome++;
                    }
                }

                hdrPath = inArray[1];

                arrOutcome = new string[cntOutcome, 2];
                cntOutcome = 0;

                //get the outcomes and weights from the inArray
                for (int x = 2; x < 9; x++)
                {
                    if (inArray[x] != "")
                    {
                        switch (x)
                        {
                            case 2:
                                arrOutcome[cntOutcome, 0] = "Collaboration";
                                break;
                            case 3:
                                arrOutcome[cntOutcome, 0] = "Content Literacy";
                                break;
                            case 4:
                                arrOutcome[cntOutcome, 0] = "Critical Thinking";
                                break;
                            case 5:
                                arrOutcome[cntOutcome, 0] = "Oral Communication";
                                break;
                            case 6:
                                arrOutcome[cntOutcome, 0] = "Technology Literacy";
                                break;
                            case 7:
                                arrOutcome[cntOutcome, 0] = "Work Ethic and Contribution";
                                break;
                            case 8:
                                arrOutcome[cntOutcome, 0] = "Written Competency";
                                break;
                        }

                        arrOutcome[cntOutcome, 1] = inArray[x];
                        cntOutcome++;
                    }
                }

                //get the number of rows in the student table 
                numRows = driver.FindElements(By.XPath(itmPath + "/tbody/tr")).Count;

                for (int x = 0; x < numRows; x++)
                {
                    //construct the correct path to select a student
                    if (x == 0)
                    {
                        schPath = itmPath + "/tbody/tr/td[2]";
                    }
                    else
                    {
                        schPath = itmPath + "/tbody/tr" + "[" + (x + 1).ToString() + "]" + "/td[2]";
                    }

                    //once the student name is found click it to bring up the student's grade page 
                    if (driver.FindElement(By.XPath(schPath)).Text == stdntName)
                    {
                        driver.FindElement(By.XPath(schPath + "/a")).Click();
                        numOutcomes = (driver.FindElements(By.XPath(hdrPath + "/thead/tr/th")).Count) - 5;

                        outcomeArray = new string[numOutcomes, 4];

                        for (int y = 0; y < numOutcomes; y++)
                        {
                            if (y == 0)
                                hdrText = hdrPath + "/thead/tr/th[5]";
                            else
                                hdrText = hdrPath + "/thead/tr/th[" + (y + 5).ToString() + "]";

                            nameColumn = driver.FindElement(By.XPath(hdrText)).GetAttribute("title");

                            outcomeArray[y, 0] = nameColumn;
                        }

                        numActivities = driver.FindElements(By.XPath(hdrPath + "/tbody/tr")).Count;
                        scores = new string[numActivities, (numOutcomes * 2) + 1];

                        for (int z = 0; z < numActivities; z++)
                        {

                            for (int a = 0; a < numOutcomes + 1; a++)
                            {
                                //get the activity name for this activitiy 

                                if (a == 0)
                                {
                                    cnt = a;

                                    if (z == 0)
                                    {

                                        scores[z, cnt] = driver.FindElement(By.XPath(hdrPath + "/tbody/tr/td/a")).Text;
                                    }
                                    else
                                    {

                                        scores[z, cnt] = driver.FindElement(By.XPath(hdrPath + "/tbody/tr[" + (z + 1).ToString() + "]/td/a")).Text;
                                    }
                                }
                                else
                                {
                                    //populate the awarded score and the total points possible in the score array
                                    for (int cnt1 = 0; cnt1 < 2; cnt1++)
                                    {
                                        //shift the table cursor forward a + 5 cols to the get the processed score
                                        cnt = a + 4;

                                        if (cnt1 == 0)
                                            //get the awarded points 
                                            if (z == 0)
                                                scores[z, tmpCnt] = driver.FindElement(By.XPath(hdrPath + "/tbody/tr/td[" + cnt.ToString() + "]/table/tbody/tr/td")).Text;
                                            else
                                                scores[z, tmpCnt] = driver.FindElement(By.XPath(hdrPath + "/tbody/tr[" + (z + 1).ToString() + "]/td[" + cnt.ToString() + "]/table/tbody/tr/td")).Text;
                                        else
                                        {
                                            //get the total points
                                            if (z == 0)
                                                scores[z, tmpCnt + 1] = driver.FindElement(By.XPath(hdrPath + "/tbody/tr/td[" + cnt.ToString() + "]/table/tbody/tr/td[3]")).Text;
                                            else
                                                scores[z, tmpCnt + 1] = driver.FindElement(By.XPath(hdrPath + "/tbody/tr[" + (z + 1).ToString() + "]/td[" + cnt.ToString() + "]/table/tbody/tr/td[3]")).Text;

                                            //since there are two scores being populated, tmpCnt is the array cursor. This is the formula that 
                                            //will place then array cursor at the correct position to log each score starting with the awarded
                                            //points for each activity
                                            tmpCnt = tmpCnt + 2;
                                        }
                                    }
                                }
                            }
                            //reset tmpCnt after populating one row of the score array
                            tmpCnt = 1;
                        }

                        //click the gradebook link to return to the class wide gradebook screen
                        driver.FindElement(By.XPath("/html/body/table/tbody/tr/td/table[2]/tbody/tr/td/div[3]/div[2]/ul/li[4]/a")).Click();

                        b = 1;

                        //total all points possible (totPoss) and points earned (earnScore) for all activities the student has been graded on
                        for (int y = 0; y < outcomeArray.GetLength(0); y++)
                        {
                            for (int a = 0; a < scores.GetLength(0); a++)
                            {
                                if (scores[a, b] != "")
                                {
                                    earnScore = earnScore + Convert.ToInt32(scores[a, b]);
                                    totPoss = totPoss + Convert.ToInt32(scores[a, b + 1]);
                                }
                                else
                                    earnScore = 0;
                            }

                            //put the total earned and the total possible scores into the outcomeArray
                            outcomeArray[y, 1] = earnScore.ToString();
                            outcomeArray[y, 2] = totPoss.ToString();

                            //get the percentage in the form of a double
                            if (totPoss != 0)
                                pctOutcome = Convert.ToDouble(earnScore) / Convert.ToDouble(totPoss);
                            else
                                pctOutcome = 0;

                            //convert the double into as decimal and round
                            d = Convert.ToDecimal(pctOutcome);
                            rounded = Math.Round(d, 2);

                            pctOutcome = Convert.ToDouble(rounded);
                            //multiply decimal by 100 to get a whole percentage, convert to a string, 
                            //append a pct sign, and put the string into the array
                            outcomeArray[y, 3] = Convert.ToString(pctOutcome * 100);

                            earnScore = 0;
                            totPoss = 0;
                            b = b + 2;
                        }

                        //process the grades using the arrays containing the activities and the outcomes with percentages
                        for (int y = 0; y < outcomeArray.GetLength(0); y++)
                        {
                            if (x == 0)
                                schItem = driver.FindElement(By.XPath(itmPath + "/tbody/tr/td[" + (y + 5).ToString() + "]")).Text;
                            else
                                schItem = driver.FindElement(By.XPath(itmPath + "/tbody/tr[" + (x + 1).ToString() + "]/td[" + (y + 5).ToString() + "]")).Text;

                            //set itmMatch bool to report on results 
                            if ((outcomeArray[y, 3] + "%") == schItem)
                            {
                                itmMatch = true;
                            }
                            else
                            {
                                //if outcomeArray[y, 3] = 0 then schItem displays as '*' . This is a good condition
                                if (outcomeArray[y, 3] == "0" && schItem == "*")
                                {
                                    itmMatch = true;
                                }
                                else
                                {
                                    itmMatch = false;
                                    tstFail = -1;
                                }
                            }

                            rsltArray = arrayAppend("verify", "outcome", itmMatch.ToString(), schItem.Trim(), stdntName, outcomeArray[y, 0], outcomeArray[y, 1], outcomeArray[y, 2], outcomeArray[y, 3], rsltArray);
                        }

                        pctOutcome = 0;

                        //Take all of the final percentages per student per outcome and muliply them by the weights and 
                        //get the final running total (totWeightScore)
                        for (int y = 0; y < arrOutcome.GetLength(0); y++)
                        {
                            for (int z = 0; z < outcomeArray.GetLength(0); z++)
                            {
                                if (outcomeArray[z, 0] == arrOutcome[y, 0])
                                {
                                    pctOutcome = pctOutcome + (Convert.ToDouble(outcomeArray[z, 3]) * (Convert.ToDouble(arrOutcome[y, 1]) * .01));
                                }
                            }
                        }

                        //convert to a decimal and round to thousandths
                        d = Convert.ToDecimal(pctOutcome);
                        rounded = Math.Round(d, 1);

                        //mutiply by 100 to get the numerical pct to on decimal point as displayed in Echo
                        fnlPct = Convert.ToDouble(rounded);

                        //convert fnlPct to a string
                        vfyPercentage = Convert.ToString(fnlPct) + "%";

                        for (int y = 0; y < numRows; y++)
                        {
                            //construct the correct path to select a student
                            if (y == 0)
                            {
                                schPath = itmPath + "/tbody/tr/td[2]";
                            }
                            else
                            {
                                schPath = itmPath + "/tbody/tr" + "[" + (y + 1).ToString() + "]" + "/td[2]";
                            }

                            //once the student name is found click it to bring up the student's grade page 
                            if (driver.FindElement(By.XPath(schPath)).Text == stdntName)
                            {
                                if (y == 0)
                                {
                                    schItem = driver.FindElement(By.XPath(itmPath + "/tbody/tr/td[4]")).Text;
                                }
                                else
                                {
                                    schItem = driver.FindElement(By.XPath(itmPath + "/tbody/tr" + "[" + (y + 1).ToString() + "]" + "/td[4]")).Text;
                                }

                                //verify what's posted vis-svis what has been calculated
                                if (schItem == vfyPercentage)
                                    itmMatch = true;
                                else
                                {
                                    itmMatch = false;
                                    tstFail = -1;
                                }

                                rsltArray = arrayAppend("verify", "percentage", itmMatch.ToString(), schItem.Trim(), stdntName, vfyPercentage, String.Empty, String.Empty, String.Empty, rsltArray);
                                break;
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                //Record failed result
                tmpString = "Failed clicking the dropdown list box: " + e.Message;
                rsltArray = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, rsltArray);

                //Record exception and begin exit process
                fndExcep = -1;
            }
        }
        #endregion

        #region vfyImages
        public void vfyImages(tstObject tstObj, string itmPath, string suffix, string getNeg, string lstVerify, string fldName,
            string fldType, string stpNum, string pth, out int fndExcep)
        {
            bool objPres;
            fndExcep = 0;

            //check to see if image is present
            try
            {
                objPres = driver.FindElement(By.XPath(itmPath)).Displayed;
            }
            catch
            {
                objPres = false;
            }

            Recorder.imgVerify(objPres, fldName, lstVerify, pth, getNeg);
        }
        #endregion

        #region xlFunctions
        public string[,] xlFunctions(tstObject tstObj, string xlPath, string tstName, string shtName, out int dataIndex)
        {
            string[,] stpArray;
            string[,] dataArray;
            string[,] fnlArray;
            int[] itmNumArray;
            int totCols;

            //Get the list of steps this test will use
            stpArray = TestSuite.getXLData(tstName, xlPath, shtName);

            //Call get the number of columns in the data sheet to dimension the data array in order to send it out to the function
            dataIndex = TestSuite.getTotDataCols(stpArray.GetLength(0), stpArray, xlPath, out itmNumArray, out totCols);


            //initialize dataArray and send off to populate with sheet data
            dataArray = new string[stpArray.GetLength(0), dataIndex + 3];

            //get the fnlArray data from the spreadsheet
            fnlArray = new string[totCols, dataIndex];

            fnlArray = TestSuite.getArrayData(stpArray, xlPath, dataIndex);

            //Once data Array is populated it will contain the following information
            //[0] - Step Name
            //[1] - Step Line
            //[2] - Step Number - used in the result sheet
            //[3+] - Data sent to the application
            for (int x = 0; x < stpArray.GetLength(0); x++)
            {
                int b = 3;

                dataArray[x, 0] = stpArray[x, 0];
                dataArray[x, 1] = stpArray[x, 1];
                dataArray[x, 2] = stpArray[x, 2];


                //check for null data value
                if (stpArray[x, 1] != "<br />")
                {
                    for (int a = 0; a < dataIndex; a++)
                    {
                        if (fnlArray[x, a] != null)
                        {
                            dataArray[x, b] = fnlArray[x, a];
                            b++;
                        }
                        else
                        {
                            dataArray[x, b] = String.Empty;
                            b++;
                        }
                    }
                }
            }

            //return the final dataArray
            return dataArray;
        }
        #endregion

        #region IsElementPresent
        public bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region driveFunction
        public void driveFunction(tstObject tstObj, string doFunc, string stpNum, string[] inArray, string baseURL, string datsource, string pth, ref string getNeg, ref string[,] tstresult, out int fndExcep, out int tstFail)
        {
            bool dispObj;
            int failArrLen;
            string objString;
            string thsItem;
            string itmPath;
            string retXpath;
            //string tmpString;
            string[,] vfyArray;
            string[,] arrOutcome;
            string tblPath;
            TimeSpan currTime;
            WebDriverWait wait;

            //Function list that corresponds to each sheet on the data sheet. 
            //Array params are passed and the appropriate function is run 
            arrOutcome = null;
            dispObj = false;
            tstFail = 0;
            fndExcep = 0;
            retXpath = "";

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            vfyArray = new string[1, 9];

            try
            {
                switch (doFunc)
                {
                    #region Login
                    case "Login":
                    {
                        try
                        {
                            //login
                            tstObj.Login(tstObj, inArray, baseURL, stpNum, brwsrType, pth, out fndExcep, ref tstresult);
                        }
                        catch (Exception e)
                        {
                            //Record failed result
                            tmpString = e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;

                        }
                        break;
                    }
                    #endregion

                    #region Logout
                    case "Logout":
                    {
                        try
                        {
                            tstObj.Logout(pth, true, stpNum, ref tstresult);
                        }
                        catch (Exception e)
                        {
                            //Record failed result
                            tmpString = e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;

                        }
                        break;
                    }
                    #endregion

                    #region addOutcome
                    case "addOutcome":
                    {
                        try
                        {
                            addOutcome(tstObj, dispObj, inArray, stpNum, pth, ref tstresult, out fndExcep, out tstFail);
                        }
                        catch (NoSuchElementException e)
                        {

                            //Record failed result
                            tmpString = "Failed clicking the dropdown list box: " + e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;
                        }
                        break;
                    }
                    #endregion

                    #region chkCheckbox
                    case "chkCheckbox":
                    {
                        try
                        {
                            clkLink = inArray[0];

                            //check to see if the checkbox to be clicked is a modal
                            if (inArray[3] == "Y" || inArray[3] == "y")
                            {
                                driver.SwitchTo().Frame(inArray[4]);

                                do
                                {
                                    dispObj = WaitUntil(inArray[0], out objString, out currTime, "");

                                    if (currTime.Seconds >= 10)
                                    {
                                        dispObj = false;
                                        break;
                                    }
                                }
                                while (dispObj != true);


                                //if there is an exception, objString will not be blank
                                //if not blank, fail test and log exception. Otherwise continue execution 
                                if (objString == "")
                                {
                                    chkCheckbox(tstObj, dispObj, inArray, stpNum, pth, ref tstresult, out fndExcep);
                                    //TextFileOps.Write(pth, "<br />", clrIndex);
                                }
                                else
                                {
                                    fndExcep = -1;
                                    TextFileOps.Write(pth, objString, -1);
                                }

                                driver.SwitchTo().DefaultContent();
                            }
                            else
                            {
                                do
                                {
                                    dispObj = WaitUntil(inArray[0], out objString, out currTime, "");

                                    if (currTime.Seconds >= 10)
                                    {
                                        dispObj = false;
                                        break;
                                    }
                                }
                                while (dispObj != true);

                                //if there is an exception, objString will not be blank
                                //if not blank, fail test and log exception. Otherwise continue execution 
                                if (objString == "")
                                {
                                    chkCheckbox(tstObj, dispObj, inArray, stpNum, pth, ref tstresult, out fndExcep);
                                    //TextFileOps.Write(pth, "<br />", clrIndex);
                                }
                                else
                                {
                                    fndExcep = -1;
                                    TextFileOps.Write(pth, objString, -1);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            //Record failed result
                            tmpString = "Failed clicking the dropdown list box: " + e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;
                        }
                        break;
                    }
                    #endregion

                    #region clkButton
                    case "clkButton":
                    {
                        try
                        {
                            clkLink = inArray[0];
                            Thread.Sleep(1000);

                            //check to see if the button to be clicked is a modal
                            if (inArray[2] == "Y" || inArray[2] == "y")
                            {
                                driver.SwitchTo().Frame(inArray[3]);

                                do
                                {
                                    dispObj = WaitUntil(inArray[0], out objString, out currTime, "");

                                    if (currTime.Seconds >= 10)
                                    {
                                        dispObj = false;
                                        break;
                                    }
                                }
                                while (dispObj != true);

                                //if there is an exception, objString will not be blank
                                //if not blank, fail test and log exception. Otherwise continue execution 
                                if (objString == "")
                                {
                                    tstObj.clkButton(tstObj, dispObj, inArray, stpNum, pth, ref tstresult, out fndExcep);
                                    //TextFileOps.Write(pth, "<br />", clrIndex);
                                }
                                else
                                {
                                    fndExcep = -1;
                                    TextFileOps.Write(pth, objString, -1);
                                }

                                driver.SwitchTo().DefaultContent();
                            }
                            else
                            {
                                do
                                {
                                    dispObj = WaitUntil(inArray[0], out objString, out currTime, "");

                                    if (currTime.Seconds >= 10)
                                    {
                                        dispObj = false;
                                        break;
                                    }
                                }
                                while (dispObj != true);


                                if (objString == "")
                                {
                                    tstObj.clkButton(tstObj, dispObj, inArray, stpNum, pth, ref tstresult, out fndExcep);
                                    //TextFileOps.Write(pth, "<br />", clrIndex);
                                }
                                else
                                {
                                    fndExcep = -1;

                                    failArrLen = tstresult.GetLength(0);

                                    throw (new Exception(tstresult[failArrLen - 1, 1]));
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            //Record failed result
                            tmpString = "Failed clicking the dropdown list box: " + e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;
                        }
                        break;
                    }
                    #endregion

                    #region inputText
                    case "inputText":
                    {
                        try
                        {
                            if (inArray[3] == "Y" || inArray[3] == "y")
                            {
                                driver.SwitchTo().Frame(inArray[4]);

                                do
                                {
                                    dispObj = WaitUntil(inArray[0], out objString, out currTime, "");

                                    if (currTime.Seconds >= 10)
                                    {
                                        dispObj = false;
                                        break;
                                    }
                                }
                                while (dispObj != true);

                                //if there is an exception, objString will not be blank
                                //if not blank, fail test and log exception. Otherwise continue execution 
                                if (objString == "")
                                {
                                    tstObj.inputText(tstObj, dispObj, inArray, stpNum, pth, ref tstresult, out fndExcep);
                                    //TextFileOps.Write(pth, "<br />", clrIndex);
                                }
                                else
                                {
                                    fndExcep = -1;
                                }

                                driver.SwitchTo().DefaultContent();
                            }
                            else
                            {
                                if (inArray[0] != "URL")
                                {
                                    do
                                    {
                                        dispObj = WaitUntil(inArray[0], out objString, out currTime, "");

                                        if (currTime.Seconds >= 10)
                                        {
                                            dispObj = false;
                                            break;
                                        }
                                    }
                                    while (dispObj != true);


                                    if (objString == "")
                                    {
                                        tstObj.inputText(tstObj, dispObj, inArray, stpNum, pth, ref tstresult, out fndExcep);
                                    }
                                    else
                                    {
                                        fndExcep = -1;
                                    }
                                }
                            }
                        }
                        catch(Exception e)
                        {
                            //Record failed result
                            tmpString = "Failed clicking the dropdown list box: " + e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;
                        }
                        break;
                    }
                    #endregion

                    #region insertGrades
                    case "insertGrades":
                    {
                        try
                        {
                            wait.Until(drv => driver.FindElement(By.XPath(inArray[0])).Enabled);

                            tstObj.insertGrades(tstObj, dispObj, inArray, stpNum, pth, ref tstresult, out fndExcep);
                        }
                        catch (Exception e)
                        {
                            //Record failed result
                            tmpString = "Failed clicking the dropdown list box: " + e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;
                        }
                        break;
                    }
                    #endregion

                    #region navLinks
                    case "navLinks":                                    //navigate to a link
                    {
                        try
                        {
                            do
                            {
                                dispObj = WaitUntil(inArray[0], out objString, out currTime, "");

                                if (currTime.Seconds >= 10)
                                {
                                    dispObj = false;
                                    break;
                                }
                            }
                            while (dispObj != true);

                            if (dispObj == true)
                            {
                                tstObj.navLinks(tstObj, dispObj, inArray, stpNum, pth, ref tstresult, out fndExcep);
                                //TextFileOps.Write(pth, "<br />", clrIndex);
                            }
                            else
                            {
                                fndExcep = -1;
                                TextFileOps.Write(pth, objString, -1);
                            }
                        }
                        catch (Exception e)
                        {
                            //Record failed result
                            tmpString = "Failed clicking the dropdown list box: " + e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;
                        }
                        break;
                    }
                    #endregion

                    #region selDropdown
                    case "selDropdown":
                    {
                        try
                        {
                            if (inArray[5] == "Y" || inArray[5] == "y")
                            {
                                driver.SwitchTo().Frame(inArray[6]);

                                do
                                {
                                    dispObj = WaitUntil(inArray[0], out objString, out currTime, "");

                                    if (currTime.Seconds >= 10)
                                    {
                                        dispObj = false;
                                        break;
                                    }
                                }
                                while (dispObj != true);

                                //if there is an exception, objString will not be blank
                                //if not blank, fail test and log exception. Otherwise continue execution 
                                if (objString == "")
                                {
                                    tstObj.selDropDown(tstObj, dispObj, inArray[0], inArray[1], inArray[2], inArray[3], inArray[4], inArray[5], inArray[6], inArray[7], stpNum, pth, ref tstresult, out fndExcep);
                                    //TextFileOps.Write(pth, "<br />", clrIndex);
                                }
                                else
                                {
                                    fndExcep = -1;
                                    TextFileOps.Write(pth, objString, -1);
                                }

                                driver.SwitchTo().DefaultContent();
                            }
                            else
                            {
                                do
                                {
                                    dispObj = WaitUntil(inArray[0], out objString, out currTime, "");

                                    if (currTime.Seconds >= 10)
                                    {
                                        dispObj = false;
                                        break;
                                    }
                                }
                                while (dispObj != true);

                                //if there is an exception, objString will not be blank
                                //if not blank, fail test and log exception. Otherwise continue execution 
                                if (objString == "")
                                {
                                    tstObj.selDropDown(tstObj, dispObj, inArray[0], inArray[1], inArray[2], inArray[3], inArray[4], inArray[5], inArray[6], inArray[7], stpNum, pth, ref tstresult, out fndExcep);
                                    //TextFileOps.Write(pth, "<br />", clrIndex);
                                }
                                else
                                {
                                    fndExcep = -1;
                                    TextFileOps.Write(pth, objString, -1);
                                }
                            }
                        }
                        catch(Exception e)
                        {
                            //Record failed result
                            tmpString = "Failed clicking the dropdown list box: " + e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;
                        }
                        break;
                    }
                    #endregion

                    #region sendKeys
                    case "sendKeys":
                        tstObj.sendKeys(tstObj, inArray, stpNum, pth, out fndExcep, stpNum);
                        break;
                    #endregion

                    #region tblSelect
                    case "tblSelect":
                        {
                            try
                            {
                                if (inArray[5] == "Y" || inArray[5] == "y")
                                {
                                    driver.SwitchTo().Frame(inArray[6]);

                                    do
                                    {
                                        dispObj = WaitUntil(inArray[0], out objString, out currTime, "");

                                        if (currTime.Seconds >= 10)
                                        {
                                            dispObj = false;
                                            break;
                                        }
                                    }
                                    while (dispObj != true);


                                    if (dispObj == true)
                                    {
                                        tstObj.tblSelect(tstObj, dispObj, inArray, stpNum, pth, ref tstresult, out fndExcep);
                                    }
                                    else
                                    {
                                        fndExcep = -1;
                                        TextFileOps.Write(pth, objString, -1);
                                    }

                                    driver.SwitchTo().DefaultContent();
                                }
                                else
                                {
                                    do
                                    {
                                        dispObj = WaitUntil(inArray[0], out objString, out currTime, "");

                                        if (currTime.Seconds >= 10)
                                        {
                                            dispObj = false;
                                            break;
                                        }
                                    }
                                    while (dispObj != true);

                                    if (dispObj == true)
                                    {
                                        tstObj.tblSelect(tstObj, dispObj, inArray, stpNum, pth, ref tstresult, out fndExcep);
                                    }
                                    else
                                    {
                                        fndExcep = -1;
                                        TextFileOps.Write(pth, objString, -1);
                                    }
                                }

                                thsItem = inArray[2];
                            }
                            catch (Exception e)
                            {
                                //Record failed result
                                tmpString = "Failed clicking the dropdown list box: " + e.Message;
                                tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                //Record exception and begin exit process
                                fndExcep = -1;
                            }

                            break;
                        }
                    #endregion

                    #region vfyGrades
                    case "vfyGrades":
                    {
                        try
                        {
                            //add string to the test results list
                            tmpString = "CHECKPOINT:";
                            tstresult = arrayAppend("True", tmpString, "20", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            inArray = vfyListArray(inArray);

                            //add string to the test results list
                            tmpString = "Searching for the grades table........(" + stpNum + ")";
                            tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //make sure that item exists
                            do
                            {
                                dispObj = WaitUntil(inArray[0], out objString, out currTime, "");

                                if (currTime.Seconds >= 10)
                                {
                                    dispObj = false;
                                    break;
                                }
                            }
                            while (dispObj != true);

                            if (dispObj == true)
                            {
                                tmpString = "Found the grade table......";
                                tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                tstObj.vfyGrades(tstObj, dispObj, inArray, stpNum, pth, ref tstresult, out fndExcep, out tstFail);
                            }
                            else
                            {
                                fndExcep = -1;
                                TextFileOps.Write(pth, objString, -1);
                            }

                            thsItem = inArray[2];

                            tmpString = "Finished checking the gradev table";
                            tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            tmpString = "END CHECKPOINT:";
                            tstresult = arrayAppend("True", tmpString, "20", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            tmpString = "<br />";
                            tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                        }
                        catch (Exception e)
                        {
                            //Record failed result
                            tmpString = "Failed clicking the dropdown list box: " + e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;
                        }
                        break;
                    }
                    #endregion

                    #region vfyTableEntry
                    case "vfyTableEntry":
                    {
                        try
                        {
                            getNeg = inArray[6];

                            tmpString = "CHECKPOINT:";
                            tstresult = arrayAppend("True", tmpString, "20", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            thsItem = inArray[8];
                            inArray = vfyListArray(inArray);

                            tmpString = "Searching for the " + inArray[8] + " table........(" + stpNum + ")";
                            tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //wait for the page to load before verification if expecting a table to be present
                            if (inArray[7] == "N")
                            {
                                do
                                {
                                    dispObj = WaitUntil(inArray[0], out objString, out currTime, inArray[5]);

                                    if (currTime.Seconds >= 10)
                                    {
                                        dispObj = false;
                                        break;
                                    }
                                }
                                while (dispObj != true);

                                if (objString == "")
                                {
                                    tmpString = "Found the " + thsItem + " table......";
                                    tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                    for (int x = 0; x < inArray.Length - 9; x++)
                                    {
                                        if (dispObj == true)
                                        {
                                            tstObj.vfyTableEntry(tstObj, inArray[0], inArray[1], inArray[2], inArray[3], inArray[4], inArray[5],
                                                    inArray[6], inArray[7], inArray[8], inArray[x + 9], datsource, stpNum, pth, ref retXpath, ref tstresult, out fndExcep, out tstFail);
                                        }
                                        else
                                        {
                                            fndExcep = -1;
                                            TextFileOps.Write(pth, objString, -1);
                                        }

                                    }
                                }
                                else
                                {
                                    fndExcep = -1;

                                    objString = "";

                                    tmpString = "The " + thsItem + " item was not found......(Step " + stpNum.ToString() + ")";
                                    tstresult = arrayAppend("True", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                    tstresult = arrayAppend("True", objString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                }
                            }
                            else
                            {
                                tmpString = "Verifying that the  " + thsItem + " table is not present";
                                tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                //TextFileOps.Write(pth, "Found the " + thsItem + " table......", 1);

                                for (int x = 0; x < inArray.Length - 9; x++)
                                {
                                    do
                                    {
                                        dispObj = WaitUntil(inArray[0], out objString, out currTime, inArray[5]);

                                        if (currTime.Seconds >= 10)
                                        {
                                            dispObj = false;
                                            break;
                                        }
                                    }
                                    while (dispObj != true);


                                    if (dispObj == true)
                                    {
                                        tstObj.vfyTableEntry(tstObj, inArray[0], inArray[1], inArray[2], inArray[3], inArray[4], inArray[5],
                                                inArray[6], inArray[7], inArray[8], inArray[x + 9], datsource, stpNum, pth, ref retXpath, ref tstresult, out fndExcep, out tstFail);
                                    }
                                    else
                                    {
                                        fndExcep = -1;
                                        TextFileOps.Write(pth, objString, -1);
                                    }

                                }
                            }

                            tmpString = "Finished checking the " + thsItem + "  item...";
                            tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            tmpString = "END CHECKPOINT:";
                            tstresult = arrayAppend("True", tmpString, "20", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                        }
                        catch (Exception e)
                        {
                            //Record failed result
                            tmpString = "Failed clicking the dropdown list box: " + e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;
                        }
                        break;
                    }
                    #endregion

                    case "vfyTooltip":
                    {
                        try
                        {
                            tblPath = ""; 
                            thsItem = inArray[4];
                            inArray = vfyListArray(inArray);

                            getNeg = inArray[2];

                            tmpString = "CHECKPOINT:";
                            tstresult = arrayAppend("True", tmpString, "20", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            tmpString = "Searching for the tooltip '" + inArray[3] + "'........(" + stpNum + ")";
                            tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //get the table xpath if the tooltip is in a table and/or assign it to tblPath
                            if (inArray[0] != "")
                                vfyTableEntry(tstObj, inArray[0], "/tbody/tr", inArray[2], "Y", "Y", String.Empty, String.Empty, String.Empty, String.Empty, inArray[4],
                                    String.Empty, String.Empty, String.Empty, ref tblPath, ref tstresult, out fndExcep, out tstFail);
                            else
                                tblPath = inArray[0];

                            //verify that the xpath is present
                            do
                            {
                                dispObj = WaitUntil(inArray[0], out objString, out currTime, "");

                                if (currTime.Seconds >= 10)
                                {
                                    dispObj = false;
                                    break;
                                }
                            }
                            while (dispObj != true);

                            if (dispObj == true)
                            {
                                vfyTooltip(tstObj, tblPath, ref tstresult, out fndExcep, out tstFail);
                            }
                            else
                            {
                                tmpString = "The " + thsItem + " tooltip was not found......";
                                tstresult = arrayAppend("True", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                fndExcep = -1;
                            }

                        }
                        catch (Exception e)
                        {
                            //Record failed result
                            tmpString = "Failed clicking the dropdown list box: " + e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;
                        }

                        break;
                    }

                    #region vfyDropdown
                    case "vfyDropdown":
                    {
                        try
                        {

                            thsItem = inArray[4];
                            inArray = vfyListArray(inArray);

                            getNeg = inArray[2];

                            tmpString = "CHECKPOINT:";
                            tstresult = arrayAppend("True", tmpString, "20", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            tmpString = "Searching for the " + inArray[3] + " dropdown........(" + stpNum + ")";
                            tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //wait for the page to load before verification if expecting a table to be present
                            if (inArray[2] != "N" && inArray[2] != "n")
                            {
                                do
                                {
                                    dispObj = WaitUntil(inArray[0], out objString, out currTime, "");

                                    if (currTime.Seconds >= 10)
                                    {
                                        dispObj = false;
                                        break;
                                    }
                                }
                                while (dispObj != true);

                                if (dispObj == true)
                                {
                                    tmpString = "Found the " + thsItem + " table......";
                                    tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                    driver.FindElement(By.XPath(inArray[0])).Click();
                                    for (int x = 0; x < inArray.Length - 4; x++)
                                    {
                                        if (x > 0)
                                        {
                                            itmPath = inArray[0] + "[" + Convert.ToString(x + 1) + "]";
                                        }
                                        else
                                        {

                                            if (inArray[1] != "")
                                            {
                                                itmPath = inArray[0] + "[" + inArray[1] + "]";
                                            }
                                            else
                                            {
                                                itmPath = inArray[0];
                                            }
                                        }

                                        tstObj.vfyObject(tstObj, itmPath, inArray[1], inArray[2], inArray[3], inArray[x + 4],
                                            " dropdown", dispObj, stpNum, pth, ref tstresult, out fndExcep, out tstFail);
                                    }
                                    driver.FindElement(By.XPath(inArray[0])).Click();
                                }
                                else
                                {
                                    tmpString = "The " + thsItem + " dropdown was not found......";
                                    tstresult = arrayAppend("True", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                    fndExcep = -1;
                                }
                            }
                            else
                            {
                                for (int x = 0; x < inArray.Length - 4; x++)
                                {
                                    tstresult = arrayAppend("verify", "table", dispObj.ToString(), inArray[x + 4], inArray[x + 4], inArray[3], String.Empty, String.Empty, String.Empty, tstresult);

                                    if (x > 0)
                                    {
                                        itmPath = inArray[1] + "[" + Convert.ToString(x + 1) + "]";
                                    }
                                    else
                                    {

                                        if (inArray[2] != "")
                                        {
                                            itmPath = inArray[1] + "[" + inArray[2] + "]";
                                        }
                                        else
                                        {
                                            itmPath = inArray[1];
                                        }
                                    }
                                    tstObj.vfyObject(tstObj, itmPath, inArray[1], inArray[2], inArray[3], inArray[x + 4],
                                            " dropdown", dispObj, stpNum, pth, ref tstresult, out fndExcep, out tstFail);
                                }
                            }

                            tmpString = "Finished checking the " + thsItem + "  item...";
                            tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            tmpString = "END CHECKPOINT:";
                            tstresult = arrayAppend("True", tmpString, "20", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                        }
                        catch (Exception e)
                        {
                            //Record failed result
                            tmpString = "Failed clicking the dropdown list box: " + e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;
                        }
                        break;
                    }
                    #endregion

                    #region vfyButton
                    case "vfyButton":
                    {
                        try
                        {
                            thsItem = inArray[4];
                            inArray = vfyListArray(inArray);

                            getNeg = inArray[2];

                            tmpString = "CHECKPOINT:";
                            tstresult = arrayAppend("True", tmpString, "20", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            tmpString = "Searching for the " + inArray[2] + " button........(" + stpNum + ")";
                            tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //section off the scenarios if the field is (!=N) or is not (==N) present on the screen
                            if (inArray[2] != "N" && inArray[2] != "n")
                            {
                                do
                                {
                                    dispObj = WaitUntil(inArray[0], out objString, out currTime, "");

                                    if (currTime.Seconds >= 10)
                                    {
                                        dispObj = false;
                                        break;
                                    }
                                }
                                while (dispObj != true);

                                if (dispObj == true)
                                {
                                    tmpString = "Found the " + thsItem + " button......";
                                    tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                    //TextFileOps.Write(pth, "Found the " + thsItem + " item......", 1);

                                    //driver.FindElement(By.XPath(inArray[0])).Click();
                                    for (int x = 0; x < inArray.Length - 4; x++)
                                    {
                                        if (x > 0)
                                        {
                                            itmPath = inArray[0] + "[" + Convert.ToString(x + 1) + "]";
                                        }
                                        else
                                        {

                                            if (inArray[1] != "")
                                            {
                                                itmPath = inArray[0] + "[" + inArray[1] + "]";
                                            }
                                            else
                                            {
                                                itmPath = inArray[0];
                                            }
                                        }

                                        tstObj.vfyObject(tstObj, itmPath, inArray[1], inArray[2], inArray[3], inArray[x + 4],
                                            " button", dispObj, stpNum, pth, ref tstresult, out fndExcep, out tstFail);
                                    }
                                }
                                else
                                {
                                    tmpString = "The " + thsItem + " button was not found......";
                                    tstresult = arrayAppend("True", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                    fndExcep = -1;
                                }
                            }
                            else
                            {
                                tmpString = "Verifying that the  " + thsItem + " button is not present";
                                tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                for (int x = 0; x < inArray.Length - 4; x++)
                                {
                                    if (x > 0)
                                    {
                                        itmPath = inArray[1] + "[" + Convert.ToString(x + 1) + "]";
                                    }
                                    else
                                    {

                                        if (inArray[1] != "")
                                        {
                                            itmPath = inArray[0] + "[" + inArray[1] + "]";
                                        }
                                        else
                                        {
                                            itmPath = inArray[0];
                                        }
                                    }
                                    tstObj.vfyObject(tstObj, itmPath, inArray[1], inArray[2], inArray[3], inArray[x + 4],
                                                " button", dispObj, stpNum, pth, ref tstresult, out fndExcep, out tstFail);
                                }
                            }

                            tmpString = "Finished checking the " + thsItem + "  button...";
                            tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            tmpString = "END CHECKPOINT:";
                            tstresult = arrayAppend("True", tmpString, "20", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                        }
                        catch (Exception e)
                        {
                            //Record failed result
                            tmpString = "Failed clicking the dropdown list box: " + e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;
                        }
                        break;
                    }
                    #endregion

                    #region vfyImages
                    case "vfyImages":
                    {
                        try
                        {
                            thsItem = inArray[4];
                            inArray = vfyListArray(inArray);

                            getNeg = inArray[2];

                            tmpString = "CHECKPOINT:";
                            tstresult = arrayAppend("True", tmpString, "20", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            tmpString = "Searching for the " + inArray[2] + " field........(" + stpNum + ")";
                            tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //section off the scenarios if the field is (!=N) or is not (==N) present on the screen
                            if (inArray[2] != "N" && inArray[2] != "n")
                            {
                                do
                                {
                                    dispObj = WaitUntil(inArray[0], out objString, out currTime, "");

                                    if (currTime.Seconds >= 10)
                                    {
                                        dispObj = false;
                                        break;
                                    }
                                }
                                while (dispObj != true);

                                if (dispObj == true)
                                {
                                    tmpString = "Found the " + thsItem + " button......";
                                    tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                    //driver.FindElement(By.XPath(inArray[0])).Click();
                                    for (int x = 0; x < inArray.Length - 4; x++)
                                    {
                                        if (x > 0)
                                        {
                                            itmPath = inArray[0] + "[" + Convert.ToString(x + 1) + "]";
                                        }
                                        else
                                        {

                                            if (inArray[1] != "")
                                            {
                                                itmPath = inArray[0] + "[" + inArray[1] + "]";
                                            }
                                            else
                                            {
                                                itmPath = inArray[0];
                                            }
                                        }

                                        tstObj.vfyObject(tstObj, itmPath, inArray[1], inArray[2], inArray[3], inArray[x + 4],
                                            " image", dispObj, stpNum, pth, ref tstresult, out fndExcep, out tstFail);
                                    }
                                    //driver.FindElement(By.XPath(inArray[0])).Click();
                                }
                                else
                                {
                                    TextFileOps.Write(pth, "The " + thsItem + " image was not found......", -1);
                                    fndExcep = -1;
                                }
                            }
                            else
                            {
                                TextFileOps.Write(pth, "Verifying that the  " + thsItem + " table is not present", 1);
                                for (int x = 0; x < inArray.Length - 4; x++)
                                {
                                    if (x > 0)
                                    {
                                        itmPath = inArray[0] + "[" + Convert.ToString(x + 1) + "]";
                                    }
                                    else
                                    {

                                        if (inArray[1] != "")
                                        {
                                            itmPath = inArray[0] + "[" + inArray[1] + "]";
                                        }
                                        else
                                        {
                                            itmPath = inArray[0];
                                        }
                                    }
                                    tstObj.vfyObject(tstObj, itmPath, inArray[1], inArray[2], inArray[3], inArray[x + 4],
                                                " image", dispObj, stpNum, pth, ref tstresult, out fndExcep, out tstFail);
                                }
                            }

                            tmpString = "Finished checking the " + thsItem + "  image...";
                            tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            tmpString = "END CHECKPOINT:";
                            tstresult = arrayAppend("True", tmpString, "20", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                        }
                        catch (Exception e)
                        {
                            //Record failed result
                            tmpString = "Failed clicking the dropdown list box: " + e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;
                        }
                        break;
                    }
                    #endregion

                    #region vfyField
                    case "vfyField":
                    {
                        try
                        {
                            thsItem = inArray[4];
                            inArray = vfyListArray(inArray);

                            getNeg = inArray[2];

                            tmpString = "CHECKPOINT:";
                            tstresult = arrayAppend("True", tmpString, "20", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            tmpString = "Searching for the " + inArray[3] + " field........(" + stpNum + ")";
                            tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //section off the scenarios if the field is (!=N) or is not (==N) present on the screen
                            if (inArray[2] != "N" && inArray[2] != "n")
                            {
                                do
                                {
                                    dispObj = WaitUntil(inArray[0], out objString, out currTime, "");

                                    if (currTime.Seconds >= 10)
                                    {
                                        dispObj = false;
                                        break;
                                    }
                                }
                                while (dispObj != true);

                                if (dispObj == true)
                                {
                                    tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                    //driver.FindElement(By.XPath(inArray[0])).Click();
                                    for (int x = 0; x < inArray.Length - 4; x++)
                                    {
                                        if (x > 0)
                                        {
                                            itmPath = inArray[0] + "[" + Convert.ToString(x + 1) + "]";
                                        }
                                        else
                                        {
                                            itmPath = inArray[0];
                                        }

                                        tstObj.vfyObject(tstObj, itmPath, inArray[1], inArray[2], inArray[3], inArray[x + 4],
                                            " field", dispObj, stpNum, pth, ref tstresult, out fndExcep, out tstFail);
                                    }
                                    //driver.FindElement(By.XPath(inArray[0])).Click();
                                }
                                else
                                {
                                    tmpString = "The " + thsItem + " image was not found......";
                                    tstresult = arrayAppend("True", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                    fndExcep = -1;
                                }
                            }
                            else
                            {
                                tmpString = "Verifying that the  " + thsItem + " table is not present";
                                tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                for (int x = 0; x < inArray.Length - 4; x++)
                                {
                                    if (x > 0)
                                    {
                                        itmPath = inArray[0] + "[" + Convert.ToString(x + 1) + "]";
                                    }
                                    else
                                    {

                                        if (inArray[2] != "")
                                        {
                                            itmPath = inArray[1] + "[" + inArray[2] + "]";
                                        }
                                        else
                                        {
                                            itmPath = inArray[1];
                                        }
                                    }

                                    tstObj.vfyObject(tstObj, itmPath, inArray[1], inArray[2], inArray[3], inArray[x + 4],
                                            " field", dispObj, stpNum, pth, ref tstresult, out fndExcep, out tstFail);
                                }
                            }

                            tmpString = "Finished checking the " + thsItem + "  field...";
                            tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            tmpString = "END CHECKPOINT:";
                            tstresult = arrayAppend("True", tmpString, "20", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                        }
                        catch (Exception e)
                        {
                            //Record failed result
                            tmpString = e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;
                        }
                        break;
                    }
                    #endregion

                    #region vfyOutcome
                    case "vfyOutcome":
                    {
                        try
                        {
                            thsItem = "";
                            inArray = vfyListArray(inArray);

                            tmpString = "CHECKPOINT:";
                            tstresult = arrayAppend("True", tmpString, "20", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            tmpString = "Checking the student for scores ........(" + stpNum + ")";
                            tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            do
                            {
                                dispObj = WaitUntil(inArray[0], out objString, out currTime, "");

                                if (currTime.Seconds >= 10)
                                {
                                    dispObj = false;
                                    break;
                                }
                            }
                            while (dispObj != true);

                            if (dispObj == true)
                            {

                                //driver.FindElement(By.XPath(inArray[0])).Click();
                                for (int x = 0; x < inArray.Length - 9; x++)
                                {
                                    thsItem = inArray[x + 9];

                                    tmpString = "Searching for " + thsItem + "'s scores";
                                    tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                    tmpString = "Found " + thsItem + "'s row in the score table......";
                                    tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                    if (x > 0)
                                    {
                                        itmPath = inArray[0] + "[" + Convert.ToString(x + 1) + "]";
                                    }
                                    else
                                    {
                                        itmPath = inArray[0];
                                    }

                                    tstObj.vfyOutcome(tstObj, inArray[0], inArray, inArray[x + 9], dispObj, stpNum, pth, ref arrOutcome, ref tstresult, out fndExcep, out tstFail);

                                    tmpString = "Finished checking the " + thsItem + "  field...";
                                    tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                    tmpString = "<br />";
                                    tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                } 
                                
                                for (int a = 0; a < arrOutcome.GetLength(0); a++)
                                {
                                    tstresult = arrayAppend("verify", "weight", arrOutcome[a, 0], arrOutcome[a, 1], String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                }
                            }
                            else
                            {
                                tmpString = "The " + thsItem + " image was not found......";
                                tstresult = arrayAppend("True", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                fndExcep = -1;
                            }

                            tmpString = "END CHECKPOINT:";
                            tstresult = arrayAppend("True", tmpString, "20", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                        }
                        catch (Exception e)
                        {
                            //Record failed result
                            tmpString = e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;
                        }
                        break;
                    }
                    #endregion

                    #region vfyText
                    case "vfyText":
                    {
                        try
                        {
                            thsItem = inArray[4];
                            inArray = vfyListArray(inArray);

                            getNeg = inArray[2];

                            tmpString = "CHECKPOINT:";
                            tstresult = arrayAppend("True", tmpString, "20", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            tmpString = "Searching for the " + inArray[2] + " text........(" + stpNum + ")";
                            tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //section off the scenarios if the field is (!=N) or is not (==N) present on the screen
                            if (inArray[1] != "N" && inArray[1] != "n")
                            {
                                do
                                {
                                    dispObj = WaitUntil(inArray[0], out objString, out currTime, "");

                                    if (currTime.Seconds >= 10)
                                    {
                                        dispObj = false;
                                        break;
                                    }
                                }
                                while (dispObj != true);

                                if (dispObj == true)
                                {
                                    tmpString = "Found the " + thsItem + " text......";
                                    tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                    for (int x = 0; x < inArray.Length - 4; x++)
                                    {
                                        if (x > 0)
                                        {
                                            itmPath = inArray[0] + "[" + Convert.ToString(x + 1) + "]";
                                        }
                                        else
                                        {

                                            if (inArray[1] != "")
                                            {
                                                itmPath = inArray[0] + "[" + inArray[1] + "]";
                                            }
                                            else
                                            {
                                                itmPath = inArray[0];
                                            }
                                        }

                                        tstObj.vfyObject(tstObj, itmPath, inArray[1], inArray[2], inArray[3], inArray[x + 4],
                                            " text", dispObj, stpNum, pth, ref tstresult, out fndExcep, out tstFail);
                                    }
                                }
                                else
                                {
                                    tmpString = "The " + thsItem + " text was not found......";
                                    tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                    fndExcep = -1;
                                }
                            }
                            else
                            {
                                tmpString = "Verifying that the  " + thsItem + " text is not present";
                                tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                            }

                            tmpString = "Finished checking the " + thsItem + "  text...";
                            tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            tmpString = "END CHECKPOINT:";
                            tstresult = arrayAppend("True", tmpString, "20", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                        }
                        catch (Exception e)
                        {
                            //Record failed result
                            tmpString = e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;
                        }
                        break;
                    }
                    #endregion

                    #region vfyLink
                    case "vfyLink":
                    {
                        try
                        {
                            thsItem = inArray[4];
                            inArray = vfyListArray(inArray);

                            getNeg = inArray[2];

                            tmpString = "CHECKPOINT:";
                            tstresult = arrayAppend(dispObj.ToString(), tmpString, "20", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            tmpString = "Searching for the " + inArray[2] + " button........(" + stpNum + ")";
                            tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //TextFileOps.Write(pth, "CHECKPOINT:", 20);
                            //Recorder.vfyNav(true, thsItem, " field", stpNum, pth);

                            //section off the scenarios if the field is (!=N) or is not (==N) present on the screen
                            if (inArray[1] != "N" && inArray[1] != "n")
                            {
                                do
                                {
                                    dispObj = WaitUntil(inArray[0], out objString, out currTime, inArray[3]);
                                }
                                while (dispObj != true);

                                if (dispObj == true)
                                {
                                    tmpString = "Found the " + thsItem + " link......";
                                    tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                    driver.FindElement(By.XPath(inArray[0])).Click();
                                    for (int x = 0; x < inArray.Length - 4; x++)
                                    {
                                        if (x > 0)
                                        {
                                            itmPath = inArray[1] + "[" + Convert.ToString(x + 1) + "]";
                                        }
                                        else
                                        {

                                            if (inArray[2] != "")
                                            {
                                                itmPath = inArray[1] + "[" + inArray[2] + "]";
                                            }
                                            else
                                            {
                                                itmPath = inArray[1];
                                            }
                                        }

                                        tstObj.vfyObject(tstObj, itmPath, inArray[1], inArray[2], inArray[3], inArray[x + 4],
                                            " link", dispObj, stpNum, pth, ref tstresult, out fndExcep, out tstFail);
                                    }
                                    driver.FindElement(By.XPath(inArray[0])).Click();
                                }
                                else
                                {
                                    tmpString = "The " + thsItem + " link was not found......";
                                    tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                    fndExcep = -1;
                                }
                            }
                            else
                            {
                                tmpString = "Verifying that the  " + thsItem + " link is not present";
                                tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                for (int x = 0; x < inArray.Length - 4; x++)
                                {
                                    if (x > 0)
                                    {
                                        itmPath = inArray[1] + "[" + Convert.ToString(x + 1) + "]";
                                    }
                                    else
                                    {

                                        if (inArray[2] != "")
                                        {
                                            itmPath = inArray[1] + "[" + inArray[2] + "]";
                                        }
                                        else
                                        {
                                            itmPath = inArray[1];
                                        }
                                    }
                                    tstObj.vfyObject(tstObj, itmPath, inArray[1], inArray[2], inArray[3], inArray[x + 4],
                                            " link", dispObj, stpNum, pth, ref tstresult, out fndExcep, out tstFail);
                                }
                            }

                            tmpString = "Finished checking the " + thsItem + "  text...";
                            tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            tmpString = "END CHECKPOINT:";
                            tstresult = arrayAppend("True", tmpString, "20", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                        }
                        catch (Exception e)
                        {
                            //Record failed result
                            tmpString = e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;
                        }
                        break;
                    }
                    #endregion

                    #region Wait
                    case "Wait":
                    {
                        try
                        {
                            tstObj.Wait(inArray[0]);

                            tstresult = arrayAppend("True", "Waiting " + inArray[0] + " seconds........(" + stpNum + ")", "80", String.Empty, String.Empty, String.Empty, String.Empty,
                                String.Empty, String.Empty, tstresult);
                        }
                        catch (Exception e)
                        {
                            //Record failed result
                            tmpString = e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;
                        }
                        break;
                    }
                    #endregion

                    #region default
                    default:
                        fndExcep = -1;
                        TextFileOps.Write(pth, "Step (" + stpNum + "), " + doFunc + " is not present in the testing app", -1);
                        break;
                    #endregion
                }
            }
            catch (Exception e)
            {
                tstresult = arrayAppend("False", e.Message, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                fndExcep = -1;
            }
        }
        #endregion

        #endregion

        #region Private Functions

            #region vfyListArray
            private string[] vfyListArray(string[] inArray)
            {
                string[] outArray;

                outArray = new string[0];

                //trim any blank spaces from the array
                //get compArray length
                for (int x = inArray.Length - 1; x >= 0; x--)
                {
                    if (inArray[x] != String.Empty)
                    {
                        outArray = new string[(inArray.Length - (inArray.Length - x)) + 1];
                        break;
                    }
                }

                //populate compArray
                for (int x = 0; x < outArray.Length; x++)
                {
                    outArray[x] = inArray[x];
                }

                return outArray;
            }
            #endregion

            #region WaitUntil
            private bool WaitUntil(string inPath, out string outString, out TimeSpan elapsedTime, string objVis)
            {
                Stopwatch stopwatch;
                bool objPres;
                outString = "";
                objPres = false;

                stopwatch = new Stopwatch();
                stopwatch.Start();
                do
                {
                    try
                    {
                        objPres = driver.FindElement(By.XPath(inPath)).Displayed;
                    }
                    catch (Exception e)
                    {
                        if (objVis == "N")
                        {
                            objPres = true;
                        }
                        else
                        {
                            objPres = false;
                            outString = e.Message;
                        }


                    }

                    elapsedTime = stopwatch.Elapsed;

                    if (elapsedTime.Seconds >= 10)
                    {
                        objPres = false;
                        break;
                    }
                } while (objPres != true);

                stopwatch.Stop();
                return objPres;
            }
            #endregion

            #region objVerify
            private bool objVerify(string vfyString, string fldName, string getNeg, ref int tstFail)
            {
                bool objPres;

                if (vfyString == fldName.Trim() && getNeg != "N")
                {
                    objPres = true;
                    tstFail = 0;
                }
                else if (vfyString == fldName.Trim() && getNeg == "N")
                {
                    objPres = true;
                    tstFail = -1;
                }
                else if (vfyString != fldName.Trim() && getNeg == "N")
                {
                    objPres = false;
                    tstFail = 0;
                }
                else
                {
                    objPres = false;
                    tstFail = -1;
                }

                return objPres;
            }
            #endregion
        
            #region arrayAppend
            private string[,] arrayAppend(string arg0, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string[,] inList)
            {
                string[,] outList;

                //set the outlist array
                if (inList.GetLength(0) == 1 && inList[0, 0] == null)
                    outList = new string[1, inList.GetLength(1)];
                else
                    outList = new string[inList.GetLength(0) + 1, inList.GetLength(1)];

                if (outList.GetLength(0) == 1 && inList[0, 0] == null)
                {
                    outList[0, 0] = arg0;
                    outList[0, 1] = arg1;
                    outList[0, 2] = arg2;
                    outList[0, 3] = arg3;
                    outList[0, 4] = arg4;
                    outList[0, 5] = arg5;
                    outList[0, 6] = arg6;
                    outList[0, 7] = arg7;
                    outList[0, 8] = arg8;
                }
                else
                {
                    //populate outList
                    for (int x = 0; x < outList.GetLength(0); x++)
                    {
                        if (x < outList.GetLength(0) - 1 && inList[0, 0] != null)
                        {
                            outList[x, 0] = inList[x, 0];
                            outList[x, 1] = inList[x, 1];
                            outList[x, 2] = inList[x, 2];
                            outList[x, 3] = inList[x, 3];
                            outList[x, 4] = inList[x, 4];
                            outList[x, 5] = inList[x, 5];
                            outList[x, 6] = inList[x, 6];
                            outList[x, 7] = inList[x, 7];
                            outList[x, 8] = inList[x, 8];
                        }

                        else
                        {
                            outList[x, 0] = arg0;
                            outList[x, 1] = arg1;
                            outList[x, 2] = arg2;
                            outList[x, 3] = arg3;
                            outList[x, 4] = arg4;
                            outList[x, 5] = arg5;
                            outList[x, 6] = arg6;
                            outList[x, 7] = arg7;
                            outList[x, 8] = arg8;
                        }
                    }
                }
                return outList;
            }
            #endregion

            #region getGrade
            private string getGrade(int inCol)
            {
                string outGrade;

                outGrade = "";

                switch (inCol)
                {
                    case 2:
                        outGrade = "A+";
                        break;
                    case 3:
                        outGrade = "A";
                        break;
                    case 4:
                        outGrade = "A-";
                        break;
                    case 5:
                        outGrade = "B+";
                        break;
                    case 6:
                        outGrade = "B";
                        break;
                    case 7:
                        outGrade = "B-";
                        break;
                    case 8:
                        outGrade = "C+";
                        break;
                    case 9:
                        outGrade = "C";
                        break;
                    case 10:
                        outGrade = "C-";
                        break;
                    case 11:
                        outGrade = "D+";
                        break;
                    case 12:
                        outGrade = "D";
                        break;
                    case 13:
                        outGrade = "D-";
                        break;
                    case 14:
                        outGrade = "F";
                        break;
                    case 15:
                        outGrade = "INC";
                        break;
                }

                return outGrade;
            }
            #endregion

        #endregion
    }
    #endregion
}
#endregion
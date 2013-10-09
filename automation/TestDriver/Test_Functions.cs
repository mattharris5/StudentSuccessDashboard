using Excel = Microsoft.Office.Interop.Excel;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Internal;
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
            public tstObject(int typNum, ref string profilePath, string baseURL)
            {

                brwsrType = typNum;

                switch (typNum)
                {
                    case 0:
                    {
                        SafariOptions opt1 = new SafariOptions();
                        opt1.CustomExtensionPath = Application.StartupPath + "\\SafariDriver2.32.0.safariextz";
                        opt1.SkipExtensionInstallation = false;

                        driver = new SafariDriver(opt1);
                        break;
                    }
                    case 1:
                    {
                        string runProfile = Application.StartupPath + "\\Firefox Profile";
                        string firebugPath = Application.StartupPath + "\\Firefox Profile\\firebug-1.9.2.xpi";
                        FirefoxProfile profile = new FirefoxProfile(runProfile);

                        //add firebug to the profile
                        //profile.AddExtension(firebugPath);

                        //add firePath to the profile
                        profile.AddExtension(firebugPath);

                        //set the webdriver_assume_untrusted_issuer to false
                        profile.SetPreference("webdriver_assume_untrusted_issuer", false);

                        //run the profile
                        driver = new FirefoxDriver(profile);

                        //maximize window
                        driver.Manage().Window.Maximize();

                        //Wait 4 seconds for an item to appear
                        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(1));
                        break;
                    }
                    //create a Chrome object
                    case 2:
                    {
                        var options = new ChromeOptions();

                        //set the startup options to start maximzed
                        options.AddArguments("start-maximized");

                        //start Chrome maximized
                        driver = new ChromeDriver(@Application.StartupPath, options);

                        //Wait 10 seconds for an item to appear
                        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                        break;
                    }

                    //create an IE object
                    case 3:
                    {
                        //var options = new InternetExplorerOptions();

                        //set the startup options to start maximzed
                        //options.ToCapabilities();

                        driver = new InternetExplorerDriver(@Application.StartupPath);

                        //maximize window
                        driver.Manage().Window.Maximize();

                        //Wait 4 seconds for an item to appear
                        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(1));

                        break;
                    }
                }
            }
            #endregion

            #region Close
            public void Close()

            {
                try
                {
                    driver.Close();
                }
                catch (Exception e)
                {
                    //add string to the test results list
                    tmpString = e.Message;

                }
            }
            #endregion

            #region Quit
            public void Quit()
            {
                driver.Quit();
            }
            #endregion

            #region Login
            public void Login(tstObject tstObj, string[] inArray, string baseURL, string stpNum, int browser, string product, string pth, out int fndExcep, ref string[,] tstresult)
            {
                bool objPres;
                int isPres;
                string btnLogin;
                string lnkLogout;
                string typLogin;
                string username;
                string password;
                IJavaScriptExecutor js = driver as IJavaScriptExecutor;
                WebDriverWait wait;

                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                //initialize variables
                fndExcep = 0;
                typLogin = "";
                username = "";
                password = "";
                objPres = false;

                //Navigate to the baseURL
                driver.Navigate().GoToUrl(baseURL + "/");

                switch(product)
                {
                    case "Echo":
                    {
                        try
                        {
                            if (browser == 2 || browser == 0)
                            {
                                lnkLogout = "/html/body/table/tbody/tr/td/table/tbody/tr/td/div/div[3]/table/tbody/tr/td[5]/div/a";
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
                            driver.FindElement(By.XPath("/html/body/div[3]/table/tbody/tr[2]/td[2]/table/tbody/tr/td[2]/div[2]/div/form/div" +
                                "/div/div[2]/div/div/div/div/div/div/div/div/table/tbody/tr/td/input")).Clear();

                            //Populate the Username field with the username parameter sent from the data sheet
                            driver.FindElement(By.XPath("/html/body/div[3]/table/tbody/tr[2]/td[2]/table/tbody/tr/td[2]/div[2]/div/form/div" +
                                "/div/div[2]/div/div/div/div/div/div/div/div/table/tbody/tr/td/input")).SendKeys(username);

                            Thread.Sleep(250);

                            isPres = driver.FindElements(By.XPath("id('text')")).Count;

                            if (isPres > 0)
                            {
                                //click the password field. This will change the input type
                                driver.FindElement(By.XPath("id('text')")).Click();

                                js.ExecuteScript("swapInput()");
                            }

                            //Clear the password field
                            driver.FindElement(By.XPath("/html/body/div[3]/table/tbody/tr[2]/td[2]/table/tbody/tr/td[2]/div[2]/div/form/div" +
                                "/div/div[2]/div/div/div/div/div/div/div/div/table/tbody/tr[3]/td/input")).Clear();

                            //Populate the Password field with the password parameter sent from the data sheet
                            driver.FindElement(By.XPath("/html/body/div[3]/table/tbody/tr[2]/td[2]/table/tbody/tr/td[2]/div[2]/div/form/div" +
                                "/div/div[2]/div/div/div/div/div/div/div/div/table/tbody/tr[3]/td/input")).SendKeys(password);

                            //Click the Sign On button 
                            // driver.FindElement(By.XPath("id('edit-pass')")).SendKeys("{TAB}");
                            objPres = tstObj.IsElementPresent(By.Id("edit-submit"));
                            driver.FindElement(By.Id("edit-submit")).Click();

                            if (browser != 0)
                            {
                                //Verify login was successful
                                objPres = driver.FindElement(By.LinkText("Home")).Enabled;
                            }

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

                        break;
                    }
                    case "Strive - SSD":
                    {
                        try
                        {
                            //assign variables from the incoming inArray from the database
                            for (int x = 0; x < inArray.Length; x++)
                            {
                                if (inArray[x] != "")
                                {
                                    switch (x)
                                    {
                                        case 0:
                                            username = inArray[x];
                                            break;
                                        case 1:
                                            password = inArray[x];
                                            break;
                                        case 2:
                                            typLogin = inArray[x];
                                            break;
                                    }
                                }
                            }

                            //click the appropriate login button
                            switch (typLogin)
                            {
                                case "live":
                                    //click the login type button
                                    driver.FindElement(By.XPath("/html/body/div/div/div/div[2]/aside/div/div/div/ul/li/button")).Click();

                                    //clear the username field
                                    driver.FindElement(By.XPath("/html/body/div[2]/div/div/div/div[2]/div[5]/div/form/div/div[4]/div/input")).Clear();

                                    //enter the username
                                    driver.FindElement(By.XPath("/html/body/div[2]/div/div/div/div[2]/div[5]/div/form/div/div[4]/div/input")).SendKeys(username);

                                    //clear the password field
                                    driver.FindElement(By.XPath("/html/body/div[2]/div/div/div/div[2]/div[5]/div/form/div/div[6]/div/input")).Clear();

                                    //enter the password
                                    driver.FindElement(By.XPath("/html/body/div[2]/div/div/div/div[2]/div[5]/div/form/div/div[6]/div/input")).SendKeys(password);

                                    //click the Sign In button
                                    driver.FindElement(By.XPath("/html/body/div[2]/div/div/div/div[2]/div[5]/div/form/div[2]/input")).Click();
                                    break;
                                case "gmail":
                                    driver.FindElement(By.XPath("/html/body/div/div/div/div[2]/aside/div/div/div/ul/li[2]/button"));
                                    break;
                                case "yahoo":
                                    driver.FindElement(By.XPath("/html/body/div/div/div/div[2]/aside/div/div/div/ul/li[3]/button"));
                                    break;

                            }

                            

                            //log result to the result file
                            tmpString = "Logging into Echo as " + username + ": (" + stpNum + ")";
                            tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                        }
                        catch(Exception e)
                        {
                            //Record failed result
                            tmpString = e.Message;
                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                            //Record exception and begin exit process
                            fndExcep = -1;
                        }
                        break;
                    }
                }
            
            }
            #endregion

            #region Logout
            //Click the Log Out link
            public void Logout(string pth, bool objPres, string stpNum, string product, ref string[,] tstresult)
            {
                int fndLink;
                int count;
                string fndList;
                DateTime startTime;
                DateTime currTime; 
                ReadOnlyCollection<IWebElement> lnkList;
                TimeSpan waitTime;
                fndLink = 0;
            
                //get the time that control entered this function as a baseline
                startTime = DateTime.Now;

                switch(product)
                {
                    case "Echo":
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

                        break;

                    case "Strive - SSD":
                        driver.FindElement(By.XPath("/html/body/div/a")).Click();

                        //Record result to result file
                        tmpString = "Searching for the Logout link........(" + stpNum + ")";
                        tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                        
                        //get count of items in the dropdown
                        count = driver.FindElements(By.XPath("/html/body/div/ul/li")).Count;

                        //load all of the dropdown elements into a ReadOnly Collection of IWebElements
                        lnkList = driver.FindElements(By.XPath("/html/body/div/ul/li"));

                        for (int x = 0; x < count; x++)
                        {
                            if (lnkList[x].Text == "Logout")
                            {
                                //increment one place on the list because the list of dropdown items and array list
                                //are not based the same. The array list is 0-based and the lnkList is not
                                if ((x + 1) == 1)
                                    fndList = "/html/body/div/ul/li";
                                else
                                    fndList = "/html/body/div/ul/li[" + (x + 1).ToString() + "]";

                                //Record result
                                tmpString = "Found list item [Logout]";
                                tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                //Click the selected list item
                                lnkList[x].FindElement(By.XPath(fndList)).Click();
                                fndLink = 1;


                                //Record successful action in result file
                                tmpString = "Successfully the Logout link";
                                tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                //TextFileOps.Write(pth, "Successfully selected item [" + lstItem + "]", 1);
                            }
                        }
                        break;
                }
                //add string to the test results list
                tmpString = "Logging out of Echo.........." + " (" + stpNum + ")";
                tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
            }
            #endregion

            #region addOutcome
            public void addOutcome(tstObject tstObj, bool objPres, string[] inArray, string stpNum, string pth, ref string[,] tstresult, out int fndExcep, out int tstFail)
            {
                int dataLen;
                int dataCount;
                int fldNum;
                int numOutcomes;            //the number of outcomes present on the Course Gradebook Setup page 
                string btnXpath1;
                string btnXpath2;
                string txtField;
                string ocTablePath;
                string[,] outcomeData;
                IWebElement thsField;

                dataLen = 0;
                fldNum = 0;
                dataCount = 0;
                tstFail = 0;
                fndExcep = 0;

                //initialize variables
                btnXpath1 = "/html/body/table/tbody/tr/td/table[2]/tbody/tr[2]/td[2]/table/tbody/tr/td/div/form/div[4]/div[2]/div[3]/table/tbody/tr/th/a";
                btnXpath2 = "/html/body/div[4]/div[2]/form/input";
                txtField = "id('txt_" + fldNum.ToString() + "')";
                ocTablePath = "/html/body/table/tbody/tr/td/table[2]/tbody/tr[2]/td[2]/table/tbody/tr/td/div/form/div[4]/div[2]/div[3]/table/tbody/tr";

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

                //eliminate all pre-populated outcomes if present
                //get the number of outcomes present (subtract 2 to account for the header row, which is a tr and the Add Outcome button)
                numOutcomes = (driver.FindElements(By.XPath(ocTablePath)).Count) - 2;

                //if numOutcomes > 2 there are outcomes that need to be deleted
                if (numOutcomes > 1)
                {
                    for (int cntOutcome = 2; cntOutcome <= numOutcomes + 1; cntOutcome++)
                    {
                        driver.FindElement(By.XPath(ocTablePath + "[" +
                             (cntOutcome).ToString() + "]/td[4]/a/img")).Click();
                    }

                    //driver.FindElement(By.XPath("id('outcome_0')/td[2]/input[2]")).Clear();
                }


                try
                {
                    //add string to the test results list
                    tmpString = "Clicking the Add Outcome button......"; ;
                    tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);


                    for (int x = 0; x < outcomeData.GetLength(0); x++)
                    {
                        //check if object exists
                        objPres = driver.FindElement(By.XPath(btnXpath1)).Enabled;

                        //click the Add Outcome button
                        driver.FindElement(By.XPath(btnXpath1)).Click();

                        //enter the outcome and click the submit button
                        thsField = driver.FindElement(By.XPath(btnXpath2));
                        thsField.SendKeys(outcomeData[x, 0]);
                        driver.FindElement(By.XPath("/html/body/div[4]/div[2]/form/input[2]")).Click();

                        thsField = driver.FindElement(By.XPath(ocTablePath + "[" + (x + (numOutcomes + 2)).ToString() + "]/td[2]/input"));
                        thsField.SendKeys(outcomeData[x,1]);
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

            #region calendarControl
            public void calendarControl(tstObject tstObj, string[] inArray, string stpNum, string pth, ref string[,] tstresult, out int fndExcep)
            {
                bool objPres;               //boolean that determines if the calendar control is present
                string thsDate;             //the date the control is looking at. Compared against the date to select
                string clkPath;             //the path of the matching item to be clicked
                string activeTab;           //the active tab on the app. used to determine how to handle each control
                string clkCntrl;            //xpath to the calendar icon on the page
                string calXpath;            //xpath to the calendar control
                string nmCalendar;          //the name of the calendar control to output to the results file 
                string isPopup;             //flag var that determines if the calendar is a popup of 
                string inPath;
                string direction;           //the direction to move from today's date for a sliding date 
                string numDays;             //the number of days to move forward or backwards for a sliding date
                string totDirection;        //combined direction and numDays for a sliding date
                string mthXpath;            //month xpath suffix - leads to the calendar control header 
                string dateXpath;           //date xpath suffix - leads to the calendar control date table
                string dispMth;             //the initial month displayed on the calendar control when it opens (current month)
                int intMth;                 //int value of a month string (e.g. January = 1; February = 2, etc.)
                int diffMth;                //the difference between the initial month displayed and the month to find  
                int dispYr;                 //the initial year displayed on the calendar control when it opens(current year)
                int yr;                     //year var extracted from date sent in
                int mth;                    //month var extracted from date sent in
                int day;                    //date var extracted from date sent in
                int numRows;                //the number of table rows containing dates in a month
                int fndItem;                //flag variable noting that the item has been found in the calendar control
                DateTime inDate;            //converted date from inArray into a DateTime variable
                DateTime newDate;           //sliding date from input 
                DateTime tgtMonth;          //the target month to navigate to onbce the calendar control opens

                //initialize variables and set locals from inArray
                fndExcep = 0;
                fndItem = 0;
                objPres = true;
                calXpath = inArray[0];
                mthXpath = inArray[1];
                dateXpath = inArray[2];
                clkCntrl = inArray[3];
                isPopup = inArray[4];
                nmCalendar = inArray[5];

                try
                {
                    //get the first two letter of inArray. A value of 'TO' or 'YE' means a sliding value
                    //of TODAY, TOMORROW, OR YESTERDAY. The app will need to convert this to a DateTime var
                if (inArray[6].Substring(0, 2) == "TO" || inArray[6].Substring(0, 2) == "YE")
                    {
                        //extract the right day and time from sliding date input. The default is (TODAY + xx)
                        switch (inArray[6])
                        {
                            case "TODAY":
                            {
                                yr = DateTime.Now.Year;
                                mth = DateTime.Now.Month;
                                day = DateTime.Now.Day;
                                break;
                            }
                            case "TOMORROW":
                            {
                                //format a sliding date to today's date to start from
                                newDate = DateTime.ParseExact(inArray[6], "MM/dd/yyyy", null);

                                //add one day to to get the new date.
                                inDate = newDate.AddDays(1);

                                //get the string variable to perform calendar control operations
                                yr = inDate.Year;
                                mth = inDate.Month;
                                day = inDate.Day;

                                break;
                            }
                            case "YESTERDAY":
                            {
                                //format a sliding date to today's date to start from
                                newDate = DateTime.ParseExact(inArray[6], "MM/dd/yyyy", null);

                                //add one day to to get the new date.
                                inDate = newDate.AddDays(-1);

                                //get the string variable to perform calendar control operations
                                yr = inDate.Year;
                                mth = inDate.Month;
                                day = inDate.Day;

                                break;
                            }
                            default:
                            {
                                //set variables
                                numDays = "";
                                direction = "";

                                //format a sliding date to today's date to start from
                                newDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                                //get the number of days to go forward or backwards
                                getTimespan(inArray[6], ref numDays, ref direction);

                                //combine numDays and direction
                                totDirection = direction + numDays;

                                //add the number of day(s) specified to to get the new date.
                                inDate = newDate.AddDays(Convert.ToInt32(totDirection));

                                //get the string variable to perform calendar control operations
                                yr = inDate.Year;
                                mth = inDate.Month;
                                day = inDate.Day;

                                break;
                            }

                        }
                    }
                    else
                    {
                        //convert the date sent in from the data source into a C# DateTime variable to split into its components
                        inDate = DateTime.ParseExact(inArray[6], "MM/dd/yyyy", null);

                        //split the indate DateTime variable into it components
                        yr = inDate.Year;
                        mth = inDate.Month;
                        day = inDate.Day;
                    }

                    tgtMonth = new DateTime(yr, mth, day);

                    inPath = "/html/body/table/tbody/tr/td/table/tbody/tr[2]/td/div/div[2]/table/tbody/tr[2]/td/ul/li";

                    //add string to the test results list
                    tmpString = "Searching for the " + nmCalendar + " calendar control........(" + stpNum + ")";
                    tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                    //verify that the desired calendar control is present. If not throw an excerption
                    if (isPopup == "Y")
                    {
                        //verify that calendar control icon is where it should be
                        objPres = tstObj.IsElementPresent(By.XPath(clkCntrl));

                        //click the calendar control to launch them popup if present. throw an exception if not
                        if(objPres == true)
                        {
                            driver.FindElement(By.XPath(clkCntrl)).Click();
                        }
                        else
                        {
                            throw new ElementNotVisibleException();
                        }
                    }
                    else
                    {
                        objPres = tstObj.IsElementPresent(By.XPath(calXpath));
                    }

                    //get the active tag in app
                    activeTab = getActiveTab(inPath);

                    if (activeTab != "Events")
                    {
                        //get the displayed month in the calendar popup
                        dispMth = (driver.FindElement(By.XPath(calXpath + mthXpath)).Text);

                        //convert to the equivalent int value (January = 1, etc.)
                        intMth = getMonth(dispMth);

                        //get the displayed year
                        dispYr = Convert.ToInt32(driver.FindElement(By.XPath(calXpath + mthXpath + "[2]")).Text);

                        //make a new datetime variable to use to get the month difference
                        inDate = new DateTime(dispYr, intMth, 1);

                        //get the month difference
                        diffMth = getMonthDiff(tgtMonth, inDate);

                        //click the back or forward buttons diffMth times to get the calendar control onto the correct month
                        for (int x = 0; x < Math.Abs(diffMth); x++)
                        {
                            if (diffMth < 0)                                        
                            {
                                driver.FindElement(By.XPath(calXpath + "/div/a")).Click();
                            }
                            else
                            {
                                driver.FindElement(By.XPath(calXpath + "/div/a[2]")).Click();
                            }
                        }

                        //get the number of rows in the calendar that contain dates
                        numRows = driver.FindElements(By.XPath(calXpath + dateXpath + "/tbody/tr")).Count;

                        //scroll down each row after scrolling across
                        for (int x = 0; x < numRows; x++)
                        {
                            for (int y = 0; y < 7; y++)
                            {
                                //set the path to extract the cell text from
                                if (x == 0 && y == 0)
                                {
                                    clkPath = calXpath + dateXpath + "/tbody/tr/td";
                                }
                                else if (x == 0 && y > 0)
                                {
                                    clkPath = calXpath + dateXpath + "/tbody/tr/td[" + (y + 1).ToString() + "]";
                                }
                                else if (x > 0 && y == 0)
                                {
                                    clkPath = calXpath + dateXpath + "/tbody/tr[" + (x + 1).ToString() + "]/td";
                                }
                                else
                                {
                                    clkPath = calXpath + dateXpath + "/tbody/tr[" + (x + 1).ToString() + "]/td[" + (y + 1).ToString() + "]";
                                }

                                //get the text of the calendar block
                                thsDate = driver.FindElement(By.XPath(clkPath)).Text;

                                //compare to the date expected. if it matcheas set up to exit both for loops
                                if (thsDate == day.ToString())
                                {
                                    //Click the element
                                    driver.FindElement(By.XPath(clkPath)).Click();

                                    //set fndElement to begin the escape sequence
                                    fndItem = 1;

                                    //log the result
                                    tmpString = "The date " + inArray[6] + " was successfully selected in the " + nmCalendar + " calendar control";
                                    tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                    break;
                                }
                            }

                            if (fndItem == 1)
                                break;
                        }
                    }
                    else
                    {
                    }
                }
                catch (ElementNotVisibleException e)
                {
                    //add string to the test results list
                    tmpString = "The " + nmCalendar + " calendar control was not found next to the " + nmCalendar + " field";
                    tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                    fndExcep = -1;

                    //add string to the test results list
                    tmpString = e.Message;
                    tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                }
                catch(Exception e)
                {
                    //add string to the test results list
                    tmpString = "ERROR FOUND:...........(" + stpNum + ")";
                    tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                    fndExcep = -1;

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
                                Thread.Sleep(500);
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
                    if (chkMod != "CT")
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
                    else
                    {

                    }
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
                string ckEditor;
                string fldName;
                string fldID;
                string txtInput;
                string chkMod;
                string modFrame;
                string inputGrade;
                string outParam;
                string tblType;
                string parm1;
                string parm2;
                string parm3;
                IJavaScriptExecutor js = driver as IJavaScriptExecutor;
                IWebElement thsField;

                //initialize variables
                ckEditor = "";
                fldName = "";
                fldID = "";
                txtInput = "";
                chkMod = "";
                modFrame = "";
                inputGrade = "";
                outParam = "";
                parm1 = "";
                parm2 = "";
                parm3 = "";
                tblType = "";
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
                            case 5:
                                inputGrade = inArray[x];
                                break;
                            case 6:
                                tblType = inArray[x];
                                break;
                            case 7:
                                ckEditor = inArray[x];
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

                        if (inArray[0] != "/html/body/table/tbody/tr/td/table[2]/tbody/tr[2]/td[2]/table/tbody/tr/td/div/form/div/div/div/div/fieldset/div[4]" + 
                            "/div/div/div[2]/div/div/div/div/div/div/div/div/div/fieldset/div[2]/div/div/table[2]/tbody/tr/td[5]/input")
                        {
                            thsField.Clear();
                        }

                        //check if this is a single grade input
                        if (inputGrade != "Y")
                        {
                            thsField.SendKeys(txtInput);
                        }
                        else
                        {
                            //click and clear the grade input field
                            driver.FindElement(By.XPath(fldID)).Clear();
                            driver.FindElement(By.XPath(fldID)).Click();

                            //get the Javascript input parameters
                            outParam = driver.FindElement(By.XPath(fldID)).GetAttribute("onblur");
                            getGradeInput(outParam, ref parm1, ref parm2, ref parm3);

                            //save any existing score
                            js.ExecuteScript("save_previous_score(" + parm2 + ", " + parm3 + ")");
                            Thread.Sleep(150);


                            //input the score
                            thsField.SendKeys(txtInput);
                            Thread.Sleep(150);

                            //executeb the correct table validation js based upon the type of grade input table
                            if (tblType != "Y")
                            {
                                js.ExecuteScript("submission_grade_tid(" + parm1 + "," + parm2 + "," + "10"  + "," + parm3 + "," + txtInput + ")");
                            }
                            else
                            {
                                js.ExecuteScript("validate_input('" + txtInput + "');");
                            }


                        }
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
                    tmpString = "The " + fldName + " field was found......";
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

            #region moveSlider
            public void moveSlider(tstObject tstObj, bool objPres, string[] inArray, string stpNum, string pth, ref string[,] tstresult, out int fndExcep, out int tstFail)
            {
                int fndVal;
                string inPath;
                string sldrMaxVal;
                string sldrValue;
                string sldrName;
                string inVal;

                //initialize variables
                fndExcep = 0;
                tstFail = 0;
                fndVal = 0;
                inPath = inArray[0];
                inVal = inArray[1];
                sldrValue = "";
                sldrMaxVal = inArray[2];
                sldrName = inArray[3];
                try
                {
                    //look to see if the slider object exists
                    objPres = driver.FindElement(By.XPath(inPath)).Displayed;

                    if (objPres == true)
                    {
                        //click on the slider
                        driver.FindElement(By.XPath(inPath)).Click();

                        //get the initial text of the slider
                        sldrValue = driver.FindElement(By.XPath(inArray[0])).Text;

                        //loop through the value click by click until the slider shows the desired value
                        do
                        {
                            //perform one button click to the right
                            driver.FindElement(By.XPath(inPath)).SendKeys(OpenQA.Selenium.Keys.Right);

                            //get new slider value
                            sldrValue = driver.FindElement(By.XPath(inPath)).Text;

                            if (sldrValue == inVal)
                            {
                                fndVal = 1;
                            }
                        }
                        while (fndVal != 1 && Convert.ToInt32(sldrValue) != Convert.ToInt32(sldrMaxVal));
                    }

                    if (fndVal == 1)
                    {
                        tmpString = "Successfully moved the " + sldrName + " to the value " + sldrValue;
                        tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                    }
                    else
                    {
                        tstFail = -1;
                        tmpString = "The " + sldrName + " could not be moved to the value " + sldrValue;
                        tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                    }
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

            #region navLinks
            //Click the link sent from the spreadsheet
            public void navLinks(tstObject tstObj, bool objPres, string[] inArray, string stpNum, string pth, ref string[,] tstresult, out int fndExcep)
            {
                int strLen;
                string iconNum;
                string inString;
                string lnkText;
                string lnkID;
                IJavaScriptExecutor js = driver as IJavaScriptExecutor;

                //initilize variables
                lnkID = "";
                fndExcep = 0;
                inString = "";
                lnkText = "";

                if (inArray[0] != "SWITCH TAB")
                {
                    objPres = driver.FindElement(By.XPath(inArray[0])).Displayed;
                }

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

                if (inString != "SWITCH TAB")
                {

                    //Format the link name
                    //add string to the test results list
                    tmpString = "Clicking the " + lnkText + " link..... (" + stpNum + ")";
                    tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                    try
                    {
                        //get the string length of the xpath being submitted for click
                        strLen = inString.Length;

                        //the app looks for a 63 char string to denote a click in the top nav. If an xpath is shorter than 63 characters, there
                        //is no need to check and the app can just proceed to click the link being sent in
                        if (strLen >= 63)
                        {
                            //determine if the link to be clicked is in the top nav bar (gear, google, or flag icons) 
                            if (inString.Substring(0, 63) == "/html/body/table/tbody/tr/td/table/tbody/tr/td/div/div[3]/table")
                            {
                                iconNum = inString.Substring(76, 1);

                                switch (iconNum)
                                {
                                    case "2":
                                        {
                                            lnkID = "notification-data";
                                            break;
                                        }
                                    case "3":
                                        {
                                            lnkID = "gapps-data";
                                            break;
                                        }
                                    case "4":
                                        {
                                            lnkID = "settings-data";
                                            break;
                                        }
                                    default:
                                        {
                                            lnkID = "";
                                            break;
                                        }
                                }
                            }
                        }
                        //switch statement governing how the app clicks on a link
                        switch (lnkID)
                        {
                            case "settings-data":
                                {
                                    //click on the gear icon
                                    driver.FindElement(By.XPath("id('settings-tipsy')")).Click();

                                    //Execute onclick javascript event
                                    js.ExecuteScript("settings_tipsy_call()");

                                    //make the resulting options list visible
                                    js.ExecuteScript("$('#settings-data').show();");

                                    //click on the link passed in to the navLinks
                                    Thread.Sleep(400);
                                    driver.FindElement(By.XPath(inString)).Click();

                                    //return the app to the previous state
                                    js.ExecuteScript("$('#settings-data').hide();");

                                    break;
                                }
                            case "gapps-data":
                                {
                                    //click on the google icon
                                    js.ExecuteScript("gapps_tipsy_call()");

                                    //make the resulting google options list visible
                                    js.ExecuteScript("$('#gapps-data').show();");

                                    //click on the link passed in to the navLinks
                                    Thread.Sleep(400);
                                    driver.FindElement(By.XPath(inString)).Click();

                                    //return the app to the previous state
                                    js.ExecuteScript("$('#gapps-data').hide();");

                                    break;
                                }
                            case "notification-data":
                                {
                                    ////click on the flag icon
                                    js.ExecuteScript("notification_tipsy_call()");

                                    //make the resulting google options list visible
                                    js.ExecuteScript("$('#notification-data').show();");

                                    //click on the link passed in to the navLinks
                                    Thread.Sleep(400);
                                    driver.FindElement(By.XPath(inString)).Click();

                                    //return the app to the previous state
                                    js.ExecuteScript("$('#notification-data').hide();");

                                    break;
                                }
                            default:
                                Thread.Sleep(400);
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
                else
                {
                    try
                    {

                        //Format the link name
                        //add string to the test results list
                        tmpString = "Navigating to tab " + lnkText + "..... (" + stpNum + ")";
                        tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);


                        tstObj.switchTab(tstObj, inArray, stpNum, pth, out fndExcep, stpNum);
                        driver.FindElement(By.XPath(inString)).Submit();

                        //driver.FindElement(By.

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
            
                IWebElement dropdownListBox;
                ReadOnlyCollection<IWebElement> lnkList;
                ReadOnlyCollection<IWebElement> lstString;
                SelectElement selection;
                int count;              //number of lst items in the dropdown
                int fndItem;
                string fndList;         //final xpath value 
                string sndKey;

                //set initial value for fndExcep
                count = 0;
                fndExcep = 0;
                fndItem = 0;
                sndKey = "A";
                lnkList = null;

                try
                {
                    //2.08 -- check for the new dropdown instituted by the Virtual Academy
                    if (clk != "Std" && clk != "CT")
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
                        }
                    
                        //Record result to result file
                        tmpString = "Searching for the " + resString + " dropdown........(" + stpNum + ")";
                        tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                        //get count of items in the dropdown
                        count = driver.FindElements(By.XPath(lstTag)).Count;

                        //load all of the dropdown elements into a ReadOnly Collection of IWebElements
                        lnkList = driver.FindElements(By.XPath(lstTag));

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
                        }
                    }
                    else if (clk == "Std")
                    {
                        //look to see if the dropdown object exists
                        objPres = driver.FindElement(By.XPath(drpName)).Enabled;

                        //Record result to result file
                        tmpString = "Searching for the " + resString + " dropdown........(" + stpNum + ")";
                        tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                        dropdownListBox = driver.FindElement(By.XPath(drpName));
                        selection = new SelectElement(dropdownListBox);

                        //scroll through the selections in the dropdown
                        for (int x = 0; x < selection.Options.Count; x++)
                        {
                            if (selection.Options[x].Text == lstItem)
                            {
                                tmpString = "Found list item [" + lstItem + "]";
                                tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                fndItem = 1;
                                selection.SelectByText(lstItem);
                                break;
                            }
                        }

                        if (fndItem == 1)
                        {
                            //Record result
                            tmpString = "Successfully selected item [" + lstItem + "]";
                            tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                        }
                        else
                        {
                            //Record result
                            tmpString = "The item [" + lstItem + "] was not found";
                            tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                        }
                    }
                    else if (clk == "CT")
                    {
                        //look to see if the dropdown object exists
                        objPres = driver.FindElement(By.XPath(drpName)).Enabled;

                        //get count of items in the dropdown
                        count = driver.FindElements(By.XPath(lstTag)).Count;

                        //load all of the dropdown elements into a ReadOnly Collection of IWebElements
                        lstString = driver.FindElements(By.XPath(lstTag));

                        //lstarray = TestSuite.splitList(Convert.ToString(lstString[0].Text), ref itmCount);

                        //scroll through the array and select the appropriate checkboix if applicable  
                        for (int y = 0; y < count; y++)
                        {
                            if (lstString[y].Text == lstItem)
                            {
                                if (y == 0)
                                {
                                    fndItem = 1;

                                    tmpString = "Found list item [" + lstItem + "]";
                                    tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                    //drop the list
                                    driver.FindElement(By.XPath(drpName)).Click();

                                    //select  the matching item
                                    driver.FindElement(By.XPath(lstTag + suffix + "/input")).Click();

                                    //click the dropdown again to close 
                                    driver.FindElement(By.XPath(drpName)).Click();

                                    //Record result
                                    tmpString = "Successfully selected item [" + lstItem + "]";
                                    tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                }
                                else
                                {
                                    fndItem = 1;

                                    tmpString = "Found list item [" + lstItem + "]";
                                    tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                    //drop the list
                                    driver.FindElement(By.XPath(drpName)).Click();

                                    //select  the matching item
                                    driver.FindElement(By.XPath(lstTag + "[" + (y + 1).ToString() + "]")).Click();
                                
                                    //click the dropdown again to close 
                                    driver.FindElement(By.XPath(drpName)).Click();
      
                                    //Record result
                                    tmpString = "Successfully selected item [" + lstItem + "]";
                                    tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                }
                            }
                        }

                        //record a failure if nothing is clicked is found
                        if (fndItem != 1)
                        {
                            //Record result
                            tmpString = "The item [" + lstItem + "] was not found";
                            tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                        }
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
                //IAlert alert;
                string keyString;
                //string BaseWindow;
                //ReadOnlyCollection<string> handles;

                fndExcep = 0;
                keyString = inArray[0];
                /*

                driver.SwitchTo().Alert().Accept();

                BaseWindow = driver.CurrentWindowHandle;

                Thread.Sleep(6000);
                handles = driver.WindowHandles;
                keyString = inArray[0];

                foreach (string handle in handles)
                {

                    if (driver.SwitchTo().Window(handle).Title.Equals("Update your email address!"))
                    {
                        driver.Close();

                    }
                }

                //driver.SwitchTo().Window("Update your email address!");*/

                if (inArray[0] == "Keys.ENTER")
                {
                    driver.FindElement(By.XPath(inArray[1])).SendKeys(OpenQA.Selenium.Keys.Enter);
                }
                else
                {
                    driver.FindElement(By.XPath(inArray[1])).SendKeys(keyString);
                }
                //SendKeys.Send("Keys." + keyString);

                //driver.SwitchTo().Window(BaseWindow);
            }
            #endregion

            #region switchTab
            public void switchTab(tstObject tstObj, string[] inArray, string inStep, string pth, out int fndExcep, string stpNum)
            {
                fndExcep = 0;

                driver.FindElement(By.XPath("html")).SendKeys(OpenQA.Selenium.Keys.Control + inArray[1]);
            }
            #endregion

            #region tblSelect
            public void tblSelect(tstObject tstObj, bool objPres, string[] inArray, string stpNum, string pth, ref string[,] tstresult, out int fndExcep)
            {
                //inArray contains all the data that has been imported from the spreadsheet. The data will parcelecd out
                //and placed into the respective values that each item corresponds to


                bool lstPath;
                bool gpPres;
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
                //string tagName;
                string tblId;
                string tblName;
                string tblTag;
                string tmpCount;
                string tmpString;
                string txtPath;
                string schCol;
                string schText;
                string slct;
                string suffix;
                string vfyString;
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
                    /*enter into the if portion if inArray[1] is not equal to 'CU'
                    'CU' denotes selecting an item in a cleanup operation and so the table location
                    data needed and will need to be set in the else portion instead of being passed*/ 
                    /*the if section will take the data passed in from inArray and set it to local variables
                    that the tblSelect function can use*/
                    if (inArray[1] != "CU")
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
                    }
                    else
                    {
                        switch (inArray[3])
                        {
                            case "Home":
                            case "Courses":
                            {
                                //set all of the location and function variables to select desired item in the table being searched  
                                tblId = inArray[2];
                                schText = inArray[0];
                                tblName = inArray[3];
                                schCol = "1";
                                tblNav = "0";
                                chkMod = "";
                                frmMod = "";
                                suffix = "/a";
                                lstPath = false;
                                slct = "Y";

                                break;
                            }
                            case "Events":
                            {
                                break;
                            }
                            case "Grades":
                            {
                                break;
                            }
                            case "Groups":
                            {
                                //set all of the location and function variables to select desired item in the table being searched  
                                tblId = inArray[2];
                                schText = inArray[0];
                                tblName = inArray[3];
                                schCol = "1";
                                tblNav = "4";
                                chkMod = "";
                                frmMod = "";
                                suffix = "/div/a";
                                lstPath = false;
                                slct = "Y";
                                break;
                            }
                            case "People":
                            {
                                break;
                            }
                            case "Library":
                            {
                                break;
                            }
                            case "Tools":
                            {
                                break;
                            }
                            default:
                            {
                                break;
                            }
                        }
                    }

                    //set the vfyString if verifying a particular entry in a table
                    if (inArray.Length == 11)
                    {
                        vfyString = inArray[10];
                    }
                    else
                    {
                        vfyString = "";
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

                    if (inArray[1] != "CU")
                    {
                        //add string to the test results list
                        tmpString = "Searching for the " + tblName + " table........(" + stpNum + ")";
                        tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                    }

                    for (int tblCount = 0; tblCount < itmCount; tblCount++)
                    {
                        numRows = driver.FindElements(By.XPath(tblId + "/tbody/tr")).Count;

                        //x is the row number currently being processed
                        for (int x = 0; x < numRows; x++)
                        {
                            try
                            {
                                //check all other tables exceprt the Events table which is slightly different
                                if (tblName != "Events")
                                {
                                    //check if the table item is present and get the text of the searched item
                                    if (x == 0)
                                    {
                                        if ((tblMove == 0 && schCol == "0") || (tblMove == 0 && schCol == "1"))
                                        {
                                            //check if initial xpath object is present
                                            objPres = driver.FindElement(By.XPath(tblId + "/tbody/tr/td")).Enabled;

                                            //get the row text of the [0] item
                                            txtPath = tblId + "/tbody/tr/td";
                                            rowText = driver.FindElement(By.XPath(tblId + "/tbody/tr/td")).Text;
                                        }
                                        else
                                        {
                                            //check if initial xpath object is present
                                            objPres = driver.FindElement(By.XPath(tblId + "/tbody/tr/td[" + schCol + "]")).Enabled;

                                            txtPath = tblId + "/tbody/tr/td[" + schCol + "]";
                                            //get the row text of the [x] item
                                            rowText = driver.FindElement(By.XPath(tblId + "/tbody/tr/td[" + schCol + "]")).Text;
                                        }
                                    }
                                    else
                                    {
                                        if (schCol == "1")
                                        {
                                            txtPath = tblId + "/tbody/tr[" + (x + 1).ToString() + "]/td";
                                            rowText = driver.FindElement(By.XPath(tblId + "/tbody/tr[" + (x + 1).ToString() +
                                                "]/td")).Text;
                                        }
                                        else
                                        {
                                            txtPath = tblId + "/tbody/tr[" + (x + 1).ToString() + "]/td[" + schCol + "]";
                                            rowText = driver.FindElement(By.XPath(tblId + "/tbody/tr[" + (x + 1).ToString() +
                                                "]/td[" + schCol + "]")).Text;
                                        }
                                    }
                                }
                                //the Events table has a different make up in that its entries are tables with a table. So the path needs to be constructed a little differently
                                else  
                                {
                                    if (x == 0)
                                    {
                                        //check if initial xpath object is present
                                        objPres = driver.FindElement(By.XPath(tblId + "/tbody/tr/td/table/tbody/tr/td[" + schCol + "]")).Enabled;

                                        //get the row text of the [0] item
                                        txtPath = tblId + "/tbody/tr/td/table/tbody/tr/td[" + schCol + "]";
                                        rowText = driver.FindElement(By.XPath(tblId + "/tbody/tr/td/table/tbody/tr/td[" + schCol + "]" + suffix)).Text;
                                    }
                                    else
                                    {
                                        txtPath = tblId + "/tbody/tr[" + (x + 1).ToString() + "]/td/table/tbody/tr/td[" + schCol + "]";
                                        rowText = driver.FindElement(By.XPath(tblId + "/tbody/tr[" + (x + 1).ToString() + "/td/table/tbody/tr/td[" + schCol + "]")).Text;
                                    }
                                }
                            }
                            catch
                            {
                                continue;
                            }

                            //if (inArray[12] != "Y")
                                //see if the search string is in the table entry
                                rowText = TestSuite.getTableEntry(driver, txtPath, rowText.Trim(), schText.Trim(), vfyString, schText.Trim().Length);
                            //else
                                //rowText = TestSuite.

                            //get the row text from the beginning to the strLen and check if it matches the schText
                            //this is in case any suffix entries (ex: member count) are present
                            if (rowText == schText.Trim())
                            {
                                if (tblName != "Events")
                                {
                                    //checking to see if the cleanup operation involves a group. Need to check if the group hasn't been approved
                                    //it won't show as a link. In this case, the schCol variable needs to be moved over 4 columns as the group can't
                                    //be deleted
                                    if (inArray[1] == "CU" && tblName == "Groups")
                                    {
                                        //get the fndPath stipulated. At this point we are looking for a link
                                        //if (x == 0)
                                            //fndPath = tblId + "/tbody/tr/td[" + schCol + "]" + suffix;
                                        //else
                                        fndPath = tblId + "/tbody/tr[" + (x + 1).ToString() + "]/td[" + schCol + "]" + suffix;

                                        //check if a link exists. The text already matches. This is for a group. If the group has not been approved yet, it can't be
                                        //opened and deleted. It will need to be deleted from table grid. This will look for a link vis-a-vis a text entry 
                                        gpPres = tstObj.IsElementPresent(By.XPath(fndPath));

                                        //if the link does not exist, the group cannot be accessed so the schCol variable needs to moved over 4 columns to the delete icon
                                        if (gpPres == false)
                                        {
                                            schCol = "5";
                                            suffix = "/div/a/img";

                                            //get the fndPath stipulated. At this point we are looking for a link
                                            if (x == 0)
                                                fndPath = tblId + "/tbody/tr/td[" + schCol + "]" + suffix;
                                            else
                                                fndPath = tblId + "/tbody/tr[" + (x + 1).ToString() + "]/td[" + schCol + "]" + suffix;
                                        }
                                    }
                                    else
                                    {

                                        if (x == 0 && ((Convert.ToInt32(schCol) + (tblMove)) == 0 || (Convert.ToInt32(schCol) + (tblMove)) == 1))
                                            fndPath = tblId + "/tbody/tr/td" + suffix;
                                        else if (x == 0)
                                            fndPath = tblId + "/tbody/tr/td[" + (Convert.ToInt32(schCol) + (tblMove)) + "]" + suffix;
                                        else if ((Convert.ToInt32(schCol) + (tblMove)) == 0 || (Convert.ToInt32(schCol) + (tblMove)) == 1)
                                            fndPath = tblId + "/tbody/tr[" + (x + 1) + "]/td" + suffix;
                                        else
                                            fndPath = tblId + "/tbody/tr[" + (x + 1) + "]/td[" + (Convert.ToInt32(schCol) + (tblMove)) + "]" + suffix;
                                    }
                                }
                                else
                                {
                                
                                }

                            
                                //if the item is to be selected, enter this portion of the if statement
                                if (slct == "Y")
                                {
                                    //set clkCell to a WebElement at the fndPath xpath
                                    driver.FindElement(By.XPath(fndPath)).Click();

                                    if (inArray[1] != "CU")
                                    {
                                        //Record the result
                                        tmpString = "The item '" + schText + "' was found in the the " + tblName + " table";
                                        tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                    }
                                }

                                //added code
                                else if (slct == "N")
                                {
                                    Boolean view;
                                    found = true;
                                    view = driver.FindElement(By.XPath(fndPath)).Displayed;
                                    if (view && vis == "Y")
                                    {
                                        if (inArray[1] != "CU")
                                        {
                                            //Record the result
                                            tmpString = "The item at position(" + fndPath + ") was found in the the " + tblName + " table, and is visible";
                                            tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                        }
                                    }
                                    else if (view && vis == "N")
                                    {
                                        if (inArray[1] != "CU")
                                        {
                                            //Record the result
                                            tmpString = "The item at position(" + fndPath + ") was found in the the " + tblName + " table, and should not be visible";
                                            tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                        }
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
                        //in inArray[1] is 'CU' and there is nothing found this is an expected (possible) condition and so
                        //there is no need to log a result. Script should continue with no errors
                        if (inArray[1] != "CU")
                        {
                            if (slct == "X")
                            {
                                objPres = false;
                                //Record the result
                                tmpString = "The item '" + schText + "' was not found in the the " + tblName + " table";
                                tstresult = arrayAppend(objPres.ToString(), tmpString, "1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
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
                            tstresult = arrayAppend(objPres.ToString(), tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

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
                double waitTime;

                waitTime = Convert.ToDouble(inTime) * 1000.0;
                Thread.Sleep(Convert.ToInt32(waitTime));
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

                            if (inArray[x] != "*")
                            {
                                gradeArray[grdCount, 1] = inArray[x];
                            }
                            else
                            {
                                gradeArray[grdCount, 1] = "0.0";
                            }
                            grdCount++;
                        }

                    }

                    //get the student's name
                    stdntName = driver.FindElement(By.XPath(xpath + "/td[2]")).Text;

                    //get the grade from the table
                    tblGrade = driver.FindElement(By.XPath(xpath + "/td[3]")).Text;

                    //get the grade percent from a straight copy of text from the Pct field and strip off the percent sign
                    tmpPct = driver.FindElement(By.XPath(xpath + "/td[4]")).Text;

                    if (tmpPct != "*")
                        tmpPct = tmpPct.Substring(0, tmpPct.Length - 1);
                    else 
                        tmpPct = "0";

                    //Convert the tmpPct to a double
                    if (tmpPct == "*" || tmpPct == "0" || tmpPct == "")
                    {
                        grdPercent = 0.0;
                    }
                    else
                    {
                        grdPercent = Convert.ToDouble(tmpPct);
                    }

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

            #region vfyNote
            public void vfyNote(string pth, string inText)
            {
                int brkNum;
                int remainder;
                int strLen;
                string thsChar;
                string preString;
                string[] strArray;

                //get the string length
                strLen = inText.Length;
                preString = "";
                thsChar = "";

                //modulus division to get the array size to hold the string segments
                remainder = strLen % 130;

                //length - mod to get the whole number of breaks
                brkNum = (strLen - remainder) / 130;
 
                //set the string array
                strArray = new string[brkNum + 1];

                for (int cnt = 0; cnt < brkNum + 1; cnt++)
                {
                    //write a 130 character long string into the string array to get a raw break of strings
                    if (cnt == 0)
                        strArray[0] = inText.Substring(0, 130);
                    else if (cnt < brkNum)
                        strArray[cnt] = preString + inText.Substring((cnt * 130), 130);
                    else if (cnt == brkNum)
                        strArray[cnt] = preString + inText.Substring((strLen - remainder), remainder);

                    //clear out the preString variable in preperation for the next line 
                    if (preString != String.Empty)
                        preString = String.Empty;
                    //back up to the previous space to avoid splitting words

                    if (cnt != brkNum)
                    {
                        for (int charCnt = strArray[cnt].Length; charCnt > 0; charCnt--)
                        {
                            thsChar = strArray[cnt].Substring(charCnt - 1, 1);

                            if (thsChar == " ")
                            {
                                strArray[cnt] = strArray[cnt].Substring(0, charCnt);
                                break;
                            }
                            else
                            {
                                preString = thsChar + preString;
                            }

                        }
                    }

                    //move any punctuation that starts a string to the previous array entry
                    if ((strArray[cnt].Substring(1, 0) == "." || strArray[cnt].Substring(1, 0) == "," ||
                        strArray[cnt].Substring(1, 0) == "?" || strArray[cnt].Substring(1, 0) == "!") && (cnt != 0))
                    {
                        strArray[cnt - 1] = strArray[cnt - 1] + strArray[cnt].Substring(1, 0);
                    }

                    strArray[cnt] = strArray[cnt].Trim();
                }

                //Write the text to the results file
                for (int cnt = 0; cnt < brkNum + 1; cnt++)
                {
                    if (cnt == 0)
                        TextFileOps.Write(pth, "<span class=\"jqtree-title\" style=\"font-family:verdana;font-size:75%;color:#000000\"><b>" + "NOTE: " + strArray[cnt] + "<br /></b></span>", 100);
                    else if (cnt == brkNum)
                        TextFileOps.Write(pth, "<span class=\"jqtree-title\" style=\"font-family:verdana;font-size:75%;color:#000000\"><b>" + "      " + strArray[cnt] + "<br /><br /></b></span>", 100);
                    else
                        TextFileOps.Write(pth, "<span class=\"jqtree-title\" style=\"font-family:verdana;font-size:75%;color:#000000\"><b>" + "      " + strArray[cnt] + "<br /></b></span>", 100);
                }
            }
            #endregion

            #region vfyTableEntry
            public void vfyTableEntry(tstObject tstObj, string tblPath, string xpath, string schCol, string mvCol, string vfySegment, string offsetItem,
                string getNeg, string isHeader, string thsTable, string schItem, string datsource, string stpNum, string pth, ref string retXpath, ref string[,]rsltArray, out int fndExcep, out int tstFail)
            {
                bool objPres;
                string tblEntry;
                string fndPath;
                string schPath;
                int numRows;
                int fndItem;

                fndPath = "";
                objPres = false;
                tblEntry = "";
                numRows = 0;
                fndItem = 0;
                tstFail = 0;
                fndExcep = 0;

                try
                {
                    //get the table Entry...this may need to be gleaned from the table entry using Firebug as there may very well be other
                    //things present (exclamation point, pencil icon, etc.) which may changer the text present
                    objPres = tstObj.IsElementPresent(By.XPath(tblPath));

                    if (fndItem == 0)
                        fndItem = 0;

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
                                        if (schCol == "1")
                                        {
                                            fndPath = tblPath + xpath + "/td";
                                            tblEntry = driver.FindElement(By.XPath(fndPath)).Text;
                                        }
                                        else
                                        {
                                            fndPath = tblPath + xpath + "/td[" + schCol + "]";
                                            tblEntry = driver.FindElement(By.XPath(fndPath)).Text;
                                        }
                                    }
                                    else
                                    {
                                        if (schCol == "1")
                                        {
                                            //subsequent row condition. Append an index [x] to the xpath
                                            fndPath = tblPath + xpath + "[" + x.ToString() + "]/td";
                                            tblEntry = driver.FindElement(By.XPath(fndPath)).Text;
                                        }
                                        else
                                        {
                                            //subsequent row condition. Append an index [x] to the xpath
                                            fndPath = tblPath + xpath + "[" + x.ToString() + "]/td[" + schCol + "]";
                                            tblEntry = driver.FindElement(By.XPath(fndPath)).Text;
                                        }
                                    }
                                }

                                if (datsource == "EX" && vfySegment != "Y")
                                {
                                    tblEntry = TestSuite.setSlash(tblEntry);
                                }
                                //construct the search string using the x variable (row position) and the schCol variable (column position) to isolate
                                //the cell text to search
                                else if (vfySegment == "Y")
                                {
                                    //if the search column is the first coliumn
                                    if (schCol != "1")
                                    {
                                        //if the current row position is the first row
                                        if (x == 1)
                                        {
                                            schPath = xpath + "/td[" + schCol + "]";
                                        }
                                        //if the current is not the first row, asppend the rownumnber on the tr tag of xpath 
                                        else
                                        {
                                            schPath = xpath + "[" + x.ToString() + "]/td[" + schCol + "]";
                                        }
                                    }
                                    else
                                    {
                                        //if the current row position is the first row
                                        if (x == 1)
                                        {
                                            schPath = xpath + "/td";
                                        }
                                        //if the current is not the first row, asppend the rownumnber on the tr tag of xpath 
                                        else
                                        {
                                            schPath = xpath + "[" + x.ToString() + "]/td";
                                        }
                                    }

                                    //send the search path (tblPath to the table -- xpath to isolate the cell in the table) to extract the table entry
                                    tblEntry = TestSuite.getTableEntry(driver, tblPath, tblEntry.Trim(), schItem, schPath, schItem.Length);
                                }

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

                                    if (offsetItem == tblEntry.Trim() && offsetItem != "NOT BLANK")
                                    {
                                        //write to results file that item was found in the offset column
                                        objPres = true;
                                        fndItem = 1;
                                        break;
                                    }
                                    else if (offsetItem == "NOT BLANK" && tblEntry.Trim() != "")
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
                                        if (getNeg == "N")
                                        {
                                            fndItem = 0;
                                            objPres = false;
                                        }
                                        else
                                        {
                                            objPres = false;
                                            tstFail = -1;
                                        }
                                    }
                                
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
                    //a negative result (field not expected) will probably result in a null value
                    //set mvCol = 0 to process if this is the case
                    if (mvCol == "")
                        mvCol = "0";
                    if (Convert.ToInt32(mvCol) == 0)
                    {
                        rsltArray = arrayAppend("verify", "table", objPres.ToString(), schItem.Trim(), schItem.Trim(), thsTable, String.Empty, String.Empty, String.Empty, rsltArray);
                    }
                    else
                    {
                        rsltArray = arrayAppend("verify", "table", objPres.ToString(), tblEntry.Trim(), offsetItem, thsTable, String.Empty, String.Empty, String.Empty, rsltArray);
                    }
                }
            }
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
                                vfyString = driver.FindElement(By.XPath(itmPath)).Text;

                                objPres = objVerify(vfyString, fldName, getNeg, ref tstFail);

                                rsltArray = arrayAppend("verify", "field text", objPres.ToString(), fldName.Trim(), vfyString, lstVerify, String.Empty, String.Empty, String.Empty, rsltArray);

                                break;
                            }
                            default:
                            {
                                break;
                            }

                        }
                        break;
                    case "text":
                        if (getNeg != "N")
                        {
                            vfyString = driver.FindElement(By.XPath(itmPath)).Text;
                        }

                        vfyString = TestSuite.getTableEntry(driver, itmPath, vfyString.Trim(), fldName, "", fldName.Length);

                        objPres = objVerify(vfyString, fldName, getNeg, ref tstFail);

                        rsltArray = arrayAppend("verify", "text", objPres.ToString(), lstVerify, vfyString, fldName.Trim(), String.Empty, String.Empty, String.Empty, rsltArray);
                        break;
                    case "button":
                        if (getNeg != "N")
                        {
                            if (fldName != "Add Outcome")
                            {
                                vfyString = driver.FindElement(By.XPath(itmPath)).Text;
                                objPres = objVerify(vfyString, fldName, getNeg, ref tstFail);
                            }
                            else
                            {
                                vfyString = "Add Outcome";

                                if ((driver.FindElement(By.XPath(itmPath)).Enabled == true && getNeg != "N") || (driver.FindElement(By.XPath(itmPath)).Enabled == false && getNeg == "N"))
                                {
                                    objPres = true;
                                    tstFail = 0;
                                }
                                else
                                {
                                    objPres = false;
                                    tstFail = 0;
                                }
                            }
                        }
                        else
                        {
                            vfyString = fldName;
                            objPres = tstObj.IsElementPresent(By.XPath(itmPath));
                        }

                        

                        rsltArray = arrayAppend("verify", "button", objPres.ToString(), fldName.Trim(), vfyString, lstVerify, String.Empty, String.Empty, String.Empty, rsltArray);
                        break;
                    case "link":
                        if (getNeg != "N")
                        {
                            vfyString = driver.FindElement(By.XPath(itmPath)).Text;
                        }

                        objPres = objVerify(vfyString, fldName, getNeg, ref tstFail);

                        rsltArray = arrayAppend("verify", "link", objPres.ToString(), lstVerify, vfyString, fldName.Trim(), String.Empty, String.Empty, String.Empty, rsltArray);
                        break;
                    case "image":
                        objPres = tstObj.IsElementPresent(By.ClassName(fldName));
                    
                        rsltArray = arrayAppend("verify", "image", objPres.ToString(), fldName.Trim(), lstVerify, String.Empty, String.Empty, String.Empty, String.Empty, rsltArray);
                        break;
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
            public string[,] xlFunctions(tstObject tstObj, string xlPath, string tstName, string shtName, out int dataIndex, ref string product)
            {
                string[,] stpArray;
                string[,] dataArray;
                string[,] fnlArray;
                int[] itmNumArray;
                int totCols;

                //Get the list of steps this test will use
                stpArray = TestSuite.getXLData(tstName, xlPath, shtName, ref product);

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
                int itmCount;
                try
                {
                    itmCount = driver.FindElements(by).Count;

                    if (itmCount > 0)
                        return true;
                    else
                        return false;
                }
                catch
                {
                    return false;
                }
            }
            #endregion

            #region driveFunction
            public void driveFunction(tstObject tstObj, string doFunc, string lnNum, string stpNum, string[] inArray, string baseURL, string product, string datsource, string pth, ref string getNeg, 
                ref string[,] tstresult, out int fndExcep, out int tstFail)
            {
                bool dispObj;
                string objString;
                string thsItem;
                string itmPath;
                string retXpath;
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
                                tstObj.Login(tstObj, inArray, baseURL, stpNum, brwsrType, product, pth, out fndExcep, ref tstresult);
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
                                tstObj.Logout(pth, true, stpNum, product, ref tstresult);
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

                        #region calendarControl
                        case "calendarControl":
                        {
                            calendarControl(tstObj, inArray, stpNum, pth, ref tstresult, out fndExcep);

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
                                        dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, "");

                                        if (currTime.Seconds >= 6)
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
                                        tmpString = objString;
                                        tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                    }

                                    driver.SwitchTo().DefaultContent();
                                }
                                else
                                {
                                    do
                                    {
                                        dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, "");

                                        if (currTime.Seconds >= 6)
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
                                        tmpString = objString;
                                        tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
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
                                        dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, "");

                                        if (currTime.Seconds >= 6)
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
                                        tmpString = objString;
                                        tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                    }

                                    driver.SwitchTo().DefaultContent();
                                }
                                else
                                {
                                    do
                                    {
                                        dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, "");

                                        if (currTime.Seconds >= 6)
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
                                        tmpString = objString;
                                        tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
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
                                        dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, "");

                                        if (currTime.Seconds >= 6)
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
                                        tmpString = objString;
                                        tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                    }

                                    driver.SwitchTo().DefaultContent();
                                }
                                else
                                {
                                    if (inArray[0] != "URL")
                                    {
                                        do
                                        {
                                            dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, "");

                                            if (currTime.Seconds >= 6)
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
                                            tmpString = objString;
                                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                        }
                                    }
                                    else
                                    {
                                        driver.Navigate().GoToUrl(inArray[2]); 
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

                        #region moveSlider
                        case "moveSlider":
                        {
                            //add string to the test results list
                            tmpString = "Setting the ........(" + stpNum + ")";
                            try
                            {
                                do
                                {
                                    dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, "");

                                    if (currTime.Seconds >= 6)
                                    {
                                        dispObj = false;
                                        break;
                                    }
                                }
                                while (dispObj != true);

                                if (dispObj == true)
                                {
                                    tstObj.moveSlider(tstObj, dispObj, inArray, stpNum, pth, ref tstresult, out fndExcep, out tstFail);
                                }
                                else
                                {
                                    fndExcep = -1;
                                    tmpString = objString;
                                    tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                }
                            }
                            catch(Exception e)
                            {
                                //Record failed result
                                tmpString = "An exception was found moving the slider: " + e.Message;
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
                            if (inArray[0] != "SWITCH TAB")
                            {
                                try
                                {
                                    do
                                    {
                                        dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, "");

                                        if (currTime.Seconds >= 6)
                                        {
                                            dispObj = false;
                                            break;
                                        }
                                    }
                                    while (dispObj != true);

                                    if (dispObj == true)
                                    {
                                        tstObj.navLinks(tstObj, dispObj, inArray, stpNum, pth, ref tstresult, out fndExcep);
                                    }
                                    else
                                    {
                                        fndExcep = -1;
                                        tmpString = objString;
                                        tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
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
                            }
                            else
                            {
                                tstObj.navLinks(tstObj, dispObj, inArray, stpNum, pth, ref tstresult, out fndExcep);
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
                                        dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, "");

                                        if (currTime.Seconds >= 6)
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
                                        tmpString = objString;
                                        tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                    }

                                    driver.SwitchTo().DefaultContent();
                                }
                                else
                                {
                                    do
                                    {
                                        dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, "");

                                        if (currTime.Seconds >= 6)
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
                                        tmpString = objString;
                                        tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
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
                                            dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, "");

                                            if (currTime.Seconds >= 6)
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
                                            tmpString = objString;
                                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                        }

                                        driver.SwitchTo().DefaultContent();
                                    }
                                    else
                                    {
                                        do
                                        {
                                            dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, "");

                                            if (currTime.Seconds >= 6)
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
                                            tmpString = objString;
                                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
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
                                    dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, "");

                                    if (currTime.Seconds >= 6)
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
                                    tmpString = objString;
                                    tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                }

                                thsItem = inArray[2];

                                tmpString = "Finished checking the grade table";
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
                                        dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, inArray[5]);

                                        if (currTime.Seconds >= 6)
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
                                                tmpString = objString;
                                                tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
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
                                    tmpString = "Verifying that the  " + thsItem + " table is present";
                                    tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                    //TextFileOps.Write(pth, "Found the " + thsItem + " table......", 1);

                                    for (int x = 0; x < inArray.Length - 9; x++)
                                    {
                                        do
                                        {
                                            dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, inArray[6]);

                                            if (currTime.Seconds >= 6)
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
                                            tmpString = objString;
                                            tstresult = arrayAppend("False", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
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
                                        dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, "");

                                        if (currTime.Seconds >= 6)
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
                                                " dropdown", dispObj, stpNum, pth, ref tstresult, out fndExcep, out tstFail);
                                        }
                                        //driver.FindElement(By.XPath(inArray[0])).Click();
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
                                        dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, "");

                                        if (currTime.Seconds >= 6)
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

                                tmpString = "Searching for the " + inArray[3] + " image........(" + stpNum + ")";
                                tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                //section off the scenarios if the field is (!=N) or is not (==N) present on the screen
                                if (inArray[2] != "N" && inArray[2] != "n")
                                {
                                    do
                                    {
                                        dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, "");

                                        if (currTime.Seconds >= 6)
                                        {
                                            dispObj = false;
                                            break;
                                        }
                                    }
                                    while (dispObj != true);

                                    if (dispObj == true)
                                    {
                                        tmpString = "Found the " + thsItem + " image......";
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
                                        tstresult = arrayAppend("True", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                                        fndExcep = -1;
                                    }
                                }
                                else
                                {
                                    TextFileOps.Write(pth, "Verifying that the  " + thsItem + " image is not present", 1);
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
                                tmpString = "Failed finding image: " + e.Message;
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
                                        dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, "");

                                        if (currTime.Seconds >= 6)
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
                                        tmpString = "The " + thsItem + " object was not found......";
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

                        #region vfyNote
                        case "vfyNote":
                        {
                            vfyNote(pth, inArray[0]);
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
                                        dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, "");

                                        if (currTime.Seconds >= 6)
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
                                        tstresult = arrayAppend("True", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
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

                                tmpString = "Searching for the " + inArray[4] + " link........(" + stpNum + ")";
                                tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

                                //section off the scenarios if the field is (!=N) or is not (==N) present on the screen
                                if (inArray[2] != "N" && inArray[2] != "n")
                                {
                                    do
                                    {
                                        dispObj = WaitUntil(tstObj, inArray[0], out objString, out currTime, inArray[3]);

                                        //break the loop if looking for more than 6 seconds
                                        if (currTime.Seconds >= 6)
                                        {
                                            dispObj = false;
                                            break;
                                        }

                                    }
                                    while (dispObj != true);

                                    if (dispObj == true)
                                    {
                                        tmpString = "Found the " + thsItem + " link......";
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
                                                " link", dispObj, stpNum, pth, ref tstresult, out fndExcep, out tstFail);
                                        }
                                        //driver.FindElement(By.XPath(inArray[0])).Click();
                                    }
                                    else
                                    {
                                        tmpString = "The " + thsItem + " link was not found......";
                                        tstresult = arrayAppend("True", tmpString, "-1", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
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

                    if (doFunc != "vfyNote")
                    {
                        if (doFunc != "Logout")
                            tmpString = "Operation - " + doFunc + ":  Excel line number - " + lnNum;
                        else
                            tmpString = "Operation - " + doFunc;

                        tstresult = arrayAppend("True", tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);

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
            private bool WaitUntil(tstObject tstObj, string inPath, out string outString, out TimeSpan elapsedTime, string objVis)
            {
                Stopwatch stopwatch;
                bool objPres;
                outString = "";
                objPres = false;

                stopwatch = new Stopwatch();

                //start a stopwatch. This will break out after 6 seconds of looking
                stopwatch.Start();
                do
                {
                    //check to see if an object is present on the screen
                    objPres = tstObj.IsElementPresent(By.XPath(inPath));

                    elapsedTime = stopwatch.Elapsed;

                    if (elapsedTime.Seconds >= 6)
                    {
                        //check to see if the object is not supposed to be present
                        if (objVis == "N")
                        {
                            objPres = true;
                        }
                        else
                        {
                            //set objPres to false and send an error message back to the calling function
                            objPres = false;
                            outString = "ERROR: The object was not found after six seconds \r\n" + inPath;
                        }

                        break;
                    }

                } while (objPres != true);

                //stop the stopwatch 
                stopwatch.Stop();

                //send objPres back to the calling function
                return objPres;
            }
            #endregion

            #region objVerify
            private bool objVerify(string vfyString, string fldName, string getNeg, ref int tstFail)
            {
                bool objPres;

                if (vfyString.Trim() == fldName.Trim() && getNeg != "N")
                {
                    objPres = true;
                    tstFail = 0;
                }
                else if (vfyString.Trim() == fldName.Trim() && getNeg == "N")
                {
                    objPres = true;
                    tstFail = -1;
                }
                else if (vfyString.Trim() != fldName.Trim() && getNeg == "N")
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
            public static string[,] arrayAppend(string arg0, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string[,] inList)
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

            #region parseTooltip
            private string parseTooltip(string instring)
            {
                string outstring;
                string tempstring;
                string thsChar;
                int strlen;
                int flag;

                //set variables
                flag = 0;
                outstring = "";
                tempstring = "";

                //set the length of the instring to run through then for loop
                strlen = instring.Length;

                for (int x = 0; x < strlen; x++)
                {
                    //set thsChar to the 'x' charcater in instring
                    thsChar = instring.Substring(x, 1);

                    //look for the first single quote. The flag variable is when the second single quote in the string
                    //is encountered. the flag variable ends the composition of the tempstring and the loop is broken
                    if (thsChar == "'" && flag != 1)
                    {
                        //nested for loop to compose the tempstring
                        for (int y = x + 1; y < strlen; y++)
                        {
                            //set the flag variable and break the loop
                            if (instring.Substring(y, 1) == "'")
                            {
                                flag = 1;
                                break;
                            }

                            //once flag is set start composing tempstring
                            tempstring = tempstring + instring.Substring(y, 1);
                        }

                        //once the flag is set the there is no need to continue.End loop
                        if (flag == 1)
                            break;
                    }
                }

                //set tempstring to outstring to send back to the calling function
                outstring = tempstring;
                return outstring;
            }
            #endregion

            #region tblSearch
            private int tblSearch(tstObject tstObj, string schPath, string schString, string suffix, int colNum)
            {
                int outNum;
                int numRows;
                int numCols;
                string fnlPath;
                string thsEntry;

                fnlPath = "";
                outNum = 0;

                //get the number of columns in the table. One column means a table with no information
                numCols = driver.FindElements(By.XPath(schPath + "/td")).Count;

                //process the table if numCols != 1
                if (numCols != 1)
                {
                    numRows = driver.FindElements(By.XPath(schPath)).Count;

                    for (int x = 0; x < numRows; x++)
                    {
                        //construct the final table entry xpath with indexes and suffixes
                        if (x == 0 && colNum == 1)
                            fnlPath = schPath + "/td" + suffix;
                        else if (x > 0 && colNum == 1)
                            fnlPath = schPath + "[" + (x + 1).ToString() + "]/td" + suffix;
                        else if (x == 0 && colNum > 1)
                            fnlPath = schPath + "/td[" + colNum.ToString() + "]" + suffix;
                        else if (x > 0 && colNum > 1)
                            fnlPath = schPath + "[" + (x + 1).ToString() + "]/td[" + colNum.ToString() + "]" + suffix;

                        //get the text of the entry and format by stripping off any extraneous info
                        thsEntry = driver.FindElement(By.XPath(fnlPath)).Text;

                        thsEntry = parseEntry(thsEntry, schString);

                        //compare this entry with the expected entry
                        if (thsEntry.Trim() == schString.Trim())
                        {
                            outNum++;
                        }

                    }
                }

                return outNum;
            }
            #endregion

            #region parseEntry
            private string parseEntry(string instring, string chkString)
            {
                int strLen;
                string outString;
                string vfyString;
                
                //set variables
                outString = instring;
                vfyString = "";

                //get the length of the string sent in
                strLen = instring.Length;

                //scroll through the instring comaring to chkString. Looking to get rid of
                //extraneous entries '(period #)...any extraneous text
                for (int x = 0; x <= strLen - chkString.Length; x++)
                {
                    vfyString = instring.Substring(x, chkString.Length);

                    if (vfyString == chkString)
                    {
                        outString = vfyString;
                        break;
                    }
                }

                    return outString;
            }
            #endregion

            #region getActiveTab
            private string getActiveTab(string tabListPath)
            {
                int numTabs;
                string tabPath;
                string activeTab;
                string clsName;

                //set variables
                activeTab = "";

                //get the number of tabs in the app
                numTabs = driver.FindElements(By.XPath(tabListPath)).Count;

                //scroll through the tabs to find the highlighted one 
                for (int x = 1; x <= numTabs; x++)
                {
                    //set the search path for each individual path li for the initial path li[x] for subsequent paths
                    if (x == 1)
                    {
                        tabPath = tabListPath;
                    }
                    else
                    {
                        tabPath = tabPath = tabListPath + "[" + x.ToString() + "]";
                    }

                    //get the class name of the tab being processed
                    clsName = driver.FindElement(By.XPath(tabPath)).GetAttribute("class");

                    //if the class name = 'active-trail' then this is the tab to process
                    if (clsName == "active-trail" || clsName == "active")
                    {
                        activeTab = driver.FindElement(By.XPath(tabPath)).Text;
                        break;
                    }
                }

                return activeTab;
            }
            #endregion

            #region getMonthDiff
            private int getMonthDiff(DateTime lValue, DateTime rValue)
            {
                int outDiff;

                outDiff = (lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year);

                return outDiff;
            }
            #endregion

            #region getMonth
            private int getMonth(string inMonth)
            {
                int outMonth;

                //initialize outMonth
                outMonth = 0;

                //convert the ascript month to an int to be returned to the calling function 
                switch (inMonth)
                {
                    case "January" :
                    {
                        outMonth = 1;
                        break;
                    }
                    case "February":
                    {
                        outMonth = 2;
                        break;
                    }
                    case "March":
                    {
                        outMonth = 3;
                        break;
                    }
                    case "April":
                    {
                        outMonth = 4;
                        break;
                    }
                    case "May":
                    {
                        outMonth = 5;
                        break;
                    }
                    case "June":
                    {
                        outMonth = 6;
                        break;
                    }
                    case "July":
                    {
                        outMonth = 7;
                        break;
                    }
                    case "August":
                    {
                        outMonth = 8;
                        break;
                    }
                    case "September":
                    {
                        outMonth = 9;
                        break;
                    }
                    case "October":
                    {
                        outMonth = 10;
                        break;
                    }
                    case "November":
                    {
                        outMonth = 11;
                        break;
                    }
                    case "December":
                    {
                        outMonth= 12;
                        break;
                    }

                }

                //return the value of the month
                return outMonth;
            }
            #endregion

            #region getTimespan
            private void getTimespan(string instring, ref string numDays, ref string direction)
            {
                char cvtAscii;          //character variable of the string
                int asciiVal;           //int ascii value of the character being processed 
                int strLen;             //length of the instring. 
                string thsChar;         //the current string character being processed
                string tempstring;      //temp string to construct the number of days

                //set the string length variable
                tempstring = "";
                strLen = instring.Length;

                //The instring will be processed in revedrase until the direction can be ascertained
                for (int x = strLen; x > 0; x--)
                {
                    //get thsChar
                    thsChar = instring.Substring(x - 1, 1);
                    cvtAscii = Convert.ToChar(thsChar);
                    asciiVal = Convert.ToInt32(cvtAscii);

                    //construct a tempstring to represent the number of days if the ascii value is between 48 and 57
                    if (asciiVal >= 48 && asciiVal <= 57)
                    {
                        tempstring = thsChar + tempstring;
                    }
                    //get the direction of days and break. There is no more to process
                    else if (asciiVal >= 43 && asciiVal <= 45)
                    {
                        direction = thsChar;
                        break;
                    }

                }

                numDays = tempstring;
            }
            #endregion

            #region getGradeInput
            private void getGradeInput(string instring, ref string outParam1, ref string outParam2, ref string outParam3)
            {
                int strLen;
                int numParam;
                int endLoop;
                string tempstring;
                string thsChar;
                string getChar;

                numParam = 0;
                endLoop = 0;
                tempstring = "";
                strLen = instring.Length;

                for (int x = 0; x < strLen; x++)
                {
                    //get the current character being processed
                    thsChar = instring.Substring(x, 1);

                    if (thsChar == "(")
                    {
                        numParam = 1;
                        //after a parenthesis has been found, increment y to the next character
                        for (int y = x + 1; y < strLen; y++)
                        {
                            //get  char = the y character in the string
                            getChar = instring.Substring(y, 1);

                            //looking for a comma (end of parameter) or end parenthesis (end of parameter string)
                            if (getChar == "," || getChar == ")")
                            {
                                //switch statement to get the first, second and fourth params in the parameter string 
                                switch (numParam)
                                {
                                    case 1:
                                    {
                                        //assign the first parameter
                                        outParam1 = tempstring;
                                        break;
                                    }
                                    case 2:
                                    {
                                        //assign the second parameter
                                        outParam2 = tempstring;
                                        break;
                                    }
                                    case 4:
                                    {
                                        //assign the fourth parameter
                                        outParam3 = tempstring;

                                        //set the exit loop variable
                                        endLoop = 1;
                                        break;
                                    }

                                }

                                //increment the numParam variable when a comma is encountered
                                numParam++;

                                //reset tempstring
                                tempstring = "";
                            }
                            else
                            {
                                //add getChar to tempstring when the assign conditions aren't met 
                                tempstring = tempstring + getChar;
                            }

                            // when the fourth parameter is set to outParamthe exit var is set. 
                            //Break nested for loop from here
                            if (endLoop == 1)
                            {
                                break;
                            }

                        }
                    }

                    //exit loop
                    if (endLoop == 1)
                    {
                        endLoop = 0;
                        break;
                    }
                }
            }
            #endregion
            
            #region selectTerm
            private int selectTerm(tstObject tstObj, SelectElement selection, string inSelection, int inIndex, bool objPres, int rsltFlag, ref string[,] tstresult)
            {
                int fndItem;

                if (rsltFlag == 1)
                {
                    tmpString = "Found list item [" + inSelection + "]";
                    tstresult = arrayAppend(objPres.ToString(), tmpString, "80", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, tstresult);
                }

                fndItem = 1;
                selection.SelectByIndex(inIndex);

                return fndItem;
            }
            #endregion

            #region trimString
            private string trimString(string inPath)
            {
                int strLen;
                int fndSlash;
                string thsChar;
                string outstring;

                //instatiate variables
                fndSlash = 0;
                outstring = String.Empty;

                //get the length of the path passed in
                strLen = inPath.Length;

                //scroll through the path. looking fpor the second slash
                for (int x = strLen; x >= 0; x--)
                {
                    //curren6t character being processed
                    thsChar = inPath.Substring(x - 1, 1);

                    //if a '/' is encountered increment fndSlash
                    if (thsChar == "/")
                        fndSlash++;

                    if (fndSlash == 2)
                    {
                        outstring = inPath.Substring(0, x - 1);
                        break;
                    }
                }

                return outstring;
            }
            #endregion

        #endregion
    }
    #endregion
}
#endregion
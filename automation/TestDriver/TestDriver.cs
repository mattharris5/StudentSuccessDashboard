using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit;
using clsNavSite;
using Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace Driver
{
    class TestDriver
    {
        //bool result;
        static void Main(string[] args)
        {
            string[,] stpArray;
            string[,] dataArray;
            string[,] fnlArray;
            int[] itmNumArray;
            int dataIndex;
            tstObject_FF tstObj = new tstObject_FF();

            //Get the list of steps this test will use
            stpArray = tstObj.getXLData();

            //Call get the number of columns in the data sheet to dimension the data array in order to send it out to the function
            dataIndex = tstObj.getMaxDataCols(stpArray.GetLength(0), stpArray, out itmNumArray);
            
            //initialize dataArray and send off to populate with sheet data
            dataArray = new string[stpArray.GetLength(0), dataIndex + 2];

            //get the fnlArray data from the spreadsheet
            fnlArray = new string[itmNumArray.GetLength(0),dataIndex];

            fnlArray = tstObj.getArrayData(stpArray, dataIndex);

            //Once data Array is populated it will contain the following information
            //[0] - Step Name
            //[1] - Step Line
            //[2] - Ubound[dataArray] - the data frome the sheet to be passed to the destination function

            for (int x = 0; x < stpArray.GetLength(0); x++)
            {
                int b = 2;
                dataArray[x, 0] = stpArray[x, 0];
                dataArray[x, 1] = stpArray[x, 1];
                for (int a = 0; a < dataIndex; a++)
                {
                    if (fnlArray[x, a] != null)
                    {
                        dataArray[x, b] = fnlArray[x, a];
                        b++;
                    }
                    else
                    {
                        b = 0;
                        break;
                    }
                }
            }


            //Login to Echo
            tstObj.Login(dataArray[0, 2], dataArray[0, 3]);                             //Login to the applic

            //Navigate to the Courses tab
            tstObj.navLinks(dataArray[1, 2]);

           //Navigate to the Home tab
            tstObj.navLinks(dataArray[2, 2]);

            //Navigate to the Events tab
            tstObj.navLinks(dataArray[3, 2]);

            //Navigate to the Home tab
            tstObj.navLinks(dataArray[4, 2]);

            //Navigate to the Grades tab
            tstObj.navLinks(dataArray[5, 2]);

            //Navigate to the Home tab
            tstObj.navLinks(dataArray[6, 2]);

            //Navigate to the Groups tab
            tstObj.navLinks(dataArray[7, 2]);

            //Navigate to the Home tab
            tstObj.navLinks(dataArray[8, 2]);

            //Navigate to the People tab
            tstObj.navLinks(dataArray[9, 2]);

            //Navigate to the Home tab
            tstObj.navLinks(dataArray[10, 2]);

            //Navigate to the Library tab
            tstObj.navLinks(dataArray[11, 2]);

            //Navigate to the Home tab
            tstObj.navLinks(dataArray[12, 2]);

            //Navigate to the Tools tab
            tstObj.navLinks(dataArray[13, 2]);

            //Navigate to the Home tab
            tstObj.navLinks(dataArray[14, 2]);

            //Navigate to the Log Out tab
            tstObj.navLinks(dataArray[15, 2]);

/*          //Navigate to the Teaching tab
            tstObj.navLinks(dataArray[16, 2]);

            //Navigate to the School Snapshot tab
            tstObj.navLinks(dataArray[17, 2]);

            //Verify text present
            //tstObj.VerifyElements(tstObj);
            */
        }
    }
}

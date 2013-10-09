using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Data.OleDb;

namespace TestDriver_GUI
{
    public partial class dbUpdater : Form
    {
        string[] thsStep;
        CheckBox[] chkArgument;
        TextBox[] lblArgument;
        string[,] dataArray;
        string tstname;
        object missing;
        string tstID;

        #region Constructor
        public dbUpdater(string inTstName)
        {
            tstname = inTstName;
            InitializeComponent();
            lblTestName.Text = lblTestName.Text + tstname;
        }
        #endregion

        #region dbUpdater_Load
        private void dbUpdater_Load(object sender, EventArgs e)
        {
            object[] rtnList;
            string[] funcArray;
            int argCount;
            int stpNum;
            string argID;
            string strSQL;
            string funcName;
            string dataCount;

            //get the test id
            strSQL = "SELECT id FROM test WHERE name = '" + tstname + "'";
            rtnList = db_access(strSQL);
            tstID = Convert.ToString(rtnList[0]);

            //get the number of steps
            strSQL = "SELECT COUNT(*) FROM step WHERE test_id = '" + tstID + "'";
            rtnList = db_access(strSQL);
            argCount = Convert.ToInt32(rtnList[0]);
            lblStpNumber.Text = lblStpNumber.Text + " " + Convert.ToString(argCount);

            //set the new array to the 
            funcArray = new string[argCount];
            dataArray = new string[argCount, 3];

            //get 
            for (int x = 0; x < argCount; x++)
            {
                strSQL = "SELECT argument_set_id FROM step WHERE test_id = '" + tstID + "' AND number = " + (x + 1).ToString();
                rtnList = db_access(strSQL);
                argID = Convert.ToString(rtnList[0]);

                //get the function name, step number, and argument count
                strSQL = "SELECT number, name, (SELECT COUNT(*) FROM argument WHERE argument_set_id = '" + argID + "') FROM step WHERE test_id = '" +
                    tstID + "' AND number = " + (x + 1).ToString();
                rtnList = db_access(strSQL);
                stpNum = Convert.ToInt32(rtnList[0]);
                funcName = Convert.ToString(rtnList[1]);
                dataCount = Convert.ToString(rtnList[2]);

                dataArray[x, 0] = Convert.ToString(stpNum);
                dataArray[x, 1] = funcName;
                dataArray[x, 2] = dataCount;

                //format the results and set to funcArray
                funcArray[x] = "(Step " + Convert.ToInt32(stpNum) + ") " + funcName + "   (" + dataCount + ")";
            }

            //add the function/step name into the form
            foreach (string item in funcArray)
            {
                cmbFuncName.Items.Add(item);
            }

        }
        #endregion

        #region btnCancel_Click
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region dbUpdater_CheckedChanged
        private void dbUpdater_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Convert.ToInt32(thsStep[2]); i++)
            {
                if (chkArgument[i].Checked == true)
                {
                    lblArgument[i].Enabled = true;
                }
                else
                {
                    lblArgument[i].Enabled = false;
                }
            }
        }
        #endregion

        #region chkBoxNumChecked
        private object[,] chkBoxNumChecked(CheckBox[] chkArgument)
        {
            object[,] chkNum;
            int numBoxes;
            int idx;

            idx = 0;
            numBoxes = 0;

            //get the numnber of unchecked boxes and instantiate an array
            for(int i = 0; i < chkArgument.Length; i++)
            {
                if (chkArgument[i].Checked == true)
                {
                    numBoxes++;
                }
            }

            chkNum = new object[numBoxes, 2];

            //fill the array with the index number and data item
            for (int i = 0; i < chkArgument.Length; i++)
            {
                if (chkArgument[i].Checked == true)
                {
                    chkNum[idx, 0] = i;
                    chkNum[idx, 1] = lblArgument[i].Text;
                    idx++;
                }
            }

            return chkNum;
        }
        #endregion

        #region cmbFuncName_Selected_Changed
        private void cmbFuncName_Selected_Changed(object sender, EventArgs e)
        {
            object[] rtnList;
            string[] argArray;
            int stpNum;
            string argID;
            string tstID;
            string slctItem;
            string funcName;
            string strSQL;
            int dataNum;
            
            //set slctItem to the selected item
            slctItem = cmbFuncName.SelectedItem.ToString();

            thsStep = new string[3];

            //set the step data(step number, function name, arg number) for the selected function to thsStep
            for (int x = 0; x < 3; x++)
                thsStep[x] = dataArray[cmbFuncName.SelectedIndex, x];

            stpNum = Convert.ToInt32(thsStep[0]);
            funcName = thsStep[1];
            dataNum = Convert.ToInt32(thsStep[2]);

            //get the test ID
            strSQL = "SELECT id FROM test WHERE name = '" + tstname + "'";
            rtnList = db_access(strSQL);
            tstID = rtnList[0].ToString();

            //get the argument set id
            strSQL = "SELECT argument_set_id FROM step WHERE (test_id = '" + tstID + "' AND number = " + stpNum + ")";
            rtnList = db_access(strSQL);
            argID = rtnList[0].ToString();
            argArray = new string[dataNum];

            //get the arguments to change
            strSQL = "SELECT value FROM argument WHERE argument_set_id = '" + argID + "'";
            rtnList = db_access(strSQL);

            //populate an array of the data arugments
            for (int x = 0; x < dataNum; x++)
                argArray[x] = Convert.ToString(rtnList[x]);

            drawLabels(dataNum, argArray);

        }
        #endregion

        #region btnOK_Click
        private void btnOK_Click(object sender, EventArgs e)
        {
            ADODB.Connection objCon;
            DialogResult update;
            string[,] argArray;
            object[,] idxUpdate;
            object[] rtnList;
            object[] exTest;
            int numTests;
            string tstList;
            string argID;
            string itmList;
            string msgString;
            string tstID;
            string strCon;
            string strSQL;

            objCon = new ADODB.Connection();
            itmList = "";
            tstList = "";
            strCon = "driver={MySQL ODBC 5.1 Driver};server=107.22.232.228;uid=qa_people;pwd=thehandcontrols;" +
                "database=functional_test_data;option=3";

            //get the number of checkboxes that are checked from dbUpdater form 
            idxUpdate = chkBoxNumChecked(chkArgument);

            for (int j = 0; j < idxUpdate.GetLength(0); j++)
                itmList = itmList + idxUpdate[j, 0] + "\t" + idxUpdate[j, 1] + "\r\n";

            //dimension an array for the arguments will be going into the database
            argArray = new string[idxUpdate.GetLength(0), 2];

            for (int z = 0; z < argArray.GetLength(0); z++)
            {
                argArray[z, 0] = idxUpdate[z, 0].ToString();
                argArray[z, 1] = lblArgument[Convert.ToInt32(idxUpdate[z, 0])].Text;
            }

            //get the test ID
            strSQL = "SELECT id FROM test WHERE name = '" + tstname + "'";
            rtnList = db_access(strSQL);
            tstID = rtnList[0].ToString();

            //get the argument set id
            strSQL = "SELECT argument_set_id FROM step WHERE (test_id = '" + tstID + "' AND number = " + thsStep[0] + ")";
            rtnList = db_access(strSQL);
            argID = rtnList[0].ToString();

            strSQL = "SELECT COUNT(*) FROM step WHERE argument_set_id = '" + argID + "'";
            rtnList = db_access(strSQL);
            numTests = Convert.ToInt32(rtnList[0]);

            if (numTests <= 1)
            {
                if (numTests != 0)
                {
                    //get the argument set id
                    strSQL = "SELECT test_id FROM step WHERE argument_set_id = '" + argID + "'";
                    rtnList = db_access(strSQL);
                }
            }
            else
            {
                strSQL = "SELECT test_id FROM step WHERE (argument_set_id = '" + argID + "' AND test_id <> (SELECT id FROM test WHERE name = '" + tstname + "'))";
                rtnList = db_access(strSQL);
            }

            if (rtnList.Length != 0)
            {
                for (int k = 0; k < rtnList.Length; k++)
                {
                    strSQL = "SELECT name from test WHERE id = '" + rtnList[k] + "'";
                    exTest = db_access(strSQL);
                    tstList = tstList + exTest[0] + "\r\n";
                }
                msgString = "You will be updating Step " + thsStep[0] + " of the " + tstname +
                    " test.\r\nThe following data items will be updated\r\n\r\nIndex#\tData Item\r\n" + itmList +
                    "\r\nYou will not be able to undo these changes once they are done. " +
                    "The argument set that you are updating is also used in the following tests:\r\n\r\n" + tstList +
                    "\r\nAre you sure you want to Continue?\r\n\r\nPress 'Yes' to Update. Press 'No' to exit";
            }
            else
            {
                msgString = "You will be updating Step " + thsStep[0] + " of the " + tstname +
                    " test.\r\nThe following data items will be updated\r\n\r\nIndex#\tData Item\r\n" + itmList +
                    "\r\nYou will not be able to undo these changes once they are done. " +
                    "The argument set that you are updating is not used in any other tests>" +
                    "\r\nAre you sure you want to Continue?\r\n\r\nPress 'Yes' to Update. Press 'No' to exit";
            }
            update = MessageBox.Show(msgString, "Database Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (update == DialogResult.Yes)
            {
                objCon.Open(strCon);

                for (int x = 0; x < argArray.GetLength(0); x++)
                {
                    strSQL = "UPDATE argument SET value='" + argArray[x, 1] + "' WHERE argument_set_id = '" + argID + "' AND seq = " 
                        + (Convert.ToInt32(argArray[x, 0]) + 1).ToString();
                    objCon.Execute(strSQL, out missing, 0);
                }

                objCon.Close();

                MessageBox.Show("Database rows updated.\r\nDone! ", "Database Updater", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
        }
        #endregion

        #region drawLabels
        private void drawLabels(int numLabels, string[] argArray)
        {
            int chkBox_x = 6;
            int chkBox_y = 25;
            int lbl_x = 30;
            int lbl_y = 22;

            chkArgument = new CheckBox[numLabels];
            lblArgument = new TextBox[numLabels];

            gpDataItems.Controls.Clear();

            for (int i = 0; i < numLabels; i++)
            {
                chkArgument[i] = new CheckBox();
                chkArgument[i].Size = new Size(18, 17);
                chkArgument[i].Location = new Point(chkBox_x, (chkBox_y + (i * 30)));
                chkArgument[i].Text = "";
                chkArgument[i].Checked = false;
                chkArgument[i].CheckedChanged += new EventHandler(dbUpdater_CheckedChanged);

                lblArgument[i] = new TextBox();
                lblArgument[i].Size = new Size(1506, 22);
                lblArgument[i].Location = new Point(lbl_x, (lbl_y + (i  * 30)));
                lblArgument[i].MaxLength = 450;
                lblArgument[i].Text = argArray[i];
                lblArgument[i].Enabled = false;

                gpDataItems.Controls.Add(chkArgument[i]);
                gpDataItems.Controls.Add(lblArgument[i]);
            }

            resizeForm(numLabels);

            gpDataItems.Visible = true;
        }
        #endregion

        #region resizeForm
        private void resizeForm(int numLabels)
        {
            int boxHeight;
            int frmHeight;

            boxHeight = (numLabels * 30) + 22; // add 12 to account for padding at the top and bottom of the box
            frmHeight = boxHeight + 100;   //add 44 to account for the top padding and 15 for the bottom padding

            if (frmHeight < this.Height)
                frmHeight = this.Height;

            this.Height = frmHeight;
            gpDataItems.Height = boxHeight;
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

            //get a one dimensional array that can be placed into the Test Suite dropdown
            dataSuite = thinArray(dataRows);

            //close the recordset
            objRec.Close();

            //close the database connection
            objCon.Close();

            return dataSuite;
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
    }
}

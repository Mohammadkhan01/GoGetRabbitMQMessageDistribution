using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace WebAPI.Controllers
{
    public class DeviceDetailsController : ApiController
    {
       
        // GET: api/GoGet
         public IEnumerable<string> Get()
        {
           
            return new string[] { "value1", "value2" };
        }

        // GET: api/DeviceDetails/1
        public DeviceDetails Get(int id)
        {
            string spath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data");
            //*odbc connection to the csv file
            string strConnString = "Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=" + spath.Trim() + ";Extensions=asc,csv,tab,txt;Persist Security Info=False";
            //Object Creation which will alow connection to alive
            OdbcConnection objCSV = new OdbcConnection(strConnString);
            //Same as sql command , difference is we have to write file name 
            OdbcCommand Cmd = new OdbcCommand("select * from SimpleDB.csv ", objCSV);
            objCSV.Open();
            OdbcDataReader rdr = Cmd.ExecuteReader();
            DeviceDetails _person = new DeviceDetails();
            //Using JavaScriptSerializer to convert response into jason.
             JavaScriptSerializer json = new JavaScriptSerializer();
            //If data is avaialable and ODBC data reader can read those data..
            while (rdr.Read())
            {
                //if deviceID equal to id(passad as paramter)
                if (Convert.ToInt32(rdr["deviceId"].ToString()) == id)
                {
                    //We are taking both data from datatable/CSV file
                    _person.deviceId = Convert.ToInt32(rdr["deviceId"]);
                    _person.Details = json.Serialize(rdr["Details"].ToString());
                }
            }
            objCSV.Close();
           //we are returning this object
            return _person;
        }

        // POST: api/GoGet
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/GoGet/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/GoGet/5
        public void Delete(int id)
        {
        }
    }
}

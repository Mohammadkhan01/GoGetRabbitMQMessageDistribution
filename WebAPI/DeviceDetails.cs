using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;


    public class DeviceDetails
    {
        public int deviceId { get; set; }
        public string Details { get; set; }
    }
    public class createDeviceDetails:DeviceDetails
    {

    }
    public class ReadDeviceDetails:DeviceDetails
    {
        public ReadDeviceDetails(DataRow row)
        {
            deviceId = Convert.ToInt32(row["deviceId"]);
            Details = row["Details"].ToString();
        }
    
}
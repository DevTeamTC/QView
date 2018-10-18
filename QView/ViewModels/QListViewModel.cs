using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QView.Models;
using QView.App_Data;

namespace QView.ViewModels
{
    public class QListViewModel
    {
        //public List<que> QRecords;
        public List<QRecord> QRecords = new List<QRecord>();
    }
}
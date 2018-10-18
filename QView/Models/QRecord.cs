using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace QView.Models
{
    public class QRecord
    {
        [Display(Name="ID")]
        public int Id {get; set;}
        public string Company { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Created { get; set; }
        public string LastScheduled { get; set; }
        public string PreviousScheduled { get; set; }
        public string FilePath { get; set; }

        public QRecord()
        {
            FilePath = null;
        }
    }


}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models.AccountViewModels
{
    public class TestingViewModel
    {
        public string Id { get; set; }

        //[DataType(DataType.Date), DisplayFormat(DataFormatString =]
        public DateTime birthDate { get; set; }
    }
}

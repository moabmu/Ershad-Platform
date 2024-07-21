﻿using ModepEduYanbu.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models
{
    public class EduProgramUploadedFile : IAzureFile
    {
        public string EduProgramUploadedFileId { get; set; }
        public string Filename { get ; set ; }
        public string Extension { get ; set ; }
        public string Uri { get ; set ; }
        public string FileTitle { get ; set ; }
        public string AzureContainer { get ; set ; }
        public string AzureBlobName { get ; set ; }
        public DateTime UploadedDate { get ; set ; }

        public EduProgram EduProgram { get; set; }
        public string EduProgramId { get; set; }
    }
}

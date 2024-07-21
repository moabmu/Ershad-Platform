using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace ModepEduYanbu.Helpers
{
    public class UploadedFileHelpers
    {
        public static Dictionary<string, string> mimeImages =
            new Dictionary<string, string>() {
                { "image/jpeg", "jpg;jpeg" },
                { "image/png","png" },
                { "image/gif","gif" }
            };
        public static Dictionary<string, string> mimeVideos =
            new Dictionary<string, string>()
            {
                { "video/mpeg","mpeg" },
                { "video/3gpp","3gpp" },
                { "video/x-msvideo","avi" },
                { "video/x-ms-wmv","wmv" },
                { "video/mp4","mp4" }
            };
        public static Dictionary<string, string> mimeDocuments =
            new Dictionary<string, string>()
            {
                { "application/pdf","pdf" },
                { "application/vnd.ms-powerpoint","ppt" },
                { "application/vnd.openxmlformats-officedocument.presentationml.presentation","pptx" },
                { "application/msword","doc" },
                { "application/vnd.openxmlformats-officedocument.wordprocessingml.document","docx" },
                { "application/vnd.ms-excel","xls" },
                { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","xlsx" }
            };
        public enum FileType
        {
            Document = 1,
            Image = 2,
            Video = 4,
            Other = 8
        }

        public enum FileSize
        {
            OneMB = 1048576,
            TenMB = 10485760,
            TwentyFiveMB = 26214400,
            SeventyFiveMB = 78643200,
            TwoHundredMB = 209715200
        }


        public static async Task<bool> ValidateFormFile(
            IFormFile formFile,
            ModelStateDictionary modelState,
            FileType requiredType,
            int maximumSize,
            Type modelType) // assign it by typeof(object)
        {
            var result = true;

            if(formFile == null)
            {
                //modelState.AddModelError(String.Empty, "لا توجد ملفات مرفقة للرفع.");
                //return result = false;

                return result;
            }

            var fieldDisplayName = string.Empty;

            #region Commented
            // Use reflection to obtain the display name for the model 
            // property associated with this IFormFile. If a display
            // name isn't found, error messages simply won't show
            // a display name.
            //MemberInfo property =
            //    modelType.GetProperty(formFile.Name.Substring(formFile.Name.IndexOf(".") + 1));

            //if (property != null)
            //{
            //    var displayAttribute =
            //        property.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;

            //    if (displayAttribute != null)
            //    {
            //        fieldDisplayName = $"{displayAttribute.Name} ";
            //    }
            //}
            #endregion

            // Use Path.GetFileName to obtain the file name, which will
            // strip any path information passed as part of the
            // FileName property. HtmlEncode the result in case it must 
            // be returned in an error message.
            var fileName = WebUtility.HtmlEncode(Path.GetFileName(formFile.FileName));
            //if (formFile.ContentType.ToLower() != "text/plain")
            //{
            //    modelState.AddModelError(formFile.Name,
            //                             $"The {fieldDisplayName}file ({fileName}) must be a text file.");
            //}

            var fileContentType = formFile.ContentType.ToLower();
            var requiredFileType = "";
            if ((requiredType & FileType.Image) == FileType.Image)
            {
                if(!mimeImages.Any(t => t.Key == fileContentType))
                {
                    //modelState.AddModelError(formFile.Name,
                    //    $"{fieldDisplayName} ({fileName}) يجب أن يكون ملف صورة.");
                    result = false;
                    requiredFileType = "صورة";
                }
                else
                {
                    result = true;
                    goto fileTypeMsg;
                }
            }

            if ((requiredType & FileType.Document) == FileType.Document)
            {
                if (!mimeDocuments.Any(t => t.Key == fileContentType))
                {
                    //modelState.AddModelError(formFile.Name,
                    //    $"{fieldDisplayName} ({fileName}) يجب أن يكون ملف مستند.");
                    result = false;
                    requiredFileType +=((requiredFileType == "")?"":" أو ") + "مستند";
                }
                else
                {
                    result = true;
                    goto fileTypeMsg;
                }
            }

            if ((requiredType & FileType.Video) == FileType.Video)
            {
                if (!mimeVideos.Any(t => t.Key == fileContentType))
                {
                    //modelState.AddModelError(formFile.Name,
                    //    $"{fieldDisplayName} ({fileName}) يجب أن يكون ملف فيديو.");
                    result = false;
                    requiredFileType += ((requiredFileType == "") ? "" : " أو ") + "فيديو";
                }
                else
                {
                    result = true;
                    goto fileTypeMsg;
                }
            }

            fileTypeMsg:
            if(result == false)
            {
                modelState.AddModelError(formFile.Name, $"نوع الملف يحب أن يكون {requiredFileType}.");
                return result;
            }

            // Check the file length and don't bother attempting to
            // read it if the file contains no content. This check
            // doesn't catch files that only have a BOM as their
            // content, so a content length check is made later after 
            // reading the file's content to catch a file that only
            // contains a BOM.
            if (formFile.Length == 0)
            {
                //modelState.AddModelError(formFile.Name, $"{fieldDisplayName} ({fileName}) هو ملف فارغ.");
                modelState.AddModelError(formFile.Name, "الملف المرفق فارغ.");
            }
            else
            {
                if(formFile.Length > maximumSize)
                {
                    modelState.AddModelError(formFile.Name, $"الحجم الأقصى للملف هو {((maximumSize)/1024.0/1024.0)} ميغابايت");
                }
                #region Commented
                //try
                //{
                //    string fileContents;

                //    // The StreamReader is created to read files that are UTF-8 encoded. 
                //    // If uploads require some other encoding, provide the encoding in the 
                //    // using statement. To change to 32-bit encoding, change 
                //    // new UTF8Encoding(...) to new UTF32Encoding().
                //    using (
                //        var reader =
                //            new StreamReader(
                //                formFile.OpenReadStream(),
                //                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true),
                //                detectEncodingFromByteOrderMarks: true))
                //    {
                //        fileContents = await reader.ReadToEndAsync();

                //        // Check the content length in case the file's only
                //        // content was a BOM and the content is actually
                //        // empty after removing the BOM.
                //        if (fileContents.Length > 0)
                //        {
                //            return fileContents;
                //        }
                //        else
                //        {
                //            modelState.AddModelError(formFile.Name,
                //                                     $"The {fieldDisplayName}file ({fileName}) is empty.");
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    modelState.AddModelError(formFile.Name,
                //                             $"The {fieldDisplayName}file ({fileName}) upload failed. " +
                //                             $"Please contact the Help Desk for support. Error: {ex.Message}");
                //    // Log the exception
                //}
                #endregion
            }

            //return string.Empty;
            return result;
        }

        public static FileType GetFileType(string fileExtension)
        {
            if (mimeDocuments.Any(t => t.Value == fileExtension.ToLower()))
            {
                return FileType.Document;
            }
            else if (mimeImages.Any(t => t.Value.Split(';').Any( x => x== fileExtension.ToLower())))
            {
                return FileType.Image;
            }
            else if(mimeVideos.Any(t => t.Value == fileExtension.ToLower()))
            {
                return FileType.Video;
            }
            else
            {
                return FileType.Other;
            }
        }
    }
}

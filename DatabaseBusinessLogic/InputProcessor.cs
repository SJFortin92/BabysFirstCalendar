﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;

namespace BabysFirstCalendar.DatabaseBusinessLogic
{
    public static class InputProcessor
    {
        //Verifies the file is an approved image type
        public static bool IsImage(HttpPostedFileBase image)
        {
            try
            {
                //Establish the allowed formats
                var allowedFormats = new[]
                {
                ImageFormat.Jpeg,
                ImageFormat.Png,
                ImageFormat.Bmp
            };

                using (var img = Image.FromStream(image.InputStream))
                {
                    return allowedFormats.Contains(img.RawFormat);
                }
            }
            catch { }
            return false;
        }

        //Verifies that the image is under 1MB
        public static bool IsRightSize(HttpPostedFileBase image)
        {
            if (image.ContentLength > 1 * 1024 * 1024)
                return false;
            
            else
                return true;
            
        }

        //This function validates that the user uploaded file is truly a web image
        //Returns false if not
        //Written so it can be iterated through multiple files in the future
        public static bool ImageValidation(HttpFileCollectionBase file, int i)
        {
            HttpPostedFileBase image = file[i];

            //If there is no image file, return false
            if (image == null)
                return false;

            //If both IsRightSize and IsImage are true, return true
            else if (IsRightSize(image) && IsImage(image))
                return true;
            
            //Otherwise, return false
            else
                return false;

        }
    }
}
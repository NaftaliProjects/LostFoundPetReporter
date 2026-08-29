using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Models
{
    public class ImageToAnimalDescriptionRequest
    {
        public List<string> PictureBase64List { get; set; } = new();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VibeAttack.Models
{
    public class PictureModel
    {
        public IEnumerable<HttpPostedFileBase> Files { get; set; }
    }
}
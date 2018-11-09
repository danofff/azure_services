using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFaceIdApp
{
    public class UserModel
    {
        [Key]
        public string ChatId { get; set; }
        public string UserName { get; set; }
        public bool? Gender { get; set; } = null;
        public int? Age { get; set; } = null;
        public string Photo { get; set; } = null;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TelegramBot.Model
{
    class Category
    {
        [Key]
        public int id { get; set; }

        public string name { get; set; }
    }
}

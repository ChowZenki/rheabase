using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib;

namespace AccountServer
{
    [Table(Name = "accounts")]
    public class Account
    {
        [Column(Name = "account_id", IsPrimaryKey = true)]
        public int AccountID { get; set; }

        [Column(Name = "username")]
        public string Username { get; set; }

        [Column(Name = "password")]
        public string Password { get; set; }

        [Column(Name = "sex", DbType = "int")]
        public Sex Sex { get; set; }

        [Column(Name = "email")]
        public string Email { get; set; }

        [Column(Name = "group")]
        public int Group { get; set; }

        [Column(Name = "state")]
        public int State { get; set; }

        [Column(Name = "bantime", CanBeNull = true)]
        public DateTime? BanTime { get; set; }
    }
}

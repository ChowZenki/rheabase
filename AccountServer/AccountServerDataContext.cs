using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbLinq.Data.Linq;
using DbLinq.MySql;

namespace AccountServer
{
    public class AccountServerDataContext : MySqlDataContext
    {
        public Table<Account> Accounts { get; private set; }

        public AccountServerDataContext(IDbConnection connection)
            : base(connection)
        {
            Accounts = GetTable<Account>();
        }
    }
}

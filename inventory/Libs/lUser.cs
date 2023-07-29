using inventory.Manager;
using Npgsql;
using System.Data;
using inventory.Data.Models;
using inventory.Data.Controllers;

namespace inventory.Libs
{
    public class lUser
    {
        private lDbConn dbconn = new lDbConn();
        private BaseController bc = new BaseController();

        internal List<dynamic> ReadUser(String iduser)
        {
            var cstrname = dbconn.constringName("idccore");
            var split = "||";
            var schema = "public";

            string spname = "getuser1";
            string p1 = "@iduser" + split + iduser + split + "s";

            return bc.ExecSqlWithReturnCustomSplit(cstrname, split, schema, spname, p1);
        }  

    }
}

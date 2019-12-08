using System.Data;
using System.Data.Common;

namespace Baavgai.Db {
    public static class ExDbConnection {

        public static DbCommand CreateCommand(this DbConnection conn, string sql) {
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            return cmd;
        }

        public static DbCommand CreateCommandSp(this DbConnection conn, string procName) {
            var cmd = conn.CreateCommand(procName);
            cmd.CommandType = CommandType.StoredProcedure;
            var p = cmd.CreateParameter();
            p.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(p);
            return cmd;
        }
    }
}

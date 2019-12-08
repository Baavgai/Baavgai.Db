using System;
using System.Data;
using System.Data.Common;

namespace Baavgai.Db {
    public static class DbParamDef {
        public static Func<DbCommand, DbParameter> MkDef(string parameterName, DbType dbType) {
            return (cmd) => {
                var p = cmd.CreateParameter();
                p.ParameterName = parameterName;
                p.DbType = dbType;
                return p;
            };
        }

        public static Func<DbCommand, DbParameter> MkDef(string parameterName, object paramValue, bool isNullable = true) {
            return (cmd) => {
                var p = cmd.CreateParameter();
                p.ParameterName = parameterName;
                p.IsNullable = isNullable;
                if (paramValue == null) {
                    p.Value = DBNull.Value;
                } else {
                    if (paramValue.GetType() == typeof(String)) { p.Size = (paramValue as string).Length + 1; }
                    p.Value = paramValue;
                }
                return p;
            };
        }
        public static Func<DbCommand, DbParameter> MkDef(string parameterName, string paramValue, bool isNullable = true) {
            return (cmd) => {
                var p = cmd.CreateParameter();
                p.ParameterName = parameterName;
                p.IsNullable = isNullable;
                if (paramValue == null) {
                    p.Value = DBNull.Value;
                } else {
                    p.Value = paramValue;
                    p.Size = paramValue.Length + 1;
                }
                return p;
            };
        }

        public static Func<DbCommand, DbParameter> MkDef(string parameterName, DbType dbType, string sourceColumn, bool isNullable = true) {
            return (cmd) => {
                var p = MkDef(parameterName, dbType)(cmd);
                p.IsNullable = isNullable;
                p.SourceColumn = sourceColumn;
                return p;
            };
        }
    }
}

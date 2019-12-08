using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Baavgai.Db {
    public static class ExDbCommand {
        public static int SafeExec(this DbCommand cmd) {
            int returnValue = 0;
            try {
                cmd.Connection.Open();
                returnValue = cmd.ExecuteNonQuery();
            } finally {
                cmd.Connection.Close();
            }
            return returnValue;
        }

        public static object SafeExecResult(this DbCommand cmd) {
            object result = null;
            try {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                var found = cmd.Parameters
                    .Cast<DbParameter>()
                    .Where(x => x.Direction == ParameterDirection.ReturnValue)
                    .First();
                if (found != null) {
                    result = found.Value;
                }
            } finally {
                cmd.Connection.Close();
            }
            return result;
        }


        public static object GetSingleValue(this DbCommand cmd) {
            object val = null;
            try {
                cmd.Connection.Open();
                val = cmd.ExecuteScalar();
            } finally {
                cmd.Connection.Close();
            }
            return val;
        }

        public static object GetSingleValue(this DbCommand cmd, object defaultValue) {
            object val = cmd.GetSingleValue();
            return (val == null) ? defaultValue : val;
        }

        private static int ObjToInt(object x) =>
            x == null
            ? -1
            : ((int.TryParse(x.ToString(), out int n)) ? n : -1);


        public static int SafeExecResultInt(this DbCommand cmd) =>
            ObjToInt(cmd.SafeExecResult());

        public static int GetSingleInt(this DbCommand cmd) =>
            ObjToInt(cmd.GetSingleValue());

        public static List<Tuple<string, Type>> GetSchemaColumns(this DbCommand cmd) {
            try {
                cmd.Connection.Open();
                var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly);
                return Enumerable
                    .Range(0, reader.FieldCount)
                    .Select(i => new Tuple<string, Type>(reader.GetName(i), reader.GetFieldType(i)))
                    .ToList();
            } finally {
                try { cmd.Connection.Close(); } catch { }
            }
        }


        public static IEnumerable<IDataRecord> GetRecords(this DbCommand cmd, int batchSize = -1) {
            Func<DbDataReader, bool> chk = (x) => { return x.Read(); };
            if (batchSize != -1) { chk = (x) => { return x.Read() && (batchSize-- > 0); }; }
            try {
                cmd.Connection.Open();
                var reader = cmd.ExecuteReader();
                while (chk(reader)) { yield return reader; }
            } finally {
                try { cmd.Connection.Close(); } catch { }
            }
        }


        public static void ProcessData(this DbCommand cmd, Action<IDataRecord> process, int batchSize = -1) {
            foreach (var rec in cmd.GetRecords(batchSize)) { process(rec); }
        }

        public static IEnumerable<T> GetData<T>(this DbCommand cmd, Func<IDataRecord, T> toValue, int batchSize = -1) =>
            cmd.GetRecords(batchSize).Select(x => toValue(x));
        

        public static void LoadValueList<T>(this DbCommand cmd, ICollection<T> results, Func<IDataRecord, T> toValue, int batchSize = -1) =>
            cmd.ProcessData((rec) => results.Add(toValue(rec)), batchSize);

        public static T GetFirstValue<T>(this DbCommand cmd, Func<IDataRecord, T> toValue) {
            T value = default;
            try {
                cmd.Connection.Open();
                DbDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) {
                    value = toValue(reader);
                }
            } finally {
                cmd.Connection.Close();
            }
            return value;
        }

        public static DbCommand AddParams(this DbCommand cmd, params Func<DbCommand, DbParameter>[] paramDefs) {
            foreach (var d in paramDefs) {
                cmd.Parameters.Add(d(cmd));
            }
            return cmd;
        }


        public static DbParameter AddParam(this DbCommand cmd, DbParameter p) {
            cmd.Parameters.Add(p);
            return p;
        }

        public static DbParameter AddParam(this DbCommand cmd, string parameterName, DbType dbType) =>
            cmd.AddParam(DbParamDef.MkDef(parameterName, dbType)(cmd));

        public static DbParameter AddParam(this DbCommand cmd, string parameterName, object paramValue) =>
            cmd.AddParam(DbParamDef.MkDef(parameterName, paramValue)(cmd));

        public static DbParameter AddParam(this DbCommand cmd, string parameterName, string paramValue) =>
            cmd.AddParam(DbParamDef.MkDef(parameterName, paramValue)(cmd));

        public static DbParameter AddParam(this DbCommand cmd, string parameterName, DbType dbType, string sourceColumn, bool isNullable = true) =>
            cmd.AddParam(DbParamDef.MkDef(parameterName, dbType, sourceColumn, isNullable)(cmd));
    }
}

// 5wtM5YxBJ4UN
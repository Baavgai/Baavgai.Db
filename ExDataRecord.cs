using System;
using System.Data;

namespace Baavgai.Db {
    public static class ExDataRecord {

        public static string ValueDump(this IDataRecord rec) {
            var sb = new System.Text.StringBuilder();
            sb.Append("{");
            for (int i = 0; i < rec.FieldCount; i++) {
                if (i != 0) { sb.Append(","); }
                var x = rec.GetValue(i);
                sb.AppendFormat("[{0}=\"{1}\"]", rec.GetName(i), (x == null ? "NULL" : x.ToString()));
            }
            return sb.ToString();
        }

        public static T? GetValueOrNullStruct<T>(this IDataRecord rec, int index) where T : struct {
            if (!rec.IsDBNull(index)) {
                var value = rec.GetValue(index);
                if (value != null) {
                    try {
                        return (T)Convert.ChangeType(value.ToString(), typeof(T));
                    } catch {
                    }
                }
            }
            return null;
        }

        public static T GetValueOrNull<T>(this IDataRecord rec, int index) where T : class {
            if (!rec.IsDBNull(index)) {
                var value = rec.GetValue(index);
                if (value != null) {
                    try {
                        return (T)Convert.ChangeType(value.ToString(), typeof(T));
                    } catch {
                    }
                }
            }
            return null;
        }

        public static T GetValueOr<T>(this IDataRecord rec, T defaultValue, int index) {
            if (!rec.IsDBNull(index)) {
                var value = rec.GetValue(index);
                if (value != null) {
                    try {
                        return (T)Convert.ChangeType(value.ToString(), typeof(T));
                    } catch {
                    }
                }
            }
            return defaultValue;
        }

    }
}

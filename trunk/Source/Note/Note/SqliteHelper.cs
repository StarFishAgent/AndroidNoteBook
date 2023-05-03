using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SQLite;
using Xamarin.Essentials;
using System.IO;
using Xamarin.Forms;
using System.Linq;
using System.Reflection;
using SQLiteNetExtensions.Attributes;

namespace Note
{
    public static class SqliteHelper
    {

        static string StrConn = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Note.db3");
        public static void CreateDb()
        {
            var conn = new SQLiteConnection(StrConn, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true);
            conn.CreateTables<NoteInfo, PicInfo, TextInfo>(CreateFlags.AllImplicit);
            conn.Execute("PRAGMA foreign_keys = ON;");
        }
        public static (DataTable,string) ExecuteQuery(this string StrSql)
        {
            var dt = new DataTable();
            var ErrMsg = "";
            try
            {
                var conn = new SQLiteConnection(StrConn);
                var info = conn.Query<NoteInfo>(StrSql);
                dt = ListToTable(info);
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                dt = new DataTable();
            }
            return (dt, ErrMsg);
        }
        
        public static (string,string) ExecuteNonQueryDel()
        {
            var ErrMsg = "";
            var IsSuccess = "";
            try
            {
                var conn = new SQLiteConnection(StrConn);
                conn.Query<NoteInfo>($"delete from noteinfo");
                //var list = new List<object>();
                //list.Add(Params);
                //var Result = conn.InsertAll(list);

                //IsSuccess = Result > 0 ? "插入成功" : "插入失败";
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
            }
            return (IsSuccess, ErrMsg);
        }
        public static (string, string) ExecuteNonQuery(object Params)
        {
            var ErrMsg = "";
            var IsSuccess = "";
            try
            {

                var conn = new SQLiteConnection(StrConn);

                var list = new List<object>();
                list.Add(Params);
                var Result = conn.InsertAll(list);

                IsSuccess = Result > 0 ? "插入成功" : "插入失败";
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
            }
            return (IsSuccess, ErrMsg);
        }

        #region 转换
        /// <summary>
        /// DataTable转化为List集合
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="dt">datatable表</param>
        /// <param name="isStoreDB">是否存入数据库datetime字段，date字段没事，取出不用判断</param>
        /// <returns>返回list集合</returns>
        public static List<T> TableToList<T>(DataTable dt, bool isStoreDB = true)
        {
            List<T> list = new List<T>();
            Type type = typeof(T);
            PropertyInfo[] pArray = type.GetProperties(); //集合属性数组
            foreach (DataRow row in dt.Rows)
            {
                T entity = Activator.CreateInstance<T>(); //新建对象实例 
                foreach (PropertyInfo p in pArray)
                {
                    if (!dt.Columns.Contains(p.Name) || row[p.Name] == null || row[p.Name] == DBNull.Value)
                    {
                        continue;  //DataTable列中不存在集合属性或者字段内容为空则，跳出循环，进行下个循环   
                    }
                    if (isStoreDB && p.PropertyType == typeof(DateTime) && Convert.ToDateTime(row[p.Name]) < Convert.ToDateTime("1753-01-01"))
                    {
                        continue;
                    }
                    try
                    {
                        var obj = Convert.ChangeType(row[p.Name], p.PropertyType);//类型强转，将table字段类型转为集合字段类型  
                        p.SetValue(entity, obj, null);
                    }
                    catch (Exception)
                    {
                        // throw;
                    }
                    
                }
                list.Add(entity);
            }
            return list;
        }

        /// <summary>
        /// List集合转DataTable
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="list">传入集合</param>
        /// <param name="isStoreDB">是否存入数据库DateTime字段，date时间范围没事，取出展示不用设置TRUE</param>
        /// <returns>返回datatable结果</returns>
        public static DataTable ListToTable<T>(List<T> list, bool isStoreDB = true)
        {
            Type tp = typeof(T);
            PropertyInfo[] proInfos = tp.GetProperties();
            DataTable dt = new DataTable();
            foreach (var item in proInfos)
            {
                dt.Columns.Add(item.Name, item.PropertyType); //添加列明及对应类型
            }
            foreach (var item in list)
            {
                DataRow dr = dt.NewRow();
                foreach (var proInfo in proInfos)
                {
                    object obj = proInfo.GetValue(item);
                    if (obj == null)
                    {
                        continue;
                    }
                    if (isStoreDB && proInfo.PropertyType == typeof(DateTime) && Convert.ToDateTime(obj) < Convert.ToDateTime("1753-01-01"))
                    {
                        continue;
                    }
                    dr[proInfo.Name] = obj;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// table指定行转对象
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="dt">传入的表格</param>
        /// <param name="rowindex">table行索引，默认为第一行</param>
        /// <returns>返回实体对象</returns>
        public static T TableToEntity<T>(DataTable dt, int rowindex = 0, bool isStoreDB = true)
        {
            Type type = typeof(T);
            T entity = Activator.CreateInstance<T>(); //创建对象实例
            if (dt == null)
            {
                return entity;
            }
            DataRow row = dt.Rows[rowindex]; //要查询的行索引
            PropertyInfo[] pArray = type.GetProperties();
            foreach (PropertyInfo p in pArray)
            {
                if (!dt.Columns.Contains(p.Name) || row[p.Name] == null || row[p.Name] == DBNull.Value)
                {
                    continue;
                }

                if (isStoreDB && p.PropertyType == typeof(DateTime) && Convert.ToDateTime(row[p.Name]) < Convert.ToDateTime("1753-01-02"))
                {
                    continue;
                }
                try
                {
                    var obj = Convert.ChangeType(row[p.Name], p.PropertyType);//类型强转，将table字段类型转为对象字段类型
                    p.SetValue(entity, obj, null);
                }
                catch (Exception)
                {
                    // throw;
                }
            }
            return entity;
        }
        #endregion

        #region 数据库模型
        public class NoteInfo
        {
            //PrimaryKey 主键
            //AutoIncrement自增长
            [PrimaryKey,AutoIncrement]
            public int id { get; set; }
            public string name { get; set; }
            public bool IsShow { get; set; }=true;
        }
        public class PicInfo
        {
            [PrimaryKey]
            public int id { get; set; }
            public string PicPath { get; set; }
            //ForeignKey外键 typeof(NoteInfo)来自NoteInfo表
            [ForeignKey(typeof(NoteInfo))]
            public int Noteid { get; set; }
        }
        public class TextInfo
        {
            [PrimaryKey]
            public int id { get; set; }
            public string TextDetil { get; set; }
            [ForeignKey(typeof(NoteInfo))]
            public int Noteid { get; set; }
        }
        #endregion
    }
}

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
using System.Collections;
using System.Security.Cryptography;

namespace Note
{
    public static class SqliteHelper
    {

        static string StrConn = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Note.db3");
        public static void CreateDb()
        {
            if (File.Exists(StrConn))
            {
                return;
            }
            else
            {
                var conn = new SQLiteConnection(StrConn, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true);
                conn.CreateTables<NoteInfo, PicInfo, TextInfo>(CreateFlags.AllImplicit);
                conn.Execute("PRAGMA foreign_keys = ON;");
            }

        }
        public static void DeleteDb()
        {
            if (File.Exists(StrConn))
                File.Delete(StrConn);
        }

        #region EQ
        public static (DataTable, string) ExecuteQuery(this string StrSql, DBTable dBTable)
        {
            var dt = new DataTable();
            var ErrMsg = "";
            try
            {
                var conn = new SQLiteConnection(StrConn);
                if (dBTable == DBTable.NoteInfo)
                {
                    var info = conn.Query<NoteInfo>(StrSql);
                    dt = ListToTable(info);
                }
                if (dBTable == DBTable.TextInfo)
                {
                    var info = conn.Query<TextInfo>(StrSql);
                    dt = ListToTable(info);
                }
                if (dBTable == DBTable.PicInfo)
                {
                    var info = conn.Query<PicInfo>(StrSql);
                    dt = ListToTable(info);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                dt = new DataTable();
            }
            return (dt, ErrMsg);
        }

        /// <summary>
        /// 取结果的第一行
        /// </summary>
        /// <param name="StrSql"></param>
        /// <param name="dBTable"></param>
        /// <returns></returns>
        public static (DataRow, string) ExecuteQueryRow(this string StrSql, DBTable dBTable)
        {
            var dt = new DataTable();
            var ErrMsg = "";
            try
            {
                var conn = new SQLiteConnection(StrConn);
                if (dBTable == DBTable.NoteInfo)
                {
                    var info = conn.Query<NoteInfo>(StrSql);
                    dt = ListToTable(info);
                }
                if (dBTable == DBTable.TextInfo)
                {
                    var info = conn.Query<TextInfo>(StrSql);
                    dt = ListToTable(info);
                }
                if (dBTable == DBTable.PicInfo)
                {
                    var info = conn.Query<PicInfo>(StrSql);
                    dt = ListToTable(info);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                dt = new DataTable();
            }
            return (dt.Rows[0], ErrMsg);
        }

        /// <summary>
        /// 获取结果第一行的id
        /// </summary>
        /// <param name="StrSql"></param>
        /// <param name="dBTable"></param>
        /// <returns></returns>
        public static (int, string) ExecuteQueryGetRowID(this string StrSql, DBTable dBTable)
        {
            var dt = new DataTable();
            var ErrMsg = "";
            try
            {
                var conn = new SQLiteConnection(StrConn);
                if (dBTable == DBTable.NoteInfo)
                {
                    var info = conn.Query<NoteInfo>(StrSql);
                    dt = ListToTable(info);
                }
                if (dBTable == DBTable.TextInfo)
                {
                    var info = conn.Query<TextInfo>(StrSql);
                    dt = ListToTable(info);
                }
                if (dBTable == DBTable.PicInfo)
                {
                    var info = conn.Query<PicInfo>(StrSql);
                    dt = ListToTable(info);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                dt = new DataTable();
            }
            if (dt != null && dt.Rows.Count > 0)
            {
                return (Convert.ToInt32(dt.Rows[0]["id"]), ErrMsg);

            }
            return (0, ErrMsg);
        }

        /// <summary>
        /// 对比是否修改标题
        /// </summary>
        /// <param name="StrSql"></param>
        /// <param name="dBTable"></param>
        /// <returns></returns>
        public static (bool, string, string) ExecuteQueryEqualsTitle(string Eqstr, int id)
        {
            var conn = new SQLiteConnection(StrConn);
            var dt = new DataTable();
            var ErrMsg = "";
            bool Result = false;
            try
            {
                var info = conn.Query<NoteInfo>($"select name from NoteInfo where id ={id}");
                dt = ListToTable(info);
                if (dt.Rows[0]["name"].ToString().Equals(Eqstr))
                {
                    Result = false;
                    ErrMsg = "未做更改，无需修改";
                }
                else
                {
                    ExecuteNonQueryUp($"update NoteInfo set name={Eqstr} where id={id}");
                    Result = true;
                    ErrMsg = "修改成功";
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
            }
            return (Result, ErrMsg, Convert.ToString(dt.Rows[0]["name"]));
        }

        /// <summary>
        /// 对比是否修改描述文本
        /// </summary>
        /// <param name="StrSql"></param>
        /// <param name="dBTable"></param>
        /// <returns></returns>
        public static (bool, string) ExecuteQueryEqualsDetil(string Eqstr, int id)
        {
            var conn = new SQLiteConnection(StrConn);
            var dt = new DataTable();
            var ErrMsg = "";
            bool Result = false;
            try
            {
                var info = conn.Query<TextInfo>($"select TextDetil from TextInfo where Noteid ='{id}'");
                dt = ListToTable(info);
                if (dt.Rows[0]["TextDetil"].ToString().Equals(Eqstr))
                {
                    Result = true;
                    ErrMsg = "未做更改，无需修改";
                }
                else
                {
                   var Results = ExecuteNonQueryUp($"update TextInfo set TextDetil='{Eqstr}' where Noteid='{id}'");
                    if (Results.Item1 == "修改成功")
                    {
                        return (true, Results.Item2);
                    }
                    else
                    {
                        return (false, Results.Item2);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
            }
            return (Result, ErrMsg);
        }

        /// <summary>
        /// 对比是否修改图片路径
        /// </summary>
        /// <param name="StrSql"></param>
        /// <param name="dBTable"></param>
        /// <returns></returns>
        public static (bool, string) ExecuteQueryEqualsPicList(ArrayList EqList, int id)
        {
            var conn = new SQLiteConnection(StrConn);
            var dt = new DataTable();
            var ErrMsg = "";
            bool Result = false;
            if (EqList == null || EqList.Count<=0)
            {
                return (false, "");
            }
            try
            {
                var info = conn.Query<PicInfo>($"select id,PicPath from PicInfo where Noteid ='{id}'");
                dt = ListToTable(info);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (string itempath in EqList)
                    {
                        bool isinsert = false;
                        foreach (DataRow dbpath in dt.Rows)
                        {
                            if (itempath.Equals(dbpath["PicPath"]))
                            {
                                isinsert = false;
                                dt.Rows.Remove(dbpath);
                                break;
                            }
                            isinsert = true;
                        }
                        if (isinsert)
                        {
                            ExecuteNonQuery(new PicInfo() { PicPath = itempath, Noteid = id });
                            Result = true;
                        }
                    }
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow item in dt.Rows)
                        {
                            ExecuteNonQueryDel($"delete from PicInfo where id = '{item["id"]}'");
                        }
                        Result = true;
                    }
                }

            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
            }
            return (Result, ErrMsg);
        }

        /// <summary>
        /// 取结果的行数
        /// </summary>
        /// <param name="StrSql"></param>
        /// <param name="dBTable"></param>
        /// <returns></returns>
        public static (int, string) ExecuteQueryRowCount(this string StrSql, DBTable dBTable)
        {
            var dt = new DataTable();
            var ErrMsg = "";
            try
            {
                var conn = new SQLiteConnection(StrConn);
                if (dBTable == DBTable.NoteInfo)
                {
                    var info = conn.Query<NoteInfo>(StrSql);
                    dt = ListToTable(info);
                }
                if (dBTable == DBTable.TextInfo)
                {
                    var info = conn.Query<TextInfo>(StrSql);
                    dt = ListToTable(info);
                }
                if (dBTable == DBTable.PicInfo)
                {
                    var info = conn.Query<PicInfo>(StrSql);
                    dt = ListToTable(info);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                dt = new DataTable();
            }
            return (dt.Rows.Count, ErrMsg);
        }

        /// <summary>
        /// 取结果的行数
        /// </summary>
        /// <param name="StrSql"></param>
        /// <param name="dBTable"></param>
        /// <returns></returns>
        public static (bool, string) IsExist(string StrField, object Value, DBTable dBTable)
        {
            var dt = new DataTable();
            var ErrMsg = "";
            bool isExist = false;
            try
            {
                var conn = new SQLiteConnection(StrConn);
                if (dBTable == DBTable.NoteInfo)
                {
                    var info = conn.Query<NoteInfo>($"select count() from NoteInfo where {StrField} = {Value}");
                    dt = ListToTable(info);
                }
                if (dBTable == DBTable.TextInfo)
                {
                    var info = conn.Query<TextInfo>($"select count() from TextInfo where {StrField} = {Value}");
                    dt = ListToTable(info);
                }
                if (dBTable == DBTable.PicInfo)
                {
                    var info = conn.Query<PicInfo>($"select count() from PicInfo where {StrField} = {Value}");
                    dt = ListToTable(info);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                dt = new DataTable();
            }
            if (dt != null && dt.Rows.Count > 0)
            {
                isExist = true;
            }

            return (isExist, ErrMsg);
        }
        #endregion

        #region ENQ
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="StrSql"></param>
        /// <returns></returns>
        public static (string, string) ExecuteNonQueryUp(this string StrSql)
        {
            var ErrMsg = "";
            var IsSuccess = "";
            try
            {
                var conn = new SQLiteConnection(StrConn);
                var Result = conn.Execute(StrSql);

                IsSuccess = Result > 0 ? "修改成功" : "修改失败";
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                IsSuccess = "修改失败";
            }
            return (IsSuccess, ErrMsg);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public static (string, string) ExecuteNonQueryDel(this string StrSql)
        {
            var ErrMsg = "";
            var IsSuccess = "";
            try
            {
                var conn = new SQLiteConnection(StrConn);
                var Result = conn.Execute(StrSql);
                IsSuccess = Result > 0 ? "删除成功" : "删除失败";
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                IsSuccess = "删除失败";
            }
            return (IsSuccess, ErrMsg);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>
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
                IsSuccess = "插入失败";
            }
            return (IsSuccess, ErrMsg);
        }
        #endregion

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
            [PrimaryKey, AutoIncrement]
            public int id { get; set; }
            public string name { get; set; }
            public bool IsShow { get; set; } = true;
        }
        public class PicInfo
        {
            [PrimaryKey, AutoIncrement]
            public int id { get; set; }
            public string PicPath { get; set; }
            //ForeignKey外键 typeof(NoteInfo)来自NoteInfo表
            [ForeignKey(typeof(NoteInfo))]
            public int Noteid { get; set; }
        }
        public class TextInfo
        {
            [PrimaryKey, AutoIncrement]
            public int id { get; set; }
            public string TextDetil { get; set; }
            [ForeignKey(typeof(NoteInfo))]
            public int Noteid { get; set; }
        }
        #endregion

        #region 枚举数据库类型
        public enum DBTable
        {
            NoteInfo, PicInfo, TextInfo
        }
        #endregion

    }
}

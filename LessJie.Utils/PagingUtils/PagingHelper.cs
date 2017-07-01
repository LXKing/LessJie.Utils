using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessJie.PagingUtils
{
    /// <summary>
    /// 分页工具类
    /// </summary>
    public class PagingHelper
    {
        /// <summary>
        /// 分页工具
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="DataSource"></param>
        /// <param name="CurrentPageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<T> ListPager<T>(List<T> DataSource, int pageIndex, int PageSize, ref int count)
        {
            //count = 0;
            //if (DataSource == null || DataSource.Count == 0)
            //    return DataSource;
            //count = DataSource.Count;
            //int startIndex = (CurrentPageIndex - 1) * PageSize;
            //if (startIndex + PageSize > DataSource.Count)
            //{
            //    PageSize = DataSource.Count - startIndex;
            //}
            //var size = PageSize;
            ////return DataSource.GetRange(startIndex, PageSize);
            //return DataSource.GetRange(startIndex, 1);
            count = 0;
            if (DataSource == null || DataSource.Count == 0)
                return DataSource;
            count = DataSource.Count;
            if (count < PageSize) { PageSize = count; }
            if (pageIndex < 1) { pageIndex = 1; }
            int startIndex = (pageIndex - 1) * PageSize;
            return DataSource.GetRange(startIndex, PageSize);
        }
    }

    public class PaginationInfo<T>
    {
        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageSize { get; private set; }
        /// <summary>
        /// 实际每页记录数，最后一页记录数可能少于每页记录数
        /// </summary>
        public int ActualPageSize { get; private set; }
        /// <summary>
        /// 页索引数
        /// </summary>
        public int PageIndex { get; private set; }
        /// <summary>
        /// 当页起始记录数，为sql server分页sql语句准备的数据
        /// </summary>
        public int PageStartCount { get; private set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalRecordCount { get; private set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPageCount { get; private set; }
        /// <summary>
        /// SQL server下，分页第一次查询的数量
        /// </summary>
        public long TopCount { get; private set; }
        /// <summary>
        /// 数据队列
        /// </summary>
        public List<T> PageContent { get; set; }
        /// <summary>
        /// 初始化分页
        /// </summary>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="pageIndex">当前页索引数</param>
        /// <param name="totalRecordCount">总记录数</param>
        protected PaginationInfo(int pageSize, int pageIndex, int totalRecordCount)
        {
            this.PageSize = pageSize;
            this.TotalRecordCount = totalRecordCount;
            this.PageIndex = pageIndex;
            PageContent = new List<T>();
            Compute();
        }
        /// <summary>
        /// 用于快速的生成一个分页数据
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalRecordCount"></param>
        /// <returns></returns>
        public static PaginationInfo<T> CreatePageData(int pageSize, int pageIndex, int totalRecordCount)
        {
            return new PaginationInfo<T>(pageSize, pageIndex, totalRecordCount);
        }
        /// <summary>
        /// 计算分页数据
        /// </summary>
        protected virtual void Compute()
        {
            #region -- 计算分页数据 --
            PageSize = PageSize < 1 ? 20 : PageSize;//默认20
            int lastPage = Convert.ToInt32(TotalRecordCount % PageSize);//计算最后一页记录数
            TotalPageCount = Convert.ToInt32(TotalRecordCount / PageSize) + (lastPage > 0 ? 1 : 0);//计算总页数
            PageIndex = PageIndex > TotalPageCount ? TotalPageCount : PageIndex;//检查当前页数大
            PageIndex = PageIndex < 1 ? 1 : PageIndex;//检查当前页小
            TopCount = PageIndex * PageSize;//sqlite中用的 top 多少记录数，比sql server少pagesize个
            ActualPageSize = (PageIndex == TotalPageCount && lastPage != 0) ? lastPage : PageSize;//判断是否最后一页，并指定页记录数
            PageStartCount = (PageIndex - 1) * PageSize;//sql server用的
            #endregion -- 计算分页完成 --
        }
    }


    public class PagedList<T> : List<T>
    {
        /// <summary>
        /// 分页编号
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 元素总共数目
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 页数
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="list">链表</param>
        /// <param name="intPageIndex">编号</param>
        /// <param name="intPageSize">大小</param>
        public PagedList(List<T> list, int intPageIndex, int intPageSize, out int total)
        {
            PageIndex = intPageIndex;
            PageSize = intPageSize;

            GetPagedList(list, out total);
        }

        /// <summary>
        /// 转为为分页
        /// </summary>
        /// <param name="list">链表</param>
        private void GetPagedList(List<T> list, out int total)
        {
            //int intStart = (PageIndex - 1) * PageSize;
            //if (intStart < 1) {
            //    intStart = 1;
            //}
            //for (int i = intStart; i < intStart + PageSize && i <= list.Count; i++)
            //{
            //    this.Add(list[i - 1]);
            //}

            //total = list.Count;
            //PageCount = total / PageSize;
            int intStart = (PageIndex - 1) * PageSize;
            for (int i = intStart; i < intStart + PageSize && i < list.Count; i++)
            {
                this.Add(list[i]);
            }

            total = list.Count;
            PageCount = total / PageSize + 1;
        }
    }

    public static class PagedListExpansion
    {
        public static PagedList<T> ToPagedList<T>(this List<T> list, int intPageIndex, int intPageSize, out int total)
        {
            return new PagedList<T>(list, intPageIndex, intPageSize, out total);
        }
    }


    //public class test<T> : BaseDataOperation<T> :class, new()
    //{
    //    //public string StandardizeSearch<T>(T model, BaseBZH baseBZH, User userInfo, int page, int rows)
    //    //{
    //    //    try
    //    //    {
    //    //        StandardizeDAL dal = new StandardizeDAL();
    //    //        int total = 0;
    //    //        var standardizeList = dal.GetListBy(s => s.Subject_ID == "1001001" && s.Cid == userInfo.OrganizationID, s => s.RelationID);
    //    //        var baseBZHResult = THelper.GetProperties(baseBZH);
    //    //        var modelResult = THelper.GetProperties(model, baseBZHResult);
    //    //        int i = 0;
    //    //        int j = 0;
    //    //        int modelLength = modelResult.Count;
    //    //        int standardizeListLength = standardizeList.Count();
    //    //        Dictionary<string, object> d = new Dictionary<string, object>();
    //    //        List<T> modelList = new List<T>();
    //    //        foreach (var item in standardizeList)
    //    //        {
    //    //            i++;
    //    //            j++;
    //    //            if (i <= modelLength)
    //    //            {
    //    //                d.Add(item.KeyName, item.ValueContent);
    //    //            }
    //    //            else
    //    //            {
    //    //                Dictionary<string, object> db = new Dictionary<string, object>();
    //    //                db = THelper.GetProperties(item);
    //    //                T t = new T();
    //    //                t = THelper.SetProperties(t, db);
    //    //                t = THelper.SetProperties(t, d);
    //    //                modelList.Add(t);
    //    //                d.Clear();
    //    //                d.Add(item.KeyName, item.ValueContent);
    //    //                i = 0;
    //    //            }
    //    //            if (j == standardizeListLength)
    //    //            {
    //    //                Dictionary<string, object> db = new Dictionary<string, object>();
    //    //                db = THelper.GetProperties(item);
    //    //                T t = new T();
    //    //                t = THelper.SetProperties(t, db);
    //    //                t = THelper.SetProperties(t, d);
    //    //                modelList.Add(t);
    //    //                d.Clear();
    //    //                i = 0;
    //    //            }
    //    //        }
    //    //        //var List = PagingUtil.ListPager(modelList, page, rows, ref total);
    //    //        PagedList<T> List = modelList.ToPagedList<T>(page, rows, out total);
    //    //        dynamic dataGrid = new EasyDataGrid
    //    //        {
    //    //            footer = null,
    //    //            rows = List,
    //    //            total = total
    //    //        };
    //    //        return JsonUtil.ObjToJson(dataGrid);
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        return ex.Message;
    //    //    }
    //    //}
    //}
}

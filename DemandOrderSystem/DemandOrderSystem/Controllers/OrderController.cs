﻿///需求單表Controller
///表一 TableOne
///表二 TableTwo 以此類推
///表三 TableThree 
///表四 TableFour
///表五 TableFive
///表六 TableSix
///表七 TableSeven
///表八 TableEight
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemandOrderSystem.Library;
using DemandOrderSystem.Models.ViewModel;
using PagedList;
using System.Net;

namespace DemandOrderSystem.Controllers
{
    public class OrderController : Controller
    {
        OrderLibrary dbLibrary = new OrderLibrary();


        /// <summary>
        /// 需求單受理狀態表
        /// </summary>
        /// <returns></returns>
        public ActionResult TableOne()
        {
            TableOneViewModel orders = new TableOneViewModel();

            orders.Orders2 = dbLibrary.getOrderDatas();

            orders.SearchDate = DateTime.Now.AddYears(-1);

            //預設為資訊中心
            orders.selectedDept = 0;

            return View(orders);
        }

        [HttpPost]
        public ActionResult TableOne(TableOneViewModel viewModel)
        {
            viewModel.Orders2 = dbLibrary.GetDataByDept(viewModel.selectedDept, viewModel.SearchDate);

            return View(viewModel);
        }

        /// <summary>
        /// 回傳需求單總表
        /// </summary>
        /// <returns></returns>
        public ActionResult TableAll(String applicant_department_list, String information_management_list, DateTime? evaluate_recived_date_start, DateTime? evaluate_recived_date_end, DateTime? closed_date_start, DateTime? closed_date_end, int page = 1)
        {
            int currentPage = page < 1 ? 1 : page;
            var tableAllViewModel = dbLibrary.GetTableAllViewModel(applicant_department_list,information_management_list,evaluate_recived_date_start,evaluate_recived_date_end,closed_date_start,closed_date_end,currentPage);
            return View(tableAllViewModel);
        }

        /// <summary>
        /// 回傳需求單總表Excel
        /// </summary>
        /// <returns></returns>
        public ActionResult TableAllExcelDownload()
        {
            return this.File(dbLibrary.GetTableAllExcelData().ToArray(), "application/vnd.ms-excel", "需求單總表.xlsx");
        }

        /// <summary>
        /// 已交付UAT但尚未結案之明細表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="orderState">需求單狀態</param>
        /// <param name="acceptionTestStartDate_0">驗收開始日_範圍起</param>
        /// <param name="acceptionTestStartDate_1">驗收開始日_範圍結</param>
        /// <param name="applyDept">申請人部室</param>
        /// <param name="orderID">需求單號</param>
        /// <param name="orderby">排序(未實作)</param>
        /// <returns></returns>
        //[HttpGet]
        //public ActionResult TableThree(int? page, string orderState, string acceptionTestStartDate_0, string acceptionTestStartDate_1, string applyDept, string orderID, string orderby)
        //{
        //    orderState = "驗收";

        //    List<TableThreeViewModel> _Table = new List<TableThreeViewModel>();
        //    List<String> item = new List<String>();

        //    if (string.IsNullOrWhiteSpace(acceptionTestStartDate_1))
        //    {
        //        acceptionTestStartDate_1 = DateTime.Now.Date.ToString();
        //    }

        //    if (!string.IsNullOrEmpty(orderID))
        //    {
        //        _Table = dbLibrary.GetTableThreeViewModel(null, null, null, null).Where(o => o.OrderID.Contains(orderID)).ToList();
        //        TempData["TempDataTest"] = _Table.Count.ToString();
        //        ViewBag.applyDept = item.Select(x => new SelectListItem() { Text = x.ToString(), Value = x.ToString() });
        //        return View(_Table.ToPagedList(page ?? 1, 20));
        //    }

        //    _Table = dbLibrary.GetTableThreeViewModel(orderState, acceptionTestStartDate_0, acceptionTestStartDate_1, "");
        //    TempData["TempDataTest"] = _Table.Count.ToString();

        //    var item2 = _Table.GroupBy(x => x.ApplyDept).Distinct().ToList();
        //    ViewBag.applyDept = item2.Select(x => new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });

        //    if (!string.IsNullOrWhiteSpace(applyDept))
        //    {
        //        _Table = _Table.Where(o => o.ApplyDept == applyDept).ToList();
        //        TempData["TempDataTest"] = _Table.Count.ToString();
        //    }

        //    return View(_Table.ToPagedList(page ?? 1, 20));
        //}


        [HttpGet]
        public ActionResult TableThree(int? page, string orderState = null, string acceptionTestStartDate_0 = null, string acceptionTestStartDate_1 = null, string applyDept = null, string orderID = null)
        {
            orderState = "驗收";

            List<TableThreeViewModel> _Table = new List<TableThreeViewModel>();

            _Table = dbLibrary.GetTableThreeViewModel(orderState, acceptionTestStartDate_0, acceptionTestStartDate_1, applyDept, orderID);
            var item = _Table.GroupBy(x => x.ApplyDept).Distinct().ToList();
            ViewBag.applyDept = item.Select(x => new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });
            TempData["TempDataTest"] = _Table.Count.ToString();

            return View(_Table.ToPagedList(page ?? 1, 20));
        }



        /// <summary>
        /// 回傳需求單分類統計表
        /// </summary>
        /// <returns></returns>
        public ActionResult TableFour(TableFourViewModel model)
        {
            TableFourViewModel classificationViewModel = new TableFourViewModel();
            if (model.startTime == new DateTime())
            {
                classificationViewModel.countByMonth = new List<CountByMonth>();
                return View(classificationViewModel);
            }
            else
            {
                classificationViewModel = dbLibrary.GetTableFourViewModel(model.startTime, model.endTime);
                classificationViewModel.startTime = model.startTime;
                classificationViewModel.endTime = model.endTime;
                return View(classificationViewModel);
            }
            
        }

        /// <summary>
        /// 回傳需求單未結案且未回覆預估完成日件數統計及明細表
        /// </summary>
        /// <param name="condition">需求單狀態選擇</param>
        /// <returns></returns>
        public ActionResult TableFive(String condition)
        {
            TableFiveViewModel result = dbLibrary.GetTableFiveViewModel(condition);


            return View(result);
        }

        /// <summary>
        /// 各業務線別逾預估完成日未結案件數統計及明細表
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public ActionResult TableSix(string date = "2017-11-12")
        {
            var view = dbLibrary.GetTableSixViewModel(Convert.ToDateTime(date));

            return View(view);
        }

        /// <summary>
        /// 撤件明細
        /// </summary>
        /// <param name="page"></param>
        /// <param name="caseCloseDate">結案日</param>
        /// <param name="dischargeDate">撤件日期</param>
        /// <param name="applyDept">申請部室</param>
        /// <param name="orderID">需求單號</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult TableSeven(int? page, string caseCloseDate, string dischargeDate, string applyDept, string orderID)
        {
            var pageSize = 20;

            List<TableSevenViewModel> _Table = new List<TableSevenViewModel>();
            List<string> item = new List<string>();

            if (!String.IsNullOrEmpty(orderID))
            {
                _Table = dbLibrary.GetTableSevenViewModel("","","").Where(o => o.OrderID.Contains(orderID)).ToList();
                TempData["TempDataTest"] = _Table.Count.ToString();
                ViewBag.applyDept = item.Select(x => new SelectListItem() { Text = x.ToString(), Value = x.ToString() });
                return View(_Table.ToPagedList(page ?? 1, pageSize));
            }

            _Table = dbLibrary.GetTableSevenViewModel(caseCloseDate, dischargeDate, "");
            TempData["TempDataTest"] = _Table.Count.ToString();

            var item2 = _Table.GroupBy(x => x.ApplyDept).Distinct().ToList();
            ViewBag.applyDept = item2.Select(x => new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });

            if (!string.IsNullOrWhiteSpace(applyDept))
            {
                _Table = _Table.Where(o => o.ApplyDept == applyDept).ToList();
                TempData["TempDataTest"] = _Table.Count.ToString();
            }

            return View(_Table.ToPagedList(page ?? 1, pageSize));
        }


        /// <summary>
        /// 已完成驗收但尚未結案
        /// </summary>
        /// <param name="page"></param>
        /// <param name="orderState">需求單狀態</param>
        /// <param name="acceptionTestFinishDate_0">驗收結束日_範圍起</param>
        /// <param name="acceptionTestFinishDate_1">驗收結束日_範圍結</param>
        /// <param name="maintainITDept">維護資訊室</param>
        /// <param name="orderby">排序(沒實裝上)</param>
        /// <param name="orderID">需求單號</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult TableEight(int? page, string orderState, string acceptionTestFinishDate_0, string acceptionTestFinishDate_1, string maintainITDept, string orderby, string orderID)
        {
            List<TableEightViewModel> _Table = new List<TableEightViewModel>();
            List<String> item = new List<String>();

            if (string.IsNullOrWhiteSpace(acceptionTestFinishDate_1))
            {
                acceptionTestFinishDate_1 = DateTime.Now.Date.ToString();
            }

            if (!string.IsNullOrEmpty(orderID))
            {
                _Table = dbLibrary.GetTableEightViewModel("","","","").Where(o => o.OrderID.Contains(orderID)).ToList();
                TempData["TempDataTest"] = _Table.Count.ToString();
                ViewBag.maintainITDept = item.Select(x => new SelectListItem() { Text = x.ToString(), Value = x.ToString() });
                return View(_Table.ToPagedList(page ?? 1, 20));
            }

            _Table = dbLibrary.GetTableEightViewModel(orderState, acceptionTestFinishDate_0, acceptionTestFinishDate_1, "");
            TempData["TempDataTest"] = _Table.Count.ToString();


            var item2 = _Table.Where(x => x.MaintainITDept != null).GroupBy(x => x.MaintainITDept).Distinct().ToList();
            ViewBag.maintainITDept = item2.Select(x => new SelectListItem() { Text = x.Key.ToString(), Value = x.Key.ToString() });

            switch (orderby)
            {
                case "Information_Room":
                    return View(_Table.OrderBy(o => o.MaintainITDept).ToPagedList(page ?? 1, 20));
                case "OrderID":
                    return View(_Table.OrderBy(o => o.OrderID).ToPagedList(page ?? 1, 20));
                case "Date":
                    return View(_Table.OrderBy(o => o.AcceptionTestFinishDate).ToPagedList(page ?? 1, 20));
            }

            if (!string.IsNullOrWhiteSpace(maintainITDept))
            {
                _Table = _Table.Where(o => o.MaintainITDept == maintainITDept).ToList();
                TempData["TempDataTest"] = _Table.Count.ToString();
            }

            return View(_Table.ToPagedList(page ?? 1, 20));
        }

        public ActionResult TableTwo(int page = 1)
        {
            TableTwoViewModel viewModel = new TableTwoViewModel();

            viewModel.Orders = dbLibrary.getOrderDatas();

            viewModel.SearchDate = DateTime.Now.AddYears(-1);

            var currentPage = page < 1 ? 1 : page;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult TableTwo(TableTwoViewModel viewModel)
        {
            viewModel.Orders = dbLibrary.GetDataSearchDate(viewModel.SearchDate);

            TableTwoViewModel newViewModel = new TableTwoViewModel();

            newViewModel.SearchDate = viewModel.SearchDate;

            newViewModel.SelectedDept = viewModel.SelectedDept;

            newViewModel.Orders = dbLibrary.ReturnDataByApplydept(viewModel.Orders, viewModel._applyDeptName[viewModel.SelectedDept]);

            return View(newViewModel);


        }


        public ActionResult MonthDatas(MonthDataViewModel viewModel)
        {
            if (viewModel.DataMonth == null || viewModel.State == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (viewModel.ApplyDeptName == null)
            {
                //從TableOne連結過來
                viewModel.Orders = dbLibrary.GetDatasByMonthAndStateAndDept(viewModel.DataMonth, viewModel.State, viewModel.DeptCode);

                viewModel.DeptName = dbLibrary.deptName[viewModel.DeptCode];
            }
            else
            {
                //從TableTwo連結過來
                viewModel.Orders = dbLibrary.GetDatasByMonthAndStateAndApplydept(viewModel.DataMonth, viewModel.State, viewModel.ApplyDeptName);
                //viewModel.DeptName = viewModel.ApplyDeptName;

            }
            //viewModel.DeptName = orderLib.deptName[viewModel.DeptCode];

            ViewData["state"] = viewModel.State;

            ViewData["dataDate"] = viewModel.DataMonth;

            ViewBag.date = viewModel.DataMonth;

            return View(viewModel);
        }

        [HttpPost]
        [ActionName("MonthDatas")]
        public ActionResult MonthDatasPost(MonthDataViewModel viewModel)
        {
            if (viewModel.DataMonth == null || viewModel.State == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (viewModel.StrSearch != "" && viewModel.StrSearch != null)
            {
                if (viewModel.ApplyDeptName == null)
                {
                    //從TableOne連結過來
                    viewModel.Orders = dbLibrary.GetDatasByMonthAndStateAndDept(viewModel.DataMonth, viewModel.State, viewModel.DeptCode)
                                                .Where(o => o.OrderName.Contains(viewModel.StrSearch))
                                                .ToList();

                    viewModel.DeptName = dbLibrary.deptName[viewModel.DeptCode];
                }
                else
                {
                    //從TableTwo連結過來
                    viewModel.Orders = dbLibrary.GetDatasByMonthAndStateAndApplydept(viewModel.DataMonth, viewModel.State, viewModel.ApplyDeptName)
                                                .Where(o => o.OrderName.Contains(viewModel.StrSearch))
                                                .ToList();

                }
            }
            else
            {
                if (viewModel.ApplyDeptName == null)
                {
                    //從TableOne連結過來
                    viewModel.Orders = dbLibrary.GetDatasByMonthAndStateAndDept(viewModel.DataMonth, viewModel.State, viewModel.DeptCode);

                    viewModel.DeptName = dbLibrary.deptName[viewModel.DeptCode];
                }
                else
                {
                    //從TableTwo連結過來
                    viewModel.Orders = dbLibrary.GetDatasByMonthAndStateAndApplydept(viewModel.DataMonth, viewModel.State, viewModel.ApplyDeptName);

                }
            }

            return View(viewModel);
        }

        public ActionResult Detail(string orderId)
        {
            if (orderId == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = dbLibrary.getOrderById(orderId);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }
    }
}
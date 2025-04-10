using System;
using System.Data;
using System.Text.Json;
using System.Data.Entity;
using System.Linq;
using System.Net;
using NEXT_BMS.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using NEXT_BMS;
using Insya.Localization;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Spreadsheet;
using NEXT_BMS.ViewModels;

namespace NEXT_BMS.Utilities;

public class DataLevelAuthorization
{
    private NEXT_BMSContext _context = new NEXT_BMSContext();
    private ISession _session;
    public DataLevelAuthorization(NEXT_BMSContext context, ISession session)
    {
        _context = context;
        _session = session;
    }
    
    int ActionBy { set; get; }
    List<Priviledges> menus { set; get; }
    List<string> actions = new List<string>();
    public DataLevelAuthorization()
    {
        if (_session.GetInt32("UserId") != null)
        {
            ActionBy = (int)_session.GetInt32("ActionBy");
            menus = GetPriviledges();
            foreach (var menu in menus)
            {
                actions.Add(menu.action);
            }
        }
    }

    private List<Priviledges> GetPriviledges()
    {
        var priviledgesJson = _session.GetString("Previleges");
        if (string.IsNullOrEmpty(priviledgesJson))
        {
            return new List<Priviledges>();
        }
        var priviledges = System.Text.Json.JsonSerializer.Deserialize<List<Priviledges>>(priviledgesJson);
        return priviledges.ToList();
    }


    private bool CanGetAllStores()
    {
        return actions.Contains("CanGetAllStores".ToUpper());
    }

    private bool CanGetAllBranchStores()
    {
        return actions.Contains("CanGetAllBranchStores".ToUpper());
    }

    private bool CanGetAllBranches()
    {
        return actions.Contains("CanGetAllBranches".ToUpper());
    }

    private bool CanGetAllDepartments()
    {
        return actions.Contains("CanGetAllDepartments".ToUpper());
    }

    private bool CanGetAllBranchDepartments()
    {
        return actions.Contains("CanGetAllBranchDepartments".ToUpper());
    }

    //public List<int> GetUserBranchs()
    //{
    //    if (CanGetAllBranches())
    //    {
    //        return _context.Branches.Where(x => x.IsActive == true && x.IsDeleted == false).Select(x => x.Id).ToList();
    //    }
    //    return _context.UserBranches.Where(x => x.UserId == ActionBy && x.IsActive == true && x.IsDeleted == false).Select(x => x.BranchId).ToList();
    //}

    //public List<int> GetUserStores()
    //{
    //    if (CanGetAllStores())
    //    {
    //        return _context.Stores.Where(x => x.IsActive && x.IsDeleted == false).Select(s => s.Id).ToList();
    //    }

    //    else if (CanGetAllBranchStores())
    //    {
    //        List<int> userbranches = GetUserBranchs();
    //        return _context.Stores.Where(x => userbranches.Contains(x.BranchId) && x.IsActive && x.IsDeleted == false).Select(s => s.Id).ToList();
    //    }

    //    return _context.StoreAllocations.Where(x => x.EmployeeId == ActionBy && x.IsActive == true && x.IsDeleted == false).Select(s => s.StokeId).ToList();
    //}

    //public List<int> GetUserBranchs()
    //{
    //    if (CanGetAllBranches())
    //    {
    //        return _context.Branches.Where(x => x.IsActive == true && x.IsDeleted == false).Select(x => x.Id).ToList();
    //    }
    //    return _context.UserBranches.Where(x => x.UserId == ActionBy && x.IsActive == true && x.IsDeleted == false).Select(x => x.BranchId).ToList();
    //}

    //public List<int> GetUserDepartments()
    //{
    //    if (CanGetAllDepartments())
    //    {
    //        return _context.BranchDepartments.Where(x => x.IsActive && x.IsDeleted == false).Select(x => x.Id).ToList();
    //    }
    //    else if (CanGetAllBranchDepartments())
    //    {
    //        List<int> userbranches = GetUserBranchs();
    //        return _context.BranchDepartments.Where(x => userbranches.Contains(x.BranchId) && x.IsActive && x.IsDeleted == false).Select(s => s.Id).ToList();
    //    }

    //    return _context.UserDepartments.Where(x => x.UserId == ActionBy && x.IsActive && x.IsDeleted == false).Select(x => x.BranchDepartmentId).ToList();
    //}

    //public List<int> GetUserDepartments()
    //{
    //    if (CanGetAllDepartments())
    //    {
    //        return _context.Departments.Where(x => x.IsActive && x.IsDeleted == false).Select(x => x.Id).ToList();
    //    }
    //    else if (CanGetAllBranchDepartments())
    //    {
    //        List<int> userbranches = GetUserBranchs();
    //        return _context.BranchDepartments.Where(x => userbranches.Contains(x.BranchId) && x.IsActive && x.IsDeleted == false).Select(s => s.DepartmentId).ToList();
    //    }

    //    return _context.UserDepartments.Where(x => x.UserId == ActionBy && x.IsActive && x.IsDeleted == false).Select(x => x.BranchDepartment.DepartmentId).ToList();
    //}

    //List<int> employeeBranchDepartments = new List<int>();
    //public List<int> GetUserBranchDepartments()
    //{
    //    if (CanGetAllBranchDepartments())
    //    {
    //        List<int> userbranches = GetUserBranchs();
    //        return _context.BranchDepartments.Where(x => userbranches.Contains(x.BranchId) && x.IsActive && x.IsDeleted == false).Select(s => s.Id).ToList();
    //    }
    //    else
    //    {
    //        var employeeJobPositionSalaries = _context.EmployeeJobPositionSalaries.Where(x => x.EmployeeId == ActionBy && x.IsActive);

    //        if (employeeJobPositionSalaries.Any())
    //        {
    //            var employeeJobPositionSalary = employeeJobPositionSalaries.FirstOrDefault();
    //            int BranchDepartmentId = employeeJobPositionSalary.JobPositionSalary.JobPosition.DepartmentBranchId;
    //            GetChildBranchDepartmentId(BranchDepartmentId);
    //        }
    //        return employeeBranchDepartments;
    //    }
    //}

    //private void GetChildBranchDepartmentId(int ParentId)
    //{
    //    employeeBranchDepartments.Add(ParentId);
    //    var branchDepartments = _context.BranchDepartments.Where(x => x.IsDeleted == false && x.ParentId == ParentId).ToList();

    //    foreach (var branchDepartment in branchDepartments)
    //    {
    //        GetChildBranchDepartmentId(branchDepartment.Id);
    //    }
    //}


}

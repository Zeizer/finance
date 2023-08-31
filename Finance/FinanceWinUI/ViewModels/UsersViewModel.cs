using CommunityToolkit.Mvvm.ComponentModel;
using Finance.Account.Data;
using Finance.Account.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceWinUI.ViewModels
{
    public partial class UsersViewModel:ObservableObject
    {
        [ObservableProperty]
        public List<User> users = new List<User>();

        public UsersViewModel()
        {
            DataFactory.Instance.GetCacheHashtable().Remove(CacheHashkey.UserList);
            Users = DataFactory.Instance.GetUserExecuter().List();
        }
    }
}

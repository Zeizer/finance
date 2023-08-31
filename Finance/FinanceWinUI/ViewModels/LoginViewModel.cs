using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Finance.Account.Controls.Commons;
using Finance.Account.Data;
using Finance.Account.SDK;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceWinUI.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        Dictionary<long, string> tidSource = new Dictionary<long, string>();

        [ObservableProperty]
        List<SampleItem> _sampleItems = new List<SampleItem>();

        [ObservableProperty]
        string password = null;
        [ObservableProperty]
        string userName = null;
        [ObservableProperty]
        SampleItem selectedItem = null;
        [ObservableProperty]
        long tid = 0;
        public LoginViewModel()
        {
            SampleItems = DataFactory.Instance.GetAccountCtlExecuter().GetAccountList();
        }

        [RelayCommand]
        public void Login()
        {
            Verification();
        }


        #region 工作方法
        /// <summary>
        /// 用于验证登陆信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns>
        /// 返回值代表 
        /// -5：用户名或密码不正确
        /// -4：密码含有特殊字符 -3：密码不能为空
        /// -2：用户名不能为空
        /// -1：数据库未连接 0：登陆失败
        ///  1：登陆成功  2：
        /// 
        /// </returns>
        public int Verification()
        {
            int flag = 1;
            try
            {
                if (string.IsNullOrEmpty(this.UserName))
                {
                    flag = -2;
                    return flag;
                }
                if (string.IsNullOrEmpty(this.Password))
                {
                    flag = -3;
                    return flag;
                }

                DataFactory.Instance.GetUserExecuter().Login(Tid, UserName, Password);

                FinanceControlEventsManager.Instance.GetAccountSubjectListEvent += () => {
                    List<AccountSubject> auxiliaries = DataFactory.Instance.GetAccountSubjectExecuter().List();
                    var json = JsonConvert.SerializeObject(auxiliaries);
                    return JsonConvert.DeserializeObject<List<AccountSubjectObj>>(json);

                };
                FinanceControlEventsManager.Instance.GetAuxiliaryObjListEvent += (type) => {
                    List<Auxiliary> auxiliaries = DataFactory.Instance.GetAuxiliaryExecuter().List()
                                                  .FindAll(aux => aux.type == (int)type)
                                                  .ToList();
                    var json = JsonConvert.SerializeObject(auxiliaries);
                    return JsonConvert.DeserializeObject<List<AuxiliaryObj>>(json);
                };

                FinanceControlEventsManager.Instance.MessageEventHandlerEvent += (level, msg) =>
                {
                    switch (level)
                    {
                        case MessageLevel.ERR:
                            //logger.Error(msg);
                            break;
                        case MessageLevel.INFO:
                            //logger.Debug(msg);
                            break;
                        case MessageLevel.WARN:
                            //logger.Warn(msg);
                            break;
                    }

                };



            }
            catch (Exception ex)
            {
                flag = -5;
                //出现错误无视
                Console.WriteLine(ex.Message);
            }
            return flag;
        }

        #endregion
    }
}

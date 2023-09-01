using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Finance.Account.Controls.Commons;
using Finance.Account.Data;
using Finance.Account.SDK;
using Microsoft.Windows.AppNotifications.Builder;
using Newtonsoft.Json;
using NuGet.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        bool autoLogin = false;
        [ObservableProperty]
        string password = null;
        [ObservableProperty]
        string userName = null;
        [ObservableProperty]
        SampleItem selectedItem = new SampleItem();
        
        public LoginViewModel()
        {
            SampleItems = DataFactory.Instance.GetAccountCtlExecuter().GetAccountList();
            if (ConfigurationManager.AppSettings["AutoLogin"] == "true")
            {
                UserName = ConfigurationManager.AppSettings["UserName"];
                Password = ConfigurationManager.AppSettings["Password"];
                SelectedItem = SampleItems.Where(item => item.id == long.Parse(ConfigurationManager.AppSettings["AccountID"])).FirstOrDefault();
                AutoLogin = true;
            }
        }

        [RelayCommand]
        public void Login()
        {
            if (AutoLogin)
            {
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                AddUpdateAppSettings("AutoLogin", "true");
                AddUpdateAppSettings("UserName", UserName);
                AddUpdateAppSettings("Password", Password);
                AddUpdateAppSettings("AccountID", SelectedItem.id.ToString());
                configuration.Save(ConfigurationSaveMode.Modified);
            }
            Verification();
        }

        private void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
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

                DataFactory.Instance.GetUserExecuter().Login(0, UserName, Password);

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

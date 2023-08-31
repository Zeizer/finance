using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Finance.Account.Controls.Commons;
using Finance.Account.Data;
using Finance.Account.SDK;
using FinanceWinUI.Pages;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace FinanceWinUI.ViewModels
{
    
    public partial class MainViewModel: ObservableObject
    {


        [ObservableProperty]
        public NavigationViewItem selectedNavigationViewItem = new NavigationViewItem();
        [ObservableProperty]
        public List<NavigationViewItem> navigationViewItems = new List<NavigationViewItem>();
        [ObservableProperty]
        public Frame selectedFrame = new Frame();


        public MainViewModel()
        {
            navigationViewItems.Add(new NavigationViewItem { Content = "首页", Icon = new SymbolIcon(Symbol.Home) });
            navigationViewItems.Add(new NavigationViewItem { Content = "账套", Icon = new SymbolIcon(Symbol.Library) });
            navigationViewItems.Add(new NavigationViewItem { Content = "凭证", Icon = new SymbolIcon(Symbol.Document) });
            navigationViewItems.Add(new NavigationViewItem { Content = "报表", Icon = new SymbolIcon(Symbol.ReportHacked) });
            navigationViewItems.Add(new NavigationViewItem { Content = "设置", Icon = new SymbolIcon(Symbol.Setting) });
            NavigationViewItems.Add(new NavigationViewItem { Content = "帮助", Icon = new SymbolIcon(Symbol.Help) });
            NavigationViewItems.Add(new NavigationViewItem { Content = "关于", Icon = new SymbolIcon(Symbol.ContactInfo) });
            NavigationViewItems.Add(new NavigationViewItem { Content = "用户", Icon = new SymbolIcon(Symbol.Contact) });
            SelectedFrame.SourcePageType = typeof(HomePage);
        }

        partial void OnSelectedNavigationViewItemChanged(NavigationViewItem value)
        {
            if (value != null)
            {
                SelectedFrame.SourcePageType = value.Content.Equals("用户") ? typeof(UsersPage) : typeof(HomePage);
            }
        }

        
    }
}

﻿using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using iNKORE.UI.WPF.Modern.Controls;
using Newtonsoft.Json;
using StarLight_Core.Utilities;
using MessageBox = iNKORE.UI.WPF.Modern.Controls.MessageBox;
using Page = System.Windows.Controls.Page;

namespace AuroraStarLauncher.Pages.FrpPages
{
    /// <summary>
    /// PN_FrpPage.xaml 的交互逻辑
    /// </summary>
    public partial class PN_FrpPage : Page
    {
        public PN_FrpPage()
        {
            InitializeComponent();

            try
            {
                string applicationDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string FolderName = "ASL";
                string FolderPath = Path.Combine(applicationDirectory, FolderName);
                string FileName = "config.json";
                string FilePath = Path.Combine(FolderPath, FileName);
                string jsonContent = File.ReadAllText(FilePath);
                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(jsonContent);

                PN_User_Name.Text = loginResponse.user;
                PN_User_ID.Text = "用户ID：" + loginResponse.pn;
                PN_Logintime.Text = "登录时间：" + loginResponse.loginTime;
            }
            catch
            {
                MessageBox.Show("请重新登录！", "提示");
            }
        }

        private async void Pn_Login_ALL_Click(object sender, RoutedEventArgs e)
        {
            string pnLoginUser = pn_Login_User_Name.Text;
            string pnloginPassword = pn_Login_User_Password.Password;
            var pnLoginData = new
            {
                user = pnLoginUser,
                password = pnloginPassword,
            };
            var json = JsonConvert.SerializeObject(pnLoginData);
            try
            {
                var loginData =
                    await HttpUtil.SendHttpPostRequest("https://PolarisNetwork.cloud:5555/api/v1/Auth/login",
                        JsonConvert.SerializeObject(pnLoginData), "application/json");
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(loginData);

                string jsonString = JsonConvert.SerializeObject(loginResponse, Formatting.Indented);
                string applicationDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string FolderName = "ASL";
                string FolderPath = Path.Combine(applicationDirectory, FolderName);
                string FileName = "config.json";
                string FilePath = Path.Combine(FolderPath, FileName);
                File.WriteAllText(FilePath, jsonString);

                //MessageBox.Show("欢迎回来," + loginResponse.user + "!", "登录成功！");
                Footer_Tip.Severity = InfoBarSeverity.Success;
                Footer_Tip.Title = "登录成功！";
                Footer_Tip.Message = "欢迎回来," + loginResponse.user + "!";
                Footer_Tip.IsOpen = true;

                PN_User_Name.Text = loginResponse.user;
                PN_User_ID.Text = "用户ID：" + loginResponse.pn;
                PN_Logintime.Text = "登录时间：" + loginResponse.loginTime;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "登录失败");
            }
        }
    }

    public class LoginResponse
    {
        public string user { get; set; }
        public DateTime loginTime { get; set; }
        public string token { get; set; }
        public int pn { get; set; }
        public bool status { get; set; }
        public string msg { get; set; }
    }
}

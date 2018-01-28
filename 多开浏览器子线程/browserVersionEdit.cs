using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 多开浏览器子线程
{
    class BrowserVersionEdit
    {
        public static void   SetIEcomp(WebBrowser WebBrowser,string version="9")
        {
            string appname = Process.GetCurrentProcess().ProcessName + ".exe";
            RegistryKey RK8 = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION");
   
            int value9 = 9999;
            int value8 = 8888;
            Version ver = WebBrowser.Version;
            int value = value9;
            try
            {
                string[] parts = ver.ToString().Split('.');
				int.TryParse(parts[0], out int vn);
				if (vn != 0)
                {
                    if (vn == 9)
                        value = value9;
                    else
                        value = value8;
                }
            }
            catch
            {
                value = value9;
            }
            //Setting the key in LocalMachine  
            if (RK8 != null)
            {
                try
                {
                    //MessageBox.Show(RK8.GetValue (appname ).ToString ());
                    RK8.SetValue(appname, value, RegistryValueKind.DWord);
                    RK8.Close();
                    MessageBox.Show("已设置版本到IE"+version);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("设置失败，请检查:"+ex.Message);  
                }
            }
        }

    }
}

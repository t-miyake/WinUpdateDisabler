using Microsoft.Win32;

namespace WinUpdateDisabler
{
    public sealed class Model
    {
        // Singleton instance.
        internal static Model Instance { get; } = new Model();
        private Model()
        {

        }

        internal bool CurrentState { get; set; } = true;

        internal void GetCurrentStatus()
        {
            switch (ReadReg(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "WUServer"))
            {
                case null:
                    CurrentState = true;
                    break;
                case "http://localhost/":
                    CurrentState = false;
                    break;
                default:
                    CurrentState = true;
                    break;
            }
        }

        internal void DoDisable()
        {
            WriteReg(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "NoAutoRebootWithLoggedOnUsers", "1", RegistryValueKind.DWord);
            WriteReg(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "NoAutoUpdate", "1", RegistryValueKind.DWord);
            WriteReg(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "UseWUServer", "1", RegistryValueKind.DWord);
            WriteReg(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "DoNotConnectToWindowsUpdateInternetLocations", "1", RegistryValueKind.DWord);
            WriteReg(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "DisableDualScan", "1", RegistryValueKind.DWord);
            WriteReg(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "BranchReadinessLevel", "20", RegistryValueKind.DWord);
            WriteReg(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "DeferFeatureUpdates", "0", RegistryValueKind.DWord);
            WriteReg(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "DeferFeatureUpdatesPeriodInDays", "0", RegistryValueKind.DWord);
            WriteReg(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "PauseFeatureUpdatesStartTime", "",RegistryValueKind.String);
            WriteReg(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "UpdateServiceUrlAlternate", @"http://localhost/", RegistryValueKind.String);
            WriteReg(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "WUServer", @"http://localhost/", RegistryValueKind.String);
            WriteReg(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "WUStatusServer", @"http://localhost/", RegistryValueKind.String);
        }

        internal void DoEnable()
        {
            DeleteReg(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate");
        }

        private void WriteReg(string subKey, string keyName, string value, RegistryValueKind kind)
        {
            var key = Registry.LocalMachine.CreateSubKey(subKey);
            key?.SetValue(keyName, value, kind);
            key?.Close();
        }

        private void DeleteReg(string subKey)
        {
            try
            {
                Registry.LocalMachine.DeleteSubKeyTree(subKey);
            }
            catch
            {
                // Do Noting.
            }
        }

        private string ReadReg(string subKey, string keyName)
        {
            try
            {
                var key = Registry.LocalMachine.OpenSubKey(subKey);
                return key?.GetValue(keyName).ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}
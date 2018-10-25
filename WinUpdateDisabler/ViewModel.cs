using System;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Input;

namespace WinUpdateDisabler
{
    public class ViewModel : ViewModelBase
    {
        private readonly Model _model = Model.Instance;

        public ICommand Enable { get; }
        public ICommand Disable { get; }

        public ViewModel()
        {
            Enable = new RelayCommand(() =>
            {
                _model.DoEnable();
                CurrentState = true;
                UpdateButton();

                if (MessageBox.Show("Enabled" + Environment.NewLine + "Please reboot.", "WinUpdateDisabler", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    Reboot();
            });

            Disable = new RelayCommand(() =>
            {
                _model.DoDisable();
                CurrentState = false;
                UpdateButton();

                if (MessageBox.Show("Disabled" + Environment.NewLine + "Please sign out.", "WinUpdateDisabler", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    SignOut();
            });

            _model.GetCurrentStatus();
            UpdateButton();
        }

        public bool CurrentState
        {
            get => _model.CurrentState;
            set
            {
                _model.CurrentState = value;
                OnPropertyChanged("CurrentStateText");
            }
        }

        public string CurrentStateText => _model.CurrentState ? "Current state：Enabled" : "Current state：Disabled";

        private bool _enableButton;
        public bool EnableButton
        {
            get => _enableButton;
            set
            {
                _enableButton = value;
                OnPropertyChanged("EnableButton");
            }
        }

        private bool _disableButton;
        public bool DisableButton
        {
            get => _disableButton;
            set
            {
                _disableButton = value;
                OnPropertyChanged("DisableButton");
            }
        }

        private void UpdateButton()
        {
            if (CurrentState)
            {
                EnableButton = false;
                DisableButton = true;
            }
            else
            {
                EnableButton = true;
                DisableButton = false;
            }
        }

        private static void SignOut()
        {
            var signOut = new ProcessStartInfo
            {
                FileName = "shutdown.exe",
                Arguments = "/l",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process.Start(signOut);
        }

        private static void Reboot()
        {
            var signOut = new ProcessStartInfo
            {
                FileName = "shutdown.exe",
                Arguments = "/r /t 0",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process.Start(signOut);
        }
    }
}
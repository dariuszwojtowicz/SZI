// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="">
//   
// </copyright>
// <summary>
//   Klasa bazowa dla wszystkich view modeli
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kelner.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Klasa bazowa dla wszystkich view modeli
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        bool? _CloseWindowFlag;
        public bool? CloseWindowFlag
        {
            get { return this._CloseWindowFlag; }
            set
            {
                this._CloseWindowFlag = value;
                this.RaisePropertyChanged("CloseWindowFlag");
            }
        }

        public virtual void CloseWindow(bool? result = true)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                this.CloseWindowFlag = this.CloseWindowFlag == null
                    ? true
                    : !this.CloseWindowFlag;
            }));
        }
    }
}

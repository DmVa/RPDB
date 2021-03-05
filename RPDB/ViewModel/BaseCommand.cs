﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RPDB.ViewModel
{
    public class BaseCommand : ICommand
    {

        private readonly Action _execute;

        private readonly Func<bool> _canExecute;

        public BaseCommand(Action execute)
            : this(execute, null)
        {
        }

        public BaseCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            _execute = execute;
            _canExecute = canExecute;
        }

        #region "ICommand"

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged(object parameter)
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        [DebuggerStepThrough()]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        #endregion
    }
}

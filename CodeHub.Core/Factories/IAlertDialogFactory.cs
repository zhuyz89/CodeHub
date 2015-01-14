﻿using System.Threading.Tasks;

namespace CodeHub.Core.Factories
{
    public interface IAlertDialogFactory
    {
        Task<bool> PromptYesNo(string title, string message);

        Task Alert(string title, string message);

        Task<string> PromptTextBox(string title, string message, string defaultValue, string okTitle);
    }
}


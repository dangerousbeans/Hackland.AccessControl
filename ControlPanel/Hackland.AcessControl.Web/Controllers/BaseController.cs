using Hackland.AccessControl.Data;
using Hackland.AccessControl.Web.Extensions;
using Hackland.AccessControl.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Controllers
{
    public class ControllerBase : Controller
    {

        public T BindMetadataFields<T>(T item, CreateUpdateModeEnum mode) where T : IMetadataFields
        {
            var userId = Guid.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (mode == CreateUpdateModeEnum.Create)
            {
                item.CreatedTimestamp = DateTime.Now;
                item.CreatedByUserId = userId;
            }
            else
            {
                item.UpdatedTimestamp = DateTime.Now;
                item.UpdatedByUserId = userId;
            }
            return item;
        }

        public void AddMessage(MessageModel message)
        {
            var controller = this;
            
            var list = controller.TempData.Get<List<MessageModel>>(MessageModel.TempDataKey) ?? new List<MessageModel>();
            list.Add(message);
            controller.TempData.Put<List<MessageModel>>(MessageModel.TempDataKey, list);
        }

        public void AddSuccess(string title, string text) => this.AddMessage(MessageModel.CreateSuccessMessage(title, text));
        public void AddSuccess(string title, string format, params object[] arguments) => this.AddMessage(MessageModel.CreateSuccessMessage(title, format, arguments));
        public void AddError(string title, string text, bool clear = true) => this.AddMessage(MessageModel.CreateErrorMessage(title, text));
        public void AddError(string title, string format, params object[] arguments) => this.AddMessage(MessageModel.CreateErrorMessage(title, format, arguments));
        public void AddInfo(string title, string text, bool clear = true) => this.AddMessage(MessageModel.CreateInfoMessage(title, text));
        public void AddInfo(string title, string format, params object[] arguments) => this.AddMessage(MessageModel.CreateInfoMessage(title, format, arguments));
        public void AddWarning(string title, string format, params object[] arguments) => this.AddMessage(MessageModel.CreateWarningMessage(title, format, arguments));
        public void AddWarning(string title, string text, bool clear = true) => this.AddMessage(MessageModel.CreateWarningMessage(title, text));
        public void AddError(string title, ModelStateDictionary state)
        {
            var controller = this;
            foreach (var error in state.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
            {
                controller.AddError(title, error);
            }
        }
    }
}

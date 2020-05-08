namespace Sitecore.ContentFactory.SmartTools.Commands
{
    using Sitecore;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Jobs;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Text;
    using Sitecore.Web.UI.Sheer;
    using Sitecore.Workflows;
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;

    [Serializable]
    public class CreateContentFactory : Command
    {
        public override void Execute(CommandContext context)
        {
            try
            {
                Error.AssertObject(context, "context");
                if (context.Items.Length == 1)
                {
                    //TODO: Remove the commented unused code.
                    //Item item = Context.ContentDatabase.GetItem(context.Items[0].ID, context.Items[0].Language, context.Items[0].Version);
                    //CultureInfo info = new CultureInfo(StringUtil.GetString(Sitecore.Context.ClientPage.ServerProperties["TranslatingLanguage"]));
                    Item item = context.Items[0];
                    NameValueCollection parameters = new NameValueCollection();
                    parameters["id"] = item.ID.ToString();
                    parameters["language"] = item.Language.ToString();
                    parameters["version"] = item.Version.ToString();

                    //IWorkflow workflow = item.Database.WorkflowProvider.GetWorkflow(item);
                    Context.ClientPage.Start(this, "Run", parameters);
                }

            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error(ex.Message, this);
                //TODO : Rethrow the exception so that end user will know about error.
            }
        }

        public override CommandState QueryState(CommandContext commandContext)
        {
            Error.AssertObject(commandContext, "context");
            if (commandContext.Items.Length != 1 || commandContext.Items[0].Appearance.ReadOnly || !commandContext.Items[0].Access.CanRead())
            {
                return CommandState.Disabled;
            }

            return base.QueryState(commandContext);
        }

        protected void Run(ClientPipelineArgs args)
        {
            try
            {
                string id = args.Parameters["id"];
                string language = args.Parameters["language"];
                string version = args.Parameters["version"];

                Item item = Context.ContentDatabase.Items[id, Language.Parse(language), Sitecore.Data.Version.Parse(version)];
                Error.AssertItemFound(item);

                if (SheerResponse.CheckModified())
                {
                    if (args.IsPostBack)
                    {
                        if (args.Result.ToLower() == "yes")
                        {
                            Context.ClientPage.SendMessage(this, "item:load(id=" + id + ",language=" + language + ",version=" + version + ")");
                            var content = new UrlString(UIUtil.GetUri("control:CreateContent"));
                            content.Add("id", item.ID.ToString());
                            content.Add("la", item.Language.ToString());
                            content.Add("vs", item.Version.ToString());
                            content.Add("ci", item.Language.ToString());
                            SheerResponse.ShowModalDialog(content.ToString(), "this is for testing"); // TODO :  Give proper meaningful message
                        }
                        // TODO : If REsult is Not "yes" then what to do?
                    }
                    else
                    {
                        var content = new UrlString(UIUtil.GetUri("control:CreateContent"));
                        content.Add("id", item.ID.ToString());
                        content.Add("la", item.Language.ToString());
                        content.Add("vs", item.Version.ToString());
                        content.Add("ci", item.Language.ToString());
                        SheerResponse.ShowModalDialog(content.ToString(), true);

                        args.WaitForPostBack();
                    }

                    var isJobDone = JobManager.GetJobs().FirstOrDefault(j => j.Name.Equals("createContent") && j.Status.State == JobState.Running);
                    if (isJobDone != null && !isJobDone.IsDone)
                    {
                        SheerResponse.Timer("PublishApi:checkStatus", 100);
                    }
                    else
                    {
                        //TODO: Can we make this item path as configurable. So that this can be used for other applciation also.
                        var finalReport = Sitecore.Context.ContentDatabase.GetItem("/sitecore/content/MNY/Global Configuration/Content Factory Last Report/Report");
                        string messageReport = finalReport["Description"];

                        Context.ClientPage.ClientResponse.Alert(messageReport);
                        finalReport.Editing.BeginEdit();
                        finalReport["Description"] = string.Empty;

                        finalReport.Editing.EndEdit();
                    }
                }
            }
            catch (Exception exception)
            {
                Sitecore.Diagnostics.Log.Error(exception.Message, this);
                //TODO : Rethrow the exception so that end user will know about error.
            }
        }
    }
}


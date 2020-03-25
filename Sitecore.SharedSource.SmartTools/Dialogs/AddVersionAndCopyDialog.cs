namespace Sitecore.SharedSource.SmartTools.Dialogs
{
    using Microsoft.ApplicationBlocks.Data;
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Data.Managers;
    using Sitecore.Diagnostics;
    using Sitecore.Shell.Applications.ContentEditor;
    using Sitecore.Web.UI.HtmlControls;
    using Sitecore.Web.UI.Pages;
    using Sitecore.Web.UI.Sheer;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Xml;
    using Sitecore.Collections;
    using Sitecore.Globalization;
    using Sitecore.SecurityModel;
    using Sitecore.Shell.Applications.Dialogs.ProgressBoxes;
    using Sitecore.Sites;
    using static Sitecore.Configuration.Settings;
    using static Sitecore.Configuration.State;
    using System.Linq;

    public class AddVersionAndCopyDialog : DialogForm
    {
        protected Language sourceLanguage;
        protected bool CopySubItems;
        protected List<string> Site;
        protected Dictionary<string, string> langNames;
        protected Combobox Source;
        protected Literal TargetLanguages;
        protected Literal Options;
        protected Literal TargetSites;
        protected Literal TargetPath;
        protected Literal TargetCatalogue;

        protected TreeList TreeListOfItems;
        protected List<Language> targetLanguagesList;
        protected List<string> targetSiteList;
        protected List<string> ProductPathList;
        protected List<string> CataloguePathList;
        protected global::System.Web.UI.WebControls.TextBox Product1;


        private void fillLanguageDictionary()
        {
            this.langNames = new Dictionary<string, string>();

            LanguageCollection languages;
            Database database = Context.ContentDatabase;
            languages = LanguageManager.GetLanguages(database);
           

            foreach (Language language in languages)
            {
                this.langNames.Add(language.CultureInfo.EnglishName, language.Name);//Danish - Key, da - Value

            }
        }

        private void fillSiteDictionary()
        {
            this.Site = new List<string>();
            Database database = Context.ContentDatabase;
            var sites = database.GetItem("/sitecore/content");
            var templateid = "{17AE6319-D1D0-44F3-B9E3-BC7EA9925F3B}";
            Sitecore.Data.Items.Item[] allItems = database.SelectItems("fast:/sitecore/content//*[@@templateid='{E1070C26-B6EB-489B-BB5B-B29AE1F97BDE}']");

            var item = sites.GetChildren().Where(w => w.TemplateID.ToString() == templateid);
                foreach(var i in allItems)
            {
                this.Site.Add(i.Name);
            }




        }
    protected override void OnLoad(EventArgs e)
        {
            try
            {
                Assert.ArgumentNotNull(e, "e");
                base.OnLoad(e);
                if (!Context.ClientPage.IsEvent)
                {
                    Item itemFromQueryString = UIUtil.GetItemFromQueryString(Context.ContentDatabase);
                    ListItem child = new ListItem();
                    this.Source.Controls.Add(child);
                    CultureInfo info = new CultureInfo(Context.Request.QueryString["ci"]);
                    child.Header = info.EnglishName;
                    child.Value = info.EnglishName;
                    child.ID = Control.GetUniqueID("I");
                   
                    if (itemFromQueryString == null)
                        throw new Exception();
                  
              
                    //var t=database.SelectItems("fast:/sitecore/content//*[@@ID="+r+"]");
                   // var siteItem = Sitecore.Context.Database.Items.GetItem(r);
              //      var sq = Sitecore.Context.Database.Items.GetItem(r1);


                    string str = "<script type='text/javascript'>function toggleChkBoxMethod2(formName){var form=$(formName);var i=form.getElements('checkbox'); i.each(function(item){item.checked = !item.checked;});$('togglechkbox').checked = !$('togglechkbox').checked;}</script>";
                    str = str + "<table><tr><td>Toggle All Checkboxes:</td><td><input type='checkbox' id='togglechkbox' onClick=toggleChkBoxMethod2(ctl15); /></td></tr><tr></tr><tr></tr>";
                    this.fillLanguageDictionary();
                    this.fillSiteDictionary();
                    foreach (KeyValuePair<string, string> pair in this.langNames)
                    {
                        //if (itemFromQueryString.Language.Name != pair.Value)
                        //{
                        //    string str2 = "chk_" + pair.Value;
                        //    string str4 = str;
                        //    str = str4 + "<tr><td>" + pair.Key + "</td><td>" + pair.Value + "</td><td><input class='reviewerCheckbox' type='checkbox' value='1' name='" + str2 + "'/></td></tr>";
                        //}
                    }
                    
                    str = str + "</table>";
                    ////this.TargetLanguages.Text = str;
                    if (itemFromQueryString != null)
                    {
                        foreach(var site in this.Site)
                        {
                            string str2 = "chk_" + site;
                            string str3 = site;
                            str =str +site+"<tr><td><input class='reviewerCheckbox' type='checkbox' value='1' name='"+str2+"'/></td></tr><br>";
                            str = str + "Product Path:"+"<input type='textbox' id='Product"+site+"'/><br>";
                             str=str + "Catalog Path:" + "<input type='textbox' id='Catalog"+site+"'/><br>";
                            
                        }
                        this.TargetLanguages.Text = str;
                    }
               

                         //Options
                         str = "";
                    str += "<table>";
                    str += "<tr><td>Include SubItems:</td><td><input class='optionsCheckbox' type='checkbox' value='1' name='chk_IncludeSubItems'/></td></tr>";
                    str += "</table>";
                    this.Options.Text = str;
                }
            }
            catch (Exception exception)
            {
              Sitecore.Diagnostics.Log.Error(exception.Message, this);
            }
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            Exception exception;            
            Item itemFromQueryString = UIUtil.GetItemFromQueryString(Context.ContentDatabase);
            this.fillLanguageDictionary();
            this.fillSiteDictionary();
            targetLanguagesList = new List<Language>();
            targetSiteList = new List<string>();
            ProductPathList = new List<string>();
            CataloguePathList = new List<string>();
            try
            {
                //if(itemFromQueryString.TemplateID.ToString()!="{ B76E9B72 - 04D9 - 44A5 - 8885 - 64D7022E1AC2}")
                //{
                //    Context.ClientPage.ClientResponse.Alert("This item is not elligible for this tool");
                //    Context.ClientPage.ClientResponse.CloseWindow();
                //}
                //Get the source language
                if (itemFromQueryString == null)
                    throw new Exception();
                sourceLanguage = itemFromQueryString.Language;
                Sitecore.Diagnostics.Log.Debug("Smart Tools: OnOK-sourceLanguage-" + sourceLanguage.Name, this);

                //Get the target languages
                foreach (KeyValuePair<string, string> pair in this.langNames)
                {
                    if (!string.IsNullOrEmpty(Context.ClientPage.Request.Params.Get("chk_" + pair.Value)))
                    {
                        targetLanguagesList.Add(Sitecore.Data.Managers.LanguageManager.GetLanguage(pair.Value));
                    }
                }
                //Get the target sites
                foreach(var site in this.Site)
                {
                    if (!string.IsNullOrEmpty(Context.ClientPage.Request.Params.Get("chk_" + site))){
                        targetSiteList.Add(site);
                        string s = Context.ClientPage.Request.Params.Get("Product"+site+"").Replace(",", "");
                        string s1 = Context.ClientPage.Request.Params.Get("Catalog"+site+"").Replace(",", "");
                        ProductPathList.Add(s);
                        CataloguePathList.Add(s1);


                    }
                }


                //Include SubItems?
                if (!string.IsNullOrEmpty(Context.ClientPage.Request.Params.Get("chk_IncludeSubItems")))
                {
                    CopySubItems = true;
                }
                Sitecore.Diagnostics.Log.Debug("Smart Tools: OnOK-CopySubItems-" + CopySubItems.ToString(), this);

                //Execute the process
                if (itemFromQueryString != null && targetSiteList.Count > 0 && sourceLanguage != null)
                {
                    //Execute the Job
                    Sitecore.Shell.Applications.Dialogs.ProgressBoxes.ProgressBox.Execute("Add Version and Copy", "Smart Tools", new ProgressBoxMethod(ExecuteOperation), itemFromQueryString);
                }
                else
                {
                    //Show the alert
                    Context.ClientPage.ClientResponse.Alert("Context Item and Target Languages are empty.");
                    Context.ClientPage.ClientResponse.CloseWindow();
                }

                Context.ClientPage.ClientResponse.Alert("Process has been completed.");
                Context.ClientPage.ClientResponse.CloseWindow();
            }
            catch (Exception exception8)
            {
                exception = exception8;
                Sitecore.Diagnostics.Log.Error(exception.Message, this);
                Context.ClientPage.ClientResponse.Alert("Exception Occured. Please check the logs.");
                Context.ClientPage.ClientResponse.CloseWindow();
            }
        }

        protected void ExecuteOperation(params object[] parameters)
        {
            Sitecore.Diagnostics.Log.Debug("Smart Tools: Job Executed.", this);

            if (parameters == null || parameters.Length == 0)
                return;

            Item item = (Item)parameters[0];
            //IterateItems(item, targetLanguagesList, sourceLanguage);
            IterateItems(item,targetSiteList,ProductPathList,CataloguePathList,sourceLanguage);
        }

        //private void IterateItems(Item item, List<Language> targetLanguages, Language sourceLang)
        private void IterateItems(Item item, List<string> targetSites,List<string> targetProduct,List<string> targetCatalogue,Language sourceLang)
        {
            //AddVersionAndCopyItems(item, targetLanguages, sourceLang);
            //AddVersionAndCopyItems(item, targetSites, sourceLang);
            AddVersionAndCopyItems(item,targetSites,targetProduct,targetCatalogue,sourceLang);
            if (CopySubItems && item.HasChildren)
            {
                foreach (Item childItem in item.Children)
                {
                    //IterateItems(childItem, targetLanguages, sourceLang);
                    IterateItems(childItem, targetSites,targetProduct,targetCatalogue,sourceLang);

                }
            }
        }

        //   private void AddVersionAndCopyItems(Item item, List<Language> targetLanguages, Language sourceLang)
        private void AddVersionAndCopyItems(Item item, List<string> targetSites, List<string> targetProduct,List<string> targetCatalogue,Language sourceLang)
        {
            //foreach (Language language in targetLanguages)
          foreach(var site in targetSites)
            {
                Item source = Context.ContentDatabase.GetItem(item.ID, sourceLang);
                Item target = Context.ContentDatabase.GetItem(item.ID);
                Database database = Context.ContentDatabase;
                string name = site.ToString().Remove(0,13);

                var catalogue = database.GetItem(source["Product"]);


                var id = source.Parent.TemplateID;
               Database masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
                //var item1 = masterDb.SelectItems("fast:/sitecore/content/"+site+"//*[@@templateid="+id+"]");
                //string path = source.Parent.Paths.Path.ToString().Replace("Example",site);
                //var r = source.Fields["Product"];
                //  if (path != null)
                // { 
                foreach (var product in targetProduct)
                {
                    foreach (var catalogue1 in targetCatalogue)
                    {
                        Sitecore.Data.Items.Item parent = masterDb.GetItem(product);
                        Sitecore.Data.Items.Item parent1 = masterDb.GetItem(catalogue1);



                        if (parent != null && parent1 != null)
                        {
                            using (new LanguageSwitcher(parent.Language))
                            {

                                  source.CopyTo(parent, source.Name);
                              catalogue.CopyTo(parent1, catalogue.Name);
                                //   ite.GetChildren();        


                                //Item parentItem = masterDb.Items[path];
                                //TemplateItem template = source.Template;
                                //parentItem.Add(source.Name, template);
                            }

                            //   }
                        }


                    }
                }

                if (source == null || target == null) return;

                Sitecore.Diagnostics.Log.Debug("Smart Tools: AddVersionAndCopyItems-SourcePath-" + source.Paths.Path, this);
                Sitecore.Diagnostics.Log.Debug("Smart Tools: AddVersionAndCopyItemsSourceLanguage-" + sourceLang.Name, this);
            //    Sitecore.Diagnostics.Log.Debug("Smart Tools: AddVersionAndCopyItems-TargetLanguage-" + language.Name, this);

                source = source.Versions.GetLatestVersion();
                target.Versions.AddVersion();
              
                

                target.Editing.BeginEdit();

                source.Fields.ReadAll();

                foreach (Field field in source.Fields)
                {
                    if (!field.Name.StartsWith("_")) //(!field.Shared)
                        target[field.Name] = source[field.Name];
                }
                target.Editing.EndEdit();

                Sitecore.Diagnostics.Log.Debug("Smart Tools: AddVersionAndCopyItems-Completed.", this);
            }
        }
    }
}


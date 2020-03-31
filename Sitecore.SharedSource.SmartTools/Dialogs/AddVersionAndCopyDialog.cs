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
    using System.Web.UI.WebControls;
    using Sitecore.Resources.Media;
    using Sitecore.Web;

    public class AddVersionAndCopyDialog : DialogForm
    {
        protected Language sourceLanguage;
        protected bool CopySubItems;
        protected List<string> Site;
        protected Dictionary<string, string> langNames;
        protected Combobox Source;
        protected Web.UI.HtmlControls.Literal TargetLanguages;
        protected Web.UI.HtmlControls.Literal Options;
        protected Web.UI.HtmlControls.Literal TargetSites;
        protected Web.UI.HtmlControls.Literal TargetPath;
        protected Web.UI.HtmlControls.Literal TargetCatalogue;

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
            foreach (var i in allItems)
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
                    Sitecore.Web.UI.HtmlControls.ListItem child = new Sitecore.Web.UI.HtmlControls.ListItem();
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
                        foreach (var site in this.Site)
                        {
                                
                                string str2 = "chk_" + site;
                            string str3 = site;
                            str = str + site + "<tr><td><input class='reviewerCheckbox' type='checkbox' value='1' name='" + str2 + "'/></td></tr><br>";
                            str = str + "Product Path:" + "<input type='textbox' id='Product" + site + "'/><br>";
                            str = str + "Catalog Path:" + "<input type='textbox' id='Catalog" + site + "'/><br>";
                          //  string str4 = "";
                          //foreach (var Lang in this.langNames)
                          // {
                          //  str4 ="chk_"+Lang+site;
                          // }
                          //  str = str + str4;
                          //  //////str = str + "Preferred Language" + "<select id='Language" + site + "'>" + "<option width='20px'>"


                            ////            + l + "</option>";






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
                foreach (var site in this.Site)
                {
                    if (!string.IsNullOrEmpty(Context.ClientPage.Request.Params.Get("chk_" + site)))
                    {
                        targetSiteList.Add(site);
                        string s = Context.ClientPage.Request.Params.Get("Product" + site + "").Replace(",", "");
                        string s1 = Context.ClientPage.Request.Params.Get("Catalog" + site + "").Replace(",", "");
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
                if (itemFromQueryString != null && targetSiteList.Count > 0 && sourceLanguage != null &&ProductPathList.Count>0 &&CataloguePathList.Count>0)
                {
                    //Execute the Job
                    Sitecore.Shell.Applications.Dialogs.ProgressBoxes.ProgressBox.Execute("Add Version and Copy", "Smart Tools", new ProgressBoxMethod(ExecuteOperation), itemFromQueryString);
                }
                else
                {
                    //Show the alert
                    Context.ClientPage.ClientResponse.Alert("Context Item or Target Paths are empty.");
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
            IterateItems(item, targetSiteList, ProductPathList, CataloguePathList, sourceLanguage);
        }

        //private void IterateItems(Item item, List<Language> targetLanguages, Language sourceLang)
        private void IterateItems(Item item, List<string> targetSites, List<string> targetProduct, List<string> targetCatalogue, Language sourceLang)
        {
            //AddVersionAndCopyItems(item, targetLanguages, sourceLang);
            //AddVersionAndCopyItems(item, targetSites, sourceLang);
            AddVersionAndCopyItems(item, targetSites, targetProduct, targetCatalogue, sourceLang);
            if (CopySubItems && item.HasChildren)
            {
                foreach (Item childItem in item.Children)
                {
                    //IterateItems(childItem, targetLanguages, sourceLang);
                    IterateItems(childItem, targetSites, targetProduct, targetCatalogue, sourceLang);

                }
            }
        }
       
        //   private void AddVersionAndCopyItems(Item item, List<Language> targetLanguages, Language sourceLang)
        private void AddVersionAndCopyItems(Item item, List<string> targetSites, List<string> targetProduct, List<string> targetCatalogue, Language sourceLang)
        {
            //foreach (Language language in targetLanguages)
            foreach (var site in targetSites)
            {
                Item source = Context.ContentDatabase.GetItem(item.ID, sourceLang);
                string path = source.Parent.Parent.Parent.Parent.Paths.FullPath.ToString().Remove(0, 36);
                string taggingpath = source.Parent.Parent.Parent.Parent.Paths.FullPath.ToString().Remove(0, 22);
                Item target = Context.ContentDatabase.GetItem(item.ID);
                Database database = Context.ContentDatabase;
               string name = site.ToString().Remove(0, 14).Replace("_","-");
       


                var catalogue = database.GetItem(source["Product"]);
                          var id = source.Parent.TemplateID;

                Database masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
                //var item1 = masterDb.SelectItems("fast:/sitecore/content/"+site+"//*[@@templateid="+id+"]");
                //string path = source.Parent.Paths.Path.ToString().Replace("Example",site);
                //var r = source.Fields["Product"];
                //  if (path != null)
                // { 
                var catalogue1 = targetCatalogue.SingleOrDefault(x => x.Contains(site));

                if (catalogue1 == null)
                    Context.ClientPage.ClientResponse.Alert("Required Path does not exist");

                Sitecore.Data.Items.Item parent1 = masterDb.GetItem(catalogue1);
               var Siteinfo= Sitecore.Configuration.Factory.GetSiteInfoList()
                   .FirstOrDefault(x => parent1.Paths.FullPath.StartsWith(x.RootPath));
                var siteLAnguage = Siteinfo.Properties["language"];

                Language lang=Sitecore.Globalization.Language.Parse(name);


                if (parent1 != null)
                    {
                    using (new LanguageSwitcher(lang))
                    {
                        foreach (Item ctname in parent1.Children)
                        {
                            if (ctname.Name == catalogue.Name)
                            {
                                

                            }
                        }
                        ////// try
                        ////// {
                        var ite = catalogue.CopyTo(parent1, catalogue.Name);
                        

                        Sitecore.Data.Fields.MultilistField multiselectField = catalogue.Fields["Brand"];

                        Sitecore.Data.Items.Item[] items = multiselectField.GetItems();

                        if (items != null)
                        {
                            string brand1 = "";
                            foreach (var it in items)
                            {
                                var brand = database.GetItem(it.Paths.FullPath.Replace(taggingpath, site));
                                if (brand != null)
                                {
                                    brand1 = brand1 + brand.ID.ToString() + "|";
                                }
                            }

                            if (brand1 != null)
                            {
                                ite.Editing.BeginEdit();
                                ite["Brand"] = brand1;
                                ite.Editing.EndEdit();
                                if (brand1 == "")
                                {

                                }

                            }


                        }
                        Sitecore.Data.Fields.MultilistField multiselectField1 = catalogue.Fields["Benefit"];

                        Sitecore.Data.Items.Item[] items1 = multiselectField1.GetItems();
                        if (items1 != null)
                        {
                            string brand1 = "";
                            foreach (var it in items1)
                            {
                                var brand = database.GetItem(it.Paths.FullPath.Replace(taggingpath, site));
                                if (brand != null)
                                {
                                    brand1 = brand1 + brand.ID.ToString() + "|";
                                }
                            }

                            if (brand1 != null)
                            {
                                ite.Editing.BeginEdit();
                                ite["Benefit"] = brand1;
                                ite.Editing.EndEdit();

                            }
                            if (brand1 == "")
                            {

                            }
                        }

                        Sitecore.Data.Fields.MultilistField multiselectField2 = catalogue.Fields["Coverage"];

                        Sitecore.Data.Items.Item[] items2 = multiselectField2.GetItems();
                        if (items2 != null)
                        {
                            string brand1 = "";
                            foreach (var it in items2)
                            {
                                var brand = database.GetItem(it.Paths.FullPath.Replace(taggingpath, site));
                                if (brand != null)
                                {
                                    brand1 = brand1 + brand.ID.ToString() + "|";
                                }
                                if (brand1 == "")
                                {

                                }
                            }
                            if (brand1 != null)
                            {
                                ite.Editing.BeginEdit();
                                ite["Coverage"] = brand1;
                                ite.Editing.EndEdit();

                                if (brand1 == "")
                                {

                                }
                            }




                        }

                        Sitecore.Data.Fields.MultilistField multiselectField3 = catalogue.Fields["Finish"];

                        Sitecore.Data.Items.Item[] items3 = multiselectField3.GetItems();
                        if (items3 != null)
                        {
                            string brand1 = "";
                            foreach (var it in items3)
                            {
                                var brand = database.GetItem(it.Paths.FullPath.Replace(taggingpath, site));
                                if (brand != null)
                                {
                                    brand1 = brand1 + brand.ID.ToString() + "|";
                                }
                                if (brand1 == "")
                                {

                                }
                            }
                            if (brand1 != null)
                            {
                                ite.Editing.BeginEdit();
                                ite["Finish"] = brand1;
                                ite.Editing.EndEdit();
                                if (brand1 == "")
                                {

                                }
                            }

                        }

                        Sitecore.Data.Fields.MultilistField multiselectField4 = catalogue.Fields["Form"];

                        Sitecore.Data.Items.Item[] items4 = multiselectField4.GetItems();
                        if (items4 != null)
                        {
                            string brand1 = "";
                            foreach (var it in items4)
                            {
                                var brand = database.GetItem(it.Paths.FullPath.Replace(taggingpath, site));
                                if (brand != null)
                                {
                                    brand1 = brand1 + brand.ID.ToString() + "|";
                                }
                            }
                            if (brand1 != null)
                            {
                                ite.Editing.BeginEdit();
                                ite["Form"] = brand1;
                                ite.Editing.EndEdit();
                                if (brand1 == "")
                                {

                                }
                            }

                        }

                        //string y = "fast:#" + catalogue.Paths.FullPath.Replace(taggingpath,site) + "#//*[@@TemplateID='{9D2C8EF7-602E-48C2-98E5-727E62C4464E}']";
                        //var childcatalogue = database.SelectItems(y);





                        var product = targetProduct.SingleOrDefault(x => x.Contains(site));

                        if (product == null)
                            Context.ClientPage.ClientResponse.Alert("Required Item Path does not exist");
                        Sitecore.Data.Items.Item parent = masterDb.GetItem(product);
                        if (parent != null)
                        {




                            var ite1 = source.CopyTo(parent, source.Name);
                            Sitecore.Data.Fields.MultilistField multiselectField5 = ite1.Fields["Product"];

                            Sitecore.Data.Items.Item[] items5 = multiselectField5.GetItems();
                            if (items5 != null)
                            {
                                foreach (var it in items5)
                                {

                                    ite1.Editing.BeginEdit();
                                    string p = catalogue.Paths.FullPath.Replace(taggingpath, site);
                                    //     var pro = database.GetItem(p);
                                    ite1["Product"] = ite.ID.ToString();
                                    ite1.Editing.EndEdit();

                                }

                            }





                        }
                        foreach (Item child in ite.Children)
                        {
                            Sitecore.Data.Fields.MultilistField shades = child.Fields["ShadeFamily"];
                            Sitecore.Data.Items.Item[] items5 = shades.GetItems();

                            if (items5 != null)
                            {
                                string shade = "";

                                foreach (var i in items5)
                                {
                                    var shadesite = database.GetItem(i.Paths.FullPath.Replace(taggingpath, site));
                                    if (shadesite != null)
                                    {
                                        shade = shade + shadesite.ID.ToString() + "|";
                                    }
                                }
                                if (shade != null)
                                {
                                    child.Editing.BeginEdit();
                                    child["ShadeFamily"] = shade;
                                    child.Editing.EndEdit();
                                    if (shade == "")
                                    {

                                    }
                                }







                            }




                            //Item parentItem = masterDb.Items[path];
                            //TemplateItem template = source.Template;
                            //parentItem.Add(source.Name, template);






                        }
                        //}

                        /// catch (AggregateException ex)
                        /// {
                        ////   ex.Data.ToString();
                        //// }
                    }
                        //   }








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



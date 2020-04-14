
using Sitecore.Jobs;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using System.Linq;
using System.Runtime.Remoting.Contexts;

namespace Sitecore.ContentFactory.SmartTools.Dialogs
{
    using Sitecore;
    using Sitecore.Collections;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Data.Managers;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Layouts;
    using Sitecore.Shell.Applications.ContentEditor;
    using Sitecore.Shell.Applications.Dialogs.ProgressBoxes;
    using Sitecore.Web.UI.HtmlControls;
    using Sitecore.Web.UI.Pages;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mail;


    public class CreateContentFactoryDialog : DialogForm
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
        protected Web.UI.HtmlControls.Literal TargetMedia;
        protected TreeList TreeListOfItems;
        protected List<Language> targetLanguagesList;
        protected List<string> targetSiteList;
        protected List<string> ProductPathList;
        protected List<string> CataloguePathList;
        protected List<string> MediaPathList;
        protected string ProductItemID;
        protected string CatalogueItemID;
        //protected string ProductItemPath;
        //protected string CatalogueItemPath;
        protected string MarketNameList;
        protected string BrandField;
        protected string CatalogueItemName;
        protected string ProductItemName;
        protected string BenefitField;
        protected string ItemName;
        protected string CoverageField;
        protected string FormField;
        protected string FinishField;
        protected string ShadeFamilyField;
        protected global::System.Web.UI.WebControls.TextBox Product1;
        protected string StatusLine;
        protected string StatusLine1;
        protected string StatusLine2;
        protected string ProductItemPath;
        protected string CatalogueItemPath;
        protected string msg;
        protected string eml;
        protected string SourceSite;




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
            Item itemFromQueryString = UIUtil.GetItemFromQueryString(Context.ContentDatabase);

            foreach (var i in allItems)
            {
                this.Site.Add(i.Name);
            }

            foreach (var i in allItems)
            {
                if (itemFromQueryString.Paths.FullPath.ToString().Contains(i.Name))
                {
                    SourceSite = i.Name;
                    this.Site.Remove(i.Name);

                }
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




                    string str = "<script type='text/javascript'>function toggleChkBoxMethod2(formName){var form=$(formName);var i=form.getElements('checkbox'); i.each(function(item){item.checked = !item.checked;});$('togglechkbox').checked = !$('togglechkbox').checked;}</script>";
                    str = str + "<table>";
                    this.fillLanguageDictionary();
                    this.fillSiteDictionary();
                    foreach (KeyValuePair<string, string> pair in this.langNames)
                    {

                    }


                    str = str + "</table>";
                    ////this.TargetLanguages.Text = str;
                    if (itemFromQueryString != null)
                    {

                        foreach (var site in this.Site)
                        {
                            string str2 = "chk_" + site;

                            str = str + "<div>" + site + " " + "<input class='reviewerCheckbox' type='checkbox' value='1' name='" + str2 + "'/> </div>";
                            str = str + "<div> Product Path:" + "<input type='textbox' id='Product" + site + "'/> </div>";
                            str = str + "<div> Catalog Path:" + "<input type='textbox' id='Catalog" + site + "'/> </div>";
                            str = str + "<div> Media   Path:" + "<input type='textbox' id='Media" + site + "'/> </div>";
                            str = str + "<style >#TranslateCustom_Reviewer div:nth-child(odd) {background: #f1e7e7;padding: 10px;}#TranslateCustom_Reviewer div:nth-child(even) {background: #CCC;padding: 10px;}#TranslateCustom_Reviewer input[type=‘text’], #Group_Options input[type=‘text’], #TranslateCustom_Reviewer  input[type=‘password’], #TranslateCustom_Reviewer  select, #TranslateCustom_Reviewer  textarea {-moz-box-sizing: border-box;box-sizing: border-box;display: inline-block;width: 65%;min-height: 34px;padding: 8px 12px;font-size: 12px;line-height: 1.42857143;color: #474747;background-color: #ffffff;background-image: none;border: 1px solid #cccccc;border-radius: 2px;box-shadow: inset 0 1px 1px rgba(0, 0, 0, 0.075);transition: border-color ease-in-out .15s, box-shadow ease-in-out .15s;font-family: Arial, Helvetica, sans-serif;margin: 0 0 0 10px;}</style>";



                        }
                        this.TargetLanguages.Text = str;
                    }

                    str = "";
                    str += "<table>";
                    str += "<tr><td>Email:</td><td><input type='textbox' id='email'/></td></tr>";
                    //str += "<tr><td>Include SubItems:</td><td><input class='optionsCheckbox' type='checkbox' value='1' name='chk_IncludeSubItems'/></td></tr>";
                    str += "</table>";
                    this.Options.Text = str;


                    //Options
                    //str = "";
                    //str += eml ="<tr><td>Email:</td><td><input type='textbox' id='email'/></td></tr>";

                    //this.Options.Text = str;
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
            //this.Email();
            targetLanguagesList = new List<Language>();
            targetSiteList = new List<string>();
            ProductPathList = new List<string>();
            CataloguePathList = new List<string>();
            MediaPathList = new List<string>();

            try
            {

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
                        string s2 = Context.ClientPage.Request.Params.Get("Media" + site + "").Replace(",", "") + site;
                        ProductPathList.Add(s);
                        CataloguePathList.Add(s1);
                        MediaPathList.Add(s2);


                    }
                }

                eml = Context.ClientPage.Request.Params.Get("email");

                //Execute the process
                if (itemFromQueryString != null && targetSiteList.Count > 0 && sourceLanguage != null && ProductPathList.Count > 0 && CataloguePathList.Count > 0)
                {
                    //Execute the Job
                    Sitecore.Shell.Applications.Dialogs.ProgressBoxes.ProgressBox.Execute("createContent", "contentfactory", new ProgressBoxMethod(ExecuteOperation), itemFromQueryString);


                }
                else
                {
                    //Show the alert
                    Context.ClientPage.ClientResponse.Alert("Context Item or Target Paths are empty.");
                    Context.ClientPage.ClientResponse.CloseWindow();
                }


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
            Sitecore.Diagnostics.Log.Debug("Content Factory: Job Executed.", this);

            if (parameters == null || parameters.Length == 0)
                return;

            Item item = (Item)parameters[0];

            IterateItems(item, targetSiteList, MediaPathList, ProductPathList, CataloguePathList, sourceLanguage);


        }





        private void IterateItems(Item item, List<string> targetSites, List<string> targetMedia, List<string> targetProduct, List<string> targetCatalogue, Language sourceLang)
        {
            CreateContent(item, targetSites, targetMedia, targetProduct, targetCatalogue, sourceLang);

            if (CopySubItems && item.HasChildren)
            {
                foreach (Item childItem in item.Children)
                {
                    IterateItems(childItem, targetSites, targetMedia, targetProduct, targetCatalogue, sourceLang);

                }
            }
        }


        private void CreateContent(Item item, List<string> targetSites, List<string> targetMedia, List<string> targetProduct, List<string> targetCatalogue, Language sourceLang)
        {
            Item producPageItem = null;
            Item CataloguePageItem = null;
            foreach (var site in targetSites)
            {
                Item source = Context.ContentDatabase.GetItem(item.ID, sourceLang);
                Item sourcelatest = source.Versions.GetLatestVersion(sourceLang);
                var language = source.Languages.FirstOrDefault(l => l.Name == sourceLang.ToString());
                if (language != null)
                {//For checking whether the source item has a version in that source language or not.
                    var languagespecific = Context.ContentDatabase.GetItem(source.ID, language);
                    if (languagespecific.Versions.Count <= 0 || languagespecific.TemplateID.ToString() != ("{B76E9B72-04D9-44A5-8885-64D7022E1AC2}"))
                    {
                        msg = msg + "Source Item version does not exist or Source Item is not elligible." + Environment.NewLine;
                    }
                    if (languagespecific != null && languagespecific.Versions.Count > 0 && languagespecific.TemplateID.ToString().Contains("{B76E9B72-04D9-44A5-8885-64D7022E1AC2}"))

                    {
                       // string path = source.Parent.Parent.Parent.Parent.Paths.FullPath.ToString().Remove(0, 36);
                        string taggingpath = SourceSite.ToString();
                        Item target = Context.ContentDatabase.GetItem(item.ID, language);
                        Database database = Context.ContentDatabase;
                        string name = site.ToString().Remove(0, 14).Replace("_", "-");
                        MarketNameList = site.ToString();
                        var catalogue = database.GetItem(sourcelatest["Product"]);
                        var cataloguelatest = catalogue.Versions.GetLatestVersion(sourceLang);

                        Database masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
                        //Conditions checked for the path provided
                        var catalogue1 = targetCatalogue.SingleOrDefault(x => x.Contains(site));

                        if (catalogue1 == null)
                            catalogue1 = "Required Path does not exist";

                        Sitecore.Data.Items.Item parent1 = masterDb.GetItem(catalogue1);
                        if (parent1 == null)
                        {
                            msg = msg + "Sorry you have either given invalid path or have not provided any path" + Environment.NewLine;
                        }

                        Language languages = Sitecore.Globalization.Language.Parse(name);


                        if (parent1 != null)
                        {
                            string catalogname = "";
                            using (new LanguageSwitcher(languages))
                            {
                                foreach (Item ctname in parent1.Children)
                                {
                                    if (ctname.Name == catalogue.Name)
                                    {
                                        catalogname = ctname.Name;
                                        CatalogueItemName = ctname.Name.ToString() + "  already exist under " + "" + parent1.Paths.FullPath + ", if you want to create again then you have to delete it first.";
                                        msg = msg + CatalogueItemName + Environment.NewLine;
                                    }

                                }
                                //For creating new catalogue
                                if (catalogname == "")

                                {
                                    var ite = cataloguelatest.CopyTo(parent1, catalogue.Name);


                                    CataloguePageItem = ite;
                                    CatalogueItemID = ite.ID.ToString();
                                    ItemName = ite.Name.ToString();
                                    msg = msg + "Details of the newly created Catalogue for " + site + ":-" + "\n" + Environment.NewLine;
                                    msg = msg + "Catalogue Name:- " + ItemName + Environment.NewLine + "Catalogue ID:- " + CatalogueItemID + Environment.NewLine + "Catalogue Path:- " + parent1.Paths.FullPath + Environment.NewLine;


                                    //For tagging in newly created Catalogue
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


                                        }
                                        int count = brand1.Split('|').Length - 2;
                                        int result = items.GetLength(0);
                                        if (count != result)
                                        {
                                            BrandField = "Brand Field, ";
                                            msg = msg + "Please update these fields manually:-" + Environment.NewLine;
                                            msg = msg + BrandField;
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
                                        int count = brand1.Split('|').Length - 2;
                                        int result = items1.GetLength(0);
                                        if (count != result)
                                        {
                                            BenefitField = "Benefit Field, ";

                                            msg = msg + BenefitField;
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

                                        }
                                        if (brand1 != null)
                                        {
                                            ite.Editing.BeginEdit();
                                            ite["Coverage"] = brand1;
                                            ite.Editing.EndEdit();
                                        }
                                        int count = brand1.Split('|').Length - 2;
                                        int result = items2.GetLength(0);
                                        if (count != result)
                                        {
                                            CoverageField = "Coverage Field, ";
                                            msg = msg + CoverageField;
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

                                        }
                                        int count = brand1.Split('|').Length - 2;
                                        int result = items3.GetLength(0);
                                        if (count != result)
                                        {
                                            FinishField = "Finish Field, ";
                                            msg = msg + FinishField;
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

                                        }
                                        int count = brand1.Split('|').Length - 2;
                                        int result = items4.GetLength(0);
                                        if (count != result)
                                        {
                                            FormField = "Form Field ";
                                            msg = msg + FormField + Environment.NewLine + "\n";
                                        }

                                    }




                                    //For checking Path provided and if it exist or not

                                    var product = targetProduct.SingleOrDefault(x => x.Contains(site));

                                    if (product == null)
                                        product = "Required Item Path does not exist";
                                    Sitecore.Data.Items.Item parent = masterDb.GetItem(product);
                                    if (parent == null)
                                    {
                                        msg = msg + "Sorry you have either given invalid path or have not provided any path" + Environment.NewLine;
                                    }
                                    if (parent != null)
                                    {
                                        string productname = "";
                                        foreach (Item ptname in parent.Children)
                                        {
                                            if (ptname.Name == catalogue.Name)
                                            {
                                                productname = "";
                                                ProductItemName = ptname.Name.ToString() + " is already exist under product, if you want to create again then you have to delete it first.";
                                                msg = msg + ProductItemName + Environment.NewLine;
                                            }

                                        }



                                        //For creation of new Product
                                        if (productname == "")
                                        {
                                            StatusLine2 = "\n" + "Details for newly created Product for " + site + ":-" + "\n";
                                            msg = msg + StatusLine2 + Environment.NewLine;
                                            var ite1 = sourcelatest.CopyTo(parent, source.Name);
                                            producPageItem = ite1;
                                            msg = msg + "Product Name:- " + ite1.Name + Environment.NewLine + "Product ID:- " + ite1.ID.ToString() + Environment.NewLine + "Product Path:- " + parent.Paths.FullPath + Environment.NewLine;
                                            Sitecore.Data.Fields.MultilistField multiselectField5 = ite1.Fields["Product"];

                                            Sitecore.Data.Items.Item[] items5 = multiselectField5.GetItems();
                                            if (items5 != null)
                                            {
                                                foreach (var it in items5)
                                                {

                                                    ite1.Editing.BeginEdit();


                                                    ite1["Product"] = ite.ID.ToString();
                                                    ite1.Editing.EndEdit();

                                                }
                                            }






                                        }

                                    }
                                    var mediapath = targetMedia.SingleOrDefault(x => x.Contains(site));
                                    var pathingswitcherimage = database.GetItem(mediapath.Replace(site, "") + "/" + "Pathing Switcher Image");
                                    var SearchResultsImage = database.GetItem(mediapath.Replace(site, "") + "/" + "Search Results Image");
                                    if (pathingswitcherimage != null)
                                    {
                                        foreach (Item childimage in pathingswitcherimage.Children)
                                        {
                                            producPageItem.Editing.BeginEdit();
                                            var prodshot = (Data.Fields.ImageField)producPageItem.Fields["Pathing Switcher Image"];

                                            prodshot.MediaID = childimage.ID;


                                            producPageItem.Editing.EndEdit();

                                        }
                                    }
                                    if (SearchResultsImage != null)
                                    {
                                        foreach (Item childimage in SearchResultsImage.Children)
                                        {
                                            producPageItem.Editing.BeginEdit();
                                            var prodshot = (Data.Fields.ImageField)producPageItem.Fields["Search Results Image"];

                                            prodshot.MediaID = childimage.ID;


                                            producPageItem.Editing.EndEdit();

                                        }
                                    }

                                    //For Shade Family
                                    foreach (Item child in ite.Children)
                                    {
                                        if (mediapath == null)
                                        {
                                            mediapath = "Please provide path";
                                            msg = msg + "You have not provided media path. So, Images will be as per source market.";
                                        }
                                        var mediapathuniq = mediapath.Replace(site, "") + "/" + child.Name + "/";
                                        var productshot = database.GetItem(mediapath.Replace(site, "") + "/" + child.Name + "/" + "Product Shot");
                                        var swatch = database.GetItem(mediapath.Replace(site, "") + "/" + child.Name + "/" + "Swatch");
                                        var additionalimages = database.GetItem(mediapath.Replace(site, "") + "/" + child.Name + "/" + "Additional Images");

                                        if (productshot != null)
                                        {

                                            foreach (Item childimage in productshot.Children)
                                            {
                                                child.Editing.BeginEdit();
                                                var prodshot = (Data.Fields.ImageField)child.Fields["Product Shot"];

                                                prodshot.MediaID = childimage.ID;


                                                child.Editing.EndEdit();

                                            }
                                        }
                                        if (swatch != null)
                                        {

                                            foreach (Item childimage in swatch.Children)
                                            {
                                                child.Editing.BeginEdit();
                                                var prodswatch = (Data.Fields.ImageField)child.Fields["Product Swatch"];
                                                prodswatch.MediaID = childimage.ID;

                                                child.Editing.EndEdit();


                                            }
                                        }

                                        if (additionalimages != null)
                                        {
                                            string image = "";
                                            foreach (Item childimage in additionalimages.Children)
                                            {
                                                image = image + childimage.ID.ToString() + "|";
                                            }
                                            child.Editing.BeginEdit();
                                            child["Additional Images"] = image;
                                            child.Editing.EndEdit();
                                        }

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
                                            }


                                            int count = shade.Split('|').Length - 2;
                                            int result = items5.GetLength(0);
                                            if (count != result)
                                            {
                                                ShadeFamilyField = "ShadeFamily Field ";
                                            }
                                        }
                                    }
                                    msg = msg + "Please update these fields manually:- " + "\n" + ShadeFamilyField + Environment.NewLine + "\n" + "\n";
                                    StatusLine1 = "Also update below Data-source for the used renderings:-" + "\n";
                                    msg = msg + StatusLine1 + Environment.NewLine;



                                    RenderingReference[] renderings = this.GetListOfSublayouts(producPageItem.ID.ToString());
                                    List<string> ListOfDataSource1 = this.GetListOfDataSource(renderings);
                                    foreach (var datasource in ListOfDataSource1)
                                    {
                                        msg = msg + datasource + Environment.NewLine;

                                    }

                                }
                            }
                        }

                    }
                }
                if (producPageItem == null || CataloguePageItem == null)
                {

                }
                else
                {
                    if (producPageItem.Name != null || CataloguePageItem.Name != null)
                    {

                        string languagetarget = site.ToString().Remove(0, 14).Replace("_", "-");
                        Language lang = Sitecore.Globalization.Language.Parse(languagetarget);


                        Item sourceProduct = Context.ContentDatabase.GetItem(producPageItem.ID, sourceLang);
                        Item TargetProductVersion = Context.ContentDatabase.GetItem(producPageItem.ID, lang);
                        Item sourceCatalogue = Context.ContentDatabase.GetItem(CataloguePageItem.ID, sourceLang);
                        Item TargetCatalogueVersion = Context.ContentDatabase.GetItem(CataloguePageItem.ID, lang);
                        if (sourceProduct == null || TargetProductVersion == null || sourceCatalogue == null || TargetCatalogueVersion == null) return;

                        Sitecore.Diagnostics.Log.Debug("Smart Tools: AddVersionAndCopyItems-SourcePath-" + sourceProduct.Paths.Path, this);
                        Sitecore.Diagnostics.Log.Debug("Smart Tools: AddVersionAndCopyItemsSourceLanguage-" + sourceLang.Name, this);
                        Sitecore.Diagnostics.Log.Debug("Smart Tools: AddVersionAndCopyItems-TargetLanguage-" + lang.Name, this);

                        sourceProduct = sourceProduct.Versions.GetLatestVersion();
                        TargetProductVersion.Versions.AddVersion();
                        TargetProductVersion.Editing.BeginEdit();
                        sourceCatalogue = sourceCatalogue.Versions.GetLatestVersion();
                        TargetCatalogueVersion.Versions.AddVersion();
                        TargetCatalogueVersion.Editing.BeginEdit();
                        sourceProduct.Fields.ReadAll();
                        sourceCatalogue.Fields.ReadAll();
                        foreach (Field field in sourceProduct.Fields)
                        {
                            //  if (!field.Name.StartsWith("_")) //(!field.Shared)
                            TargetProductVersion[field.Name] = sourceProduct[field.Name];
                        }
                        foreach (Field field in sourceCatalogue.Fields)
                        {
                            // if (!field.Name.StartsWith("_")) //(!field.Shared)
                            TargetCatalogueVersion[field.Name] = sourceCatalogue[field.Name];
                        }


                        foreach (Item variant in CataloguePageItem.Children)
                        {
                            var variantshade = Context.ContentDatabase.GetItem(variant.ID, sourceLang);
                            var targetshade = Context.ContentDatabase.GetItem(variant.ID, lang);
                            variantshade = variantshade.Versions.GetLatestVersion();
                            targetshade.Versions.AddVersion();
                            targetshade.Editing.BeginEdit();
                            variantshade.Fields.ReadAll();
                            foreach (Field field in variantshade.Fields)
                            {
                                //   if (!field.Name.StartsWith("_")) //(!field.Shared)
                                targetshade[field.Name] = variantshade[field.Name];
                            }
                            targetshade.Editing.EndEdit();
                            variantshade.Versions.RemoveVersion();
                        }
                        TargetProductVersion.Editing.EndEdit();
                        TargetCatalogueVersion.Editing.EndEdit();

                        sourceProduct.Versions.RemoveVersion();
                        sourceCatalogue.Versions.RemoveVersion();
                        Sitecore.Diagnostics.Log.Debug("Smart Tools: Create Content-Completed.", this);

                        var getCataloguePageItems = Sitecore.Context.ContentDatabase.GetItem(CataloguePageItem.ID, lang, new Data.Version(1));
                        var getProductPageItems = Sitecore.Context.ContentDatabase.GetItem(producPageItem.ID, lang, new Data.Version(1));

                        foreach (Item child1 in getCataloguePageItems.Children)
                        {
                            var child = Sitecore.Context.ContentDatabase.GetItem(child1.ID, lang, new Data.Version(1));
                            this.RemovePreviousVersions(child, true);

                        }
                        this.RemovePreviousVersions(getCataloguePageItems, true);
                        this.RemovePreviousVersions(getProductPageItems, true);


                    }
                }
                if (producPageItem == null || CataloguePageItem == null)
                {

                }
                else
                {
                    foreach (Item child in CataloguePageItem.Children)
                    {
                        var shade = Sitecore.Context.ContentDatabase.GetItem(child.ID, sourceLang);
                        if (shade.Versions.Count == 1)
                        {
                            shade.Versions.RemoveVersion();
                        }
                    }
                    var producsource = Sitecore.Context.ContentDatabase.GetItem(producPageItem.ID, sourceLang);
                    if (producsource.Versions.Count == 1)
                    {
                        producsource.Versions.RemoveVersion();
                    }
                    var catalogsource = Sitecore.Context.ContentDatabase.GetItem(CataloguePageItem.ID, sourceLang);
                    if (catalogsource.Versions.Count == 1)
                    {
                        catalogsource.Versions.RemoveVersion();
                    }

                    if (sourceLang.ToString() == "en-HK" || sourceLang.ToString() == "zh-HK")
                    {
                        Language languagehk = Sitecore.Globalization.Language.Parse("en-Hk");
                        Language languagechk = Sitecore.Globalization.Language.Parse("zh-HK");

                        var getproduct = Sitecore.Context.ContentDatabase.GetItem(producsource.ID, languagehk, new Data.Version(1));
                        var getproductchk = Sitecore.Context.ContentDatabase.GetItem(producsource.ID, languagechk, new Data.Version(1));
                        getproduct.Versions.RemoveVersion();
                        getproductchk.Versions.RemoveVersion();
                        var getcatalog = Sitecore.Context.ContentDatabase.GetItem(catalogsource.ID, languagehk, new Data.Version(1));
                        var getcatalogchk = Sitecore.Context.ContentDatabase.GetItem(catalogsource.ID, languagechk, new Data.Version(1));
                        getcatalog.Versions.RemoveVersion();
                        getcatalogchk.Versions.RemoveVersion();
                        foreach (Item child in catalogsource.Children)
                        {
                            var shadehk = Sitecore.Context.ContentDatabase.GetItem(child.ID, languagehk, new Data.Version(1));
                            var shadechk = Sitecore.Context.ContentDatabase.GetItem(child.ID, languagechk, new Data.Version(1));
                            shadehk.Versions.RemoveVersion();
                            shadechk.Versions.RemoveVersion();

                        }

                    }
                    if (site == "Maybelline_V3_zh_HK")
                    {
                        Language forhkproduct = Sitecore.Globalization.Language.Parse("en-HK");
                        string languagetarget = site.ToString().Remove(0, 14).Replace("_", "-");
                        Language lang = Sitecore.Globalization.Language.Parse(languagetarget);

                        var productsource = Context.ContentDatabase.GetItem(producPageItem.ID, lang);
                        var producthk = Context.ContentDatabase.GetItem(producPageItem.ID, forhkproduct);
                        productsource = productsource.Versions.GetLatestVersion();
                        productsource.Fields.ReadAll();
                        producthk.Versions.AddVersion();
                        producthk.Editing.BeginEdit();
                        foreach (Field field in productsource.Fields)
                        {
                            //   if (!field.Name.StartsWith("_")) //(!field.Shared)
                            producthk[field.Name] = productsource[field.Name];
                        }
                        producthk.Editing.EndEdit();
                        producthk = producthk.Versions.GetLatestVersion();
                        if (producthk.Version.Number == 2)
                        {
                            producthk.Versions.RemoveVersion();
                        }
                    }

                    if (site == "Maybelline_V3_zh_HK")
                    {
                        Language forhkproduct = Sitecore.Globalization.Language.Parse("en-HK");
                        string languagetarget = site.ToString().Remove(0, 14).Replace("_", "-");
                        Language lang = Sitecore.Globalization.Language.Parse(languagetarget);


                        var cataloguesource = Context.ContentDatabase.GetItem(CataloguePageItem.ID, lang);
                        var cataloguehk = Context.ContentDatabase.GetItem(CataloguePageItem.ID, forhkproduct);
                        cataloguesource = cataloguesource.Versions.GetLatestVersion();
                        cataloguesource.Fields.ReadAll();
                        cataloguehk.Versions.AddVersion();
                        cataloguehk.Editing.BeginEdit();
                        foreach (Field field in cataloguesource.Fields)
                        {
                            //   if (!field.Name.StartsWith("_")) //(!field.Shared)
                            cataloguehk[field.Name] = cataloguesource[field.Name];
                        }

                        foreach (Item child in CataloguePageItem.Children)
                        {
                            var shade = Context.ContentDatabase.GetItem(child.ID, lang);
                            var childitem = Context.ContentDatabase.GetItem(child.ID, forhkproduct);
                            shade = shade.Versions.GetLatestVersion();
                            shade.Fields.ReadAll();
                            childitem.Versions.AddVersion();
                            childitem.Editing.BeginEdit();
                            foreach (Field field in shade.Fields)
                            {
                                //   if (!field.Name.StartsWith("_")) //(!field.Shared)
                                childitem[field.Name] = shade[field.Name];
                            }
                            childitem.Editing.EndEdit();
                            childitem = childitem.Versions.GetLatestVersion();
                            if (childitem.Version.Number == 2)
                            {
                                childitem.Versions.RemoveVersion();
                            }
                        }

                        cataloguehk.Editing.EndEdit();
                        cataloguehk = cataloguehk.Versions.GetLatestVersion();
                        if (cataloguehk.Version.Number == 2)
                        {
                            cataloguehk.Versions.RemoveVersion();
                        }
                    }
                }
            }
            this.Email();
            var finalReport = Sitecore.Context.ContentDatabase.GetItem("/sitecore/content/MNY/Global Configuration/Content Factory Last Report/Report");
            finalReport.Editing.BeginEdit();
            finalReport["Description"] = msg;
            finalReport["Value"] = System.DateTime.Now.ToString();
            finalReport.Editing.EndEdit();
        }




        private void RemovePreviousVersions(Item myItem, bool includeAllLanguages)
        {
            // get the most recent version
            Item currentVersion = myItem;
            Item[] versions = myItem.Versions.GetVersions(includeAllLanguages);

            // loop through the item versions
            foreach (Item itemVersion in versions)
            {
                // remove the version if it is not the most recent
                if (!itemVersion.Version.Number.Equals(currentVersion.Version.Number))
                {
                    itemVersion.Versions.RemoveVersion();
                }
            }
        }
        private void Email()
        {
            var EmailItem = Context.ContentDatabase.GetItem("/sitecore/content/MNY/Global Configuration/Content Factory Last Report/Email Info");
            string from = EmailItem["Value"].ToString();
            string address = EmailItem["Description"].ToString();
            string[] result = address.Split(new char[] { ',' });

            try
            {
                if (eml != "")
                {
                    MailMessage message = new MailMessage();
                    SmtpClient smtp = new SmtpClient(result[0], System.Convert.ToInt32(result[1]));
                    message.From = new MailAddress(from);

                    message.To.Add(new MailAddress(eml));

                    //message.To.Add(new MailAddress("priyanka.khetarpal@valtech.com"));
                    message.Subject = "Details for newly created items for your market";
                    message.IsBodyHtml = false;
                    message.Body.Replace("\r\n", "\n").Replace("\n", "<br />");
                    message.Body = Environment.NewLine + "Hello,\n" + Environment.NewLine;
                    message.Body = message.Body + msg;


                    message.Body += Environment.NewLine + "Thank You!!";
                    smtp.EnableSsl = false;
                    smtp.UseDefaultCredentials = true;

                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(message);

                }

                else
                {

                }
            }
            catch (Exception exception)
            {
                Sitecore.Diagnostics.Log.Error(exception.Message, this);
            }

        }
        public RenderingReference[] GetListOfSublayouts(string itemId)
        {
            Database database = Context.ContentDatabase;

            Sitecore.Layouts.RenderingReference[] renderings = null;
            if (Sitecore.Data.ID.IsID(itemId))
            {
                Item item = Context.ContentDatabase.GetItem(Sitecore.Data.ID.Parse(itemId));
                if (item != null)
                {
                    var layoutField = new Data.Fields.LayoutField(item.Fields[Sitecore.FieldIDs.FinalLayoutField]);
                    //    var layoutField = new Data.Fields.LayoutField(item);
                    LayoutDefinition layoutDef = LayoutDefinition.Parse(layoutField.Value);
                    DeviceItem device = database.Resources.Devices["Default"];
                    DeviceDefinition deviceDef = layoutDef.GetDevice(device.ID.ToString());
                    renderings = item.Visualization.GetRenderings(device, true);
                    var x = deviceDef.Renderings;

                }
            }
            return renderings;
        }


        #region sandeep

        public RenderingReference[] GetRenderings(string placeholderKey, string deviceName, string itemId)
        {
            Item item = Context.ContentDatabase.GetItem(Sitecore.Data.ID.Parse(itemId));

            Sitecore.Data.Fields.LayoutField layoutField = new Sitecore.Data.Fields.LayoutField(item.Fields[Sitecore.FieldIDs.FinalLayoutField]);
            Sitecore.Layouts.RenderingReference[] renderings = layoutField.GetReferences(GetDeviceItem("Default"));

            //var filteredRenderingList = new List();

            //foreach (var rendering in renderings)
            //{
            //    if (rendering.Placeholder.Contains(placeholderKey))
            //    {
            //        filteredRenderingList.Add(new RenderingItemWrapper(rendering.RenderingItem));
            //    }
            //}

            return renderings;
        }

        //public IEnumerable GetRenderingsSettings(string placeholderKey, string deviceName)
        //{
        //    var currentItem = Sitecore.Context.Item;

        //    Sitecore.Data.Fields.LayoutField layoutField = new Sitecore.Data.Fields.LayoutField(currentItem);
        //    Sitecore.Layouts.RenderingReference[] renderings = layoutField.GetReferences(GetDeviceItem(deviceName));

        //    var filteredRenderingList = new List();

        //    foreach (var rendering in renderings)
        //    {
        //        if (rendering.Placeholder.Contains(placeholderKey))
        //        {
        //            filteredRenderingList.Add(new RenderingSettingsWrapper(rendering.Settings));
        //        }
        //    }

        //    return filteredRenderingList;
        //}

        /// Get the Device from the context database and the Device name
        /// 
        /// 
        /// 
        private Sitecore.Data.Items.DeviceItem GetDeviceItem(string deviceName)
        {
            if (Sitecore.Data.ID.IsID(deviceName))
            {
                return Sitecore.Context.Database.Resources.Devices.GetAll().Where(d => d.ID.Guid == new Guid(deviceName)).First();
            }
            else
            {
                return Sitecore.Context.Database.Resources.Devices.GetAll().Where(d => d.Name.ToLower() == deviceName.ToLower()).First();
            }
        }

        /// 

        /// Get the Datasources for all renderings inside a placeholders
        /// 
        /// 
        /// 
        //public IEnumerable GetRenderingDatasources(string placeholderKey)
        //{
        //    var renderingDatasources = new List();

        //    IEnumerable renderingsSettings = GetRenderingsSettings(placeholderKey);
        //    foreach (var renderingSettings in renderingsSettings)
        //    {
        //        if (renderingSettings.DataSource != null)
        //        {
        //            Sitecore.Data.Items.Item datasourceItem;
        //            if (Sitecore.Data.ID.IsID(renderingSettings.DataSource))
        //            {
        //                datasourceItem = Context.Database.GetItem(new Guid(renderingSettings.DataSource));
        //            }
        //            else
        //            {
        //                datasourceItem = Context.Database.GetItem(renderingSettings.DataSource);
        //            }

        //            if (datasourceItem == null)
        //                continue;

        //            renderingDatasources.Add(datasourceItem);
        //        }
        //    }

        //    return renderingDatasources;
        //}
        #endregion


        public List<string> GetListOfDataSource(RenderingReference[] renderings)
        {
            Database database = Context.ContentDatabase;
            List<string> ListOfDataSource = new List<string>();
            List<string> ListOfDataSource1 = new List<string>();
            foreach (RenderingReference rendering in renderings)
            {
                ListOfDataSource.Add(rendering.Settings.DataSource);
                foreach (string list in ListOfDataSource)
                {
                    if (list != "")
                    {
                        //var itemm1 =database.GetItem(new ID("{CC6E5109-5FB7-48BC-8306-DB39DDEF8AD3}"));
                        //var itemm = database.Items.GetItem(rendering.RenderingID);
                        var renderingName = database.Items.GetItem(rendering.RenderingID).Name.ToString();
                        ListOfDataSource1.Add("DataSource ID:- " + list + "\n" + "Rendering Name:- " + renderingName + "\n" + "Rendering ID:- " + rendering.RenderingID.ToString() + "\n");
                    }
                }
            }
            return ListOfDataSource1;
        }


    }
}

//ite= stores the newly created catalogue
//ite1=stores the newly created Product
//brand1 stores all the tagging which are to be added

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
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;
    /// <summary>
    ///ite= stores the newly created catalogue
    ///ite1=stores the newly created Product
    ///brand1 stores all the tagging which are to be added
    /// </summary>

    public class CreateContentFactoryDialog : DialogForm
    {
        #region Constant Variables
        const string V3_SITE_NAME_GLOBAL = "Maybelline_V3_Global";
        const string V3_SITE_NAME_HK = "Maybelline_V3_zh_HK";
        const string LANGUAGE_TEMPLATE_ID = "{B76E9B72-04D9-44A5-8885-64D7022E1AC2}";
        const string PRODUCT_PAGE_MEDIA_ITEM_ID = "{D8B079D5-D92C-4E53-A3DE-B6414332A3CD}";
        const string PRODUCT_PAGE_CATEGORY_ITEM_ID = "{0D3C8A98-F75A-4C8C-94BE-0FD57B05CB70}";
        const string TEMPLATE_ID = "{17AE6319-D1D0-44F3-B9E3-BC7EA9925F3B}";
        const string ITEM_PATH = "fast:/sitecore/content//*[@@templateid='{E1070C26-B6EB-489B-BB5B-B29AE1F97BDE}']";
        const string CONTENT_FACTORY_PATH = "/sitecore/media library/Content Factory/";
        const string CONTENT_PATH = "/sitecore/content";
        const string CONTENT_PATH_MNY = "sitecore/content/MNY/";
        const string REPORT_PATH = "/sitecore/content/MNY/Global Configuration/Content Factory Last Report/Report";
        const string HOST_CONFIG = "/sitecore/content/MNY/Global Configuration/Content Factory Last Report/HostName Configuration";
        const string LANGUAGE_EN_HK = "en-HK";
        const string LANGUAGE_ZH_HK = "zh-HK";
        const string EMAIL_INFO = "/sitecore/content/MNY/Global Configuration/Content Factory Last Report/Email Info";
        #endregion

        #region Protected Variables
        protected Language SourceLanguage;
        protected bool CopySubItems;
        protected List<string> Sites;
        protected Dictionary<string, string> LangNames;
        protected Combobox Source;
        protected Literal TargetLanguages;
        protected Literal Options;
        protected Literal TargetSites;
        protected Literal TargetPath;
        protected Literal TargetCatalogue;
        protected Literal TargetMedia;
        protected Literal TargetImages;
        protected TreeList TreeListOfItems;
        protected List<Language> TargetLanguagesList;
        protected List<string> TargetSiteList;
        protected List<string> ProductPathList;
        protected List<string> CataloguePathList;
        protected List<string> MediaPathList;
        protected List<string> TargetMediaList;
        protected string SiteName;
        protected string SiteNameList;
        protected string ProductItemID;
        protected string CatalogueItemID;
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
        protected StringBuilder Msg;
        protected string EmailTo;
        protected string Eml;
        protected string SourceSite;
        protected StringBuilder FinaReportDetails;
        protected List<string> EmailList;
        #endregion

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
                    LayoutDefinition layoutDef = LayoutDefinition.Parse(layoutField.Value);
                    DeviceItem device = database.Resources.Devices["Default"];
                    DeviceDefinition deviceDef = layoutDef.GetDevice(device.ID.ToString());
                    renderings = item.Visualization.GetRenderings(device, true);
                    var x = deviceDef.Renderings;
                }
            }
            return renderings;
        }

        public List<string> GetListOfDataSource(RenderingReference[] renderings)
        {
            Database database = Context.ContentDatabase;
            List<string> ListOfDataSource = new List<string>();
            foreach (RenderingReference rendering in renderings)
            {
                if (rendering.Settings.DataSource != string.Empty)
                {
                    var renderingName = database.Items.GetItem(rendering.RenderingID).Name;
                    ListOfDataSource.Add("DataSource ID:- " + rendering.Settings.DataSource + "\n" + "Rendering Name:- " + renderingName + "\n" + "Rendering ID:- " + rendering.RenderingID.ToString() + "\n");
                }
            }
            return ListOfDataSource;
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
                    var child = new Sitecore.Web.UI.HtmlControls.ListItem();
                    this.Source.Controls.Add(child);
                    CultureInfo cltInfo = new CultureInfo(Context.Request.QueryString["ci"]);
                    child.Header = cltInfo.EnglishName;
                    child.Value = cltInfo.EnglishName;
                    child.ID = Control.GetUniqueID("I");

                    if (itemFromQueryString == null)
                        throw new Exception("Item from Query string is null");

                    StringBuilder sbScript = new StringBuilder();
                    sbScript.Append("<script type='text/javascript'>function toggleChkBoxMethod2(formName){var form=$(formName);var i=form.getElements('checkbox'); i.each(function(item){item.checked = !item.checked;});$('togglechkbox').checked = !$('togglechkbox').checked;}</script>");

                    //  sbScript.Append("<script type='text/javascript'>window.addEventListener('load', function () { var acc = document.getElementsByClassName(‘accordion_tab’); var i; for (i = 0; i < acc.length; i++) { acc[i].addEventListener(‘click’, function() { this.classList.toggle(‘active’); var panel = this.nextElementSibling; if (panel.style.display === ‘block’) { panel.style.display = ‘none’; } else { panel.style.display = ‘block’; } }); } //Media path check var checkboxes = document.getElementsByClassName('media_path_checkbox'); for (var index in checkboxes) { //bind event to each checkbox checkboxes[index].onchange = toggleMediapath; } function toggleMediapath() { if (this.checked) { this.parentNode.nextElementSibling.querySelector('.media_path_input').disabled = 'disabled'; } else { this.parentNode.nextElementSibling.querySelector('.media_path_input').disabled = ''; } } });</script>");
                    sbScript.Append("<table>");
                    //this.FillLanguageDictionary();
                    this.FillSiteDictionary();
                    sbScript.Append("</table>");

                    if (itemFromQueryString != null)
                    {

                        foreach (var site in this.Sites)
                        {
                            string str2 = "chk_" + site;
                            string str3 = "chk_" + "Copy Media Images from Source to Target Markets" + site;

                            sbScript.Append("<div class='accordion_tab'>" + site + " " + "<input class='reviewerCheckbox' type='checkbox' value='1' name='" + str2 + "'/> </div>");
                            sbScript.Append("<div class='accordion_content'>");
                            sbScript.Append("<div> Product Path:" + "<input type='textbox' id='Product" + site + "'/> </div>");
                            sbScript.Append("<div> Catalog Path:" + "<input type='textbox' id='Catalog" + site + "'/> </div>");
                            sbScript.Append("<div>  Email:" + "<input type='textbox' id='Email1" + site + "'/> </div>");
                            sbScript.Append("<div>" + "Copy Media Images from Source to Target Markets" + " " + "<input class='reviewerCheckbox media_path_checkbox ' checked='true' type='checkbox' value='1' name='" + str3 + "'/> </div>");
                            sbScript.Append("<div>  Media   Path:" + "<input type='textbox' class='media_path_input' disabled='disabled' id='Media" + site + "'/> </div>");
                            sbScript.Append("</div>");
                        }
                        sbScript.Append("<style>#TranslateCustom_Reviewer div:nth-child(odd){background:#f1e7e7;padding:10px;height:22px;position:relative}#TranslateCustom_Reviewer div:nth-child(even){background:#ccc;padding:10px;height:22px;position:relative}#Group_Options input[type=‘text’],#TranslateCustom_Reviewer input[type=‘password’],#TranslateCustom_Reviewer input[type=‘text’],#TranslateCustom_Reviewer select,#TranslateCustom_Reviewer textarea{-moz-box-sizing:border-box;box-sizing:border-box;display:inline-block;width:65%;min-height:34px;padding:8px 12px;font-size:12px;line-height:1.42857143;color:#474747;background-color:#fff;background-image:none;border:1px solid #ccc;border-radius:2px;box-shadow:inset 0 1px 1px rgba(0,0,0,.075);transition:border-color ease-in-out .15s,box-shadow ease-in-out .15s;font-family:Arial,Helvetica,sans-serif;margin:0 0 0 10px}#Group_Options input[type=textbox],#TranslateCustom_Reviewer input[type=textbox]{height:24px;min-width:65%;position:absolute;left:120px;top:7px;background-color:#fff;background-image:none;border:1px solid #8a8888;border-radius:2px;box-shadow:inset 0 1px 1px rgba(0,0,0,.075);transition:border-color ease-in-out .15s,box-shadow ease-in-out .15s;font-family:Arial,Helvetica,sans-serif}#Group_Options input[type=textbox]{top:0;position: relative; left: 0; width: 285px;}#Group_Options{width:100%;background:#a9a9a9;margin: 10px 0;position:relative;overflow:hidden}#Group_Options #Group_Optionslegend{background:#a9a9a9;width:100%;padding:10px}#Group_Options #Options{padding:10px;display:block}</style>");

                        this.TargetLanguages.Text = sbScript.ToString();
                    }

                    sbScript.Clear();
                    sbScript.Append("<table>");
                    sbScript.Append("<tr><td>Send Report to Email:</td><td><input type='textbox' id='email'/></td></tr>");
                    sbScript.Append("</table>");
                    this.Options.Text = sbScript.ToString();
                }
            }
            catch (Exception exception)
            {
                Sitecore.Diagnostics.Log.Error(exception.Message, this);
                //TODO : Rethrow the exception so that end user will know about error.
            }
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            Item itemFromQueryString = UIUtil.GetItemFromQueryString(Context.ContentDatabase);
            if (itemFromQueryString == null)
            {
                Sitecore.Diagnostics.Log.Debug("Item from Querystring is null", this);
                Context.ClientPage.ClientResponse.Alert("Item from Querystring is null. Please check the logs.");
                Context.ClientPage.ClientResponse.CloseWindow();
            }
            //this.FillLanguageDictionary();
            this.FillSiteDictionary();
            TargetLanguagesList = new List<Language>();
            TargetSiteList = new List<string>();
            ProductPathList = new List<string>();
            CataloguePathList = new List<string>();
            MediaPathList = new List<string>();
            EmailList = new List<string>();
            TargetMediaList = new List<string>();

            try
            {
                //Get the source language
                SourceLanguage = itemFromQueryString.Language;
                Sitecore.Diagnostics.Log.Debug("Smart Tools: OnOK-sourceLanguage-" + SourceLanguage.Name, this);


                //Get the target sites
                foreach (var site in this.Sites)
                {
                    if (!string.IsNullOrEmpty(Context.ClientPage.Request.Params.Get("chk_" + site)))
                    {
                        string productPath = Context.ClientPage.Request.Params.Get("Product" + site).Replace(",", "");
                        string catalogPath = Context.ClientPage.Request.Params.Get("Catalog" + site).Replace(",", "");
                        string mediaPath = Context.ClientPage.Request.Params.Get("Media" + site).Replace(",", "") + site;
                        string emailVal = Context.ClientPage.Request.Params.Get("Email1" + site) + ";" + site;

                        TargetSiteList.Add(site);
                        ProductPathList.Add(productPath);
                        CataloguePathList.Add(catalogPath);
                        MediaPathList.Add(mediaPath);
                        EmailList.Add(emailVal);

                        if (!string.IsNullOrEmpty(Context.ClientPage.Request.Params.Get("chk_" + "Copy Media Images from Source to Target Markets" + site)))
                        {
                            TargetMediaList.Add(site);
                        }
                    }
                }

                Eml = Context.ClientPage.Request.Params.Get("email");

                //Execute the process
                if (itemFromQueryString != null && TargetSiteList.Count > 0 && SourceLanguage != null && ProductPathList.Count > 0 && CataloguePathList.Count > 0)
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
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error(ex.Message, this);
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

            IterateItems(item, TargetSiteList, MediaPathList, ProductPathList, CataloguePathList, TargetMediaList, SourceLanguage);
        }

        #region Private Methods
        private void IterateItems(Item item, List<string> targetSites, List<string> targetMedia, List<string> targetProduct, List<string> targetCatalogue, List<string> targetimage, Language sourceLang)
        {
            CreateContent(item, targetSites, targetMedia, targetProduct, targetCatalogue, targetimage, sourceLang);

            if (CopySubItems && item.HasChildren)
            {
                foreach (Item childItem in item.Children)
                {
                    IterateItems(childItem, targetSites, targetMedia, targetProduct, targetCatalogue, targetimage, sourceLang);
                }
            }
        }

        private void CreateContent(Item item, List<string> targetSites, List<string> targetMedia, List<string> targetProduct, List<string> targetCatalogue, List<string> targetimage, Language sourceLang)
        {
            Item producPageItem = null;
            Item CataloguePageItem = null;
            FinaReportDetails = new StringBuilder();
           

            foreach (var site in targetSites)
            {
                try
                {
                    Msg = new StringBuilder();
                    Item source = Context.ContentDatabase.GetItem(item.ID, sourceLang);
                    Item sourcelatest = source.Versions.GetLatestVersion(sourceLang);
                    var language = source.Languages.FirstOrDefault(l => l.Name == sourceLang.ToString());

                    UpdateSiteItem(item, targetMedia, targetProduct, targetCatalogue, targetimage, sourceLang, ref producPageItem, ref CataloguePageItem, site, source, sourcelatest, language);

                    var media = targetimage.SingleOrDefault(x => x.Contains(site));

                    if (producPageItem != null && media != null)
                    {
                        string name = site.Remove(0, 14).Replace("_", "-");
                        var medipath = Context.ContentDatabase.GetItem(CONTENT_FACTORY_PATH + name);
                        string SubCategoryName = producPageItem.Parent.Name;
                        string CategoryName = producPageItem.Parent.Parent.Name;

                        if (producPageItem.Parent.TemplateID.ToString().Contains(PRODUCT_PAGE_MEDIA_ITEM_ID))
                        {
                            if (!medipath.Children.Any(i => i.Name == CategoryName))
                            {
                                var category = medipath.Add(CategoryName, medipath.Template);
                                if (!medipath.Children.Any(i => i.Name == SubCategoryName))
                                {
                                    category.Add(SubCategoryName, medipath.Template);
                                }
                            }

                            Item parentSubCategory = medipath.Axes.GetDescendants().Where(i => i.Name == SubCategoryName).SingleOrDefault();
                            if (parentSubCategory == null)
                            {
                                var parentcategory = medipath.Axes.GetDescendants().Where(i => i.Name == CategoryName).SingleOrDefault();
                                parentcategory.Add(SubCategoryName, medipath.Template);
                            }

                            parentSubCategory = medipath.Axes.GetDescendants().Where(i => i.Name == SubCategoryName).SingleOrDefault();
                            if (parentSubCategory.Axes.GetDescendants().Any(i => i.Name == producPageItem.Name))
                            {
                                var product = parentSubCategory.Axes.GetDescendants().Where(i => i.Name == producPageItem.Name).SingleOrDefault();
                                product.Delete();
                            }

                            if (!parentSubCategory.Axes.GetDescendants().Any(i => i.Name == producPageItem.Name))
                            {
                                var productfolder = parentSubCategory.Add(producPageItem.Name, medipath.Template);
                                Msg.Append("Media Images Path:-" + productfolder.Parent.Paths.FullPath + Environment.NewLine);

                                Item sourceProduct = Context.ContentDatabase.GetItem(item.ID, sourceLang);
                                Item cataloguelatest = Context.ContentDatabase.GetItem(sourceProduct["Product"]);

                                CopyEditPageItem(producPageItem, productfolder, sourceProduct, "Pathing Switcher Image");
                                CopyEditPageItem(producPageItem, productfolder, sourceProduct, "Search Results Image");

                                if (medipath != null)
                                {
                                    foreach (Item child in CataloguePageItem.Children)
                                    {
                                        Sitecore.Data.Fields.ImageField productshotsource = child.Fields["Product Shot"];
                                        Sitecore.Data.Fields.ImageField swatchsource = child.Fields["Product Swatch"];

                                        var shade = productfolder.Add(child.Name, medipath.Template);

                                        var productshot = shade.Add("Product Shot", shade.Template);
                                        var sourceproductshot = Context.ContentDatabase.GetItem(productshotsource.MediaID);
                                        if (sourceproductshot != null)
                                        {
                                            sourceproductshot.CopyTo(productshot, sourceproductshot.Name);
                                            foreach (Item childimage in productshot.Children)
                                            {
                                                child.Editing.BeginEdit();
                                                var prodshot = (Data.Fields.ImageField)child.Fields["Product Shot"];
                                                prodshot.MediaID = childimage.ID;
                                                child.Editing.EndEdit();
                                            }
                                        }

                                        var swatch = shade.Add("Swatch", shade.Template);
                                        var sourceproductswatch = Context.ContentDatabase.GetItem(swatchsource.MediaID);
                                        if (sourceproductswatch != null)
                                        {
                                            sourceproductswatch.CopyTo(swatch, sourceproductswatch.Name);
                                            foreach (Item childimage in swatch.Children)
                                            {
                                                child.Editing.BeginEdit();
                                                var prodswatch = (Data.Fields.ImageField)child.Fields["Product Swatch"];
                                                prodswatch.MediaID = childimage.ID;
                                                child.Editing.EndEdit();
                                            }
                                        }

                                        var additionalimagestarget = shade.Add("Additional Images", shade.Template);
                                        Sitecore.Data.Fields.MultilistField additionalImages = child.Fields["Additional Images"];
                                        foreach (var field in additionalImages)
                                        {
                                            if (field != null)
                                            {
                                                var additionalimage = Sitecore.Context.ContentDatabase.GetItem(field.ToString());
                                                if (additionalimage != null)
                                                {
                                                    additionalimage.CopyTo(additionalimagestarget, additionalimage.Name);
                                                }
                                            }
                                        }

                                        string image = string.Empty;
                                        foreach (Item childimage in additionalimagestarget.Children)
                                        {
                                            image = image + childimage.ID.ToString() + "|";
                                        }
                                        child.Editing.BeginEdit();
                                        child["Additional Images"] = image;
                                        child.Editing.EndEdit();
                                    }
                                }
                            }
                        }

                        if (producPageItem.Parent.TemplateID.ToString().Contains(PRODUCT_PAGE_CATEGORY_ITEM_ID))
                        {
                            var categoryname = producPageItem.Parent.Name;
                            if (!medipath.Children.Any(i => i.Name == categoryname))
                            {
                                var category = medipath.Add(categoryname, medipath.Template);
                            }

                            Item ParentCategory = medipath.Axes.GetDescendants().Where(i => i.Name == categoryname).SingleOrDefault();
                            if (ParentCategory.Axes.GetDescendants().Any(i => i.Name == producPageItem.Name))
                            {
                                var product = ParentCategory.Axes.GetDescendants().Where(i => i.Name == producPageItem.Name).SingleOrDefault();
                                product.Delete();
                            }

                            if (!ParentCategory.Axes.GetDescendants().Any(i => i.Name == producPageItem.Name))
                            {
                                var productfolder = ParentCategory.Add(producPageItem.Name, medipath.Template);
                                Msg.Append("Media Images Path:-" + productfolder.Parent.Paths.FullPath + Environment.NewLine);

                                Item sourceProduct = Context.ContentDatabase.GetItem(item.ID, sourceLang);
                                Item cataloguelatest = Context.ContentDatabase.GetItem(sourceProduct["Product"]);

                                Sitecore.Data.Fields.ImageField pathingswitcher = sourceProduct.Fields["Pathing Switcher Image"];
                                if (pathingswitcher != null)
                                {
                                    var pathingimage = Context.ContentDatabase.GetItem(pathingswitcher.MediaID);
                                    if (pathingimage != null)
                                    {
                                        var PathingSwitcherImageFolder = productfolder.Add("Pathing Switcher Image", productfolder.Template);
                                        pathingimage.CopyTo(PathingSwitcherImageFolder, pathingimage.Name);
                                        foreach (Item images in PathingSwitcherImageFolder.Children)
                                        {
                                            producPageItem.Editing.BeginEdit();
                                            var pathingswitchimage = (Data.Fields.ImageField)producPageItem.Fields["Pathing Switcher Image"];
                                            pathingswitchimage.MediaID = images.ID;
                                            producPageItem.Editing.EndEdit();
                                        }
                                    }
                                }

                                Sitecore.Data.Fields.ImageField searchimage = sourceProduct.Fields["Search Results Image"];
                                if (searchimage != null)
                                {
                                    var searchresults = Context.ContentDatabase.GetItem(searchimage.MediaID);
                                    if (searchresults != null)
                                    {
                                        var SearchResultsFolder = productfolder.Add("Search Results Image", productfolder.Template);
                                        searchresults.CopyTo(SearchResultsFolder, searchresults.Name);
                                        foreach (Item images1 in SearchResultsFolder.Children)
                                        {
                                            producPageItem.Editing.BeginEdit();
                                            var searchresultimage = (Data.Fields.ImageField)producPageItem.Fields["Search Results Image"];
                                            searchresultimage.MediaID = images1.ID;
                                            producPageItem.Editing.EndEdit();
                                        }
                                    }
                                }

                                if (medipath != null)
                                {
                                    foreach (Item child in CataloguePageItem.Children)
                                    {
                                        Sitecore.Data.Fields.ImageField productshotsource = child.Fields["Product Shot"];
                                        Sitecore.Data.Fields.ImageField swatchsource = child.Fields["Product Swatch"];

                                        var shade = productfolder.Add(child.Name, medipath.Template);
                                        var productshot = shade.Add("Product Shot", shade.Template);

                                        var sourceproductshot = Context.ContentDatabase.GetItem(productshotsource.MediaID);
                                        if (sourceproductshot != null)
                                        {
                                            sourceproductshot.CopyTo(productshot, sourceproductshot.Name);
                                            foreach (Item childimage in productshot.Children)
                                            {
                                                child.Editing.BeginEdit();
                                                var prodshot = (Data.Fields.ImageField)child.Fields["Product Shot"];
                                                prodshot.MediaID = childimage.ID;
                                                child.Editing.EndEdit();
                                            }
                                        }

                                        var swatch = shade.Add("Swatch", shade.Template);
                                        var sourceproductswatch = Context.ContentDatabase.GetItem(swatchsource.MediaID);
                                        if (sourceproductswatch != null)
                                        {
                                            sourceproductswatch.CopyTo(swatch, sourceproductswatch.Name);
                                            foreach (Item childimage in swatch.Children)
                                            {
                                                child.Editing.BeginEdit();
                                                var prodswatch = (Data.Fields.ImageField)child.Fields["Product Swatch"];
                                                prodswatch.MediaID = childimage.ID;
                                                child.Editing.EndEdit();
                                            }
                                        }

                                        var additionalimagestarget = shade.Add("Additional Images", shade.Template);
                                        Sitecore.Data.Fields.MultilistField additionalImages = child.Fields["Additional Images"];
                                        foreach (var field in additionalImages)
                                        {
                                            if (field != null)
                                            {
                                                var additionalimage = Sitecore.Context.ContentDatabase.GetItem(field.ToString());
                                                if (additionalimage != null)
                                                {
                                                    additionalimage.CopyTo(additionalimagestarget, additionalimage.Name);
                                                }
                                            }
                                        }

                                        string image = string.Empty;
                                        foreach (Item childimage in additionalimagestarget.Children)
                                        {
                                            image = image + childimage.ID.ToString() + "|";
                                        }
                                        child.Editing.BeginEdit();
                                        child["Additional Images"] = image;
                                        child.Editing.EndEdit();
                                    }
                                }
                            }
                        }
                    }

                    if (producPageItem != null && CataloguePageItem != null)
                    {
                        string ProductPath = producPageItem.Paths.FullPath.ToString().Replace(CONTENT_PATH_MNY + site + "/Home/", "");
                        var HostEntry = Context.ContentDatabase.GetItem(HOST_CONFIG);
                        string _urlParamsToParse = HostEntry["Host Entries"];
                        NameValueCollection namevalue = Sitecore.Web.WebUtil.ParseUrlParameters(_urlParamsToParse);

                        foreach (var nv in namevalue.Keys)
                        {
                            string sitename = site.Replace("_", "");
                            if (sitename == nv.ToString())
                            {
                                string value = namevalue[nv.ToString()];
                                string PDPURL = value + ProductPath;
                                Msg.Append("Note: Please publish the product from Sitecore after that you can see the product on below URL." + Environment.NewLine);
                                Msg.Append("URL for the newly created product:-  " + PDPURL + Environment.NewLine + "\n");
                            }
                        }

                        string languagetarget = site.Remove(0, 14).Replace("_", "-");
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
                            TargetProductVersion[field.Name] = sourceProduct[field.Name];
                        }

                        foreach (Field field in sourceCatalogue.Fields)
                        {
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

                    if (producPageItem != null && CataloguePageItem != null)
                    {
                        foreach (var Language in producPageItem.Languages)
                        {
                            string languagetarget = site.Remove(0, 14).Replace("_", "-");
                            Language lang = Sitecore.Globalization.Language.Parse(languagetarget);

                            if (Language != lang)
                            {
                                foreach (Item child in CataloguePageItem.Children)
                                {
                                    var shade = Sitecore.Context.ContentDatabase.GetItem(child.ID, Language);
                                    if (shade.Versions.Count == 1)
                                    {
                                        shade.Versions.RemoveVersion();
                                    }
                                }

                                var producsource1 = Sitecore.Context.ContentDatabase.GetItem(producPageItem.ID, Language);
                                if (producsource1.Versions.Count == 1)
                                {
                                    producsource1.Versions.RemoveVersion();
                                }

                                var catalogsource1 = Sitecore.Context.ContentDatabase.GetItem(CataloguePageItem.ID, Language);
                                if (catalogsource1.Versions.Count == 1)
                                {
                                    catalogsource1.Versions.RemoveVersion();
                                }
                            }
                        }
                        var producsource = Sitecore.Context.ContentDatabase.GetItem(producPageItem.ID, sourceLang);
                        var catalogsource = Sitecore.Context.ContentDatabase.GetItem(CataloguePageItem.ID, sourceLang);

                        if (sourceLang.ToString() == LANGUAGE_EN_HK || sourceLang.ToString() == LANGUAGE_ZH_HK)
                        {
                            Language languagehk = Sitecore.Globalization.Language.Parse(LANGUAGE_EN_HK);
                            Language languagechk = Sitecore.Globalization.Language.Parse(LANGUAGE_ZH_HK);

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

                        if (site == V3_SITE_NAME_HK)
                        {
                            Language forhkproduct = Sitecore.Globalization.Language.Parse(LANGUAGE_EN_HK);
                            string languagetarget = site.Remove(0, 14).Replace("_", "-");
                            Language lang = Sitecore.Globalization.Language.Parse(languagetarget);

                            var productsource = Context.ContentDatabase.GetItem(producPageItem.ID, lang);
                            var producthk = Context.ContentDatabase.GetItem(producPageItem.ID, forhkproduct);

                            productsource = productsource.Versions.GetLatestVersion();
                            productsource.Fields.ReadAll();
                            producthk.Versions.AddVersion();
                            producthk.Editing.BeginEdit();

                            foreach (Field field in productsource.Fields)
                            {
                                producthk[field.Name] = productsource[field.Name];
                            }
                            producthk.Editing.EndEdit();
                            producthk = producthk.Versions.GetLatestVersion();
                            if (producthk.Version.Number == 2)
                            {
                                producthk.Versions.RemoveVersion();
                            }


                        }

                        if (site == V3_SITE_NAME_HK)
                        {
                            Language forhkproduct = Sitecore.Globalization.Language.Parse(LANGUAGE_EN_HK);
                            string languagetarget = site.Remove(0, 14).Replace("_", "-");
                            Language lang = Sitecore.Globalization.Language.Parse(languagetarget);

                            var cataloguesource = Context.ContentDatabase.GetItem(CataloguePageItem.ID, lang);
                            var cataloguehk = Context.ContentDatabase.GetItem(CataloguePageItem.ID, forhkproduct);

                            cataloguesource = cataloguesource.Versions.GetLatestVersion();
                            cataloguesource.Fields.ReadAll();
                            cataloguehk.Versions.AddVersion();
                            cataloguehk.Editing.BeginEdit();
                            foreach (Field field in cataloguesource.Fields)
                            {
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

                    EmailTo = EmailList.SingleOrDefault(x => x.Contains(site)).Split(new char[] { ';' })[0];
                    SiteName = site;
                    SiteNameList += site + ", ";

                    this.Email();
                    FinaReportDetails.Append(Msg);

                    producPageItem = null;
                    CataloguePageItem = null;
                }
                catch (Exception)
                {
                    Msg.Append("Something went wrong for " + site + Environment.NewLine);
                }
            }

            Msg = FinaReportDetails;
            SiteName = SiteNameList;            EmailTo = Eml;

            this.Email();


            var finalReport = Sitecore.Context.ContentDatabase.GetItem(REPORT_PATH);
            finalReport.Editing.BeginEdit();
            finalReport["Description"] = Msg.ToString();
            finalReport["Value"] = System.DateTime.Now.ToString();
            finalReport.Editing.EndEdit();
        }

        private static void CopyEditPageItem(Item producPageItem, Item productfolder, Item sourceProduct, string itemType)
        {
            Sitecore.Data.Fields.ImageField pathingswitcher = sourceProduct.Fields[itemType];
            if (pathingswitcher != null)
            {
                var pathingimage = Context.ContentDatabase.GetItem(pathingswitcher.MediaID);
                if (pathingimage != null)
                {
                    var PathingSwitcherImageFolder = productfolder.Add(itemType, productfolder.Template);
                    pathingimage.CopyTo(PathingSwitcherImageFolder, pathingimage.Name);
                    foreach (Item images in PathingSwitcherImageFolder.Children)
                    {
                        producPageItem.Editing.BeginEdit();
                        var pathingswitchimage = (Data.Fields.ImageField)producPageItem.Fields[itemType];
                        pathingswitchimage.MediaID = images.ID;
                        producPageItem.Editing.EndEdit();
                    }
                }
            }
        }

        private void UpdateSiteItem(Item item, List<string> targetMedia, List<string> targetProduct, List<string> targetCatalogue, List<string> targetimage, Language sourceLang, ref Item producPageItem, ref Item CataloguePageItem, string site, Item source, Item sourcelatest, Language language)
        {
            if (language == null)
            {
                return;
            }

            //For checking whether the source item has a version in that source language or not.
            var languagespecific = Context.ContentDatabase.GetItem(source.ID, language);
            Database database = Context.ContentDatabase;
            var catalogue = database.GetItem(sourcelatest["Product"]);
            if (catalogue == null)
                catalogue = source;

            var cataloguespecific = Context.ContentDatabase.GetItem(catalogue.ID, language);

            if (cataloguespecific.Versions.Count <= 0 || languagespecific.Versions.Count <= 0 || languagespecific.TemplateID.ToString() != (LANGUAGE_TEMPLATE_ID))
            {
                Msg.Append("Product version or Catalogue Version does not exist or Product is not elligible." + Environment.NewLine + "\n");
            }

            if (languagespecific != null && cataloguespecific != null && cataloguespecific.Versions.Count > 0 && languagespecific.Versions.Count > 0 && languagespecific.TemplateID.ToString().Contains(LANGUAGE_TEMPLATE_ID))
            {
                string taggingPath = SourceSite;
                Item target = Context.ContentDatabase.GetItem(item.ID, language);
                string name = site.Remove(0, 14).Replace("_", "-");
                MarketNameList = site;
                Database masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
                var cataloguelatest = catalogue.Versions.GetLatestVersion(sourceLang);

                #region Conditions checked for the path provided
                var catalogue1 = targetCatalogue.SingleOrDefault(x => x.Contains(site)).Trim();
                if (catalogue1 == null)
                {
                    catalogue1 = "Required Path does not exist";
                }

                Sitecore.Data.Items.Item parentItem = masterDb.GetItem(catalogue1);
                if (parentItem == null)
                {
                    Msg.Append("Sorry you have either given invalid path or have not provided any path for " + site + " for Catalogue" + Environment.NewLine + "\n");
                }

                var product = targetProduct.SingleOrDefault(x => x.Contains(site)).Trim();
                if (product == null)
                {
                    product = "Required Item Path does not exist";
                }

                Sitecore.Data.Items.Item parent = masterDb.GetItem(product);
                if (parent == null)
                {
                    Msg.Append("Sorry you have either given invalid path or have not provided any path for " + site + " for Product Page" + Environment.NewLine + "\n");
                }
                #endregion

                Language languages = Sitecore.Globalization.Language.Parse(name);

                if (parentItem != null && parent != null)
                {
                    string catalogName = string.Empty;
                    using (new LanguageSwitcher(languages))
                    {
                        foreach (Item ctname in parentItem.Children)
                        {
                            if (ctname.Name == catalogue.Name)
                            {
                                catalogName = ctname.Name;
                                CatalogueItemName = ctname.Name + "  already exist under " + "" + parentItem.Paths.FullPath + " for  " + site.ToString() + ", if you want to create again then you have to delete it first.";
                                Msg.Append(CatalogueItemName + Environment.NewLine + "\n");
                            }
                        }
                        //For creating new catalogue
                        if (string.IsNullOrEmpty(catalogName))
                        {
                            Item catalogItem = cataloguelatest.CopyTo(parentItem, catalogue.Name);
                            CataloguePageItem = catalogItem;
                            CatalogueItemID = catalogItem.ID.ToString();
                            ItemName = catalogItem.Name;
                            Msg.Append("Details of the newly created Catalogue for " + site + ":-" + "\n" + Environment.NewLine);
                            Msg.Append("Catalogue Name:- " + ItemName + Environment.NewLine + "Catalogue ID:- " + CatalogueItemID + Environment.NewLine + "Catalogue Path:- " + parentItem.Paths.FullPath + Environment.NewLine + "\n");

                            string message = string.Empty;

                            //For tagging in newly created Catalogue
                            TaggNewlyCreatedCatalogue(site, catalogue, taggingPath, catalogItem, "Brand", ref BrandField, ref message);
                            TaggNewlyCreatedCatalogue(site, catalogue, taggingPath, catalogItem, "Benefit", ref BenefitField, ref message);
                            TaggNewlyCreatedCatalogue(site, catalogue, taggingPath, catalogItem, "Coverage", ref CoverageField, ref message);
                            TaggNewlyCreatedCatalogue(site, catalogue, taggingPath, catalogItem, "Finish", ref FinishField, ref message);
                            TaggNewlyCreatedCatalogue(site, catalogue, taggingPath, catalogItem, "Form", ref FormField, ref message);

                            if (message != string.Empty)
                            {
                                Msg.Append("Please update these fields manually:-" + Environment.NewLine);
                                Msg.Append(message + Environment.NewLine + "\n");
                            }

                            //For checking Path provided and if it exist or not
                            if (parent != null)
                            {
                                string productname = string.Empty;
                                foreach (Item ptname in parent.Children)
                                {
                                    if (ptname.Name == catalogue.Name)
                                    {
                                        productname = string.Empty;
                                        ProductItemName = ptname.Name + " is already exist under" + " " + parent.Paths.FullPath + " for  " + site.ToString() + ", if you want to create again then you have to delete it first.";
                                        Msg.Append(ProductItemName + "\n" + Environment.NewLine);
                                    }
                                }

                                #region For creation of new Product
                                if (productname == string.Empty)
                                {
                                    StatusLine2 = "\n" + "Details for newly created Product for " + site + ":-" + "\n";
                                    Msg.Append(StatusLine2 + Environment.NewLine);

                                    var ite1 = sourcelatest.CopyTo(parent, source.Name);
                                    producPageItem = ite1;
                                    Msg.Append("Product Name:- " + ite1.Name + Environment.NewLine + "Product ID:- " + ite1.ID.ToString() + Environment.NewLine + "Product Path:- " + parent.Paths.FullPath + Environment.NewLine + "\n");

                                    Sitecore.Data.Fields.MultilistField multiselectField5 = ite1.Fields["Product"];
                                    var items5 = multiselectField5.GetItems();
                                    if (items5 != null)
                                    {
                                        foreach (var it in items5)
                                        {
                                            ite1.Editing.BeginEdit();
                                            ite1["Product"] = catalogItem.ID.ToString();
                                            ite1.Editing.EndEdit();
                                        }
                                    }
                                }
                                #endregion
                            }

                            var mediapath = targetMedia.SingleOrDefault(x => x.Contains(site)).Trim();
                            if (mediapath == site)
                            {
                                mediapath = "some text";
                            }

                            EditProductPageItem(producPageItem, site, database, mediapath, "Pathing Switcher Image");
                            EditProductPageItem(producPageItem, site, database, mediapath, "Search Results Image");

                            var media1 = targetimage.SingleOrDefault(x => x.Contains(site));
                            //For Shade Family
                            UpdateShadeFamily(site, database, taggingPath, catalogItem, mediapath, media1);

                            StatusLine1 = "Also update below Data-source for the used renderings:-" + "\n";
                            Msg.Append(StatusLine1 + Environment.NewLine);

                            RenderingReference[] renderings = this.GetListOfSublayouts(producPageItem.ID.ToString());
                            var ListOfDataSource1 = this.GetListOfDataSource(renderings);
                            foreach (var datasource in ListOfDataSource1)
                            {
                                Msg.Append(datasource + Environment.NewLine);
                            }
                        }
                    }
                }
            }

        }

        private void UpdateShadeFamily(string site, Database database, string taggingPath, Item catalogItem, string mediapath, string media1)
        {
            foreach (Item child in catalogItem.Children)
            {
                if (mediapath == "some text" && media1 == null)
                {
                    Msg.Append("You have not selected any option for media images. So, Images will be as per source market" + Environment.NewLine + "\n");
                    mediapath = "Please provide path";
                }

                //var mediapathuniq = mediapath.Replace(site, "") + "/" + child.Name + "/";
                var productshot = database.GetItem(mediapath.Replace(site, "") + "/" + child.Name + "/" + "Product Shot");
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

                var swatch = database.GetItem(mediapath.Replace(site, "") + "/" + child.Name + "/" + "Swatch");
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

                var additionalimages = database.GetItem(mediapath.Replace(site, "") + "/" + child.Name + "/" + "Additional Images");
                if (additionalimages != null)
                {
                    string image = string.Empty;
                    foreach (Item childimage in additionalimages.Children)
                    {
                        image = image + childimage.ID.ToString() + "|";
                    }
                    child.Editing.BeginEdit();
                    child["Additional Images"] = image;
                    child.Editing.EndEdit();
                }

                Sitecore.Data.Fields.MultilistField shades = child.Fields["ShadeFamily"];
                var items5 = shades.GetItems();
                if (items5 != null)
                {
                    string shade = string.Empty;
                    foreach (var i in items5)
                    {
                        var shadesite = database.GetItem(i.Paths.FullPath.Replace(taggingPath, site));
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

                    if ((shade.Split('|').Length - 1) != items5.GetLength(0))
                    {
                        ShadeFamilyField = "ShadeFamily Field, ";
                        Msg.Append("Please update these fields manually:- " + "\n" + ShadeFamilyField + Environment.NewLine + "\n" + "\n");
                    }
                }
            }
        }

        private static void EditProductPageItem(Item producPageItem, string site, Database database, string mediapath, string itemType)
        {
            var item = database.GetItem(mediapath.Replace(site, "") + "/" + itemType);
            if (item != null)
            {
                foreach (Item childimage in item.Children)
                {
                    producPageItem.Editing.BeginEdit();
                    var prodshot = (Data.Fields.ImageField)producPageItem.Fields[itemType];
                    prodshot.MediaID = childimage.ID;
                    producPageItem.Editing.EndEdit();
                }
            }
        }

        private void TaggNewlyCreatedCatalogue(string site, Item catalogue, string taggingPath, Item catalogItem, string fieldKey, ref string FieldValue, ref string message)
        {
            Database database = Context.ContentDatabase;
            Sitecore.Data.Fields.MultilistField multiselectField = catalogue.Fields[fieldKey];
            Sitecore.Data.Items.Item[] items = multiselectField.GetItems();
            if (items != null)
            {
                string brand1 = string.Empty;
                foreach (var it in items)
                {
                    var brand = database.GetItem(it.Paths.FullPath.Replace(taggingPath, site));
                    if (brand != null)
                    {
                        brand1 = brand1 + brand.ID.ToString() + "|";
                    }
                }

                if (brand1 != null)
                {
                    catalogItem.Editing.BeginEdit();
                    catalogItem["Brand"] = brand1;
                    catalogItem.Editing.EndEdit();
                }
                int count = brand1.Split('|').Length - 1;
                int result = items.GetLength(0);
                if (count != result)
                {
                    FieldValue = fieldKey + " Field, ";
                    message = message + FieldValue;
                }
            }
        }

        private void RemovePreviousVersions(Item myItem, bool includeAllLanguages)
        {
            // get the most recent version
            Item currentVersion = myItem;
            var versions = myItem.Versions.GetVersions(includeAllLanguages);

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
            var emailItem = Context.ContentDatabase.GetItem(EMAIL_INFO);
            string from = emailItem["Value"].ToString();
            string address = emailItem["Description"].ToString();
            string[] result = address.Split(new char[] { ',' });

            try
            {
                if (EmailTo != string.Empty)
                {
                    MailMessage message = new MailMessage();
                    SmtpClient smtp = new SmtpClient(result[0], System.Convert.ToInt32(result[1]));

                    message.From = new MailAddress(from);
                    message.To.Add(new MailAddress(EmailTo));
                    message.Subject = "Details of newly created items for " + SiteName + ":-";
                    message.IsBodyHtml = false;
                    message.Body.Replace("\r\n", "\n").Replace("\n", "<br />");
                    message.Body = Environment.NewLine + "Hello,\n" + Environment.NewLine;
                    message.Body = message.Body + Msg.ToString();
                    message.Body += Environment.NewLine + "Thank You!!";

                    smtp.EnableSsl = false;
                    smtp.UseDefaultCredentials = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error(ex.Message, this);
            }
        }

        private void FillSiteDictionary()
        {
            this.Sites = new List<string>();
            Database database = Context.ContentDatabase;
            var sites = database.GetItem(CONTENT_PATH);
            var allItems = database.SelectItems(ITEM_PATH);

            var item = sites.GetChildren().Where(w => w.TemplateID.ToString() == TEMPLATE_ID);
            Item itemFromQueryString = UIUtil.GetItemFromQueryString(Context.ContentDatabase);

            foreach (var itm in allItems)
            {
                this.Sites.Add(itm.Name);
            }

            foreach (var itm in allItems)
            {
                if (itemFromQueryString.Paths.FullPath.Contains(itm.Name))
                {
                    //TODO : Last Item value will be placed in SourceSite, is it expected?
                    SourceSite = itm.Name;
                    this.Sites.Remove(V3_SITE_NAME_GLOBAL);
                    this.Sites.Remove(itm.Name);
                }
            }
        }
        #endregion
    }
}
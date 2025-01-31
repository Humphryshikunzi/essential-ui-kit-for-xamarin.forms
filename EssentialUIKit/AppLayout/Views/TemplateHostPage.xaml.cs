﻿using System;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using EssentialUIKit.AppLayout.Models;

namespace EssentialUIKit.AppLayout.Views
{
    [Preserve(AllMembers = true)]
    [QueryProperty("QueryData", "data")]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TemplateHostPage
    {
        #region Fields

        private double width;
        private double height;

        #endregion

        #region  Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateHostPage" /> class.
        /// </summary>
        public TemplateHostPage(Template selectedTemplate)
        {
            InitializeComponent();
            TemplateHostView.HeightRequest = HostViewContainer.HeightRequest = Application.Current.MainPage.Height - 55;
            TemplateHostView.WidthRequest = HostViewContainer.WidthRequest = Application.Current.MainPage.Width;

            Title.Text = selectedTemplate.Name;

            this.LoadPage(selectedTemplate.PageName);
        }

        #endregion

        #region Properties

        public string QueryData
        {
            set => this.LoadPage(value);
        }

        #endregion

        #region Methods

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if ((Device.RuntimePlatform == "iOS" || Device.RuntimePlatform == "Android") &&
                AppSettings.Instance.IsSafeAreaEnabled)
            {
                if (width != this.width || height != this.height)
                {
                    var safeAreaHeight = AppSettings.Instance.SafeAreaHeight;
                    this.width = width;
                    this.height = height;

                    if (width < height)
                    {
                        iOSSafeArea.Height = safeAreaHeight;
                    }
                    else
                    {
                        iOSSafeArea.Height = 0;
                    }

                    if (Device.RuntimePlatform == "iOS")
                    {
                        (TemplateHostView.Template as NavigationPage).CurrentPage.Layout(new Rectangle(0, 0, width, height - safeAreaHeight));
                    }
                }
            }
        }

        private void LoadPage(string pageURL)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;

            var page = (Page)Activator.CreateInstance(assembly.GetType($"EssentialUIKit.{pageURL}"));

            TemplateHostView.Template = new NavigationPage(page);

        }

        private void BackButtonPressed(object sender, EventArgs e)
        {
            Navigation.PopAsync(true);
        }

        #endregion
    }
}
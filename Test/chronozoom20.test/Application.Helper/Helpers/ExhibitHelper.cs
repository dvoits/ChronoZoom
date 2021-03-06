﻿using System;
using System.Collections.ObjectModel;
using Application.Driver;
using Application.Helper.Constants;
using Application.Helper.Entities;
using Application.Helper.UserActions;
using OpenQA.Selenium;

namespace Application.Helper.Helpers
{
    public class ExhibitHelper : DependentActions
    {
        public void AddExhibit(Exhibit exhibit)
        {
            Logger.Log("<- " + exhibit);
            InitExhibitCreationMode();
            SetExhibitPoint();
            SetExhibitTitle(exhibit.Title);
            SaveAndClose();
            Logger.Log("->");
        }

        public void AddExhibitWithContentItem(Exhibit exhibit)
        {
            Logger.Log("<- " + exhibit);
            InitExhibitCreationMode();
            SetExhibitPoint();
            SetExhibitTitle(exhibit.Title);
            FillContentItems(exhibit.ContentItems,0);
            SaveAndClose();
            Logger.Log("->");
        }

        public Exhibit GetNewExhibit()
        {
            Logger.Log("<-");
            const string script = Javascripts.LastCanvasElement;
            var exhibit = new Exhibit();
            var contentItem = new ContentItem();
            exhibit.ContentItems = new Collection<Chronozoom.Entities.ContentItem>();
            exhibit.Title = GetJavaScriptExecutionResult(script + ".title");
            int contentItemsCount = int.Parse(GetJavaScriptExecutionResult(script + ".contentItems.length"));
            Logger.Log("- contentItemsCount: " + contentItemsCount);
            for (int i = 0; i < contentItemsCount; i++)
            {
                string item = string.Format("{0}.contentItems[{1}].", script, i);
                contentItem.Title = GetJavaScriptExecutionResult(item + "title");
                Logger.Log("- contentItem.Title: " + contentItem.Title);
                contentItem.Caption = GetJavaScriptExecutionResult(item + "description");
                Logger.Log("- contentItem.Caption: " + contentItem.Caption);
                contentItem.Uri = GetJavaScriptExecutionResult(item + "uri");
                Logger.Log("- contentItem.MediaSource: " + contentItem.MediaSource);
                contentItem.MediaType = GetJavaScriptExecutionResult(item + "mediaType");
                Logger.Log("- contentItem.MediaType: " + contentItem.MediaType);
                contentItem.MediaSource = GetJavaScriptExecutionResult(item + "mediaSource");
                Logger.Log("- contentItem.MediaSource: " + contentItem.MediaSource);
                contentItem.Attribution = GetJavaScriptExecutionResult(item + "attribution");
                Logger.Log("- contentItem.attribution: " + contentItem.Attribution);
                exhibit.ContentItems.Add(contentItem);
            }
            exhibit.Id = new Guid(GetJavaScriptExecutionResult(script + ".guid"));
            Logger.Log("- exhibit.Id: " + exhibit.Id);
            Logger.Log("->" + exhibit);
            return exhibit;
        }

        public void DeleteExhibit(Exhibit exhibit)
        {
            int childrenCount = int.Parse(GetJavaScriptExecutionResult(Javascripts.CosmosChildrenCount));
            for (int i = 0; i < childrenCount; i++)
            {
                string title =
                    GetJavaScriptExecutionResult(
                        string.Format("{0}.children[{1}].title",Javascripts.Cosmos, i));
                if (title == exhibit.Title)
                {
                    ExecuteJavaScript(string.Format("CZ.Service.deleteExhibit({0}.children[{1}])",Javascripts.Cosmos,i));
                }
            }
        }

        public string GetContentItemDescription()
        {
            Logger.Log("<-");
            string description = GetText(By.XPath("//*[@id='vc']/*[@class='contentItemDescription']/div"));
            Logger.Log("-> description: " + description);
            return description;
        }

        private void SaveAndClose()
        {
            Logger.Log("<-");
            Click(By.XPath("//*[@class='ui-dialog-buttonset']/*[1]"));
            Logger.Log("->");
        }

        private void SetExhibitTitle(string timelineName)
        {
            Logger.Log("<- name: " + timelineName);
            TypeText(By.Id("exhibitTitleInput"), timelineName);
            Logger.Log("->");
        }

        private void InitExhibitCreationMode()
        {
            Logger.Log("<-");
            MoveToElementAndClick(By.XPath("//*[@id='footer-authoring']/a[3]"));
            Logger.Log("->");
        }

        private void SetExhibitPoint()
        {
            Logger.Log("<-");
            MoveToElementAndClick(By.ClassName("virtualCanvasLayerCanvas"));
            Logger.Log("->");
        }

        private void FillContentItems(Collection<Chronozoom.Entities.ContentItem> contentItems, int i)
        {
            SetTitle(contentItems[i].Title, i + 1);
            SetCaption(contentItems[i].Caption, i + 1);
            SetUrl(contentItems[i].Uri, i + 1);
            SelectMediaType(contentItems[i].MediaType, i + 1);
            SetAttribution(contentItems[i].Attribution, i + 1);
            SetMediaSourse(contentItems[i].MediaSource, i + 1);
        }

        private void SetMediaSourse(string mediaSource, int i)
        {
            TypeText(By.XPath(string.Format("(//*[@class='cz-authoring-ci-media-source'])[{0}]", i)), mediaSource);
        }

        private void SetAttribution(string attribution, int i)
        {
            TypeText(By.XPath(string.Format("(//*[@class='cz-authoring-ci-attribution'])[{0}]", i)), attribution);
        }

        private void SelectMediaType(string mediaType, int index)
        {
            Select(By.XPath(string.Format("(//*[@class='cz-authoring-ci-media-type'])[{0}]", index)), mediaType);
        }

        private void SetUrl(string mediaSourse, int index)
        {
            TypeText(By.XPath(string.Format("(//*[@class='cz-authoring-ci-uri'])[{0}]", index)), mediaSourse);
        }

        private void SetCaption(string description, int index)
        {
            TypeText(By.XPath(string.Format("(//*[@class='cz-authoring-ci-description'])[{0}]", index)), description);
        }

        private void SetTitle(string title, int index)
        {
            TypeText(By.XPath(string.Format("(//*[@class='cz-authoring-ci-title'])[{0}]", index)), title);
        }
    }
}
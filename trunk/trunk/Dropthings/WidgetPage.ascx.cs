﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dropthings.DataAccess;
using Dropthings.Business;
using Dropthings.Business.Workflows;
using Dropthings.Business.Container;
using Dropthings.Business.Workflows.TabWorkflows;
using System.Workflow.Runtime;
using System.Web.UI.HtmlControls;

public partial class WidgetPage : System.Web.UI.UserControl
{
    public event EventHandler OnReloadPage;

    private const string WIDGET_CONTAINER = "WidgetContainer.ascx";
    private string[] updatePanelIDs = new string[] { "LeftUpdatePanel", "MiddleUpdatePanel", "RightUpdatePanel" };

    public Dropthings.DataAccess.Page CurrentPage { get; set; }
    public List<WidgetInstance> WidgetInstances { get; set; }

    public void LoadWidgets(Dropthings.DataAccess.Page page, 
        Func<WidgetInstance, bool> isWidgetFirstLoad,
        string widgetContainerPath)
    {
        this.CurrentPage = page;
    //    this.WidgetInstances = widgetInstances;

    //    this.WidgetPanelsLayout.SetLayout(this.CurrentPage.LayoutType);
    //    this.SetupWidgets(isWidgetFirstLoad);

        this.SetupWidgetZones(isWidgetFirstLoad, widgetContainerPath);
    }

    private int[] GetColumnWidths()
    {
        int[] panels;
        if (this.CurrentPage.LayoutType == 2)
            panels = new int[] { 25, 75 };
        else if (this.CurrentPage.LayoutType == 3)
            panels = new int[] { 75, 25 };
        else if (this.CurrentPage.LayoutType == 4)
            panels = new int[] { 100 };
        else
            panels = new int[] { 33, 33, 33 };

        return panels;
    }

    private void SetupWidgetZones(
        Func<WidgetInstance, bool> isWidgetFirstLoad,
        string widgetContainerPath)    
    {
        this.Controls.Clear();

        var maxBackground = new HtmlGenericControl("div");
        maxBackground.ID = "widgetMaxBackground";
        maxBackground.Attributes.Add("class", "widget_max_holder");
        maxBackground.Style.Add("display", "none");
        this.Controls.Add(maxBackground);

        var columns = ObjectContainer.Resolve<IWorkflowHelper>()
            .ExecuteWorkflow<GetColumnsInPageWorkflow, GetColumnsInPageWorkflowRequest, GetColumnsInPageWorkflowResponse>(
                ObjectContainer.Resolve<WorkflowRuntime>(),
                new GetColumnsInPageWorkflowRequest { PageId = this.CurrentPage.ID, UserName = Profile.UserName }
            ).Columns;
            
        columns.Each<Column>( (column, index) =>
        {
            var widgetZone = LoadControl("~/WidgetInstanceZone.ascx") as WidgetInstanceZone;
            widgetZone.ID = "WidgetZone" + column.WidgetZoneId;
            widgetZone.WidgetZoneId = column.WidgetZoneId;
            widgetZone.WidgetContainerPath = widgetContainerPath;
            widgetZone.WidgetZoneClass = "widget_zone";
            widgetZone.WidgetClass = "widget";
            widgetZone.NewWidgetClass = "new_widget";
            widgetZone.HandleClass = "widget_header";
            widgetZone.UserName = Profile.UserName;

            var panel = new Panel();
            panel.ID = "WidgetZonePanel" + index;
            panel.CssClass = "column";
            panel.Style.Add(HtmlTextWriterStyle.Width, column.ColumnWidth.Percent());
            
            this.Controls.Add(panel);
            panel.Controls.Add(widgetZone);           
 
            widgetZone.LoadWidgets(isWidgetFirstLoad);
        });
    }
    
    public void RefreshZone(int widgetZoneId)
    {
        var widgetZone = this.FindControl("WidgetZone" + widgetZoneId) as WidgetInstanceZone;
        widgetZone.Refresh();
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {

    }

}

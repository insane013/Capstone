using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TodoListApp.Models.WebApp;

namespace TodoListApp.WebApp.Infrastructure;

[HtmlTargetElement("div", Attributes = "page-model")]
public class PageButtonTagHelper : TagHelper
{
    private readonly IUrlHelperFactory urlHelperFactory;

    public PageButtonTagHelper(IUrlHelperFactory urlHelperFactory)
    {
        this.urlHelperFactory = urlHelperFactory;
    }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext? ViewContext { get; set; }

    public PagingInfo? PageModel { get; set; }

    public string PageAction { get; set; } = string.Empty;

    public string PageController { get; set; } = string.Empty;

    public string PageFilterName { get; set; } = string.Empty;

    public Uri? PageReturnUrl { get; set; }

    public bool PageClassesEnabled { get; set; }

    public string PageClass { get; set; } = string.Empty;

    public string PageClassNormal { get; set; } = string.Empty;

    public string PageClassSelected { get; set; } = string.Empty;

    public string PageFilterStorage { get; set; } = string.Empty;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);
        if (this.ViewContext == null || this.PageModel == null || string.IsNullOrEmpty(this.PageAction))
        {
            return;
        }

        var urlHelper = this.urlHelperFactory.GetUrlHelper(this.ViewContext);
        var result = new TagBuilder("div");

        var url = urlHelper.Action(new UrlActionContext
        {
            Action = this.PageAction,
            Controller = this.PageController,
            Values = new { filterName = this.PageFilterName, returnUrl = this.PageReturnUrl },
        });

        for (int i = 1; i <= this.PageModel.TotalPages; i++)
        {
            var tag = new TagBuilder("button");
            tag.Attributes["type"] = "button";
            tag.Attributes["data-page"] = i.ToString(CultureInfo.InvariantCulture);
            tag.Attributes["data-url"] = url ?? string.Empty;
            tag.Attributes["data-storage"] = this.PageFilterStorage;

            if (this.PageClassesEnabled)
            {
                tag.AddCssClass(this.PageClass);
                tag.AddCssClass(i == this.PageModel.CurrentPage
                    ? this.PageClassSelected
                    : this.PageClassNormal);

                if (i == this.PageModel.CurrentPage)
                {
                    tag.Attributes["disabled"] = "true";
                    tag.AddCssClass("disabled");
                }

                tag.AddCssClass("me-1");
            }

            _ = tag.InnerHtml.Append(i.ToString(CultureInfo.InvariantCulture));
            _ = result.InnerHtml.AppendHtml(tag);
        }

        _ = output.Content.AppendHtml(result.InnerHtml);
    }
}

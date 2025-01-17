﻿namespace Known.Pages;

[StreamRendering]
[Route("/sys/dictionaries")]
public class SysDictionaryList : BaseTablePage<SysDictionary>
{
    private IDictionaryService Service;
    private List<CodeInfo> categories;
    private bool isAddCategory;
    private CodeInfo category;
    private int total;
    private string searchKey;

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Service = await CreateServiceAsync<IDictionaryService>();

        Table.FormTitle = row => $"{PageName} - {row.CategoryName}";
        Table.RowKey = r => r.Id;
        Table.OnQuery = QueryDictionarysAsync;
        Table.Column(c => c.Category).Template((b, r) => b.Text(r.CategoryName));
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
            await LoadCategoriesAsync();
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Div("kui-row-28", () =>
        {
            builder.Div("kui-card", () => BuildListBox(builder));
            builder.Div(() => base.BuildPage(builder));
        });
    }

    public override async Task RefreshAsync()
    {
        if (isAddCategory)
            await LoadCategoriesAsync();
        else
            await base.RefreshAsync();
    }

    private void BuildListBox(RenderTreeBuilder builder)
    {
        builder.Div("kui-dict-search", () =>
        {
            UI.BuildSearch(builder, new InputModel<string>
            {
                Placeholder = "Search",
                Value = searchKey,
                ValueChanged = this.Callback<string>(value =>
                {
                    searchKey = value;
                    StateChanged();
                })
            });
        });
        var items = categories;
        if (!string.IsNullOrWhiteSpace(searchKey))
            items = items.Where(c => c.Code.Contains(searchKey) || c.Name.Contains(searchKey)).ToList();
        builder.Component<KListBox>()
               .Set(c => c.Items, items)
               .Set(c => c.ItemTemplate, ItemTemplate)
               .Set(c => c.OnItemClick, OnCategoryClick)
               .Build();
    }

    private RenderFragment ItemTemplate(CodeInfo info) => b => b.Text($"{info.Name} ({info.Code})");

    private Task OnCategoryClick(CodeInfo info)
    {
        category = info;
        return Table.RefreshAsync();
    }

    private async Task<PagingResult<SysDictionary>> QueryDictionarysAsync(PagingCriteria criteria)
    {
        if (category == null)
            return default;

        criteria.SetQuery(nameof(SysDictionary.Category), QueryType.Equal, category?.Code);
        var result = await Service.QueryDictionariesAsync(criteria);
        total = result.TotalCount;
        return result;
    }

    public void AddCategory()
    {
        isAddCategory = true;
        var model = new DialogModel
        {
            Title = Language.GetString("Button.AddCategory"),
            Width = 800,
            Content = b => b.Component<CategoryGrid>()
                            .Set(c => c.Service, Service)
                            .Set(c => c.OnRefresh, RefreshAsync)
                            .Build()
        };
        UI.ShowDialog(model);
    }

    public void New() => NewForm(category, total);
    public void Edit(SysDictionary row) => Table.EditForm(Service.SaveDictionaryAsync, row);
    public void Delete(SysDictionary row) => Table.Delete(Service.DeleteDictionariesAsync, row);
    public void DeleteM() => Table.DeleteM(Service.DeleteDictionariesAsync);
    public void Import() => ShowImportForm();

    private async Task LoadCategoriesAsync()
    {
        categories = await Service.GetCategoriesAsync();
        category ??= categories?.FirstOrDefault();
        await OnCategoryClick(category);
        await StateChangedAsync();
    }

    private void NewForm(CodeInfo info, int sort)
    {
        isAddCategory = false;
        Table.NewForm(Service.SaveDictionaryAsync, new SysDictionary
        {
            Category = info?.Code,
            CategoryName = info?.Name,
            Sort = sort + 1
        });
    }
}

class CategoryGrid : BaseTable<SysDictionary>
{
    private readonly CodeInfo category = new(Constants.DicCategory, Constants.DicCategory, Constants.DicCategory, null);
    private int total;

    [Parameter] public IDictionaryService Service { get; set; }
    [Parameter] public Func<Task> OnRefresh { get; set; }

    public override async Task RefreshAsync()
    {
        await base.RefreshAsync();
        await OnRefresh?.Invoke();
    }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Table.ShowPager = true;
        Table.OnQuery = QueryDictionariesAsync;
        Table.Form = new FormInfo { Width = 500 };
        Table.Toolbar.AddAction(nameof(New));
        Table.AddColumn(c => c.Code, true).Width(100);
        Table.AddColumn(c => c.Name, true).Width(100);
        Table.AddColumn(c => c.Sort).Width(80);
        Table.AddColumn(c => c.Enabled).Width(80);
        Table.AddColumn(c => c.Note).Width(200);
        Table.AddAction(nameof(Edit));
        Table.AddAction(nameof(Delete));
    }

    public void New()
    {
        Table.NewForm(Service.SaveDictionaryAsync, new SysDictionary
        {
            Category = category.Code,
            CategoryName = category.Name,
            Sort = total + 1
        });
    }

    public void Edit(SysDictionary row) => Table.EditForm(Service.SaveDictionaryAsync, row);
    public void Delete(SysDictionary row) => Table.Delete(Service.DeleteDictionariesAsync, row);

    private async Task<PagingResult<SysDictionary>> QueryDictionariesAsync(PagingCriteria criteria)
    {
        if (category == null)
            return default;

        criteria.SetQuery(nameof(SysDictionary.Category), QueryType.Equal, category?.Code);
        var result = await Service.QueryDictionariesAsync(criteria);
        total = result.TotalCount;
        return result;
    }
}
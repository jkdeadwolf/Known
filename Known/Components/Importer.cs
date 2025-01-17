﻿namespace Known.Components;

class Importer : BaseComponent
{
    private bool isFinished;
    private string fileInfo;
    private string error;
    private string message;
    private IFileService Service;
    private FileDataInfo file;

    private string ErrorMessage => Language["Import.Error"];

    [Parameter] public ImportFormInfo Model { get; set; }
    [Parameter] public Action OnSuccess { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        isFinished = Model.IsFinished;
        error = Model.Error;
        message = Model.Message;
        Service = await CreateServiceAsync<IFileService>();
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-import", () =>
        {
            builder.Div("danger", Language["Import.Tips"]);
            builder.Div("item", () =>
            {
                BuildInputFile(builder);
                if (isFinished)
                    builder.Button(Language.Import, this.Callback<MouseEventArgs>(OnImportAsync), "primary");
                builder.Div("async", () =>
                {
                    UI.BuildCheckBox(builder, new InputModel<bool>
                    {
                        Disabled = !isFinished,
                        Placeholder = Language["Import.IsAsync"],
                        Value = Model.IsAsync,
                        ValueChanged = this.Callback<bool>(v => Model.IsAsync = v)
                    });
                });
            });
            builder.Div(() =>
            {
                builder.Link(Language["Import.Download"], this.Callback(OnDownloadTemplateAsync));
                if (!string.IsNullOrWhiteSpace(error))
                    builder.Link(ErrorMessage, this.Callback(OnErrorMessage));
                builder.Span("size", fileInfo);
            });
            var style = string.IsNullOrWhiteSpace(error) ? "primary" : "danger";
            builder.Div($"kui-import-message {style}", message);
        });
    }

    private void OnErrorMessage()
    {
        UI.ShowDialog(new DialogModel
        {
            Title = ErrorMessage,
            Content = builder => builder.Markup(error)
        });
    }

    private void BuildInputFile(RenderTreeBuilder builder)
    {
        builder.OpenComponent<InputFile>(0);
        builder.AddAttribute(1, "accept", "text/plain,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        builder.AddAttribute(2, "disabled", !isFinished);
        builder.AddAttribute(3, "OnChange", this.Callback<InputFileChangeEventArgs>(OnInputFilesChanged));
        builder.CloseComponent();
    }

    private async void OnInputFilesChanged(InputFileChangeEventArgs e)
    {
        if (e.File == null || e.File.Size == 0)
            return;

        fileInfo = $"{Language["Import.Size"]}{e.File.Size / 1024}KB";
        this.file = await e.File.ReadFileAsync();
    }

    private async void OnImportAsync(MouseEventArgs e)
    {
        if (file == null)
        {
            UI.Error(Language["Import.SelectFile"]);
            return;
        }

        message = Language["Import.Importing"];
        isFinished = false;

        var info = new UploadInfo<ImportFormInfo>(Model);
        info.Files["Upload"] = [file];
        var result = await Service.ImportFilesAsync(info);
        if (!result.IsValid)
        {
            message = result.Message;
            isFinished = true;
            return;
        }

        if (Model.IsAsync)
        {
            message = result.Message;
            isFinished = false;
        }
        else
        {
            OnSuccess?.Invoke();
        }
    }

    private async Task OnDownloadTemplateAsync()
    {
        var bytes = await Service.GetImportRuleAsync(Model.BizId);
        if (bytes == null || bytes.Length == 0)
        {
            UI.Error(Language["Import.FileNotExists"]);
            return;
        }

        var stream = new MemoryStream(bytes);
        await JS.DownloadFileAsync($"{Language["Import.Template"]}_{Model.Name}.xlsx", stream);
    }
}
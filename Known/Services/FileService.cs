﻿namespace Known.Services;

public interface IFileService : IService
{
    Task<PagingResult<SysFile>> QueryFilesAsync(PagingCriteria criteria);
    Task<List<SysFile>> GetFilesAsync(string bizId);
    Task<ImportFormInfo> GetImportAsync(string bizId);
    Task<byte[]> GetImportRuleAsync(string bizId);
    Task<Result> DeleteFilesAsync(List<SysFile> models);
    Task<Result> DeleteFileAsync(SysFile file);
    Task<Result> ImportFilesAsync(UploadInfo<ImportFormInfo> info);
}

class FileService(Context context) : ServiceBase(context), IFileService
{
    //Static
    internal static async Task DeleteFilesAsync(Database db, string bizId, List<string> oldFiles)
    {
        var files = await GetFilesByBizIdAsync(db, bizId);
        await DeleteFilesAsync(db, files, oldFiles);
    }

    //internal static async Task<SysFile> SaveFileAsync(IDatabase db, AttachFile file, string bizId, string bizType, List<string> oldFiles)
    //{
    //    if (file == null)
    //        return null;

    //    await DeleteFilesAsync(db, bizId, bizType, oldFiles);
    //    return await AddFileAsync(db, file, bizId, bizType, "");
    //}

    internal static async Task<List<SysFile>> AddFilesAsync(Database db, List<AttachFile> files, string bizId, string bizType)
    {
        if (files == null || files.Count == 0)
            return null;

        var sysFiles = new List<SysFile>();
        foreach (var item in files)
        {
            var file = await AddFileAsync(db, item, bizId, bizType, "");
            sysFiles.Add(file);
        }
        return sysFiles;
    }

    //File
    public Task<PagingResult<SysFile>> QueryFilesAsync(PagingCriteria criteria)
    {
        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [$"{nameof(SysFile.CreateTime)} desc"];
        return Database.QueryPageAsync<SysFile>(criteria);
    }

    public async Task<Result> DeleteFilesAsync(List<SysFile> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var oldFiles = new List<string>();
        var result = await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await DeleteFileAsync(db, item, oldFiles);
            }
        });
        if (result.IsValid)
            Platform.DeleteFiles(oldFiles);
        return result;
    }

    public async Task<Result> DeleteFileAsync(SysFile file)
    {
        if (file == null || string.IsNullOrWhiteSpace(file.Path))
            return Result.Error(Language["Tip.FileNotExists"]);

        await Database.DeleteAsync(file);
        AttachFile.DeleteFile(file);
        return Result.Success(Language.Success(Language.Delete));
    }

    public async Task<ImportFormInfo> GetImportAsync(string bizId)
    {
        var task = await Repository.GetTaskByBizIdAsync(Database, bizId);
        return ImportHelper.GetImport(Context, bizId, task);
    }

    public async Task<byte[]> GetImportRuleAsync(string bizId)
    {
        return await ImportHelper.GetImportRuleAsync(Database, bizId);
    }

    public Task<List<SysFile>> GetFilesAsync(string bizId) => GetFilesAsync(Database, bizId);

    internal static async Task<List<SysFile>> GetFilesAsync(Database db, string bizId)
    {
        if (string.IsNullOrWhiteSpace(bizId))
            return new List<SysFile>();

        var bizIds = bizId.Split(';');
        if (bizIds.Length > 1)
        {
            var files1 = await db.QueryListAsync<SysFile>(d => d.BizId.In(bizIds));
            return files1?.OrderBy(d => d.CreateTime).ToList();
        }

        if (!bizId.Contains('_'))
            return await GetFilesByBizIdAsync(db, bizId);

        var bizId1 = bizId.Substring(0, bizId.IndexOf('_'));
        var bizType = bizId.Substring(bizId.IndexOf('_') + 1);
        var files = await db.QueryListAsync<SysFile>(d => d.BizId == bizId1 && d.Type == bizType);
        return files?.OrderBy(d => d.CreateTime).ToList();
    }

    public async Task<Result> ImportFilesAsync(UploadInfo<ImportFormInfo> info)
    {
        var form = info.Model;
        SysTask task = null;
        var sysFiles = new List<SysFile>();
        var user = CurrentUser;
        var files = info.Files.GetAttachFiles(user, "Upload", form);
        var result = await Database.TransactionAsync(Language.Upload, async db =>
        {
            sysFiles = await AddFilesAsync(db, files, form.BizId, form.BizType);
            if (form.BizType == ImportHelper.BizType)
            {
                task = ImportHelper.CreateTask(form);
                task.Target = sysFiles[0].Id;
                if (form.IsAsync)
                    await db.SaveAsync(task);
            }
        });
        result.Data = sysFiles;
        if (result.IsValid && form.BizType == ImportHelper.BizType)
        {
            if (form.IsAsync)
                result.Message += Language["Import.FileImporting"];
            else if (task != null)
                result = await ImportHelper.ExecuteAsync(Database, task);
        }
        return result;
    }

    //public Task<Result> UploadImageAsync(UploadInfo info) => UploadFileAsync(info, "Image");
    //public Task<Result> UploadVideoAsync(UploadInfo info) => UploadFileAsync(info, "Video");

    //private async Task<Result> UploadFileAsync(UploadInfo info, string type)
    //{
    //    if (info == null || info.Data == null || info.Data.Length == 0)
    //        return Result.Success("", "");

    //    var user = CurrentUser;
    //    var attach = new AttachFile(info, user);
    //    var fileId = Utils.GetGuid();
    //    attach.IsWeb = true;
    //    attach.FilePath = $@"{user.CompNo}\{type}\{fileId}{attach.ExtName}";
    //    attach.Category1 = "WWW";
    //    attach.Category2 = type;
    //    var file = await AddFileAsync(Database, attach, "Upload", type, "");
    //    return Result.Success("", file.Url);
    //}

    //internal static async Task DeleteFilesAsync(Database db, string bizId, string bizType, List<string> oldFiles)
    //{
    //    var files = await FileRepository.GetFilesAsync(db, bizId, bizType);
    //    await DeleteFilesAsync(db, files, oldFiles);
    //}

    private static async Task<List<SysFile>> GetFilesByBizIdAsync(Database db, string bizId)
    {
        var files = await db.QueryListAsync<SysFile>(d => d.BizId == bizId);
        return files?.OrderBy(d => d.CreateTime).ToList();
    }

    private static async Task DeleteFilesAsync(Database db, List<SysFile> files, List<string> oldFiles)
    {
        if (files == null || files.Count == 0)
            return;

        foreach (var item in files)
        {
            await DeleteFileAsync(db, item, oldFiles);
        }
    }

    private static async Task DeleteFileAsync(Database db, SysFile item, List<string> oldFiles)
    {
        oldFiles.Add(item.Path);
        if (!string.IsNullOrWhiteSpace(item.ThumbPath))
            oldFiles.Add(item.ThumbPath);

        await db.DeleteAsync(item);
    }

    private static async Task<SysFile> AddFileAsync(Database db, AttachFile attach, string bizId, string bizType, string note)
    {
        await attach.SaveAsync();
        attach.BizId = bizId;
        attach.BizType = bizType;
        var file = new SysFile
        {
            CompNo = db.User.CompNo,
            AppId = db.User.AppId,
            Category1 = attach.Category1 ?? "File",
            Category2 = attach.Category2,
            Type = attach.BizType,
            BizId = attach.BizId,
            Name = attach.SourceName,
            Path = attach.FilePath,
            Size = attach.Size,
            SourceName = attach.SourceName,
            ExtName = attach.ExtName,
            ThumbPath = attach.ThumbPath,
            Note = note
        };
        await db.SaveAsync(file);
        return file;
    }
}
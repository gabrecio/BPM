using DevExpress.Pdf;
using DevExpress.Spreadsheet;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
using DevExpress.XtraRichEdit;
using ERP.Globalization;
using ERP.Reports.Gateway;
using ERP.Reports.Gateway.Model;
using ERP.Shared.Application.Contracts.Reporting;
using ERP.Shared.Domain.Reporting;
using ERP.Shared.Service.Contracts.CustomReporting;
using ERP.Shared.Service.Contracts.Reporting;
using ERP.Shared.Service.Contracts.Transactions;
using ERP.Shared.Service.Reporting.Extensions;
using Ionic.Zip;
using Leantect.Domain;
using Leantect.Extensions;
using PVF.Core.Application;
using PVF.Core.Application.Extensions;
using PVF.Core.Application.Helpers;
using PVF.Core.Domain.Configurations;
using PVF.Core.Service;
using PVF.Core.Service.CommandService;
using PVF.Core.Service.Contracts.Files;
using PVF.Core.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Management;
using System.Text;

namespace ERP.Shared.Service.Reporting
{
    public class ReportService : EditServiceBase<Report, IReport, ReportView, IReportRepository, IReportViewRepository>,
                                 IReportService
    {
        public System.Drawing.Image image;
        private readonly ReportFolderConfiguration _reportFolderConfiguration;

        public ReportService(IUnitOfWork unitOfWork, ReportFolderConfiguration reportFolderConfiguration)
            : base(unitOfWork)
        {
            this._reportFolderConfiguration = reportFolderConfiguration;
        }

        //private PictureEdit _image = new DevExpress.XtraEditors.PictureEdit();

        #region Public Method
        protected override void AfterUpdate(Report model, IReportRepository repository, IReport dto)
        {
            repository.RemoveRange(model.LineLogger.Removed);
            base.AfterUpdate(model, repository, dto);
        }



        public ZipReport CreateReportsZipFileClass(List<PDFReportDTO> listOfReportDTO, List<TransactionFileViewDTO> listOfTransactionFileViewDTO)
        {
            using (var compressedFileStream = new MemoryStream())
            {
                var zipReport = new ZipReport();
                //Create an archive and store the stream in memory.
                //using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create))
                using (var zipArchive = new ZipFile())
                {
                    if (listOfReportDTO != null)
                    {
                        foreach (var rptDto in listOfReportDTO)
                        {
                            //Get the report PDF
                            var file = this.GetService<IFileService>().DownloadFile(rptDto.DownloadKey);
                            if (file == null)
                                continue;
                            zipArchive.TryAddFileFromBytes(new ZipArchiveEntryExtensions.FileEntry(rptDto.FileName.Replace(".rpt", ".pdf"), file.Bytes));
                            zipArchive.Save(compressedFileStream);

                            zipReport.OverSize = compressedFileStream.Length / 1024 > 12000;
                            if (zipReport.OverSize)
                                return zipReport;
                        }
                    }

                    if (listOfTransactionFileViewDTO != null)
                    {
                        foreach (var f in listOfTransactionFileViewDTO)
                        {
                            zipArchive.TryAddFileFromBytes(new ZipArchiveEntryExtensions.FileEntry(f.FileName, f.Bytes));
                            zipArchive.Save(compressedFileStream);
                            zipReport.OverSize = compressedFileStream.Length / 1024 > 12000;
                            if (zipReport.OverSize)
                                return zipReport;
                        }
                    }
                }

                zipReport.Bytes = compressedFileStream.ToArray();
                return zipReport;
            }
        }

        public byte[] GetBytesOfFileRPT(int organizationId, string reportId, CustomReportParamDTO customParamDto = null)
        {
            using (this.UnitOfWork)
            {
                customParamDto = customParamDto ?? new CustomReportParamDTO();
                var reportView = this.GetReportView(organizationId, reportId, customParamDto);

                var reportPath = Path.Combine(_reportFolderConfiguration.Folder, reportView.FileName);
                return File.ReadAllBytes(reportPath);
            }
        }

        public decimal GetMaxSizeOfFilesAttachmentsInMegabytes(int organizationId)
        {
            return this.GetConfigByKeyToDecimal(organizationId, nameof(ConfigurationManager.MaxSizeOfFilesAttachmentsInMegabytes));
        }

        public PDFReportDTO GetReportDTO(int organizationId, string reportId, Dictionary<string, object> parameters, Contracts.Reporting.CustomReportParamDTO customReportParamDTO = null, string trxNumber = null, int? trxId = null)
        {
            try
            {

                customReportParamDTO = customReportParamDTO ?? new CustomReportParamDTO();

                using (ReportModel report = GetPDFReport(organizationId, reportId, parameters, customReportParamDTO, trxNumber, trxId))
                {
                    var pdfReport = Map(report);
                    //Save in DownloadFile
                    SaveInDownloadFile(report, pdfReport.DownloadKey);
                    return pdfReport;
                }
            }
            catch (Exception ex)
            {
                throw base.Handle(ex);
            }
        }

        private void SaveInDownloadFile(ReportModel report, Guid downloadKey)
        {
            this.GetService<IFileService>().CreateOrUpdateFile(new FileToUpdateDTO()
            {
                Bytes = report.Bytes,
                FileName = report.FileName,
                DownloadKey = downloadKey
            });
        }
        public ReportFileDTO GetChuckReport(int organizationId, string reportId, Dictionary<string, object> parameters, CustomReportParamDTO customReportParamDTO = null, string trxNumber = null, int? trxId = null)
        {
            customReportParamDTO = customReportParamDTO ?? new CustomReportParamDTO();

            using (ReportModel report = GetPDFReport(organizationId, reportId, parameters, customReportParamDTO, trxNumber, trxId))
            {
                var directory = this.CreateRandomDirectory();
                var fullPath = Path.Combine(directory.FullName, report.FileName);
                File.WriteAllBytes(fullPath, report.Bytes);
                var downloadKey = Guid.NewGuid();
                SaveInDownloadFile(report, downloadKey);
                return new ReportFileDTO
                {
                    FilePathOnServer = fullPath,
                    ReadingPointer = 0,
                    FriendlyName = report.FileName,
                    DownloadKey = downloadKey,
                    FileSize = report.Bytes.Length
                };
            }
        }
        private PDFReportDTO Map(ReportModel report) => new PDFReportDTO
        {

            Copies = report.Copies,
            EmailAttached = report.EmailAttached,
            FileName = report.FileName,
            FileSize = report.Bytes.Length,
            IsPrinted = report.IsPrinted,
            OrganizationId = report.TransactionOrganizationId,
            PrinterName = report.PrinterName,
            WeightForCustomer = report.WeightForCustomer,
            TransactionId = report.TransactionId
        };

        public PDFReportDTO Print(int organizationId, int userId, string reportId, int? copies, Dictionary<string, object> parameters, CustomReportParamDTO customReportParamDTO, string trxNumber = null, int? trxId = null)
        {
            PDFReportDTO pdfReport = null;
            try
            {
                using (this.UnitOfWork)
                {
                    customReportParamDTO = customReportParamDTO ?? new CustomReportParamDTO();
                    using (ReportModel report = GetPDFReport(organizationId, reportId, parameters, customReportParamDTO, trxNumber, trxId))
                    {
                        pdfReport = Map(report);
                        var logInfoPrinter = this.GetConfigByKeyToInt(organizationId, ConfigurationByUserView.EnableLogPrinterInformation);


                        //first check user custom printing...
                        var entityPrinters = report.ReportEntityPrinters
                            .Where(x => x.OrganizationId == organizationId
                                && x.EntityId == userId
                                && x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.User)
                            .ToList();

                        if (entityPrinters.Any())
                        {
                            bool printedInAllPrinters = true;
                            List<string> printerNames = new List<string>();
                            foreach (var printer in entityPrinters)
                            {
                                printedInAllPrinters = ValidateAndPrintInSpecificPrinter(copies ?? printer.QtyCopies, pdfReport, printedInAllPrinters, printer.PrinterName, logInfoPrinter, userId, report);
                                printerNames.Add(printer.PrinterName);
                            }

                            pdfReport.IsPrinted = printedInAllPrinters;
                            //pdfReport.DisposeBytes();
                            pdfReport.PrinterName = string.Join(", ", printerNames.Distinct());
                            return pdfReport;
                        }

                        //then check report custom printing
                        if (report.PrinterId.HasValue)
                        {
                            var printed = ValidateAndPrintInSpecificPrinter(copies ?? pdfReport.Copies, pdfReport, true, report.PrinterName, logInfoPrinter, userId, report);

                            //pdfReport.DisposeBytes();
                            pdfReport.IsPrinted = printed;
                            pdfReport.PrinterName = report.PrinterName;
                            return pdfReport;
                        }

                        LogPrintingInfo(pdfReport.FileName, report.PrinterName, logInfoPrinter, msg: "BEFORE return report to be printed locally", userId: userId);

                        //if no custom printing, return report to be printed locally
                        //Save in DownloadFile (Para No enviar los Bytes y al buscar por el DownloadKey en Attachment se descarguen)
                        SaveInDownloadFile(report, pdfReport.DownloadKey);

                        //pdfReport.DisposeBytes();

                        pdfReport.Copies = copies ?? pdfReport.Copies;
                        pdfReport.IsPrinted = false;
                        pdfReport.PrinterName = null;

                        LogPrintingInfo(pdfReport.FileName, report.PrinterName, logInfoPrinter, msg: "AFTER return report to be printed locally", userId: userId);

                        return pdfReport;
                    };
                }
            }
            catch (Exception ex)
            {
                var extendedError = this.HandlerExecuteReportError(pdfReport, userId, parameters);
                throw base.Handle(ex, extendedError);
            }
        }

        private ReportModel GetPDFReport(int organizationId, string reportId, Dictionary<string, object> parameters, CustomReportParamDTO customReportParamDTO, string trxNumber, int? trxId)
        {
            IReportApiGateway reportApiGateway = GetService<IReportApiGateway>();

            var request = new GetReportRequest()
            {
                CustomReportParameters = new CustomReportParameter
                {
                    CustomReportType = customReportParamDTO?.CustomReportType,
                    EntityId = customReportParamDTO?.EntityId,
                    TrxNumber = customReportParamDTO?.TrxNumber,
                },
                Parameters = parameters,
                ReportId = reportId,
                Transaction = new TransactionBasic
                {
                    Id = trxId,
                    Number = trxNumber,
                    OrganizationId = organizationId
                }
            };
            return reportApiGateway.GetReport(request);
        }

        private string HandlerExecuteReportError(PDFReportDTO report, int userId, Dictionary<string, object> parameters)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"***REPORT*** REPORT ID: {report.FileName}");
            sb.AppendLine($"User Id: {userId}");

            foreach (var p in parameters)
            {
                sb.AppendLine($"{p.Key} : {p.Value ?? (object)DBNull.Value}");
            }

            return sb.ToString();
        }


        //TODO:  download shelved and refactor both Methods Print(...) to make a new generic function.
        public PDFReportDTO Print(int organizationId, ReportParamsDTO reportParams, CustomReportParamDTO customReportParamDTO = null)
        {
            PDFReportDTO pdfReport = null;
            try
            {
                using (this.UnitOfWork)
                {
                    customReportParamDTO = customReportParamDTO ?? new CustomReportParamDTO();

                    ReportModel report = GetPDFReport(organizationId, reportParams.ReportId, reportParams.Parameters, customReportParamDTO, reportParams.TrxNumber, reportParams.TrxId);
                    pdfReport = Map(report);
                    var logInfoPrinter = this.GetConfigByKeyToInt(organizationId, ConfigurationByUserView.EnableLogPrinterInformation);

                    List<ReportEntityPrinterModel> printers = new List<ReportEntityPrinterModel>();

                    switch (reportParams.ReportId)
                    {
                        case Constants.Reports.QuickPicking:
                            //first will be find any printed related to picking's warehouse
                            printers = report.ReportEntityPrinters.Where(x => x.OrganizationId == organizationId &&
                                                                              (
                                                                                (x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.Warehouse && x.EntityId == reportParams.EntityId) ||
                                                                                (x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.User && x.EntityId == reportParams.UserId)
                                                                               )).ToList();
                            break;
                        case Constants.Reports.PullingTicket:
                            //Pulling puede ser por WH, PickingType o AllocatedDemandLog
                            var pirntersOfOrganization = report.ReportEntityPrinters.Where(x => x.OrganizationId == organizationId);
                            var printersByWharehouse = pirntersOfOrganization.Where(x => x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.Warehouse && x.EntityId == reportParams.EntityId && !x.Entity2Id.HasValue && !x.Entity3Id.HasValue);
                            var printersByWharehouseAndPickingTransaction = pirntersOfOrganization.Where(x => x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.Warehouse && x.EntityId == reportParams.EntityId && x.Entity2Id.HasValue && x.Entity2Id == reportParams.EntityTransactionTypeId && !x.Entity3Id.HasValue);
                            var printersByWharehouseAndLogType = pirntersOfOrganization.Where(x => x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.Warehouse && x.EntityId == reportParams.EntityId && x.Entity3Id.HasValue && x.Entity3Id == reportParams.EntityPartStockWarehouseMfrDemandAllocationLogTypeId && !x.Entity2Id.HasValue);
                            var printersByWharehouseAndPickingTransferAndLogType = pirntersOfOrganization.Where(x => x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.Warehouse && x.EntityId == reportParams.EntityId && x.Entity2Id.HasValue && x.Entity2Id == reportParams.EntityTransactionTypeId && x.Entity3Id.HasValue && x.Entity3Id.HasValue && x.Entity3Id == reportParams.EntityPartStockWarehouseMfrDemandAllocationLogTypeId);


                            var printersByUser = pirntersOfOrganization.Where(x => x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.User && x.EntityId == reportParams.UserId && !x.Entity2Id.HasValue && !x.Entity3Id.HasValue);
                            var printersByUserAndPickingTransaction = pirntersOfOrganization.Where(x => x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.User && x.EntityId == reportParams.UserId && x.Entity2Id.HasValue && x.Entity2Id == reportParams.EntityTransactionTypeId && !x.Entity3Id.HasValue);
                            var printersByUserAndLogType = pirntersOfOrganization.Where(x => x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.User && x.EntityId == reportParams.UserId && x.Entity3Id.HasValue && x.Entity3Id == reportParams.EntityPartStockWarehouseMfrDemandAllocationLogTypeId && !x.Entity2Id.HasValue);
                            var printersByUserAndPickingTransferAndLogType = pirntersOfOrganization.Where(x => x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.User && x.EntityId == reportParams.UserId && x.Entity2Id.HasValue && x.Entity2Id == reportParams.EntityTransactionTypeId && x.Entity3Id.HasValue && x.Entity3Id.HasValue && x.Entity3Id == reportParams.EntityPartStockWarehouseMfrDemandAllocationLogTypeId);

                            printers = printersByWharehouse
                                .Union(printersByWharehouseAndPickingTransaction).Union(printersByWharehouseAndLogType).Union(printersByWharehouseAndPickingTransferAndLogType)
                                .Union(printersByUser).Union(printersByUserAndPickingTransaction).Union(printersByUserAndLogType).Union(printersByUserAndPickingTransferAndLogType)
                                .ToList();

                            break;
                        case Constants.Reports.ParcelLabel:
                            //first will be find any printed related to picking's warehouse
                            printers = report.ReportEntityPrinters.Where(x => x.OrganizationId == organizationId &&
                                                                              (
                                                                                (x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.Warehouse && x.EntityId == reportParams.EntityId) ||
                                                                                (x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.User && x.EntityId == reportParams.UserId)
                                                                               )).ToList();
                            break;
                        //For other case add here!
                        //case Constants.Reports.XXX:
                        //    break;

                        default:
                            //default will be find by User type
                            printers = report.ReportEntityPrinters.Where(x => x.OrganizationId == organizationId &&
                                                                              x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.User &&
                                                                              x.EntityId == reportParams.UserId).ToList();
                            break;
                    }

                    //When a printer is found, we must use the parameters of it, instead of the general parameters.
                    if (printers.Any())
                    {
                        bool printedInAllPrinters = true;
                        foreach (var printer in printers)
                        {
                            printedInAllPrinters = ValidateAndPrintInSpecificPrinter(printer.QtyCopies == 0 ? 1 : printer.QtyCopies, pdfReport, printedInAllPrinters, printer.PrinterName, logInfoPrinter, reportParams.UserId, report, false);
                        }
                        pdfReport.IsPrinted = printedInAllPrinters;
                        //pdfReport.DisposeBytes();
                        return pdfReport;
                    }

                    //then check report custom printing
                    if (report.PrinterId.HasValue)
                    {
                        bool printed = ValidateAndPrintInSpecificPrinter(reportParams.Copies ?? report.Copies, pdfReport, true, report.PrinterName, logInfoPrinter, reportParams.UserId, report, false);
                        var qtyCopies = reportParams.Copies ?? report.Copies;
                        pdfReport.IsPrinted = printed;
                        //pdfReport.DisposeBytes();
                        return pdfReport;
                    }

                    LogPrintingInfo(pdfReport.FileName, report.PrinterName, logInfoPrinter, msg: "BEFORE return report to be printed locally", userId: reportParams.UserId);

                    //if no custom printing, return report to be printed locally
                    //Save in DownloadFile (Para No enviar los Bytes y al buscar por el DownloadKey en Attachment se descarguen)
                    SaveInDownloadFile(report, pdfReport.DownloadKey);

                    //pdfReport.DisposeBytes();
                    pdfReport.Copies = reportParams.Copies ?? pdfReport.Copies;
                    pdfReport.IsPrinted = false;

                    LogPrintingInfo(pdfReport.FileName, report.PrinterName, logInfoPrinter, msg: "if no custom printing, return report to be printed locally", userId: reportParams.UserId);

                    return pdfReport;
                }


            }
            catch (Exception ex)
            {
                var extendedError = this.HandlerExecuteReportError(pdfReport, reportParams.UserId, reportParams.Parameters);
                throw base.Handle(ex, extendedError);
            }
        }

        public bool ExistPrintersForReport(int organizationId, ReportParamsDTO reportParams)
        {
            try
            {
                using (this.UnitOfWork)
                {
                    var customReportParamDTO = new CustomReportParamDTO();

                    var report = this.GetReportView(organizationId, reportParams.ReportId, customReportParamDTO);

                    List<Domain.Reporting.ReportEntityPrinterView> printers = new List<Domain.Reporting.ReportEntityPrinterView>();
                    switch (reportParams.ReportId)
                    {
                        case Constants.Reports.QuickPicking:
                            printers = report.ReportEntityPrinters.Where(x => x.OrganizationId == organizationId &&
                                                                                (
                                                                                (x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.Warehouse && x.EntityId == reportParams.EntityId) ||
                                                                                (x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.User && x.EntityId == reportParams.UserId)
                                                                                )).ToList();
                            break;
                        case Constants.Reports.ParcelLabel:
                            printers = report.ReportEntityPrinters.Where(x => x.OrganizationId == organizationId &&
                                                                                (
                                                                                (x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.Warehouse && x.EntityId == reportParams.EntityId) ||
                                                                                (x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.User && x.EntityId == reportParams.UserId)
                                                                                )).ToList();
                            break;
                    }

                    return printers.Any();
                }
            }
            catch (Exception ex)
            {
                throw base.Handle(ex);
            }
        }

        public bool PrintBytesFromDB(int organizationId, Guid downloadKey, ReportParamsDTO reportParams)
        {
            string tmpFolder = string.Empty;
            string reportId = reportParams.ReportId;
            int userId = reportParams.UserId;
            try
            {
                var fileInDB = this.GetService<IFileService>().DownloadFile(downloadKey);

                if (fileInDB == null)
                    return false;

                using (this.UnitOfWork)
                {
                    using (var stream = new MemoryStream(fileInDB.Bytes))
                    {
                        var ext = Path.GetExtension(fileInDB.FileName);
                        if (ext == null)
                            return false;

                        var printed = false;
                        tmpFolder = System.IO.Path.Combine(this.GetTempFolder(), Guid.NewGuid().ToString());
                        if (!Directory.Exists(tmpFolder))
                            Directory.CreateDirectory(tmpFolder);
                        string fullPath = System.IO.Path.Combine(tmpFolder, System.IO.Path.GetFileName(fileInDB.FileName));
                        File.WriteAllBytes(fullPath, (stream).ToArray());

                        var report = GetRepository<IReportViewRepository>().SingleOrDefault(x => x.OrganizationId == organizationId && x.Id == reportId);
                        List<ERP.Shared.Domain.Reporting.ReportEntityPrinterView> entityPrinters = new List<ERP.Shared.Domain.Reporting.ReportEntityPrinterView>();

                        switch (reportParams.ReportId)
                        {
                            case Constants.Reports.ParcelLabel:
                                //first will be find any printed related to picking's warehouse
                                entityPrinters = report.ReportEntityPrinters.Where(x => x.OrganizationId == organizationId &&
                                                                                  (
                                                                                    (x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.Warehouse && x.EntityId == reportParams.EntityId) ||
                                                                                    (x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.User && x.EntityId == reportParams.UserId)
                                                                                   )).ToList();
                                break;
                            //For other case add here!
                            //case Constants.Reports.XXX:
                            //    break;

                            default:
                                //default will be find by User type
                                entityPrinters = report.ReportEntityPrinters.Where(x => x.OrganizationId == organizationId &&
                                                                                  x.ReportEntityPrinterTypeId == (int)ReportEntityPrinterTypes.User &&
                                                                                  x.EntityId == reportParams.UserId).ToList();
                                break;
                        }

                        if (entityPrinters.Any())
                        {
                            printed = true;
                            foreach (var printer in entityPrinters)
                            {
                                for (int i = 0; i < printer.QtyCopies; i++)
                                {
                                    PrintFile(ext, fullPath, printer.PrinterName);
                                }
                            }
                        }

                        //then check report custom printing
                        if (report.PrinterId.HasValue)
                        {
                            printed = true;
                            for (int i = 0; i < report.QtyCopies; i++)
                            {
                                PrintFile(ext, fullPath, report.PrinterName);
                            }
                        }

                        return printed;
                    }
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                TempFolderInServicesHelper.TryDeleteDirectory(tmpFolder, true);
            }
        }

        public ReportViewDTO UpdateReport(int organizationId, ReportDTO dto)
        {
            try
            {
                //can't use the inherit method because field 'Id' is not numeric value!
                //this.UpdateModel(organizationId, dto, dto.Id, dto.Id).Map();
                //just provide an new implementation for Update()
                try
                {
                    Report rep;

                    using (var trx = GetTransactionScopeStandard())
                    {
                        using (this.UnitOfWork)
                        {
                            var repository = GetRepository<IReportRepository>();
                            Report report = null;
                            CommandServiceDispatcher
                                .Create(this.CustomManager, String.Format(CoreMessages.Updating, EntitiesNames.Report))
                                .Execute(() => report = repository.SingleOrDefault(x => x.OrganizationId == organizationId && x.Id == dto.Id))
                                .IfTrueAndSuccessfullThen(report != null, () => report.Initialize().UpdateFromContract(dto))
                                .IfSuccessfulThen(() => this.AfterUpdate(report, repository, dto))
                                .IfSuccessfulThen(() => this.UnitOfWork.Commit(), CoreMessages.CommitChanges)
                                .IfSuccessfulThen(() => trx.Complete(), String.Format(CoreMessages.UpdatedSuccessfully, EntitiesNames.Report, string.Empty))
                                .ThrowIfNotValid();
                        }
                    }

                    using (this.UnitOfWork)
                    {
                        return GetReportViewDTO(x => x.OrganizationId == organizationId && x.Id == dto.Id);
                    }
                }
                catch (Exception ex)
                {
                    throw base.Handle(ex);
                }
            }
            catch (Exception ex)
            {
                throw base.Handle(ex);
            }
        }

        public string GetEmailsFromReportEntityPrinter(int organizationId, ReportParamsDTO reportParams)
        {
            try
            {
                using (this.UnitOfWork)
                {
                    var report = this.GetReportView(organizationId, reportParams.ReportId, new CustomReportParamDTO());

                    var reportEntityPrinter = report.ReportEntityPrinters.FirstOrDefault(x => x.OrganizationId == organizationId &&
                                                                      x.ReportEntityPrinterTypeId == reportParams.ReportEntityPrinterTypeId &&
                                                                      x.EntityId == reportParams.EntityId);
                    return reportEntityPrinter?.EmailTo ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw base.Handle(ex);
            }
        }

        #endregion Public Method

        #region Private Method

        private static void DocServerPrint(string tmpFileName, string printerName)
        {
            using (var docServer = new RichEditDocumentServer())
            {
                docServer.LoadDocument(tmpFileName);

                using (var printingSystem = new PrintingSystemBase())
                {
                    using (var link = new PrintableComponentLinkBase(printingSystem))
                    {
                        link.Component = docServer;
                        link.CreateDocument();

                        // Print to the specified printer.
                        PrintToolBase tool = new PrintToolBase(printingSystem);
                        tool.Print(printerName);

                    }
                }
            }
        }
        private static void PDFPrint(string tmpFileName, string printerName)
        {
            using (PdfDocumentProcessor docServer = new PdfDocumentProcessor())
            {
                docServer.LoadDocument(tmpFileName);
                var setting = new System.Drawing.Printing.PrinterSettings();
                if (!string.IsNullOrEmpty(printerName))
                    setting.PrinterName = printerName;
                docServer.Print(new PdfPrinterSettings(setting));
            }
        }

        private static void SpreedSheetServerPrint(string tmpFileName, string printerName)
        {
            using (var wb = new Workbook())
            {
                wb.LoadDocument(tmpFileName);
                using (var printingSystem = new PrintingSystemBase())
                {
                    using (var link = new PrintableComponentLinkBase(printingSystem))
                    {
                        link.Component = wb;
                        link.CreateDocument(printingSystem);
                        // Print to the specified printer.
                        PrintToolBase tool = new PrintToolBase(printingSystem);
                        tool.Print(printerName);
                    }
                }
            }
        }

        private static bool ValidateAndPrintInSpecificPrinter(int qtyCopies, PDFReportDTO rptDTO, bool printed, string printerName, int logInfoPrinter, int userId, ReportModel report, bool thwowError = true)
        {
            ManagementScope scope = null;
            ManagementObjectCollection printersSearched = null;
            var configuredPrinterIsNotAvailable = false;
            var reportName = rptDTO.FileName;

            PrinterSettings printerSettings = new PrinterSettings
            {
                PrinterName = printerName,
                Copies = (short)qtyCopies,
                Collate = false
            };

            try
            {
                var scopeServer = @"\\" + Environment.MachineName + @"\root\cimv2";

                ConnectionOptions options = new ConnectionOptions
                {
                    Impersonation = ImpersonationLevel.Impersonate
                };

                LogPrintingInfo(reportName, printerName, logInfoPrinter, msg: $"BEGIN Step 1: {scopeServer} SELECT * FROM Win32_Printer Where Name='{printerName}'", userId: userId);

                scope = new ManagementScope(scopeServer);

                scope.Connect();

                // Select Printers from WMI Object Collections
                using (var searcher = new ManagementObjectSearcher($"SELECT * FROM Win32_Printer Where Name='{printerName}'"))
                {
                    printersSearched = searcher.Get();

                    configuredPrinterIsNotAvailable = printersSearched == null || printersSearched.Count == 0;
                }
            }
            catch (UnauthorizedAccessException err)
            {
                var errorMsg = $"{printerName}: PrinterSpool - You do not have authorization to access this printer.\r\n {err.ToString()} ";
                //Cualquier otro error al imprimir, la impresora se esta borrando, etc
                LogPrintingInfo(reportName, printerName, logInfoPrinter, msg: errorMsg, exception: err, userId: userId);

                if (printersSearched != null)
                {
                    printersSearched.Dispose();
                    printersSearched = null;
                }
            }
            catch (Exception ex)
            {
                var errorMsg = $"{printerName}: {ex.Message}";
                //Cualquier otro error al imprimir, la impresora se esta borrando, etc
                LogPrintingInfo(reportName, printerName, logInfoPrinter, msg: errorMsg, exception: ex, userId: userId);

                if (printersSearched != null)
                {
                    printersSearched.Dispose();
                    printersSearched = null;
                }
            }
            finally
            {
                LogPrintingInfo(reportName, printerName, logInfoPrinter, msg: $"END Step 1", userId: userId);
            }

            if (!configuredPrinterIsNotAvailable)
                configuredPrinterIsNotAvailable = !printerSettings.IsValid;

            if (configuredPrinterIsNotAvailable)
            {
                LogPrintingInfo(reportName, printerName, logInfoPrinter, msg: $"BEGIN Step 2: configuredPrinterIsNotAvailable", userId: userId);
                printed = false;
                var messageError = string.Format(Messages.TheConfiguredPrinter0For1ReportIsNotAvailable, printerSettings.PrinterName, rptDTO.FileName);
                rptDTO.Errors.Add(messageError);

                LogPrintingInfo(reportName, printerName, logInfoPrinter, msg: messageError);

                if (printersSearched != null)
                {
                    printersSearched.Dispose();
                    printersSearched = null;
                }

                LogPrintingInfo(reportName, printerName, logInfoPrinter, msg: $"END Step 2", userId: userId);

                if (thwowError)
                    throw new DomainException(messageError);

                return printed;
            }
            else
            {
                LogPrintingInfo(reportName, printerName, logInfoPrinter, msg: $"BEGIN Step 3: OffLine", userId: userId);
                if (printersSearched != null)
                {
                    using (var printer = printersSearched.Cast<ManagementObject>().First())
                    {
                        if (printer["WorkOffline"].ToString().ToLower().Equals("true"))
                        {
                            rptDTO.Errors.Add($"The configured printer ({printerSettings.PrinterName}) for \"{rptDTO.FileName}\" report is offline. When the printer is connected again, they are printed.");
                            //Manda a imprimir igual queda en cola
                        }

                        LogPrintingInfo(reportName, printerName, logInfoPrinter, printer, userId: userId);
                    }
                }
                LogPrintingInfo(reportName, printerName, logInfoPrinter, msg: $"END Step 3", userId: userId);
            }

            try
            {

                LogPrintingInfo(reportName, printerName, logInfoPrinter, msg: $"BEGIN Step 4: Printing", userId: userId);

                rptDTO.PrinterName = printerSettings.PrinterName;

                using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
                using (MemoryStream stream = new MemoryStream(report.Bytes))
                {

                    documentProcessor.LoadDocument(stream);
                    // Declare printer settings.
                    PdfPrinterSettings pdfPrinterSettings = new PdfPrinterSettings();
                    pdfPrinterSettings.EnableLegacyPrinting = true;
                    pdfPrinterSettings.Settings.Copies = printerSettings.Copies;
                    pdfPrinterSettings.Settings.PrinterName = printerSettings.PrinterName;
                    pdfPrinterSettings.Settings.Collate = printerSettings.Collate;
                    // Print the document
                    documentProcessor.Print(pdfPrinterSettings);
                }

            }
            catch (Exception ex2)
            {
                var errorMsg = $"{printerName}: {ex2.Message}";
                //Cualquier otro error al imprimir, la impresora se esta borrando, etc
                rptDTO.Errors.Add(errorMsg);
                printed = false;

                LogPrintingInfo(reportName, printerName, logInfoPrinter, exception: ex2, userId: userId);

                if (printersSearched != null)
                {
                    printersSearched.Dispose();
                    printersSearched = null;
                }

                if (thwowError)
                    throw new DomainException(errorMsg);
            }
            finally
            {
                LogPrintingInfo(reportName, printerName, logInfoPrinter, msg: $"END Step 4", userId: userId);

                if (printersSearched != null)
                {
                    printersSearched.Dispose();
                    printersSearched = null;
                }
            }

            return printed;
        }

        private static void LogPrintingInfo(string friendlyName, string printerName, int logInfoPrinter, ManagementObject printer = null, string msg = null, Exception exception = null, int userId = 0)
        {
            if (logInfoPrinter > 0)
            {
                try
                {
                    printerName = string.IsNullOrEmpty(printerName) ? "NO PRINTER" : printerName;
                    //https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-printer#properties
                    var sb = new StringBuilder();

                    sb.AppendLine("Report Name : " + friendlyName);
                    sb.AppendLine("User Id : " + (userId == 0 ? "NO USER" : userId.ToString()));

                    if (!string.IsNullOrEmpty(msg) && (logInfoPrinter == 2 || logInfoPrinter == 3))
                    {
                        sb.AppendLine("Message : " + msg);
                    }

                    if (exception != null && (logInfoPrinter == 2 || logInfoPrinter == 3))
                    {
                        var fullMessage = BaseExceptionHandler.ExceptionFullMessage(exception);
                        sb.AppendLine("Error Full Message : " + fullMessage);
                    }

                    if (printer != null && (logInfoPrinter == 1 || logInfoPrinter == 3))
                    {
                        sb.AppendLine("Name : " + GetValueFromPrinter(printer, "Name"));
                        sb.AppendLine("PortName : " + GetValueFromPrinter(printer, "PortName"));
                        sb.AppendLine("DriverName : " + GetValueFromPrinter(printer, "DriverName"));
                        sb.AppendLine("DeviceID : " + GetValueFromPrinter(printer, "DeviceID"));
                        sb.AppendLine("Shared : " + GetValueFromPrinter(printer, "Shared"));
                        sb.AppendLine("ShareName : " + GetValueFromPrinter(printer, "ShareName"));
                        sb.AppendLine("WorkOffline : " + GetValueFromPrinter(printer, "WorkOffline"));
                        sb.AppendLine("Local : " + GetValueFromPrinter(printer, "Local"));
                        sb.AppendLine("Network : " + GetValueFromPrinter(printer, "Network"));
                        sb.AppendLine("ServerName : " + GetValueFromPrinter(printer, "ServerName"));
                        sb.AppendLine("Hidden : " + GetValueFromPrinter(printer, "Hidden"));
                        sb.AppendLine("PrinterStatus : " + GetValueFromPrinter(printer, "PrinterStatus"));
                        sb.AppendLine("ExtendedPrinterStatus : " + GetValueFromPrinter(printer, "ExtendedPrinterStatus"));
                        sb.AppendLine("Status : " + GetValueFromPrinter(printer, "Status"));
                        sb.AppendLine("StatusInfo : " + GetValueFromPrinter(printer, "StatusInfo"));
                        sb.AppendLine("Availability : " + GetValueFromPrinter(printer, "Availability"));
                        sb.AppendLine("ConfigManagerUserConfig : " + GetValueFromPrinter(printer, "ConfigManagerUserConfig"));
                        sb.AppendLine("ConfigManagerErrorCode : " + GetValueFromPrinter(printer, "ConfigManagerErrorCode"));
                        sb.AppendLine("DetectedErrorState : " + GetValueFromPrinter(printer, "DetectedErrorState"));
                        sb.AppendLine("ExtendedDetectedErrorState : " + GetValueFromPrinter(printer, "ExtendedDetectedErrorState"));
                        sb.AppendLine("ErrorDescription : " + GetValueFromPrinter(printer, "ErrorDescription"));
                        sb.AppendLine("---------------");
                        sb.AppendLine("https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-printer#properties");
                    }
                    sb.AppendLine("---------------------------------------------------------------");

                    Leantect.Logging.Log.Error($"LOG INFORMATION FOR PRINTER {printerName}", sb.ToString());
                }
                catch (Exception ex1)
                {
                    var errorMsg = $"{printerName}: {ex1.Message}";
                    //In this particular case, do not throw any exception
                    Leantect.Logging.Log.Error(errorMsg, ex1.ToString());
                }
            }
        }

        private static string GetValueFromPrinter(ManagementObject printer, string propertyName)
        {
            var value = string.Empty;

            try
            {
                if (printer[propertyName] != null)
                    value = printer[propertyName].ToString();
            }
            catch (Exception ex)
            {
                value = propertyName + " - " + ex.Message;
            }

            return value;
        }

        private ReportView GetReportView(int organizationId, string reportId, CustomReportParamDTO customReportParamDTO)
        {
            var repo = this.GetRepository<IReportViewRepository>();
            //Get Report
            var reportView = repo.SingleOrDefault(x => x.OrganizationId == organizationId && x.Id == reportId);
            DomainValidationResult.Create().WithError(reportView == null, "Cannot find report " + reportId);

            //Check for customization
            if (customReportParamDTO.EntityId.HasValue && customReportParamDTO.CustomReportType.HasValue && !reportView.ReportCustomizations.Empty())
            {
                var customization = reportView.ReportCustomizations
                    .SingleOrDefault(x => x.OrganizationId == organizationId &&
                                                              x.Id == reportId &&
                                                              x.CustomizationTypeId == customReportParamDTO.CustomReportType &&
                                                              x.EntityId == customReportParamDTO.EntityId);


                if (customization != null)
                {
                    reportView = repo.SingleOrDefault(x => x.OrganizationId == customization.OrganizationId && x.Id == customization.ReportId);
                    DomainValidationResult.Create().WithError(reportView == null, "Cannot find report " + customization.ReportId);
                }
            }

            return reportView;
        }

        private ReportViewDTO GetReportViewDTO(Expression<Func<ReportView, bool>> predicate)
        {
            var repository = GetRepository<IReportViewRepository>();
            var report = repository.SingleOrDefault(predicate);

            return (report == null) ? null : report.Map();
        }



        private void ImageServerPrint(string tmpFileName, string printerName)
        {
            image = null;
            using (image = System.Drawing.Image.FromFile(tmpFileName))
            {
                using (var link = new PrintDocument())
                {
                    link.OriginAtMargins = true;
                    link.DocumentName = "";
                    link.PrinterSettings.PrinterName = printerName;

                    link.PrintPage += link_PrintPage;
                    link.Print();
                }
            }
        }

        private ReportEntityPrinter InitializeReportEntityPrinter(int organizationId, ReportEntityPrinterDTO dto)
        {
            return new ReportEntityPrinter(organizationId, dto);
        }

        private void link_PrintPage(object sender, PrintPageEventArgs e)
        {
            Point loc = new Point(10, 10);
            e.Graphics.DrawImage(image, loc);
        }

        private void PrintFile(string ext, string tmpFileName, string printerName)
        {
            switch (ext.ToLower())
            {
                case ".docx":
                    DocServerPrint(tmpFileName, printerName);
                    break;

                case ".doc":
                    DocServerPrint(tmpFileName, printerName);
                    break;

                case ".xls":
                    SpreedSheetServerPrint(tmpFileName, printerName);
                    break;

                case ".xlsx":
                    SpreedSheetServerPrint(tmpFileName, printerName);
                    break;

                case ".pdf":
                    PDFPrint(tmpFileName, printerName);
                    break;

                case ".jpg":
                    ImageServerPrint(tmpFileName, printerName);
                    break;

                case ".bmp":
                    ImageServerPrint(tmpFileName, printerName);
                    break;

                case ".tiff":
                    ImageServerPrint(tmpFileName, printerName);
                    break;

                case ".tif":
                    ImageServerPrint(tmpFileName, printerName);
                    break;

                case ".png":
                    ImageServerPrint(tmpFileName, printerName);
                    break;
            }
        }

        private string FormattedExceptionMessage(string message)
        {
            char[] delimiters = new char[] { '\\' };
            string[] lines = message.Split(delimiters);
            var formattedMessage = "'" + lines.Last();

            return string.Format(Messages.CouldNotFindTheReport0, formattedMessage);
        }

        #endregion Private Method

        #region Implementation of EditServiceBase

        protected override System.Linq.Expressions.Expression<Func<Report, bool>> FindModelById(int organizationId, int id)
        {
            throw new NotImplementedException();
        }

        protected override Expression<Func<ReportView, bool>> FindViewById(int organizationId, int id)
        {
            //TODO: review because id is string and NOT int!!!!
            //return x => x.Id == id;

            return null;
        }

        protected override string GetEntityName()
        {
            return this.EntityName;
        }

        protected override Report NewModel(int organizationId, IReport dto)
        {
            return new Report(dto).Initialize();
        }

        protected override Report UpdateModel(Report model, IReport dto)
        {
            return model.Initialize().UpdateFromContract(dto);
        }

        #endregion Implementation of EditServiceBase

        #region IReportService Implementation

        public List<ReportViewDTO> GetAllReports()
        {
            try
            {
                using (this.UnitOfWork)
                    return GetRepository<IReportViewRepository>().AsQueryable().ToList().Map();
            }
            catch (Exception ex)
            {
                throw base.Handle(ex);
            }
        }

        public List<ReportCustomizationTypeViewDTO> GetReportCustomizationTypes()
        {
            try
            {
                using (this.UnitOfWork)
                    return GetRepository<IReportCustomizationTypeViewRepository>().AsQueryable().ToList().Map();
            }
            catch (Exception ex)
            {
                throw base.Handle(ex);
            }
        }

        public List<ReportEntityPrinterTypeViewDTO> GetReportEntityPrinterTypes()
        {
            try
            {
                using (this.UnitOfWork)
                    return GetRepository<IReportEntityPrinterTypeViewRepository>().AsQueryable().ToList().Map();
            }
            catch (Exception ex)
            {
                throw base.Handle(ex);
            }
        }

        public List<ReportViewDTO> Search(int organizationId, ReportSearchParamsDTO param)
        {
            try
            {
                using (this.UnitOfWork)
                {
                    var repository = GetRepository<IReportViewRepository>();
                    List<ReportView> reports;

                    reports = repository.AsQueryable()
                            .Where(x =>
                                  (x.OrganizationId == organizationId) &&
                                  (string.IsNullOrEmpty(param.Id) || x.Id.Contains(param.Id)) &&
                                  (string.IsNullOrEmpty(param.FileName) || x.FileName.Contains(param.FileName)) &&
                                  (string.IsNullOrEmpty(param.FriendlyName) || x.FileName.Contains(param.FriendlyName)) &&
                                  (string.IsNullOrEmpty(param.StoreProcedure) || x.FileName.Contains(param.StoreProcedure)))
                            .OrderBy(x => x.Id).ToList();

                    return reports.Map();
                }
            }
            catch (Exception ex)
            {
                throw base.Handle(ex);
            }
        }

        public List<ReportViewDTO> GetAllReportsExcept(int organizationId, string reportExcludeId)
        {
            try
            {
                using (this.UnitOfWork)
                {
                    var repo = this.GetRepository<IReportViewRepository>();
                    var results = repo
                        .AsQueryable(x => x.OrganizationId == organizationId && x.Id != reportExcludeId)
                        .Map();
                    return results;
                }
            }
            catch (Exception ex)
            {

                throw base.Handle(ex);
            }
        }

        public void UpdateMessageOfTheDay(List<IMessageOfTheDay> dto)
        {
            using (this.UnitOfWork)
            {
                var repo = GetRepository<IReportRepository>();
                repo.UpdateMessageOfTheDay(dto);
            }
        }

        #endregion IReportService Implementation
    }
}

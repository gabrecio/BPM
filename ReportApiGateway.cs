using ERP.Reports.Gateway.Model;
using Leantect.Domain;
using PVF.Core.Service;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Reports.Gateway
{

    public class ReportApiGateway : ServiceBase, IReportApiGateway
    {
        private const string ERP_REPORTS_API_TOKEN = "ERP.Reports.Api.Token";
        private readonly IApiReportClient apiReportClient;

        public ReportApiGateway(IUnitOfWork unitOfWork, IApiReportClient apiReportClient)
            : base(unitOfWork)
        {
            this.apiReportClient = apiReportClient;
        }
        public ReportModel GetReport(GetReportRequest reportRequest)
        {
            var response = CallReportApi(apiReportClient, api => api.Reports_GetReportAsync(reportRequest));
            return Map(response.File);
        }

        private T CallReportApi<T>(IApiReportClient apiReportClient, Func<IApiReportClient, Task<T>> action)
        {
            try
            {
                SetToken(apiReportClient);
                return Task
                    .Run(async () => await action.Invoke(apiReportClient))
                    .GetAwaiter()
                    .GetResult();

            }
            catch (ApiException<ErrorResponse> apiException)
            {
                throw HandleApiExeptionWithErrorResponse(apiReportClient, apiException);

            }
            catch (ApiException apiException)
            {
                throw HandleApiExeption(apiReportClient, apiException);
            }
            catch (Exception ex)
            {
                throw HandleExpetion(apiReportClient, ex);
            }
        }

        private static DomainException HandleExpetion(IApiReportClient apiReportClient, Exception ex)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Base Url: ");
            stringBuilder.AppendLine(apiReportClient.BaseUrl);

            stringBuilder.AppendLine(ExceptionFullMessage(ex));
            Leantect.Logging.Log.Error("Error call ERP.Reports.Api", stringBuilder.ToString());
            return new DomainException("Unknown fault in ERP.Reports.Api. Please contact with administrator for review.");
        }

        private static DomainException HandleApiExeption(IApiReportClient apiReportClient, ApiException apiException)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Base Url: ");
            stringBuilder.AppendLine(apiReportClient.BaseUrl);
            stringBuilder.Append("Status Code: ");
            stringBuilder.AppendLine(apiException.StatusCode.ToString());
            stringBuilder.AppendLine(ExceptionFullMessage(apiException));
            Leantect.Logging.Log.Error("Error call ERP.Reports.Api", stringBuilder.ToString());
            switch ((HttpStatusCode)apiException.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    return new DomainException("Invalid Credential for access to ERP.Reports.Api, Please contact with administrator for review.");
                case HttpStatusCode.BadRequest:
                    return new DomainException("Invalid Request for ERP.Reports.Api, Please contact with administrator for review.");
                default:
                    return new DomainException("Unknown fault in ERP.Reports.Api. Please contact with administrator for review.");

            }
        }

        private static string ExceptionFullMessage(Exception ex)
        {
            var sb = new StringBuilder();
            var fullMessage = ex.ToString();

            sb.AppendLine("Error call ERP.Reports.Api");
            var inner = ex;
            while (inner != null)
            {
                sb.AppendLine(inner.Message);
                inner = inner.InnerException;
            }

            sb.AppendLine(fullMessage);
            return sb.ToString();
        }


        private static DomainException HandleApiExeptionWithErrorResponse(IApiReportClient apiReportClient, ApiException<ErrorResponse> apiException)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Base Url: ");
            stringBuilder.AppendLine(apiReportClient.BaseUrl);
            stringBuilder.Append("Status Code: ");
            stringBuilder.AppendLine(apiException.StatusCode.ToString());
            stringBuilder.AppendLine(apiException.Result.Message);
            foreach (var error in apiException.Result.Errors)
            {
                stringBuilder.AppendLine(error.Message);
            };
            stringBuilder.AppendLine(ExceptionFullMessage(apiException));
            Leantect.Logging.Log.Error("Error call ERP.Reports.Api", stringBuilder.ToString());
            switch ((HttpStatusCode)apiException.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    return new DomainException($"Invalid Credential for access to ERP.Reports.Api ({apiReportClient.BaseUrl}), Please contact with administrator for review.");
                case HttpStatusCode.BadRequest:
                    return new DomainException($"Invalid Request for ERP.Reports.Api ({apiReportClient.BaseUrl}), Please contact with administrator for review.");
                default:
                    return new DomainException($"Unknown fault in ERP.Reports.Api '{apiReportClient.BaseUrl}'. Please contact with administrator for review.");

            }
        }

        private void SetToken(IApiReportClient apiReportClient)
        {
            var tokenCache = this.GetFromCache<string>(ERP_REPORTS_API_TOKEN);
            string token = null;
            apiReportClient.SetToken(token);
            if (!tokenCache.Exist || string.IsNullOrEmpty(tokenCache.Value))
            {
                var result = Task
                   .Run(async () => await apiReportClient.User_AuthenticateAsync(new AuthenticateRequestDTO
                   {
                       User = apiReportClient.User,
                       Password = apiReportClient.Password,
                   }))
                   .GetAwaiter()
                   .GetResult();
                this.AddToCache<string>(ERP_REPORTS_API_TOKEN, result.Token, DateTime.Now.AddMinutes(29));
                token = result.Token;
            }
            else if (tokenCache.Exist && !string.IsNullOrEmpty(tokenCache.Value))
                token = tokenCache.Value;
            else
                throw new ArgumentNullException(token);
            apiReportClient.SetToken(token);

        }

        private ReportModel Map(FileModel file) => ReportModel.Create(file);
    }
}

using System.Text.Json.Serialization;

namespace Wissance.WebApiToolkit.Dto
{
    public class OperationResultDto<T>
    {
        public OperationResultDto()
        {
        }

        public OperationResultDto(bool success, int status, string message, T data)
        {
            Success = success;
            Status = status;
            Message = message;
            Data = data;
        }

        public bool Success { get; set; }
        [JsonIgnore]
        public int Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}

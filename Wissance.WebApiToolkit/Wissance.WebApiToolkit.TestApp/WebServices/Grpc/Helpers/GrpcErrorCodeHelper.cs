using Grpc.Core;

namespace Wissance.WebApiToolkit.TestApp.WebServices.Grpc.Helpers
{
    public static class GrpcErrorCodeHelper
    {
        public static Status GetGrpcStatus(int statusCode, string message)
        {
            StatusCode grpcCode = StatusCode.Unknown;
            if (_httpToGrpcCodeMap.ContainsKey(statusCode))
                grpcCode = _httpToGrpcCodeMap[statusCode];
            return new Status(grpcCode, message);
        }

        private static IDictionary<int, StatusCode> _httpToGrpcCodeMap = new Dictionary<int, StatusCode>()
        {
            {200, StatusCode.OK},
            {201, StatusCode.OK},
            {400, StatusCode.InvalidArgument},
            {404, StatusCode.NotFound},
            {500, StatusCode.Internal}
        };
    }
}
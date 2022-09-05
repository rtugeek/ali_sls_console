// See https://aka.ms/new-console-template for more information

using System.Net.NetworkInformation;
using Aliyun.Api.LogService;
using Aliyun.Api.LogService.Domain.Log;
using Aliyun.Api.LogService.Infrastructure.Protocol.Http;
using CommandLine;

namespace AliSls
{
    static class Program
    {
        static async Task Main(String[] args)
        {
            ParserResult<Options> parserResult = Parser.Default.ParseArguments<Options>(args);
            var options = parserResult.Value!;
            // 1. 构建 `ILogServiceClient` 实例：
            var client = LogServiceClientBuilders.HttpBuilder
                .Endpoint(options.Endpoint, options.Project) // 设置「服务入口」和「项目名称」。
                .Credential(options.AccessKeyId, options.AccessKey) // 设置「访问密钥」（RAM模式）
                .Build();

            // 2. 使用 `client` **异步**访问接口（请注意 `await`）：
            LogGroupInfo groupInfo = new LogGroupInfo();
            groupInfo.Topic = options.Topic;
            var logInfo = new LogInfo();
            logInfo.Time = DateTimeOffset.Now;
            logInfo.Contents.Add("content", options.Content);
            logInfo.Contents.Add("platform", options.Platform);
            logInfo.Contents.Add("MAC", DeviceUtil.GetMac());
            logInfo.Contents.Add("machineGUID", DeviceUtil.GetMachineGuid());
            logInfo.Contents.Add("SmBIOS", DeviceUtil.GetSystemId());
            logInfo.Contents.Add("version", options.AppVersion);
            logInfo.Contents.Add("systemVersion", DeviceUtil.GetSystemVersion());
            logInfo.Contents.Add("app", options.AppName);
            groupInfo.Logs.Add(logInfo);
            var response = await client.PostLogStoreLogsAsync(options.Logstore, groupInfo);

            // 3. 从 `response` 中获取业务结果：
            var result = response
                .EnsureSuccess(); // 此方法会确保返回的响应失败时候抛出 `LogServiceException`。
            Console.Write(result.IsSuccess);
        }
    }
}
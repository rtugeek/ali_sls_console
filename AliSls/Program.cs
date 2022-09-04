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
            var SmBIOS = DeviceUtil.GetSystemId();
            var mac = DeviceUtil.GetMac();
            var machineGUID = DeviceUtil.GetMachineGuid();
            Console.WriteLine(machineGUID);
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
            logInfo.Contents.Add("MAC", mac);
            logInfo.Contents.Add("machineGUID", machineGUID);
            logInfo.Contents.Add("SmBIOS", SmBIOS);
            logInfo.Contents.Add("version", options.AppVersion);
            logInfo.Contents.Add("app", options.AppName);
            groupInfo.Logs.Add(logInfo);
            var response = await client.PostLogStoreLogsAsync(options.Logstore, groupInfo);

            // 3. 从 `response` 中获取业务结果：
            var result = response
                .EnsureSuccess(); // 此方法会确保返回的响应失败时候抛出 `LogServiceException`。
            Console.WriteLine(result.IsSuccess);
            var resultStr = await result.GetHttpResponseMessage().Content.ReadAsStringAsync();
            Console.WriteLine(resultStr);
        }
    }

    public class Options
    {
        [Option('p', "project", Required = false, Default = "haihaihai", HelpText = "Project name")]
        public string Project { get; set; }

        [Option('l', "logstore", Required = true, HelpText = "Log store name")]
        public string Logstore { get; set; }

        [Option('i', "key-id", Required = true, HelpText = "accessKeyId")]
        public string AccessKeyId { get; set; }

        [Option('k', "key", Required = true, HelpText = "accessKeyId")]
        public string AccessKey { get; set; }

        [Option('c', "content", Required = false, Default = "", HelpText = "Content")]
        public string Content { get; set; }

        [Option('p', "platform", Required = false, Default = "windows", HelpText = "Platforms: windows macOS etc.")]
        public string Platform { get; set; }

        [Option('v', "app-version", Required = true, HelpText = "Application version")]
        public string AppVersion { get; set; }

        [Option('n', "app-name", Required = true, HelpText = "Application name")]
        public string AppName { get; set; }

        [Option('e', "endpoint", Required = false, Default = "cn-guangzhou.log.aliyuncs.com",
            HelpText = "Application version")]
        public string Endpoint { get; set; }

        [Option('t', "topic", Required = false, Default = "", HelpText = "Topic of this log")]
        public string Topic { get; set; }
    }
}
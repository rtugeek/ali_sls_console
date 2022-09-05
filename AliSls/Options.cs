using CommandLine;

namespace AliSls;

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
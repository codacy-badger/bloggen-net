using System;
using System.IO;
using CommandLine;
using Microsoft.Extensions.Configuration;

namespace Bloggen.Net
{
    class Program
    {
        public class CommandLineOptions
        {
            [Option('s', "source", Required = true, HelpText = "Source directory")]
            public string SourceDirectory { get; set; } = string.Empty;

            [Option('o', "output", Required = true, HelpText = "Output directory")]
            public string OutputDirectory { get; set; } = string.Empty;
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed<CommandLineOptions>(Build)
                .WithNotParsed(errors =>
                {
                    Environment.Exit(1);
                });
        }

        static void Build(CommandLineOptions options)
        {
            var siteConfiguration = new ConfigurationBuilder()
                .AddYamlFile(Path.Combine(options.SourceDirectory, "config.yml"), false)
                .Build();

            var serviceProvider = ServiceConfiguration.ConfigureServiceProvider(siteConfiguration);
        }
    }
}
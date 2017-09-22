﻿using System;
using Microsoft.Extensions.CommandLineUtils;

namespace Mvc.KodKod.Tool
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.ReadLine();
            var app = new CommandLineApplication();
            var appName = app.Option("--application-name", "Application name", CommandOptionType.SingleValue);
            var outputPath = app.Option("--output-path", "Output path", CommandOptionType.SingleValue);
            var applicationBaseUrl = app.Option("--base-url", "The url application is hosted at.", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                try
                {
                    var options = new KodKodOptions
                    {
                        ApplicationName = appName.Value(),
                        OutputPath = outputPath.Value(),
                        BaseUrl = applicationBaseUrl.Value(),
                    };

                    new KodKod { Options = options }.Execute();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[error]: {ex}");
                    return 1;
                }

                return 0;
            });

            app.Execute(args);
        }
    }
}
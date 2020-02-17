using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using Bloggen.Net.Model;
using Bloggen.Net.Output.Implementation;
using Bloggen.Net.Template;

namespace Bloggen.Net.Output
{
    public class FileSystemOutputHandler : IOutputHandler
    {
        public const string POSTS_DIRECTORY = "posts";

        public const string TAGS_DIRECTORY = "tags";

        private const string EXTENSION = "html";

        private readonly string outputDirectory;

        private readonly IFileSystem fileSystem;

        private readonly IContext<Post, Tag> context;

        private readonly ITemplateHandler templateHandler;

        public FileSystemOutputHandler(
            CommandLineOptions commandLineOptions,
            IFileSystem fileSystem,
            IContext<Post, Tag> context,
            ITemplateHandler templateHandler) =>
            (this.outputDirectory, this.fileSystem, this.context, this.templateHandler) =
            (commandLineOptions.OutputDirectory, fileSystem, context, templateHandler);

        public void Generate()
        {
            this.ClearOutput();

            this.Generate(POSTS_DIRECTORY, this.context.Posts, p => p.FileName, "post");

            this.Generate(TAGS_DIRECTORY, this.context.Tags, t => t.Name, "tag");
        }

        private void ClearOutput()
        {
            if (this.fileSystem.Directory.Exists(this.outputDirectory))
            {
                this.fileSystem.Directory.Delete(this.outputDirectory, true);
            }

            this.fileSystem.Directory.CreateDirectory(this.outputDirectory);
        }

        private void Generate<T>(string directory, IEnumerable<T> items, Func<T, string> nameSelector, string layout) where T : class
        {
            var path = this.fileSystem.Path.Combine(this.outputDirectory, directory);

            this.fileSystem.Directory.CreateDirectory(path);

            foreach(var item in items)
            {
                using var sw = this.fileSystem.File.CreateText(
                    $"{this.fileSystem.Path.Combine(path, nameSelector(item))}.{EXTENSION}"
                );

                this.templateHandler.Write(sw, layout, item);
            }
        }
    }
}
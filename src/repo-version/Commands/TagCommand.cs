using System;
using LibGit2Sharp;

namespace repo_version.Commands
{
    public class TagCommand : ICommand
    {
        private readonly IVersionCalculator _calculator;
        private readonly IGitFolderFinder _folderFinder;

        public TagCommand(IVersionCalculator calculator, IGitFolderFinder folderFinder)
        {
            _calculator = calculator;
            _folderFinder = folderFinder;
        }

        public int Execute(Options options)
        {
            var version = _calculator.CalculateVersion(options);

            if (version == null)
            {
                return 1;
            }

            if (version.IsDirty)
            {
                Console.WriteLine("Cannot apply tag with uncommitted changes");
                return 1;
            }

            var gitFolder = _folderFinder.FindGitFolder(options.Path);

            using (var repo = new Repository(gitFolder))
            {
                repo.ApplyTag(version.SemVer);
            }

            Console.WriteLine($"Created Tag: {version.SemVer}");

            return 0;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.RemoveDirectory
{
  public class RemoveDirectoryCommandLine : CommandLine
  {
    #region Fields

    private RemoveDirectoryCommandLineOptions options;

    #endregion

    #region Constructors

    public RemoveDirectoryCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = RemoveDirectoryOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, RemoveDirectoryOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, RemoveDirectoryOptions.Version);
      }
      else
      {
        StartRemoveDirectory();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartRemoveDirectory()
    {
      try
      {
        string path = WildcardCharacterHelper.TranslateWildcardDirectoryPath(options.Directory);
        SearchDirectory(path);
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Gimela.Toolkit.CommandLines.RemoveDirectory.RemoveDirectoryCommandLine.OutputText(System.String)")]
    private void SearchDirectory(string path)
    {
      DirectoryInfo directory = new DirectoryInfo(path);
      if (!directory.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such directory -- {0}", directory.FullName));
      }
      else
      {
        DirectoryInfo[] directories = directory.GetDirectories();
        foreach (var item in directories)
        {
          if (IsMatchRegexPattern(item.Name))
          {
            try
            {
              item.Delete(true);
              OutputText(string.Format(CultureInfo.CurrentCulture, "Removed - {0}", item.FullName));
            }
            catch (UnauthorizedAccessException ex)
            {
              throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                "Operation exception -- {0}, {1}", item.FullName, ex.Message));
            }
            catch (DirectoryNotFoundException ex)
            {
              throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                "Operation exception -- {0}, {1}", item.FullName, ex.Message));
            }
            catch (SecurityException ex)
            {
              throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                "Operation exception -- {0}, {1}", item.FullName, ex.Message));
            }
            catch (IOException ex)
            {
              throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                "Operation exception -- {0}, {1}", item.FullName, ex.Message));
            }
          }
          else
          {
            if (options.IsSetRecursive)
            {
              SearchDirectory(item.FullName);
            }
          }
        }
      }
    }

    private bool IsMatchRegexPattern(string name)
    {
      bool result = false;

      if (name == options.RegexPattern)
      {
        result = true;
      }

      return result;
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static RemoveDirectoryCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      RemoveDirectoryCommandLineOptions targetOptions = new RemoveDirectoryCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          RemoveDirectoryOptionType optionType = RemoveDirectoryOptions.GetOptionType(arg);
          if (optionType == RemoveDirectoryOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case RemoveDirectoryOptionType.Directory:
              targetOptions.Directory = commandLineOptions.Arguments[arg];
              break;
            case RemoveDirectoryOptionType.Recursive:
              targetOptions.IsSetRecursive = true;
              break;
            case RemoveDirectoryOptionType.RegexPattern:
              targetOptions.RegexPattern = commandLineOptions.Arguments[arg];
              break;
            case RemoveDirectoryOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case RemoveDirectoryOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(RemoveDirectoryCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        if (string.IsNullOrEmpty(checkedOptions.Directory))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a directory."));
        }
        if (string.IsNullOrEmpty(checkedOptions.RegexPattern))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a pattern for matching."));
        }
      }
    }

    #endregion
  }
}
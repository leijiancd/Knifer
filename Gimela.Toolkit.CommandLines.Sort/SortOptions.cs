﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Gimela.Toolkit.CommandLines.Sort
{
	internal static class SortOptions
	{
		public static readonly ReadOnlyCollection<string> InputFileOptions;
		public static readonly ReadOnlyCollection<string> OutputFileOptions;
		public static readonly ReadOnlyCollection<string> ReverseOrderOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<SortOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static SortOptions()
		{
			InputFileOptions = new ReadOnlyCollection<string>(new string[] { "f", "file" });
			OutputFileOptions = new ReadOnlyCollection<string>(new string[] { "o", "output" });
			ReverseOrderOptions = new ReadOnlyCollection<string>(new string[] { "r", "reverse" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<SortOptionType, ICollection<string>>();
			Options.Add(SortOptionType.InputFile, InputFileOptions);
			Options.Add(SortOptionType.OutputFile, OutputFileOptions);
			Options.Add(SortOptionType.ReverseOrder, ReverseOrderOptions);
			Options.Add(SortOptionType.Help, HelpOptions);
			Options.Add(SortOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(SortOptions.ReverseOrderOptions);
			singleOptionList.AddRange(SortOptions.HelpOptions);
			singleOptionList.AddRange(SortOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public const string Version = @"Sort v1.0";
		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	sort - sort lines of a text file

SYNOPSIS

	sort [OPTION] FILE

DESCRIPTION

	The Sort utility program sorts the lines in a text file.

OPTIONS

	-f, --file=FILE
	{0}{0}The FILE that needs to be sorted.
	-o, --output=FILE
	{0}{0}The FILE represents the output file.
	-r, --reverse
	{0}{0}Sorts in reverse order.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	sort -r file.txt
	Sort the file 'file.txt' in reverse order.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static SortOptionType GetOptionType(string option)
		{
			SortOptionType optionType = SortOptionType.None;

			foreach (var pair in Options)
			{
				foreach (var item in pair.Value)
				{
					if (item == option)
					{
						optionType = pair.Key;
						break;
					}
				}
			}

			return optionType;
		}
	}
}
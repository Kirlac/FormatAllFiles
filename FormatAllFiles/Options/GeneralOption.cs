﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using FormatAllFiles.Text;

namespace FormatAllFiles.Options
{
    /// <summary>
    /// 全般設定のオプションです。
    /// </summary>
    public class GeneralOption
    {
        /// <summary>
        /// ドキュメントをフォーマットするコマンドです。
        /// </summary>
        private const string FORMAT_DOCUMENT_COMMAND = "Edit.FormatDocument";

        /// <summary>
        /// using を整理、削除および並び替えるコマンドです。
        /// </summary>
        private const string REMOVE_AND_SORT_COMMAND = "Edit.RemoveAndSort";

        /// <summary>
        /// 各ファイルをフォーマットするかどうかを取得または設定します。
        /// </summary>
        [Category("Command")]
        [DisplayName("Enable Format Document")]
        [Description("When true, each files will be formatted. This command is 'Edit.FormatDocument'.")]
        public bool EnableFormatDocument { get; set; }

        /// <summary>
        /// 各ファイルの using を整理、削除および並び替えるかどうかを取得または設定します。
        /// </summary>
        [Category("Command")]
        [DisplayName("Enable Remove and Sort Usings")]
        [Description("When true, each files will be removed and sorted usings. This command is 'Edit.RemoveAndSort'.")]
        public bool EnableRemoveAndSortUsing { get; set; }

        /// <summary>
        /// T4で作成されたファイルを対象から除外するかどうかを取得または設定します。
        /// </summary>
        [Category("Target File")]
        [DisplayName("Exclude Generated T4")]
        [Description("Exclude target files generated by T4 text template.")]
        public bool ExcludeGeneratedT4 { get; set; }

        /// <summary>
        /// 対象とするファイルを除外するワイルドカードのパターンです。
        /// </summary>
        [Category("Target File")]
        [DisplayName("Exclusion Pattern")]
        [Description("This is a pattern to search exclusion files. You can also use wildcards. This is given priority over Inclusion Pattern.")]
        public string ExclusionFilePattern { get; set; }

        /// <summary>
        /// 対象とするファイルを検索するワイルドカードのパターンです。
        /// </summary>
        [Category("Target File")]
        [DisplayName("Inclusion Pattern")]
        [Description("This is a pattern to search inclusion files. You can use wildcards like '*.cs\' ('?' matches any character, '*' matches any number of characters.), and use multi paterns with the delimiter(;). When this is empty, all files apply.")]
        public string InclusionFilePattern { get; set; }

        /// <summary>
        /// 各ファイルに実行するコマンドを取得または設定します。
        /// </summary>
        [Category("Command")]
        [DisplayName("Other Execution Command")]
        [Description("A command to execute each files.")]
        public string OtherCommand { get; set; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public GeneralOption()
        {
            EnableFormatDocument = true;

            InclusionFilePattern = "*.*";
            ExcludeGeneratedT4 = true;
        }

        /// <summary>
        /// 対象ファイルを名前で絞り込むフィルターを作成します。
        /// </summary>
        /// <returns>ファイル名で絞り込む処理</returns>
        public Func<string, bool> CreateFileFilter()
        {
            Func<string, bool> inclusionFilter;
            if (string.IsNullOrWhiteSpace(InclusionFilePattern))
            {
                inclusionFilter = name => true;
            }
            else
            {
                var wildCard = new WildCard(InclusionFilePattern, WildCardOptions.MultiPattern);
                inclusionFilter = wildCard.IsMatch;

            }

            Func<string, bool> exclusionFilter;
            if (string.IsNullOrWhiteSpace(ExclusionFilePattern))
            {
                exclusionFilter = name => false;
            }
            else
            {
                var wildCard = new WildCard(ExclusionFilePattern, WildCardOptions.MultiPattern);
                exclusionFilter = wildCard.IsMatch;
            }

            return name => exclusionFilter(name) == false && inclusionFilter(name);
        }

        /// <summary>
        /// 対象ファイルを階層のパスで絞り込むフィルターを作成します。
        /// </summary>
        /// <returns>階層のパスで絞り込む処理</returns>
        public Func<string, bool> CreateHierarchyFilter()
        {
            if (ExcludeGeneratedT4)
            {
                return path => Path.GetExtension(path) != ".tt";
            }
            else
            {
                return path => true;
            }
        }

        /// <summary>
        /// 各ファイルに対して実行するコマンドの一覧を取得します。
        /// </summary>
        public IList<string> GetCommands()
        {
            var result = new List<string>();

            if (EnableFormatDocument)
            {
                result.Add(FORMAT_DOCUMENT_COMMAND);
            }

            if (EnableRemoveAndSortUsing)
            {
                result.Add(REMOVE_AND_SORT_COMMAND);
            }

            if (string.IsNullOrWhiteSpace(OtherCommand) == false)
            {
                result.Add(OtherCommand);
            }

            return result;
        }
    }
}

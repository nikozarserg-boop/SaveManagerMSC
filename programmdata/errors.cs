using System;
using System.Collections.Generic;

namespace SaveManagerMSC
{
    public enum ErrCode
    {
        SOURCE_UNAVAILABLE = 1,
        TARGET_UNAVAILABLE = 2,
        NO_SAVE_SELECTED = 3,
        SAVE_NOT_FOUND = 4,
        CREATE_SAVE_FAILED = 5,
        DELETE_SAVE_FAILED = 6,
        RESTORE_SAVE_FAILED = 7,
        DISK_FULL = 8,
        HASH_MISMATCH = 9,
        METADATA_NOT_FOUND = 10,
        ORIGINAL_PATH_MISSING = 11,
        LOG_NOT_FOUND = 12,
        README_NOT_FOUND = 13,
        CONFIG_ERROR = 14,
        MUSIC_INIT_FAILED = 15,
        FOLDER_INIT_FAILED = 16,
        AUTO_DETECT_FAILED = 17,
        UNHANDLED_EXCEPTION = 18,
        SHOW_DETAILS_FAILED = 19,
        POPULATE_FAILED = 20,
        OPEN_LOG_FAILED = 21,
        LOCALE_LOAD_FAILED = 22
    }

    public static class Errors
    {
        private static readonly Dictionary<ErrCode, string> CodeNames = new()
        {
            { ErrCode.SOURCE_UNAVAILABLE, "SOURCE_UNAVAILABLE" },
            { ErrCode.TARGET_UNAVAILABLE, "TARGET_UNAVAILABLE" },
            { ErrCode.NO_SAVE_SELECTED, "NO_SAVE_SELECTED" },
            { ErrCode.SAVE_NOT_FOUND, "SAVE_NOT_FOUND" },
            { ErrCode.CREATE_SAVE_FAILED, "CREATE_SAVE_FAILED" },
            { ErrCode.DELETE_SAVE_FAILED, "DELETE_SAVE_FAILED" },
            { ErrCode.RESTORE_SAVE_FAILED, "RESTORE_SAVE_FAILED" },
            { ErrCode.DISK_FULL, "DISK_FULL" },
            { ErrCode.HASH_MISMATCH, "HASH_MISMATCH" },
            { ErrCode.METADATA_NOT_FOUND, "METADATA_NOT_FOUND" },
            { ErrCode.ORIGINAL_PATH_MISSING, "ORIGINAL_PATH_MISSING" },
            { ErrCode.LOG_NOT_FOUND, "LOG_NOT_FOUND" },
            { ErrCode.README_NOT_FOUND, "README_NOT_FOUND" },
            { ErrCode.CONFIG_ERROR, "CONFIG_ERROR" },
            { ErrCode.MUSIC_INIT_FAILED, "MUSIC_INIT_FAILED" },
            { ErrCode.FOLDER_INIT_FAILED, "FOLDER_INIT_FAILED" },
            { ErrCode.AUTO_DETECT_FAILED, "AUTO_DETECT_FAILED" },
            { ErrCode.UNHANDLED_EXCEPTION, "UNHANDLED_EXCEPTION" },
            { ErrCode.SHOW_DETAILS_FAILED, "SHOW_DETAILS_FAILED" },
            { ErrCode.POPULATE_FAILED, "POPULATE_FAILED" },
            { ErrCode.OPEN_LOG_FAILED, "OPEN_LOG_FAILED" },
            { ErrCode.LOCALE_LOAD_FAILED, "LOCALE_LOAD_FAILED" }
        };

        public static string Code(ErrCode code)
        {
            return $"ERR_{code:D3}";
        }

        public static string Tag(ErrCode code)
        {
            return $"[{Code(code)}]";
        }

        public static string FullTag(ErrCode code)
        {
            return $"[{Code(code)} {CodeNames[code]}]";
        }

        public static string LogMessage(ErrCode code, string detail)
        {
            return $"{FullTag(code)} {detail}";
        }

        public static string MessageBoxText(ErrCode code, string detail)
        {
            return $"{Code(code)}: {detail}";
        }

        public static string GetDescription(ErrCode code, Dictionary<string, string> texts)
        {
            string key = $"err_{code:D3}_desc";
            return texts.GetValueOrDefault(key, CodeNames.GetValueOrDefault(code, "Unknown error"));
        }

        public static string GetName(ErrCode code)
        {
            return CodeNames.GetValueOrDefault(code, "UNKNOWN");
        }
    }
}

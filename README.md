SaveManagerMSC v1.0
утилита для создания и восстановления сохранений игры (версия 1.0).

Инструкции по использованию
-------------------------------
1. Установите приложение (по умолчанию в C:\Program Files (x86)\SaveManagerMSC).
2. Запустите SaveManagerMSC.exe (права администратора не требуются, но могут понадобиться для записи в Program Files).
3. Укажите исходную папку игры и папку для сохранений (по умолчанию: C:\Program Files (x86)\SaveManagerMSC\Saves).
4. Создавайте, удаляйте и восстанавливайте сохранения с помощью соответствующих кнопок.
5. Логи сохраняются в %AppData%\SaveManagerMSC\programmLog.txt

Коды ошибок
---------------
| Код | Имя | Описание |
|-----|-----|----------|
| ERR_001 | SOURCE_UNAVAILABLE | Исходная папка недоступна или указан неверный путь. |
| ERR_002 | TARGET_UNAVAILABLE | Целевая папка недоступна или указан неверный путь. |
| ERR_003 | NO_SAVE_SELECTED | Ни одно сохранение не выбрано в списке. |
| ERR_004 | SAVE_NOT_FOUND | Запрашиваемое сохранение не найдено на диске. |
| ERR_005 | CREATE_SAVE_FAILED | Не удалось создать сохранение. |
| ERR_006 | DELETE_SAVE_FAILED | Не удалось удалить сохранение. |
| ERR_007 | RESTORE_SAVE_FAILED | Не удалось восстановить сохранение. |
| ERR_008 | DISK_FULL | Недостаточно свободного места на диске. |
| ERR_009 | HASH_MISMATCH | Несоответствие хэшей файлов (сохранение может быть повреждено). |
| ERR_010 | METADATA_NOT_FOUND | Файл метаданных для выбранного сохранения не найден. |
| ERR_011 | ORIGINAL_PATH_MISSING | В метаданных отсутствует путь к исходной папке. |
| ERR_012 | LOG_NOT_FOUND | Файл лога не найден. |
| ERR_013 | README_NOT_FOUND | Файл ReadMe не найден. |
| ERR_014 | CONFIG_ERROR | Ошибка чтения/записи файла конфигурации. |
| ERR_015 | MUSIC_INIT_FAILED | Ошибка инициализации фоновой музыки. |
| ERR_016 | FOLDER_INIT_FAILED | Ошибка создания стандартных папок приложения. |
| ERR_017 | AUTO_DETECT_FAILED | Ошибка авто-определения папки игры. |
| ERR_018 | UNHANDLED_EXCEPTION | Необработанная ошибка (смотрите лог). |
| ERR_019 | SHOW_DETAILS_FAILED | Ошибка отображения деталей сохранения. |
| ERR_020 | POPULATE_FAILED | Ошибка загрузки списка сохранений. |
| ERR_021 | OPEN_LOG_FAILED | Ошибка открытия файла лога. |
| ERR_022 | LOCALE_LOAD_FAILED | Ошибка загрузки языковых файлов. |

Контакты авторов
--------------------
Telegram: @Ssshhhiiitttzcx
Telegram: @pinkcvrse

----------------------------------------

SaveManagerMSC v1.0
utility for creating and restoring game saves (version 1.0).

Usage Instructions
-------------------------------
1. Install the application (default: C:\Program Files (x86)\SaveManagerMSC).
2. Run SaveManagerMSC.exe (administrator rights are not required but may be needed to write into Program Files).
3. Specify the game's source folder and the saves folder (default: C:\Program Files (x86)\SaveManagerMSC\Saves).
4. Create, delete and restore saves using the corresponding buttons.
5. Logs are stored in %AppData%\SaveManagerMSC\programmLog.txt
6. Choose your preferred language from the Language dropdown (en / ru / uk / es / fi).

Error codes
---------------
| Code | Name | Description |
|------|------|-------------|
| ERR_001 | SOURCE_UNAVAILABLE | Source folder is unavailable or the path is invalid. |
| ERR_002 | TARGET_UNAVAILABLE | Target folder is unavailable or the path is invalid. |
| ERR_003 | NO_SAVE_SELECTED | No save is selected in the list. |
| ERR_004 | SAVE_NOT_FOUND | The requested save was not found on disk. |
| ERR_005 | CREATE_SAVE_FAILED | Failed to create the save. |
| ERR_006 | DELETE_SAVE_FAILED | Failed to delete the save. |
| ERR_007 | RESTORE_SAVE_FAILED | Failed to restore the save. |
| ERR_008 | DISK_FULL | Not enough free disk space to complete the operation. |
| ERR_009 | HASH_MISMATCH | File hash mismatch (the save may be corrupted). |
| ERR_010 | METADATA_NOT_FOUND | Metadata file for the selected save was not found. |
| ERR_011 | ORIGINAL_PATH_MISSING | The original source path is missing in the metadata. |
| ERR_012 | LOG_NOT_FOUND | The log file was not found. |
| ERR_013 | README_NOT_FOUND | The ReadMe file was not found. |
| ERR_014 | CONFIG_ERROR | Failed to read or write the configuration file. |
| ERR_015 | MUSIC_INIT_FAILED | Failed to initialize background music. |
| ERR_016 | FOLDER_INIT_FAILED | Failed to create the application data folders. |
| ERR_017 | AUTO_DETECT_FAILED | Failed to auto-detect the game source folder. |
| ERR_018 | UNHANDLED_EXCEPTION | An unhandled exception occurred (see the log). |
| ERR_019 | SHOW_DETAILS_FAILED | Failed to display save details. |
| ERR_020 | POPULATE_FAILED | Failed to populate the saves list. |
| ERR_021 | OPEN_LOG_FAILED | Failed to open the log file. |
| ERR_022 | LOCALE_LOAD_FAILED | Failed to load language locale files. |

Authors
--------------------
Telegram: @Ssshhhiiitttzcx
Telegram: @pinkcvrse

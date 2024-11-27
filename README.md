# fnorm - FixNormalization
Fix Unicode normalization in filenames.

## Introduction
fnorm(EEF-NOHRM) is a simple desktop application to fix Unicode normalization in filenames. From Form D to Form C, and vice versa.

It is especially useful when you met nasty messed filename including characters which can be decomposed after moving files from macOS (or Darwin based-OSes) to Windows or Linux.
(e.g. 한글 -> ᄒ​ᅡ​ᆫᄀ​ᅳ​ᆯ). In particular in Hangul, it has been considered really notorious problem among people who need to communicate files with different OSes.

This problem is derived from discrepancy in processing Unicode characters between Darwin (**macOS**, iPadOS, iOS, tvOS, watchOS, visionOS) and others, because HFS+, a filesystem that once Apple had been applied in their OSes, uses Normalization D to save filenames in UTF-16.
Although APFS(Apple File System), a newer file system recognizes and preserves normalization types, but files or directories created in those environments still have Form-D typed filename. This is why the problem persists...

Other characters, such as diacritics, they seem to be shown correctly with subtly behavior in Windows anyway, but Hangul characters are NOT at all. Read [an article](https://devblogs.microsoft.com/oldnewthing/20201009-00/?p=104351) regarding this issue in detail.

Although this is a file system problem and limited to only filename, so manual renaming is a solution, but it is really time-consuming when you have a lot of files.

That's why I made this humble tool.

## Installation

```fnorm``` supports Windows, and Linux with x64 and ARM64 architectures. macOS will be supported soon.

### Requirements
* [.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) Runtime

You can install ```fnorm``` in various ways.

Visit [the release page](https://github.com/Capella87/FixNormalization/releases) and download the latest version of fnorm. See assets.
Or you can use a package manager shown below.

### Scoop

You can easily install fnorm in Windows with [Scoop](https://scoop.sh) and my bucket, [capella-bucket](https://github.com/Capella87/capella-bucket).
Make sure that you must add capella-bucket to local scoop bucket list.

```shell
scoop add capella-bucket https://github.com/Capella87/capella-bucket
```

Then, 

```shell
scoop install capella-bucket/fnorm
```


## Usage

You can use fnorm easily with commands:

```shell
fnorm fix [paths] --form [form]
```

### Paths
Supports both relative path and absolute path. You can specify multiple paths at once.

It is not yet supported to remove redundant paths. For example, If there are a file path and its parent directory in order, fnorm will try to process the individual file and files under the directory.

But you don't have to worry about, if the file is already normalized, it will be ignored at processing files in the folder.

It is still in development that fnorm will support recursive search in directories.


### ```--form```

```nfc``` or ```formc``` **[Default]**

Convert filenames to follow Normalization Form C. If you don't specify ```--form``` option, it will be automatically set to ```nfc```.

```nfd``` or ```formd```

Convert filenames to follow Normalization Form D. In general cases you don't have to care about this option.

### Example

```shell
fnorm fix C:\\Users\user\Desktop C:\\Users\user\Documents --form nfc
```

## Disclaimer
This software is distributed under MIT License. I don't have any responsibility for any damage caused by using this software.

---

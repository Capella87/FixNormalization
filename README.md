# fnorm - FixNormalization
Fix Unicode normalization of filename.

> [!IMPORTANT]
> This software is in development. It may contains unknown or potential bugs and unexpected behaviors. If you find or encounter those, Open an issue at this repository and describe it.

## Introduction
**fnorm**(EEF-NOHRM) is a simple desktop application to fix Unicode normalization in filenames. From Form D to Form C, and vice versa.

It is especially useful when you met <ins>a nasty messed filename</ins> including characters which can be decomposed after moving files from macOS (or Darwin based-OSes) to Windows or Linux.
(e.g. 한글 -> ᄒ​ᅡ​ᆫᄀ​ᅳ​ᆯ). In particular in Hangul, it has been considered really notorious problem among people who need to communicate files with different OSes.

This problem is derived from discrepancy in processing Unicode characters between Darwin (**macOS**, iPadOS, iOS, tvOS, watchOS and visionOS) and others, because HFS+, a filesystem that Apple had conducted in their OSes, uses Normalization D to save filename in UTF-16.
Although APFS(Apple File System), a newer file system can recognize and keep filename's normalization types, but files or directories created in those environments still have Form D filename. This is why the problem persists...

Other characters, such as diacritics, they seem to be shown correctly with subtly behavior in Windows anyway, but Hangul characters are NOT at all. Read [an article](https://devblogs.microsoft.com/oldnewthing/20201009-00/?p=104351) regarding this issue in detail.

Although this is a file system problem and limited to only filename, so manual renaming is a solution, but it is really time-consuming when you have a lot of files.

That's why I made this simple tool with .NET.

## Installation

```fnorm``` supports Windows, and Linux with x64 and ARM64 architectures. macOS will be supported soon.
You can install ```fnorm``` in various ways.

Visit [the release page](https://github.com/Capella87/FixNormalization/releases) and download the latest version of fnorm. See assets.
Or you can use a package manager shown below.

### Requirements
* **[.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)** Runtime [Mandatory]

Although the program is automatically setting console output to UTF-8, you may have to enable Windows option, '**Beta: Use Unicode UTF-8 for worldwide language support**' (Beta: 세계 언어 지원을 위해 Unicode UTF-8 사용) in Region settings to see Unicode characters correctly in Windows console environment.
That's because Windows still uses MBCS(Multi-Byte Character Set) as a default encoding in system for legacy, (e.g. CP949 for Korean) and it may causes problem in processing and showing latest Unicode characters right.

But this option can be problematic in some programs which forces or are sticked to specific encoding. (e.g. MapleStory, Fallout: New Vegas)
You can also make a workaround by type ```chcp 65001``` to set UTF-8 encoding in the console section prior to executing this program.

I recommend to run this program in Windows Terminal with PowerShell 7.

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
Windows and Linux normally use this form.

```nfd``` or ```formd```

Convert filenames to follow Normalization Form D. In general cases you don't have to care about this option.
macOS or any Apple's operating systems are using this form.

### ```--recursive``` or ```-r```

Enable recursive search in directories.

### Example

```shell
fnorm fix C:\\Users\user\Desktop C:\\Users\user\Documents --form nfc
```

## Disclaimer
This software is distributed under MIT License. I don't have any responsibility for any damage caused by using this software.

---

Copyright © 2024-2026 **Capella87**. Distributed under MIT License.

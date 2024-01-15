# Palmtree.IO.Compression.Stream.Plugin.SevenZip.Deflate

## 1. Overview

<!--
このパッケージは、パッケージ `Palmtree.IO.Compression.Archive.Zip`で Deflate 圧縮方式をサポートするためのプラグインライブラリのパッケージです。
-->
This package is a plugin library package to support the Deflate compression method in the package `Palmtree.IO.Compression.Archive.Zip`.

## 2. Required environment


| Item | Condition |
| --- | --- |
| CPU | x64 / x86 |
| OS | Windows / Linux |
| .NET rumtime | 7.0 / 8.0 |
| 7-zip | Confirmed to work with 7-zip 23.01 |

<!--
別途、 7-zip のインストールが必要です。
詳細については パッケージ "SevenZip.Compression.Wrapper.NET" のドキュメントを参照してください。
-->
7-zip must be installed separately.
See [documentation for package `SevenZip.Compression.Wrapper.NET`](https://github.com/rougemeilland/SevenZip.Compression.Wrapper.NET/blob/main/docs/HowToInstall7z_en.md) for more information.

## 3. Usage

<!--
Deflate 圧縮方式を有効にするためには、アプリケーションプログラムで最初に以下のステートメントを実行してください。
-->
To enable the Deflate compression method, first execute the following statement in your application program.

```csharp
 Palmtree.IO.Compression.Stream.Plugin.SevenZip.DeflateCoderPlugin.EnablePlugin();
```

## 4. License
The source code of this software is covered by the MIT License.

## 5. Disclaimer
The developer of this software is not responsible for any defects or troubles that may occur when using this software. Please understand that.

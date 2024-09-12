<h1 align="center">
    Booski
</h1>

<p align="center">
    <strong>Booski</strong> is a Bluesky cross-poster for inferior services<br /><em>(and a .NET library for <a href="https://atproto.com/">ATProto</a> and <a href="https://bsky.social/">Bluesky</a>)</em>
</p>

<p align="center">
    <strong>
        <a href="https://github.com/electricduck/booski/releases/latest">⬇️ Download</a> &nbsp;|&nbsp;
        <a href="https://github.com/electricduck/booski/wiki/Getting-Started">✨ Getting Started</a> &nbsp;|&nbsp;
        <a href="https://github.com/electricduck/booski/wiki">📖 Docs</a> &nbsp;|&nbsp;
        <a href="https://github.com/electricduck/booski/issues/new">💣 Submit Issue</a>
    </strong>
</p>

<hr />

## ✨ Quick Start

>  This Quick Start makes general assumptions about your environment, such as using Linux (x86_64). For a more comprehensive guide, see [Getting Started](https://github.com/electricduck/booski/wiki/Getting-Started).

```sh
cd ~/.local/bin
wget https://github.com/electricduck/booski/releases/download/v%2F0.x%2F0.4-rc1/booski-0.4-rc1-linux-x64.bin
chmod +x booski-0.4-rc1-linux-x64.bin
./booski-0.4-rc1-linux-x64.bin start --no-daemon
```

## 🏗️ Building

This repository currently houses two tangenically related projects (under `./src`): the Bluesky cross-poster (`Booski`), and a general-purpose Bluesky library (`Booski.Lib`).

> For some background, Booski began life as a Bluesky library. Shortly into development, these plans folded as maitaining a library (especially for an ever-evolving unstable API) is a hard task at scale and [another, more feature-complete, Bluesky library already exists](https://github.com/drasticactions/FishyFlip). Instead of ditching weeks-worth of work that created the library (it's some of my best work, if I do say so myself) it was decided to utilize it for the new cross-poster, which itself became **Booski**, and the library became **Booski.Lib**.
>
> **Booski.Lib** is still being developed, as its used in other various personal projects, but it will never be considered truely stable or feature-complete. However, you are welcome to use it in your own projects (sorry, there's no NuGet package) and [submit tickets]([https://github.com/electricduck/booski/labels/Lib](https://github.com/electricduck/booski/issues/new)) (tagging with [Lib](https://github.com/electricduck/booski/labels/Lib)).

### Setting Up

#### Prerequisites

* **[.NET](https://dotnet.microsoft.com/)**
   * [Version 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) (and above) required. Due to [dotnet/runtime#72604](https://github.com/dotnet/runtime/issues/72604#issuecomment-1440708052), older versions are not supported.
* **Linux**, **Windows**, or **macOS**
* **Bash**
   * Scripts under `tools/` require this shell. There are currently no native scripts for Windows.

#### Fetching the Repository

```sh
git clone https://github.com/electricduck/booski.git
cd booski
```

### Building

_(Todo)_

### Using

#### _Booski_

_(Todo)_

#### _Booski.Lib_

_(Todo)_

<!--
## 🤝 Acknowledgements

_(Todo)_
-->

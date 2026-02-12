# VectraLauncher
![GitHub Release](https://img.shields.io/github/v/release/DuncanMcPherson/VectraLauncher)
[![Release](https://github.com/DuncanMcPherson/VectraLauncher/actions/workflows/release.yml/badge.svg)](https://github.com/DuncanMcPherson/VectraLauncher/actions/workflows/release.yml)
![GitHub License](https://img.shields.io/github/license/DuncanMcPherson/VectraLauncher)
![GitHub Release Date](https://img.shields.io/github/release-date/DuncanMcPherson/VectraLauncher)

## Overview
**VectraLauncher** is a simple launcher shim for the Vectra compiler. It allows you to easily manage multiple versions of the compiler on your system. With VectraLauncher, you can install, update, list, use, and uninstall different versions of the compiler with simple commands.

## Features
- Install specific versions of the Vectra compiler
- Update installed versions to the latest release
- List all available and installed versions
- Switch between different versions of the compiler
- Uninstall versions you no longer need

## Getting Started
### Installation
To install VectraLauncher, follow these steps:

1. Download the latest release from the [GitHub Releases](https://github.com/DuncanMcPherson/VectraLauncher/releases/latest)
2. Extract the downloaded archive to a directory of your choice
3. Run the self install command:
```bash
# Windows (PowerShell)
./vecc.exe self install

# Linux
./vecc self install
```
4. Follow the prompts to complete the installation

## Usage
Run the launcher from the command line:
```bash
vecc [command] [options]
```

### Commands
- **install \<version>**  
  Installs the specified version of the Vectra compiler.
- **update**  
  Installs the most recent version of the Vectra compiler.
- **list**  
  Lists all available and installed versions.
- **use \<version>**  
  Sets the specified version as the active compiler.
- **uninstall \<version>**  
  Removes the specified version from your system.
#### Examples
```bash
vecc install 1.2.3
vecc list
vecc update
vecc use 1.2.3
vecc uninstall 1.2.3
```
## Contributing
Contributions are welcome! If you have an idea for a new feature or have found a bug, please submit an issue or a pull request.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.